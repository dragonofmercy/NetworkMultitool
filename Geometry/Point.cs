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
    }
}
