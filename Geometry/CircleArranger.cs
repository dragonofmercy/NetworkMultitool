using Unity.Mathematics;

namespace NetworkMultitool.Geometry
{
    public static class CircleArranger
    {
        /// <summary>Circle through the first, middle and last positions (radius in XZ, center y = average y). False when they are (almost) collinear.</summary>
        public static bool TryFit(float3[] positions, out float3 center, out float radius)
        {
            center = float3.zero;
            radius = 0f;
            var a = positions[0];
            var b = positions[positions.Length / 2];
            var c = positions[positions.Length - 1];
            var ab = b - a;
            var ac = c - a;
            var cross = Vec.CrossXZ(ab, ac);
            if (math.abs(cross) < 1e-3f * math.length(Vec.Flat(ab)) * math.length(Vec.Flat(ac)) || math.abs(cross) < 1e-9f)
            {
                return false;
            }
            var d = 2f * cross;
            var ab2 = Vec.DotXZ(ab, ab);
            var ac2 = Vec.DotXZ(ac, ac);
            var ux = (ac.z * ab2 - ab.z * ac2) / d;
            var uz = (ab.x * ac2 - ac.x * ab2) / d;
            center = new float3(a.x + ux, (a.y + b.y + c.y) / 3f, a.z + uz);
            radius = math.sqrt(ux * ux + uz * uz);
            return true;
        }

        public static float[] Angles(float3[] positions, float3 center)
        {
            var angles = new float[positions.Length];
            for (var i = 0; i < positions.Length; i += 1)
            {
                angles[i] = math.atan2(positions[i].z - center.z, positions[i].x - center.x);
            }
            return angles;
        }

        /// <summary>Puts every point on the circle, either radially projected or evenly spread between the first and last angle in chain order. Heights are kept.</summary>
        public static float3[] Distribute(float3[] positions, float3 center, float radius, bool evenly)
        {
            var count = positions.Length;
            var result = new float3[count];
            if (!evenly)
            {
                for (var i = 0; i < count; i += 1)
                {
                    result[i] = Vec.WithY(center + Vec.NormalizeXZ(positions[i] - center) * radius, positions[i].y);
                }
                return result;
            }

            var angles = Angles(positions, center);
            var sweep = 0f;
            for (var i = 1; i < count; i += 1)
            {
                var delta = angles[i] - angles[i - 1];
                sweep += delta - 2f * math.PI * math.round(delta / (2f * math.PI));
            }
            for (var i = 0; i < count; i += 1)
            {
                var angle = angles[0] + sweep * i / math.max(count - 1, 1);
                result[i] = new float3(center.x + radius * math.cos(angle), positions[i].y, center.z + radius * math.sin(angle));
            }
            return result;
        }
    }
}
