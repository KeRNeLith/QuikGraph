using System;
using JetBrains.Annotations;
using NUnit.Framework;
using static QuikGraph.Tests.GraphTestHelpers;

namespace QuikGraph.Tests.Structures
{
    /// <summary>
    /// Tests for <see cref="AdjacencyGraph{TVertex,TEdge}"/>.
    /// </summary>
    [TestFixture]
    internal sealed class AdjacencyGraphTests : GraphTestsBase
    {
        [Test]
        public void Construction()
        {
            var graph = new AdjacencyGraph<int, IEdge<int>>();
            AssertGraphProperties(graph);

            graph = new AdjacencyGraph<int, IEdge<int>>(true);
            AssertGraphProperties(graph);

            graph = new AdjacencyGraph<int, IEdge<int>>(false);
            AssertGraphProperties(graph, false);

            graph = new AdjacencyGraph<int, IEdge<int>>(true, 12);
            AssertGraphProperties(graph);

            graph = new AdjacencyGraph<int, IEdge<int>>(false, 12);
            AssertGraphProperties(graph, false);

            graph = new AdjacencyGraph<int, IEdge<int>>(true, 42, 12);
            AssertGraphProperties(graph, edgeCapacity: 12);

            graph = new AdjacencyGraph<int, IEdge<int>>(false, 42, 12);
            AssertGraphProperties(graph, false, 12);

            #region Local function

            void AssertGraphProperties<TVertex, TEdge>(
                AdjacencyGraph<TVertex, TEdge> g,
                bool parallelEdges = true,
                int edgeCapacity = 0)
                where TEdge : IEdge<TVertex>
            {
                Assert.IsTrue(g.IsDirected);
                Assert.AreEqual(parallelEdges, g.AllowParallelEdges);
                AssertEmptyGraph(g);
                Assert.AreEqual(edgeCapacity, g.EdgeCapacity);
                Assert.AreSame(typeof(int), g.VertexType);
                Assert.AreSame(typeof(IEdge<int>), g.EdgeType);
            }

            #endregion
        }

        #region Add Vertices

        [Test]
        public void AddVertex()
        {
            var graph = new AdjacencyGraph<TestVertex, IEdge<TestVertex>>();
            AddVertex_Test(graph);
        }

        [Test]
        public void AddVertex_Throws()
        {
            var graph = new AdjacencyGraph<TestVertex, IEdge<TestVertex>>();
            AddVertex_Throws_Test(graph);
        }

        [Test]
        public void AddVertex_EquatableVertex()
        {
            var graph = new AdjacencyGraph<EquatableTestVertex, Edge<EquatableTestVertex>>();
            AddVertex_EquatableVertex_Test(graph);
        }

        [Test]
        public void AddVertexRange()
        {
            var graph = new AdjacencyGraph<TestVertex, IEdge<TestVertex>>();
            AddVertexRange_Test(graph);
        }

        [Test]
        public void AddVertexRange_Throws()
        {
            var graph = new AdjacencyGraph<TestVertex, IEdge<TestVertex>>();
            AddVertexRange_Throws_Test(graph);
        }

        #endregion

        #region Add Edges

        [Test]
        public void AddEdge_ParallelEdges()
        {
            var graph = new AdjacencyGraph<int, IEdge<int>>();
            AddEdge_ParallelEdges_Test(graph);
        }

        [Test]
        public void AddEdge_ParallelEdges_EquatableEdge()
        {
            var graph = new AdjacencyGraph<int, EquatableEdge<int>>();
            AddEdge_ParallelEdges_EquatableEdge_Test(graph);
        }

        [Test]
        public void AddEdge_NoParallelEdges()
        {
            var graph = new AdjacencyGraph<int, IEdge<int>>(false);
            AddEdge_NoParallelEdges_Test(graph);
        }

        [Test]
        public void AddEdge_NoParallelEdges_EquatableEdge()
        {
            var graph = new AdjacencyGraph<int, EquatableEdge<int>>(false);
            AddEdge_NoParallelEdges_EquatableEdge_Test(graph);
        }

