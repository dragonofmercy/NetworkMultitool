using System.Collections.Generic;
using NetworkMultitool.Geometry;
using NetworkMultitool.Tool;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

namespace NetworkMultitool.Modes
{
    /// <summary>Connects two segments with a single arc, direct or looping (port of CS1 CreateLoopMode).</summary>
    public sealed class LoopMode : CreateModeBase
    {
        private static readonly Color Gray = new Color(0.8f, 0.8f, 0.8f, 0.6f);

        private LoopSolver _solver;
        private Point _start;
        private Point _end;
        private bool _dragging;

        public override ToolModeId Id => ToolModeId.Loop;

        protected override string ExtraInfo => _solver == null ? string.Empty : $"\nRadius: {_solver.Arc.Radius:0.0} m, angle: {math.degrees(_solver.Arc.Angle):0} deg\nUp/Down: radius, Ctrl+S: direct/loop, drag center to move";

        protected override SolveResult Solve(ToolContext ctx, List<Point> points)
        {
            if (_solver == null || !_start.Equals(StartPoint) || !_end.Equals(EndPoint))
            {
                var old = _solver;
                _start = StartPoint;
                _end = EndPoint;
                _solver = new LoopSolver(StartPoint, EndPoint, ctx.Net.Width(First) * 2f, 5000f);
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
            if (Second == Entity.Null || _solver == null || Result != SolveResult.Ok || !ctx.HasHit || math.distance(Vec.Flat(ctx.MousePosition), Vec.Flat(_solver.Arc.Center)) >= 5f)
                return false;
            _dragging = true;
            return true;
        }

        protected override void OnKeys(ToolContext ctx)
        {
            if (_solver == null || Second == Entity.Null)
            {
                _dragging = false;
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

            if (!_dragging)
                return;
            if (!ctx.ApplyHeld)
            {
                _dragging = false;
                return;
            }

            if (ctx.HasHit)
            {
                _solver.SetCenter(ctx.MousePosition);
                MarkDirty();
            }
        }

        protected override void RenderShape(ToolContext ctx, Overlay overlay)
        {
            if (_solver == null || Second == Entity.Null || Result != SolveResult.Ok)
                return;
            var arc = _solver.Arc;
            var center = Vec.WithY(arc.Center, arc.Center.y + Height);
            overlay.CircleOutline(center, arc.Radius, Gray);
            overlay.Disc(center, 4f, Overlay.White);
        }

        protected override void OnReset()
        {
            _solver = null;
            _dragging = false;
        }
    }
}
