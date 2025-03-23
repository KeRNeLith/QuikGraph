using System;
using NUnit.Framework;
using QuikGraph.Predicates;

namespace QuikGraph.Tests.Predicates
{
    /// <summary>
    /// Tests for <see cref="FilteredImplicitGraph{TVertex,TEdge,TGraph}"/>.
    /// </summary>
    [TestFixture]
    internal sealed class FilteredImplicitGraphTests : FilteredGraphTestsBase
    {
        [Test]
        public void Construction()
        {
            Func<int, bool> vertexPredicate = _ => true;
            Func<Edge<int>, bool> edgePredicate = _ => true;

            var graph = new AdjacencyGraph<int, Edge<int>>();
            var filteredGraph = graph.FilteredBy(
                vertexPredicate,
                edgePredicate);
            AssertGraphProperties(filteredGraph, graph);

            graph = new AdjacencyGraph<int, Edge<int>>(false);
            filteredGraph = graph.FilteredBy(
                vertexPredicate,
                edgePredicate);
            AssertGraphProperties(filteredGraph, graph, false);

            #region Local function

            void AssertGraphProperties<TVertex, TEdge, TGraph>(
                FilteredImplicitGraph<TVertex, TEdge, TGraph> g,
                TGraph expectedGraph,
                bool parallelEdges = true)
                where TEdge : IEdge<TVertex>
                where TGraph : IImplicitGraph<TVertex, TEdge>
            {
                Assert.AreSame(expectedGraph, g.BaseGraph);
                Assert.IsTrue(g.IsDirected);
                Assert.AreEqual(parallelEdges, g.AllowParallelEdges);
                Assert.AreSame(vertexPredicate, g.VertexPredicate);
                Assert.AreSame(edgePredicate, g.EdgePredicate);
            }

            #endregion
        }

        [Test]
        public void Construction_Throws()
        {
            // ReSharper disable ObjectCreationAsStatement
            // ReSharper disable AssignNullToNotNullAttribute
            Assert.Throws<ArgumentNullException>(
                () => new FilteredImplicitGraph<int, IEdge<int>, AdjacencyGraph<int, IEdge<int>>>(
                    new AdjacencyGraph<int, IEdge<int>>(),
                    _ => true,
                    null));

            Assert.Throws<ArgumentNullException>(
                () => new FilteredImplicitGraph<int, IEdge<int>, AdjacencyGraph<int, IEdge<int>>>(
                    new AdjacencyGraph<int, IEdge<int>>(),
                    null,
                    _ => true));

            Assert.Throws<ArgumentNullException>(
                () => new FilteredImplicitGraph<int, IEdge<int>, AdjacencyGraph<int, IEdge<int>>>(
                    null,
                    _ => true,
                    _ => true));

            Assert.Throws<ArgumentNullException>(
                () => new FilteredImplicitGraph<int, IEdge<int>, AdjacencyGraph<int, IEdge<int>>>(
                    new AdjacencyGraph<int, IEdge<int>>(),
                    null,
                    null));

            Assert.Throws<ArgumentNullException>(
                () => new FilteredImplicitGraph<int, IEdge<int>, AdjacencyGraph<int, IEdge<int>>>(
                    null,
                    _ => true,
                    null));

            Assert.Throws<ArgumentNullException>(
                () => new FilteredImplicitGraph<int, IEdge<int>, AdjacencyGraph<int, IEdge<int>>>(
                    null,
                    null,
                    _ => true));

            Assert.Throws<ArgumentNullException>(
                () => new FilteredImplicitGraph<int, IEdge<int>, AdjacencyGraph<int, IEdge<int>>>(
                    null,
                    null,
                    null));
            // ReSharper restore AssignNullToNotNullAttribute
            // ReSharper restore ObjectCreationAsStatement
        }

        #region Contains Vertex

        [Test]
        public void ContainsVertex()
        {
            var graph = new AdjacencyGraph<int, IEdge<int>>();
            ContainsVertex_Test(
                graph,
                (vertexPredicate, edgePredicate) => 
                    new FilteredImplicitGraph<int, IEdge<int>, AdjacencyGraph<int, IEdge<int>>>(
                        graph,
                        vertexPredicate,
                        edgePredicate));
        }

