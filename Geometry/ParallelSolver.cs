using System.Collections.Generic;
using Colossal.Mathematics;
using Unity.Mathematics;

namespace NetworkMultitool.Geometry
{
    /// <summary>Offsets a chain of edges sideways: port of CS1 CreateParallelMode.Calculate, with line intersection instead of bezier intersection.</summary>
    public static class ParallelSolver
    {
        private const float MergeDistance = 8f;

        // Below this turn the two offset lines are nearly parallel: their intersection is ill-conditioned (float noise far from the origin slides it tens of meters), and the mean is within centimeters.
        private const float SmoothAngle = 0.05f;

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
                var smooth = math.abs(Vec.Angle(db, df)) < SmoothAngle;
                // CS1 accepts a hit anywhere on the offset segments.
                if (!smooth && Vec.IntersectXZ(b, db, f, df, out var t1, out var t2) && math.abs(t1) <= FlatChord(incoming) && math.abs(t2) <= FlatChord(outgoing))
                {
                    var hit = (b + db * t1 + f + df * t2) / 2f;
                    result.Add(new Point(Vec.WithY(hit, outgoing.a.y + height), df, db)); // separate tangents, or the new road wiggles before the corner
                }
                else if (math.distance(Vec.Flat(b), Vec.Flat(f)) < MergeDistance)
                {
                    var mergedDir = Vec.NormalizeXZ(db + df);
                    if (math.lengthsq(mergedDir) < 1e-6f)
                        mergedDir = db; // antiparallel tangents (hairpin) sum to zero
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

        /// <summary>Horizontal distance from position to the closest point of the chain, positive on the left side (the side Solve takes with left = true). Edges are oriented along the chain, as for <see cref="Solve"/>.</summary>
        public static float SignedOffset(IReadOnlyList<Bezier4x3> edges, float3 position)
        {
            var signed = float.MaxValue;
            for (var i = 0; i < edges.Count; i++)
            {
                var offset = SignedOffset(edges[i], position);
                if (math.abs(offset) < math.abs(signed))
                    signed = offset;
            }

            return edges.Count > 0 ? signed : 0f;
        }

        /// <summary><see cref="SignedOffset(IReadOnlyList{Bezier4x3}, float3)"/> for a single edge.</summary>
        public static float SignedOffset(Bezier4x3 edge, float3 position)
        {
            var distance = MathUtils.Distance(edge.xz, position.xz, out var t);
            var closest = MathUtils.Position(edge, t);
            return Vec.DotXZ(Vec.Turn90(Vec.NormalizeXZ(MathUtils.Tangent(edge, t)), false), position - closest) >= 0f ? distance : -distance;
        }

        private static float3 Offset(float3 position, float3 dir, float shift, bool left, float height) => position + Vec.Turn90(dir, !left) * shift + new float3(0f, height, 0f);

        private static float FlatChord(Bezier4x3 edge) => math.distance(Vec.Flat(edge.a), Vec.Flat(edge.d));
    }
}
