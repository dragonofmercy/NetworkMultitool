using Unity.Mathematics;

namespace NetworkMultitool.Geometry
{
    /// <summary>A position with its forward tangent (flat and normalized). InDirection is the tangent a course arrives with; it differs from Direction only at a corner.</summary>
    public readonly struct Point
    {
        public float3 Position { get; }
        public float3 Direction { get; }
        public float3 InDirection { get; }

        public Point(float3 position, float3 direction) : this(position, direction, direction)
        {
        }

        public Point(float3 position, float3 direction, float3 inDirection)
        {
            Position = position;
            Direction = direction;
            InDirection = inDirection;
        }

        /// <summary>The same point on the path run the other way: tangents negated, and in/out swapped.</summary>
        public Point Reversed() => new Point(Position, -InDirection, -Direction);

        /// <summary>The same directions moved sideways by shift metres: positive goes right of Direction, negative left. Direction must be flat (as everywhere in Point).</summary>
        public Point Shifted(float shift) => new Point(Position + Vec.Turn90(Direction, true) * shift, Direction, InDirection);
    }
}
