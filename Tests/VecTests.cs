using System;
using NetworkMultitool.Geometry;
using Unity.Mathematics;
using Xunit;

namespace NetworkMultitool.Tests
{
    public class VecTests
    {
        private static void AssertNear(float3 expected, float3 actual, float tol = 1e-5f)
        {
            Assert.True(math.distance(expected, actual) < tol, $"expected {expected} but got {actual}");
        }

        [Fact]
        public void Turn90_CounterClockwise_RotatesXToZ()
        {
            AssertNear(new float3(0, 0, 1), Vec.Turn90(new float3(1, 0, 0), false));
        }

        [Fact]
        public void Angle_QuarterTurn_IsHalfPi()
        {
            Assert.Equal(math.PI / 2, Vec.Angle(new float3(1, 0, 0), new float3(0, 0, 1)), 4);
        }

        [Fact]
        public void IntersectXZ_Crossing_ReturnsParams()
        {
            var hit = Vec.IntersectXZ(new float3(0, 0, 0), new float3(1, 0, 0), new float3(10, 0, -5), new float3(0, 0, 1), out var t1, out var t2);
            Assert.True(hit);
            Assert.Equal(10f, t1, 4);
            Assert.Equal(5f, t2, 4);
        }

        [Fact]
        public void IntersectXZ_Parallel_ReturnsFalse()
        {
            Assert.False(Vec.IntersectXZ(new float3(0, 0, 0), new float3(1, 0, 0), new float3(0, 0, 5), new float3(2, 0, 0), out _, out _));
        }
    }
}
