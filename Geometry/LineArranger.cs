using Unity.Mathematics;

namespace NetworkMultitool.Geometry
{
    public static class LineArranger
    {
        /// <summary>Redistributes the chain at equal arc length along a Bezier between its ends. startDir/endDir point from the end node into the chain; zero = free end.</summary>
        public static Point[] Arrange(float3[] positions, float3 startDir, float3 endDir)
        {
            var count = positions.Length;
            var start = positions[0];
            var end = positions[count - 1];
            var trajectory = new BezierTrajectory(start, Vec.NormalizeXZ(startDir), end, -Vec.NormalizeXZ(endDir));

            var result = new Point[count];
            var step = trajectory.Length / (count - 1);
            var t = 0f;
            for (var i = 0; i < count; i += 1)
            {
                if (i == count - 1)
                {
                    t = 1f;
                }
                result[i] = new Point(i == 0 ? start : i == count - 1 ? end : trajectory.Position(t), Vec.NormalizeXZ(trajectory.Tangent(t)));
                t = trajectory.Travel(t, step);
            }
            return result;
        }
    }
}
