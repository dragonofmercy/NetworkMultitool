using System.Collections.Generic;
using Unity.Mathematics;

namespace NetworkMultitool.Geometry
{
    /// <summary>One arc tangent to both guide rays, either inside their corner (direct) or as a loop around it; port of CS1 BaseCreateLoopMode and MiddleCircle.</summary>
    public sealed class LoopSolver
    {
        private readonly Point start;
        private readonly Point end;
        private readonly float minPossibleRadius;
        private readonly float maxPossibleRadius;
        private readonly float height;
        private readonly bool intersects;
        private readonly float startT;
        private readonly float endT;
        private readonly float guideAngle;
        private readonly float3 centerLinePos;
        private readonly float3 centerLineDir;

        private float radius = 50f;
        private float minInsideRadius;
        private float maxInsideRadius;
        private float minOutsideRadius;
        private float maxOutsideRadius;

        /// <summary>Directions point into the connection from each attach node.</summary>
        public LoopSolver(Point start, Point end, float minRadius, float maxRadius)
        {
            this.start = start;
            this.end = end;
            minPossibleRadius = minRadius;
            maxPossibleRadius = maxRadius;
            height = (start.Position.y + end.Position.y) / 2f;

            intersects = Vec.IntersectXZ(start.Position, start.Direction, end.Position, end.Direction, out startT, out endT) && math.abs(startT) <= 5000f && math.abs(endT) <= 5000f;
            guideAngle = Vec.Angle(start.Direction, end.Direction);
            centerLinePos = Vec.WithY((start.Position + start.Direction * startT + end.Position + end.Direction * endT) / 2f, height);
            centerLineDir = Vec.NormalizeXZ(start.Direction + end.Direction);

            CalculateLimits();
        }

        public Arc Arc { get; } = new Arc();
        public Straight StartStraight { get; private set; }
        public Straight EndStraight { get; private set; }

        public bool PossibleDirect => startT >= 0f && endT >= 0f;
        public bool ForceLoop { get; set; }
        public bool IsDirect => PossibleDirect && !ForceLoop;

        public float MinRadius => IsDirect ? minInsideRadius : minOutsideRadius;
        public float MaxRadius => IsDirect ? maxInsideRadius : maxOutsideRadius;

        public float Radius
        {
            get => radius;
            set => radius = math.clamp(value, MinRadius, MaxRadius);
        }

        private float HalfAbsGuideAngle => math.abs(guideAngle) / 2f;
        private Turn Turn => IsDirect == guideAngle >= 0f ? Turn.Right : Turn.Left;

        /// <summary>Port of the MiddleCircle.CenterPos setter: derives the radius (and possibly the loop mode) from a dragged center.</summary>
        public void SetCenter(float3 position)
        {
            if (!intersects)
                return;

            var normal = Vec.Turn90(centerLineDir, true);
            if (!Vec.IntersectXZ(centerLinePos, centerLineDir, position, normal, out var t, out _))
                return;

            var newRadius = t * math.sin(HalfAbsGuideAngle);
            if (PossibleDirect)
            {
                newRadius = math.abs(newRadius);
                if (newRadius < MinRadius)
                    ForceLoop = !ForceLoop;
                Radius = newRadius;
            }
            else
                Radius = math.max(newRadius, 0f);
        }

        public SolveResult Solve(float maxSegmentLength, List<Point> points)
        {
            points.Clear();
            if (!intersects)
                return SolveResult.NotIntersect;

            CalculateLimits();
            Radius = radius;

            var turn = Turn;
            var distant = radius / math.sin(HalfAbsGuideAngle);
            Arc.Height = height;
            Arc.Turn = turn;
            Arc.MinRadius = MinRadius;
            Arc.MaxRadius = MaxRadius;
            Arc.Radius = radius;
            Arc.Center = centerLinePos + centerLineDir * (IsDirect ? -distant : distant);
            Arc.StartRadiusDir = Vec.Turn90(start.Direction, turn == Turn.Left);
            Arc.EndRadiusDir = Vec.Turn90(end.Direction, turn == Turn.Right);

            var startPos = Vec.WithY(start.Position, height);
            var endPos = Vec.WithY(end.Position, height);
            StartStraight = new Straight(startPos, Arc.StartPos);
            EndStraight = new Straight(Arc.EndPos, endPos);

            if (!Arc.IsCorrect)
                return SolveResult.WrongShape;

            points.Add(new Point(startPos, start.Direction));
            if (!StartStraight.IsShort)
                points.AddRange(StartStraight.Parts(maxSegmentLength));
            points.AddRange(Arc.GetParts(StartStraight, EndStraight, maxSegmentLength));
            if (!EndStraight.IsShort)
                points.AddRange(EndStraight.Parts(maxSegmentLength));
            points.Add(new Point(endPos, -end.Direction));
            return SolveResult.Ok;
        }

        // Port of MiddleCircle.Calculate.
        private void CalculateLimits()
        {
            var tan = math.tan(HalfAbsGuideAngle);
            minInsideRadius = minPossibleRadius;
            maxInsideRadius = tan * math.min(startT, endT);
            minOutsideRadius = math.max(tan * math.max(-math.min(startT, 0f), -math.min(endT, 0f)), minPossibleRadius);
            maxOutsideRadius = math.max(minOutsideRadius + 200f, maxPossibleRadius);
        }
    }
}
