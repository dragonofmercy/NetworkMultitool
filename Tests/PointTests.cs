using NetworkMultitool.Geometry;
using Unity.Mathematics;
using Xunit;

namespace NetworkMultitool.Tests
{
    public class PointTests
    {
        private static void AssertNear(float3 expected, float3 actual, float tol = 1e-5f)
        {
            Assert.True(math.distance(expected, actual) < tol, $"expected {expected} but got {actual}");
        }

        [Fact]
        public void Shifted_PositiveGoesRight()
        {
            var shifted = new Point(new float3(10, 5, 20), new float3(0, 0, 1)).Shifted(3f);
            AssertNear(new float3(13, 5, 20), shifted.Position);
            AssertNear(new float3(0, 0, 1), shifted.Direction);
        }

        [Fact]
        public void Shifted_NegativeGoesLeft()
        {
            var shifted = new Point(new float3(10, 5, 20), new float3(0, 0, 1)).Shifted(-2f);
            AssertNear(new float3(8, 5, 20), shifted.Position);
        }

        [Fact]
        public void Shifted_KeepsInDirection()
        {
            var point = new Point(new float3(10, 5, 20), new float3(0, 0, 1), new float3(1, 0, 0));
            var shifted = point.Shifted(3f);
            AssertNear(new float3(1, 0, 0), shifted.InDirection);
            AssertNear(new float3(0, 0, 1), shifted.Direction);
        }
    }
}
