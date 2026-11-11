using System.Collections.Generic;
using Unity.Mathematics;

namespace NetworkMultitool.Geometry
{
    /// <summary>Straight segment between two arcs; port of CS1 BaseCreate.Straight without labels.</summary>
    public sealed class Straight
    {
        public float3 Start { get; }
        public float3 End { get; }
        public float3 Direction { get; }
        public float Length { get; }
        public bool IsShort => Length < 8f;

        public Point StartPoint => new Point(Start, Direction);
        public Point MiddlePoint => new Point(math.lerp(Start, End, 0.5f), Direction);
        public Point EndPoint => new Point(End, Direction);

        public Straight(float3 start, float3 end)
        {
            Start = start;
            End = end;
            Direction = Vec.NormalizeXZ(end - start);
            Length = math.distance(start, end);
        }

        /// <summary>Inner points only, evenly spaced, at most maxSegmentLength apart.</summary>
        public IEnumerable<Point> Parts(float maxSegmentLength)
        {
            var count = (int)math.ceil(Length / maxSegmentLength);
            for (var i = 1; i < count; i++)
                yield return new Point(math.lerp(Start, End, (float)i / count), Direction);
        }
    }
}
