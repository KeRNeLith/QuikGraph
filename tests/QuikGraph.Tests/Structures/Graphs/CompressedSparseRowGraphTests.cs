using System;
using NUnit.Framework;
using static QuikGraph.Tests.GraphTestHelpers;

namespace QuikGraph.Tests.Structures
{
    /// <summary>
    /// Tests for <see cref="CompressedSparseRowGraph{TVertex}"/>.
    /// </summary>
    [TestFixture]
    internal sealed class CompressedSparseRowGraphTests : GraphTestsBase
    {
        [Test]
        public void Construction()
        {
            var wrappedGraph = new AdjacencyGraph<int, IEdge<int>>();
            var graph = CompressedSparseRowGraph<int>.FromGraph(wrappedGraph);
            AssertGraphProperties(graph);
            AssertEmptyGraph(graph);

            wrappedGraph.AddVertexRange( 1, 2, 3 );
            graph = CompressedSparseRowGraph<int>.FromGraph(wrappedGraph);
            AssertGraphProperties(graph);
            graph.AssertHasVertices(1, 2, 3 );
            graph.AssertNoEdge();

            var edge1 = Edge.Create(1, 2);
            var edge2 = Edge.Create(1, 3);
            var edge3 = Edge.Create(2, 1);
            var edge4 = Edge.Create(2, 2);
            var edge5 = Edge.Create(2, 3);
            wrappedGraph.AddEdgeRange( edge1, edge2, edge3, edge4, edge5 );
            graph = CompressedSparseRowGraph<int>.FromGraph(wrappedGraph);
            AssertGraphProperties(graph);
            graph.AssertHasVertices(1, 2, 3 );
            graph.AssertHasEdges(
                new SEquatableEdge<int>[]
                {
                    new (1, 2),
                    new (1, 3),
                    new (2, 1),
                    new (2, 2),
                    new (2, 3)
                });

            var edge6 = Edge.Create(3, 1);
            var edge7 = Edge.Create(3, 2);
            wrappedGraph.Clear();
            wrappedGraph.AddVertexRange( 1, 2, 3, 4 );
            wrappedGraph.AddEdgeRange( edge1, edge2, edge6, edge7 );
            graph = CompressedSparseRowGraph<int>.FromGraph(wrappedGraph);
            AssertGraphProperties(graph);
            graph.AssertHasVertices(1, 2, 3, 4 );
            graph.AssertHasEdges(
                new SEquatableEdge<int>[]
                {
                    new(1, 2),
                    new(1, 3),
                    new(3, 1),
                    new(3, 2)
                });

            #region Local function

            void AssertGraphProperties<TVertex>(
                // ReSharper disable once ParameterOnlyUsedForPreconditionCheck.Local
                CompressedSparseRowGraph<TVertex> g)
            {
                Assert.IsTrue(g.IsDirected);
                Assert.IsFalse(g.AllowParallelEdges);
            }

            #endregion
        }

        [Test]
        public void Construction_Throws()
        {
            // ReSharper disable once AssignNullToNotNullAttribute
            Assert.Throws<ArgumentNullException>(
                () => CompressedSparseRowGraph<int>.FromGraph<Edge<int>>(null));
        }

        #region Contains Vertex

        [Test]
        public void ContainsVertex()
        {
            var wrappedGraph = new AdjacencyGraph<TestVertex, IEdge<TestVertex>>();
            ContainsVertex_ImmutableGraph_Test(
                wrappedGraph,
                () => CompressedSparseRowGraph<TestVertex>.FromGraph(wrappedGraph));
        }

        [Test]
        public void ContainsVertex_EquatableVertex()
        {
            var wrappedGraph = new AdjacencyGraph<EquatableTestVertex, Edge<EquatableTestVertex>>();
            ContainsVertex_EquatableVertex_ImmutableGraph_Test(
                wrappedGraph,
                () => CompressedSparseRowGraph<EquatableTestVertex>.FromGraph(wrappedGraph));
        }

