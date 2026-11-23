using System.Globalization;
using System.Text;
using Colossal.Mathematics;
using NetworkMultitool.Geometry;
using NetworkMultitool.Net;
using NetworkMultitool.Tool;
using Unity.Mathematics;
using static NetworkMultitool.Localization.Texts;

namespace NetworkMultitool.Modes
{
    /// <summary>Gives the selected chain a constant slope between its two end nodes (port of CS1 SetSlope).</summary>
    public sealed class SlopeMode : NodeChainMode
    {
        private string _slopes = string.Empty;

        public override ToolModeId Id => ToolModeId.Slope;

        public override string Info => _slopes.Length > 0 ? base.Info + "\n" + _slopes : base.Info;

        protected override void OnChainChanged(ToolContext ctx) => _slopes = Nodes.Count >= 2 ? Slopes(ctx) : string.Empty;

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
                NodeMover.SetCurve(em, edge, Vec.ConstantSlope(bezier));
            }

            foreach (var node in Nodes)
                NodeMover.MarkUpdated(em, node);
        }

        protected override void RenderPreview(ToolContext ctx, Overlay overlay)
        {
            // Like CS1 SlopeNode: each segment shows its current slope at its middle.
            var degrees = InDegrees(ctx);
            for (var i = 0; i < Nodes.Count - 1; i++)
            {
                var edge = ChainEdge(ctx, i);
                overlay.Edge(edge, Overlay.Yellow);
                overlay.Label(MathUtils.Position(ctx.Net.Curve(edge), 0.5f), float3.zero, SlopeText(ctx, i, degrees));
            }
        }

        private static bool InDegrees(ToolContext ctx) => ctx.Settings != null && ctx.Settings.SlopeInDegrees;

        // Current slope of each segment, in chain order.
        private string Slopes(ToolContext ctx)
        {
            var degrees = InDegrees(ctx);
            var text = new StringBuilder();
            for (var i = 0; i < Nodes.Count - 1; i++)
                text.Append('\n').Append(T("SlopeSegment", i + 1)).Append(": ").Append(SlopeText(ctx, i, degrees));
            return text.ToString();
        }

        /// <summary>Slope of chain segment i, from node i to node i + 1, in percent or degrees.</summary>
        private string SlopeText(ToolContext ctx, int i, bool degrees)
        {
            var net = ctx.Net;
            var length = MathUtils.Length(net.Curve(ChainEdge(ctx, i)));
            var slope = length > 0f ? (net.NodePosition(Nodes[i + 1]).y - net.NodePosition(Nodes[i]).y) / length : 0f;
            var value = degrees ? math.degrees(math.atan(slope)) : slope * 100f;
            return value.ToString("+0.0;-0.0", CultureInfo.InvariantCulture) + (degrees ? "°" : "%");
        }
    }
}
