using NetworkMultitool.Geometry;
using Unity.Mathematics;
using Xunit;

namespace NetworkMultitool.Tests
{
    public class TrajectoryTests
    {
        [Fact]
        public void Bezier_Travel_BeyondEnd_ClampsToOne()
        {
            var t = new BezierTrajectory(new float3(0, 0, 0), new float3(1, 0, 0), new float3(100, 0, 100), new float3(0, 0, 1));
            Assert.Equal(1f, t.Travel(0.9f, 1000f));
        }

        [Fact]
        public void Bezier_FromDirections_StartsAndEndsAtPoints()
        {
            var start = new float3(0, 0, 0);
            var end = new float3(100, 0, 100);
            var t = new BezierTrajectory(start, new float3(1, 0, 0), end, new float3(0, 0, 1));
            Assert.True(math.distance(start, t.Position(0f)) < 1e-4f);
            Assert.True(math.distance(end, t.Position(1f)) < 1e-4f);
            Assert.Equal(1f, math.length(t.Tangent(0f)), 4);
            var tan = t.Tangent(0f);
            Assert.True(math.distance(tan, new float3(1, 0, 0)) < 1e-4f);
        }
    }
}
