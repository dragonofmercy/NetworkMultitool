using System.Collections.Generic;
using NetworkMultitool.Geometry;
using Unity.Mathematics;
using Xunit;

namespace NetworkMultitool.Tests
{
    public class SolverTests
    {
        private const float MinRadius = 16f;
        private const float MaxRadius = 5000f;

        private static Point P(float x, float y, float z, float dx, float dz) => new Point(new float3(x, y, z), new float3(dx, 0f, dz));

        private static void AssertContinuous(List<Point> pts, Point start, Point end, float maxLen)
        {
            Assert.True(pts.Count >= 2, $"expected at least 2 points, got {pts.Count}");
            Assert.True(math.distance(pts[0].Position, start.Position) < 1e-3f, $"first point {pts[0].Position} != {start.Position}");
            Assert.True(math.distance(pts[pts.Count - 1].Position, end.Position) < 1e-3f, $"last point {pts[pts.Count - 1].Position} != {end.Position}");

            for (var i = 0; i < pts.Count; i++)
            {
                Assert.False(math.any(math.isnan(pts[i].Position)) || math.any(math.isnan(pts[i].Direction)), $"NaN at {i}");
                if (i > 0)
                {
                    var dist = math.distance(pts[i - 1].Position, pts[i].Position);
                    Assert.True(dist <= maxLen * 1.05f + 8f, $"gap {dist} between {i - 1} and {i}");
                }
                if (i > 0 && i < pts.Count - 1)
                {
                    var dot = Vec.DotXZ(pts[i].Direction, Vec.NormalizeXZ(pts[i + 1].Position - pts[i - 1].Position));
                    Assert.True(dot > 0.7f, $"direction at {i} deviates (dot {dot})");
                }
            }
        }

        [Fact]
        public void Curve_FacingSegments_Ok()
        {
            var start = P(0, 0, 0, 1, 0);
            var end = P(200, 0, 100, -1, 0);
            var pts = new List<Point>();
            Assert.Equal(SolveResult.Ok, CurveSolver.Solve(start, end, 40f, pts));
            AssertContinuous(pts, start, end, 40f);
            Assert.True(pts.Count > 3);
            Assert.Equal(new float3(1, 0, 0), pts[0].Direction);
            Assert.Equal(new float3(1, 0, 0), pts[pts.Count - 1].Direction);
        }

        [Fact]
        public void Curve_BackToBack_WrongShape()
        {
            var pts = new List<Point>();
            Assert.Equal(SolveResult.WrongShape, CurveSolver.Solve(P(0, 0, 0, -1, 0), P(200, 0, 0, 1, 0), 40f, pts));
        }

        [Fact]
        public void Loop_ParallelGuides_NotIntersect()
        {
            var solver = new LoopSolver(P(0, 0, 0, 1, 0), P(0, 0, 50, 1, 0), MinRadius, MaxRadius);
            Assert.Equal(SolveResult.NotIntersect, solver.Solve(40f, new List<Point>()));
        }

        [Fact]
        public void Loop_PerpendicularGuides_Ok()
        {
            var start = P(0, 0, 0, 1, 0);
            var end = P(100, 0, 100, 0, -1);
            var solver = new LoopSolver(start, end, MinRadius, MaxRadius) { Radius = 30f };
            var pts = new List<Point>();
            Assert.Equal(SolveResult.Ok, solver.Solve(40f, pts));
            AssertContinuous(pts, start, end, 40f);
            Assert.True(solver.IsDirect);
            Assert.Equal(30f, solver.Arc.Radius, 4);
            Assert.True(math.distance(new float3(70, 0, 30), solver.Arc.Center) < 1e-3f, $"center {solver.Arc.Center}");
            Assert.Equal(math.PI / 2f, solver.Arc.Angle, 4);
            Assert.Equal(new float3(0, 0, 1), pts[pts.Count - 1].Direction);
        }

