using System;
using NUnit.Framework;
using static QuikGraph.Tests.GraphTestHelpers;

namespace QuikGraph.Tests.Structures
{
    /// <summary>
    /// Tests for <see cref="ArrayBidirectionalGraph{TVertex,TEdge}"/>.
    /// </summary>
    [TestFixture]
    internal sealed class ArrayBidirectionalGraphTests : GraphTestsBase
    {
        [Test]
        public void Construction()
        {
            var wrappedGraph = new BidirectionalGraph<int, IEdge<int>>();

            var graph = new ArrayBidirectionalGraph<int, IEdge<int>>(wrappedGraph);
            AssertGraphProperties(graph);
            AssertEmptyGraph(graph);

            wrappedGraph.AddVertexRange( 2, 3, 1 );
            graph = new ArrayBidirectionalGraph<int, IEdge<int>>(wrappedGraph);
            AssertGraphProperties(graph);
            graph.AssertHasVertices(1, 2, 3 );
            graph.AssertNoEdge();

            var edge1 = Edge.Create(1, 2);
            var edge2 = Edge.Create(2, 2);
            var edge3 = Edge.Create(3, 4);
            var edge4 = Edge.Create(1, 4);
            wrappedGraph.AddVerticesAndEdgeRange( edge1, edge2, edge3, edge4 );
            graph = new ArrayBidirectionalGraph<int, IEdge<int>>(wrappedGraph);
            AssertGraphProperties(graph);
            graph.AssertHasVertices(1, 2, 3, 4 );
            graph.AssertHasEdges(edge1, edge2, edge3, edge4 );

            wrappedGraph = new BidirectionalGraph<int, IEdge<int>>(false);
            wrappedGraph.AddVerticesAndEdgeRange( edge1, edge1, edge2, edge3, edge4 );
            graph = new ArrayBidirectionalGraph<int, IEdge<int>>(wrappedGraph);
            AssertGraphProperties(graph, false);
            graph.AssertHasVertices(1, 2, 3, 4 );
            graph.AssertHasEdges(edge1, edge2, edge3, edge4 );

            #region Local function

            void AssertGraphProperties<TVertex, TEdge>(
                ArrayBidirectionalGraph<TVertex, TEdge> g,
                bool allowParallelEdges = true)
                where TEdge : IEdge<TVertex>
            {
                Assert.IsTrue(g.IsDirected);
                Assert.AreEqual(allowParallelEdges, g.AllowParallelEdges);
            }

            #endregion
        }

        [Test]
        public void Construction_Throws()
        {
            // ReSharper disable once ObjectCreationAsStatement
            // ReSharper disable once AssignNullToNotNullAttribute
            Assert.Throws<ArgumentNullException>(() => new ArrayBidirectionalGraph<int, IEdge<int>>(null));
        }

        #region Add Vertex => no effect

        [Test]
        public void AddVertex()
        {
            var wrappedGraph = new BidirectionalGraph<int, IEdge<int>>();
            AddVertex_ImmutableGraph_NoUpdate(
                wrappedGraph,
                () => new ArrayBidirectionalGraph<int, IEdge<int>>(wrappedGraph));
        }

        #endregion

        #region Add Edge => no effect

        [Test]
        public void AddEdge()
        {
            var wrappedGraph = new BidirectionalGraph<int, IEdge<int>>();
            AddEdge_ImmutableGraph_NoUpdate(
                wrappedGraph,
                () => new ArrayBidirectionalGraph<int, IEdge<int>>(wrappedGraph));
        }

        #endregion

        #region Contains Vertex

        [Test]
        public void ContainsVertex()
        {
            var wrappedGraph = new BidirectionalGraph<TestVertex, IEdge<TestVertex>>();
            ContainsVertex_ImmutableGraph_Test(
                wrappedGraph,
                () => new ArrayBidirectionalGraph<TestVertex, IEdge<TestVertex>>(wrappedGraph));
        }

