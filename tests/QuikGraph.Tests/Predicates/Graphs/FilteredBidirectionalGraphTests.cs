using System;
using NUnit.Framework;
using QuikGraph.Predicates;
using static QuikGraph.Tests.GraphTestHelpers;

namespace QuikGraph.Tests.Predicates
{
    /// <summary>
    /// Tests for <see cref="FilteredBidirectionalGraph{TVertex,TEdge,TGraph}"/>.
    /// </summary>
    [TestFixture]
    internal sealed class FilteredBidirectionalGraphTests : FilteredGraphTestsBase
    {
        [Test]
        public void Construction()
        {
            Func<int, bool> vertexPredicate = _ => true;
            Func<Edge<int>, bool> edgePredicate = _ => true;

            var graph = new BidirectionalGraph<int, Edge<int>>();
            var filteredGraph = graph.FilteredBiDir(vertexPredicate, edgePredicate);
            AssertGraphProperties(filteredGraph, graph);

            graph = new BidirectionalGraph<int, Edge<int>>(false);
            filteredGraph = graph.FilteredBiDir(vertexPredicate, edgePredicate);
            AssertGraphProperties(filteredGraph, graph, false);

            #region Local function

            void AssertGraphProperties<TVertex, TEdge, TGraph>(
                FilteredBidirectionalGraph<TVertex, TEdge, TGraph> g,
                TGraph expectedGraph,
                bool parallelEdges = true)
                where TEdge : IEdge<TVertex>
                where TGraph : IBidirectionalGraph<TVertex, TEdge>
            {
                Assert.AreSame(expectedGraph, g.BaseGraph);
                Assert.IsTrue(g.IsDirected);
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
            Assert.Throws<ArgumentNullException>(()
                    => new BidirectionalGraph<int, Edge<int>>()
                        .FilteredBiDir<int, Edge<int>, BidirectionalGraph<int, Edge<int>>>(
                    _ => true,
                    null));

            Assert.Throws<ArgumentNullException>(()
                => new BidirectionalGraph<int, Edge<int>>()
                .FilteredBiDir<int, Edge<int>, BidirectionalGraph<int, Edge<int>>>(
                    null,
                    _ => true));

            Assert.Throws<ArgumentNullException>(
                () => new FilteredBidirectionalGraph<int, IEdge<int>, BidirectionalGraph<int, IEdge<int>>>(
                    null,
                    _ => true,
                    _ => true));

            Assert.Throws<ArgumentNullException>(()
                => new BidirectionalGraph<int, Edge<int>>()
                .FilteredBiDir<int, Edge<int>, BidirectionalGraph<int, Edge<int>>>(
                    null,
                    null));

            Assert.Throws<ArgumentNullException>(
                () => new FilteredBidirectionalGraph<int, IEdge<int>, BidirectionalGraph<int, IEdge<int>>>(
                    null,
                    _ => true,
                    null));

            Assert.Throws<ArgumentNullException>(
                () => new FilteredBidirectionalGraph<int, IEdge<int>, BidirectionalGraph<int, IEdge<int>>>(
                    null,
                    null,
                    _ => true));

            Assert.Throws<ArgumentNullException>(
                () => new FilteredBidirectionalGraph<int, IEdge<int>, BidirectionalGraph<int, IEdge<int>>>(
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
            var wrappedGraph = new BidirectionalGraph<int, IEdge<int>>();
            Vertices_Test(wrappedGraph, (vertexPredicate, edgePredicate)
                => wrappedGraph.FilteredBiDir(vertexPredicate, edgePredicate));
        }

        [Test]
        public void Edges()
        {
            var wrappedGraph = new BidirectionalGraph<int, IEdge<int>>();
            Edges_Test(wrappedGraph, (vertexPredicate, edgePredicate)
                => wrappedGraph.FilteredBiDir(vertexPredicate, edgePredicate));
        }

        #endregion

        #region Contains Vertex

        [Test]
        public void ContainsVertex()
        {
            var graph = new BidirectionalGraph<int, IEdge<int>>();
            ContainsVertex_Test(graph, (vertexPredicate, edgePredicate)
                => graph.FilteredBiDir(vertexPredicate, edgePredicate));
        }

        [Test]
        public void ContainsVertex_Throws()
        {
            var filteredGraph = new BidirectionalGraph<TestVertex, IEdge<TestVertex>>()
                .FilteredBiDir<TestVertex, IEdge<TestVertex>, BidirectionalGraph<TestVertex, IEdge<TestVertex>>>(
                _ => true,
                _ => true);
            ContainsVertex_Throws_Test(filteredGraph);
        }

        #endregion

        #region Contains Edge

        [Test]
        public void ContainsEdge()
        {
            var graph = new BidirectionalGraph<int, IEdge<int>>();
            ContainsEdge_Test(graph, (vertexPredicate, edgePredicate)
                => graph.FilteredBiDir(vertexPredicate, edgePredicate));
        }

        [Test]
        public void ContainsEdge_EquatableEdge()
        {
            var graph = new BidirectionalGraph<int, EquatableEdge<int>>();
            ContainsEdge_EquatableEdge_Test(graph, (vertexPredicate, edgePredicate)
                => graph.FilteredBiDir(vertexPredicate, edgePredicate));
        }

        [Test]
        public void ContainsEdge_SourceTarget()
        {
            var graph = new BidirectionalGraph<int, IEdge<int>>();
            ContainsEdge_SourceTarget_Test(graph, (vertexPredicate, edgePredicate)
                => graph.FilteredBiDir(vertexPredicate, edgePredicate));
        }

        [Test]
        public void ContainsEdge_Throws()
        {
            var filteredGraph = new BidirectionalGraph<TestVertex, IEdge<TestVertex>>()
                .FilteredBiDir<TestVertex, IEdge<TestVertex>, BidirectionalGraph<TestVertex, IEdge<TestVertex>>>(
                _ => true,
                _ => true);
            ContainsEdge_NullThrows_Test(filteredGraph);
            ContainsEdge_SourceTarget_Throws_Test(filteredGraph);
        }

        #endregion

        #region Out Edges

        [Test]
        public void OutEdge()
        {
            var graph = new BidirectionalGraph<int, IEdge<int>>();
            OutEdge_Test(graph, (vertexPredicate, edgePredicate)
                => graph.FilteredBiDir(vertexPredicate, edgePredicate));
        }

        [Test]
        public void OutEdge_Throws()
        {
            var graph1 = new BidirectionalGraph<int, IEdge<int>>();
            OutEdge_Throws_Test(graph1, (vertexPredicate, edgePredicate)
                => graph1.FilteredBiDir(vertexPredicate, edgePredicate));
        }

        [Test]
        public void OutEdgeUnFiltered_Throws()
        {
            var graph2 = new BidirectionalGraph<TestVertex, IEdge<TestVertex>>();
            var filteredGraph2 = graph2.FilteredBiDir<TestVertex, IEdge<TestVertex>, BidirectionalGraph<TestVertex, IEdge<TestVertex>>>(
                _ => true,
                _ => true);
            OutEdge_NullThrows_Test(filteredGraph2);
        }

        [Test]
        public void OutEdges()
        {
            var graph = new BidirectionalGraph<int, IEdge<int>>();
            OutEdges_Test(graph, (vertexPredicate, edgePredicate)
                => graph.FilteredBiDir(vertexPredicate, edgePredicate));
        }

        [Test]
        public void OutEdgesUnFiltered_Throws()
        {
            var graph1 = new BidirectionalGraph<EquatableTestVertex, Edge<EquatableTestVertex>>();
            var filteredGraph1 = graph1.FilteredBiDir
                <
                    EquatableTestVertex,
                    Edge<EquatableTestVertex>,
                    BidirectionalGraph<EquatableTestVertex, Edge<EquatableTestVertex>>
                >(
                _ => true,
                _ => true);
            OutEdges_NullThrows_Test(filteredGraph1);
            OutEdges_Throws_Test(filteredGraph1);
        }

        [Test]
        public void OutEdges_Throws()
        {
            var graph2 = new BidirectionalGraph<int, Edge<int>>();
            var filteredGraph2 = graph2.FilteredBiDir<int, Edge<int>, BidirectionalGraph<int, Edge<int>>>(
                vertex => vertex < 4,
                _ => true);

            graph2.AddVertexRange( 1, 2, 3, 4, 5);
            // ReSharper disable ReturnValueOfPureMethodIsNotUsed
            Assert.IsEmpty(filteredGraph2.OutEdges(4));
            Assert.IsEmpty(filteredGraph2.OutEdges(5));
            // ReSharper restore ReturnValueOfPureMethodIsNotUsed
        }

        #endregion

        #region In Edges

        [Test]
        public void InEdge()
        {
            var graph = new BidirectionalGraph<int, IEdge<int>>();
            InEdge_Test(graph, (vertexPredicate, edgePredicate)
                => graph.FilteredBiDir(vertexPredicate, edgePredicate));
        }

        [Test]
        public void InEdge_Throws()
        {
            var graph1 = new BidirectionalGraph<int, IEdge<int>>();
            InEdge_Throws_Test(graph1, (vertexPredicate, edgePredicate)
                => graph1.FilteredBiDir(vertexPredicate, edgePredicate));
        }

        [Test]
        public void InEdgeUnfiltered_Throws()
        {
            var graph2 = new BidirectionalGraph<TestVertex, IEdge<TestVertex>>();
            var filteredGraph2 = graph2.FilteredBiDir<TestVertex, IEdge<TestVertex>, BidirectionalGraph<TestVertex, IEdge<TestVertex>>>(
                _ => true,
                _ => true);
            InEdge_NullThrows_Test(filteredGraph2);
        }

        [Test]
        public void InEdges()
        {
            var graph = new BidirectionalGraph<int, IEdge<int>>();
            InEdges_Test(graph, (vertexPredicate, edgePredicate)
                => graph.FilteredBiDir(vertexPredicate, edgePredicate));
        }

        [Test]
        public void InEdgesUnFiltered_Throws()
        {
            var graph1 = new BidirectionalGraph<EquatableTestVertex, Edge<EquatableTestVertex>>();
            var filteredGraph1 = graph1.FilteredBiDir
            <
                EquatableTestVertex,
                Edge<EquatableTestVertex>,
                BidirectionalGraph<EquatableTestVertex, Edge<EquatableTestVertex>>
            >(
                _ => true,
                _ => true);
            InEdges_NullThrows_Test(filteredGraph1);
            InEdges_Throws_Test(filteredGraph1);
        }

        [Test]
        public void InEdges_Throws()
        {
            var graph2 = new BidirectionalGraph<int, Edge<int>>();
            var filteredGraph2 = graph2.FilteredBiDir<int, Edge<int>, BidirectionalGraph<int, Edge<int>>>(
                vertex => vertex < 4,
                _ => true);

            graph2.AddVertexRange( 1, 2, 3, 4, 5 );
            // ReSharper disable ReturnValueOfPureMethodIsNotUsed
            Assert.IsNull(filteredGraph2.InEdges(4));
            Assert.IsNull(filteredGraph2.InEdges(5));
            // ReSharper restore ReturnValueOfPureMethodIsNotUsed
        }

        #endregion

        #region Degree

        [Test]
        public void Degree()
        {
            var graph = new BidirectionalGraph<int, IEdge<int>>();
            Degree_Test(graph, (vertexPredicate, edgePredicate)
                => graph.FilteredBiDir(vertexPredicate, edgePredicate));
        }

        [Test]
        public void Degree_Throws()
        {
            var graph = new BidirectionalGraph<EquatableTestVertex, Edge<EquatableTestVertex>>();
            var filteredGraph = graph.FilteredBiDir
            <
                EquatableTestVertex,
                Edge<EquatableTestVertex>,
                BidirectionalGraph<EquatableTestVertex, Edge<EquatableTestVertex>>
            >(_ => true, _ => true);
            Degree_Throws_Test(filteredGraph);
        }

        #endregion

        #region Try Get Edges

        [Test]
        public void TryGetEdge()
        {
            var graph = new BidirectionalGraph<int, IEdge<int>>();
            TryGetEdge_Test(graph, (vertexPredicate, edgePredicate)
                => graph.FilteredBiDir(vertexPredicate, edgePredicate));
        }

        [Test]
        public void TryGetEdge_Throws()
        {
            var filteredGraph = new BidirectionalGraph<TestVertex, IEdge<TestVertex>>()
                .FilteredBiDir<TestVertex, IEdge<TestVertex>, BidirectionalGraph<TestVertex, IEdge<TestVertex>>>(
                _ => true,
                _ => true);
            TryGetEdge_Throws_Test(filteredGraph);
        }

        [Test]
        public void GetEdges()
        {
            var graph = new BidirectionalGraph<int, IEdge<int>>();
            GetEdges_Test(
                graph,
                (vertexPredicate, edgePredicate)
                    => graph.FilteredBiDir(vertexPredicate, edgePredicate));
        }

        [Test]
        public void GetEdges_Throws()
        {
            var filteredGraph =
                new BidirectionalGraph<TestVertex, IEdge<TestVertex>>()
                    .FilteredBiDir<TestVertex, IEdge<TestVertex>, BidirectionalGraph<TestVertex, IEdge<TestVertex>>>(
                _ => true,
                _ => true);
            GetEdges_Throws_Test(filteredGraph);
        }

        [Test]
        public void GetOutEdges()
        {
            var graph = new BidirectionalGraph<int, IEdge<int>>();
            GetOutEdges_Test(graph, (vertexPredicate, edgePredicate)
                => graph.FilteredBiDir(vertexPredicate, edgePredicate));
        }

        [Test]
        public void GetOutEdges_Throws()
        {
            GetOutEdges_Throws_Test(new BidirectionalGraph<TestVertex, IEdge<TestVertex>>()
                .FilteredBiDir<TestVertex, IEdge<TestVertex>, BidirectionalGraph<TestVertex, IEdge<TestVertex>>>(
                    _ => true,
                    _ => true));
        }

        [Test]
        public void GetInEdges()
        {
            var graph = new BidirectionalGraph<int, IEdge<int>>();
            GetInEdges_Test(graph, (vertexPredicate, edgePredicate)
                => graph.FilteredBiDir(vertexPredicate, edgePredicate));
        }

        [Test]
        public void GetInEdges_Throws()
        {
            GetInEdges_Throws_Test(
                    new BidirectionalGraph<TestVertex, IEdge<TestVertex>>()
                        .FilteredBiDir<TestVertex, IEdge<TestVertex>, BidirectionalGraph<TestVertex, IEdge<TestVertex>>>(
                    _ => true,
                    _ => true));
        }

        #endregion
    }
}