using System.Collections.Generic;
using Unity.Mathematics;

namespace NetworkMultitool.Geometry
{
    /// <summary>Circular arc on the XZ plane; port of CS1 BaseCreate.Circle (geometry only).</summary>
    public sealed class Arc
    {
        public const float MinDelta = 0.01f;

        private float3 center;
        private float height;
        private bool isCorrect = true;

        public float Height
        {
            get => height;
            set
            {
                height = value;
                center.y = value;
            }
        }

        public float3 Center
        {
            get => center;
            set => center = Vec.WithY(value, height);
        }

        public float Radius { get; set; } = 50f;
        public float3 StartRadiusDir { get; set; }
        public float3 EndRadiusDir { get; set; }
        public Turn Turn { get; set; }
        public float MinRadius { get; set; }
        public float MaxRadius { get; set; } = float.MaxValue;

        public bool IsCorrect
        {
            get => isCorrect && MinRadius <= Radius && Radius <= MaxRadius;
            set => isCorrect = value;
        }

        private bool ClockWise => Turn == Turn.Right;

        public float3 StartPos => Center + StartRadiusDir * Radius;
        public float3 EndPos => Center + EndRadiusDir * Radius;
        public float3 StartDir => Vec.Turn90(StartRadiusDir, ClockWise);
        public float3 EndDir => Vec.Turn90(EndRadiusDir, ClockWise);

        /// <summary>Angle swept from start to end in the Turn sense, in [0, 2pi) (0 when start and end coincide, as in CS1).</summary>
        public float Angle
        {
            get
            {
                // Vec.Angle is counter-clockwise positive, so Left keeps it and Right negates it.
                var angle = Vec.Angle(StartDir, EndDir);
                if (ClockWise)
                    angle = -angle;
                return angle < 0f ? angle + 2f * math.PI : angle;
            }
        }

        public float Length => Radius * Angle;
        public bool IsShort => Length < 8f;

        public Point StartPoint => new Point(StartPos, StartDir);
        public Point MiddlePoint => PointAt(Angle / 2f);
        public Point EndPoint => new Point(EndPos, EndDir);

        private Point PointAt(float angle) => new Point(Center + Vec.TurnRad(StartRadiusDir, angle, ClockWise) * Radius, Vec.TurnRad(StartDir, angle, ClockWise));

        /// <summary>Inner points only.</summary>
        public IEnumerable<Point> Parts(float maxSegmentLength)
        {
            var angle = Angle;
            var length = Radius * angle;

            var minByLength = (int)math.ceil(length / 50f);
            var maxByLength = (int)math.ceil(length / maxSegmentLength);
            var maxByAngle = (int)math.ceil(angle / (math.PI / 2f));
            var count = math.max(maxByLength, math.min(minByLength, maxByAngle));

            for (var i = 1; i < count; i++)
                yield return PointAt(angle / count * i);
        }

        public IEnumerable<Point> GetParts(Straight before, Straight after, float maxSegmentLength)
        {
            if (!IsShort)
            {
                if (!before.IsShort)
                    yield return StartPoint;

                foreach (var part in Parts(maxSegmentLength))
                    yield return part;

                if (!after.IsShort)
                    yield return EndPoint;
            }
            else if (!before.IsShort && !after.IsShort)
                yield return MiddlePoint;
        }

        private static float CenterDistance(Arc first, Arc second) => math.length(Vec.Flat(second.Center - first.Center));

        private static float3 CenterDirection(Arc first, Arc second)
        {
            var dir = Vec.NormalizeXZ(second.Center - first.Center);
            // Coinciding centers: pick an arbitrary axis so radius directions stay unit length.
            return math.lengthsq(dir) > 0f ? dir : new float3(1f, 0f, 0f);
        }

        private static float GetDelta(Arc first, Arc second)
        {
            var length = CenterDistance(first, second);
            return first.Turn == second.Turn ? math.abs(math.abs(first.Radius - second.Radius) - length) : math.abs(length - (first.Radius + second.Radius));
        }

        private static float Ratio(float value, float length) => length > 0f ? math.clamp(value / length, -1f, 1f) : 1f;

        /// <summary>Sets first.EndRadiusDir and second.StartRadiusDir so both arcs share a tangent line.</summary>
        public static void Connect(Arc first, Arc second)
        {
            var length = CenterDistance(first, second);
            var direction = CenterDirection(first, second);
            var delta = GetDelta(first, second);

            if (first.Turn == second.Turn)
            {
                if (delta < MinDelta)
                {
                    var dir = first.Radius >= second.Radius ? direction : -direction;
                    first.EndRadiusDir = dir;
                    second.StartRadiusDir = dir;
                }
                else
                {
                    // Ratio is clamped so overlapping circles (CheckRadii false) degrade instead of producing NaN.
                    var deltaAngle = math.asin(Ratio(math.abs(first.Radius - second.Radius), length));
                    first.EndRadiusDir = Vec.TurnRad(direction, math.PI / 2f + (first.Radius >= second.Radius ? -deltaAngle : deltaAngle), !first.ClockWise);
                    second.StartRadiusDir = -Vec.TurnRad(direction, math.PI / 2f + (second.Radius >= first.Radius ? -deltaAngle : deltaAngle), second.ClockWise);
                }
            }
            else
            {
                if (delta < MinDelta)
                {
                    first.EndRadiusDir = direction;
                    second.StartRadiusDir = -direction;
                }
                else
                {
                    var deltaAngle = math.acos(Ratio(first.Radius + second.Radius, length));
                    first.EndRadiusDir = Vec.TurnRad(direction, deltaAngle, !first.ClockWise);
                    second.StartRadiusDir = -Vec.TurnRad(direction, deltaAngle, second.ClockWise);
                }
            }
        }

        public static bool CheckRadii(Arc first, Arc second)
        {
            var length = CenterDistance(first, second);
            if (first.Turn == second.Turn)
                return second.Radius - first.Radius <= length + MinDelta && first.Radius - second.Radius <= length + MinDelta;
            else
                return first.Radius + second.Radius <= length + MinDelta;
        }

        /// <summary>Straight joining first's end to second's start; a 2 m stub across the contact point when they touch.</summary>
        public static Straight GetStraight(Arc first, Arc second, float height)
        {
            var start = first.EndPos;
            var end = second.StartPos;
            if (math.lengthsq(end - start) >= 1f)
                return new Straight(Vec.WithY(start, height), Vec.WithY(end, height));

            var pos = (start + end) / 2f;
            var dir = first.Turn == second.Turn
                ? Vec.Turn90(Vec.NormalizeXZ(first.EndRadiusDir + second.StartRadiusDir), first.ClockWise)
                : Vec.Turn90(CenterDirection(first, second), first.ClockWise);
            return new Straight(Vec.WithY(pos - dir, height), Vec.WithY(pos + dir, height));
        }
    }
}
