using System.Collections.Generic;
using Colossal.Mathematics;
using Game.Common;
using Game.Net;
using Game.Prefabs;
using NetworkMultitool.Geometry;
using Unity.Entities;
using Unity.Mathematics;

namespace NetworkMultitool.Net
{
    /// <summary>Read-only helpers over the game's net components.</summary>
    public readonly struct NetQuery : INetGraph<Entity>
    {
        private const float DefaultWidth = 8f;

        private readonly EntityManager _em;

        public NetQuery(EntityManager em)
        {
            _em = em;
        }

        public float3 NodePosition(Entity node) => _em.GetComponentData<Node>(node).m_Position;

        public quaternion NodeRotation(Entity node) => _em.GetComponentData<Node>(node).m_Rotation;

        public int Degree(Entity node) => EdgeList(node).Count;

        /// <summary>
        /// Edges starting or ending at the node, copied so callers may make structural changes while iterating.
        /// The ConnectedEdge buffer also lists edges where the node is only a middle node; those are skipped, as vanilla does.
        /// </summary>
        public IEnumerable<Entity> Edges(Entity node) => EdgeList(node);

        private List<Entity> EdgeList(Entity node)
        {
            var result = new List<Entity>();
            if (!_em.HasBuffer<ConnectedEdge>(node))
                return result;

            var buffer = _em.GetBuffer<ConnectedEdge>(node, true);
            for (var i = 0; i < buffer.Length; i += 1)
            {
                var edge = buffer[i].m_Edge;
                if (IsEnd(edge, node))
                    result.Add(edge);
            }
            return result;
        }

        /// <summary>The other node of each connected edge.</summary>
        public IEnumerable<Entity> Neighbors(Entity node)
        {
            var result = new List<Entity>();
            foreach (var edge in Edges(node))
                result.Add(OtherNode(edge, node));
            return result;
        }

        public Entity OtherNode(Entity edge, Entity node)
        {
            var data = Ends(edge);
            return data.m_Start == node ? data.m_End : data.m_Start;
        }

        /// <summary>Start and end nodes of an edge.</summary>
        public Edge Ends(Entity edge) => _em.GetComponentData<Edge>(edge);

        public bool IsStart(Entity edge, Entity node) => Ends(edge).m_Start == node;

        public bool IsEnd(Entity edge, Entity node)
        {
            var ends = Ends(edge);
            return ends.m_Start == node || ends.m_End == node;
        }

        /// <summary>First edge starting or ending at the node, Entity.Null when none. Unlike <see cref="Edges"/> it allocates nothing, for per-frame drawing.</summary>
        public Entity FirstEdge(Entity node)
        {
            if (!_em.HasBuffer<ConnectedEdge>(node))
                return Entity.Null;

            var buffer = _em.GetBuffer<ConnectedEdge>(node, true);
            for (var i = 0; i < buffer.Length; i += 1)
            {
                if (IsEnd(buffer[i].m_Edge, node))
                    return buffer[i].m_Edge;
            }

            return Entity.Null;
        }

        /// <summary>Game.Net.Fixed.m_Index of the edge, -1 when absent (vanilla NetCourse.m_FixedIndex).</summary>
        public int FixedIndex(Entity edge) => _em.HasComponent<Fixed>(edge) ? _em.GetComponentData<Fixed>(edge).m_Index : -1;

        public Bezier4x3 Curve(Entity edge) => _em.GetComponentData<Curve>(edge).m_Bezier;

        /// <summary>Flat normalized tangent leaving <paramref name="node"/> towards the inside of <paramref name="edge"/>.</summary>
        public float3 DirectionAt(Entity edge, Entity node)
        {
            var bezier = Curve(edge);
            return IsStart(edge, node) ? Vec.NormalizeXZ(MathUtils.StartTangent(bezier)) : Vec.NormalizeXZ(-MathUtils.EndTangent(bezier));
        }

        public bool TryCommonEdge(Entity a, Entity b, out Entity edge)
        {
            foreach (var candidate in Edges(a))
            {
                if (OtherNode(candidate, a) == b)
                {
                    edge = candidate;
                    return true;
                }
            }

            edge = Entity.Null;
            return false;
        }

        public Entity Prefab(Entity entity) => _em.HasComponent<PrefabRef>(entity) ? _em.GetComponentData<PrefabRef>(entity).m_Prefab : Entity.Null;

        public float Width(Entity edge) => PrefabWidth(Prefab(edge));

        /// <summary>Default width of a net prefab, 8 m when unknown.</summary>
        public float PrefabWidth(Entity prefab) => prefab != Entity.Null && _em.HasComponent<NetGeometryData>(prefab) ? _em.GetComponentData<NetGeometryData>(prefab).m_DefaultWidth : DefaultWidth;

        /// <summary>+1 when every road lane of the prefab runs along the edge direction, -1 when against it, 0 for two-way or non-road types (RoadFlags set by NetInitializeSystem).</summary>
        public int OneWay(Entity prefab)
        {
            if (prefab == Entity.Null || !_em.HasComponent<RoadData>(prefab))
                return 0;
            var flags = _em.GetComponentData<RoadData>(prefab).m_Flags;
            return (flags & Game.Prefabs.RoadFlags.DefaultIsForward) != 0 ? 1 : (flags & Game.Prefabs.RoadFlags.DefaultIsBackward) != 0 ? -1 : 0;
        }

        public float2 Elevation(Entity entity) => _em.HasComponent<Elevation>(entity) ? _em.GetComponentData<Elevation>(entity).m_Elevation : float2.zero;

        /// <summary>True when the entity's net prefab is on one of the layers.</summary>
        public bool OnLayer(Entity entity, Layer layers)
        {
            var prefab = Prefab(entity);
            return prefab != Entity.Null && _em.HasComponent<NetData>(prefab) && (_em.GetComponentData<NetData>(prefab).m_RequiredLayers & layers) != 0;
        }

        /// <summary>Upgrade flags of an edge (bike lanes, wide sidewalks, sound barriers...); default when it has none.</summary>
        public CompositionFlags Upgrades(Entity edge) => _em.HasComponent<Upgraded>(edge) ? _em.GetComponentData<Upgraded>(edge).m_Flags : default;

        /// <summary>Copy of the edge's replaced sub-objects (trees, grass...).</summary>
        public List<SubReplacement> SubReplacements(Entity edge)
        {
            var result = new List<SubReplacement>();
            if (!_em.HasBuffer<SubReplacement>(edge))
                return result;
            var buffer = _em.GetBuffer<SubReplacement>(edge, true);
            for (var i = 0; i < buffer.Length; i++)
                result.Add(buffer[i]);
            return result;
        }

        public bool HasOwner(Entity entity) => _em.HasComponent<Owner>(entity);

        public bool IsNode(Entity e) => _em.HasComponent<Node>(e);

        public bool IsEdge(Entity e) => _em.HasComponent<Edge>(e);
    }
}
