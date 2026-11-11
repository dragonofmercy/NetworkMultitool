using System.Collections.Generic;
using System.Linq;
using Game.Net;
using Game.Tools;
using NetworkMultitool.Geometry;
using NetworkMultitool.Tool;
using Unity.Entities;
using Unity.Mathematics;

namespace NetworkMultitool.Modes
{
    /// <summary>Detaches some edges of a node and joins them at a new node placed at the cursor.</summary>
    public sealed class SplitNodeMode : IToolMode
    {
        private const float MaxDistance = 200f;

        private enum State
        {
            SelectSource,
            ClickSource,
            NotAllowed,
            SelectToSplit,
            ClickToOrder,
            ClickFromOrder,
            OrderIsFull,
            ClickToSplit,
            TooFar,
        }

        private readonly List<Entity> _selected = new List<Entity>();
        private Entity _source;
        private Entity _hover;
        private State _state;
        private float3 _target;
        private float _width;

        public ToolModeId Id => ToolModeId.SplitNode;

        public string Info
        {
            get
            {
                switch (_state)
                {
                    case State.ClickSource: return "Click to select source node";
                    case State.NotAllowed: return "This node can't be split";
                    case State.SelectToSplit: return "Select segments to split";
                    case State.ClickToOrder: return "Click to add segment to split order";
                    case State.ClickFromOrder: return "Click to remove segment from split order";
                    case State.OrderIsFull: return "Split order is full";
                    case State.ClickToSplit: return "Click to split node (hold Shift to keep height)";
                    case State.TooFar: return "Position is too far from source node";
                    default: return "Select source node";
                }
            }
        }

        public void Enter(ToolContext ctx) => Reset();

        public void Exit(ToolContext ctx) => Reset();

        public void Update(ToolContext ctx)
        {
            ctx.ApplyMode = ApplyMode.None;
            _hover = Entity.Null;
            var net = ctx.Net;
            if (_source != Entity.Null && (!ctx.Em.Exists(_source) || _selected.Any(e => !ctx.Em.Exists(e))))
                Reset();

            if (ctx.SecondaryPressed)
            {
                if (_selected.Count > 0)
                    _selected.Clear();
                else
                    _source = Entity.Null;
            }

            var hoverExists = ctx.Hover != Entity.Null && ctx.Em.Exists(ctx.Hover);
            if (_source == Entity.Null)
            {
                _state = State.SelectSource;
                if (!ctx.HoverIsNode || !hoverExists)
                    return;

                _hover = ctx.Hover;
                _state = net.Degree(_hover) >= 2 && !net.HasOwner(_hover) ? State.ClickSource : State.NotAllowed;
                if (_state == State.ClickSource && ctx.ApplyPressed)
                {
                    _source = _hover;
                    _hover = Entity.Null;
                    _state = State.SelectToSplit;
                }

                return;
            }

            _state = State.SelectToSplit;
            if (ctx.HoverIsEdge && hoverExists && net.Edges(_source).Contains(ctx.Hover))
            {
                _hover = ctx.Hover;
                if (_selected.Contains(_hover))
                {
                    _state = State.ClickFromOrder;
                    if (ctx.ApplyPressed)
                    {
                        _selected.Remove(_hover);
                        _state = State.ClickToOrder;
                    }
                }
                else if (_selected.Count >= net.Degree(_source) - 1)
                    _state = State.OrderIsFull;
                else
                {
                    _state = State.ClickToOrder;
                    if (ctx.ApplyPressed)
                    {
                        _selected.Add(_hover);
                        _state = State.ClickFromOrder;
                    }
                }

                return;
            }

            if (_selected.Count == 0 || ctx.HoverIsNode || !ctx.HasHit)
                return;

            var sourcePos = net.NodePosition(_source);
            _target = ctx.StepMultiplier == 10f ? Vec.WithY(ctx.MousePosition, sourcePos.y) : ctx.MousePosition;
            _width = _selected.Max(e => net.Width(e));
            if (math.distance(sourcePos.xz, _target.xz) > MaxDistance)
            {
                _state = State.TooFar;
                return;
            }

            _state = State.ClickToSplit;
            foreach (var edge in _selected)
            {
                var other = net.OtherNode(edge, _source);
                var otherPos = net.NodePosition(other);
                var dirOther = net.DirectionAt(edge, other);
                var dirTarget = Vec.NormalizeXZ(_target - otherPos);
                // Relink wants the curve oriented from the edge's start side to its end side.
                var curve = net.IsStart(edge, other) ? NetUtils.FitCurve(otherPos, dirOther, dirTarget, _target) : NetUtils.FitCurve(_target, -dirTarget, -dirOther, otherPos);
                ctx.Emitter(net.Prefab(edge)).Relink(edge, other, _target, curve);
            }

            ctx.ApplyMode = ApplyMode.Clear;
            if (ctx.ApplyPressed || ctx.Pressed(ToolAction.Apply))
            {
                ctx.ApplyMode = ApplyMode.Apply;
                Reset();
            }
        }

        public void Render(ToolContext ctx, Overlay overlay)
        {
            if (_source == Entity.Null)
            {
                if (_hover != Entity.Null)
                    overlay.Node(_hover, _state == State.ClickSource ? Overlay.Green : Overlay.Red);
                return;
            }

            overlay.Node(_source, Overlay.White);
            foreach (var edge in _selected)
                overlay.Edge(edge, Overlay.Yellow);

            if (_hover != Entity.Null)
                overlay.Edge(_hover, _state == State.ClickToOrder ? Overlay.Green : _state == State.ClickFromOrder ? Overlay.Yellow : Overlay.Red);
            else if (_state == State.ClickToSplit || _state == State.TooFar)
                overlay.Disc(_target, _width, _state == State.ClickToSplit ? Overlay.Green : Overlay.Red);
        }

        private void Reset()
        {
            _source = Entity.Null;
            _hover = Entity.Null;
            _selected.Clear();
            _state = State.SelectSource;
        }
    }
}