        [Theory]
        // start x, z, dx, dz, end x, z, dx, dz, force loop
        [InlineData(0, 0, 1, 0, 100, 100, 0, -1, false)]
        [InlineData(0, 0, 1, 0, 100, 100, 0, -1, true)]
        [InlineData(0, 0, 1, 0, 100, -100, 0, 1, false)]
        [InlineData(0, 0, 1, 0, 100, -100, 0, 1, true)]
        [InlineData(0, 0, 1, 0, 150, 80, -0.5f, -0.866f, false)]
        [InlineData(0, 0, 1, 0, 150, 80, -0.5f, -0.866f, true)]
        [InlineData(0, 0, 1, 0, 150, -80, -0.5f, 0.866f, true)]
        // Guides crossing behind the start node: only a loop is possible.
        [InlineData(0, 0, 1, 0, -100, 100, 0, -1, false)]
        [InlineData(0, 0, 1, 0, -100, -100, 0, 1, false)]
        // Guides crossing behind both nodes.
        [InlineData(0, 0, 1, 0, -100, -100, 0, -1, false)]
        [InlineData(0, 0, 1, 0, -100, 100, 0, 1, false)]
        public void Loop_ArcIsTangentToBothGuides(float sx, float sz, float sdx, float sdz, float ex, float ez, float edx, float edz, bool forceLoop)
        {
            var start = P(sx, 0, sz, sdx, sdz);
            var end = P(ex, 0, ez, edx, edz);
            var solver = new LoopSolver(start, end, MinRadius, MaxRadius) { ForceLoop = forceLoop };
            solver.Radius = 30f;
            var pts = new List<Point>();
            Assert.Equal(SolveResult.Ok, solver.Solve(40f, pts));
            var arc = solver.Arc;
            // The arc leaves the start guide along it and joins the end guide heading to the end node.
            Assert.True(math.abs(Vec.CrossXZ(arc.StartPos - start.Position, start.Direction)) < 1e-2f, $"start pos {arc.StartPos} off guide");
            Assert.True(math.abs(Vec.CrossXZ(arc.EndPos - end.Position, end.Direction)) < 1e-2f, $"end pos {arc.EndPos} off guide");
            Assert.True(Vec.DotXZ(arc.StartDir, Vec.NormalizeXZ(start.Direction)) > 0.999f, $"start dir {arc.StartDir}");
            Assert.True(Vec.DotXZ(arc.EndDir, -Vec.NormalizeXZ(end.Direction)) > 0.999f, $"end dir {arc.EndDir}");
            // Straights run forward: away from the start node, toward the end node.
            Assert.True(Vec.DotXZ(arc.StartPos - start.Position, start.Direction) > -1e-2f, $"start straight goes backward ({arc.StartPos})");
            Assert.True(Vec.DotXZ(arc.EndPos - end.Position, end.Direction) > -1e-2f, $"end straight goes backward ({arc.EndPos})");
        }

        [Fact]
        public void Connection_InsertAndRemoveMiddleArc()
        {
            var start = P(0, 0, 0, 1, 0);
            var end = P(300, 0, 120, -1, 0);
            var solver = new ConnectionSolver(start, end, MinRadius, MaxRadius);
            var pts = new List<Point>();
            Assert.Equal(SolveResult.Ok, solver.Solve(40f, pts));

            var middle = solver.Straights[1];
            var hit = math.lerp(middle.Start, middle.End, 0.5f);
            Assert.Equal(1, solver.StraightAt(hit, 4f));
            Assert.Equal(-1, solver.StraightAt(hit + new float3(0, 0, 30), 4f));

            foreach (var turn in new[] { Turn.Right, Turn.Left })
            {
                const int index = 1;
                solver.InsertArc(index, hit);
                if (solver.Arcs[index].Turn != turn)
                    solver.SwitchSide(index);
                Assert.Equal(3, solver.Arcs.Count);
                Assert.True(solver.IsMiddle(1));
                Assert.Equal(SolveResult.Ok, solver.Solve(40f, pts));
                AssertContinuous(pts, start, end, 40f);
                Assert.Equal(4, solver.Straights.Count);

                solver.DragCenter(1, hit + new float3(0, 0, 40));
                solver.DragEdge(1, hit + new float3(30, 0, 40));
                Assert.Equal(30f, solver.Arcs[1].Radius, 3);

                solver.RemoveArc(1);
                solver.RemoveArc(0);
                Assert.Equal(2, solver.Arcs.Count);
                Assert.Equal(SolveResult.Ok, solver.Solve(40f, pts));
            }
        }

        [Fact]
        public void Connection_Default_Ok()
        {
            var start = P(0, 0, 0, 1, 0);
            var end = P(300, 0, 120, -1, 0);
            var solver = new ConnectionSolver(start, end, MinRadius, MaxRadius) { FirstRadius = 50f, SecondRadius = 50f };
            var pts = new List<Point>();
            Assert.Equal(SolveResult.Ok, solver.Solve(40f, pts));
            AssertContinuous(pts, start, end, 40f);
            Assert.Equal(Turn.Left, solver.First.Turn);
            Assert.Equal(Turn.Right, solver.Second.Turn);
            Assert.True(math.distance(new float3(0, 0, 50), solver.First.Center) < 1e-3f, $"first center {solver.First.Center}");
            Assert.True(math.distance(new float3(300, 0, 70), solver.Second.Center) < 1e-3f, $"second center {solver.Second.Center}");
        }

