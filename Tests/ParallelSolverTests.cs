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

        private static Bezier4x3 Arc(float3 center, float radius, float from, float to)
        {
            var k = 4f / 3f * math.tan((to - from) / 4f) * radius;
            var a = center + new float3(math.cos(from), 0, math.sin(from)) * radius;
            var d = center + new float3(math.cos(to), 0, math.sin(to)) * radius;
            return new Bezier4x3(a, a + new float3(-math.sin(from), 0, math.cos(from)) * k, d - new float3(-math.sin(to), 0, math.cos(to)) * k, d);
        }

        // Far from the origin, float noise makes the tangents of a smooth node almost but not exactly parallel; the line intersection must not slide the node along the curve.
        [Fact]
        public void SmoothArc_FarFromOrigin_KeepsNodesAcross()
        {
            var center = new float3(3000, 0, 3000);
            var step = math.radians(15f);
            var edges = new Bezier4x3[6];
            for (var i = 0; i < edges.Length; i++)
                edges[i] = Arc(center, 300f, -1f + i * step, -1f + (i + 1) * step);
            var points = Solve(12f, true, 0f, edges);
            Assert.Equal(7, points.Count);
            for (var i = 0; i < points.Count; i++)
            {
                var angle = -1f + i * step;
                var expected = center + new float3(math.cos(angle), 0, math.sin(angle)) * 288f;
                Assert.True(math.distance(expected, points[i].Position) < 0.5f, $"point {i}: expected {expected} but got {points[i].Position}");
            }
        }

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
        public void RightAngle_InnerCorner_KeepsSeparateTangents()
        {
            var points = Solve(10f, true, 0f, Line(new float3(0, 0, 0), new float3(100, 0, 0)), Line(new float3(100, 0, 0), new float3(100, 0, 100)));
            AssertNear(new float3(0, 0, 1), points[1].Direction);
            AssertNear(new float3(1, 0, 0), points[1].InDirection);
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
    

        [Fact]
        public void SignedOffset_LeftIsPositive()
        {
            var edges = new[] { Line(new float3(0, 0, 0), new float3(0, 0, 100)) };
            Assert.Equal(5f, ParallelSolver.SignedOffset(edges, new float3(-5, 0, 50)), 3);
            Assert.Equal(-7f, ParallelSolver.SignedOffset(edges, new float3(7, 3, 50)), 3);
        }

        [Fact]
        public void SignedOffset_PicksClosestEdge()
        {
            var edges = new[] { Line(new float3(0, 0, 0), new float3(0, 0, 100)), Line(new float3(0, 0, 100), new float3(100, 0, 100)) };
            Assert.Equal(4f, ParallelSolver.SignedOffset(edges, new float3(50, 0, 104)), 3);
        }

        [Theory]
        [InlineData(true, 6f)]
        [InlineData(false, -6f)]
        public void SignedOffset_MatchesSolve(bool left, float expected)
        {
            var edges = new[] { Line(new float3(0, 0, 0), new float3(0, 0, 100)) };
            var points = ParallelSolver.Solve(edges, 6f, left, 0f);
            Assert.Equal(expected, ParallelSolver.SignedOffset(edges, (points[0].Position + points[1].Position) / 2f), 3);
        }
    }
}
