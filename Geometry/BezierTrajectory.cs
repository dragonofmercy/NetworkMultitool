using Colossal.Mathematics;
using Unity.Mathematics;

namespace NetworkMultitool.Geometry
{
    public sealed class BezierTrajectory : ITrajectory
    {
        private const int Samples = 64;

        // cumulative[i] = arc length from t = 0 to t = i / Samples (polyline approximation)
        private readonly float[] cumulative = new float[Samples + 1];

        public BezierTrajectory(Bezier4x3 bezier)
        {
            Bezier = bezier;
            Length = MathUtils.Length(bezier);
            var prev = bezier.a;
            for (var i = 1; i <= Samples; i += 1)
            {
                var p = MathUtils.Position(bezier, (float)i / Samples);
                cumulative[i] = cumulative[i - 1] + math.distance(prev, p);
                prev = p;
            }
        }

        /// <summary>Fits a cubic with handles at distance(start, end) / 3; a zero direction gives a straight-line handle. endDir is the forward tangent at end.</summary>
        public BezierTrajectory(float3 start, float3 startDir, float3 end, float3 endDir) : this(Fit(start, startDir, end, endDir)) { }

        public Bezier4x3 Bezier { get; }

        public float Length { get; }

        public float3 Position(float t) => MathUtils.Position(Bezier, t);

        public float3 Tangent(float t)
        {
            var v = MathUtils.Tangent(Bezier, t);
            var len = math.length(v);
            return len < 1e-6f ? float3.zero : v / len;
        }

        public float Travel(float fromT, float distance)
        {
            var total = cumulative[Samples];
            if (total <= 0f)
            {
                return math.clamp(fromT, 0f, 1f);
            }
            var target = math.clamp(ToLength(math.clamp(fromT, 0f, 1f)) + distance, 0f, total);
            return ToT(target);
        }

        private float ToLength(float t)
        {
            var x = t * Samples;
            var i = math.min((int)x, Samples - 1);
            return math.lerp(cumulative[i], cumulative[i + 1], x - i);
        }

        private float ToT(float length)
        {
            var lo = 0;
            var hi = Samples;
            while (hi - lo > 1)
            {
                var mid = (lo + hi) / 2;
                if (cumulative[mid] <= length)
                {
                    lo = mid;
                }
                else
                {
                    hi = mid;
                }
            }
            var seg = cumulative[lo + 1] - cumulative[lo];
            var frac = seg > 0f ? (length - cumulative[lo]) / seg : 0f;
            return math.clamp((lo + frac) / Samples, 0f, 1f);
        }

        private static Bezier4x3 Fit(float3 start, float3 startDir, float3 end, float3 endDir)
        {
            var d = math.distance(start, end) / 3f;
            var sd = math.normalizesafe(startDir);
            var ed = math.normalizesafe(endDir);
            return new Bezier4x3
            {
                a = start,
                b = sd.Equals(float3.zero) ? start + (end - start) / 3f : start + sd * d,
                c = ed.Equals(float3.zero) ? end - (end - start) / 3f : end - ed * d,
                d = end,
            };
        }
    }
}
