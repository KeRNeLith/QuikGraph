using System;
using System.Linq;
using NUnit.Framework;
using static QuikGraph.Tests.GraphTestHelpers;

namespace QuikGraph.Tests.Structures
{
    /// <summary>
    /// Tests for <see cref="BidirectionalMatrixGraph{TEdge}"/>.
    /// </summary>
    [TestFixture]
    internal sealed class BidirectionalMatrixGraphTests : GraphTestsBase
    {
        [Test]
        public void Construction()
        {
            var graph = new BidirectionalMatrixGraph<Edge<int>>(1);
            AssertGraphProperties(graph, 1);

            graph = new BidirectionalMatrixGraph<Edge<int>>(12);
            AssertGraphProperties(graph, 12);

            #region Local function

            void AssertGraphProperties<TEdge>(
                BidirectionalMatrixGraph<TEdge> g,
                int nbVertices)
                where TEdge : IEdge<int>
            {
                Assert.IsTrue(g.IsDirected);
                Assert.IsFalse(g.AllowParallelEdges);
                g.AssertHasVertices(Enumerable.Range(0, nbVertices));
                g.AssertNoEdge();
            }

            #endregion
        }

        [Test]
        public void Construction_Throws()
        {
            // ReSharper disable ObjectCreationAsStatement
            Assert.Throws<ArgumentException>(() => new BidirectionalMatrixGraph<Edge<int>>(0));
            Assert.Throws<ArgumentException>(() => new BidirectionalMatrixGraph<Edge<int>>(-1));
            Assert.Throws<ArgumentException>(() => new BidirectionalMatrixGraph<Edge<int>>(-42));
            // ReSharper restore ObjectCreationAsStatement
        }

        #region Add Edges

        [Test]
        public void AddEdge()
        {
            var graph = new BidirectionalMatrixGraph<IEdge<int>>(5);
            AddEdge_ForbiddenParallelEdges_Test(graph);
        }

        [Test]
        public void AddEdge_EquatableEdge()
        {
            var graph = new BidirectionalMatrixGraph<EquatableEdge<int>>(5);
            AddEdge_EquatableEdge_ForbiddenParallelEdges_Test(graph);
        }

        [Test]
        public void AddEdge_Throws()
        {
            var graph = new BidirectionalMatrixGraph<IEdge<int>>(2);

            // ReSharper disable once AssignNullToNotNullAttribute
            Assert.Throws<ArgumentNullException>(() => graph.AddEdge(null));

            graph.AddEdge(Edge.Create(0, 1));
            Assert.Throws<ParallelEdgeNotAllowedException>(() => graph.AddEdge(Edge.Create(0, 1)));

            Assert.Throws<VertexNotFoundException>(() => graph.AddEdge(Edge.Create(1, 2)));
            Assert.Throws<VertexNotFoundException>(() => graph.AddEdge(Edge.Create(2, 1)));
        }

        [Test]
        public void AddEdgeRange()
        {
            AddEdgeRange_ForbiddenParallelEdges_Test();
        }

        [Test]
        public void AddEdgeRange_Throws()
        {
            var graph = new BidirectionalMatrixGraph<IEdge<int>>(3);
            AddEdgeRange_ForbiddenParallelEdges_Throws_Test(graph);
        }

        #endregion

        #region Contains Vertex

        [Test]
        public void ContainsVertex()
        {
            var graph = new BidirectionalMatrixGraph<Edge<int>>(3);

            Assert.IsTrue(graph.ContainsVertex(0));
            Assert.IsTrue(graph.ContainsVertex(1));
            Assert.IsTrue(graph.ContainsVertex(2));
            Assert.IsFalse(graph.ContainsVertex(-1));
            Assert.IsFalse(graph.ContainsVertex(12));
        }

        #endregion

        #region Contains Edge

        [Test]
        public void ContainsEdge()
        {
            var graph = new BidirectionalMatrixGraph<IEdge<int>>(4);
            ContainsEdge_ForbiddenParallelEdges_ImmutableVertices_Test(graph);
        }

        [Test]
        public void ContainsEdge_EquatableEdge()
        {
            var graph = new BidirectionalMatrixGraph<EquatableEdge<int>>(4);
            ContainsEdge_EquatableEdges_ForbiddenParallelEdges_ImmutableVertices_Test(graph);
        }

