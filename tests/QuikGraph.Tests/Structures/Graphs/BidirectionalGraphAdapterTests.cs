using System;
using NUnit.Framework;
using static QuikGraph.Tests.GraphTestHelpers;

namespace QuikGraph.Tests.Structures
{
    /// <summary>
    /// Tests for <see cref="BidirectionalAdapterGraph{TVertex,TEdge}"/>.
    /// </summary>
    [TestFixture]
    internal sealed class BidirectionalAdapterGraphTests : GraphTestsBase
    {
        [Test]
        public void Construction()
        {
            var wrappedDirectedGraph = new AdjacencyGraph<int, IEdge<int>>();

            var graph = new BidirectionalAdapterGraph<int, IEdge<int>>(wrappedDirectedGraph);
            AssertGraphProperties(graph);
            AssertEmptyGraph(graph);

            // Graph has updated content but in-edges properties are broken
            // after updates of the wrapped graph
            wrappedDirectedGraph.AddVertexRange( 2, 3, 1 );

            AssertGraphProperties(graph);
            graph.AssertHasVertices(1, 2, 3 );
            graph.AssertNoEdge();

            graph = new BidirectionalAdapterGraph<int, IEdge<int>>(wrappedDirectedGraph);
            AssertGraphProperties(graph);
            graph.AssertHasVertices(1, 2, 3 );
            graph.AssertNoEdge();

            var edge1 = Edge.Create(1, 2);
            var edge2 = Edge.Create(2, 2);
            var edge3 = Edge.Create(3, 4);
            var edge4 = Edge.Create(1, 4);

            // Graph has updated content but in-edges properties are broken
            // after updates of the wrapped graph
            wrappedDirectedGraph.AddVerticesAndEdgeRange( edge1, edge2, edge3, edge4 );

            AssertGraphProperties(graph);
            graph.AssertHasVertices(1, 2, 3, 4 );
            graph.AssertHasEdges(edge1, edge2, edge3, edge4 );

            graph = new BidirectionalAdapterGraph<int, IEdge<int>>(wrappedDirectedGraph);
            AssertGraphProperties(graph);
            graph.AssertHasVertices(1, 2, 3, 4 );
            graph.AssertHasEdges(edge1, edge2, edge3, edge4 );

            wrappedDirectedGraph = new AdjacencyGraph<int, IEdge<int>>(false);
            wrappedDirectedGraph.AddVerticesAndEdgeRange( edge1, edge1, edge2, edge3, edge4 );
            graph = new BidirectionalAdapterGraph<int, IEdge<int>>(wrappedDirectedGraph);
            AssertGraphProperties(graph, allowParallelEdges: false);
            graph.AssertHasVertices(1, 2, 3, 4 );
            graph.AssertHasEdges(edge1, edge2, edge3, edge4 );

            #region Local function

            void AssertGraphProperties<TVertex, TEdge>(
                BidirectionalAdapterGraph<TVertex, TEdge> g,
                bool isDirected = true,
                bool allowParallelEdges = true)
                where TEdge : IEdge<TVertex>
            {
                Assert.AreEqual(isDirected, g.IsDirected);
                Assert.AreEqual(allowParallelEdges, g.AllowParallelEdges);
            }

            #endregion
        }

        [Test]
        public void Construction_Throws()
        {
            // ReSharper disable once ObjectCreationAsStatement
            // ReSharper disable once AssignNullToNotNullAttribute
            Assert.Throws<ArgumentNullException>(() => new BidirectionalAdapterGraph<int, IEdge<int>>(null));
        }

        #region Add Vertex => has effect

        [Test]
        public void AddVertex()
        {
            var wrappedGraph = new BidirectionalGraph<int, IEdge<int>>();
            AddVertex_ImmutableGraph_WithUpdate(
                wrappedGraph,
                () => new BidirectionalAdapterGraph<int, IEdge<int>>(wrappedGraph));
        }

        #endregion

        #region Add Edge => has effect

        [Test]
        public void AddEdge()
        {
            var wrappedGraph = new BidirectionalGraph<int, IEdge<int>>();
            AddEdge_ImmutableGraph_WithUpdate(
                wrappedGraph,
                () => new BidirectionalAdapterGraph<int, IEdge<int>>(wrappedGraph));
        }

        #endregion

        #region Contains Vertex

        [Test]
        public void ContainsVertex()
        {
            var wrappedGraph = new AdjacencyGraph<TestVertex, IEdge<TestVertex>>();
            ContainsVertex_ImmutableGraph_Test(
                wrappedGraph,
                () => new BidirectionalAdapterGraph<TestVertex, IEdge<TestVertex>>(wrappedGraph));
        }

        [Test]
        public void ContainsVertex_EquatableVertex()
        {
            var wrappedGraph = new AdjacencyGraph<EquatableTestVertex, Edge<EquatableTestVertex>>();
            ContainsVertex_EquatableVertex_ImmutableGraph_Test(
                wrappedGraph,
                () => new BidirectionalAdapterGraph<EquatableTestVertex, Edge<EquatableTestVertex>>(wrappedGraph));
        }

