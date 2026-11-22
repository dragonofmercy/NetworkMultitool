using System.Collections.Generic;
using NetworkMultitool.Geometry;
using NetworkMultitool.Tool;
using Unity.Entities;
using Unity.Mathematics;

namespace NetworkMultitool.Modes
{
    /// <summary>Connects two segments with two arcs joined by a straight (port of CS1 CreateConnectionMode).</summary>
    public sealed class ConnectionMode : CreateModeBase
    {
        private ConnectionSolver _solver;
        private Point _start;
        private Point _end;
        private bool _selectFirst = true;
        private bool _switched;
        private Drag _drag;

        private const float GrabDistance = 5f;

        private enum Drag
        {
            None,
            Center,
            Edge,
        }

        /// <summary>Both arcs are placed, even when they overlap: the circles stay visible and draggable so the shape can be fixed.</summary>
        private bool HasArcs => _solver != null && Second != Entity.Null && FirstNode != SecondNode && (Result == SolveResult.Ok || Result == SolveResult.WrongShape);

        public override ToolModeId Id => ToolModeId.Connection;

        protected override string ExtraValues => _solver == null ? string.Empty : $"First radius: {_solver.FirstRadius:0.0} m\nFirst offset: {_solver.FirstOffset:0.0} m\nSecond radius: {_solver.SecondRadius:0.0} m\nSecond offset: {_solver.SecondOffset:0.0} m\n";

        protected override string ExtraHints => "Drag edge: radius\nDrag center: offset\nUp/Down: radius (Shift: both)\nLeft/Right: offset\nTab: switch circle\nCtrl+S: switch side\n";

        protected override SolveResult Solve(ToolContext ctx, List<Point> points)
        {
            if (_solver == null || !_start.Equals(StartPoint) || !_end.Equals(EndPoint))
            {
                var old = _solver;
                _start = StartPoint;
                _end = EndPoint;
                _solver = new ConnectionSolver(StartPoint, EndPoint, ctx.Net.Width(First), 5000f);
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

        protected override bool ConsumeClick(ToolContext ctx)
        {
            _drag = Grab(ctx, out var first);
            if (_drag != Drag.None)
                _selectFirst = first;
            return _drag != Drag.None;
        }

        /// <summary>What a click at the cursor would grab, and on which circle.</summary>
        private Drag Grab(ToolContext ctx, out bool first)
        {
            first = _selectFirst;
            if (!HasArcs || !ctx.HasHit)
                return Drag.None;

            var mouse = Vec.Flat(ctx.MousePosition);
            var toFirst = math.distance(mouse, Vec.Flat(_solver.First.Center));
            var toSecond = math.distance(mouse, Vec.Flat(_solver.Second.Center));
            // Centers first: one circle's edge can pass over the other's center.
            if (math.min(toFirst, toSecond) < GrabDistance)
            {
                first = toFirst <= toSecond;
                return Drag.Center;
            }

            // Each circle touches its guide near a node: a click on an end node keeps changing the direction.
            if (HoverEnd)
                return Drag.None;
            var firstEdge = math.abs(toFirst - _solver.First.Radius);
            var secondEdge = math.abs(toSecond - _solver.Second.Radius);
            if (math.min(firstEdge, secondEdge) >= GrabDistance)
                return Drag.None;
            first = firstEdge <= secondEdge;
            return Drag.Edge;
        }

        protected override void OnKeys(ToolContext ctx)
        {
            if (ctx.Pressed(ToolAction.SwitchOffset))
                _selectFirst = !_selectFirst;
            if (_solver == null || Second == Entity.Null)
            {
                _drag = Drag.None;
                return;
            }

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
                    _solver.DragCenter(_selectFirst, ctx.MousePosition);
                else
                    _solver.DragEdge(_selectFirst, ctx.MousePosition);
                MarkDirty();
            }
        }

        protected override void RenderShape(ToolContext ctx, Overlay overlay)
        {
            if (!HasArcs)
                return;
            // The part under the cursor that can be dragged, or the one being dragged, is highlighted.
            var first = _selectFirst;
            var grab = _drag != Drag.None ? _drag : Grab(ctx, out first);
            DrawArc(overlay, _solver.First, _selectFirst, first ? grab : Drag.None);
            DrawArc(overlay, _solver.Second, !_selectFirst, first ? Drag.None : grab);
        }

        protected override void OnReset()
        {
            _solver = null;
            _selectFirst = true;
            _switched = false;
            _drag = Drag.None;
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

        private void DrawArc(Overlay overlay, Arc arc, bool selected, Drag grab)
        {
            var center = Vec.WithY(arc.Center, arc.Center.y + Height);
            overlay.CircleOutline(center, arc.Radius, grab == Drag.Edge ? Overlay.Green : Result != SolveResult.Ok ? Overlay.Red : selected ? Overlay.Yellow : Overlay.Gray, grab == Drag.Edge ? 2f : 1f);
            overlay.Disc(center, grab == Drag.Center ? 6f : 4f, grab == Drag.Center ? Overlay.Green : Overlay.White);
        }
    }
}
