using System.Collections.Generic;
using NetworkMultitool.Geometry;
using NetworkMultitool.Tool;
using Unity.Entities;
using Unity.Mathematics;

namespace NetworkMultitool.Modes
{
    /// <summary>Connects two segments with a single arc, direct or looping (port of CS1 CreateLoopMode).</summary>
    public sealed class LoopMode : CreateModeBase
    {
        private LoopSolver _solver;
        private Point _start;
        private Point _end;
        private const float GrabDistance = 5f;

        private enum Drag
        {
            None,
            Center,
            Edge,
        }

        private Drag _drag;

        public override ToolModeId Id => ToolModeId.Loop;

        protected override string ExtraValues => _solver == null ? string.Empty : $"Radius: {_solver.Arc.Radius:0.0} m\nAngle: {math.degrees(_solver.Arc.Angle):0} deg\n";

        protected override string ExtraHints => "Drag: center or circle edge\nUp/Down: radius\nCtrl+S: direct/loop\n";

        protected override SolveResult Solve(ToolContext ctx, List<Point> points)
        {
            if (_solver == null || !_start.Equals(StartPoint) || !_end.Equals(EndPoint))
            {
                var old = _solver;
                _start = StartPoint;
                _end = EndPoint;
                _solver = new LoopSolver(StartPoint, EndPoint, ctx.Net.Width(First), 5000f);
                if (old != null)
                {
                    // ForceLoop first: the radius clamp depends on it.
                    _solver.ForceLoop = old.ForceLoop;
                    _solver.Radius = old.Radius;
                }
            }

            return _solver.Solve(ctx.MaxSegmentLength, points);
        }

        protected override bool ConsumeClick(ToolContext ctx)
        {
            _drag = Grab(ctx);
            return _drag != Drag.None;
        }

        /// <summary>What a click at the cursor would grab.</summary>
        private Drag Grab(ToolContext ctx)
        {
            if (Second == Entity.Null || _solver == null || Result != SolveResult.Ok || !ctx.HasHit)
                return Drag.None;
            var distance = math.distance(Vec.Flat(ctx.MousePosition), Vec.Flat(_solver.Arc.Center));
            if (distance < GrabDistance)
                return Drag.Center;
            // The circle touches both guides near their nodes: a click on an end node keeps changing the direction.
            return !HoverEnd && math.abs(distance - _solver.Arc.Radius) < GrabDistance ? Drag.Edge : Drag.None;
        }

        protected override void OnKeys(ToolContext ctx)
        {
            if (_solver == null || Second == Entity.Null)
            {
                _drag = Drag.None;
                return;
            }

            if (ctx.Pressed(ToolAction.RadiusUp))
            {
                _solver.Radius += 10f * ctx.StepMultiplier;
                MarkDirty();
            }

            if (ctx.Pressed(ToolAction.RadiusDown))
            {
                _solver.Radius -= 10f * ctx.StepMultiplier;
                MarkDirty();
            }

            if (ctx.Pressed(ToolAction.Side))
            {
                _solver.ForceLoop = !_solver.ForceLoop;
                MarkDirty();
            }

            if (_drag == Drag.None)
                return;
            if (!ctx.ApplyHeld)
            {
                _drag = Drag.None;
                return;
            }

            if (ctx.HasHit)
            {
                if (_drag == Drag.Center)
                    _solver.SetCenter(ctx.MousePosition);
                else
                    _solver.SetEdge(ctx.MousePosition);
                MarkDirty();
            }
        }

        protected override void RenderShape(ToolContext ctx, Overlay overlay)
        {
            if (_solver == null || Second == Entity.Null || Result != SolveResult.Ok)
                return;
            var arc = _solver.Arc;
            var center = Vec.WithY(arc.Center, arc.Center.y + Height);
            // The part under the cursor that can be dragged, or the one being dragged, is highlighted.
            var grab = _drag != Drag.None ? _drag : Grab(ctx);
            overlay.CircleOutline(center, arc.Radius, grab == Drag.Edge ? Overlay.Green : Overlay.Gray, grab == Drag.Edge ? 2f : 1f);
            overlay.Disc(center, grab == Drag.Center ? 6f : 4f, grab == Drag.Center ? Overlay.Green : Overlay.White);
        }

        protected override void OnReset()
        {
            _solver = null;
            _drag = Drag.None;
        }
    }
}
