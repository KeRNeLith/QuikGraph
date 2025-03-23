using System;
using NUnit.Framework;
using static QuikGraph.Tests.GraphTestHelpers;

namespace QuikGraph.Tests.Structures
{
    /// <summary>
    /// Tests for <see cref="ReversedBidirectionalGraph{TVertex,TEdge}"/>.
    /// </summary>
    [TestFixture]
    internal sealed class ReversedBidirectionalGraphTests : GraphTestsBase
    {
        [Test]
        public void Construction()
        {
            var wrappedGraph = new BidirectionalGraph<int, IEdge<int>>();
            var graph = wrappedGraph.CreateReversedBidirectionalGraph();
            AssertGraphProperties(graph);

            wrappedGraph = new BidirectionalGraph<int, IEdge<int>>(true);
            graph = wrappedGraph.CreateReversedBidirectionalGraph();
            AssertGraphProperties(graph);

            wrappedGraph = new BidirectionalGraph<int, IEdge<int>>(false);
            graph = wrappedGraph.CreateReversedBidirectionalGraph();
            AssertGraphProperties(graph, false);

            #region Local function

            void AssertGraphProperties<TVertex, TEdge>(
                ReversedBidirectionalGraph<TVertex, TEdge> g,
                bool parallelEdges = true)
                where TEdge : IEdge<TVertex>
            {
                Assert.IsTrue(g.IsDirected);
                Assert.AreEqual(parallelEdges, g.AllowParallelEdges);
                Assert.IsNotNull(g.OriginalGraph);
                AssertEmptyGraph(g);
            }

            #endregion
        }

        static readonly IBidirectionalGraph<int, IEdge<int>> nullGraph = null;

        [Test]
        public void Construction_Throws()
        {
            // ReSharper disable once ObjectCreationAsStatement
            // ReSharper disable once AssignNullToNotNullAttribute
            Assert.Throws<ArgumentNullException>(static () => _ = nullGraph.CreateReversedBidirectionalGraph());
        }

        #region Add Vertex => has effect

        [Test]
        public void AddVertex()
        {
            var wrappedGraph = new BidirectionalGraph<int, IEdge<int>>();
            AddVertex_ImmutableGraph_WithUpdate(
                wrappedGraph,
                () => wrappedGraph.CreateReversedBidirectionalGraph());
        }

        #endregion

        #region Add Edge => has effect

        [Test]
        public void AddEdge()
        {
            var wrappedGraph = new BidirectionalGraph<int, IEdge<int>>();
            var graph = wrappedGraph.CreateReversedBidirectionalGraph();

            var edge = Edge.Create(1, 2);
            wrappedGraph.AddVertex(1);
            wrappedGraph.AddVertex(2);
            wrappedGraph.AddEdge(edge);

            graph.AssertHasEdges(new[] { edge });  // Graph is updated
        }

        #endregion

        #region Contains Vertex

        [Test]
        public void ContainsVertex()
        {
            var wrappedGraph = new BidirectionalGraph<TestVertex, IEdge<TestVertex>>();
            ContainsVertex_ImmutableGraph_Test(
                wrappedGraph,
                () => wrappedGraph.CreateReversedBidirectionalGraph());
        }

        [Test]
        public void ContainsVertex_EquatableVertex()
        {
            var wrappedGraph = new BidirectionalGraph<EquatableTestVertex, Edge<EquatableTestVertex>>();
            ContainsVertex_EquatableVertex_ImmutableGraph_Test(
                wrappedGraph,
                () => wrappedGraph.CreateReversedBidirectionalGraph());
        }

        [Test]
        public void ContainsVertex_Throws()
        {
            var wrappedGraph = new BidirectionalGraph<TestVertex, IEdge<TestVertex>>();
            var graph = wrappedGraph.CreateReversedBidirectionalGraph();
            ContainsVertex_Throws_Test(graph);
        }

        #endregion

        #region Contains Edge

        [Test]
        public void ContainsEdge()
        {
            var wrappedGraph = new BidirectionalGraph<int, IEdge<int>>();
            ContainsEdge_ImmutableGraph_ReversedTest(
                wrappedGraph,
                () => wrappedGraph.CreateReversedBidirectionalGraph());
        }

        [Test]
        public void ContainsEdge_EquatableEdge()
        {
            var wrappedGraph = new BidirectionalGraph<int, EquatableEdge<int>>();
            ContainsEdge_EquatableEdge_ImmutableGraph_ReversedTest(
                wrappedGraph,
                () => wrappedGraph.CreateReversedBidirectionalGraph());
        }

        [Test]
        public void ContainsEdge_SourceTarget()
        {
            var wrappedGraph = new BidirectionalGraph<int, IEdge<int>>();
            ContainsEdge_SourceTarget_ImmutableGraph_ReversedTest(
                wrappedGraph,
                () => wrappedGraph.CreateReversedBidirectionalGraph());
        }

        [Test]
        public void ContainsEdge_Throws()
        {
            var wrappedGraph = new BidirectionalGraph<TestVertex, IEdge<TestVertex>>();
            var graph = wrappedGraph.CreateReversedBidirectionalGraph();
            ContainsEdge_NullThrows_ReversedTest(graph);
            ContainsEdge_SourceTarget_Throws_Test(graph);
        }

        #endregion

        #region Out Edges

        [Test]
        public void OutEdge()
        {
            var wrappedGraph = new BidirectionalGraph<int, IEdge<int>>();
            OutEdge_ImmutableGraph_ReversedTest(
                wrappedGraph,
                () => wrappedGraph.CreateReversedBidirectionalGraph());
        }

