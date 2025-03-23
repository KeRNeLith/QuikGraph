using System;
using NUnit.Framework;
using QuikGraph.Predicates;
using static QuikGraph.Tests.GraphTestHelpers;

namespace QuikGraph.Tests.Predicates
{
    /// <summary> Tests for <see cref="FilteredUndirectedGraph{TVertex,TEdge,TGraph}"/>. </summary>
    [TestFixture]
    internal sealed class FilteredUndirectedGraphTests : FilteredGraphTestsBase
    {
        [Test]
        public void Construction()
        {
            Func<int, bool> vertexPredicate = _ => true;
            Func<IEdge<int>, bool> edgePredicate = _ => true;

            var graph = new UndirectedGraph<int, IEdge<int>>();
            var filteredGraph = new FilteredUndirectedGraph<int, IEdge<int>, UndirectedGraph<int, IEdge<int>>>(
                graph,
                vertexPredicate,
                edgePredicate);
            AssertGraphProperties(filteredGraph);

            graph = new UndirectedGraph<int, IEdge<int>>(false);
            filteredGraph = new FilteredUndirectedGraph<int, IEdge<int>, UndirectedGraph<int, IEdge<int>>>(
                graph,
                vertexPredicate,
                edgePredicate);
            AssertGraphProperties(filteredGraph, false);

            #region Local function

            void AssertGraphProperties<TVertex, TEdge, TGraph>(
                FilteredUndirectedGraph<TVertex, TEdge, TGraph> g,
                bool parallelEdges = true)
                where TEdge : IEdge<TVertex>
                where TGraph : IUndirectedGraph<TVertex, TEdge>
            {
                Assert.AreSame(graph, g.BaseGraph);
                Assert.IsFalse(g.IsDirected);
                Assert.AreEqual(parallelEdges, g.AllowParallelEdges);
                Assert.AreSame(vertexPredicate, g.VertexPredicate);
                Assert.AreSame(edgePredicate, g.EdgePredicate);
                Assert.IsNotNull(g.EdgeEqualityComparer);
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
                () => new FilteredUndirectedGraph<int, IEdge<int>, UndirectedGraph<int, IEdge<int>>>(
                    new UndirectedGraph<int, IEdge<int>>(),
                    _ => true,
                    null));

            Assert.Throws<ArgumentNullException>(
                () => new FilteredUndirectedGraph<int, IEdge<int>, UndirectedGraph<int, IEdge<int>>>(
                    new UndirectedGraph<int, IEdge<int>>(),
                    null,
                    _ => true));

            Assert.Throws<ArgumentNullException>(
                () => new FilteredUndirectedGraph<int, IEdge<int>, UndirectedGraph<int, IEdge<int>>>(
                    null,
                    _ => true,
                    _ => true));

            Assert.Throws<ArgumentNullException>(
                () => new FilteredUndirectedGraph<int, IEdge<int>, UndirectedGraph<int, IEdge<int>>>(
                    new UndirectedGraph<int, IEdge<int>>(),
                    null,
                    null));

            Assert.Throws<ArgumentNullException>(
                () => new FilteredUndirectedGraph<int, IEdge<int>, UndirectedGraph<int, IEdge<int>>>(
                    null,
                    _ => true,
                    null));

            Assert.Throws<ArgumentNullException>(
                () => new FilteredUndirectedGraph<int, IEdge<int>, UndirectedGraph<int, IEdge<int>>>(
                    null,
                    null,
                    _ => true));

            Assert.Throws<ArgumentNullException>(
                () => new FilteredUndirectedGraph<int, IEdge<int>, UndirectedGraph<int, IEdge<int>>>(
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
            var wrappedGraph = new UndirectedGraph<int, IEdge<int>>();
            Vertices_Test(wrappedGraph, (vertexPredicate, edgePredicate)
                    => wrappedGraph.FilterByUndirected(vertexPredicate, edgePredicate));
        }

        [Test]
        public void Edges()
        {
            var wrappedGraph = new UndirectedGraph<int, IEdge<int>>();
            Edges_Test(wrappedGraph, (vertexPredicate, edgePredicate)
                => wrappedGraph.FilterByUndirected(vertexPredicate, edgePredicate));
        }

        #endregion

        #region Contains Vertex

        [Test]
        public void ContainsVertex()
        {
            var graph = new UndirectedGraph<int, IEdge<int>>();
            ContainsVertex_Test(graph, (vertexPredicate, edgePredicate)
                => graph.FilterByUndirected(vertexPredicate, edgePredicate));
        }

        [Test]
        public void ContainsVertex_Throws()
        {
            var filteredGraph = new FilteredUndirectedGraph<TestVertex, IEdge<TestVertex>, UndirectedGraph<TestVertex, IEdge<TestVertex>>>(
                new UndirectedGraph<TestVertex, IEdge<TestVertex>>(),
                _ => true,
                _ => true);
            ContainsVertex_Throws_Test(filteredGraph);
        }

        #endregion

        #region Contains Edge

        [Test]
        public void ContainsEdge()
        {
            var graph = new UndirectedGraph<int, IEdge<int>>();
            ContainsEdge_Test(graph, (vertexPredicate, edgePredicate)
                => graph.FilterByUndirected(vertexPredicate, edgePredicate));
        }

        [Test]
        public void ContainsEdge_EquatableEdge()
        {
            var graph = new UndirectedGraph<int, EquatableEdge<int>>();
            ContainsEdge_EquatableEdge_Test(graph, (vertexPredicate, edgePredicate)
                => graph.FilterByUndirected(vertexPredicate, edgePredicate));
        }

        [Test]
        public void ContainsEdge_SourceTarget()
        {
            var graph = new UndirectedGraph<int, IEdge<int>>();
            ContainsEdge_SourceTarget_UndirectedGraph_Test(graph, (vertexPredicate, edgePredicate)
                => graph.FilterByUndirected(vertexPredicate, edgePredicate));
        }

        [Test]
        public void ContainsEdge_Throws()
        {
            var filteredGraph = new FilteredUndirectedGraph<TestVertex, IEdge<TestVertex>, UndirectedGraph<TestVertex, IEdge<TestVertex>>>(
                new UndirectedGraph<TestVertex, IEdge<TestVertex>>(),
                _ => true,
                _ => true);
            ContainsEdge_NullThrows_Test(filteredGraph);
            ContainsEdge_SourceTarget_Throws_UndirectedGraph_Test(filteredGraph);
        }

        #endregion

        #region Adjacent Edges

        [Test]
        public void AdjacentEdge()
        {
            var graph = new UndirectedGraph<int, IEdge<int>>();
            AdjacentEdge_Test(graph, (vertexPredicate, edgePredicate)
                => graph.FilterByUndirected(vertexPredicate, edgePredicate));
        }

        [Test]
        public void AdjacentEdge_Throws()
        {
            var graph1 = new UndirectedGraph<int, IEdge<int>>();
            AdjacentEdge_Throws_Test(graph1, (vertexPredicate, edgePredicate)
                => graph1.FilterByUndirected(vertexPredicate, edgePredicate));
        }

        [Test]
        public void AdjacentEdgeUnfiltered_Throws()
        {
            var graph2 = new UndirectedGraph<TestVertex, IEdge<TestVertex>>();
            var filteredGraph2 = graph2.FilterByUndirected<TestVertex, IEdge<TestVertex>, UndirectedGraph<TestVertex, IEdge<TestVertex>>>(
                _ => true,
                _ => true);
            AdjacentEdge_NullThrows_Test(filteredGraph2);
        }

        [Test]
        public void AdjacentEdges()
        {
            var graph = new UndirectedGraph<int, IEdge<int>>();
            AdjacentEdges_Test(graph, (vertexPredicate, edgePredicate)
                => graph.FilterByUndirected(vertexPredicate, edgePredicate));
        }

        [Test]
        public void AdjacentEdges_Throws()
        {
            var graph1 = new UndirectedGraph<EquatableTestVertex, Edge<EquatableTestVertex>>();
            var filteredGraph1 = new FilteredUndirectedGraph
                <
                    EquatableTestVertex,
                    Edge<EquatableTestVertex>,
                    UndirectedGraph<EquatableTestVertex, Edge<EquatableTestVertex>>
                >(
                graph1,
                _ => true,
                _ => true);
            AdjacentEdges_NullThrows_Test(filteredGraph1);
            AdjacentEdges_Throws_Test(filteredGraph1);

            var graph2 = new UndirectedGraph<int, IEdge<int>>();
            var filteredGraph2 = new FilteredUndirectedGraph<int, IEdge<int>, UndirectedGraph<int, IEdge<int>>>(
                graph2,
                vertex => vertex < 4,
                _ => true);

            graph2.AddVertexRange( 1, 2, 3, 4, 5 );
            // ReSharper disable ReturnValueOfPureMethodIsNotUsed
            Assert.IsNull(filteredGraph2.AdjacentEdges(4));
            Assert.IsNull(filteredGraph2.AdjacentEdges(5));
            // ReSharper restore ReturnValueOfPureMethodIsNotUsed
        }

        #endregion

        #region Try Get Edges

        [Test]
        public void TryGetEdge()
        {
            var graph = new UndirectedGraph<int, IEdge<int>>();
            TryGetEdge_UndirectedGraph_Test(
                graph, (vertexPredicate, edgePredicate) =>
                    graph.FilterByUndirected(vertexPredicate, edgePredicate));
        }

        [Test]
        public void TryGetEdge_Throws()
        {
            var filteredGraph = new FilteredUndirectedGraph<TestVertex, IEdge<TestVertex>, UndirectedGraph<TestVertex, IEdge<TestVertex>>>(
                new UndirectedGraph<TestVertex, IEdge<TestVertex>>(),
                _ => true,
                _ => true);
            TryGetEdge_Throws_UndirectedGraph_Test(filteredGraph);
        }

        #endregion
    }
}