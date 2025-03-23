using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using QuikGraph.Algorithms.Exploration;

namespace QuikGraph.Tests.Algorithms.Exploration
{
    internal sealed class TestTransitionFactory<TVertex> : ITransitionFactory<TVertex, IEdge<TVertex>>
        where TVertex : CloneableTestVertex
    {
        private readonly Dictionary<TVertex, List<IEdge<TVertex>>> _edges = new();

        public struct VertexEdgesSet
        {
            public VertexEdgesSet(
                [NotNull] TVertex vertex,
                [NotNull, ItemNotNull] IEnumerable<IEdge<TVertex>> edges)
            {
                Vertex = vertex;
                Edges = edges;
            }

            [NotNull]
            public TVertex Vertex { get; }

            [NotNull, ItemNotNull]
            public IEnumerable<IEdge<TVertex>> Edges { get; }
        }

        public TestTransitionFactory(
            [NotNull] TVertex vertex,
            [NotNull, ItemNotNull] params IEdge<TVertex>[] edges) : this(vertex,edges.AsEnumerable())
        {
        }

        public TestTransitionFactory(
            [NotNull] TVertex vertex,
            [NotNull, ItemNotNull] IEnumerable<IEdge<TVertex>> edges)
        {
            AddEdgeSet(vertex, edges);
        }

        public TestTransitionFactory([NotNull] IEnumerable<VertexEdgesSet> sets)
        {
            foreach (VertexEdgesSet set in sets)
            {
                AddEdgeSet(set.Vertex, set.Edges);
            }
        }

        private void AddEdgeSet(
            [NotNull] TVertex vertex,
            [NotNull, ItemNotNull] IEnumerable<IEdge<TVertex>> edges)
        {
            _edges.Add(vertex, edges.ToList());
        }

        public bool IsValid(TVertex vertex)
        {
            return _edges.ContainsKey(vertex);
        }

        public IEnumerable<IEdge<TVertex>> Apply(TVertex source) => _edges[source];
    }
}