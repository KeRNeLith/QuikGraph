using System;
using NUnit.Framework;
using static QuikGraph.Tests.GraphTestHelpers;

namespace QuikGraph.Tests.Structures
{
    /// <summary>
    /// Tests for <see cref="EdgeListGraph{TVertex,TEdge}"/>.
    /// </summary>
    [TestFixture]
    internal sealed class EdgeListGraphTests : GraphTestsBase
    {
        [Test]
        public void Construction()
        {
            var graph = new EdgeListGraph<int, IEdge<int>>();
            AssertGraphProperties(graph);

            graph = new EdgeListGraph<int, IEdge<int>>(false, false);
            AssertGraphProperties(graph, false, false);

            graph = new EdgeListGraph<int, IEdge<int>>(false, true);
            AssertGraphProperties(graph, false);

            graph = new EdgeListGraph<int, IEdge<int>>(true, false);
            AssertGraphProperties(graph, parallelEdges: false);

            graph = new EdgeListGraph<int, IEdge<int>>(true, true);
            AssertGraphProperties(graph);

            #region Local function

            void AssertGraphProperties<TVertex, TEdge>(
                EdgeListGraph<TVertex, TEdge> g,
                bool isDirected = true,
                bool parallelEdges = true)
                where TEdge : IEdge<TVertex>
            {
                Assert.AreEqual(isDirected, g.IsDirected);
                Assert.AreEqual(parallelEdges, g.AllowParallelEdges);
                AssertEmptyGraph(g);
            }

            #endregion
        }

        #region Add Edges

        [Test]
        public void AddEdge_ParallelEdges()
        {
            var directedGraph = new EdgeListGraph<int, IEdge<int>>(true, true);
            var undirectedGraph = new EdgeListGraph<int, IEdge<int>>(false, true);
            AddEdge_ParallelEdges_EdgesOnly_Test(
                directedGraph,
                undirectedGraph,
                (graph, edge) => graph.AddEdge(edge));
        }

        [Test]
        public void AddEdge_ParallelEdges_EquatableEdge()
        {
            var directedGraph = new EdgeListGraph<int, EquatableEdge<int>>(true, true);
            var undirectedGraph = new EdgeListGraph<int, EquatableEdge<int>>(false, true);
            AddEdge_ParallelEdges_EquatableEdge_EdgesOnly_Test(
                directedGraph,
                undirectedGraph,
                (graph, edge) => graph.AddEdge(edge));
        }

        [Test]
        public void AddEdge_NoParallelEdges()
        {
            var directedGraph = new EdgeListGraph<int, IEdge<int>>(true, false);
            var undirectedGraph = new EdgeListGraph<int, IEdge<int>>(false, false);
            AddEdge_NoParallelEdges_EdgesOnly_Test(
                directedGraph,
                undirectedGraph,
                (graph, edge) => graph.AddEdge(edge));
        }

        [Test]
        public void AddEdge_NoParallelEdges_EquatableEdge()
        {
            var directedGraph = new EdgeListGraph<int, EquatableEdge<int>>(true, false);
            var undirectedGraph = new EdgeListGraph<int, EquatableEdge<int>>(false, false);
            AddEdge_NoParallelEdges_EquatableEdge_EdgesOnly_Test(
                directedGraph,
                undirectedGraph,
                (graph, edge) => graph.AddEdge(edge));
        }

        [Test]
        public void AddEdge_Throws()
        {
            var graph = new EdgeListGraph<int, IEdge<int>>();
            AddEdge_Throws_EdgesOnly_Test(graph);
        }

        [Test]
        public void AddEdgeRange()
        {
            var graph = new EdgeListGraph<int, IEdge<int>>(true, false);
            AddEdgeRange_EdgesOnly_Test(graph);
        }

        [Test]
        public void AddEdgeRange_Throws()
        {
            var graph = new EdgeListGraph<int, IEdge<int>>();
            AddEdgeRange_Throws_EdgesOnly_Test(graph);
        }

        #endregion

        #region Add Vertices & Edges

        [Test]
        public void AddVerticesAndEdge_ParallelEdges()
        {
            var directedGraph = new EdgeListGraph<int, IEdge<int>>(true, true);
            var undirectedGraph = new EdgeListGraph<int, IEdge<int>>(false, true);
            AddEdge_ParallelEdges_EdgesOnly_Test(
                directedGraph,
                undirectedGraph,
                (graph, edge) => graph.AddVerticesAndEdge(edge));
        }

        [Test]
        public void AddVerticesAndEdge_ParallelEdges_EquatableEdge()
        {
            var directedGraph = new EdgeListGraph<int, EquatableEdge<int>>(true, true);
            var undirectedGraph = new EdgeListGraph<int, EquatableEdge<int>>(false, true);
            AddEdge_ParallelEdges_EquatableEdge_EdgesOnly_Test(
                directedGraph,
                undirectedGraph,
                (graph, edge) => graph.AddVerticesAndEdge(edge));
        }

        [Test]
        public void AddVerticesAndEdge_NoParallelEdges()
        {
            var directedGraph = new EdgeListGraph<int, IEdge<int>>(true, false);
            var undirectedGraph = new EdgeListGraph<int, IEdge<int>>(false, false);
            AddEdge_NoParallelEdges_EdgesOnly_Test(
                directedGraph,
                undirectedGraph,
                (graph, edge) => graph.AddVerticesAndEdge(edge));
        }

