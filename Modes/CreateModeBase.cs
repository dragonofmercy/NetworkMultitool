using System.Collections.Generic;
using Game.Tools;
using NetworkMultitool.Geometry;
using NetworkMultitool.Tool;
using Unity.Entities;
using Unity.Mathematics;

namespace NetworkMultitool.Modes
{
    /// <summary>Two-segment creation flow (port of CS1 BaseCreateMode): select two edges, pick their ends, preview and create the solved path.</summary>
    public abstract class CreateModeBase : IToolMode
    {
        private readonly List<Point> _emitted = new List<Point>();
        private bool _dirty = true;
        private bool _hoverEnd;

        protected Entity First;
        protected Entity Second;
        protected Entity FirstNode;
        protected Entity SecondNode;
        protected Point StartPoint;
        protected Point EndPoint;
        protected readonly List<Point> Points = new List<Point>();
        protected SolveResult Result;
        protected float Height;
        protected bool Invert;

        public abstract ToolModeId Id { get; }

        public string Info
        {
            get
            {
                if (First == Entity.Null)
                    return "Select first segment";
                if (Second == Entity.Null)
                    return "Select second segment";
                if (_hoverEnd)
                    return "Click a node to change direction";
                return $"{Status(Result)}\nPageUp/PageDown: height ({Height:0.0} m)\nCtrl+I: invert{ExtraInfo}";
            }
        }

        /// <summary>Appended to the status text once both segments are selected; start it with a newline.</summary>
        protected virtual string ExtraInfo => string.Empty;

        /// <summary>Fills <paramref name="points"/> (already cleared) from <see cref="StartPoint"/> to <see cref="EndPoint"/>.</summary>
        protected abstract SolveResult Solve(ToolContext ctx, List<Point> points);

        /// <summary>Mode-specific keys, handled every frame after the base keys. Call <see cref="MarkDirty"/> when an input changes.</summary>
        protected virtual void OnKeys(ToolContext ctx)
        {
        }

        /// <summary>Extra overlay drawn after the base selection and path.</summary>
        protected virtual void RenderShape(ToolContext ctx, Overlay overlay)
        {
        }

        /// <summary>Called at the end of every full reset (Enter, Exit, Apply, right click deselecting First, entity loss) to clear mode-specific state.</summary>
        protected virtual void OnReset()
        {
        }

        /// <summary>Called on a left click before the base selection logic; return true when the mode handled the click itself.</summary>
        protected virtual bool ConsumeClick(ToolContext ctx) => false;

        protected void MarkDirty() => _dirty = true;

        public virtual void Enter(ToolContext ctx) => Reset();

        public virtual void Exit(ToolContext ctx) => Reset();

        public void Update(ToolContext ctx)
        {
            var em = ctx.Em;
            if ((First != Entity.Null && !em.Exists(First)) || (Second != Entity.Null && !em.Exists(Second)) || (FirstNode != Entity.Null && !em.Exists(FirstNode)) || (SecondNode != Entity.Null && !em.Exists(SecondNode)))
                Reset();

            HandleSelection(ctx);
            HandleKeys(ctx);
            if (Second == Entity.Null)
                return;

            var net = ctx.Net;
            if (_dirty)
            {
                _dirty = false;
                StartPoint = new Point(net.NodePosition(FirstNode), -net.DirectionAt(First, FirstNode));
                EndPoint = new Point(net.NodePosition(SecondNode), -net.DirectionAt(Second, SecondNode));
                Points.Clear();
                // Both chosen ends on the same node would make a degenerate zero-length course.
                Result = FirstNode == SecondNode ? SolveResult.WrongShape : Solve(ctx, Points);
                ApplyHeights();
            }

            if (Result != SolveResult.Ok)
                return;

            // The preview built from last frame's definitions becomes permanent; nothing is emitted this frame.
            if (ctx.Pressed(ToolAction.Apply))
            {
                ctx.ApplyMode = ApplyMode.Apply;
                Reset();
                return;
            }

            var startNode = FirstNode;
            var endNode = SecondNode;
            _emitted.Clear();
            if (Invert)
            {
                startNode = SecondNode;
                endNode = FirstNode;
                for (var i = Points.Count - 1; i >= 0; i--)
                    _emitted.Add(new Point(Points[i].Position, -Points[i].Direction));
            }
            else
            {
                _emitted.AddRange(Points);
            }

            // Ground-level networks may follow the terrain; anything elevated, lowered or offset keeps the solved heights.
            var startElevation = net.Elevation(startNode);
            var endElevation = net.Elevation(endNode);
            var followTerrain = ctx.Settings != null && ctx.Settings.FollowTerrain && Height == 0f && math.all(math.abs(startElevation) < 1f) && math.all(math.abs(endElevation) < 1f);
            ctx.Emitter(net.Prefab(First)).Chain(startNode, endNode, _emitted, startElevation, endElevation, followTerrain, followTerrain ? null : (System.Func<float3, float>)ctx.TerrainHeight);
        }