        [Test]
        public void AddEdge_Throws()
        {
            var graph = new AdjacencyGraph<int, IEdge<int>>();
            AddEdge_Throws_Test(graph);
        }

        [Test]
        public void AddEdgeRange()
        {
            var graph = new AdjacencyGraph<int, IEdge<int>>(false);
            AddEdgeRange_Test(graph);
        }

        [Test]
        public void AddEdgeRange_Throws()
        {
            var graph = new AdjacencyGraph<int, IEdge<int>>();
            AddEdgeRange_Throws_Test(graph);
        }

        #endregion

        #region Add Vertices & Edges

        [Test]
        public void AddVerticesAndEdge()
        {
            var graph = new AdjacencyGraph<int, IEdge<int>>();
            AddVerticesAndEdge_Test(graph);
        }

        [Test]
        public void AddVerticesAndEdge_Throws()
        {
            var graph = new AdjacencyGraph<int, IEdge<int>>();
            AddVerticesAndEdge_Throws_Test(graph);
        }

        [Test]
        public void AddVerticesAndEdgeRange()
        {
            var graph = new AdjacencyGraph<int, IEdge<int>>(false);
            AddVerticesAndEdgeRange_Test(graph);
        }

        [Test]
        public void AddVerticesAndEdgeRange_Throws()
        {
            var graph = new AdjacencyGraph<int, IEdge<int>>(false);
            AddVerticesAndEdgeRange_Throws_Test(graph);
        }

        #endregion

        #region Contains Vertex

        [Test]
        public void ContainsVertex()
        {
            var graph = new AdjacencyGraph<TestVertex, IEdge<TestVertex>>();
            ContainsVertex_Test(graph);
        }

        [Test]
        public void ContainsVertex_EquatableVertex()
        {
            var graph = new AdjacencyGraph<EquatableTestVertex, Edge<EquatableTestVertex>>();
            ContainsVertex_EquatableVertex_Test(graph);
        }

        [Test]
        public void ContainsVertex_Throws()
        {
            var graph = new AdjacencyGraph<TestVertex, IEdge<TestVertex>>();
            ContainsVertex_Throws_Test(graph);
        }

        #endregion

        #region Contains Edge

        [Test]
        public void ContainsEdge()
        {
            var graph = new AdjacencyGraph<int, IEdge<int>>();
            ContainsEdge_Test(graph);
        }

        [Test]
        public void ContainsEdge_EquatableEdge()
        {
            var graph = new AdjacencyGraph<int, EquatableEdge<int>>();
            ContainsEdge_EquatableEdge_Test(graph);
        }

        [Test]
        public void ContainsEdge_SourceTarget()
        {
            var graph = new AdjacencyGraph<int, IEdge<int>>();
            ContainsEdge_SourceTarget_Test(graph);
        }

        [Test]
        public void ContainsEdge_Throws()
        {
            var graph = new AdjacencyGraph<TestVertex, IEdge<TestVertex>>();
            ContainsEdge_NullThrows_Test(graph);
            ContainsEdge_SourceTarget_Throws_Test(graph);
        }

        #endregion

        #region Out Edges

        [Test]
        public void OutEdge()
        {
            var graph = new AdjacencyGraph<int, IEdge<int>>();
            OutEdge_Test(graph);
        }

        [Test]
        public void OutEdge_Throws()
        {
            var graph1 = new AdjacencyGraph<TestVertex, IEdge<TestVertex>>();
            OutEdge_NullThrows_Test(graph1);

            var graph2 = new AdjacencyGraph<int, IEdge<int>>();
            OutEdge_Throws_Test(graph2);
        }

        [Test]
        public void OutEdges()
        {
            var graph = new AdjacencyGraph<int, IEdge<int>>();
            OutEdges_Test(graph);
        }

