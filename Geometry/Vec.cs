using Colossal.Mathematics;
using Unity.Mathematics;

namespace NetworkMultitool.Geometry
{
    /// <summary>Vector helpers on the XZ plane (Y is height). Counter-clockwise is seen from above with x to the right and z up.</summary>
    public static class Vec
    {
        public static float3 Flat(float3 v) => new float3(v.x, 0f, v.z);

        public static float3 NormalizeXZ(float3 v)
        {
            var len = math.sqrt(v.x * v.x + v.z * v.z);
            return len > 0f ? new float3(v.x / len, 0f, v.z / len) : float3.zero;
        }

        public static float DotXZ(float3 a, float3 b) => a.x * b.x + a.z * b.z;

        public static float CrossXZ(float3 a, float3 b) => a.x * b.z - a.z * b.x;

        public static float3 Turn90(float3 v, bool clockwise) => clockwise ? new float3(v.z, v.y, -v.x) : new float3(-v.z, v.y, v.x);

        public static float3 TurnRad(float3 v, float angle, bool clockwise)
        {
            var a = clockwise ? -angle : angle;
            math.sincos(a, out var s, out var c);
            return new float3(v.x * c - v.z * s, v.y, v.x * s + v.z * c);
        }

        /// <summary>Signed angle in radians in ]-pi, pi]. Same sign convention as the CS1 MathExtention.GetAngle (positive = counter-clockwise).</summary>
        public static float Angle(float3 from, float3 to)
        {
            var a = NormalizeXZ(from);
            var b = NormalizeXZ(to);
            var angle = math.atan2(CrossXZ(a, b), DotXZ(a, b));
            return angle <= -math.PI ? math.PI : angle;
        }

        /// <summary>Intersects p1 + t1 * d1 with p2 + t2 * d2 on the XZ plane.</summary>
        public static bool IntersectXZ(float3 p1, float3 d1, float3 p2, float3 d2, out float t1, out float t2)
        {
            var cross = CrossXZ(d1, d2);
            if (math.abs(cross) < 1e-6f)
            {
                t1 = t2 = 0f;
                return false;
            }
            var diff = p2 - p1;
            t1 = CrossXZ(diff, d2) / cross;
            t2 = CrossXZ(diff, d1) / cross;
            return true;
        }

        /// <summary>Curves fitted from flat tangents keep b.y = a.y and c.y = d.y: spread the height evenly between the ends instead.</summary>
        public static Bezier4x3 ConstantSlope(Bezier4x3 curve)
        {
            curve.b.y = math.lerp(curve.a.y, curve.d.y, 1f / 3f);
            curve.c.y = math.lerp(curve.a.y, curve.d.y, 2f / 3f);
            return curve;
        }

        public static float3 WithY(float3 v, float y) => new float3(v.x, y, v.z);
    }
}
