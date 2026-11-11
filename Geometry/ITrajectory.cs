using Unity.Mathematics;

namespace NetworkMultitool.Geometry
{
    public interface ITrajectory
    {
        float Length { get; }
        float3 Position(float t);
        /// <summary>Normalized forward direction at t (float3.zero if degenerate).</summary>
        float3 Tangent(float t);

        /// <summary>Returns the t reached after walking <paramref name="distance"/> meters of arc from <paramref name="fromT"/>, clamped to [0, 1].</summary>
        float Travel(float fromT, float distance);
    }
}