        [Test]
        public void ContainsVertex_EquatableVertex()
        {
            var wrappedGraph = new BidirectionalGraph<EquatableTestVertex, Edge<EquatableTestVertex>>();
            ContainsVertex_EquatableVertex_ImmutableGraph_Test(
                wrappedGraph,
                () => new ArrayBidirectionalGraph<EquatableTestVertex, Edge<EquatableTestVertex>>(wrappedGraph));
        }

        [Test]
        public void ContainsVertex_Throws()
        {
            var wrappedGraph = new BidirectionalGraph<TestVertex, IEdge<TestVertex>>();
            var graph = new ArrayBidirectionalGraph<TestVertex, IEdge<TestVertex>>(wrappedGraph);
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
                () => new ArrayBidirectionalGraph<int, IEdge<int>>(wrappedGraph));
        }

        [Test]
        public void ContainsEdge_EquatableEdge()
        {
            var wrappedGraph = new BidirectionalGraph<int, EquatableEdge<int>>();
            ContainsEdge_EquatableEdge_ImmutableGraph_Test(
                wrappedGraph,
                () => new ArrayBidirectionalGraph<int, EquatableEdge<int>>(wrappedGraph));
        }

        [Test]
        public void ContainsEdge_SourceTarget()
        {
            var wrappedGraph = new BidirectionalGraph<int, IEdge<int>>();
            ContainsEdge_SourceTarget_ImmutableGraph_Test(
                wrappedGraph,
                () => new ArrayBidirectionalGraph<int, IEdge<int>>(wrappedGraph));
        }

        [Test]
        public void ContainsEdge_Throws()
        {
            var wrappedGraph = new BidirectionalGraph<TestVertex, IEdge<TestVertex>>();
            var graph = new ArrayBidirectionalGraph<TestVertex, IEdge<TestVertex>>(wrappedGraph);
            ContainsEdge_NullThrows_Test(graph);
            ContainsEdge_SourceTarget_Throws_Test(graph);
        }

        #endregion

        #region Out Edges

        [Test]
        public void OutEdge()
        {
            var wrappedGraph = new BidirectionalGraph<int, IEdge<int>>();
            OutEdge_ImmutableGraph_Test(
                wrappedGraph,
                () => new ArrayBidirectionalGraph<int, IEdge<int>>(wrappedGraph));
        }

        [Test]
        public void OutEdge_Throws()
        {
            var wrappedGraph1 = new BidirectionalGraph<TestVertex, IEdge<TestVertex>>();
            var graph1 = new ArrayBidirectionalGraph<TestVertex, IEdge<TestVertex>>(wrappedGraph1);
            OutEdge_NullThrows_Test(graph1);

            var wrappedGraph2 = new BidirectionalGraph<int, IEdge<int>>();
            OutEdge_Throws_ImmutableGraph_Test(
                wrappedGraph2,
                () => new ArrayBidirectionalGraph<int, IEdge<int>>(wrappedGraph2));
        }

        [Test]
        public void OutEdges()
        {
            var wrappedGraph = new BidirectionalGraph<int, IEdge<int>>();
            OutEdges_ImmutableGraph_Test(
                wrappedGraph,
                () => new ArrayBidirectionalGraph<int, IEdge<int>>(wrappedGraph));
        }

        [Test]
        public void OutEdges_Throws()
        {
            var wrappedGraph1 = new BidirectionalGraph<TestVertex, IEdge<TestVertex>>();
            var graph1 = new ArrayBidirectionalGraph<TestVertex, IEdge<TestVertex>>(wrappedGraph1);
            OutEdges_NullThrows_Test(graph1);

            var wrappedGraph2 = new BidirectionalGraph<EquatableTestVertex, Edge<EquatableTestVertex>>();
            var graph2 = new ArrayBidirectionalGraph<EquatableTestVertex, Edge<EquatableTestVertex>>(wrappedGraph2);
            OutEdges_Throws_Test(graph2);
        }

        #endregion

        #region In Edges

        [Test]
        public void InEdge()
        {
            var wrappedGraph = new BidirectionalGraph<int, IEdge<int>>();
            InEdge_ImmutableGraph_Test(
                wrappedGraph,
                () => new ArrayBidirectionalGraph<int, IEdge<int>>(wrappedGraph));
        }

