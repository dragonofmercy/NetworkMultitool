using Colossal.Mathematics;
using Game.Tools;
using NetworkMultitool.Tool;
using Unity.Entities;
using Unity.Mathematics;

namespace NetworkMultitool.Modes
{
    /// <summary>Splits the hovered edge at the cursor, inserting a node.</summary>
    public sealed class AddNodeMode : IToolMode
    {
        private enum State
        {
            None,
            Possible,
            TooClose,
            Owned,
        }

        private State _state;
        private Entity _edge;
        private float3 _position;
        private float _width;

        public ToolModeId Id => ToolModeId.AddNode;

        public string Info
        {
            get
            {
                switch (_state)
                {
                    case State.Possible: return "Click to add a node";
                    case State.TooClose: return "Too close to an existing node";
                    case State.Owned: return "This segment can't be split";
                    default: return "Select a segment to add a node";
                }
            }
        }

        public void Enter(ToolContext ctx) => _state = State.None;

        public void Exit(ToolContext ctx) => _state = State.None;

        public void Update(ToolContext ctx)
        {
            _state = State.None;
            if (!ctx.HoverIsEdge || !ctx.Em.Exists(ctx.Hover))
                return;

            var net = ctx.Net;
            _edge = ctx.Hover;
            _width = net.Width(_edge);
            var t = ctx.Hit.m_CurvePosition;
            _position = MathUtils.Position(net.Curve(_edge), t);
            if (net.HasOwner(_edge))
            {
                _state = State.Owned;
                return;
            }

            var ends = net.Ends(_edge);
            var minDistance = math.min(_width / 2f, 8f) + _width;
            if (math.distance(_position.xz, net.NodePosition(ends.m_Start).xz) < minDistance || math.distance(_position.xz, net.NodePosition(ends.m_End).xz) < minDistance)
            {
                _state = State.TooClose;
                return;
            }

            _state = State.Possible;
            // The preview built from last frame's definitions becomes permanent; nothing is emitted this frame.
            if (ctx.ApplyPressed || ctx.Pressed(ToolAction.Apply))
            {
                ctx.ApplyMode = ApplyMode.Apply;
                return;
            }

            ctx.Emitter(net.Prefab(_edge)).Split(_edge, t, _position);
        }

        public void Render(ToolContext ctx, Overlay overlay)
        {
            if (_state == State.None)
                return;

            overlay.Edge(_edge, Overlay.White);
            overlay.Disc(_position, _width, _state == State.Possible ? Overlay.Green : Overlay.Red);
        }
    }
}