        [Test]
        public void ContainsVertex_Throws()
        {
            var wrappedGraph = new AdjacencyGraph<TestVertex, IEdge<TestVertex>>();
            var graph = new BidirectionalAdapterGraph<TestVertex, IEdge<TestVertex>>(wrappedGraph);
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
                () => new BidirectionalAdapterGraph<int, IEdge<int>>(wrappedGraph));
        }

        [Test]
        public void ContainsEdge_EquatableEdge()
        {
            var wrappedGraph = new AdjacencyGraph<int, EquatableEdge<int>>();
            ContainsEdge_EquatableEdge_ImmutableGraph_Test(
                wrappedGraph,
                () => new BidirectionalAdapterGraph<int, EquatableEdge<int>>(wrappedGraph));
        }

        [Test]
        public void ContainsEdge_SourceTarget()
        {
            var wrappedGraph = new AdjacencyGraph<int, IEdge<int>>();
            ContainsEdge_SourceTarget_ImmutableGraph_Test(
                wrappedGraph,
                () => new BidirectionalAdapterGraph<int, IEdge<int>>(wrappedGraph));
        }

        [Test]
        public void ContainsEdge_Throws()
        {
            var wrappedGraph = new AdjacencyGraph<TestVertex, IEdge<TestVertex>>();
            var graph = new BidirectionalAdapterGraph<TestVertex, IEdge<TestVertex>>(wrappedGraph);
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
                () => new BidirectionalAdapterGraph<int, IEdge<int>>(wrappedGraph));
        }

        [Test]
        public void OutEdge_Throws()
        {
            var wrappedGraph1 = new AdjacencyGraph<TestVertex, IEdge<TestVertex>>();
            var graph1= new BidirectionalAdapterGraph<TestVertex, IEdge<TestVertex>>(wrappedGraph1);
            OutEdge_NullThrows_Test(graph1);

            var wrappedGraph2 = new AdjacencyGraph<int, IEdge<int>>();
            OutEdge_Throws_ImmutableGraph_Test(
                wrappedGraph2,
                () => new BidirectionalAdapterGraph<int, IEdge<int>>(wrappedGraph2));
        }

        [Test]
        public void OutEdges()
        {
            var wrappedGraph = new AdjacencyGraph<int, IEdge<int>>();
            OutEdges_ImmutableGraph_Test(
                wrappedGraph,
                () => new BidirectionalAdapterGraph<int, IEdge<int>>(wrappedGraph));
        }

        [Test]
        public void OutEdges_Throws()
        {
            var wrappedGraph1 = new AdjacencyGraph<TestVertex, IEdge<TestVertex>>();
            var graph1 = new BidirectionalAdapterGraph<TestVertex, IEdge<TestVertex>>(wrappedGraph1);
            OutEdges_NullThrows_Test(graph1);

            var wrappedGraph2 = new AdjacencyGraph<EquatableTestVertex, Edge<EquatableTestVertex>>();
            var graph2 = new BidirectionalAdapterGraph<EquatableTestVertex, Edge<EquatableTestVertex>>(wrappedGraph2);
            OutEdges_Throws_Test(graph2);
        }

        #endregion

        #region In Edges

        [Test]
        public void InEdge()
        {
            var wrappedGraph = new AdjacencyGraph<int, IEdge<int>>();
            InEdge_ImmutableGraph_Test(
                wrappedGraph,
                () => new BidirectionalAdapterGraph<int, IEdge<int>>(wrappedGraph));
        }

        [Test]
        public void InEdge_Throws()
        {
            var wrappedGraph1 = new AdjacencyGraph<int, IEdge<int>>();
            InEdge_Throws_ImmutableGraph_Test(
                wrappedGraph1,
                () => new BidirectionalAdapterGraph<int, IEdge<int>>(wrappedGraph1));

            var wrappedGraph2 = new AdjacencyGraph<TestVertex, IEdge<TestVertex>>();
            var graph2 = new BidirectionalAdapterGraph<TestVertex, IEdge<TestVertex>>(wrappedGraph2);
            InEdge_NullThrows_Test(graph2);
        }

        [Test]
        public void InEdges()
        {
            var wrappedGraph = new AdjacencyGraph<int, IEdge<int>>();
            InEdges_ImmutableGraph_Test(
                wrappedGraph,
                () => new BidirectionalAdapterGraph<int, IEdge<int>>(wrappedGraph));
        }

        [Test]
        public void InEdges_Throws()
        {
            var wrappedGraph1 = new AdjacencyGraph<TestVertex, IEdge<TestVertex>>();
            var graph1 = new BidirectionalAdapterGraph<TestVertex, IEdge<TestVertex>>(wrappedGraph1);
            InEdges_NullThrows_Test(graph1);

            var wrappedGraph2 = new AdjacencyGraph<EquatableTestVertex, Edge<EquatableTestVertex>>();
            var graph2 = new BidirectionalAdapterGraph<EquatableTestVertex, Edge<EquatableTestVertex>>(wrappedGraph2);
            InEdges_Throws_Test(graph2);
        }

        #endregion

        // The Bidirectional adapter has limits in its implementation
        // It is not informed about changes made in the wrapped graph
        // So the In-Edges/Degree etc par becomes obsolete after any wrapped
        // graph update. => Be aware of the issue
        // Otherwise it will be possible to have an implementation that tracks
        // underlying graph updates but it also implies a stronger constraint
        // on the wrapped graph (has events to notify changes).
        [Test]
        public void AdapterLimits()
        {
            var edge12 = Edge.Create(1, 2);
            var edge13 = Edge.Create(1, 3);
            var edge23 = Edge.Create(2, 3);
            var edge33 = Edge.Create(3, 3);

            var wrappedGraph = new AdjacencyGraph<int, IEdge<int>>();
            wrappedGraph.AddVerticesAndEdgeRange( edge12, edge13, edge23, edge33 );

            var graph = new BidirectionalAdapterGraph<int, IEdge<int>>(wrappedGraph);
            graph.AssertHasVertices(1, 2, 3 );
            graph.AssertHasEdges(edge12, edge13, edge23, edge33 );
            AssertHasOutEdges(graph, 3, edge33 );
            AssertHasInEdges(graph, 3, edge13, edge23, edge33 );

            var edge35 = Edge.Create(3, 5);
            // Update wrapped graph => breaking change
            var edge43 = Edge.Create(4, 3);
            wrappedGraph.AddVerticesAndEdgeRange( edge35, edge43 );

            graph.AssertHasVertices(1, 2, 3, 4, 5 );                  // Vertices data are up to date
            graph.AssertHasEdges(edge12, edge13, edge23, edge33, edge35, edge43 );    // Edges data are up to date
            AssertHasOutEdges(graph, 3, edge33, edge35 );          // Out data are up to date
            AssertHasInEdges(graph, 3, edge13, edge23, edge33 );   // Missing edge43
        }

        [Test]
        public void Degree()
        {
            var wrappedGraph = new AdjacencyGraph<int, IEdge<int>>();
            Degree_ImmutableGraph_Test(
                wrappedGraph,
                () => new BidirectionalAdapterGraph<int, IEdge<int>>(wrappedGraph));
        }

        [Test]
        public void Degree_Throws()
        {
            var wrappedGraph = new AdjacencyGraph<EquatableTestVertex, Edge<EquatableTestVertex>>();
            var graph = new BidirectionalAdapterGraph<EquatableTestVertex, Edge<EquatableTestVertex>>(wrappedGraph);
            Degree_Throws_Test(graph);
        }

        #region Try Get Edges

        [Test]
        public void TryGetEdge()
        {
            var wrappedGraph = new AdjacencyGraph<int, IEdge<int>>();
            TryGetEdge_ImmutableGraph_Test(
                wrappedGraph,
                () => new BidirectionalAdapterGraph<int, IEdge<int>>(wrappedGraph));
        }

        [Test]
        public void TryGetEdge_Throws()
        {
            var wrappedGraph = new AdjacencyGraph<TestVertex, IEdge<TestVertex>>();
            var graph = new BidirectionalAdapterGraph<TestVertex, IEdge<TestVertex>>(wrappedGraph);
            TryGetEdge_Throws_Test(graph);
        }

        [Test]
        public void GetEdges()
        {
            var wrappedGraph = new AdjacencyGraph<int, IEdge<int>>();
            GetEdges_ImmutableGraph_Test(
                wrappedGraph,
                () => new BidirectionalAdapterGraph<int, IEdge<int>>(wrappedGraph));
        }

        [Test]
        public void GetEdges_Throws()
        {
            var wrappedGraph = new AdjacencyGraph<TestVertex, IEdge<TestVertex>>();
            var graph = new BidirectionalAdapterGraph<TestVertex, IEdge<TestVertex>>(wrappedGraph);
            GetEdges_Throws_Test(graph);
        }

        [Test]
        public void GetOutEdges()
        {
            var wrappedGraph = new AdjacencyGraph<int, IEdge<int>>();
            GetOutEdges_ImmutableGraph_Test(
                wrappedGraph,
                () => new BidirectionalAdapterGraph<int, IEdge<int>>(wrappedGraph));
        }

        [Test]
        public void GetOutEdges_Throws()
        {
            var wrappedGraph = new AdjacencyGraph<TestVertex, IEdge<TestVertex>>();
            var graph = new BidirectionalAdapterGraph<TestVertex, IEdge<TestVertex>>(wrappedGraph);
            GetOutEdges_Throws_Test(graph);
        }

        [Test]
        public void GetInEdges()
        {
            var wrappedGraph = new AdjacencyGraph<int, IEdge<int>>();
            GetInEdges_ImmutableGraph_Test(
                wrappedGraph,
                () => new BidirectionalAdapterGraph<int, IEdge<int>>(wrappedGraph));
        }

        [Test]
        public void GetInEdges_Throws()
        {
            var wrappedGraph = new AdjacencyGraph<TestVertex, IEdge<TestVertex>>();
            var graph = new BidirectionalAdapterGraph<TestVertex, IEdge<TestVertex>>(wrappedGraph);
            GetInEdges_Throws_Test(graph);
        }

        #endregion
    }
}