        [Test]
        public void InEdge_Throws()
        {
            var wrappedGraph1 = new BidirectionalGraph<int, IEdge<int>>();
            InEdge_Throws_ImmutableGraph_Test(
                wrappedGraph1,
                () => new ArrayBidirectionalGraph<int, IEdge<int>>(wrappedGraph1));

            var wrappedGraph2 = new BidirectionalGraph<TestVertex, IEdge<TestVertex>>();
            var graph2 = new ArrayBidirectionalGraph<TestVertex, IEdge<TestVertex>>(wrappedGraph2);
            InEdge_NullThrows_Test(graph2);
        }

        [Test]
        public void InEdges()
        {
            var wrappedGraph = new BidirectionalGraph<int, IEdge<int>>();
            InEdges_ImmutableGraph_Test(
                wrappedGraph,
                () => new ArrayBidirectionalGraph<int, IEdge<int>>(wrappedGraph));
        }

        [Test]
        public void InEdges_Throws()
        {
            var wrappedGraph1 = new BidirectionalGraph<TestVertex, IEdge<TestVertex>>();
            var graph1 = new ArrayBidirectionalGraph<TestVertex, IEdge<TestVertex>>(wrappedGraph1);
            InEdges_NullThrows_Test(graph1);

            var wrappedGraph2 = new BidirectionalGraph<EquatableTestVertex, Edge<EquatableTestVertex>>();
            var graph2 = new ArrayBidirectionalGraph<EquatableTestVertex, Edge<EquatableTestVertex>>(wrappedGraph2);
            InEdges_Throws_Test(graph2);
        }

        #endregion

        [Test]
        public void Degree()
        {
            var wrappedGraph = new BidirectionalGraph<int, IEdge<int>>();
            Degree_ImmutableGraph_Test(
                wrappedGraph,
                () => new ArrayBidirectionalGraph<int, IEdge<int>>(wrappedGraph));
        }

        [Test]
        public void Degree_Throws()
        {
            var wrappedGraph = new BidirectionalGraph<EquatableTestVertex, Edge<EquatableTestVertex>>();
            var graph = new ArrayBidirectionalGraph<EquatableTestVertex, Edge<EquatableTestVertex>>(wrappedGraph);
            Degree_Throws_Test(graph);
        }

        #region Try Get Edges

        [Test]
        public void TryGetEdge()
        {
            var wrappedGraph = new BidirectionalGraph<int, IEdge<int>>();
            TryGetEdge_ImmutableGraph_Test(
                wrappedGraph,
                () => new ArrayBidirectionalGraph<int, IEdge<int>>(wrappedGraph));
        }

        [Test]
        public void TryGetEdge_Throws()
        {
            var wrappedGraph = new BidirectionalGraph<TestVertex, IEdge<TestVertex>>();
            var graph = new ArrayBidirectionalGraph<TestVertex, IEdge<TestVertex>>(wrappedGraph);
            TryGetEdge_Throws_Test(graph);
        }

        [Test]
        public void GetEdges()
        {
            var wrappedGraph = new BidirectionalGraph<int, IEdge<int>>();
            GetEdges_ImmutableGraph_Test(
                wrappedGraph,
                () => new ArrayBidirectionalGraph<int, IEdge<int>>(wrappedGraph));
        }

        [Test]
        public void GetEdges_Throws()
        {
            var wrappedGraph = new BidirectionalGraph<TestVertex, IEdge<TestVertex>>();
            var graph = new ArrayBidirectionalGraph<TestVertex, IEdge<TestVertex>>(wrappedGraph);
            GetEdges_Throws_Test(graph);
        }

        [Test]
        public void GetOutEdges()
        {
            var wrappedGraph = new BidirectionalGraph<int, IEdge<int>>();
            GetOutEdges_ImmutableGraph_Test(
                wrappedGraph,
                () => new ArrayBidirectionalGraph<int, IEdge<int>>(wrappedGraph));
        }

        [Test]
        public void GetOutEdges_Throws()
        {
            var wrappedGraph = new BidirectionalGraph<TestVertex, IEdge<TestVertex>>();
            var graph = new ArrayBidirectionalGraph<TestVertex, IEdge<TestVertex>>(wrappedGraph);
            GetOutEdges_Throws_Test(graph);
        }

