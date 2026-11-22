using System.Collections.Generic;
using Colossal.Mathematics;
using Game.Common;
using Game.Net;
using Game.Tools;
using NetworkMultitool.Geometry;
using Unity.Entities;
using Unity.Mathematics;

namespace NetworkMultitool.Net
{
    /// <summary>
    /// Writes definition entities (<see cref="CreationDefinition"/> + <see cref="Updated"/> + <see cref="NetCourse"/>)
    /// consumed by the game's net generation systems, which build Temp previews applied by the tool.
    /// </summary>
    public sealed class CourseEmitter
    {
        private const float StraightTolerance = 1e-3f;

        private readonly EntityCommandBuffer _ecb;
        private readonly NetQuery _net;
        private readonly Entity _prefab;

        public CourseEmitter(EntityCommandBuffer ecb, NetQuery net, Entity prefab)
        {
            _ecb = ecb;
            _net = net;
            _prefab = prefab;
        }

        /// <summary>One course per pair of consecutive points (pattern NetToolSystem.CreateStraightLine).</summary>
        /// <remarks>
        /// CoursePosFlags.FreeHeight lets the game overwrite the position height with terrain + m_Elevation (CourseSplitSystem.InitializeCoursePos).
        /// Without it the heights are kept, and <paramref name="terrainHeight"/> gives inner points their real elevation so bridges and pillars match.
        /// </remarks>
        public void Chain(Entity startNode, Entity endNode, IReadOnlyList<Point> points, float2 startElevation, float2 endElevation, bool followTerrain, System.Func<float3, float> terrainHeight = null)
        {
            var last = points.Count - 1;
            for (var i = 0; i < last; i += 1)
            {
                var p0 = points[i];
                var p1 = points[i + 1];
                var curve = Fit(p0, p1);
                // Flat directions leave b.y = a.y and c.y = d.y: restore a constant slope.
                curve.b.y = math.lerp(curve.a.y, curve.d.y, 1f / 3f);
                curve.c.y = math.lerp(curve.a.y, curve.d.y, 2f / 3f);

                float2 ElevationAt(int index, float3 position) => terrainHeight == null || index == 0 || index == last ? Lerp(startElevation, endElevation, index, last) : new float2(position.y - terrainHeight(position));

                var common = CoursePosFlags.IsLeft | CoursePosFlags.IsRight | (followTerrain ? CoursePosFlags.FreeHeight : 0);
                var start = Pos(i == 0 ? startNode : Entity.Null, p0.Position, MathUtils.StartTangent(curve), 0f, ElevationAt(i, p0.Position), common | (i == 0 ? CoursePosFlags.IsFirst : 0));
                var end = Pos(i + 1 == last ? endNode : Entity.Null, p1.Position, MathUtils.EndTangent(curve), 1f, ElevationAt(i + 1, p1.Position), common | (i + 1 == last ? CoursePosFlags.IsLast : 0));

                Emit(new CreationDefinition { m_Prefab = _prefab, m_Flags = CreationFlags.SubElevation, m_RandomSeed = i }, new NetCourse
                {
                    m_Curve = curve,
                    m_Length = MathUtils.Length(curve),
                    m_FixedIndex = -1,
                    m_Elevation = (start.m_Elevation + end.m_Elevation) * 0.5f,
                    m_StartPosition = start,
                    m_EndPosition = end,
                });
            }
        }

        /// <summary>Zero-length course on the edge that the game turns into a new node (flags of NetworkTools NetCourseEmitter.EmitSplit).</summary>
        public void Split(Entity edge, float t, float3 position)
        {
            var pos = Pos(edge, position, MathUtils.Tangent(_net.Curve(edge), t), 0f, float2.zero, CoursePosFlags.IsFirst | CoursePosFlags.IsLast | CoursePosFlags.IsLeft | CoursePosFlags.IsRight);
            pos.m_SplitPosition = t;
            var end = pos;
            end.m_CourseDelta = 1f;

            Emit(new CreationDefinition { m_Prefab = _net.Prefab(edge), m_Flags = CreationFlags.Construction }, new NetCourse
            {
                m_Curve = new Bezier4x3(position, position, position, position),
                m_FixedIndex = -1,
                m_StartPosition = pos,
                m_EndPosition = end,
            });
        }

        /// <summary>Deletion definition for an existing edge (mirrors BulldozeToolSystem.AddEntity: no m_Prefab, no CoursePos flags).</summary>
        public void Delete(Entity edge)
        {
            var curve = _net.Curve(edge);
            var ends = _net.Ends(edge);
            var course = Course(curve, ends.m_Start, curve.a, ends.m_End, curve.d);
            course.m_FixedIndex = _net.FixedIndex(edge);
            Emit(new CreationDefinition { m_Original = edge, m_Flags = CreationFlags.Delete }, course);
        }

        private void Emit(CreationDefinition definition, NetCourse course)
        {
            var entity = _ecb.CreateEntity();
            _ecb.AddComponent(entity, definition);
            _ecb.AddComponent(entity, default(Updated));
            _ecb.AddComponent(entity, course);
        }

        private static NetCourse Course(Bezier4x3 curve, Entity startEntity, float3 startPosition, Entity endEntity, float3 endPosition) => new NetCourse
        {
            m_Curve = curve,
            m_Length = MathUtils.Length(curve),
            m_FixedIndex = -1,
            m_StartPosition = new CoursePos { m_Entity = startEntity, m_Position = startPosition, m_Rotation = NetUtils.GetNodeRotation(MathUtils.StartTangent(curve)), m_CourseDelta = 0f },
            m_EndPosition = new CoursePos { m_Entity = endEntity, m_Position = endPosition, m_Rotation = NetUtils.GetNodeRotation(MathUtils.EndTangent(curve)), m_CourseDelta = 1f },
        };

        private static CoursePos Pos(Entity entity, float3 position, float3 tangent, float courseDelta, float2 elevation, CoursePosFlags flags) => new CoursePos
        {
            m_Entity = entity,
            m_Position = position,
            m_Rotation = NetUtils.GetNodeRotation(tangent),
            m_Elevation = elevation,
            m_CourseDelta = courseDelta,
            m_SplitPosition = 0f,
            m_Flags = flags,
            m_ParentMesh = -1,
        };

        private static float2 Lerp(float2 from, float2 to, int index, int count) => count > 0 ? math.lerp(from, to, (float)index / count) : from;

        /// <summary>Leaves p0 along d0 and reaches p1 along d1 (FitCurve's end tangent is the forward one at the end).</summary>
        internal static Bezier4x3 Fit(Point p0, Point p1)
        {
            var d0 = p0.Direction;
            var d1 = p1.Direction;
            var straight = math.abs(Vec.Angle(d0, d1)) < StraightTolerance && math.abs(Vec.Angle(d0, p1.Position - p0.Position)) < StraightTolerance;
            return straight ? NetUtils.StraightCurve(p0.Position, p1.Position) : NetUtils.FitCurve(p0.Position, d0, d1, p1.Position);
        }
    }
}
