using System.Collections.Generic;
using NetworkMultitool.Geometry;
using NetworkMultitool.Tool;
using Unity.Entities;
using UnityEngine;

namespace NetworkMultitool.Modes
{
    /// <summary>Connects two segments with two arcs joined by a straight (port of CS1 CreateConnectionMode).</summary>
    public sealed class ConnectionMode : CreateModeBase
    {
        private static readonly Color Gray = new Color(0.8f, 0.8f, 0.8f, 0.6f);

        private ConnectionSolver _solver;
        private Point _start;
        private Point _end;
        private bool _selectFirst = true;
        private bool _switched;

        public override ToolModeId Id => ToolModeId.Connection;

        protected override string ExtraInfo => _solver == null ? string.Empty : $"\nFirst radius: {_solver.FirstRadius:0.0} m, second radius: {_solver.SecondRadius:0.0} m\nOffsets: {_solver.FirstOffset:0.0} m / {_solver.SecondOffset:0.0} m (Tab: switch, Left/Right: change)\nUp/Down: radius (Shift: both), Ctrl+S: switch side";

        protected override SolveResult Solve(ToolContext ctx, List<Point> points)
        {
            if (_solver == null || !_start.Equals(StartPoint) || !_end.Equals(EndPoint))
            {
                var old = _solver;
                _start = StartPoint;
                _end = EndPoint;
                _solver = new ConnectionSolver(StartPoint, EndPoint, ctx.Net.Width(First) * 2f, 5000f);
                if (old != null)
                {
                    _solver.FirstRadius = old.FirstRadius;
                    _solver.SecondRadius = old.SecondRadius;
                    _solver.FirstOffset = old.FirstOffset;
                    _solver.SecondOffset = old.SecondOffset;
                }

                // The side is kept relative to the geometric default the new solver computed.
                if (_switched)
                    _solver.SwitchSide();
            }

            return _solver.Solve(ctx.MaxSegmentLength, points);
        }

        protected override void OnKeys(ToolContext ctx)
        {
            if (ctx.Pressed(ToolAction.SwitchOffset))
                _selectFirst = !_selectFirst;
            if (_solver == null || Second == Entity.Null)
                return;

            // Shift is the only modifier giving StepMultiplier 10, so it doubles as the "both radii" signal.
            var both = ctx.StepMultiplier == 10f;
            if (ctx.Pressed(ToolAction.RadiusUp))
                ChangeRadius(10f * ctx.StepMultiplier, both);
            if (ctx.Pressed(ToolAction.RadiusDown))
                ChangeRadius(-10f * ctx.StepMultiplier, both);

            if (ctx.Pressed(ToolAction.OffsetUp))
                ChangeOffset(ctx.StepMultiplier);
            if (ctx.Pressed(ToolAction.OffsetDown))
                ChangeOffset(-ctx.StepMultiplier);

            if (ctx.Pressed(ToolAction.Side))
            {
                _switched = !_switched;
                _solver.SwitchSide();
                MarkDirty();
            }
        }

        protected override void RenderShape(ToolContext ctx, Overlay overlay)
        {
            if (_solver == null || Second == Entity.Null || Result != SolveResult.Ok)
                return;
            DrawArc(overlay, _solver.First, _selectFirst);
            DrawArc(overlay, _solver.Second, !_selectFirst);
        }

        protected override void OnReset()
        {
            _solver = null;
            _selectFirst = true;
            _switched = false;
        }

        private void ChangeRadius(float delta, bool both)
        {
            if (both || _selectFirst)
                _solver.FirstRadius += delta;
            if (both || !_selectFirst)
                _solver.SecondRadius += delta;
            MarkDirty();
        }

        private void ChangeOffset(float delta)
        {
            if (_selectFirst)
                _solver.FirstOffset += delta;
            else
                _solver.SecondOffset += delta;
            MarkDirty();
        }

        private void DrawArc(Overlay overlay, Arc arc, bool selected)
        {
            var center = Vec.WithY(arc.Center, arc.Center.y + Height);
            overlay.CircleOutline(center, arc.Radius, selected ? Overlay.Yellow : Gray);
            overlay.Disc(center, 4f, Overlay.White);
        }
    }
}