        [Test]
        public void ContainsEdge_SourceTarget()
        {
            var graph = new BidirectionalMatrixGraph<IEdge<int>>(4);
            ContainsEdge_SourceTarget_ForbiddenParallelEdges_Test(graph);
        }

        [Test]
        public void ContainsEdge_Throws()
        {
            var graph = new BidirectionalMatrixGraph<Edge<int>>(3);
            ContainsEdge_NullThrows_Test(graph);
        }

        #endregion

        #region Out Edges

        [Test]
        public void OutEdge()
        {
            var graph = new BidirectionalMatrixGraph<IEdge<int>>(5);
            OutEdge_ImmutableVertices_Test(graph);
        }

        [Test]
        public void OutEdge_Throws()
        {
            var graph = new BidirectionalMatrixGraph<IEdge<int>>(3);
            OutEdge_Throws_ImmutableVertices_Test(graph);
        }

        [Test]
        public void OutEdges()
        {
            var graph = new BidirectionalMatrixGraph<IEdge<int>>(4);
            OutEdges_ImmutableVertices_Test(graph);
        }

        [Test]
        public void OutEdges_Throws()
        {
            var graph = new BidirectionalMatrixGraph<Edge<int>>(2);
            OutEdges_Throws_Matrix_Test(graph);
        }

        #endregion

        #region In Edges

        [Test]
        public void InEdge()
        {
            var graph = new BidirectionalMatrixGraph<IEdge<int>>(5);
            InEdge_ImmutableVertices_Test(graph);
        }

        [Test]
        public void InEdge_Throws()
        {
            var graph = new BidirectionalMatrixGraph<IEdge<int>>(3);
            InEdge_Throws_ImmutableVertices_Test(graph);
        }

        [Test]
        public void InEdges()
        {
            var graph = new BidirectionalMatrixGraph<IEdge<int>>(4);
            InEdges_ImmutableVertices_Test(graph);
        }

        [Test]
        public void InEdges_Throws()
        {
            var graph = new BidirectionalMatrixGraph<Edge<int>>(2);
            InEdges_Throws_Matrix_Test(graph);
        }

        #endregion

        [Test]
        public void Degree()
        {
            var graph = new BidirectionalMatrixGraph<IEdge<int>>(5);
            Degree_ImmutableVertices_Test(graph);
        }

        [Test]
        public void Degree_Throws()
        {
            var graph = new BidirectionalMatrixGraph<Edge<int>>(2);
            Degree_Throws_Matrix_Test(graph);
        }

        #region Try Get Edges

        [Test]
        public void TryGetEdge()
        {
            var graph = new BidirectionalMatrixGraph<IEdge<int>>(5);
            TryGetEdge_ImmutableVertices_Test(graph);
        }

        [Test]
        public void GetEdges()
        {
            var graph = new BidirectionalMatrixGraph<IEdge<int>>(5);
            GetEdges_ImmutableVertices_Test(graph);
        }

        [Test]
        public void GetOutEdges()
        {
            var graph = new BidirectionalMatrixGraph<IEdge<int>>(6);
            GetOutEdges_ImmutableVertices_Test(graph);
        }

        [Test]
        public void GetInEdges()
        {
            var graph = new BidirectionalMatrixGraph<IEdge<int>>(6);
            GetInEdges_ImmutableVertices_Test(graph);
        }

        #endregion

        #region Remove Edges

        [Test]
        public void RemoveEdge()
        {
            var graph = new BidirectionalMatrixGraph<IEdge<int>>(4);
            RemoveEdge_ImmutableVertices_Test(graph);
        }

        [Test]
        public void RemoveEdge_EquatableEdge()
        {
            var graph = new BidirectionalMatrixGraph<EquatableEdge<int>>(4);
            RemoveEdge_EquatableEdge_ImmutableVertices_Test(graph);
        }

        [Test]
        public void RemoveEdge_Throws()
        {
            var graph = new BidirectionalMatrixGraph<Edge<int>>(3);
            RemoveEdge_Throws_Test(graph);
        }

        [Test]
        public void RemoveEdgeIf_Throws()
        {
            var graph = new BidirectionalMatrixGraph<Edge<int>>(1);
            Assert.Throws<NotSupportedException>(() => graph.RemoveEdgeIf(_ => true));
        }

        [Test]
        public void RemoveOutEdgeIf()
        {
            var graph = new BidirectionalMatrixGraph<IEdge<int>>(4);
            RemoveOutEdgeIf_ImmutableVertices_Test(graph);
        }

