using System.Globalization;
using System.Text;
using Colossal.Mathematics;
using NetworkMultitool.Geometry;
using NetworkMultitool.Net;
using NetworkMultitool.Tool;
using Unity.Mathematics;

namespace NetworkMultitool.Modes
{
    /// <summary>Gives the selected chain a constant slope between its two end nodes (port of CS1 SetSlope).</summary>
    public sealed class SlopeMode : NodeChainMode
    {
        private string _slopes = string.Empty;

        public override ToolModeId Id => ToolModeId.Slope;

        public override string Info => base.Info + _slopes;

        public override void Update(ToolContext ctx)
        {
            base.Update(ctx);
            _slopes = Nodes.Count >= 2 ? Slopes(ctx) : string.Empty;
        }

        protected override void OnChainChanged(ToolContext ctx)
        {
        }

        protected override void Apply(ToolContext ctx)
        {
            var net = ctx.Net;
            var em = ctx.Em;
            var count = Nodes.Count;
            var lengths = new float[count - 1];
            for (var i = 0; i < count - 1; i++)
                lengths[i] = MathUtils.Length(net.Curve(ChainEdge(ctx, i)));

            var heights = SlopeSolver.Heights(net.NodePosition(Nodes[0]).y, net.NodePosition(Nodes[count - 1]).y, lengths);
            for (var i = 1; i < count - 1; i++)
                NodeMover.MoveNode(em, Nodes[i], Vec.WithY(net.NodePosition(Nodes[i]), heights[i]), net.NodeRotation(Nodes[i]));

            for (var i = 0; i < count - 1; i++)
            {
                var edge = ChainEdge(ctx, i);
                var bezier = net.Curve(edge);
                var forward = net.IsStart(edge, Nodes[i]);
                bezier.a.y = forward ? heights[i] : heights[i + 1];
                bezier.d.y = forward ? heights[i + 1] : heights[i];
                bezier.b.y = bezier.a.y + (bezier.d.y - bezier.a.y) / 3f;
                bezier.c.y = bezier.a.y + 2f * (bezier.d.y - bezier.a.y) / 3f;
                NodeMover.SetCurve(em, edge, bezier);
            }

            foreach (var node in Nodes)
                NodeMover.MarkUpdated(em, node);
        }

        protected override void RenderPreview(ToolContext ctx, Overlay overlay)
        {
            for (var i = 0; i < Nodes.Count - 1; i++)
                overlay.Edge(ChainEdge(ctx, i), Overlay.Yellow);
        }

        // Current slope of each segment, in chain order.
        private string Slopes(ToolContext ctx)
        {
            var net = ctx.Net;
            var degrees = ctx.Settings != null && ctx.Settings.SlopeInDegrees;
            var text = new StringBuilder();
            for (var i = 0; i < Nodes.Count - 1; i++)
            {
                var length = MathUtils.Length(net.Curve(ChainEdge(ctx, i)));
                var slope = length > 0f ? (net.NodePosition(Nodes[i + 1]).y - net.NodePosition(Nodes[i]).y) / length : 0f;
                var value = degrees ? math.degrees(math.atan(slope)) : slope * 100f;
                text.Append('\n').Append("Segment ").Append(i + 1).Append(": ").Append(value.ToString("+0.0;-0.0", CultureInfo.InvariantCulture)).Append(degrees ? " deg" : "%");
            }

            return text.ToString();
        }
    }
}
