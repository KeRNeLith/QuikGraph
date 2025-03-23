using System;
using NUnit.Framework;
using QuikGraph.Predicates;

namespace QuikGraph.Tests.Predicates
{
    /// <summary>
    /// Tests for <see cref="FilteredImplicitVertexSet{TVertex,TEdge,TGraph}"/>.
    /// </summary>
    [TestFixture]
    internal sealed class FilteredImplicitVertexSetGraphTests : FilteredGraphTestsBase
    {
        [Test]
        public void Construction()
        {
            Func<int, bool> vertexPredicate = _ => true;
            Func<IEdge<int>, bool> edgePredicate = _ => true;

            var graph1 = new AdjacencyGraph<int, IEdge<int>>();
            var filteredGraph1 = new FilteredImplicitVertexSet<int, IEdge<int>, AdjacencyGraph<int, IEdge<int>>>(
                graph1,
                vertexPredicate,
                edgePredicate);
            AssertGraphProperties(filteredGraph1, graph1);

            graph1 = new AdjacencyGraph<int, IEdge<int>>(false);
            filteredGraph1 = new FilteredImplicitVertexSet<int, IEdge<int>, AdjacencyGraph<int, IEdge<int>>>(
                graph1,
                vertexPredicate,
                edgePredicate);
            AssertGraphProperties(filteredGraph1, graph1, parallelEdges: false);

            var graph2 = new UndirectedGraph<int, IEdge<int>>();
            var filteredGraph2 = new FilteredImplicitVertexSet<int, IEdge<int>, UndirectedGraph<int, IEdge<int>>>(
                graph2,
                vertexPredicate,
                edgePredicate);
            AssertGraphProperties(filteredGraph2, graph2, false);

            graph2 = new UndirectedGraph<int, IEdge<int>>(false);
            filteredGraph2 = new FilteredImplicitVertexSet<int, IEdge<int>, UndirectedGraph<int, IEdge<int>>>(
                graph2,
                vertexPredicate,
                edgePredicate);
            AssertGraphProperties(filteredGraph2, graph2, false, false);

            #region Local function

            void AssertGraphProperties<TVertex, TEdge, TGraph>(
                FilteredImplicitVertexSet<TVertex, TEdge, TGraph> g,
                TGraph expectedGraph,
                bool isDirected = true,
                bool parallelEdges = true)
                where TEdge : IEdge<TVertex>
                where TGraph : IGraph<TVertex, TEdge>, IImplicitVertexSet<TVertex>
            {
                Assert.AreSame(expectedGraph, g.BaseGraph);
                Assert.AreEqual(isDirected, g.IsDirected);
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
                () => new FilteredImplicitVertexSet<int, IEdge<int>, AdjacencyGraph<int, IEdge<int>>>(
                    new AdjacencyGraph<int, IEdge<int>>(),
                    _ => true,
                    null));

            Assert.Throws<ArgumentNullException>(
                () => new FilteredImplicitVertexSet<int, IEdge<int>, AdjacencyGraph<int, IEdge<int>>>(
                    new AdjacencyGraph<int, IEdge<int>>(),
                    null,
                    _ => true));

            Assert.Throws<ArgumentNullException>(
                () => new FilteredImplicitVertexSet<int, IEdge<int>, AdjacencyGraph<int, IEdge<int>>>(
                    null,
                    _ => true,
                    _ => true));

            Assert.Throws<ArgumentNullException>(
                () => new FilteredImplicitVertexSet<int, IEdge<int>, AdjacencyGraph<int, IEdge<int>>>(
                    new AdjacencyGraph<int, IEdge<int>>(),
                    null,
                    null));

            Assert.Throws<ArgumentNullException>(
                () => new FilteredImplicitVertexSet<int, IEdge<int>, AdjacencyGraph<int, IEdge<int>>>(
                    null,
                    _ => true,
                    null));

            Assert.Throws<ArgumentNullException>(
                () => new FilteredImplicitVertexSet<int, IEdge<int>, AdjacencyGraph<int, IEdge<int>>>(
                    null,
                    null,
                    _ => true));

            Assert.Throws<ArgumentNullException>(
                () => new FilteredImplicitVertexSet<int, IEdge<int>, AdjacencyGraph<int, IEdge<int>>>(
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
                    new FilteredImplicitVertexSet<int, IEdge<int>, AdjacencyGraph<int, IEdge<int>>>(
                        graph,
                        vertexPredicate,
                        edgePredicate));
        }

        [Test]
        public void ContainsVertex_Throws()
        {
            var filteredGraph = new FilteredImplicitVertexSet<TestVertex, IEdge<TestVertex>, AdjacencyGraph<TestVertex, IEdge<TestVertex>>>(
                new AdjacencyGraph<TestVertex, IEdge<TestVertex>>(),
                _ => true,
                _ => true);
            ContainsVertex_Throws_Test(filteredGraph);
        }

        #endregion
    }
}