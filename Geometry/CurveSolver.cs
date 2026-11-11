using System.Collections.Generic;
using Unity.Mathematics;

namespace NetworkMultitool.Geometry
{
    /// <summary>Single Bezier between two attach nodes; port of CS1 CreateCurveMode.Init/Calculate.</summary>
    public static class CurveSolver
    {
        /// <summary>Directions point into the connection from each attach node. Fills points with start, inner points and end.</summary>
        public static SolveResult Solve(Point start, Point end, float maxSegmentLength, List<Point> points)
        {
            points.Clear();
            var connectDir = Vec.NormalizeXZ(end.Position - start.Position);
            if (Vec.DotXZ(Vec.NormalizeXZ(start.Direction), connectDir) < -0.7f || Vec.DotXZ(Vec.NormalizeXZ(end.Direction), -connectDir) < -0.7f)
                return SolveResult.WrongShape;

            var height = (start.Position.y + end.Position.y) / 2f;
            var startPos = Vec.WithY(start.Position, height);
            var endPos = Vec.WithY(end.Position, height);
            var bezier = new BezierTrajectory(startPos, start.Direction, endPos, -end.Direction);

            points.Add(new Point(startPos, start.Direction));
            var count = (int)math.ceil(bezier.Length / maxSegmentLength);
            var t = 0f;
            for (var i = 1; i < count; i++)
            {
                t = bezier.Travel(t, bezier.Length / count);
                points.Add(new Point(bezier.Position(t), Vec.NormalizeXZ(bezier.Tangent(t))));
            }
            points.Add(new Point(endPos, -end.Direction));
            return SolveResult.Ok;
        }
    }
}
