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
                var ends = Ends(edge);
                if (ends.m_Start == node || ends.m_End == node)
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

        public float Width(Entity edge)
        {
            var prefab = Prefab(edge);
            return prefab != Entity.Null && _em.HasComponent<NetGeometryData>(prefab) ? _em.GetComponentData<NetGeometryData>(prefab).m_DefaultWidth : DefaultWidth;
        }

        public float2 Elevation(Entity entity) => _em.HasComponent<Elevation>(entity) ? _em.GetComponentData<Elevation>(entity).m_Elevation : float2.zero;

        public bool HasOwner(Entity entity) => _em.HasComponent<Owner>(entity);

        public bool IsNode(Entity e) => _em.HasComponent<Node>(e);

        public bool IsEdge(Entity e) => _em.HasComponent<Edge>(e);
    }
}
