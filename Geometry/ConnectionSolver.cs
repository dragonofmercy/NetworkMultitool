using System.Collections.Generic;
using Unity.Mathematics;

namespace NetworkMultitool.Geometry
{
    /// <summary>Two arcs, each tangent to one guide ray, joined by a common tangent; port of CS1 BaseCreateConnectionMode with its two EdgeCircles (no snapping).</summary>
    public sealed class ConnectionSolver
    {
        private const float MaxOffset = 5000f;

        private readonly Point start;
        private readonly Point end;
        private readonly float minRadius;
        private readonly float maxRadius;
        private readonly float height;

        private float firstRadius;
        private float secondRadius;
        private float firstOffset;
        private float secondOffset;

        /// <summary>Directions point into the connection from each attach node.</summary>
        public ConnectionSolver(Point start, Point end, float minRadius, float maxRadius)
        {
            this.start = start;
            this.end = end;
            this.minRadius = minRadius;
            this.maxRadius = maxRadius;
            height = (start.Position.y + end.Position.y) / 2f;
            FirstRadius = 50f;
            SecondRadius = 50f;
            SetSides();
        }

        public Arc First { get; } = new Arc();
        public Arc Second { get; } = new Arc();
        public Straight StartStraight { get; private set; }
        public Straight MiddleStraight { get; private set; }
        public Straight EndStraight { get; private set; }

        public float FirstRadius
        {
            get => firstRadius;
            set => firstRadius = math.clamp(value, minRadius, maxRadius);
        }

        public float SecondRadius
        {
            get => secondRadius;
            set => secondRadius = math.clamp(value, minRadius, maxRadius);
        }

        /// <summary>Distance along the start guide where the first arc begins (CS1 EdgeCircle.Offset).</summary>
        public float FirstOffset
        {
            get => firstOffset;
            set => firstOffset = math.clamp(value, 0f, MaxOffset);
        }

        /// <summary>Distance along the end guide where the second arc ends.</summary>
        public float SecondOffset
        {
            get => secondOffset;
            set => secondOffset = math.clamp(value, 0f, MaxOffset);
        }

        /// <summary>Flips both arcs to the other turning sense.</summary>
        public void SwitchSide()
        {
            First.Turn = First.Turn == Turn.Right ? Turn.Left : Turn.Right;
            Second.Turn = Second.Turn == Turn.Right ? Turn.Left : Turn.Right;
        }

        /// <summary>Slides the arc along its guide so its center follows a dragged position; the radius is kept.</summary>
        public void DragCenter(bool first, float3 position)
        {
            var guide = first ? start : end;
            var offset = Vec.DotXZ(position - guide.Position, guide.Direction);
            if (first)
                FirstOffset = offset;
            else
                SecondOffset = offset;
        }

        /// <summary>Sets the radius so the circle, still tangent to its guide at the current offset, passes through a dragged position.</summary>
        public void DragEdge(bool first, float3 position)
        {
            var guide = first ? start : end;
            // Same normal as the centers in Solve: guide point + normal * radius.
            var normal = first ? Vec.Turn90(start.Direction, First.Turn == Turn.Right) : -Vec.Turn90(end.Direction, Second.Turn == Turn.Right);
            var q = Vec.Flat(position - guide.Position - guide.Direction * (first ? firstOffset : secondOffset));
            var side = Vec.DotXZ(q, normal);
            // The mouse is on the other side of the guide: no circle on this side reaches it.
            if (side <= 0f)
                return;

            var radius = math.lengthsq(q) / (2f * side);
            if (first)
                FirstRadius = radius;
            else
                SecondRadius = radius;
        }

        public SolveResult Solve(float maxSegmentLength, List<Point> points)
        {
            points.Clear();
            var startPos = Vec.WithY(start.Position, height);
            var endPos = Vec.WithY(end.Position, height);

            // EdgeCircle.Calculate + CenterPos getter: the guide is tangent to the arc at node + dir * offset.
            Setup(First, firstRadius);
            First.StartRadiusDir = -Vec.Turn90(start.Direction, First.Turn == Turn.Right);
            First.Center = startPos + start.Direction * firstOffset - First.StartRadiusDir * firstRadius;

            Setup(Second, secondRadius);
            Second.EndRadiusDir = Vec.Turn90(end.Direction, Second.Turn == Turn.Right);
            Second.Center = endPos + end.Direction * secondOffset - Second.EndRadiusDir * secondRadius;

            var isCorrect = Arc.CheckRadii(First, Second);
            First.IsCorrect = isCorrect;
            Second.IsCorrect = isCorrect;

            StartStraight = new Straight(startPos, First.StartPos);
            Arc.Connect(First, Second);
            MiddleStraight = Arc.GetStraight(First, Second, height);
            EndStraight = new Straight(Second.EndPos, endPos);

            if (!First.IsCorrect || !Second.IsCorrect)
                return SolveResult.WrongShape;

            // GetParts with every circle correct.
            points.Add(new Point(startPos, start.Direction));
            if (!StartStraight.IsShort)
                points.AddRange(StartStraight.Parts(maxSegmentLength));
            points.AddRange(First.GetParts(StartStraight, MiddleStraight, maxSegmentLength));
            if (!MiddleStraight.IsShort)
                points.AddRange(MiddleStraight.Parts(maxSegmentLength));
            else if (!First.IsShort && !Second.IsShort)
                points.Add(MiddleStraight.MiddlePoint);
            else if (!First.IsShort)
                points.Add(MiddleStraight.StartPoint);
            else if (!Second.IsShort)
                points.Add(MiddleStraight.EndPoint);
            points.AddRange(Second.GetParts(MiddleStraight, EndStraight, maxSegmentLength));
            if (!EndStraight.IsShort)
                points.AddRange(EndStraight.Parts(maxSegmentLength));
            points.Add(new Point(endPos, -end.Direction));
            return SolveResult.Ok;
        }

        private void Setup(Arc arc, float radius)
        {
            arc.Height = height;
            arc.MinRadius = minRadius;
            arc.MaxRadius = maxRadius;
            arc.Radius = radius;
        }

        // Port of EdgeCircle.GetSides; CS1 CrossXZ is the negated Vec cross, hence the <= 0 tests.
        private void SetSides()
        {
            var connect = Vec.NormalizeXZ(end.Position - start.Position);
            var firstDir = Vec.CrossXZ(start.Direction, connect) <= 0f;
            var lastDir = Vec.CrossXZ(end.Direction, -connect) <= 0f;
            var firstDot = Vec.DotXZ(start.Direction, connect) >= 0f;
            var lastDot = Vec.DotXZ(end.Direction, -connect) >= 0f;

            if (firstDot != lastDot && firstDir != lastDir)
                lastDir = !lastDir;

            First.Turn = firstDir ? Turn.Right : Turn.Left;
            Second.Turn = lastDir ? Turn.Left : Turn.Right;
        }
    }
}
