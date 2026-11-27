using System.Collections.Generic;
using NetworkMultitool.Geometry;
using NetworkMultitool.Tool;
using Unity.Entities;
using Unity.Mathematics;
using static NetworkMultitool.Localization.Texts;

namespace NetworkMultitool.Modes
{
    /// <summary>Connects two segments with a chain of arcs joined by straights (port of CS1 CreateConnectionMode).</summary>
    public sealed class ConnectionMode : CreateModeBase
    {
        // Close to the Windows default double-click time.
        private const float DoubleClickTime = 0.5f;

        private ConnectionSolver _solver;
        private Point _start;
        private Point _end;
        private float _width;
        private int _selected;
        private bool _firstSwitched;
        private bool _secondSwitched;
        private float _lastClick = -1f;
        private float _lastSecondaryClick = -1f;

        /// <summary>All arcs are placed, even when they overlap: the circles stay visible and draggable so the shape can be fixed.</summary>
        private bool HasArcs => _solver != null && Second != Entity.Null && FirstNode != SecondNode && (Result == SolveResult.Ok || Result == SolveResult.WrongShape);

        private int LastArc => _solver.Arcs.Count - 1;

        public override ToolModeId Id => ToolModeId.Connection;

        protected override int ShiftedEnd => _solver == null ? -1 : _selected == 0 ? 0 : _selected == LastArc ? 1 : -1;

        protected override string ShiftEndHint => string.Empty;

        protected override string ExtraHints => $"{T("ConnectionHintDragEdge")}\n{T("ConnectionHintDragCenter")}\n{T("HintSnap")}\n{T("ConnectionHintFlip")}\n{T("ConnectionHintAdd")}\n{T("ConnectionHintRemove")}\n" +
            $"{T("ConnectionHintRadius", KeyNames.Of(ToolAction.RadiusUp), KeyNames.Of(ToolAction.RadiusDown))}\n{T("HintOffset", KeyNames.Of(ToolAction.OffsetDown), KeyNames.Of(ToolAction.OffsetUp))}\n" +
            $"{T("ConnectionHintSwitch", KeyNames.Of(ToolAction.SwitchOffset))}\n";

        protected override SolveResult Solve(ToolContext ctx, List<Point> points)
        {
            if (_solver == null || !_start.Equals(StartPoint) || !_end.Equals(EndPoint) || _width != NewWidth(ctx))
            {
                var old = _solver;
                _start = StartPoint;
                _end = EndPoint;
                _width = NewWidth(ctx);
                _solver = new ConnectionSolver(StartPoint, EndPoint, _width, 5000f);
                if (old != null)
                    _solver.CopyFrom(old);

                // The edge sides are kept relative to the geometric default the new solver computed.
                if (_firstSwitched)
                    _solver.SwitchSide(0);
                if (_secondSwitched)
                    _solver.SwitchSide(LastArc);
            }

            return _solver.Solve(ctx.MaxSegmentLength, points);
        }

        protected override bool ConsumeClick(ToolContext ctx)
        {
            var isDouble = IsDoubleClick(ref _lastClick);
            var grab = Grab(ctx, out var index);
            if (isDouble && HasArcs && grab == Drag.Center)
            {
                SwitchSide(index);
                return true;
            }

            if (isDouble && HasArcs && ctx.HasHit)
            {
                var straight = _solver.StraightAt(ctx.MousePosition, NewWidth(ctx) / 2f);
                if (straight >= 0)
                {
                    _solver.InsertArc(straight, ctx.MousePosition);
                    _selected = straight;
                    MarkDirty();
                    return true;
                }
            }

            Dragging = grab;
            if (Dragging != Drag.None)
                _selected = index;
            return Dragging != Drag.None;
        }

        /// <summary>A right click on a center never deselects the segment (CS1); a right double click there removes a middle circle.</summary>
        protected override bool ConsumeSecondaryClick(ToolContext ctx)
        {
            if (Grab(ctx, out var index) != Drag.Center)
                return false;

            if (IsDoubleClick(ref _lastSecondaryClick) && _solver.IsMiddle(index))
            {
                _solver.RemoveArc(index);
                if (_selected >= index)
                    _selected = math.max(_selected - 1, 0);
                MarkDirty();
            }

            return true;
        }

        /// <summary>What a click at the cursor would grab, and on which circle.</summary>
        private Drag Grab(ToolContext ctx, out int index)
        {
            index = _selected;
            if (!HasArcs || !ctx.HasHit)
                return Drag.None;

            var mouse = Vec.Flat(ctx.MousePosition);
            // Centers first: one circle's edge can pass over another's center.
            if (Nearest(arc => math.distance(mouse, Vec.Flat(arc.Center)), ref index))
                return Drag.Center;

            // The edge circles touch their guide near a node: a click on an end node keeps changing the direction.
            if (HoverEnd)
                return Drag.None;
            return Nearest(arc => math.abs(math.distance(mouse, Vec.Flat(arc.Center)) - arc.Radius), ref index) ? Drag.Edge : Drag.None;
        }

