using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using NUnit.Framework;
using QuikGraph.Algorithms;
using QuikGraph.Algorithms.Condensation;

namespace QuikGraph.Tests.Algorithms.Condensation
{
    /// <summary>
    /// Tests for <see cref="CondensationGraphAlgorithm{TVertex,TEdge,TGraph}"/> (strongly connected).
    /// </summary>
    [TestFixture]
    internal sealed class StronglyConnectedCondensationGraphAlgorithmTests : CondensationGraphAlgorithmTestsBase
    {
        [TestCaseSource(typeof(TestGraphFactory), nameof(TestGraphFactory.GetAdjacencyGraphs_All))]
        public static void RunStronglyConnectedCondensationAndCheck<TVertex, TEdge>(
            [NotNull] IVertexAndEdgeListGraph<TVertex, TEdge> graph)
            where TEdge : IEdge<TVertex>
        {
            IMutableBidirectionalGraph<AdjacencyGraph<TVertex, TEdge>, CondensedEdge<TVertex, TEdge, AdjacencyGraph<TVertex, TEdge>>> condensedGraph =
                graph.CondensateStronglyConnected<TVertex, TEdge, AdjacencyGraph<TVertex, TEdge>>();

            Assert.IsNotNull(condensedGraph);
            CheckVertexCount(graph, condensedGraph);
            CheckEdgeCount(graph, condensedGraph);
            CheckComponentCount(graph, condensedGraph);
            CheckDAG(condensedGraph);
        }

        private static void CheckComponentCount<TVertex, TEdge>(
            [NotNull] IVertexListGraph<TVertex, TEdge> graph,
            [NotNull] IVertexSet<AdjacencyGraph<TVertex, TEdge>> condensedGraph)
            where TEdge : IEdge<TVertex>
        {
            // Check number of vertices = number of strongly connected components
            var components = new Dictionary<TVertex, int>();
            int componentCount = graph.StronglyConnectedComponentsCount(components);
            Assert.AreEqual(componentCount, condensedGraph.VertexCount, "Component count does not match.");
        }

        [Test]
        public void OneStronglyConnectedComponent()
        {
            var graph = new AdjacencyGraph<int, IEdge<int>>();
            graph.AddVerticesAndEdgeRange(
                Edge.Create(1, 2), Edge.Create(2, 3), Edge.Create(3, 1)
            );

            var condensedGraph = graph.CondensateStronglyConnected<int, IEdge<int>, AdjacencyGraph<int, IEdge<int>>>();

            Assert.IsNotNull(condensedGraph);
            Assert.AreEqual(1, condensedGraph.VertexCount);
            Assert.AreEqual(0, condensedGraph.EdgeCount);
            CollectionAssert.AreEquivalent(graph.Vertices, condensedGraph.Vertices.ElementAt(0).Vertices);
            CollectionAssert.AreEquivalent(graph.Edges, condensedGraph.Vertices.ElementAt(0).Edges);
        }

        [Test]
        public void MultipleStronglyConnectedComponents()
        {
            var edge12 = Edge.Create(1, 2);
            var edge23 = Edge.Create(2, 3);
            var edge24 = Edge.Create(2, 4);
            var edge25 = Edge.Create(2, 5);
            var edge31 = Edge.Create(3, 1);
            var edge34 = Edge.Create(3, 4);
            var edge46 = Edge.Create(4, 6);
            var edge56 = Edge.Create(5, 6);
            var edge57 = Edge.Create(5, 7);
            var edge64 = Edge.Create(6, 4);
            var edge75 = Edge.Create(7, 5);
            var edge78 = Edge.Create(7, 8);
            var edge86 = Edge.Create(8, 6);
            var edge87 = Edge.Create(8, 7);

            var graph = new AdjacencyGraph<int, IEdge<int>>();
            graph.AddVerticesAndEdgeRange(
                edge12, edge23, edge24, edge25, edge31, edge34, edge46,
                edge56, edge57, edge64, edge75, edge78, edge86, edge87
            );
            graph.AddVertex(10);

            var condensedGraph = graph.CondensateStronglyConnected<int, IEdge<int>, AdjacencyGraph<int, IEdge<int>>>();

            Assert.IsNotNull(condensedGraph);
            Assert.AreEqual(4, condensedGraph.VertexCount);
            Assert.AreEqual(3, condensedGraph.EdgeCount);

            // Condensed edge
            CollectionAssert.AreEquivalent(
                new[] { edge56, edge86 },
                condensedGraph.Edges.ElementAt(0).Edges);
            CollectionAssert.AreEquivalent(
                new[] { edge24, edge34 },
                condensedGraph.Edges.ElementAt(1).Edges);
            CollectionAssert.AreEquivalent(
                new[] { edge25 },
                condensedGraph.Edges.ElementAt(2).Edges);

            // Components
            CollectionAssert.AreEquivalent(
                new[] { 4, 6 },
                condensedGraph.Vertices.ElementAt(0).Vertices);
            CollectionAssert.AreEquivalent(
                new[] { edge46, edge64 },
                condensedGraph.Vertices.ElementAt(0).Edges);

            CollectionAssert.AreEquivalent(
                new[] { 5, 7, 8 },
                condensedGraph.Vertices.ElementAt(1).Vertices);
            CollectionAssert.AreEquivalent(
                new[] { edge57, edge75, edge78, edge87 },
                condensedGraph.Vertices.ElementAt(1).Edges);

            CollectionAssert.AreEquivalent(
                new[] { 1, 2, 3 },
                condensedGraph.Vertices.ElementAt(2).Vertices);
            CollectionAssert.AreEquivalent(
                new[] { edge12, edge23, edge31 },
                condensedGraph.Vertices.ElementAt(2).Edges);

            CollectionAssert.AreEquivalent(
                new[] { 10 },
                condensedGraph.Vertices.ElementAt(3).Vertices);
            CollectionAssert.IsEmpty(condensedGraph.Vertices.ElementAt(3).Edges);
        }

    }
}