        [Test]
        public void GetInEdges()
        {
            var wrappedGraph = new BidirectionalGraph<int, IEdge<int>>();
            GetInEdges_ImmutableGraph_Test(
                wrappedGraph,
                () => new ArrayBidirectionalGraph<int, IEdge<int>>(wrappedGraph));
        }

        [Test]
        public void GetInEdges_Throws()
        {
            var wrappedGraph = new BidirectionalGraph<TestVertex, IEdge<TestVertex>>();
            var graph = new ArrayBidirectionalGraph<TestVertex, IEdge<TestVertex>>(wrappedGraph);
            GetInEdges_Throws_Test(graph);
        }

        #endregion

        [Test]
        public void Clone()
        {
            var wrappedGraph = new BidirectionalGraph<int, IEdge<int>>();

            var graph = new ArrayBidirectionalGraph<int, IEdge<int>>(wrappedGraph);
            AssertEmptyGraph(graph);

            var clonedGraph = graph.Clone();
            Assert.IsNotNull(clonedGraph);
            AssertEmptyGraph(clonedGraph);

            clonedGraph = (ArrayBidirectionalGraph<int, IEdge<int>>)((ICloneable)graph).Clone();
            Assert.IsNotNull(clonedGraph);
            AssertEmptyGraph(clonedGraph);

            wrappedGraph.AddVertexRange( 1, 2, 3 );
            graph = new ArrayBidirectionalGraph<int, IEdge<int>>(wrappedGraph);
            graph.AssertHasVertices(1, 2, 3 );
            graph.AssertNoEdge();

            clonedGraph = graph.Clone();
            Assert.IsNotNull(clonedGraph);
            clonedGraph.AssertHasVertices(1, 2, 3 );
            clonedGraph.AssertNoEdge();

            clonedGraph = (ArrayBidirectionalGraph<int, IEdge<int>>)((ICloneable)graph).Clone();
            Assert.IsNotNull(clonedGraph);
            clonedGraph.AssertHasVertices(1, 2, 3 );
            clonedGraph.AssertNoEdge();

            var edge1 = Edge.Create(1, 2);
            var edge2 = Edge.Create(1, 3);
            var edge3 = Edge.Create(2, 3);
            wrappedGraph.AddVerticesAndEdgeRange( edge1, edge2, edge3 );

            graph = new ArrayBidirectionalGraph<int, IEdge<int>>(wrappedGraph);
            graph.AssertHasVertices(1, 2, 3 );
            graph.AssertHasEdges(edge1, edge2, edge3 );

            clonedGraph = graph.Clone();
            Assert.IsNotNull(clonedGraph);
            clonedGraph.AssertHasVertices(1, 2, 3 );
            clonedGraph.AssertHasEdges(edge1, edge2, edge3 );

            clonedGraph = (ArrayBidirectionalGraph<int, IEdge<int>>)((ICloneable)graph).Clone();
            Assert.IsNotNull(clonedGraph);
            clonedGraph.AssertHasVertices(1, 2, 3 );
            clonedGraph.AssertHasEdges(edge1, edge2, edge3 );

            wrappedGraph.AddVertex(4);
            graph = new ArrayBidirectionalGraph<int, IEdge<int>>(wrappedGraph);
            graph.AssertHasVertices(1, 2, 3, 4 );
            graph.AssertHasEdges(edge1, edge2, edge3 );

            clonedGraph = graph.Clone();
            Assert.IsNotNull(clonedGraph);
            clonedGraph.AssertHasVertices(1, 2, 3, 4 );
            clonedGraph.AssertHasEdges(edge1, edge2, edge3 );

            clonedGraph = (ArrayBidirectionalGraph<int, IEdge<int>>)((ICloneable)graph).Clone();
            Assert.IsNotNull(clonedGraph);
            clonedGraph.AssertHasVertices(1, 2, 3, 4 );
            clonedGraph.AssertHasEdges(edge1, edge2, edge3 );
        }
    }
}