        [Test]
        public void OutEdges_Throws()
        {
            var graph1 = new AdjacencyGraph<TestVertex, IEdge<TestVertex>>();
            OutEdges_NullThrows_Test(graph1);

            var graph2 = new AdjacencyGraph<EquatableTestVertex, Edge<EquatableTestVertex>>();
            OutEdges_Throws_Test(graph2);
        }

        #endregion

        #region Try Get Edges

        [Test]
        public void TryGetEdge()
        {
            var graph = new AdjacencyGraph<int, IEdge<int>>();
            TryGetEdge_Test(graph);
        }

        [Test]
        public void TryGetEdge_Throws()
        {
            var graph = new AdjacencyGraph<TestVertex, IEdge<TestVertex>>();
            TryGetEdge_Throws_Test(graph);
        }

        [Test]
        public void GetEdges()
        {
            var graph = new AdjacencyGraph<int, IEdge<int>>();
            GetEdges_Test(graph);
        }

        [Test]
        public void GetEdges_Throws()
        {
            var graph = new AdjacencyGraph<TestVertex, IEdge<TestVertex>>();
            GetEdges_Throws_Test(graph);
        }

        [Test]
        public void GetOutEdges()
        {
            var graph = new AdjacencyGraph<int, IEdge<int>>();
            GetOutEdges_Test(graph);
        }

        [Test]
        public void GetOutEdges_Throws()
        {
            var graph = new AdjacencyGraph<TestVertex, IEdge<TestVertex>>();
            GetOutEdges_Throws_Test(graph);
        }

        #endregion

        #region Remove Vertices

        [Test]
        public void RemoveVertex()
        {
            var graph = new AdjacencyGraph<int, IEdge<int>>();
            RemoveVertex_Test(graph);
        }

        [Test]
        public void RemoveVertex_Throws()
        {
            var graph = new AdjacencyGraph<TestVertex, IEdge<TestVertex>>();
            RemoveVertex_Throws_Test(graph);
        }

        [Test]
        public void RemoveVertexIf()
        {
            var graph = new AdjacencyGraph<int, IEdge<int>>();
            RemoveVertexIf_Test(graph);

            graph = new AdjacencyGraph<int, IEdge<int>>();
            RemoveVertexIf_Test2(graph);
        }

        [Test]
        public void RemoveVertexIf_Throws()
        {
            var graph = new AdjacencyGraph<TestVertex, IEdge<TestVertex>>();
            RemoveVertexIf_Throws_Test(graph);
        }

        #endregion

        #region Remove Edges

        [Test]
        public void RemoveEdge()
        {
            var graph = new AdjacencyGraph<int, IEdge<int>>();
            RemoveEdge_Test(graph);
        }

        [Test]
        public void RemoveEdge_EquatableEdge()
        {
            var graph = new AdjacencyGraph<int, EquatableEdge<int>>();
            RemoveEdge_EquatableEdge_Test(graph);
        }

        [Test]
        public void RemoveEdge_Throws()
        {
            var graph = new AdjacencyGraph<TestVertex, IEdge<TestVertex>>();
            RemoveEdge_Throws_Test(graph);
        }

        [Test]
        public void RemoveEdgeIf()
        {
            var graph = new AdjacencyGraph<int, IEdge<int>>();
            RemoveEdgeIf_Test(graph);
        }

        [Test]
        public void RemoveEdgeIf_Throws()
        {
            var graph = new AdjacencyGraph<TestVertex, IEdge<TestVertex>>();
            RemoveEdgeIf_Throws_Test(graph);
        }

        [Test]
        public void RemoveOutEdgeIf()
        {
            var graph = new AdjacencyGraph<int, IEdge<int>>();
            RemoveOutEdgeIf_Test(graph);
        }

        [Test]
        public void RemoveOutEdgeIf_Throws()
        {
            var graph = new AdjacencyGraph<TestVertex, IEdge<TestVertex>>();
            RemoveOutEdgeIf_Throws_Test(graph);
        }

        #endregion

        #region Clear

