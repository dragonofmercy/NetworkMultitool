using System.Linq;
using Game.Tools;
using NetworkMultitool.Geometry;
using NetworkMultitool.Tool;
using Unity.Entities;

namespace NetworkMultitool.Modes
{
    /// <summary>Removes the hovered degree-2 node, merging its two edges into one.</summary>
    public sealed class RemoveNodeMode : IToolMode
    {
        private Entity _node;
        private bool _correct;

        public ToolModeId Id => ToolModeId.RemoveNode;

        public string Info => _node == Entity.Null ? "Select a node to remove" : _correct ? "Click to remove this node" : "This node can't be removed";

        public void Enter(ToolContext ctx) => _node = Entity.Null;

        public void Exit(ToolContext ctx) => _node = Entity.Null;

        public void Update(ToolContext ctx)
        {
            _node = Entity.Null;
            _correct = false;
            ctx.SnapHoverToNode();
            if (!ctx.HoverIsNode || !ctx.Em.Exists(ctx.Hover))
                return;

            var net = ctx.Net;
            _node = ctx.Hover;
            var edges = net.Edges(_node).ToList();
            _correct = edges.Count == 2 && !net.HasOwner(_node) && !HasSubObjects(ctx, _node) && net.Prefab(edges[0]) == net.Prefab(edges[1]) && net.OtherNode(edges[0], _node) != net.OtherNode(edges[1], _node);
            if (!_correct)
                return;

            // The preview built from last frame's definitions becomes permanent; nothing is emitted this frame.
            if (ctx.ApplyPressed || ctx.Pressed(ToolAction.Apply))
            {
                ctx.ApplyMode = ApplyMode.Apply;
                return;
            }

            var edgeA = edges[0];
            var edgeB = edges[1];
            var a = net.OtherNode(edgeA, _node);
            var b = net.OtherNode(edgeB, _node);
            var points = new[] { new Point(net.NodePosition(a), net.DirectionAt(edgeA, a)), new Point(net.NodePosition(b), -net.DirectionAt(edgeB, b)) };

            var emitter = ctx.Emitter(net.Prefab(edgeA));
            emitter.Delete(edgeA);
            emitter.Delete(edgeB);
            emitter.Chain(a, b, points, net.Elevation(a), net.Elevation(b), false, null, edgeA, !net.IsStart(edgeA, a));
        }

        private static bool HasSubObjects(ToolContext ctx, Entity node) => ctx.Em.HasBuffer<Game.Objects.SubObject>(node) && ctx.Em.GetBuffer<Game.Objects.SubObject>(node, true).Length > 0;

        public void Render(ToolContext ctx, Overlay overlay)
        {
            if (_node != Entity.Null)
                overlay.Node(_node, _correct ? Overlay.Green : Overlay.Red);
        }
    }
}
