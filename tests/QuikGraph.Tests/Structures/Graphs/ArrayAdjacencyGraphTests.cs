using System;
using NUnit.Framework;
using static QuikGraph.Tests.GraphTestHelpers;

namespace QuikGraph.Tests.Structures
{
    /// <summary>
    /// Tests for <see cref="ArrayAdjacencyGraph{TVertex,TEdge}"/>.
    /// </summary>
    [TestFixture]
    internal sealed class ArrayAdjacencyGraphTests : GraphTestsBase
    {
        [Test]
        public void Construction()
        {
            var wrappedGraph = new AdjacencyGraph<int, IEdge<int>>();

            var graph = new ArrayAdjacencyGraph<int, IEdge<int>>(wrappedGraph);
            AssertGraphProperties(graph);
            AssertEmptyGraph(graph);

            wrappedGraph.AddVertexRange( 2, 3, 1 );
            graph = new ArrayAdjacencyGraph<int, IEdge<int>>(wrappedGraph);
            AssertGraphProperties(graph);
            graph.AssertHasVertices(1, 2, 3 );
            graph.AssertNoEdge();

            var edge1 = Edge.Create(1, 2);
            var edge2 = Edge.Create(2, 2);
            var edge3 = Edge.Create(3, 4);
            var edge4 = Edge.Create(1, 4);
            wrappedGraph.AddVerticesAndEdgeRange( edge1, edge2, edge3, edge4 );
            graph = new ArrayAdjacencyGraph<int, IEdge<int>>(wrappedGraph);
            AssertGraphProperties(graph);
            graph.AssertHasVertices(1, 2, 3, 4 );
            graph.AssertHasEdges(edge1, edge2, edge3, edge4 );

            wrappedGraph = new AdjacencyGraph<int, IEdge<int>>(false);
            wrappedGraph.AddVerticesAndEdgeRange( edge1, edge1, edge2, edge3, edge4 );
            graph = new ArrayAdjacencyGraph<int, IEdge<int>>(wrappedGraph);
            AssertGraphProperties(graph, false);
            graph.AssertHasVertices(1, 2, 3, 4 );
            graph.AssertHasEdges(edge1, edge2, edge3, edge4 );

            #region Local function

            void AssertGraphProperties<TVertex, TEdge>(
                ArrayAdjacencyGraph<TVertex, TEdge> g,
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
            Assert.Throws<ArgumentNullException>(() => new ArrayAdjacencyGraph<int, IEdge<int>>(null));
        }

        #region Add Vertex => no effect

        [Test]
        public void AddVertex()
        {
            var wrappedGraph = new AdjacencyGraph<int, IEdge<int>>();
            AddVertex_ImmutableGraph_NoUpdate(
                wrappedGraph,
                () => new ArrayAdjacencyGraph<int, IEdge<int>>(wrappedGraph));
        }

        #endregion

        #region Add Edge => no effect

        [Test]
        public void AddEdge()
        {
            var wrappedGraph = new AdjacencyGraph<int, IEdge<int>>();
            AddEdge_ImmutableGraph_NoUpdate(
                wrappedGraph,
                () => new ArrayAdjacencyGraph<int, IEdge<int>>(wrappedGraph));
        }

        #endregion

        #region Contains Vertex

        [Test]
        public void ContainsVertex()
        {
            var wrappedGraph = new AdjacencyGraph<TestVertex, IEdge<TestVertex>>();
            ContainsVertex_ImmutableGraph_Test(
                wrappedGraph,
                () => new ArrayAdjacencyGraph<TestVertex, IEdge<TestVertex>>(wrappedGraph));
        }

        [Test]
        public void ContainsVertex_EquatableVertex()
        {
            var wrappedGraph = new AdjacencyGraph<EquatableTestVertex, Edge<EquatableTestVertex>>();
            ContainsVertex_EquatableVertex_ImmutableGraph_Test(
                wrappedGraph,
                () => new ArrayAdjacencyGraph<EquatableTestVertex, Edge<EquatableTestVertex>>(wrappedGraph));
        }

        [Test]
        public void ContainsVertex_Throws()
        {
            var wrappedGraph = new AdjacencyGraph<TestVertex, IEdge<TestVertex>>();
            var graph = new ArrayAdjacencyGraph<TestVertex, IEdge<TestVertex>>(wrappedGraph);
            ContainsVertex_Throws_Test(graph);
        }

        #endregion

        #region Contains Edge

        [Test]
        public void ContainsEdge()
        {
            var wrappedGraph = new AdjacencyGraph<int, IEdge<int>>();
            ContainsEdge_ImmutableGraph_Test(
                wrappedGraph,
                () => new ArrayAdjacencyGraph<int, IEdge<int>>(wrappedGraph));
        }

        [Test]
        public void ContainsEdge_EquatableEdge()
        {
            var wrappedGraph = new AdjacencyGraph<int, EquatableEdge<int>>();
            ContainsEdge_EquatableEdge_ImmutableGraph_Test(
                wrappedGraph,
                () => new ArrayAdjacencyGraph<int, EquatableEdge<int>>(wrappedGraph));
        }

        [Test]
        public void ContainsEdge_SourceTarget()
        {
            var wrappedGraph = new AdjacencyGraph<int, IEdge<int>>();
            ContainsEdge_SourceTarget_ImmutableGraph_Test(
                wrappedGraph,
                () => new ArrayAdjacencyGraph<int, IEdge<int>>(wrappedGraph));
        }

        [Test]
        public void ContainsEdge_Throws()
        {
            var wrappedGraph = new AdjacencyGraph<TestVertex, IEdge<TestVertex>>();
            var graph = new ArrayAdjacencyGraph<TestVertex, IEdge<TestVertex>>(wrappedGraph);
            ContainsEdge_NullThrows_Test(graph);
            ContainsEdge_SourceTarget_Throws_Test(graph);
        }

        #endregion

        #region Out Edges

        [Test]
        public void OutEdge()
        {
            var wrappedGraph = new AdjacencyGraph<int, IEdge<int>>();
            OutEdge_ImmutableGraph_Test(
                wrappedGraph,
                () => new ArrayAdjacencyGraph<int, IEdge<int>>(wrappedGraph));
        }

        [Test]
        public void OutEdge_Throws()
        {
            var wrappedGraph1 = new AdjacencyGraph<TestVertex, IEdge<TestVertex>>();
            var graph1 = new ArrayAdjacencyGraph<TestVertex, IEdge<TestVertex>>(wrappedGraph1);
            OutEdge_NullThrows_Test(graph1);

            var wrappedGraph2 = new AdjacencyGraph<int, IEdge<int>>();
            OutEdge_Throws_ImmutableGraph_Test(
                wrappedGraph2,
                () => new ArrayAdjacencyGraph<int, IEdge<int>>(wrappedGraph2));
        }

        [Test]
        public void OutEdges()
        {
            var wrappedGraph = new AdjacencyGraph<int, IEdge<int>>();
            OutEdges_ImmutableGraph_Test(
                wrappedGraph,
                () => new ArrayAdjacencyGraph<int, IEdge<int>>(wrappedGraph));
        }

        [Test]
        public void OutEdges_Throws()
        {
            var wrappedGraph1 = new AdjacencyGraph<TestVertex, IEdge<TestVertex>>();
            var graph1 = new ArrayAdjacencyGraph<TestVertex, IEdge<TestVertex>>(wrappedGraph1);
            OutEdges_NullThrows_Test(graph1);

            var wrappedGraph2 = new AdjacencyGraph<EquatableTestVertex, Edge<EquatableTestVertex>>();
            var graph2 = new ArrayAdjacencyGraph<EquatableTestVertex, Edge<EquatableTestVertex>>(wrappedGraph2);
            OutEdges_Throws_Test(graph2);
        }

        #endregion

        #region Try Get Edges

        [Test]
        public void TryGetEdge()
        {
            var wrappedGraph = new AdjacencyGraph<int, IEdge<int>>();
            TryGetEdge_ImmutableGraph_Test(
                wrappedGraph,
                () => new ArrayAdjacencyGraph<int, IEdge<int>>(wrappedGraph));
        }

        [Test]
        public void TryGetEdge_Throws()
        {
            var wrappedGraph = new AdjacencyGraph<TestVertex, IEdge<TestVertex>>();
            var graph = new ArrayAdjacencyGraph<TestVertex, IEdge<TestVertex>>(wrappedGraph);
            TryGetEdge_Throws_Test(graph);
        }

        [Test]
        public void GetEdges()
        {
            var wrappedGraph = new AdjacencyGraph<int, IEdge<int>>();
            GetEdges_ImmutableGraph_Test(
                wrappedGraph,
                () => new ArrayAdjacencyGraph<int, IEdge<int>>(wrappedGraph));
        }

        [Test]
        public void GetEdges_Throws()
        {
            var wrappedGraph = new AdjacencyGraph<TestVertex, IEdge<TestVertex>>();
            var graph = new ArrayAdjacencyGraph<TestVertex, IEdge<TestVertex>>(wrappedGraph);
            GetEdges_Throws_Test(graph);
        }

        [Test]
        public void GetOutEdges()
        {
            var wrappedGraph = new AdjacencyGraph<int, IEdge<int>>();
            GetOutEdges_ImmutableGraph_Test(
                wrappedGraph,
                () => new ArrayAdjacencyGraph<int, IEdge<int>>(wrappedGraph));
        }

        [Test]
        public void GetOutEdges_Throws()
        {
            var wrappedGraph = new AdjacencyGraph<TestVertex, IEdge<TestVertex>>();
            var graph = new ArrayAdjacencyGraph<TestVertex, IEdge<TestVertex>>(wrappedGraph);
            GetOutEdges_Throws_Test(graph);
        }

        #endregion

        [Test]
        public void Clone()
        {
            var wrappedGraph = new AdjacencyGraph<int, IEdge<int>>();

            var graph = new ArrayAdjacencyGraph<int, IEdge<int>>(wrappedGraph);
            AssertEmptyGraph(graph);

            var clonedGraph = graph.Clone();
            Assert.IsNotNull(clonedGraph);
            AssertEmptyGraph(clonedGraph);

            clonedGraph = (ArrayAdjacencyGraph<int, IEdge<int>>)((ICloneable)graph).Clone();
            Assert.IsNotNull(clonedGraph);
            AssertEmptyGraph(clonedGraph);

            wrappedGraph.AddVertexRange( 1, 2, 3 );
            graph = new ArrayAdjacencyGraph<int, IEdge<int>>(wrappedGraph);
            graph.AssertHasVertices(1, 2, 3 );
            graph.AssertNoEdge();

            clonedGraph = graph.Clone();
            Assert.IsNotNull(clonedGraph);
            clonedGraph.AssertHasVertices(1, 2, 3 );
            clonedGraph.AssertNoEdge();

            clonedGraph = (ArrayAdjacencyGraph<int, IEdge<int>>)((ICloneable)graph).Clone();
            Assert.IsNotNull(clonedGraph);
            clonedGraph.AssertHasVertices(1, 2, 3 );
            clonedGraph.AssertNoEdge();

            var edge1 = Edge.Create(1, 2);
            var edge2 = Edge.Create(1, 3);
            var edge3 = Edge.Create(2, 3);
            wrappedGraph.AddVerticesAndEdgeRange( edge1, edge2, edge3 );

            graph = new ArrayAdjacencyGraph<int, IEdge<int>>(wrappedGraph);
            graph.AssertHasVertices(1, 2, 3 );
            graph.AssertHasEdges(edge1, edge2, edge3 );

            clonedGraph = graph.Clone();
            Assert.IsNotNull(clonedGraph);
            clonedGraph.AssertHasVertices(1, 2, 3 );
            clonedGraph.AssertHasEdges(edge1, edge2, edge3 );

            clonedGraph = (ArrayAdjacencyGraph<int, IEdge<int>>)((ICloneable)graph).Clone();
            Assert.IsNotNull(clonedGraph);
            clonedGraph.AssertHasVertices(1, 2, 3 );
            clonedGraph.AssertHasEdges(edge1, edge2, edge3 );

            wrappedGraph.AddVertex(4);
            graph = new ArrayAdjacencyGraph<int, IEdge<int>>(wrappedGraph);
            graph.AssertHasVertices(1, 2, 3, 4 );
            graph.AssertHasEdges(edge1, edge2, edge3 );

            clonedGraph = graph.Clone();
            Assert.IsNotNull(clonedGraph);
            clonedGraph.AssertHasVertices(1, 2, 3, 4 );
            clonedGraph.AssertHasEdges(edge1, edge2, edge3 );

            clonedGraph = (ArrayAdjacencyGraph<int, IEdge<int>>)((ICloneable)graph).Clone();
            Assert.IsNotNull(clonedGraph);
            clonedGraph.AssertHasVertices(1, 2, 3, 4 );
            clonedGraph.AssertHasEdges(edge1, edge2, edge3 );
        }
    }
}