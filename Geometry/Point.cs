using Unity.Mathematics;

namespace NetworkMultitool.Geometry
{
    /// <summary>A position with its forward tangent (flat and normalized).</summary>
    public readonly struct Point
    {
        public float3 Position { get; }
        public float3 Direction { get; }

        public Point(float3 position, float3 direction)
        {
            Position = position;
            Direction = direction;
        }
    }
}
