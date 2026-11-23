using System.Collections.Generic;
using Unity.Mathematics;

namespace NetworkMultitool.Geometry
{
    /// <summary>A chain of arcs joined by common tangents; the first and last are each tangent to one guide ray, the middle ones are placed freely.
    /// Port of CS1 BaseCreateConnectionMode / CreateConnectionMode (no snapping).</summary>
    public sealed class ConnectionSolver
    {
        private const float MaxOffset = 5000f;
        private const float DefaultRadius = 50f;

        private readonly Point start;
        private readonly Point end;
        private readonly float minRadius;
        private readonly float maxRadius;
        private readonly float height;
        private readonly List<Arc> arcs = new List<Arc>();
        private readonly List<Straight> straights = new List<Straight>();

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
            arcs.Add(new Arc());
            arcs.Add(new Arc());
            FirstRadius = DefaultRadius;
            SecondRadius = DefaultRadius;
            SetSides();
        }

        public IReadOnlyList<Arc> Arcs => arcs;

        /// <summary>One more than the arcs: from the start node, between consecutive arcs, to the end node. Filled by <see cref="Solve"/>.</summary>
        public IReadOnlyList<Straight> Straights => straights;

        public Arc First => arcs[0];
        public Arc Second => arcs[arcs.Count - 1];
        private int Last => arcs.Count - 1;

        public float FirstRadius
        {
            get => First.Radius;
            set => First.Radius = Clamp(value);
        }

        public float SecondRadius
        {
            get => Second.Radius;
            set => Second.Radius = Clamp(value);
        }

        /// <summary>Distance along the start guide where the first arc begins (CS1 EdgeCircle.Offset).</summary>
        public float FirstOffset
        {
            get => firstOffset;
            set => firstOffset = math.clamp(value, 0f, MaxOffset);
        }

        /// <summary>Distance along the end guide where the last arc ends.</summary>
        public float SecondOffset
        {
            get => secondOffset;
            set => secondOffset = math.clamp(value, 0f, MaxOffset);
        }

        public bool IsMiddle(int index) => index > 0 && index < Last;

        public void SetRadius(int index, float radius) => arcs[index].Radius = Clamp(radius);

        private float Clamp(float radius) => math.clamp(radius, minRadius, maxRadius);

        /// <summary>Flips one arc to the other turning sense.</summary>
        public void SwitchSide(int index)
        {
            var arc = arcs[index];
            arc.Turn = arc.Turn == Turn.Right ? Turn.Left : Turn.Right;
        }

        /// <summary>Edge arcs slide along their guide so the center follows the position (radius kept); a middle arc is moved there.</summary>
        public void DragCenter(int index, float3 position)
        {
            if (IsMiddle(index))
            {
                arcs[index].Center = position;
                return;
            }

            var first = index == 0;
            var guide = first ? start : end;
            var offset = Vec.DotXZ(position - guide.Position, guide.Direction);
            if (first)
                FirstOffset = offset;
            else
                SecondOffset = offset;
        }

        /// <summary>Sets the radius so the circle passes through the position; an edge circle stays tangent to its guide at the current offset.</summary>
        public void DragEdge(int index, float3 position)
        {
            if (IsMiddle(index))
            {
                SetRadius(index, math.distance(Vec.Flat(position), Vec.Flat(arcs[index].Center)));
                return;
            }

            var first = index == 0;
            var guide = first ? start : end;
            // Same normal as the centers in Solve: guide point + normal * radius.
            var normal = first ? Vec.Turn90(start.Direction, First.Turn == Turn.Right) : -Vec.Turn90(end.Direction, Second.Turn == Turn.Right);
            var q = Vec.Flat(position - guide.Position - guide.Direction * (first ? firstOffset : secondOffset));
            var side = Vec.DotXZ(q, normal);
            // The mouse is on the other side of the guide: no circle on this side reaches it.
            if (side <= 0f)
                return;

            SetRadius(index, math.lengthsq(q) / (2f * side));
        }

        /// <summary>CS1 double click on a straight: a new free arc centered at the position, inserted where that straight was. Returns its index.</summary>
        public int InsertArc(int straight, float3 position)
        {
            var arc = new Arc { Height = height, Radius = Clamp(DefaultRadius), Turn = Turn.Right };
            arc.Center = position;
            arcs.Insert(straight, arc);
            return straight;
        }