        public void Render(ToolContext ctx, Overlay overlay)
        {
            if (First != Entity.Null)
                overlay.Edge(First, Overlay.Blue);
            if (Second != Entity.Null)
                overlay.Edge(Second, Overlay.Blue);
            if (IsSelectable(ctx, ctx.Hover))
                overlay.Edge(ctx.Hover, Overlay.White);

            if (Second != Entity.Null)
            {
                var net = ctx.Net;
                var a = net.Ends(First);
                var b = net.Ends(Second);
                overlay.Node(a.m_Start, a.m_Start == FirstNode ? Overlay.Yellow : Overlay.White);
                overlay.Node(a.m_End, a.m_End == FirstNode ? Overlay.Yellow : Overlay.White);
                overlay.Node(b.m_Start, b.m_Start == SecondNode ? Overlay.Yellow : Overlay.White);
                overlay.Node(b.m_End, b.m_End == SecondNode ? Overlay.Yellow : Overlay.White);
            }

            if (Points.Count >= 2)
                overlay.Path(Points, ctx.Net.Width(First), Result == SolveResult.Ok ? Overlay.White : Overlay.Red);

            RenderShape(ctx, overlay);
        }

        /// <summary>The solvers work at a flat mean height: spread the height difference of the two nodes along the path, plus the user offset on inner points.</summary>
        private void ApplyHeights()
        {
            var last = Points.Count - 1;
            if (last < 1)
                return;
            var total = 0f;
            for (var i = 0; i < last; i++)
                total += math.distance(Vec.Flat(Points[i].Position), Vec.Flat(Points[i + 1].Position));

            var run = 0f;
            for (var i = 0; i <= last; i++)
            {
                if (i > 0)
                    run += math.distance(Vec.Flat(Points[i - 1].Position), Vec.Flat(Points[i].Position));
                var y = math.lerp(StartPoint.Position.y, EndPoint.Position.y, total > 0f ? run / total : 0f) + (i == 0 || i == last ? 0f : Height);
                Points[i] = new Point(Vec.WithY(Points[i].Position, y), Points[i].Direction);
            }
        }

        private void HandleSelection(ToolContext ctx)
        {
            var net = ctx.Net;
            if (Second != Entity.Null)
                ctx.SnapHoverToNode();
            _hoverEnd = Second != Entity.Null && ctx.HoverIsNode && ctx.Em.Exists(ctx.Hover) && (IsEnd(ctx, First, ctx.Hover) || IsEnd(ctx, Second, ctx.Hover));

            if (ctx.SecondaryPressed)
            {
                if (Second != Entity.Null)
                {
                    Second = Entity.Null;
                    SecondNode = Entity.Null;
                    Points.Clear();
                    _dirty = true;
                }
                else if (First != Entity.Null)
                {
                    Reset();
                }

                return;
            }

            if (!ctx.ApplyPressed || ConsumeClick(ctx))
                return;

            if (IsSelectable(ctx, ctx.Hover))
            {
                if (First == Entity.Null)
                {
                    First = ctx.Hover;
                    FirstNode = SmallerEnd(ctx, First);
                }
                else
                {
                    Second = ctx.Hover;
                    SecondNode = SmallerEnd(ctx, Second);
                    if (SecondNode == FirstNode)
                        SecondNode = ctx.Net.OtherNode(Second, SecondNode);
                }

                _dirty = true;
            }
            else if (_hoverEnd)
            {
                if (IsEnd(ctx, First, ctx.Hover))
                    FirstNode = ctx.Hover;
                else
                    SecondNode = ctx.Hover;
                _dirty = true;
            }
        }

        private void HandleKeys(ToolContext ctx)
        {
            if (ctx.Pressed(ToolAction.HeightUp))
            {
                Height += ctx.StepMultiplier;
                _dirty = true;
            }

            if (ctx.Pressed(ToolAction.HeightDown))
            {
                Height -= ctx.StepMultiplier;
                _dirty = true;
            }

            if (ctx.Pressed(ToolAction.Invert))
            {
                Invert = !Invert;
                _dirty = true;
            }

            OnKeys(ctx);
        }

        /// <summary>An ownerless edge that can become First or Second.</summary>
        private bool IsSelectable(ToolContext ctx, Entity edge) => Second == Entity.Null && ctx.HoverIsEdge && edge == ctx.Hover && edge != First && ctx.Em.Exists(edge) && !ctx.Net.HasOwner(edge);

        private static bool IsEnd(ToolContext ctx, Entity edge, Entity node)
        {
            var ends = ctx.Net.Ends(edge);
            return ends.m_Start == node || ends.m_End == node;
        }

        /// <summary>CS1 GetIsStart: the end with fewer segments, the start on ties.</summary>
        private static Entity SmallerEnd(ToolContext ctx, Entity edge)
        {
            var ends = ctx.Net.Ends(edge);
            return ctx.Net.Degree(ends.m_Start) <= ctx.Net.Degree(ends.m_End) ? ends.m_Start : ends.m_End;
        }

        private static string Status(SolveResult result)
        {
            switch (result)
            {
                case SolveResult.WrongShape: return "Wrong shape";
                case SolveResult.NotIntersect: return "No intersection";
                case SolveResult.BigRadius: return "Radius too big";
                case SolveResult.SmallRadius: return "Radius too small";
                default: return "Press Enter to create";
            }
        }

        private void Reset()
        {
            First = Entity.Null;
            Second = Entity.Null;
            FirstNode = Entity.Null;
            SecondNode = Entity.Null;
            Points.Clear();
            Result = SolveResult.Ok;
            Height = 0f;
            Invert = false;
            _hoverEnd = false;
            _dirty = true;
            OnReset();
        }
    }
}
