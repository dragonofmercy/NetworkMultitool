using Game.Net;
using NetworkMultitool.Geometry;
using NetworkMultitool.Net;
using NetworkMultitool.Tool;
using Unity.Entities;
using Unity.Mathematics;
using static NetworkMultitool.Localization.Texts;

namespace NetworkMultitool.Modes
{
    /// <summary>Redistributes the selected chain evenly along a curve guided by the edges at its ends (port of CS1 ArrangeLine).</summary>
    public sealed class ArrangeLineMode : NodeChainMode
    {
        private Entity _firstGuide;
        private Entity _lastGuide;
        private Entity _hoverGuide;
        private bool _hoverGuideIsFirst;
        private Point[] _points;

        public override ToolModeId Id => ToolModeId.ArrangeLine;

        public override string Info => _hoverGuide != Entity.Null ? base.Info + "\n" + T("ArrangeGuide") : base.Info;

        public override void Update(ToolContext ctx)
        {
            base.Update(ctx);
            if ((_firstGuide != Entity.Null && !ctx.Em.Exists(_firstGuide)) || (_lastGuide != Entity.Null && !ctx.Em.Exists(_lastGuide)))
                OnChainChanged(ctx);

            _hoverGuide = HoverGuide(ctx);
            if (_hoverGuide == Entity.Null || !ctx.ApplyPressed)
                return;

            if (_hoverGuideIsFirst)
                _firstGuide = _hoverGuide;
            else
                _lastGuide = _hoverGuide;
            Arrange(ctx);
        }

        protected override void OnChainChanged(ToolContext ctx)
        {
            _firstGuide = Guide(ctx, true);
            _lastGuide = Guide(ctx, false);
            Arrange(ctx);
        }

        protected override void Apply(ToolContext ctx)
        {
            var em = ctx.Em;
            for (var i = 1; i < Nodes.Count - 1; i++)
                NodeMover.MoveNode(em, Nodes[i], _points[i].Position, NetUtils.GetNodeRotation(_points[i].Direction));

            RebuildChainEdges(ctx, _points);

            foreach (var node in Nodes)
                NodeMover.MarkUpdated(em, node);
        }

        protected override void RenderPreview(ToolContext ctx, Overlay overlay)
        {
            if (_hoverGuide != Entity.Null)
                overlay.Edge(_hoverGuide, Overlay.Purple);
            if (_points != null)
                overlay.Path(_points, ctx.Net.Width(ChainEdge(ctx, 0)), Overlay.Yellow);
        }

        private void Arrange(ToolContext ctx)
        {
            _points = null;
            if (Nodes.Count < 3)
                return;

            var net = ctx.Net;
            var positions = new float3[Nodes.Count];
            for (var i = 0; i < Nodes.Count; i++)
                positions[i] = net.NodePosition(Nodes[i]);

            // Without an outside guide, an end keeps the direction of its own chain edge: the chain becomes one smooth curve between its two end segments.
            var last = Nodes.Count - 1;
            var startDir = _firstGuide == Entity.Null ? net.DirectionAt(ChainEdge(ctx, 0), Nodes[0]) : -net.DirectionAt(_firstGuide, Nodes[0]);
            var endDir = _lastGuide == Entity.Null ? net.DirectionAt(ChainEdge(ctx, last - 1), Nodes[last]) : -net.DirectionAt(_lastGuide, Nodes[last]);
            _points = LineArranger.Arrange(positions, startDir, endDir);
        }

        // CS1 GetGuide: the end node's edge most opposite to the chain edge, none when the node has no other edge.
        private Entity Guide(ToolContext ctx, bool first)
        {
            if (Nodes.Count < 2)
                return Entity.Null;

            var net = ctx.Net;
            var node = first ? Nodes[0] : Nodes[Nodes.Count - 1];
            var next = first ? Nodes[1] : Nodes[Nodes.Count - 2];
            if (net.Degree(node) <= 1 || !net.TryCommonEdge(node, next, out var common))
                return Entity.Null;

            var commonDir = net.DirectionAt(common, node);
            var result = Entity.Null;
            var resultDot = float.MaxValue;
            foreach (var edge in net.Edges(node))
            {
                if (edge == common)
                    continue;

                var dot = Vec.DotXZ(net.DirectionAt(edge, node), commonDir);
                if (dot < resultDot)
                {
                    resultDot = dot;
                    result = edge;
                }
            }

            return result;
        }

        // An edge outside the chain touching one of its ends.
        private Entity HoverGuide(ToolContext ctx)
        {
            _hoverGuideIsFirst = false;
            if (!ctx.HoverIsEdge || Nodes.Count < 2 || !ctx.Em.Exists(ctx.Hover))
                return Entity.Null;

            var net = ctx.Net;
            var first = Nodes[0];
            var last = Nodes[Nodes.Count - 1];
            var touchesFirst = net.IsEnd(ctx.Hover, first) && net.OtherNode(ctx.Hover, first) != Nodes[1];
            var touchesLast = net.IsEnd(ctx.Hover, last) && net.OtherNode(ctx.Hover, last) != Nodes[Nodes.Count - 2];
            _hoverGuideIsFirst = touchesFirst;
            return touchesFirst || touchesLast ? ctx.Hover : Entity.Null;
        }
    }
}
