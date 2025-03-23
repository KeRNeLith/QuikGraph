using System;
using NUnit.Framework;
using static QuikGraph.Tests.GraphTestHelpers;

namespace QuikGraph.Tests.Structures
{
    /// <summary>
    /// Tests for <see cref="ArrayUndirectedGraph{TVertex,TEdge}"/>.
    /// </summary>
    [TestFixture]
    internal sealed class ArrayUndirectedGraphTests : GraphTestsBase
    {
        [Test]
        public void Construction()
        {
            EdgeEqualityComparer<int> comparer = (edge, source, target) =>
                edge.Source.Equals(source) && edge.Target.Equals(target);


            var wrappedGraph = new UndirectedGraph<int, IEdge<int>>();

            var graph = new ArrayUndirectedGraph<int, IEdge<int>>(wrappedGraph);
            AssertGraphProperties(graph);
            AssertEmptyGraph(graph);

            wrappedGraph.AddVertexRange( 2, 3, 1 );
            graph = new ArrayUndirectedGraph<int, IEdge<int>>(wrappedGraph);
            AssertGraphProperties(graph);
            graph.AssertHasVertices(1, 2, 3 );
            graph.AssertNoEdge();

            var edge1 = Edge.Create(1, 2);
            var edge2 = Edge.Create(2, 2);
            var edge3 = Edge.Create(3, 4);
            var edge4 = Edge.Create(1, 4);
            wrappedGraph.AddVerticesAndEdgeRange( edge1, edge2, edge3, edge4 );
            graph = new ArrayUndirectedGraph<int, IEdge<int>>(wrappedGraph);
            AssertGraphProperties(graph);
            graph.AssertHasVertices(1, 2, 3, 4 );
            graph.AssertHasEdges(edge1, edge2, edge3, edge4 );

            wrappedGraph = new UndirectedGraph<int, IEdge<int>>(false);
            wrappedGraph.AddVerticesAndEdgeRange( edge1, edge1, edge2, edge3, edge4 );
            graph = new ArrayUndirectedGraph<int, IEdge<int>>(wrappedGraph);
            AssertGraphProperties(graph, false);
            graph.AssertHasVertices(1, 2, 3, 4 );
            graph.AssertHasEdges(edge1, edge2, edge3, edge4 );

            wrappedGraph = new UndirectedGraph<int, IEdge<int>>(true, comparer);
            graph = new ArrayUndirectedGraph<int, IEdge<int>>(wrappedGraph);
            AssertGraphProperties(graph);
            AssertEmptyGraph(graph);

            #region Local function

            void AssertGraphProperties<TVertex, TEdge>(
                ArrayUndirectedGraph<TVertex, TEdge> g,
                bool allowParallelEdges = true)
                where TEdge : IEdge<TVertex>
            {
                Assert.IsFalse(g.IsDirected);
                Assert.AreEqual(allowParallelEdges, g.AllowParallelEdges);
                Assert.AreSame(wrappedGraph.EdgeEqualityComparer, g.EdgeEqualityComparer);
            }

            #endregion
        }

        [Test]
        public void Construction_Throws()
        {
            // ReSharper disable once ObjectCreationAsStatement
            // ReSharper disable once AssignNullToNotNullAttribute
            Assert.Throws<ArgumentNullException>(() => new ArrayUndirectedGraph<int, IEdge<int>>(null));
        }

        #region Add Vertex => no effect

        [Test]
        public void AddVertex()
        {
            var wrappedGraph = new UndirectedGraph<int, IEdge<int>>();
            AddVertex_ImmutableGraph_NoUpdate(
                wrappedGraph,
                () => new ArrayUndirectedGraph<int, IEdge<int>>(wrappedGraph));
        }

        #endregion

        #region Add Edge => no effect

        [Test]
        public void AddEdge()
        {
            var wrappedGraph = new UndirectedGraph<int, IEdge<int>>();
            AddEdge_ImmutableGraph_NoUpdate(
                wrappedGraph,
                () => new ArrayUndirectedGraph<int, IEdge<int>>(wrappedGraph));
        }

        #endregion

        #region Contains Vertex

        [Test]
        public void ContainsVertex()
        {
            var wrappedGraph = new UndirectedGraph<TestVertex, IEdge<TestVertex>>();
            ContainsVertex_ImmutableGraph_Test(
                wrappedGraph,
                () => new ArrayUndirectedGraph<TestVertex, IEdge<TestVertex>>(wrappedGraph));
        }