        [Test]
        public void AddVerticesAndEdge_NoParallelEdges_EquatableEdge()
        {
            var directedGraph = new EdgeListGraph<int, EquatableEdge<int>>(true, false);
            var undirectedGraph = new EdgeListGraph<int, EquatableEdge<int>>(false, false);
            AddEdge_NoParallelEdges_EquatableEdge_EdgesOnly_Test(
                directedGraph,
                undirectedGraph,
                (graph, edge) => graph.AddVerticesAndEdge(edge));
        }

        [Test]
        public void AddVerticesAndEdge_Throws()
        {
            var graph = new EdgeListGraph<int, IEdge<int>>();
            AddVerticesAndEdge_Throws_EdgesOnly_Test(graph);
        }

        [Test]
        public void AddVerticesAndEdgeRange()
        {
            var graph = new EdgeListGraph<int, IEdge<int>>();
            AddVerticesAndEdgeRange_EdgesOnly_Test(graph);
        }

        [Test]
        public void AddVerticesAndEdgeRange_Throws()
        {
            var graph = new EdgeListGraph<int, IEdge<int>>();
            AddVerticesAndEdgeRange_Throws_EdgesOnly_Test(graph);
        }

        #endregion

        #region Contains Vertex

        [Test]
        public void ContainsVertex()
        {
            var graph = new EdgeListGraph<TestVertex, IEdge<TestVertex>>();
            ContainsVertex_OnlyEdges_Test(graph);
        }

        [Test]
        public void ContainsVertex_EquatableVertex()
        {
            var graph = new EdgeListGraph<EquatableTestVertex, IEdge<EquatableTestVertex>>();
            ContainsVertex_EquatableVertex_OnlyEdges_Test(graph);
        }

        [Test]
        public void ContainsVertex_Throws()
        {
            var graph = new EdgeListGraph<TestVertex, IEdge<TestVertex>>();
            ContainsVertex_Throws_Test(graph);
        }

        #endregion

        #region Contains Edge

        [Test]
        public void ContainsEdge()
        {
            var graph = new EdgeListGraph<int, IEdge<int>>();
            ContainsEdge_EdgesOnly_Test(graph);
        }

        [Test]
        public void ContainsEdge_EquatableEdge()
        {
            var graph = new EdgeListGraph<int, EquatableEdge<int>>();
            ContainsEdge_EquatableEdge_EdgesOnly_Test(graph);
        }

        [Test]
        public void ContainsEdge_Throws()
        {
            var graph = new EdgeListGraph<TestVertex, IEdge<TestVertex>>();
            ContainsEdge_NullThrows_Test(graph);
        }

        #endregion

        #region Remove Edges

        [Test]
        public void RemoveEdge()
        {
            var graph = new EdgeListGraph<int, IEdge<int>>();
            RemoveEdge_EdgesOnly_Test(graph);
        }

        [Test]
        public void RemoveEdge_EquatableEdge()
        {
            var graph = new EdgeListGraph<int, EquatableEdge<int>>();
            RemoveEdge_EquatableEdge_EdgesOnly_Test(graph);
        }

        [Test]
        public void RemoveEdge_Throws()
        {
            var graph = new EdgeListGraph<TestVertex, IEdge<TestVertex>>();
            RemoveEdge_Throws_Test(graph);
        }

        [Test]
        public void RemoveEdgeIf()
        {
            var graph = new EdgeListGraph<int, IEdge<int>>();
            RemoveEdgeIf_EdgesOnly_Test(graph);
        }

        [Test]
        public void RemoveEdgeIf_Throws()
        {
            var graph = new EdgeListGraph<TestVertex, IEdge<TestVertex>>();
            RemoveEdgeIf_Throws_Test(graph);
        }

        #endregion

        #region Clear

        [Test]
        public void Clear()
        {
            int edgesRemoved = 0;

            var graph = new EdgeListGraph<int, IEdge<int>>();
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
            CheckCounter(0);

            graph.AddVerticesAndEdge(Edge.Create(1, 2));
            graph.AddVerticesAndEdge(Edge.Create(2, 3));
            graph.AddVerticesAndEdge(Edge.Create(3, 1));

            graph.Clear();
            AssertEmptyGraph(graph);
            CheckCounter(3);

            #region Local function

            void CheckCounter(int expectedEdgesRemoved)
            {
                Assert.AreEqual(expectedEdgesRemoved, edgesRemoved);
                edgesRemoved = 0;
            }

            #endregion
        }

        #endregion

        [Test]
        public void Clone()
        {
            var graph = new EdgeListGraph<int, IEdge<int>>();
            AssertEmptyGraph(graph);

            var clonedGraph = graph.Clone();
            Assert.IsNotNull(clonedGraph);
            AssertEmptyGraph(clonedGraph);

            clonedGraph = (EdgeListGraph<int, IEdge<int>>)((ICloneable)graph).Clone();
            Assert.IsNotNull(clonedGraph);
            AssertEmptyGraph(clonedGraph);

            var edge1 = Edge.Create(1, 2);
            var edge2 = Edge.Create(1, 3);
            var edge3 = Edge.Create(2, 3);
            graph.AddVerticesAndEdgeRange( edge1, edge2, edge3 );
            graph.AssertHasVertices(1, 2, 3 );
            graph.AssertHasEdges(edge1, edge2, edge3 );

            clonedGraph = graph.Clone();
            Assert.IsNotNull(clonedGraph);
            clonedGraph.AssertHasVertices(1, 2, 3 );
            clonedGraph.AssertHasEdges(edge1, edge2, edge3 );

            clonedGraph = (EdgeListGraph<int, IEdge<int>>)((ICloneable)graph).Clone();
            Assert.IsNotNull(clonedGraph);
            clonedGraph.AssertHasVertices(1, 2, 3 );
            clonedGraph.AssertHasEdges(edge1, edge2, edge3 );
        }
    }
}