        [Test]
        public void OutEdge_Throws()
        {
            var wrappedGraph1 = new BidirectionalGraph<TestVertex, IEdge<TestVertex>>();
            var graph1 = wrappedGraph1.CreateReversedBidirectionalGraph();
            OutEdge_NullThrows_Test(graph1);

            var wrappedGraph2 = new BidirectionalGraph<int, IEdge<int>>();
            OutEdge_Throws_ImmutableGraph_ReversedTest(
                wrappedGraph2,
                () => wrappedGraph2.CreateReversedBidirectionalGraph());
        }

        [Test]
        public void OutEdges()
        {
            var wrappedGraph = new BidirectionalGraph<int, IEdge<int>>();
            OutEdges_ImmutableGraph_ReversedTest(
                wrappedGraph,
                () => wrappedGraph.CreateReversedBidirectionalGraph());
        }

        [Test]
        public void OutEdges_Throws()
        {
            var wrappedGraph1 = new BidirectionalGraph<TestVertex, IEdge<TestVertex>>();
            var graph1 = wrappedGraph1.CreateReversedBidirectionalGraph();
            OutEdges_NullThrows_Test(graph1);
        }

        #endregion

        #region In Edges

        [Test]
        public void InEdge()
        {
            var wrappedGraph = new BidirectionalGraph<int, IEdge<int>>();
            InEdge_ImmutableGraph_ReversedTest(
                wrappedGraph,
                () => wrappedGraph.CreateReversedBidirectionalGraph());
        }

        [Test]
        public void InEdge_Throws()
        {
            var wrappedGraph1 = new BidirectionalGraph<int, IEdge<int>>();
            InEdge_Throws_ImmutableGraph_ReversedTest(
                wrappedGraph1,
                () => wrappedGraph1.CreateReversedBidirectionalGraph());

            var wrappedGraph2 = new BidirectionalGraph<TestVertex, IEdge<TestVertex>>();
            var graph2 = wrappedGraph2.CreateReversedBidirectionalGraph();
            InEdge_NullThrows_Test(graph2);
        }

        [Test]
        public void InEdges()
        {
            var wrappedGraph = new BidirectionalGraph<int, IEdge<int>>();
            InEdges_ImmutableGraph_ReversedTest(
                wrappedGraph,
                () => wrappedGraph.CreateReversedBidirectionalGraph());
        }

        [Test]
        public void InEdges_Throws()
        {
            var wrappedGraph1 = new BidirectionalGraph<TestVertex, IEdge<TestVertex>>();
            var graph1 = wrappedGraph1.CreateReversedBidirectionalGraph();
            InEdges_NullThrows_Test(graph1);
        }

        #endregion

        [Test]
        public void Degree()
        {
            var wrappedGraph = new BidirectionalGraph<int, IEdge<int>>();
            Degree_ImmutableGraph_ReversedTest(
                wrappedGraph,
                () => wrappedGraph.CreateReversedBidirectionalGraph());
        }

        [Test]
        public void Degree_Throws()
        {
            var wrappedGraph = new BidirectionalGraph<EquatableTestVertex, Edge<EquatableTestVertex>>();
            var graph = wrappedGraph.CreateReversedBidirectionalGraph();
            Degree_Throws_Test(graph);
        }

        #region Try Get Edges

        [Test]
        public void TryGetEdge()
        {
            var wrappedGraph = new BidirectionalGraph<int, IEdge<int>>();
            GetEdge_ImmutableGraph_ReversedTest(
                wrappedGraph,
                () => wrappedGraph.CreateReversedBidirectionalGraph());
        }

        [Test]
        public void TryGetEdge_Throws()
        {
            var wrappedGraph = new BidirectionalGraph<TestVertex, IEdge<TestVertex>>();
            var graph = wrappedGraph.CreateReversedBidirectionalGraph();
            TryGetEdge_Throws_Test(graph);
        }

        [Test]
        public void GetEdges()
        {
            var wrappedGraph = new BidirectionalGraph<int, IEdge<int>>();
            GetEdges_ImmutableGraph_ReversedTest(wrappedGraph, () => wrappedGraph.CreateReversedBidirectionalGraph());
        }

        [Test]
        public void GetEdges_Throws()
        {
            var wrappedGraph = new BidirectionalGraph<TestVertex, IEdge<TestVertex>>();
            var graph = wrappedGraph.CreateReversedBidirectionalGraph();
            GetEdges_Throws_Test(graph);
        }

        [Test]
        public void GetOutEdges()
        {
            var wrappedGraph = new BidirectionalGraph<int, IEdge<int>>();
            GetOutEdges_ImmutableGraph_ReversedTest(
                wrappedGraph,
                () => wrappedGraph.CreateReversedBidirectionalGraph());
        }

        [Test]
        public void GetOutEdges_Throws()
        {
            var wrappedGraph = new BidirectionalGraph<TestVertex, IEdge<TestVertex>>();
            var graph = wrappedGraph.CreateReversedBidirectionalGraph();
            GetOutEdges_Throws_Test(graph);
        }

        [Test]
        public void GetInEdges()
        {
            var wrappedGraph = new BidirectionalGraph<int, IEdge<int>>();
            GetInEdges_ImmutableGraph_ReversedTest(
                wrappedGraph,
                () => wrappedGraph.CreateReversedBidirectionalGraph());
        }

        [Test]
        public void GetInEdges_Throws()
        {
            var wrappedGraph = new BidirectionalGraph<TestVertex, IEdge<TestVertex>>();
            var graph = wrappedGraph.CreateReversedBidirectionalGraph();
            GetInEdges_Throws_Test(graph);
        }

        #endregion
    }
}