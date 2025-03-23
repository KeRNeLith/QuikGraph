using System;
using NUnit.Framework;
using static QuikGraph.Tests.GraphTestHelpers;

namespace QuikGraph.Tests.Structures
{
    /// <summary>
    /// Tests for <see cref="UndirectedBidirectionalGraph{TVertex,TEdge}"/>.
    /// </summary>
    [TestFixture]
    internal sealed class UndirectedBidirectionalGraphTests : GraphTestsBase
    {
        [Test]
        public void Construction()
        {
            var wrappedGraph = new BidirectionalGraph<int, IEdge<int>>(true);
            var graph = new UndirectedBidirectionalGraph<int, IEdge<int>>(wrappedGraph);
            AssertGraphProperties(graph);

            wrappedGraph = new BidirectionalGraph<int, IEdge<int>>(false);
            graph = new UndirectedBidirectionalGraph<int, IEdge<int>>(wrappedGraph);
            AssertGraphProperties(graph, false);

            #region Local function

            void AssertGraphProperties<TVertex, TEdge>(UndirectedBidirectionalGraph<TVertex, TEdge> g, bool parallelEdges = true)
                where TEdge : IEdge<TVertex>
            {
                Assert.IsFalse(g.IsDirected);
                Assert.AreEqual(parallelEdges, g.AllowParallelEdges);
                Assert.IsNotNull(g.OriginalGraph);
                AssertEmptyGraph(g);
            }

            #endregion
        }

        [Test]
        public void Construction_Throws()
        {
            // ReSharper disable once ObjectCreationAsStatement
            // ReSharper disable once AssignNullToNotNullAttribute
            Assert.Throws<ArgumentNullException>(() => new UndirectedBidirectionalGraph<int, IEdge<int>>(null));
        }

        #region Add Vertex => has effect

        [Test]
        public void AddVertex()
        {
            var wrappedGraph = new BidirectionalGraph<int, IEdge<int>>();
            AddVertex_ImmutableGraph_WithUpdate(
                wrappedGraph,
                () => new UndirectedBidirectionalGraph<int, IEdge<int>>(wrappedGraph));
        }

        #endregion

        #region Add Edge => has effect

        [Test]
        public void AddEdge()
        {
            var wrappedGraph = new BidirectionalGraph<int, IEdge<int>>();
            AddEdge_ImmutableGraph_WithUpdate(
                wrappedGraph,
                () => new UndirectedBidirectionalGraph<int, IEdge<int>>(wrappedGraph));
        }

        #endregion

        #region Contains Vertex

        [Test]
        public void ContainsVertex()
        {
            var wrappedGraph = new BidirectionalGraph<TestVertex, IEdge<TestVertex>>();
            ContainsVertex_ImmutableGraph_Test(
                wrappedGraph,
                () => new UndirectedBidirectionalGraph<TestVertex, IEdge<TestVertex>>(wrappedGraph));
        }

        [Test]
        public void ContainsVertex_EquatableVertex()
        {
            var wrappedGraph = new BidirectionalGraph<EquatableTestVertex, Edge<EquatableTestVertex>>();
            ContainsVertex_EquatableVertex_ImmutableGraph_Test(
                wrappedGraph,
                () => new UndirectedBidirectionalGraph<EquatableTestVertex, Edge<EquatableTestVertex>>(wrappedGraph));
        }

        [Test]
        public void ContainsVertex_Throws()
        {
            var wrappedGraph = new BidirectionalGraph<TestVertex, IEdge<TestVertex>>();
            var graph = new UndirectedBidirectionalGraph<TestVertex, IEdge<TestVertex>>(wrappedGraph);
            ContainsVertex_Throws_Test(graph);
        }

        #endregion

        #region Contains Edge

        [Test]
        public void ContainsEdge()
        {
            var wrappedGraph = new BidirectionalGraph<int, IEdge<int>>();
            ContainsEdge_ImmutableGraph_Test(
                wrappedGraph,
                () => new UndirectedBidirectionalGraph<int, IEdge<int>>(wrappedGraph));
        }