        [Test]
        public void ContainsVertex_Throws()
        {
            var filteredGraph = new FilteredImplicitGraph<TestVertex, IEdge<TestVertex>, AdjacencyGraph<TestVertex, IEdge<TestVertex>>>(
                new AdjacencyGraph<TestVertex, IEdge<TestVertex>>(),
                _ => true,
                _ => true);
            ContainsVertex_Throws_Test(filteredGraph);
        }

        #endregion

        #region Out Edges

        [Test]
        public void OutEdge()
        {
            var graph = new AdjacencyGraph<int, IEdge<int>>();
            OutEdge_Test(
                graph,
                (vertexPredicate, edgePredicate) =>
                    new FilteredImplicitGraph<int, IEdge<int>, AdjacencyGraph<int, IEdge<int>>>(
                        graph,
                        vertexPredicate,
                        edgePredicate));
        }

        [Test]
        public void OutEdge_Throws()
        {
            var graph1 = new AdjacencyGraph<int, IEdge<int>>();
            OutEdge_Throws_Test(
                graph1,
                (vertexPredicate, edgePredicate) =>
                    new FilteredImplicitGraph<int, IEdge<int>, AdjacencyGraph<int, IEdge<int>>>(
                        graph1,
                        vertexPredicate,
                        edgePredicate));

            var graph2 = new AdjacencyGraph<TestVertex, IEdge<TestVertex>>();
            var filteredGraph2 = new FilteredImplicitGraph<TestVertex, IEdge<TestVertex>, AdjacencyGraph<TestVertex, IEdge<TestVertex>>>(
                graph2,
                _ => true,
                _ => true);
            OutEdge_NullThrows_Test(filteredGraph2);
        }

        [Test]
        public void OutEdges()
        {
            var graph = new AdjacencyGraph<int, IEdge<int>>();
            OutEdges_Test(
                graph,
                (vertexPredicate, edgePredicate) =>
                    new FilteredImplicitGraph<int, IEdge<int>, AdjacencyGraph<int, IEdge<int>>>(
                        graph,
                        vertexPredicate,
                        edgePredicate));
        }

        [Test]
        public void OutEdges_Throws()
        {
            var graph1 = new AdjacencyGraph<EquatableTestVertex, Edge<EquatableTestVertex>>();
            var filteredGraph1 = new FilteredImplicitGraph
                <
                    EquatableTestVertex,
                    Edge<EquatableTestVertex>,
                    AdjacencyGraph<EquatableTestVertex, Edge<EquatableTestVertex>>
                >(
                graph1,
                _ => true,
                _ => true);
            OutEdges_NullThrows_Test(filteredGraph1);
            OutEdges_Throws_Test(filteredGraph1);

            var graph2 = new AdjacencyGraph<int, IEdge<int>>();
            var filteredGraph2 = new FilteredImplicitGraph<int, IEdge<int>, AdjacencyGraph<int, IEdge<int>>>(
                graph2,
                vertex => vertex < 4,
                _ => true);

            graph2.AddVertexRange( 1, 2, 3, 4, 5 );
            // ReSharper disable ReturnValueOfPureMethodIsNotUsed
            Assert.IsEmpty(filteredGraph2.OutEdges(4));
            Assert.IsEmpty(filteredGraph2.OutEdges(5));
            // ReSharper restore ReturnValueOfPureMethodIsNotUsed
        }

        #endregion

        #region Try Get Edges

        [Test]
        public void GetOutEdges()
        {
            var graph = new AdjacencyGraph<int, IEdge<int>>();
            GetOutEdges_Test(
                graph,
                (vertexPredicate, edgePredicate) =>
                    new FilteredImplicitGraph<int, IEdge<int>, AdjacencyGraph<int, IEdge<int>>>(
                        graph,
                        vertexPredicate,
                        edgePredicate));
        }

        [Test]
        public void GetOutEdges_Throws()
        {
            GetOutEdges_Throws_Test(
                new FilteredImplicitGraph<TestVertex, IEdge<TestVertex>, AdjacencyGraph<TestVertex, IEdge<TestVertex>>>(
                    new AdjacencyGraph<TestVertex, IEdge<TestVertex>>(),
                    _ => true,
                    _ => true));
        }

        #endregion
    }
}