        /// <summary>Removes a middle arc; the edge arcs stay.</summary>
        public void RemoveArc(int index)
        {
            if (IsMiddle(index))
                arcs.RemoveAt(index);
        }

        /// <summary>Takes over the user settings of a solver built for other end points: radii, offsets and the middle arcs.</summary>
        public void CopyFrom(ConnectionSolver old)
        {
            FirstRadius = old.FirstRadius;
            SecondRadius = old.SecondRadius;
            FirstOffset = old.FirstOffset;
            SecondOffset = old.SecondOffset;
            for (var i = 1; i < old.Last; i++)
                arcs.Insert(i, old.arcs[i]);
        }

        /// <summary>Index of the straight between two arcs (never the guide ones) passing within halfWidth of the position, or -1.</summary>
        public int StraightAt(float3 position, float halfWidth)
        {
            var p = Vec.Flat(position);
            for (var i = 1; i < straights.Count - 1; i++)
            {
                var s = straights[i];
                if (s.IsShort)
                    continue;
                var a = Vec.Flat(s.Start);
                var along = math.dot(p - a, s.Direction);
                if (along >= 0f && along <= math.distance(a, Vec.Flat(s.End)) && math.distance(p, a + s.Direction * along) <= halfWidth)
                    return i;
            }

            return -1;
        }

        public SolveResult Solve(float maxSegmentLength, List<Point> points)
        {
            points.Clear();
            var startPos = Vec.WithY(start.Position, height);
            var endPos = Vec.WithY(end.Position, height);

            foreach (var arc in arcs)
            {
                arc.Height = height;
                arc.MinRadius = minRadius;
                arc.MaxRadius = maxRadius;
                arc.IsCorrect = true;
            }

            // EdgeCircle.Calculate + CenterPos getter: the guide is tangent to the arc at node + dir * offset.
            First.StartRadiusDir = -Vec.Turn90(start.Direction, First.Turn == Turn.Right);
            First.Center = startPos + start.Direction * firstOffset - First.StartRadiusDir * First.Radius;
            Second.EndRadiusDir = Vec.Turn90(end.Direction, Second.Turn == Turn.Right);
            Second.Center = endPos + end.Direction * secondOffset - Second.EndRadiusDir * Second.Radius;

            for (var i = 1; i < arcs.Count; i++)
            {
                var isCorrect = Arc.CheckRadii(arcs[i - 1], arcs[i]);
                arcs[i - 1].IsCorrect = arcs[i - 1].IsCorrect && isCorrect;
                arcs[i].IsCorrect = arcs[i].IsCorrect && isCorrect;
            }

            straights.Clear();
            straights.Add(new Straight(startPos, First.StartPos));
            for (var i = 1; i < arcs.Count; i++)
            {
                Arc.Connect(arcs[i - 1], arcs[i]);
                straights.Add(Arc.GetStraight(arcs[i - 1], arcs[i], height));
            }

            straights.Add(new Straight(Second.EndPos, endPos));

            foreach (var arc in arcs)
            {
                if (!arc.IsCorrect)
                    return SolveResult.WrongShape;
            }

            // CS1 GetParts with every circle correct: straight 0, arc 0, straight 1, ..., arc n-1, straight n.
            points.Add(new Point(startPos, start.Direction));
            for (var j = 0; j < straights.Count; j++)
            {
                var straight = straights[j];
                if (!straight.IsShort)
                    points.AddRange(straight.Parts(maxSegmentLength));
                else if (j != 0 && j != straights.Count - 1)
                {
                    if (!arcs[j - 1].IsShort && !arcs[j].IsShort)
                        points.Add(straight.MiddlePoint);
                    else if (!arcs[j - 1].IsShort)
                        points.Add(straight.StartPoint);
                    else if (!arcs[j].IsShort)
                        points.Add(straight.EndPoint);
                }

                if (j < arcs.Count)
                    points.AddRange(arcs[j].GetParts(straight, straights[j + 1], maxSegmentLength));
            }

            points.Add(new Point(endPos, -end.Direction));
            return SolveResult.Ok;
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