        [Fact]
        public void Loop_SetEdge_CircleReachesDraggedPoint()
        {
            var solver = new LoopSolver(P(0, 0, 0, 1, 0), P(100, 0, 100, 0, -1), MinRadius, MaxRadius) { Radius = 45f };
            // On the radius 30 circle centered at (70, 30).
            solver.SetEdge(new float3(70, 0, 60));
            Assert.Equal(30f, solver.Radius, 3);
            Assert.True(solver.IsDirect);
        }

        [Fact]
        public void Connection_DragEdge_CircleReachesDraggedPoint()
        {
            var solver = new ConnectionSolver(P(0, 0, 0, 1, 0), P(300, 0, 120, -1, 0), MinRadius, MaxRadius) { FirstRadius = 80f, SecondRadius = 80f };
            // Circles tangent to the guides at the nodes: radius 50 centered at (0, 50) and (300, 70).
            solver.DragEdge(0, new float3(50, 0, 50));
            solver.DragEdge(1, new float3(300, 0, 20));
            Assert.Equal(50f, solver.FirstRadius, 3);
            Assert.Equal(50f, solver.SecondRadius, 3);
            // Behind the guide: ignored.
            solver.DragEdge(0, new float3(10, 0, -40));
            Assert.Equal(50f, solver.FirstRadius, 3);
        }

        [Fact]
        public void Connection_DragCenter_SlidesAlongGuide()
        {
            var solver = new ConnectionSolver(P(0, 0, 0, 1, 0), P(300, 0, 120, -1, 0), MinRadius, MaxRadius);
            solver.DragCenter(0, new float3(30, 0, 77));
            solver.DragCenter(1, new float3(250, 0, 0));
            Assert.Equal(30f, solver.FirstOffset, 3);
            Assert.Equal(50f, solver.SecondOffset, 3);
            Assert.Equal(50f, solver.FirstRadius, 3);
            solver.DragCenter(0, new float3(-20, 0, 0));
            Assert.Equal(0f, solver.FirstOffset, 3);
        }

        [Fact]
        public void Connection_RadiusTooBig_WrongShape()
        {
            var solver = new ConnectionSolver(P(0, 0, 0, 1, 0), P(300, 0, 120, -1, 0), MinRadius, MaxRadius) { FirstRadius = 400f, SecondRadius = 400f };
            Assert.Equal(SolveResult.WrongShape, solver.Solve(40f, new List<Point>()));
        }

        [Fact]
        public void Connection_SwitchSide_StillContinuous()
        {
            var start = P(0, 0, 0, 1, 0);
            var end = P(300, 0, 120, -1, 0);
            var solver = new ConnectionSolver(start, end, MinRadius, MaxRadius) { FirstRadius = 50f, SecondRadius = 50f };
            solver.SwitchSide(0);
            solver.SwitchSide(1);
            var pts = new List<Point>();
            Assert.Equal(SolveResult.Ok, solver.Solve(40f, pts));
            AssertContinuous(pts, start, end, 40f);
            Assert.Equal(Turn.Right, solver.First.Turn);
            Assert.Equal(Turn.Left, solver.Second.Turn);
            Assert.True(math.distance(new float3(0, 0, -50), solver.First.Center) < 1e-3f, $"first center {solver.First.Center}");
        }

        [Fact]
        public void Connection_SwitchOneSide_OnlyThatArcFlips()
        {
            var start = P(0, 0, 0, 1, 0);
            var end = P(300, 0, 120, -1, 0);
            var solver = new ConnectionSolver(start, end, MinRadius, MaxRadius) { FirstRadius = 50f, SecondRadius = 50f };
            var second = solver.Second.Turn;
            solver.SwitchSide(0);
            var pts = new List<Point>();
            Assert.Equal(SolveResult.Ok, solver.Solve(40f, pts));
            AssertContinuous(pts, start, end, 40f);
            Assert.Equal(Turn.Right, solver.First.Turn);
            Assert.Equal(second, solver.Second.Turn);
        }
    }
}
