using System.Collections.Generic;
using Colossal.Mathematics;
using NetworkMultitool.Geometry;
using Unity.Mathematics;
using Xunit;

namespace NetworkMultitool.Tests
{
    public class ParallelSolverTests
    {
        private static Bezier4x3 Line(float3 a, float3 d) => new Bezier4x3(a, math.lerp(a, d, 1f / 3f), math.lerp(a, d, 2f / 3f), d);

        private static void AssertNear(float3 expected, float3 actual)
        {
            Assert.True(math.distance(expected, actual) < 1e-3f, $"expected {expected} but got {actual}");
        }

        private static List<Point> Solve(float shift, bool left, float height, params Bezier4x3[] edges) => ParallelSolver.Solve(edges, shift, left, height);

        [Fact]
        public void SingleEdge_LeftShift_OffsetsTowardsPlusZ()
        {
            var points = Solve(10f, true, 0f, Line(new float3(0, 0, 0), new float3(100, 0, 0)));
            Assert.Equal(2, points.Count);
            AssertNear(new float3(0, 0, 10), points[0].Position);
            AssertNear(new float3(100, 0, 10), points[1].Position);
            AssertNear(new float3(1, 0, 0), points[0].Direction);
            AssertNear(new float3(1, 0, 0), points[1].Direction);
        }

        [Fact]
        public void SingleEdge_RightShift_OffsetsTowardsMinusZ()
        {
            var points = Solve(10f, false, 0f, Line(new float3(0, 0, 0), new float3(100, 0, 0)));
            Assert.Equal(2, points.Count);
            AssertNear(new float3(0, 0, -10), points[0].Position);
            AssertNear(new float3(100, 0, -10), points[1].Position);
        }

        [Fact]
        public void Collinear_MergesInnerPoint()
        {
            var points = Solve(10f, true, 0f, Line(new float3(0, 0, 0), new float3(100, 0, 0)), Line(new float3(100, 0, 0), new float3(200, 0, 0)));
            Assert.Equal(3, points.Count);
            AssertNear(new float3(100, 0, 10), points[1].Position);
        }

        [Fact]
        public void RightAngle_InnerCorner()
        {
            var points = Solve(10f, true, 0f, Line(new float3(0, 0, 0), new float3(100, 0, 0)), Line(new float3(100, 0, 0), new float3(100, 0, 100)));
            Assert.Equal(3, points.Count);
            AssertNear(new float3(0, 0, 10), points[0].Position);
            AssertNear(new float3(90, 0, 10), points[1].Position);
            AssertNear(new float3(90, 0, 100), points[2].Position);
        }

        [Fact]
        public void Hairpin_KeepsTwoPoints()
        {
            var hairpin = new Bezier4x3(new float3(100, 0, 0), new float3(60, 0, 0), new float3(60, 0, -40), new float3(100, 0, -40));
            var points = Solve(10f, true, 0f, Line(new float3(0, 0, 0), new float3(100, 0, 0)), hairpin);
            Assert.Equal(4, points.Count);
            AssertNear(new float3(100, 0, 10), points[1].Position);
            AssertNear(new float3(100, 0, -10), points[2].Position);
        }

        [Fact]
        public void Hairpin_SmallShift_MergesWithIncomingDirection()
        {
            var hairpin = new Bezier4x3(new float3(100, 0, 0), new float3(60, 0, 0), new float3(60, 0, -40), new float3(100, 0, -40));
            var points = Solve(3f, true, 0f, Line(new float3(0, 0, 0), new float3(100, 0, 0)), hairpin);
            Assert.Equal(3, points.Count);
            AssertNear(new float3(100, 0, 0), points[1].Position);
            AssertNear(new float3(1, 0, 0), points[1].Direction);
        }

        [Fact]
        public void Height_RaisesEveryPoint()
        {
            var points = Solve(10f, true, 5f, Line(new float3(0, 0, 0), new float3(100, 0, 0)));
            Assert.All(points, p => Assert.Equal(5f, p.Position.y, 3));
        }
    }
}
