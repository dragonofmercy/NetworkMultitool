using Unity.Mathematics;

namespace NetworkMultitool.Geometry
{
    public sealed class StraightTrajectory : ITrajectory
    {
        private readonly float3 start;
        private readonly float3 end;

        public StraightTrajectory(float3 start, float3 end)
        {
            this.start = start;
            this.end = end;
            Length = math.distance(start, end);
        }

        public float Length { get; }

        public float3 Position(float t) => math.lerp(start, end, t);

        public float3 Tangent(float t) => math.normalizesafe(end - start);

        public float Travel(float fromT, float distance) => Length > 0f ? math.clamp(fromT + distance / Length, 0f, 1f) : math.clamp(fromT, 0f, 1f);
    }
}