        [Test]
        public void ContainsEdge_EquatableEdge()
        {
            var wrappedGraph = new BidirectionalGraph<int, EquatableEdge<int>>();
            ContainsEdge_EquatableEdge_ImmutableGraph_Test(
                wrappedGraph,
                () => new UndirectedBidirectionalGraph<int, EquatableEdge<int>>(wrappedGraph));
        }

        [Test]
        public void ContainsEdge_SourceTarget()
        {
            var wrappedGraph = new BidirectionalGraph<int, IEdge<int>>();
            ContainsEdge_SourceTarget_ImmutableGraph_UndirectedGraph_Test(
                wrappedGraph,
                () => new UndirectedBidirectionalGraph<int, IEdge<int>>(wrappedGraph));
        }

        [Test]
        public void ContainsEdge_Throws()
        {
            var wrappedGraph = new BidirectionalGraph<TestVertex, IEdge<TestVertex>>();
            var graph = new UndirectedBidirectionalGraph<TestVertex, IEdge<TestVertex>>(wrappedGraph);
            ContainsEdge_NullThrows_Test(graph);
            ContainsEdge_SourceTarget_Throws_UndirectedGraph_Test(graph);
        }

        [Test]
        public void ContainsEdge_Undirected()
        {
            var wrappedGraph1 = new BidirectionalGraph<int, EquatableEdge<int>>();
            var wrappedGraph2 = new BidirectionalGraph<int, EquatableUndirectedEdge<int>>();
            ContainsEdge_UndirectedEdge_ImmutableGraph_UndirectedGraph_Test(
                wrappedGraph1,
                () =>  new UndirectedBidirectionalGraph<int, EquatableEdge<int>>(wrappedGraph1),
                wrappedGraph2,
                () => new UndirectedBidirectionalGraph<int, EquatableUndirectedEdge<int>>(wrappedGraph2));
        }

        #endregion

        #region Adjacent Edges

        [Test]
        public void AdjacentEdge_Throws()
        {
            var wrappedGraph = new BidirectionalGraph<int, IEdge<int>>();
            var graph = new UndirectedBidirectionalGraph<int, IEdge<int>>(wrappedGraph);
            // ReSharper disable once ReturnValueOfPureMethodIsNotUsed
            Assert.Throws<NotSupportedException>(() => graph.AdjacentEdge(1, 0));
        }

        [Test]
        public void AdjacentEdges()
        {
            var wrappedGraph = new BidirectionalGraph<int, IEdge<int>>();
            AdjacentEdges_ImmutableGraph_Test(
                wrappedGraph,
                () => new UndirectedBidirectionalGraph<int, IEdge<int>>(wrappedGraph));
        }

        [Test]
        public void AdjacentEdges_Throws()
        {
            var wrappedGraph = new BidirectionalGraph<EquatableTestVertex, Edge<EquatableTestVertex>>();
            var graph = new UndirectedBidirectionalGraph<EquatableTestVertex, Edge<EquatableTestVertex>>(wrappedGraph);
            AdjacentEdges_NullThrows_Test(graph);

            var vertex = new EquatableTestVertex();
            Assert.IsTrue(graph.IsAdjacentEdgesEmpty(vertex));
            Assert.AreEqual(null, graph.AdjacentDegree(vertex));
            Assert.IsEmpty(graph.AdjacentEdges(vertex));
        }

        #endregion

        #region Try Get Edges

        [Test]
        public void TryGetEdge()
        {
            var wrappedGraph = new BidirectionalGraph<int, IEdge<int>>();
            TryGetEdge_ImmutableGraph_UndirectedGraph_Test(
                wrappedGraph,
                () => new UndirectedBidirectionalGraph<int, IEdge<int>>(wrappedGraph));
        }

        [Test]
        public void TryGetEdge_Throws()
        {
            var wrappedGraph = new BidirectionalGraph<TestVertex, IEdge<TestVertex>>();
            var graph = new UndirectedBidirectionalGraph<TestVertex, IEdge<TestVertex>>(wrappedGraph);
            TryGetEdge_Throws_UndirectedGraph_Test(graph);
        }

        #endregion
    }
}