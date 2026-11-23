using System.Collections.Generic;
using Colossal.Mathematics;
using Unity.Mathematics;

namespace NetworkMultitool.Geometry
{
    /// <summary>Offsets a chain of edges sideways: port of CS1 CreateParallelMode.Calculate, with line intersection instead of bezier intersection.</summary>
    public static class ParallelSolver
    {
        private const float MergeDistance = 8f;

        /// <summary>edges[i] runs from chain node i to chain node i + 1. Points come back in chain order.</summary>
        public static List<Point> Solve(IReadOnlyList<Bezier4x3> edges, float shift, bool left, float height)
        {
            var result = new List<Point>();
            if (edges.Count == 0)
                return result;

            var first = edges[0];
            var startDir = Vec.NormalizeXZ(MathUtils.StartTangent(first));
            result.Add(new Point(Offset(first.a, startDir, shift, left, height), startDir));

            for (var i = 1; i < edges.Count; i++)
            {
                var incoming = edges[i - 1];
                var outgoing = edges[i];
                var db = Vec.NormalizeXZ(MathUtils.EndTangent(incoming));
                var df = Vec.NormalizeXZ(MathUtils.StartTangent(outgoing));
                var b = Offset(incoming.d, db, shift, left, height);
                var f = Offset(outgoing.a, df, shift, left, height);
                var mergedDir = Vec.NormalizeXZ(db + df);
                if (math.lengthsq(mergedDir) < 1e-6f)
                    mergedDir = db; // antiparallel tangents (hairpin) sum to zero

                if (Vec.IntersectXZ(b, db, f, df, out var t1, out var t2) && math.abs(t1) <= FlatChord(incoming) / 2f && math.abs(t2) <= FlatChord(outgoing) / 2f)
                {
                    var hit = (b + db * t1 + f + df * t2) / 2f;
                    result.Add(new Point(Vec.WithY(hit, outgoing.a.y + height), mergedDir));
                }
                else if (math.distance(Vec.Flat(b), Vec.Flat(f)) < MergeDistance)
                {
                    result.Add(new Point((b + f) / 2f, mergedDir));
                }
                else
                {
                    result.Add(new Point(b, db));
                    result.Add(new Point(f, df));
                }
            }

            var last = edges[edges.Count - 1];
            var endDir = Vec.NormalizeXZ(MathUtils.EndTangent(last));
            result.Add(new Point(Offset(last.d, endDir, shift, left, height), endDir));
            return result;
        }

        private static float3 Offset(float3 position, float3 dir, float shift, bool left, float height) => position + Vec.Turn90(dir, !left) * shift + new float3(0f, height, 0f);

        private static float FlatChord(Bezier4x3 edge) => math.distance(Vec.Flat(edge.a), Vec.Flat(edge.d));
    }
}
