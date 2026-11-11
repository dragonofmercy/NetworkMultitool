using System.Collections.Generic;
using NetworkMultitool.Geometry;
using NetworkMultitool.Tool;

namespace NetworkMultitool.Modes
{
    /// <summary>Connects two segments with a smooth curve (port of CS1 CreateCurveMode).</summary>
    public sealed class CurveMode : CreateModeBase
    {
        public override ToolModeId Id => ToolModeId.Curve;

        protected override SolveResult Solve(ToolContext ctx, List<Point> points) => CurveSolver.Solve(StartPoint, EndPoint, ctx.MaxSegmentLength, points);
    }
}
