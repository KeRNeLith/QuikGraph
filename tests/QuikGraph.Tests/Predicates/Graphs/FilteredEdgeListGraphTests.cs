using System;
using NUnit.Framework;
using QuikGraph.Predicates;
using static QuikGraph.Tests.GraphTestHelpers;

namespace QuikGraph.Tests.Predicates
{
    /// <summary>
    /// Tests for <see cref="FilteredEdgeListGraph{TVertex,TEdge,TGraph}"/>.
    /// </summary>
    [TestFixture]
    internal sealed class FilteredEdgeListGraphTests : FilteredGraphTestsBase
    {
        [Test]
        public void Construction()
        {
            Func<int, bool> vertexPredicate = _ => true;
            Func<Edge<int>, bool> edgePredicate = _ => true;

            var graph1 = new AdjacencyGraph<int, Edge<int>>();
            var filteredGraph1 = graph1.FilterByEdges(vertexPredicate, edgePredicate);
            AssertGraphProperties(filteredGraph1, graph1);

            graph1 = new AdjacencyGraph<int, Edge<int>>(false);
            filteredGraph1 = graph1.FilterByEdges(vertexPredicate, edgePredicate);
            AssertGraphProperties(filteredGraph1, graph1, parallelEdges: false);

            var graph2 = new UndirectedGraph<int, Edge<int>>();
            var filteredGraph2 = graph2.FilterByEdges(vertexPredicate, edgePredicate);
            AssertGraphProperties(filteredGraph2, graph2, false);

            graph2 = new UndirectedGraph<int, Edge<int>>(false);
            filteredGraph2 = graph2.FilterByEdges(vertexPredicate, edgePredicate);
            AssertGraphProperties(filteredGraph2, graph2, false, false);

            #region Local function

            void AssertGraphProperties<TVertex, TEdge, TGraph>(
                FilteredEdgeListGraph<TVertex, TEdge, TGraph> g,
                TGraph expectedGraph,
                bool isDirected = true,
                bool parallelEdges = true)
                where TEdge : IEdge<TVertex>
                where TGraph : IEdgeListGraph<TVertex, TEdge>
            {
                Assert.AreSame(expectedGraph, g.BaseGraph);
                Assert.AreEqual(isDirected, g.IsDirected);
                Assert.AreEqual(parallelEdges, g.AllowParallelEdges);
                Assert.AreSame(vertexPredicate, g.VertexPredicate);
                Assert.AreSame(edgePredicate, g.EdgePredicate);
                AssertEmptyGraph(g);
            }

            #endregion
        }

        [Test]
        public void Construction_Throws()
        {
            // ReSharper disable ObjectCreationAsStatement
            // ReSharper disable AssignNullToNotNullAttribute
            Assert.Throws<ArgumentNullException>(
                () => new AdjacencyGraph<int, Edge<int>>()
                    .FilterByEdges<int, Edge<int>, AdjacencyGraph<int, Edge<int>>>(
                    _ => true,
                    null));

            Assert.Throws<ArgumentNullException>(
                () => new AdjacencyGraph<int, Edge<int>>()
                    .FilterByEdges<int, Edge<int>, AdjacencyGraph<int, Edge<int>>>(
                    null,
                    _ => true));

            Assert.Throws<ArgumentNullException>(
                () => new FilteredEdgeListGraph<int, IEdge<int>, AdjacencyGraph<int, IEdge<int>>>(
                    null,
                    _ => true,
                    _ => true));

            Assert.Throws<ArgumentNullException>(
                () => new AdjacencyGraph<int, Edge<int>>()
                    .FilterByEdges<int, Edge<int>, AdjacencyGraph<int, Edge<int>>>(
                    null,
                    null));

            Assert.Throws<ArgumentNullException>(
                () => new FilteredEdgeListGraph<int, IEdge<int>, AdjacencyGraph<int, IEdge<int>>>(
                    null,
                    _ => true,
                    null));

            Assert.Throws<ArgumentNullException>(
                () => new FilteredEdgeListGraph<int, IEdge<int>, AdjacencyGraph<int, IEdge<int>>>(
                    null,
                    null,
                    _ => true));

            Assert.Throws<ArgumentNullException>(
                () => new FilteredEdgeListGraph<int, IEdge<int>, AdjacencyGraph<int, IEdge<int>>>(
                    null,
                    null,
                    null));
            // ReSharper restore AssignNullToNotNullAttribute
            // ReSharper restore ObjectCreationAsStatement
        }

        #region Vertices & Edges

        [Test]
        public void Vertices()
        {
            var wrappedGraph = new AdjacencyGraph<int, IEdge<int>>();
            Vertices_Test(wrappedGraph, (vertexPredicate, edgePredicate)
                => wrappedGraph.FilterByEdges(vertexPredicate, edgePredicate));
        }

        [Test]
        public void Edges()
        {
            var wrappedGraph = new AdjacencyGraph<int, IEdge<int>>();
            Edges_Test(wrappedGraph, (vertexPredicate, edgePredicate)
                => wrappedGraph.FilterByEdges(vertexPredicate, edgePredicate));
        }

        #endregion

        #region Contains Vertex

        [Test]
        public void ContainsVertex()
        {
            var graph = new AdjacencyGraph<int, IEdge<int>>();
            ContainsVertex_Test(graph, (vertexPredicate, edgePredicate)
                => graph.FilterByEdges(vertexPredicate, edgePredicate));
        }

        [Test]
        public void ContainsVertex_Throws()
        {
            var filteredGraph = new AdjacencyGraph<TestVertex, IEdge<TestVertex>>()
                .FilterByEdges<TestVertex, IEdge<TestVertex>, AdjacencyGraph<TestVertex, IEdge<TestVertex>>>(
                _ => true,
                _ => true);
            ContainsVertex_Throws_Test(filteredGraph);
        }

        #endregion

        #region Contains Edge

        [Test]
        public void ContainsEdge()
        {
            var graph = new AdjacencyGraph<int, IEdge<int>>();
            ContainsEdge_Test(graph, (vertexPredicate, edgePredicate)
                    => graph.FilterByEdges(vertexPredicate, edgePredicate));
        }

        [Test]
        public void ContainsEdge_EquatableEdge()
        {
            var graph = new AdjacencyGraph<int, EquatableEdge<int>>();
            ContainsEdge_EquatableEdge_Test(graph, (vertexPredicate, edgePredicate)
                    => graph.FilterByEdges(vertexPredicate, edgePredicate));
        }

        #endregion
    }
}