        [Test]
        public void RemoveOutEdgeIf_Throws()
        {
            var graph = new BidirectionalMatrixGraph<Edge<int>>(2);
            RemoveOutEdgeIf_Throws_Test(graph);
        }

        [Test]
        public void RemoveInEdgeIf()
        {
            var graph = new BidirectionalMatrixGraph<IEdge<int>>(4);
            RemoveInEdgeIf_ImmutableVertices_Test(graph);
        }

        [Test]
        public void RemoveInEdgeIf_Throws()
        {
            var graph = new BidirectionalMatrixGraph<Edge<int>>(2);
            RemoveInEdgeIf_Throws_Test(graph);
        }

        #endregion

        #region Clear

        [Test]
        public void Clear()
        {
            int edgesRemoved = 0;

            var graph = new BidirectionalMatrixGraph<IEdge<int>>(3);
            // ReSharper disable once ParameterOnlyUsedForPreconditionCheck.Local
            graph.EdgeRemoved += e =>
            {
                Assert.IsNotNull(e);
                // ReSharper disable once AccessToModifiedClosure
                ++edgesRemoved;
            };

            graph.AssertHasVertices(0, 1, 2 );
            graph.AssertNoEdge();

            graph.Clear();

            graph.AssertHasVertices(0, 1, 2 );
            graph.AssertNoEdge();
            CheckCounter(0);

            graph.AddEdge(Edge.Create(0, 1));
            graph.AddEdge(Edge.Create(1, 2));
            graph.AddEdge(Edge.Create(2, 0));

            graph.Clear();

            graph.AssertHasVertices(0, 1, 2 );
            graph.AssertNoEdge();
            CheckCounter(3);

            graph.AddEdge(Edge.Create(0, 1));
            graph.AddEdge(Edge.Create(2, 1));
            graph.AddEdge(Edge.Create(2, 0));
            graph.AddEdge(Edge.Create(2, 2));

            graph.Clear();

            graph.AssertHasVertices(0, 1, 2 );
            graph.AssertNoEdge();
            CheckCounter(4);

            #region Local function

            void CheckCounter(int expectedEdgesRemoved)
            {
                Assert.AreEqual(expectedEdgesRemoved, edgesRemoved);
                edgesRemoved = 0;
            }

            #endregion
        }