        [Test]
        public void ContainsVertex_EquatableVertex()
        {
            var wrappedGraph = new UndirectedGraph<EquatableTestVertex, Edge<EquatableTestVertex>>();
            ContainsVertex_EquatableVertex_ImmutableGraph_Test(
                wrappedGraph,
                () => new ArrayUndirectedGraph<EquatableTestVertex, Edge<EquatableTestVertex>>(wrappedGraph));
        }

        [Test]
        public void ContainsVertex_Throws()
        {
            var wrappedGraph = new UndirectedGraph<TestVertex, IEdge<TestVertex>>();
            var graph = new ArrayUndirectedGraph<TestVertex, IEdge<TestVertex>>(wrappedGraph);
            ContainsVertex_Throws_Test(graph);
        }

        #endregion

        #region Contains Edge

        [Test]
        public void ContainsEdge()
        {
            var wrappedGraph = new UndirectedGraph<int, IEdge<int>>();
            ContainsEdge_ImmutableGraph_Test(
                wrappedGraph, 
                () => new ArrayUndirectedGraph<int, IEdge<int>>(wrappedGraph));
        }

        [Test]
        public void ContainsEdge_EquatableEdge()
        {
            var wrappedGraph = new UndirectedGraph<int, EquatableEdge<int>>();
            ContainsEdge_EquatableEdge_ImmutableGraph_Test(
                wrappedGraph,
                () => new ArrayUndirectedGraph<int, EquatableEdge<int>>(wrappedGraph));
        }

        [Test]
        public void ContainsEdge_SourceTarget()
        {
            var wrappedGraph = new UndirectedGraph<int, IEdge<int>>();
            ContainsEdge_SourceTarget_ImmutableGraph_UndirectedGraph_Test(
                wrappedGraph,
                () => new ArrayUndirectedGraph<int, IEdge<int>>(wrappedGraph));
        }

        [Test]
        public void ContainsEdge_Throws()
        {
            var wrappedGraph = new UndirectedGraph<TestVertex, IEdge<TestVertex>>();
            var graph = new ArrayUndirectedGraph<TestVertex, IEdge<TestVertex>>(wrappedGraph);
            ContainsEdge_NullThrows_Test(graph);
            ContainsEdge_SourceTarget_Throws_UndirectedGraph_Test(graph);
        }

        [Test]
        public void ContainsEdge_Undirected()
        {
            var wrappedGraph1 = new UndirectedGraph<int, EquatableEdge<int>>();
            var wrappedGraph2 = new UndirectedGraph<int, EquatableUndirectedEdge<int>>();
            ContainsEdge_UndirectedEdge_ImmutableGraph_UndirectedGraph_Test(
                wrappedGraph1,
                () => new ArrayUndirectedGraph<int, EquatableEdge<int>>(wrappedGraph1), 
                wrappedGraph2,
                () => new ArrayUndirectedGraph<int, EquatableUndirectedEdge<int>>(wrappedGraph2));
        }

        #endregion

        #region Adjacent Edges

        [Test]
        public void AdjacentEdge()
        {
            var wrappedGraph = new UndirectedGraph<int, IEdge<int>>();
            AdjacentEdge_ImmutableGraph_Test(
                wrappedGraph,
                () => new ArrayUndirectedGraph<int, IEdge<int>>(wrappedGraph));
        }

        [Test]
        public void AdjacentEdge_Throws()
        {
            var wrappedGraph1 = new UndirectedGraph<int, IEdge<int>>();
            AdjacentEdge_Throws_ImmutableGraph_Test(
                wrappedGraph1, 
                () => new ArrayUndirectedGraph<int, IEdge<int>>(wrappedGraph1));

            var wrappedGraph2 = new UndirectedGraph<TestVertex, IEdge<TestVertex>>();
            var graph2 = new ArrayUndirectedGraph<TestVertex, IEdge<TestVertex>>(wrappedGraph2);
            AdjacentEdge_NullThrows_Test(graph2);
        }

        [Test]
        public void AdjacentEdges()
        {
            var wrappedGraph = new UndirectedGraph<int, IEdge<int>>();
            AdjacentEdges_ImmutableGraph_Test(
                wrappedGraph,
                () => new ArrayUndirectedGraph<int, IEdge<int>>(wrappedGraph));
        }

