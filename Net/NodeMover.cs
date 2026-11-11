using Colossal.Mathematics;
using Game.Common;
using Game.Net;
using Unity.Entities;
using Unity.Mathematics;

namespace NetworkMultitool.Net
{
    /// <summary>Direct component writes on existing nodes and edges (Move It pattern).</summary>
    public static class NodeMover
    {
        /// <summary>Rewrites position and rotation, keeping every other field of the existing <see cref="Node"/>.</summary>
        public static void MoveNode(EntityManager em, Entity node, float3 position, quaternion rotation)
        {
            var data = em.GetComponentData<Node>(node);
            data.m_Position = position;
            data.m_Rotation = rotation;
            em.SetComponentData(node, data);
        }

        public static void SetCurve(EntityManager em, Entity edge, Bezier4x3 bezier)
        {
            em.SetComponentData(edge, new Curve { m_Bezier = bezier, m_Length = MathUtils.Length(bezier) });
        }

        /// <summary>Flags the node, its edges, their opposite nodes and their aggregates as updated. Safe to call repeatedly.</summary>
        public static void MarkUpdated(EntityManager em, Entity node)
        {
            var net = new NetQuery(em);
            Mark(em, node);
            foreach (var edge in net.Edges(node))
            {
                Mark(em, edge);
                Mark(em, net.OtherNode(edge, node));
                if (em.HasComponent<Aggregated>(edge))
                    Mark(em, em.GetComponentData<Aggregated>(edge).m_Aggregate);
            }
        }

        private static void Mark(EntityManager em, Entity entity)
        {
            if (entity == Entity.Null || !em.Exists(entity))
                return;
            if (!em.HasComponent<Updated>(entity))
                em.AddComponent<Updated>(entity);
            if (!em.HasComponent<BatchesUpdated>(entity))
                em.AddComponent<BatchesUpdated>(entity);
        }
    }
}
