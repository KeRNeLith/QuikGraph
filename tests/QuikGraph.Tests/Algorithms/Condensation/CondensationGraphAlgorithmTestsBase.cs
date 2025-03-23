using JetBrains.Annotations;
using NUnit.Framework;
using QuikGraph.Algorithms;
using QuikGraph.Algorithms.Condensation;

namespace QuikGraph.Tests.Algorithms.Condensation
{
    /// <summary>
    /// Base class for condensation algorithms.
    /// </summary>
    internal abstract class CondensationGraphAlgorithmTestsBase
    {
        protected static void CheckVertexCount<TVertex, TEdge>(
            [NotNull] IVertexSet<TVertex> graph,
            [NotNull] IVertexSet<AdjacencyGraph<TVertex, TEdge>> condensedGraph)
            where TEdge : IEdge<TVertex>
        {
            int count = 0;
            foreach (AdjacencyGraph<TVertex, TEdge> vertices in condensedGraph.Vertices)
                count += vertices.VertexCount;
            Assert.AreEqual(graph.VertexCount, count, $"{nameof(graph.VertexCount)} does not match.");
        }

        protected static void CheckEdgeCount<TVertex, TEdge>(
            [NotNull] IEdgeSet<TVertex, TEdge> graph,
            [NotNull] IEdgeListGraph<AdjacencyGraph<TVertex, TEdge>, CondensedEdge<TVertex, TEdge, AdjacencyGraph<TVertex, TEdge>>> condensedGraph)
            where TEdge : IEdge<TVertex>
        {
            // Check edge count
            int count = 0;
            foreach (CondensedEdge<TVertex, TEdge, AdjacencyGraph<TVertex, TEdge>> edges in condensedGraph.Edges)
                count += edges.Edges.Count;
            foreach (AdjacencyGraph<TVertex, TEdge> vertices in condensedGraph.Vertices)
                count += vertices.EdgeCount;
            Assert.AreEqual(graph.EdgeCount, count, $"{nameof(graph.EdgeCount)} does not match.");
        }

        protected static void CheckDAG<TVertex, TEdge>(
            [NotNull] IVertexAndEdgeListGraph<AdjacencyGraph<TVertex, TEdge>, CondensedEdge<TVertex, TEdge, AdjacencyGraph<TVertex, TEdge>>> condensedGraph)
            where TEdge : IEdge<TVertex>
        {
            Assert.IsTrue(condensedGraph.IsDirectedAcyclicGraph());
        }

    }
}