        [Test]
        public void AdjacentEdges_Throws()
        {
            var wrappedGraph = new UndirectedGraph<EquatableTestVertex, Edge<EquatableTestVertex>>();
            var graph = new ArrayUndirectedGraph<EquatableTestVertex, Edge<EquatableTestVertex>>(wrappedGraph);
            AdjacentEdges_Throws_Test(graph);
        }

        #endregion

        #region Try Get Edges

        [Test]
        public void TryGetEdge()
        {
            var wrappedGraph = new UndirectedGraph<int, IEdge<int>>();
            TryGetEdge_ImmutableGraph_UndirectedGraph_Test(
                wrappedGraph,
                () => new ArrayUndirectedGraph<int, IEdge<int>>(wrappedGraph));
        }

        [Test]
        public void TryGetEdge_Throws()
        {
            var wrappedGraph = new UndirectedGraph<TestVertex, IEdge<TestVertex>>();
            var graph = new ArrayUndirectedGraph<TestVertex, IEdge<TestVertex>>(wrappedGraph);
            TryGetEdge_Throws_UndirectedGraph_Test(graph);
        }

        #endregion

        [Test]
        public void Clone()
        {
            var wrappedGraph = new UndirectedGraph<int, IEdge<int>>();

            var graph = new ArrayUndirectedGraph<int, IEdge<int>>(wrappedGraph);
            AssertEmptyGraph(graph);

            var clonedGraph = graph.Clone();
            Assert.IsNotNull(clonedGraph);
            AssertEmptyGraph(clonedGraph);

            clonedGraph = (ArrayUndirectedGraph<int, IEdge<int>>)((ICloneable)graph).Clone();
            Assert.IsNotNull(clonedGraph);
            AssertEmptyGraph(clonedGraph);

            wrappedGraph.AddVertexRange( 1, 2, 3 );
            graph = new ArrayUndirectedGraph<int, IEdge<int>>(wrappedGraph);
            graph.AssertHasVertices(1, 2, 3 );
            graph.AssertNoEdge();

            clonedGraph = graph.Clone();
            Assert.IsNotNull(clonedGraph);
            clonedGraph.AssertHasVertices(1, 2, 3 );
            clonedGraph.AssertNoEdge();

            clonedGraph = (ArrayUndirectedGraph<int, IEdge<int>>)((ICloneable)graph).Clone();
            Assert.IsNotNull(clonedGraph);
            clonedGraph.AssertHasVertices(1, 2, 3 );
            clonedGraph.AssertNoEdge();

            var edge1 = Edge.Create(1, 2);
            var edge2 = Edge.Create(1, 3);
            var edge3 = Edge.Create(2, 3);
            wrappedGraph.AddVerticesAndEdgeRange( edge1, edge2, edge3 );

            graph = new ArrayUndirectedGraph<int, IEdge<int>>(wrappedGraph);
            graph.AssertHasVertices(1, 2, 3 );
            graph.AssertHasEdges(edge1, edge2, edge3 );

            clonedGraph = graph.Clone();
            Assert.IsNotNull(clonedGraph);
            clonedGraph.AssertHasVertices(1, 2, 3 );
            clonedGraph.AssertHasEdges(edge1, edge2, edge3 );

            clonedGraph = (ArrayUndirectedGraph<int, IEdge<int>>)((ICloneable)graph).Clone();
            Assert.IsNotNull(clonedGraph);
            clonedGraph.AssertHasVertices(1, 2, 3 );
            clonedGraph.AssertHasEdges(edge1, edge2, edge3 );

            wrappedGraph.AddVertex(4);
            graph = new ArrayUndirectedGraph<int, IEdge<int>>(wrappedGraph);
            graph.AssertHasVertices(1, 2, 3, 4 );
            graph.AssertHasEdges(edge1, edge2, edge3 );

            clonedGraph = graph.Clone();
            Assert.IsNotNull(clonedGraph);
            clonedGraph.AssertHasVertices(1, 2, 3, 4 );
            clonedGraph.AssertHasEdges(edge1, edge2, edge3 );

            clonedGraph = (ArrayUndirectedGraph<int, IEdge<int>>)((ICloneable)graph).Clone();
            Assert.IsNotNull(clonedGraph);
            clonedGraph.AssertHasVertices(1, 2, 3, 4 );
            clonedGraph.AssertHasEdges(edge1, edge2, edge3 );
        }
    }
}