        [Test]
        public void ContainsVertex_Throws()
        {
            var wrappedGraph = new AdjacencyGraph<TestVertex, IEdge<TestVertex>>();
            var graph = CompressedSparseRowGraph<TestVertex>.FromGraph(wrappedGraph);
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
                () => CompressedSparseRowGraph<int>.FromGraph(wrappedGraph));
        }

        [Test]
        public void ContainsEdge_SourceTarget()
        {
            var wrappedGraph = new AdjacencyGraph<int, IEdge<int>>();
            ContainsEdge_SourceTarget_ImmutableGraph_Test(
                wrappedGraph,
                () => CompressedSparseRowGraph<int>.FromGraph(wrappedGraph));
        }

        [Test]
        public void ContainsEdge_Throws()
        {
            var wrappedGraph = new AdjacencyGraph<TestVertex, IEdge<TestVertex>>();
            var graph = CompressedSparseRowGraph<TestVertex>.FromGraph(wrappedGraph);
            ContainsEdge_DefaultNullThrows_Test(graph);
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
                () => CompressedSparseRowGraph<int>.FromGraph(wrappedGraph));
        }

        [Test]
        public void OutEdge_Throws()
        {
            var wrappedGraph1 = new AdjacencyGraph<TestVertex, IEdge<TestVertex>>();
            var graph1 = CompressedSparseRowGraph<TestVertex>.FromGraph(wrappedGraph1);
            OutEdge_NullThrows_Test(graph1);

            var wrappedGraph2 = new AdjacencyGraph<int, IEdge<int>>();
            OutEdge_Throws_ImmutableGraph_Test(
                wrappedGraph2,
                () => CompressedSparseRowGraph<int>.FromGraph(wrappedGraph2));
        }

        [Test]
        public void OutEdges()
        {
            var wrappedGraph = new AdjacencyGraph<int, IEdge<int>>();
            OutEdges_ImmutableGraph_Test(
                wrappedGraph,
                () => CompressedSparseRowGraph<int>.FromGraph(wrappedGraph));
        }

        [Test]
        public void OutEdges_Throws()
        {
            var wrappedGraph1 = new AdjacencyGraph<TestVertex, IEdge<TestVertex>>();
            var graph1 = CompressedSparseRowGraph<TestVertex>.FromGraph(wrappedGraph1);
            OutEdges_NullThrows_Test(graph1);
        }

        #endregion

        #region Try Get Edges

        [Test]
        public void TryGetEdge()
        {
            var wrappedGraph = new AdjacencyGraph<int, IEdge<int>>();
            TryGetEdge_ImmutableGraph_Test(
                wrappedGraph,
                () => CompressedSparseRowGraph<int>.FromGraph(wrappedGraph));
        }

        [Test]
        public void TryGetEdge_Throws()
        {
            var wrappedGraph = new AdjacencyGraph<TestVertex, IEdge<TestVertex>>();
            var graph = CompressedSparseRowGraph<TestVertex>.FromGraph(wrappedGraph);
            TryGetEdge_Throws_Test(graph);
        }

        [Test]
        public void GetEdges()
        {
            var wrappedGraph = new AdjacencyGraph<int, IEdge<int>>();
            GetEdges_ImmutableGraph_Test(wrappedGraph, () => CompressedSparseRowGraph<int>.FromGraph(wrappedGraph));
        }

        [Test]
        public void GetEdges_Throws()
        {
            var wrappedGraph = new AdjacencyGraph<TestVertex, IEdge<TestVertex>>();
            var graph = CompressedSparseRowGraph<TestVertex>.FromGraph(wrappedGraph);
            GetEdges_Throws_Test(graph);
        }

        [Test]
        public void GetOutEdges()
        {
            var wrappedGraph = new AdjacencyGraph<int, IEdge<int>>();
            GetOutEdges_ImmutableGraph_Test(
                wrappedGraph,
                () => CompressedSparseRowGraph<int>.FromGraph(wrappedGraph));
        }

        [Test]
        public void GetOutEdges_Throws()
        {
            var wrappedGraph = new AdjacencyGraph<TestVertex, IEdge<TestVertex>>();
            var graph = CompressedSparseRowGraph<TestVertex>.FromGraph(wrappedGraph);
            GetOutEdges_Throws_Test(graph);
        }

        #endregion

        [Test]
        public void Clone()
        {
            var wrappedGraph = new AdjacencyGraph<int, IEdge<int>>();
            var graph = CompressedSparseRowGraph<int>.FromGraph(wrappedGraph);
            AssertEmptyGraph(graph);

            var clonedGraph = graph.Clone();
            Assert.IsNotNull(clonedGraph);
            AssertEmptyGraph(clonedGraph);

            clonedGraph = (CompressedSparseRowGraph<int>)((ICloneable)graph).Clone();
            Assert.IsNotNull(clonedGraph);
            AssertEmptyGraph(clonedGraph);

            wrappedGraph.AddVertexRange( 1, 2, 3 );
            graph = CompressedSparseRowGraph<int>.FromGraph(wrappedGraph);
            graph.AssertHasVertices(1, 2, 3 );
            graph.AssertNoEdge();

            clonedGraph = graph.Clone();
            Assert.IsNotNull(clonedGraph);
            clonedGraph.AssertHasVertices(1, 2, 3 );
            clonedGraph.AssertNoEdge();

            clonedGraph = (CompressedSparseRowGraph<int>)((ICloneable)graph).Clone();
            Assert.IsNotNull(clonedGraph);
            clonedGraph.AssertHasVertices(1, 2, 3 );
            clonedGraph.AssertNoEdge();

            var edge1 = Edge.Create(1, 2);
            var edge2 = Edge.Create(1, 3);
            var edge3 = Edge.Create(2, 3);
            wrappedGraph.AddVerticesAndEdgeRange( edge1, edge2, edge3 );
            graph = CompressedSparseRowGraph<int>.FromGraph(wrappedGraph);
            graph.AssertHasVertices(1, 2, 3 );
            graph.AssertHasEdges(
                new SEquatableEdge<int>[]
                {
                    new (1, 2),
                    new (1, 3),
                    new (2, 3)
                });

            clonedGraph = graph.Clone();
            Assert.IsNotNull(clonedGraph);
            clonedGraph.AssertHasVertices(1, 2, 3 );
            clonedGraph.AssertHasEdges(
                new SEquatableEdge<int>[]
                {
                    new (1, 2),
                    new (1, 3),
                    new (2, 3)
                });

            clonedGraph = (CompressedSparseRowGraph<int>)((ICloneable)graph).Clone();
            Assert.IsNotNull(clonedGraph);
            clonedGraph.AssertHasVertices(1, 2, 3 );
            clonedGraph.AssertHasEdges(
                new SEquatableEdge<int>[]
                {
                    new (1, 2),
                    new (1, 3),
                    new (2, 3)
                });

            wrappedGraph.AddVertex(4);
            graph = CompressedSparseRowGraph<int>.FromGraph(wrappedGraph);
            graph.AssertHasVertices(1, 2, 3, 4 );
            graph.AssertHasEdges(
                new SEquatableEdge<int>[]
                {
                    new (1, 2),
                    new (1, 3),
                    new (2, 3)
                });

            clonedGraph = graph.Clone();
            Assert.IsNotNull(clonedGraph);
            clonedGraph.AssertHasVertices(1, 2, 3, 4 );
            clonedGraph.AssertHasEdges(
                new SEquatableEdge<int>[]
                {
                    new (1, 2),
                    new (1, 3),
                    new (2, 3)
                });

            clonedGraph = (CompressedSparseRowGraph<int>)((ICloneable)graph).Clone();
            Assert.IsNotNull(clonedGraph);
            clonedGraph.AssertHasVertices(1, 2, 3, 4 );
            clonedGraph.AssertHasEdges(
                new SEquatableEdge<int>[]
                {
                    new (1, 2),
                    new (1, 3),
                    new (2, 3)
                });
        }
    }
}