        [Test]
        public void Clear()
        {
            int verticesRemoved = 0;
            int edgesRemoved = 0;

            var graph = new AdjacencyGraph<int, IEdge<int>>();
            // ReSharper disable once ParameterOnlyUsedForPreconditionCheck.Local
            graph.VertexRemoved += v =>
            {
                Assert.IsNotNull(v);
                // ReSharper disable once AccessToModifiedClosure
                ++verticesRemoved;
            };
            // ReSharper disable once ParameterOnlyUsedForPreconditionCheck.Local
            graph.EdgeRemoved += e =>
            {
                Assert.IsNotNull(e);
                // ReSharper disable once AccessToModifiedClosure
                ++edgesRemoved;
            };

            AssertEmptyGraph(graph);

            graph.Clear();
            AssertEmptyGraph(graph);
            CheckCounters(0, 0);

            graph.AddVerticesAndEdge(Edge.Create(1, 2));
            graph.AddVerticesAndEdge(Edge.Create(2, 3));
            graph.AddVerticesAndEdge(Edge.Create(3, 1));

            graph.Clear();
            AssertEmptyGraph(graph);
            CheckCounters(3, 3);

            #region Local function

            void CheckCounters(int expectedVerticesRemoved, int expectedEdgesRemoved)
            {
                Assert.AreEqual(expectedVerticesRemoved, verticesRemoved);
                Assert.AreEqual(expectedEdgesRemoved, edgesRemoved);
                verticesRemoved = 0;
                edgesRemoved = 0;
            }

            #endregion
        }

        private static void ClearEdgesCommon([NotNull, InstantHandle] Action<AdjacencyGraph<int, IEdge<int>>, int> clearEdges)
        {
            int edgesRemoved = 0;

            var graph = new AdjacencyGraph<int, IEdge<int>>();
            // ReSharper disable once ParameterOnlyUsedForPreconditionCheck.Local
            graph.EdgeRemoved += e =>
            {
                Assert.IsNotNull(e);
                // ReSharper disable once AccessToModifiedClosure
                ++edgesRemoved;
            };

            AssertEmptyGraph(graph);

            // Clear 1 => not in graph
            clearEdges(graph, 1);
            AssertEmptyGraph(graph);
            CheckCounter(0);

            // Clear 1 => In graph but no out edges
            graph.AddVertex(1);
            clearEdges(graph, 1);
            graph.AssertHasVertices(new[] { 1 });
            graph.AssertNoEdge();
            CheckCounter(0);

            var edge12 = Edge.Create(1, 2);
            var edge23 = Edge.Create(2, 3);
            graph.AddVerticesAndEdgeRange( edge12, edge23 );

            // Clear 1
            clearEdges(graph, 1);

            graph.AssertHasEdges(new[] { edge23 });
            CheckCounter(1);

            var edge13 = Edge.Create(1, 3);
            var edge31 = Edge.Create(3, 1);
            var edge32 = Edge.Create(3, 2);
            graph.AddVerticesAndEdgeRange( edge12, edge13, edge31, edge32 );

            // Clear 3
            clearEdges(graph, 3);

            graph.AssertHasEdges(new[] { edge12, edge13, edge23 });
            CheckCounter(2);

            // Clear 1
            clearEdges(graph, 1);

            graph.AssertHasEdges(new[] { edge23 });
            CheckCounter(2);

            // Clear 2 = Clear
            clearEdges(graph, 2);

            graph.AssertNoEdge();
            CheckCounter(1);

            #region Local function

            void CheckCounter(int expectedRemovedEdges)
            {
                Assert.AreEqual(expectedRemovedEdges, edgesRemoved);
                edgesRemoved = 0;
            }

            #endregion
        }

        [Test]
        public void ClearOutEdges()
        {
            ClearEdgesCommon((graph, vertex) => graph.ClearOutEdges(vertex));
        }

        [Test]
        public void ClearEdges()
        {
            ClearEdgesCommon((graph, vertex) => graph.ClearEdges(vertex));
        }

