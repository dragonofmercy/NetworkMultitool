using System.Collections.Generic;
using Game.Net;
using NetworkMultitool.Geometry;
using NetworkMultitool.Net;
using NetworkMultitool.Tool;
using Unity.Entities;
using Unity.Mathematics;

namespace NetworkMultitool.Modes
{
    /// <summary>Base of the modes working on an ordered chain of connected nodes (port of CS1 BaseNodeSet / BaseNodeLine).</summary>
    public abstract class NodeChainMode : IToolMode
    {
        private const int MaxSteps = 20;

        protected enum ChainState
        {
            None,
            CanAdd,
            IsEnd,
            NotConnected,
        }

        /// <summary>Selected nodes in chain order; consecutive nodes share an edge.</summary>
        protected readonly List<Entity> Nodes = new List<Entity>();

        /// <summary>Nodes the next click adds, in insertion order (traversed nodes then the hovered one).</summary>
        protected readonly List<Entity> ToAdd = new List<Entity>();

        /// <summary>True when <see cref="ToAdd"/> extends the chain from its first node.</summary>
        protected bool Prepend;

        protected ChainState State { get; private set; }

        public abstract ToolModeId Id { get; }

        public virtual string Info
        {
            get
            {
                switch (State)
                {
                    case ChainState.CanAdd: return "Click to add nodes";
                    case ChainState.IsEnd: return "Click to unselect";
                    case ChainState.NotConnected: return "Not connected";
                    default: return Nodes.Count >= 3 ? "Press Enter to apply" : "Select nodes in order";
                }
            }
        }

        public virtual void Enter(ToolContext ctx) => Clear(ctx);

        public virtual void Exit(ToolContext ctx) => Clear(ctx);

        public virtual void Update(ToolContext ctx)
        {
            if (!IsValid(ctx))
                Clear(ctx);

            UpdateState(ctx);

            if (ctx.SecondaryPressed)
            {
                Clear(ctx);
            }
            else if (ctx.ApplyPressed && State == ChainState.CanAdd)
            {
                if (Prepend)
                {
                    foreach (var node in ToAdd)
                        Nodes.Insert(0, node);
                }
                else
                {
                    Nodes.AddRange(ToAdd);
                }

                OnChainChanged(ctx);
            }
            else if (ctx.ApplyPressed && State == ChainState.IsEnd)
            {
                Nodes.RemoveAt(ctx.Hover == Nodes[0] ? 0 : Nodes.Count - 1);
                OnChainChanged(ctx);
            }
            else if (Nodes.Count >= 3 && ctx.Pressed(ToolAction.Apply))
            {
                OnApplyRequested(ctx);
            }
            else
            {
                return;
            }

            UpdateState(ctx);
        }

        public virtual void Render(ToolContext ctx, Overlay overlay)
        {
            foreach (var node in Nodes)
                overlay.Node(node, Overlay.White);

            switch (State)
            {
                case ChainState.CanAdd:
                    foreach (var node in ToAdd)
                        overlay.Node(node, Overlay.Green);
                    break;
                case ChainState.IsEnd:
                    overlay.Node(ctx.Hover, Overlay.Yellow);
                    break;
                case ChainState.NotConnected:
                    overlay.Node(ctx.Hover, Overlay.Red);
                    break;
            }

            RenderPreview(ctx, overlay);
        }

        /// <summary>Validation key pressed with at least 3 nodes in the chain. Default: <see cref="Apply"/> then clear the chain.</summary>
        protected virtual void OnApplyRequested(ToolContext ctx)
        {
            Apply(ctx);
            Clear(ctx);
        }

        /// <summary>Called after <see cref="Nodes"/> changed through a click or a reset.</summary>
        protected abstract void OnChainChanged(ToolContext ctx);

        /// <summary>Writes the result on the network; the chain holds at least 3 valid nodes. The chain is cleared afterwards.</summary>
        protected abstract void Apply(ToolContext ctx);

        protected abstract void RenderPreview(ToolContext ctx, Overlay overlay);

        /// <summary>The edge between Nodes[i] and Nodes[i + 1]; the chain is valid during Update, Render and Apply.</summary>
        protected Entity ChainEdge(ToolContext ctx, int i)
        {
            ctx.Net.TryCommonEdge(Nodes[i], Nodes[i + 1], out var edge);
            return edge;
        }

        /// <summary>Rewrites every chain edge as <c>NetUtils.FitCurve</c> between consecutive points (one per chain node, forward direction in chain order) with a constant slope.</summary>
        protected void RebuildChainEdges(ToolContext ctx, IReadOnlyList<Point> points)
        {
            var net = ctx.Net;
            for (var i = 0; i < Nodes.Count - 1; i++)
            {
                var edge = ChainEdge(ctx, i);
                var p0 = points[i];
                var p1 = points[i + 1];
                // The curve must start at the edge's m_Start node.
                if (!net.IsStart(edge, Nodes[i]))
                {
                    p0 = new Point(points[i + 1].Position, -points[i + 1].Direction);
                    p1 = new Point(points[i].Position, -points[i].Direction);
                }

                var bezier = NetUtils.FitCurve(p0.Position, p0.Direction, p1.Direction, p1.Position);
                // Flat tangents keep b.y = a.y and c.y = d.y: restore a constant slope, as CourseEmitter does.
                bezier.b.y = math.lerp(bezier.a.y, bezier.d.y, 1f / 3f);
                bezier.c.y = math.lerp(bezier.a.y, bezier.d.y, 2f / 3f);
                NodeMover.SetCurve(ctx.Em, edge, bezier);
            }
        }

        private void UpdateState(ToolContext ctx)
        {
            State = ChainState.None;
            ToAdd.Clear();
            Prepend = false;
            if (!ctx.HoverIsNode || !ctx.Em.Exists(ctx.Hover))
                return;

            var hover = ctx.Hover;
            if (Nodes.Count > 0 && (hover == Nodes[0] || hover == Nodes[Nodes.Count - 1]))
            {
                State = ChainState.IsEnd;
                return;
            }

            State = NodeChain.TryExtend(ctx.Net, Nodes, hover, MaxSteps, ToAdd, out Prepend) && !ToAdd.Exists(ctx.Net.HasOwner) ? ChainState.CanAdd : ChainState.NotConnected;
            if (State == ChainState.NotConnected)
                ToAdd.Clear();
        }

        // Nodes or edges may vanish or be replaced between frames (undo, other tools, our own apply).
        protected bool IsValid(ToolContext ctx)
        {
            for (var i = 0; i < Nodes.Count; i++)
            {
                if (!ctx.Em.Exists(Nodes[i]))
                    return false;
                if (i > 0 && !ctx.Net.TryCommonEdge(Nodes[i - 1], Nodes[i], out _))
                    return false;
            }

            return true;
        }

        protected void Clear(ToolContext ctx)
        {
            Nodes.Clear();
            ToAdd.Clear();
            Prepend = false;
            State = ChainState.None;
            OnChainChanged(ctx);
        }
    }
}
