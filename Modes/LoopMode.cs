using System.Collections.Generic;
using NetworkMultitool.Geometry;
using NetworkMultitool.Tool;
using Unity.Entities;
using Unity.Mathematics;
using static NetworkMultitool.Localization.Texts;

namespace NetworkMultitool.Modes
{
    /// <summary>Connects two segments with a single arc, direct or looping (port of CS1 CreateLoopMode).</summary>
    public sealed class LoopMode : CreateModeBase
    {
        private LoopSolver _solver;
        private Point _start;
        private Point _end;
        private float _width;

        public override ToolModeId Id => ToolModeId.Loop;

        protected override string ExtraHints => $"{T("LoopHintDrag")}\n{T("HintSnap")}\n{T("HintRadius", KeyNames.Of(ToolAction.RadiusUp), KeyNames.Of(ToolAction.RadiusDown))}\n{T("LoopHintSide", KeyNames.Of(ToolAction.Side))}\n";

        protected override SolveResult Solve(ToolContext ctx, List<Point> points)
        {
            if (_solver == null || !_start.Equals(StartPoint) || !_end.Equals(EndPoint) || _width != NewWidth(ctx))
            {
                var old = _solver;
                _start = StartPoint;
                _end = EndPoint;
                _width = NewWidth(ctx);
                _solver = new LoopSolver(StartPoint, EndPoint, _width, 5000f);
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
            Dragging = Grab(ctx);
            return Dragging != Drag.None;
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
                return;

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
        }

        protected override void OnDrag(ToolContext ctx)
        {
            if (Dragging == Drag.Center)
                _solver.SetCenter(ctx.MousePosition);
            else
                _solver.SetEdge(ctx.MousePosition);
            if (ctx.ShiftHeld)
                _solver.Radius = math.round(_solver.Radius);
        }

        protected override void RenderShape(ToolContext ctx, Overlay overlay)
        {
            if (_solver == null || Second == Entity.Null || Result != SolveResult.Ok)
                return;
            DrawHandles(ctx, overlay, _solver.Arc, Overlay.Gray, Dragging != Drag.None ? Dragging : Grab(ctx));
        }

        protected override void OnReset() => _solver = null;
    }
}