        /// <summary>The arc closest by <paramref name="distance"/>, if one is within grab distance.</summary>
        private bool Nearest(System.Func<Arc, float> distance, ref int index)
        {
            var best = GrabDistance;
            var found = false;
            for (var i = 0; i < _solver.Arcs.Count; i++)
            {
                var d = distance(_solver.Arcs[i]);
                if (d < best)
                {
                    best = d;
                    index = i;
                    found = true;
                }
            }

            return found;
        }

        /// <summary>True on the second click within the double-click time; that click does not start a new pair.</summary>
        private static bool IsDoubleClick(ref float last)
        {
            var now = UnityEngine.Time.realtimeSinceStartup;
            var isDouble = now - last < DoubleClickTime;
            last = isDouble ? -1f : now;
            return isDouble;
        }

        protected override void OnKeys(ToolContext ctx)
        {
            if (_solver == null || Second == Entity.Null)
                return;
            if (ctx.Pressed(ToolAction.SwitchOffset))
                _selected = (_selected + 1) % _solver.Arcs.Count;

            // Shift is the only modifier giving StepMultiplier 10, so it doubles as the "all radii" signal.
            var all = ctx.StepMultiplier == 10f;
            if (ctx.Pressed(ToolAction.RadiusUp))
                ChangeRadius(10f * ctx.StepMultiplier, all);
            if (ctx.Pressed(ToolAction.RadiusDown))
                ChangeRadius(-10f * ctx.StepMultiplier, all);

            if (ctx.Pressed(ToolAction.OffsetUp))
                ChangeOffset(ctx.StepMultiplier);
            if (ctx.Pressed(ToolAction.OffsetDown))
                ChangeOffset(-ctx.StepMultiplier);
        }

        protected override void OnDrag(ToolContext ctx)
        {
            var p = ctx.MousePosition;
            if (Dragging == Drag.Edge)
            {
                _solver.DragEdge(_selected, p);
                _solver.SetRadius(_selected, ctx.Snap(_solver.Arcs[_selected].Radius));
            }
            else if (_solver.IsMiddle(_selected))
                // A free circle center snaps to the world 1 m grid.
                _solver.DragCenter(_selected, new float3(ctx.Snap(p.x), p.y, ctx.Snap(p.z)));
            else
            {
                _solver.DragCenter(_selected, p);
                if (_selected == 0)
                    _solver.FirstOffset = ctx.Snap(_solver.FirstOffset);
                else
                    _solver.SecondOffset = ctx.Snap(_solver.SecondOffset);
            }
        }

        protected override void RenderShape(ToolContext ctx, Overlay overlay)
        {
            if (!HasArcs)
                return;
            var index = _selected;
            var grab = Dragging != Drag.None ? Dragging : Grab(ctx, out index);
            for (var i = 0; i < _solver.Arcs.Count; i++)
                DrawHandles(ctx, overlay, _solver.Arcs[i], IdleColor(i == _selected), i == index ? grab : Drag.None);

            // The straight a double click would split with a new circle.
            if (grab == Drag.None && Dragging == Drag.None && ctx.HasHit)
            {
                var width = NewWidth(ctx);
                var straight = _solver.StraightAt(ctx.MousePosition, width / 2f);
                if (straight >= 0)
                {
                    var s = _solver.Straights[straight];
                    overlay.Path(new[] { s.StartPoint, s.EndPoint }, width + 0.7f, Overlay.Green);
                }
            }
        }

        protected override void OnReset()
        {
            _solver = null;
            _selected = 0;
            _firstSwitched = false;
            _secondSwitched = false;
            _lastClick = -1f;
            _lastSecondaryClick = -1f;
        }

        private void SwitchSide(int index)
        {
            if (index == 0)
                _firstSwitched = !_firstSwitched;
            else if (index == LastArc)
                _secondSwitched = !_secondSwitched;
            _solver.SwitchSide(index);
            MarkDirty();
        }

        private void ChangeRadius(float delta, bool all)
        {
            for (var i = 0; i < _solver.Arcs.Count; i++)
            {
                if (all || i == _selected)
                    _solver.SetRadius(i, _solver.Arcs[i].Radius + delta);
            }

            MarkDirty();
        }

        /// <summary>Only the edge circles have an offset along their guide.</summary>
        private void ChangeOffset(float delta)
        {
            if (_selected == 0)
                _solver.FirstOffset += delta;
            else if (_selected == LastArc)
                _solver.SecondOffset += delta;
            MarkDirty();
        }

        private UnityEngine.Color IdleColor(bool selected) => Result != SolveResult.Ok ? Overlay.Red : selected ? Overlay.Yellow : Overlay.Gray;
    }
}
