using NetworkMultitool.Geometry;
using Unity.Mathematics;
using Xunit;

namespace NetworkMultitool.Tests
{
    public class ArrangerTests
    {
        [Fact]
        public void Line_Straight_EvenSpacing()
        {
            var pts = new[] { new float3(0, 0, 0), new float3(10, 0, 30), new float3(50, 0, -10), new float3(100, 0, 0) };
            var res = LineArranger.Arrange(pts, float3.zero, float3.zero);
            var expected = new[] { 0f, 100f / 3f, 200f / 3f, 100f };
            Assert.Equal(4, res.Length);
            for (var i = 0; i < 4; i += 1)
            {
                Assert.InRange(res[i].Position.x, expected[i] - 0.01f, expected[i] + 0.01f);
                Assert.InRange(res[i].Position.z, -0.01f, 0.01f);
            }
        }

        [Fact]
        public void Line_KeepsEnds()
        {
            var pts = new[] { new float3(0, 1, 0), new float3(10, 2, 30), new float3(50, 3, -10), new float3(100, 4, 20) };
            var res = LineArranger.Arrange(pts, new float3(0, 0, 1), new float3(0, 0, 1));
            Assert.True(math.distance(pts[0], res[0].Position) < 1e-3f);
            Assert.True(math.distance(pts[3], res[3].Position) < 1e-3f);
        }

        [Fact]
        public void Circle_TryFit_ThreePoints()
        {
            var ok = CircleArranger.TryFit(new[] { new float3(50, 0, 0), new float3(0, 0, 50), new float3(-50, 0, 0) }, out var c, out var r);
            Assert.True(ok);
            Assert.True(math.distance(c, float3.zero) < 1e-3f);
            Assert.InRange(r, 49.99f, 50.01f);
        }

        [Fact]
        public void Circle_TryFit_Collinear_ReturnsFalse()
        {
            Assert.False(CircleArranger.TryFit(new[] { new float3(0, 0, 0), new float3(1, 0, 0), new float3(2, 0, 0) }, out _, out _));
        }

        [Fact]
        public void Circle_Distribute_Evenly_EqualAngles()
        {
            var pts = new float3[4];
            var a = new[] { 0f, 0.1f, 0.5f, math.PI / 2f };
            for (var i = 0; i < 4; i += 1)
            {
                pts[i] = new float3(50 * math.cos(a[i]), 0, 50 * math.sin(a[i]));
            }
            var res = CircleArranger.Distribute(pts, float3.zero, 50f, true);
            var ang = CircleArranger.Angles(res, float3.zero);
            var d = ang[1] - ang[0];
            Assert.InRange(ang[2] - ang[1], d - 1e-4f, d + 1e-4f);
            Assert.InRange(ang[3] - ang[2], d - 1e-4f, d + 1e-4f);
        }

        [Fact]
        public void Slope_Heights_Proportional()
        {
            var h = SlopeSolver.Heights(0f, 10f, new[] { 10f, 30f });
            Assert.Equal(new[] { 0f, 2.5f, 10f }, h);
        }

        [Fact]
        public void Slope_ZeroLength_NoNaN()
        {
            Assert.Equal(new[] { 5f, 5f, 5f }, SlopeSolver.Heights(5f, 9f, new[] { 0f, 0f }));
        }
    }
}
