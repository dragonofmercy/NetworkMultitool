using System.Collections.Generic;
using System.Linq;
using NetworkMultitool.Geometry;
using Unity.Mathematics;
using Xunit;

namespace NetworkMultitool.Tests
{
    public class ArcTests
    {
        private static void AssertNear(float3 expected, float3 actual, float tol = 1e-3f)
        {
            Assert.True(math.distance(expected, actual) < tol, $"expected {expected} but got {actual}");
        }

        private static Arc QuarterArc(Turn turn) => new Arc
        {
            Center = float3.zero,
            Radius = 50f,
            StartRadiusDir = new float3(1, 0, 0),
            EndRadiusDir = new float3(0, 0, 1),
            Turn = turn,
        };

        [Fact]
        public void Angle_LeftQuarter_IsHalfPi()
        {
            var arc = QuarterArc(Turn.Left);
            Assert.Equal(math.PI / 2f, arc.Angle, 4);
            AssertNear(new float3(50, 0, 0), arc.StartPos);
            AssertNear(new float3(0, 0, 50), arc.EndPos);
            AssertNear(new float3(0, 0, 1), arc.StartDir);
        }

        [Fact]
        public void Angle_RightFromSameDirs_IsThreeHalfPi()
        {
            var arc = QuarterArc(Turn.Right);
            Assert.Equal(1.5f * math.PI, arc.Angle, 4);
            AssertNear(new float3(0, 0, -1), arc.StartDir);
        }

        [Fact]
        public void Parts_RespectMaxLength()
        {
            var arc = QuarterArc(Turn.Left);
            var parts = arc.Parts(20f).ToList();
            Assert.True(parts.Count >= 3, $"only {parts.Count} inner points");

            var chain = new List<float3> { arc.StartPos };
            chain.AddRange(parts.Select(p => p.Position));
            chain.Add(arc.EndPos);
            for (var i = 1; i < chain.Count; i++)
                Assert.True(math.distance(chain[i - 1], chain[i]) <= 20.5f, $"gap {math.distance(chain[i - 1], chain[i])} at {i}");

            foreach (var p in parts)
                Assert.Equal(50f, math.distance(arc.Center, p.Position), 3);
        }

        [Fact]
        public void Connect_OppositeTurns_TangentsMatch()
        {
            var first = new Arc { Center = float3.zero, Radius = 50f, Turn = Turn.Left };
            var second = new Arc { Center = new float3(150, 0, 0), Radius = 50f, Turn = Turn.Right };
            Arc.Connect(first, second);

            AssertNear(Vec.Flat(first.EndDir), Vec.Flat(second.StartDir));
            AssertNear(Vec.NormalizeXZ(second.StartPos - first.EndPos), Vec.Flat(first.EndDir));
            Assert.Equal(50f, math.distance(first.Center, first.EndPos), 3);
            Assert.Equal(50f, math.distance(second.Center, second.StartPos), 3);
        }

        [Fact]
        public void CheckRadii_TooBig_ReturnsFalse()
        {
            var first = new Arc { Center = float3.zero, Radius = 100f, Turn = Turn.Left };
            var second = new Arc { Center = new float3(150, 0, 0), Radius = 100f, Turn = Turn.Right };
            Assert.False(Arc.CheckRadii(first, second));
        }

        [Fact]
        public void Straight_Parts_SpacingUnderMax()
        {
            var straight = new Straight(float3.zero, new float3(100, 0, 0));
            var parts = straight.Parts(30f).ToList();
            Assert.Equal(3, parts.Count);
            AssertNear(new float3(25, 0, 0), parts[0].Position);
            AssertNear(new float3(50, 0, 0), parts[1].Position);
            AssertNear(new float3(75, 0, 0), parts[2].Position);
            AssertNear(new float3(1, 0, 0), parts[1].Direction);
        }
    }
}
