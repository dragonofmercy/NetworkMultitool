using System;
using System.Collections.Generic;
using System.Linq;

namespace NetworkMultitool.Net
{
    /// <summary>Extends an ordered node chain towards a target through degree-2 nodes (port of CS1 BaseNodeSet.Check).</summary>
    public static class NodeChain
    {
        /// <summary>
        /// Tries to reach <paramref name="target"/> from the last node of the chain, then from the first one (prepend = true).
        /// On success <paramref name="toAdd"/> holds the traversed nodes followed by the target, in insertion order.
        /// </summary>
        public static bool TryExtend<T>(INetGraph<T> graph, IReadOnlyList<T> chain, T target, int maxSteps, List<T> toAdd, out bool prepend) where T : IEquatable<T>
        {
            toAdd.Clear();
            prepend = false;

            if (chain.Count == 0)
            {
                toAdd.Add(target);
                return true;
            }

            if (chain.Contains(target))
                return false;

            if (Walk(graph, chain, chain[chain.Count - 1], target, maxSteps, toAdd))
                return true;

            if (chain.Count > 1 && Walk(graph, chain, chain[0], target, maxSteps, toAdd))
            {
                prepend = true;
                return true;
            }

            toAdd.Clear();
            return false;
        }

        private static bool Walk<T>(INetGraph<T> graph, IReadOnlyList<T> chain, T from, T target, int maxSteps, List<T> toAdd) where T : IEquatable<T>
        {
            foreach (var first in graph.Neighbors(from))
            {
                toAdd.Clear();
                var prev = from;
                var next = first;

                for (var i = 0; i < maxSteps; i += 1)
                {
                    if (chain.Contains(next))
                        break;

                    toAdd.Add(next);
                    if (next.Equals(target))
                        return true;

                    if (graph.Degree(next) != 2)
                        break;

                    if (!TryOther(graph, next, prev, out var other))
                        break;

                    prev = next;
                    next = other;
                }
            }

            toAdd.Clear();
            return false;
        }

        private static bool TryOther<T>(INetGraph<T> graph, T node, T exclude, out T other) where T : IEquatable<T>
        {
            foreach (var n in graph.Neighbors(node))
            {
                if (!n.Equals(exclude))
                {
                    other = n;
                    return true;
                }
            }

            other = default;
            return false;
        }
    }
}
