using JetBrains.Annotations;
using NUnit.Framework;

namespace QuikGraph.Tests.Structures
{
    /// <summary> Tests related to array graphs. </summary>
    [TestFixture]
    internal sealed class ArrayGraphTests
    {
        [TestCaseSource(typeof(TestGraphFactory), nameof(TestGraphFactory.GetAdjacencyGraphs_All))]
        public static void AssertSameProperties<TVertex, TEdge>([NotNull] IVertexAndEdgeListGraph<TVertex, TEdge> graph)
            where TEdge : IEdge<TVertex>
        {
            var adjacencyGraph = graph.ToArrayAdjacencyGraph();

            Assert.AreEqual(graph.VertexCount, adjacencyGraph.VertexCount);
            CollectionAssert.AreEqual(graph.Vertices, adjacencyGraph.Vertices);

            Assert.AreEqual(graph.EdgeCount, adjacencyGraph.EdgeCount);
            CollectionAssert.AreEqual(graph.Edges, adjacencyGraph.Edges);

            foreach (TVertex vertex in graph.Vertices)
                CollectionAssert.AreEqual(graph.OutEdges(vertex), adjacencyGraph.OutEdges(vertex));
        }

        [TestCaseSource(typeof(TestGraphFactory), nameof(TestGraphFactory.GetBidirectionalGraphs_All))]
        public static void AssertSameProperties<TVertex, TEdge>([NotNull] IBidirectionalGraph<TVertex, TEdge> graph)
            where TEdge : IEdge<TVertex>
        {
            var bidirectionalGraph = graph.ToArrayBidirectionalGraph();

            Assert.AreEqual(graph.VertexCount, bidirectionalGraph.VertexCount);
            CollectionAssert.AreEqual(graph.Vertices, bidirectionalGraph.Vertices);

            Assert.AreEqual(graph.EdgeCount, bidirectionalGraph.EdgeCount);
            CollectionAssert.AreEqual(graph.Edges, bidirectionalGraph.Edges);

            foreach (TVertex vertex in graph.Vertices)
                CollectionAssert.AreEqual(graph.OutEdges(vertex), bidirectionalGraph.OutEdges(vertex));
        }

        [TestCaseSource(typeof(TestGraphFactory), nameof(TestGraphFactory.GetUndirectedGraphs_All))]
        public static void AssertSameProperties<TVertex, TEdge>([NotNull] IUndirectedGraph<TVertex, TEdge> graph) where TEdge : IEdge<TVertex>
        {
            var undirectedGraph = graph.ToArrayUndirectedGraph();

            Assert.AreEqual(graph.VertexCount, undirectedGraph.VertexCount);
            CollectionAssert.AreEqual(graph.Vertices, undirectedGraph.Vertices);

            Assert.AreEqual(graph.EdgeCount, undirectedGraph.EdgeCount);
            CollectionAssert.AreEqual(graph.Edges, undirectedGraph.Edges);

            foreach (TVertex vertex in graph.Vertices)
                CollectionAssert.AreEqual(graph.AdjacentEdges(vertex), undirectedGraph.AdjacentEdges(vertex));
        }
    }
}