        [Test]
        public void ClearEdges_Throws()
        {
            // ReSharper disable AssignNullToNotNullAttribute
            Assert.Throws<ArgumentNullException>(() => new AdjacencyGraph<TestVertex, IEdge<TestVertex>>().ClearOutEdges(null));
            Assert.Throws<ArgumentNullException>(() => new AdjacencyGraph<TestVertex, IEdge<TestVertex>>().ClearEdges(null));
            // ReSharper restore AssignNullToNotNullAttribute
        }

        #endregion

        [Test]
        public void Clone()
        {
            var graph = new AdjacencyGraph<int, IEdge<int>>();
            AssertEmptyGraph(graph);

            AdjacencyGraph<int, IEdge<int>> clonedGraph = graph.Clone();
            Assert.IsNotNull(clonedGraph);
            AssertEmptyGraph(clonedGraph);

            clonedGraph = (AdjacencyGraph<int, IEdge<int>>)((ICloneable)graph).Clone();
            Assert.IsNotNull(clonedGraph);
            AssertEmptyGraph(clonedGraph);

            graph.AddVertexRange( 1, 2, 3 );
            graph.AssertHasVertices(new[] { 1, 2, 3 });
            graph.AssertNoEdge();

            clonedGraph = graph.Clone();
            Assert.IsNotNull(clonedGraph);
            clonedGraph.AssertHasVertices(new[] { 1, 2, 3 });
            clonedGraph.AssertNoEdge();

            clonedGraph = (AdjacencyGraph<int, IEdge<int>>)((ICloneable)graph).Clone();
            Assert.IsNotNull(clonedGraph);
            clonedGraph.AssertHasVertices(new[] { 1, 2, 3 });
            clonedGraph.AssertNoEdge();

            var edge1 = Edge.Create(1, 2);
            var edge2 = Edge.Create(1, 3);
            var edge3 = Edge.Create(2, 3);
            graph.AddVerticesAndEdgeRange( edge1, edge2, edge3 );
            graph.AssertHasVertices(new[] { 1, 2, 3 });
            graph.AssertHasEdges(new[] { edge1, edge2, edge3 });

            clonedGraph = graph.Clone();
            Assert.IsNotNull(clonedGraph);
            clonedGraph.AssertHasVertices(new[] { 1, 2, 3 });
            clonedGraph.AssertHasEdges(new[] { edge1, edge2, edge3 });

            clonedGraph = (AdjacencyGraph<int, IEdge<int>>)((ICloneable)graph).Clone();
            Assert.IsNotNull(clonedGraph);
            clonedGraph.AssertHasVertices(new[] { 1, 2, 3 });
            clonedGraph.AssertHasEdges(new[] { edge1, edge2, edge3 });

            graph.AddVertex(4);
            graph.AssertHasVertices(new[] { 1, 2, 3, 4 });
            graph.AssertHasEdges(new[] { edge1, edge2, edge3 });

            clonedGraph = graph.Clone();
            Assert.IsNotNull(clonedGraph);
            clonedGraph.AssertHasVertices(new[] { 1, 2, 3, 4 });
            clonedGraph.AssertHasEdges(new[] { edge1, edge2, edge3 });

            clonedGraph = (AdjacencyGraph<int, IEdge<int>>)((ICloneable)graph).Clone();
            Assert.IsNotNull(clonedGraph);
            clonedGraph.AssertHasVertices(new[] { 1, 2, 3, 4 });
            clonedGraph.AssertHasEdges(new[] { edge1, edge2, edge3 });
        }

        [Test]
        public void TrimEdgeExcess()
        {
            var graph = new AdjacencyGraph<int, IEdge<int>>(true, 12, 50);
            graph.AddVerticesAndEdgeRange(
                Edge.Create(1, 2),
                Edge.Create(1, 3),
                Edge.Create(1, 4)
            );

            Assert.DoesNotThrow(graph.TrimEdgeExcess);
        }
    }
}