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
    }
}
