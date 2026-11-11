using System.Collections.Generic;
using System.Linq;
using NetworkMultitool.Net;
using Xunit;

namespace NetworkMultitool.Tests
{
    public class NodeChainTests
    {
        private sealed class IntGraph : INetGraph<int>
        {
            private readonly Dictionary<int, List<int>> _adjacency = new Dictionary<int, List<int>>();

            public IntGraph Link(int a, int b)
            {
                Get(a).Add(b);
                Get(b).Add(a);
                return this;
            }

            public static IntGraph Line(int count)
            {
                var graph = new IntGraph();
                for (var i = 1; i < count; i += 1)
                    graph.Link(i, i + 1);
                return graph;
            }

            private List<int> Get(int node)
            {
                if (!_adjacency.TryGetValue(node, out var list))
                    _adjacency[node] = list = new List<int>();
                return list;
            }

            public int Degree(int node) => _adjacency.TryGetValue(node, out var list) ? list.Count : 0;
            public IEnumerable<int> Neighbors(int node) => _adjacency.TryGetValue(node, out var list) ? list : Enumerable.Empty<int>();
        }

        [Fact]
        public void Empty_AddsTarget()
        {
            var toAdd = new List<int>();
            Assert.True(NodeChain.TryExtend(IntGraph.Line(5), new int[0], 3, 20, toAdd, out var prepend));
            Assert.Equal(new[] { 3 }, toAdd);
            Assert.False(prepend);
        }

        [Fact]
        public void Path_ThroughDegreeTwo_Succeeds()
        {
            var toAdd = new List<int>();
            Assert.True(NodeChain.TryExtend(IntGraph.Line(5), new[] { 1 }, 4, 20, toAdd, out var prepend));
            Assert.Equal(new[] { 2, 3, 4 }, toAdd);
            Assert.False(prepend);
        }

        [Fact]
        public void Prepend_FromStart()
        {
            var toAdd = new List<int>();
            Assert.True(NodeChain.TryExtend(IntGraph.Line(5), new[] { 3, 4 }, 1, 20, toAdd, out var prepend));
            Assert.Equal(new[] { 2, 1 }, toAdd);
            Assert.True(prepend);
        }

        [Fact]
        public void Blocked_ByJunction_Fails()
        {
            var graph = IntGraph.Line(5).Link(3, 9);
            Assert.False(NodeChain.TryExtend(graph, new[] { 1 }, 5, 20, new List<int>(), out _));
        }

        [Fact]
        public void MaxSteps_Respected()
        {
            Assert.False(NodeChain.TryExtend(IntGraph.Line(30), new[] { 1 }, 30, 20, new List<int>(), out _));
        }

        [Fact]
        public void AlreadyInChain_Fails()
        {
            Assert.False(NodeChain.TryExtend(IntGraph.Line(5), new[] { 1, 2, 3 }, 2, 20, new List<int>(), out _));
        }
    }
}
