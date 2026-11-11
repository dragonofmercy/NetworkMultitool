using NetworkMultitool.Geometry;
using Unity.Mathematics;
using Xunit;

namespace NetworkMultitool.Tests
{
    public class TrajectoryTests
    {
        [Fact]
        public void Straight_Length_IsDistance()
        {
            Assert.Equal(50f, new StraightTrajectory(new float3(0, 0, 0), new float3(30, 0, 40)).Length, 4);
        }

        [Fact]
        public void Straight_Travel_HalfLength_IsHalfT()
        {
            Assert.Equal(0.5f, new StraightTrajectory(new float3(0, 0, 0), new float3(30, 0, 40)).Travel(0f, 25f), 4);
        }

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
            var tan = math.normalize(t.Tangent(0f));
            Assert.True(math.distance(tan, new float3(1, 0, 0)) < 1e-4f);
        }
    }
}