        [Test]
        public void ClearOutEdges()
        {
            int edgesRemoved = 0;

            var graph = new BidirectionalMatrixGraph<IEdge<int>>(3);
            // ReSharper disable once ParameterOnlyUsedForPreconditionCheck.Local
            graph.EdgeRemoved += e =>
            {
                Assert.IsNotNull(e);
                // ReSharper disable once AccessToModifiedClosure
                ++edgesRemoved;
            };

            graph.AssertNoEdge();

            // Clear 6 => not in graph
            graph.ClearOutEdges(6);
            graph.AssertNoEdge();

            // Clear 0 => In graph but no out edges
            graph.ClearOutEdges(0);
            graph.AssertNoEdge();
            CheckCounter(0);

            var edge01 = Edge.Create(0, 1);
            var edge12 = Edge.Create(1, 2);
            graph.AddEdgeRange( edge01, edge12 );

            // Clear out 0
            graph.ClearOutEdges(0);

            graph.AssertHasEdges(edge12 );
            CheckCounter(1);

            var edge02 = Edge.Create(0, 2);
            var edge20 = Edge.Create(2, 0);
            var edge21 = Edge.Create(2, 1);
            graph.AddEdgeRange( edge01, edge02, edge20, edge21 );

            // Clear out 2
            graph.ClearOutEdges(2);

            graph.AssertHasEdges(edge01, edge02, edge12 );
            CheckCounter(2);

            // Clear out 0
            graph.ClearOutEdges(0);

            graph.AssertHasEdges(edge12 );
            CheckCounter(2);

            // Clear out 1 = Clear
            graph.ClearOutEdges(1);

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
        public void ClearInEdges()
        {
            int edgesRemoved = 0;

            var graph = new BidirectionalMatrixGraph<IEdge<int>>(3);
            // ReSharper disable once ParameterOnlyUsedForPreconditionCheck.Local
            graph.EdgeRemoved += e =>
            {
                Assert.IsNotNull(e);
                // ReSharper disable once AccessToModifiedClosure
                ++edgesRemoved;
            };

            graph.AssertNoEdge();

            // Clear 6 => not in graph
            graph.ClearInEdges(6);
            graph.AssertNoEdge();
            CheckCounter(0);

            // Clear 0 => In graph but no in edges
            graph.ClearInEdges(0);
            graph.AssertNoEdge();
            CheckCounter(0);

            var edge01 = Edge.Create(0, 1);
            var edge12 = Edge.Create(1, 2);
            graph.AddEdgeRange( edge01, edge12 );

            // Clear in 1
            graph.ClearInEdges(1);

            graph.AssertHasEdges(edge12 );
            CheckCounter(1);

            var edge02 = Edge.Create(0, 2);
            var edge20 = Edge.Create(2, 0);
            var edge21 = Edge.Create(2, 1);
            graph.AddEdgeRange( edge01, edge02, edge20, edge21 );

            // Clear in 2
            graph.ClearInEdges(2);

            graph.AssertHasEdges(edge01, edge20, edge21 );
            CheckCounter(2);

            // Clear in 0
            graph.ClearInEdges(0);

            graph.AssertHasEdges(edge01, edge21 );
            CheckCounter(1);

            // Clear 1 = Clear
            graph.ClearInEdges(1);

            graph.AssertNoEdge();
            CheckCounter(2);

            #region Local function

            void CheckCounter(int expectedRemovedEdges)
            {
                Assert.AreEqual(expectedRemovedEdges, edgesRemoved);
                edgesRemoved = 0;
            }

            #endregion
        }

        [Test]
        public void ClearEdges()
        {
            int edgesRemoved = 0;

            var graph = new BidirectionalMatrixGraph<IEdge<int>>(3);
            // ReSharper disable once ParameterOnlyUsedForPreconditionCheck.Local
            graph.EdgeRemoved += e =>
            {
                Assert.IsNotNull(e);
                // ReSharper disable once AccessToModifiedClosure
                ++edgesRemoved;
            };

            graph.AssertNoEdge();

            // Clear 6 => not in graph
            graph.ClearEdges(6);
            graph.AssertNoEdge();
            CheckCounter(0);

            // Clear 0 => In graph but not in/out edges
            graph.ClearEdges(0);
            graph.AssertNoEdge();
            CheckCounter(0);

            var edge01 = Edge.Create(0, 1);
            var edge12 = Edge.Create(1, 2);
            graph.AddEdgeRange( edge01, edge12 );

            // Clear 1
            graph.ClearEdges(1);

            graph.AssertNoEdge();
            CheckCounter(2);

            var edge02 = Edge.Create(0, 2);
            var edge20 = Edge.Create(2, 0);
            var edge21 = Edge.Create(2, 1);
            graph.AddEdgeRange( edge01, edge02, edge20, edge21 );

            // Clear 2
            graph.ClearEdges(2);

            graph.AssertHasEdges(edge01 );
            CheckCounter(3);

            // Clear 0 = clear
            graph.ClearEdges(0);

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

        #endregion

        [Test]
        public void Clone()
        {
            var graph = new BidirectionalMatrixGraph<IEdge<int>>(1);
            graph.AssertHasVertices(0 );
            graph.AssertNoEdge();

            var clonedGraph = graph.Clone();
            Assert.IsNotNull(clonedGraph);
            graph.AssertHasVertices(0 );
            graph.AssertNoEdge();

            clonedGraph = (BidirectionalMatrixGraph<IEdge<int>>)((ICloneable)graph).Clone();
            Assert.IsNotNull(clonedGraph);
            graph.AssertHasVertices(0 );
            graph.AssertNoEdge();

            var edge1 = Edge.Create(0, 1);
            var edge2 = Edge.Create(0, 2);
            var edge3 = Edge.Create(1, 2);
            graph = new BidirectionalMatrixGraph<IEdge<int>>(3);
            graph.AddEdgeRange( edge1, edge2, edge3 );
            graph.AssertHasVertices(0, 1, 2 );
            graph.AssertHasEdges(edge1, edge2, edge3 );

            clonedGraph = graph.Clone();
            Assert.IsNotNull(clonedGraph);
            clonedGraph.AssertHasVertices(0, 1, 2 );
            clonedGraph.AssertHasEdges(edge1, edge2, edge3 );

            clonedGraph = (BidirectionalMatrixGraph<IEdge<int>>)((ICloneable)graph).Clone();
            Assert.IsNotNull(clonedGraph);
            clonedGraph.AssertHasVertices(0, 1, 2 );
            clonedGraph.AssertHasEdges(edge1, edge2, edge3 );
        }
    }
}