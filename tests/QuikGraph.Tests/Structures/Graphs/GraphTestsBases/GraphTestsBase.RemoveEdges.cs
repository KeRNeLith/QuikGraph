using System;
using JetBrains.Annotations;
using NUnit.Framework;
using static QuikGraph.Tests.GraphTestHelpers;

namespace QuikGraph.Tests.Structures
{
    internal partial class GraphTestsBase
    {
        protected static void RemoveEdge_Test(
            [NotNull] IMutableVertexAndEdgeSet<int, IEdge<int>> graph)
        {
            int verticesRemoved = 0;
            int edgesRemoved = 0;

            // ReSharper disable once ParameterOnlyUsedForPreconditionCheck.Local
            graph.VertexRemoved += v =>
            {
                Assert.IsNotNull(v);
                ++verticesRemoved;
            };
            // ReSharper disable once ParameterOnlyUsedForPreconditionCheck.Local
            graph.EdgeRemoved += e =>
            {
                Assert.IsNotNull(e);
                // ReSharper disable once AccessToModifiedClosure
                ++edgesRemoved;
            };

            var edge12 = Edge.Create(1, 2);
            var edge13 = Edge.Create(1, 3);
            var edge13Bis = Edge.Create(1, 3);
            var edge14 = Edge.Create(1, 4);
            var edge24 = Edge.Create(2, 4);
            var edge31 = Edge.Create(3, 1);
            var edge33 = Edge.Create(3, 3);
            var edgeNotInGraph = Edge.Create(3, 4);
            var edgeWithVertexNotInGraph1 = Edge.Create(2, 10);
            var edgeWithVertexNotInGraph2 = Edge.Create(10, 2);
            var edgeWithVerticesNotInGraph = Edge.Create(10, 11);
            var edgeNotEquatable = Edge.Create(1, 2);
            graph.AddVerticesAndEdgeRange( edge12, edge13, edge13Bis, edge14, edge24, edge31, edge33 );

            Assert.IsFalse(graph.RemoveEdge(edgeNotInGraph));
            CheckCounters(0);

            Assert.IsFalse(graph.RemoveEdge(edgeWithVertexNotInGraph1));
            CheckCounters(0);

            Assert.IsFalse(graph.RemoveEdge(edgeWithVertexNotInGraph2));
            CheckCounters(0);

            Assert.IsFalse(graph.RemoveEdge(edgeWithVerticesNotInGraph));
            CheckCounters(0);

            Assert.IsFalse(graph.RemoveEdge(edgeNotEquatable));
            CheckCounters(0);

            Assert.IsTrue(graph.RemoveEdge(edge13Bis));
            CheckCounters(1);
            graph.AssertHasVertices(new[] { 1, 2, 3, 4 });
            graph.AssertHasEdges(new[] { edge12, edge13, edge14, edge24, edge31, edge33 });

            Assert.IsTrue(graph.RemoveEdge(edge31));
            CheckCounters(1);
            graph.AssertHasVertices(new[] { 1, 2, 3, 4 });
            graph.AssertHasEdges(new[] { edge12, edge13, edge14, edge24, edge33 });

            Assert.IsTrue(graph.RemoveEdge(edge12));
            Assert.IsTrue(graph.RemoveEdge(edge13));
            Assert.IsTrue(graph.RemoveEdge(edge14));
            Assert.IsTrue(graph.RemoveEdge(edge24));
            Assert.IsTrue(graph.RemoveEdge(edge33));
            CheckCounters(5);
            graph.AssertHasVertices(new[] { 1, 2, 3, 4 });
            graph.AssertNoEdge();

            #region Local function

            void CheckCounters(int expectedRemovedEdges)
            {
                Assert.AreEqual(0, verticesRemoved);
                Assert.AreEqual(expectedRemovedEdges, edgesRemoved);
                edgesRemoved = 0;
            }

            #endregion
        }

        protected static void RemoveEdge_EdgesOnly_Test(
            [NotNull] EdgeListGraph<int, IEdge<int>> graph)
        {
            int edgesRemoved = 0;

            // ReSharper disable once ParameterOnlyUsedForPreconditionCheck.Local
            graph.EdgeRemoved += e =>
            {
                Assert.IsNotNull(e);
                // ReSharper disable once AccessToModifiedClosure
                ++edgesRemoved;
            };

            var edge12 = Edge.Create(1, 2);
            var edge13 = Edge.Create(1, 3);
            var edge13Bis = Edge.Create(1, 3);
            var edge14 = Edge.Create(1, 4);
            var edge24 = Edge.Create(2, 4);
            var edge31 = Edge.Create(3, 1);
            var edge33 = Edge.Create(3, 3);
            var edgeNotInGraph = Edge.Create(3, 4);
            var edgeNotEquatable = Edge.Create(1, 2);
            graph.AddVerticesAndEdgeRange( edge12, edge13, edge13Bis, edge14, edge24, edge31, edge33 );

            Assert.IsFalse(graph.RemoveEdge(edgeNotInGraph));
            CheckCounter(0);

            Assert.IsFalse(graph.RemoveEdge(edgeNotEquatable));
            CheckCounter(0);

            Assert.IsTrue(graph.RemoveEdge(edge13Bis));
            CheckCounter(1);
            graph.AssertHasVertices(new[] { 1, 2, 3, 4 });
            graph.AssertHasEdges(new[] { edge12, edge13, edge14, edge24, edge31, edge33 });

            Assert.IsTrue(graph.RemoveEdge(edge31));
            CheckCounter(1);
            graph.AssertHasVertices(new[] { 1, 2, 3, 4 });
            graph.AssertHasEdges(new[] { edge12, edge13, edge14, edge24, edge33 });

            Assert.IsTrue(graph.RemoveEdge(edge12));
            Assert.IsTrue(graph.RemoveEdge(edge13));
            Assert.IsTrue(graph.RemoveEdge(edge14));
            Assert.IsTrue(graph.RemoveEdge(edge24));
            Assert.IsTrue(graph.RemoveEdge(edge33));
            CheckCounter(5);
            AssertEmptyGraph(graph);    // Vertices removed in the same time as edges

            #region Local function

            void CheckCounter(int expectedRemovedEdges)
            {
                Assert.AreEqual(expectedRemovedEdges, edgesRemoved);
                edgesRemoved = 0;
            }

            #endregion
        }

        protected static void RemoveEdge_ImmutableVertices_Test(
            [NotNull] BidirectionalMatrixGraph<IEdge<int>> graph)
        {
            int edgesRemoved = 0;

            // ReSharper disable once ParameterOnlyUsedForPreconditionCheck.Local
            graph.EdgeRemoved += e =>
            {
                Assert.IsNotNull(e);
                // ReSharper disable once AccessToModifiedClosure
                ++edgesRemoved;
            };

            var edge01 = Edge.Create(0, 1);
            var edge02 = Edge.Create(0, 2);
            var edge03 = Edge.Create(0, 3);
            var edge13 = Edge.Create(1, 3);
            var edge20 = Edge.Create(2, 0);
            var edge22 = Edge.Create(2, 2);
            var edgeNotInGraph = Edge.Create(2, 3);
            var edgeWithVertexNotInGraph1 = Edge.Create(2, 10);
            var edgeWithVertexNotInGraph2 = Edge.Create(10, 2);
            var edgeWithVerticesNotInGraph = Edge.Create(10, 11);
            var edgeNotEquatable = Edge.Create(0, 1);
            graph.AddEdgeRange( edge01, edge02, edge03, edge13, edge20, edge22 );

            Assert.IsFalse(graph.RemoveEdge(edgeNotInGraph));
            CheckCounter(0);

            Assert.IsFalse(graph.RemoveEdge(edgeWithVertexNotInGraph1));
            CheckCounter(0);

            Assert.IsFalse(graph.RemoveEdge(edgeWithVertexNotInGraph2));
            CheckCounter(0);

            Assert.IsFalse(graph.RemoveEdge(edgeWithVerticesNotInGraph));
            CheckCounter(0);

            Assert.IsTrue(graph.RemoveEdge(edgeNotEquatable));
            CheckCounter(1);
            graph.AssertHasVertices(new[] { 0, 1, 2, 3 });
            graph.AssertHasEdges(new[] { edge02, edge03, edge13, edge20, edge22 });

            Assert.IsTrue(graph.RemoveEdge(edge02));
            CheckCounter(1);
            graph.AssertHasVertices(new[] { 0, 1, 2, 3 });
            graph.AssertHasEdges(new[] { edge03, edge13, edge20, edge22 });

            Assert.IsTrue(graph.RemoveEdge(edge20));
            CheckCounter(1);
            graph.AssertHasVertices(new[] { 0, 1, 2, 3 });
            graph.AssertHasEdges(new[] { edge03, edge13, edge22 });

            Assert.IsTrue(graph.RemoveEdge(edge03));
            Assert.IsTrue(graph.RemoveEdge(edge13));
            Assert.IsTrue(graph.RemoveEdge(edge22));
            CheckCounter(3);
            graph.AssertHasVertices(new[] { 0, 1, 2, 3 });
            graph.AssertNoEdge();

            #region Local function

            void CheckCounter(int expectedRemovedEdges)
            {
                Assert.AreEqual(expectedRemovedEdges, edgesRemoved);
                edgesRemoved = 0;
            }

            #endregion
        }

        protected static void RemoveEdge_Clusters_Test(
            [NotNull] ClusteredAdjacencyGraph<int, IEdge<int>> graph)
        {
            var edge12 = Edge.Create(1, 2);
            var edge13 = Edge.Create(1, 3);
            var edge13Bis = Edge.Create(1, 3);
            var edge14 = Edge.Create(1, 4);
            var edge24 = Edge.Create(2, 4);
            var edge31 = Edge.Create(3, 1);
            var edge33 = Edge.Create(3, 3);
            var edgeNotInGraph = Edge.Create(3, 4);
            var edgeWithVertexNotInGraph1 = Edge.Create(2, 10);
            var edgeWithVertexNotInGraph2 = Edge.Create(10, 2);
            var edgeWithVerticesNotInGraph = Edge.Create(10, 11);
            var edgeNotEquatable = Edge.Create(1, 2);
            graph.AddVerticesAndEdgeRange( edge12, edge13, edge13Bis, edge14, edge24, edge31, edge33 );

            Assert.IsFalse(graph.RemoveEdge(edgeNotInGraph));
            Assert.IsFalse(graph.RemoveEdge(edgeWithVertexNotInGraph1));
            Assert.IsFalse(graph.RemoveEdge(edgeWithVertexNotInGraph2));
            Assert.IsFalse(graph.RemoveEdge(edgeWithVerticesNotInGraph));
            Assert.IsFalse(graph.RemoveEdge(edgeNotEquatable));

            Assert.IsTrue(graph.RemoveEdge(edge13Bis));
            graph.AssertHasVertices(new[] { 1, 2, 3, 4 });
            graph.AssertHasEdges(new[] { edge12, edge13, edge14, edge24, edge31, edge33 });

            Assert.IsTrue(graph.RemoveEdge(edge31));
            graph.AssertHasVertices(new[] { 1, 2, 3, 4 });
            graph.AssertHasEdges(new[] { edge12, edge13, edge14, edge24, edge33 });

            Assert.IsTrue(graph.RemoveEdge(edge12));
            Assert.IsTrue(graph.RemoveEdge(edge13));
            Assert.IsTrue(graph.RemoveEdge(edge14));
            Assert.IsTrue(graph.RemoveEdge(edge24));
            Assert.IsTrue(graph.RemoveEdge(edge33));
            graph.AssertHasVertices(new[] { 1, 2, 3, 4 });
            graph.AssertNoEdge();


            // With cluster
            graph.AddVerticesAndEdgeRange( edge12, edge13, edge14, edge24, edge31 );
            graph.AssertHasEdges(new[] { edge12, edge13, edge14, edge24, edge31 });

            ClusteredAdjacencyGraph<int, IEdge<int>> cluster1 = graph.AddCluster();
            ClusteredAdjacencyGraph<int, IEdge<int>> cluster2 = graph.AddCluster();
            ClusteredAdjacencyGraph<int, IEdge<int>> cluster3 = graph.AddCluster();

            cluster1.AddVerticesAndEdgeRange( edge12, edge13 );
            cluster1.AssertHasEdges(new[] { edge12, edge13 });

            cluster2.AddVerticesAndEdgeRange( edge12, edge14, edge24 );
            cluster2.AssertHasEdges(new[] { edge12, edge14, edge24 });

            cluster3.AddVerticesAndEdge(edge12);
            cluster3.AssertHasEdges(new[] { edge12 });


            graph.RemoveEdge(edge12);
            graph.AssertHasEdges(new[] { edge13, edge14, edge24, edge31 });
            cluster1.AssertHasEdges(new[] { edge13 });
            cluster2.AssertHasEdges(new[] { edge14, edge24 });
            cluster3.AssertNoEdge();

            graph.RemoveEdge(edge13);
            graph.AssertHasEdges(new[] { edge14, edge24, edge31 });
            cluster1.AssertNoEdge();
            cluster2.AssertHasEdges(new[] { edge14, edge24 });
            cluster3.AssertNoEdge();

            graph.RemoveEdge(edge24);
            graph.AssertHasEdges(new[] { edge14, edge31 });
            cluster1.AssertNoEdge();
            cluster2.AssertHasEdges(new[] { edge14 });
            cluster3.AssertNoEdge();

            graph.RemoveEdge(edge14);
            graph.AssertHasEdges(new[] { edge31 });
            cluster1.AssertNoEdge();
            cluster2.AssertNoEdge();
            cluster3.AssertNoEdge();
        }

        protected static void RemoveEdge_EquatableEdge_Test(
            [NotNull] IMutableVertexAndEdgeSet<int, EquatableEdge<int>> graph)
        {
            int verticesRemoved = 0;
            int edgesRemoved = 0;

            // ReSharper disable once ParameterOnlyUsedForPreconditionCheck.Local
            graph.VertexRemoved += v =>
            {
                Assert.IsNotNull(v);
                ++verticesRemoved;
            };
            // ReSharper disable once ParameterOnlyUsedForPreconditionCheck.Local
            graph.EdgeRemoved += e =>
            {
                Assert.IsNotNull(e);
                // ReSharper disable once AccessToModifiedClosure
                ++edgesRemoved;
            };

            var edge12 = new EquatableEdge<int>(1, 2);
            var edge13 = new EquatableEdge<int>(1, 3);
            var edge13Bis = new EquatableEdge<int>(1, 3);
            var edge14 = new EquatableEdge<int>(1, 4);
            var edge24 = new EquatableEdge<int>(2, 4);
            var edge31 = new EquatableEdge<int>(3, 1);
            var edge33 = new EquatableEdge<int>(3, 3);
            var edgeNotInGraph = new EquatableEdge<int>(3, 4);
            var edgeWithVertexNotInGraph1 = new EquatableEdge<int>(2, 10);
            var edgeWithVertexNotInGraph2 = new EquatableEdge<int>(10, 2);
            var edgeWithVerticesNotInGraph = new EquatableEdge<int>(10, 11);
            var edgeEquatable = new EquatableEdge<int>(1, 2);
            graph.AddVerticesAndEdgeRange( edge12, edge13, edge13Bis, edge14, edge24, edge31, edge33 );

            Assert.IsFalse(graph.RemoveEdge(edgeNotInGraph));
            CheckCounters(0);

            Assert.IsFalse(graph.RemoveEdge(edgeWithVertexNotInGraph1));
            CheckCounters(0);

            Assert.IsFalse(graph.RemoveEdge(edgeWithVertexNotInGraph2));
            CheckCounters(0);

            Assert.IsFalse(graph.RemoveEdge(edgeWithVerticesNotInGraph));
            CheckCounters(0);

            Assert.IsTrue(graph.RemoveEdge(edgeEquatable));
            CheckCounters(1);
            graph.AssertHasVertices(new[] { 1, 2, 3, 4 });
            graph.AssertHasEdges(new[] { edge13, edge13Bis, edge14, edge24, edge31, edge33 });

            Assert.IsTrue(graph.RemoveEdge(edge13Bis));
            CheckCounters(1);
            graph.AssertHasVertices(new[] { 1, 2, 3, 4 });
            graph.AssertHasEdges(new[] { edge13, edge14, edge24, edge31, edge33 });

            Assert.IsTrue(graph.RemoveEdge(edge31));
            CheckCounters(1);
            graph.AssertHasVertices(new[] { 1, 2, 3, 4 });
            graph.AssertHasEdges(new[] { edge13, edge14, edge24, edge33 });

            Assert.IsTrue(graph.RemoveEdge(edge13));
            Assert.IsTrue(graph.RemoveEdge(edge14));
            Assert.IsTrue(graph.RemoveEdge(edge24));
            Assert.IsTrue(graph.RemoveEdge(edge33));
            CheckCounters(4);
            graph.AssertHasVertices(new[] { 1, 2, 3, 4 });
            graph.AssertNoEdge();

            #region Local function

            void CheckCounters(int expectedRemovedEdges)
            {
                Assert.AreEqual(0, verticesRemoved);
                Assert.AreEqual(expectedRemovedEdges, edgesRemoved);
                edgesRemoved = 0;
            }

            #endregion
        }

        protected static void RemoveEdge_EquatableEdge_EdgesOnly_Test(
            [NotNull] EdgeListGraph<int, EquatableEdge<int>> graph)
        {
            int edgesRemoved = 0;

            // ReSharper disable once ParameterOnlyUsedForPreconditionCheck.Local
            graph.EdgeRemoved += e =>
            {
                Assert.IsNotNull(e);
                // ReSharper disable once AccessToModifiedClosure
                ++edgesRemoved;
            };

            var edge12 = new EquatableEdge<int>(1, 2);
            var edge13 = new EquatableEdge<int>(1, 3);
            var edge14 = new EquatableEdge<int>(1, 4);
            var edge24 = new EquatableEdge<int>(2, 4);
            var edge31 = new EquatableEdge<int>(3, 1);
            var edge33 = new EquatableEdge<int>(3, 3);
            var edgeNotInGraph = new EquatableEdge<int>(3, 4);
            var edgeEquatable = new EquatableEdge<int>(1, 2);
            graph.AddVerticesAndEdgeRange( edge12, edge13, edge14, edge24, edge31, edge33 );

            Assert.IsFalse(graph.RemoveEdge(edgeNotInGraph));
            CheckCounter(0);

            Assert.IsTrue(graph.RemoveEdge(edgeEquatable));
            CheckCounter(1);
            graph.AssertHasVertices(new[] { 1, 2, 3, 4 });
            graph.AssertHasEdges(new[] { edge13, edge14, edge24, edge31, edge33 });

            Assert.IsTrue(graph.RemoveEdge(edge13));
            CheckCounter(1);
            graph.AssertHasVertices(new[] { 1, 2, 3, 4 });
            graph.AssertHasEdges(new[] { edge14, edge24, edge31, edge33 });

            Assert.IsTrue(graph.RemoveEdge(edge31));
            CheckCounter(1);
            graph.AssertHasVertices(new[] { 1, 2, 3, 4 });
            graph.AssertHasEdges(new[] { edge14, edge24, edge33 });

            Assert.IsTrue(graph.RemoveEdge(edge14));
            Assert.IsTrue(graph.RemoveEdge(edge24));
            Assert.IsTrue(graph.RemoveEdge(edge33));
            CheckCounter(3);
            AssertEmptyGraph(graph);    // Vertices removed in the same time as edges

            #region Local function

            void CheckCounter(int expectedRemovedEdges)
            {
                Assert.AreEqual(expectedRemovedEdges, edgesRemoved);
                edgesRemoved = 0;
            }

            #endregion
        }

        protected static void RemoveEdge_EquatableEdge_ImmutableVertices_Test(
            [NotNull] BidirectionalMatrixGraph<EquatableEdge<int>> graph)
        {
            int edgesRemoved = 0;

            // ReSharper disable once ParameterOnlyUsedForPreconditionCheck.Local
            graph.EdgeRemoved += e =>
            {
                Assert.IsNotNull(e);
                // ReSharper disable once AccessToModifiedClosure
                ++edgesRemoved;
            };

            var edge01 = new EquatableEdge<int>(0, 1);
            var edge02 = new EquatableEdge<int>(0, 2);
            var edge03 = new EquatableEdge<int>(0, 3);
            var edge13 = new EquatableEdge<int>(1, 3);
            var edge20 = new EquatableEdge<int>(2, 0);
            var edge22 = new EquatableEdge<int>(2, 2);
            var edgeNotInGraph = new EquatableEdge<int>(2, 3);
            var edgeWithVertexNotInGraph1 = new EquatableEdge<int>(2, 10);
            var edgeWithVertexNotInGraph2 = new EquatableEdge<int>(10, 2);
            var edgeWithVerticesNotInGraph = new EquatableEdge<int>(10, 11);
            var edgeNotEquatable = new EquatableEdge<int>(0, 1);
            graph.AddEdgeRange( edge01, edge02, edge03, edge13, edge20, edge22 );

            Assert.IsFalse(graph.RemoveEdge(edgeNotInGraph));
            CheckCounter(0);

            Assert.IsFalse(graph.RemoveEdge(edgeWithVertexNotInGraph1));
            CheckCounter(0);

            Assert.IsFalse(graph.RemoveEdge(edgeWithVertexNotInGraph2));
            CheckCounter(0);

            Assert.IsFalse(graph.RemoveEdge(edgeWithVerticesNotInGraph));
            CheckCounter(0);

            Assert.IsTrue(graph.RemoveEdge(edgeNotEquatable));
            CheckCounter(1);
            graph.AssertHasVertices(new[] { 0, 1, 2, 3 });
            graph.AssertHasEdges(new[] { edge02, edge03, edge13, edge20, edge22 });

            Assert.IsTrue(graph.RemoveEdge(edge02));
            CheckCounter(1);
            graph.AssertHasVertices(new[] { 0, 1, 2, 3 });
            graph.AssertHasEdges(new[] { edge03, edge13, edge20, edge22 });

            Assert.IsTrue(graph.RemoveEdge(edge20));
            CheckCounter(1);
            graph.AssertHasVertices(new[] { 0, 1, 2, 3 });
            graph.AssertHasEdges(new[] { edge03, edge13, edge22 });

            Assert.IsTrue(graph.RemoveEdge(edge03));
            Assert.IsTrue(graph.RemoveEdge(edge13));
            Assert.IsTrue(graph.RemoveEdge(edge22));
            CheckCounter(3);
            graph.AssertHasVertices(new[] { 0, 1, 2, 3 });
            graph.AssertNoEdge();

            #region Local function

            void CheckCounter(int expectedRemovedEdges)
            {
                Assert.AreEqual(expectedRemovedEdges, edgesRemoved);
                edgesRemoved = 0;
            }

            #endregion
        }

        protected static void RemoveEdge_EquatableEdge_Clusters_Test(
            [NotNull] ClusteredAdjacencyGraph<int, EquatableEdge<int>> graph)
        {
            var edge12 = new EquatableEdge<int>(1, 2);
            var edge13 = new EquatableEdge<int>(1, 3);
            var edge13Bis = new EquatableEdge<int>(1, 3);
            var edge14 = new EquatableEdge<int>(1, 4);
            var edge24 = new EquatableEdge<int>(2, 4);
            var edge31 = new EquatableEdge<int>(3, 1);
            var edge33 = new EquatableEdge<int>(3, 3);
            var edgeNotInGraph = new EquatableEdge<int>(3, 4);
            var edgeWithVertexNotInGraph1 = new EquatableEdge<int>(2, 10);
            var edgeWithVertexNotInGraph2 = new EquatableEdge<int>(10, 2);
            var edgeWithVerticesNotInGraph = new EquatableEdge<int>(10, 11);
            var edgeEquatable = new EquatableEdge<int>(1, 2);
            graph.AddVerticesAndEdgeRange( edge12, edge13, edge13Bis, edge14, edge24, edge31, edge33 );

            Assert.IsFalse(graph.RemoveEdge(edgeNotInGraph));
            Assert.IsFalse(graph.RemoveEdge(edgeWithVertexNotInGraph1));
            Assert.IsFalse(graph.RemoveEdge(edgeWithVertexNotInGraph2));
            Assert.IsFalse(graph.RemoveEdge(edgeWithVerticesNotInGraph));

            Assert.IsTrue(graph.RemoveEdge(edgeEquatable));
            graph.AssertHasVertices(new[] { 1, 2, 3, 4 });
            graph.AssertHasEdges(new[] { edge13, edge13Bis, edge14, edge24, edge31, edge33 });

            Assert.IsTrue(graph.RemoveEdge(edge13Bis));
            graph.AssertHasVertices(new[] { 1, 2, 3, 4 });
            graph.AssertHasEdges(new[] { edge13, edge14, edge24, edge31, edge33 });

            Assert.IsTrue(graph.RemoveEdge(edge31));
            graph.AssertHasVertices(new[] { 1, 2, 3, 4 });
            graph.AssertHasEdges(new[] { edge13, edge14, edge24, edge33 });

            Assert.IsTrue(graph.RemoveEdge(edge13));
            Assert.IsTrue(graph.RemoveEdge(edge14));
            Assert.IsTrue(graph.RemoveEdge(edge24));
            Assert.IsTrue(graph.RemoveEdge(edge33));
            graph.AssertHasVertices(new[] { 1, 2, 3, 4 });
            graph.AssertNoEdge();
        }

        protected static void RemoveEdge_Throws_Test<TVertex, TEdge>(
            [NotNull] IMutableEdgeListGraph<TVertex, TEdge> graph)
            where TEdge : IEdge<TVertex>
        {
            // ReSharper disable once AssignNullToNotNullAttribute
            Assert.Throws<ArgumentNullException>(() => graph.RemoveEdge(default));
        }

        protected static void RemoveEdge_Throws_Clusters_Test<TVertex, TEdge>(
            [NotNull] ClusteredAdjacencyGraph<TVertex, TEdge> graph)
            where TEdge : IEdge<TVertex>
        {
            // ReSharper disable once AssignNullToNotNullAttribute
            Assert.Throws<ArgumentNullException>(() => graph.RemoveEdge(default));
        }

        protected static void RemoveEdgeIf_Test<TGraph>([NotNull] TGraph graph)
            where TGraph : IMutableVertexSet<int>, IMutableEdgeListGraph<int, IEdge<int>>
        {
            int verticesRemoved = 0;
            int edgesRemoved = 0;

            // ReSharper disable once ParameterOnlyUsedForPreconditionCheck.Local
            graph.VertexRemoved += v =>
            {
                Assert.IsNotNull(v);
                ++verticesRemoved;
            };
            // ReSharper disable once ParameterOnlyUsedForPreconditionCheck.Local
            graph.EdgeRemoved += e =>
            {
                Assert.IsNotNull(e);
                // ReSharper disable once AccessToModifiedClosure
                ++edgesRemoved;
            };

            var edge12 = Edge.Create(1, 2);
            var edge13 = Edge.Create(1, 3);
            var edge13Bis = Edge.Create(1, 3);
            var edge14 = Edge.Create(1, 4);
            var edge24 = Edge.Create(2, 4);
            var edge31 = Edge.Create(3, 1);
            var edge33 = Edge.Create(3, 3);
            graph.AddVertexRange( 1, 2, 3, 4 );
            graph.AddEdgeRange( edge12, edge13, edge13Bis, edge14, edge24, edge31, edge33 );

            Assert.AreEqual(0, graph.RemoveEdgeIf(edge => edge.Target == 5));
            CheckCounters(0);

            Assert.AreEqual(2, graph.RemoveEdgeIf(edge => edge.Source == 3));
            CheckCounters(2);
            graph.AssertHasVertices(new[] { 1, 2, 3, 4 });
            graph.AssertHasEdges(new[] { edge12, edge13, edge13Bis, edge14, edge24 });

            Assert.AreEqual(5, graph.RemoveEdgeIf(_ => true));
            CheckCounters(5);
            graph.AssertHasVertices(new[] { 1, 2, 3, 4 });
            graph.AssertNoEdge();

            #region Local function

            void CheckCounters(int expectedRemovedEdges)
            {
                Assert.AreEqual(0, verticesRemoved);
                Assert.AreEqual(expectedRemovedEdges, edgesRemoved);
                edgesRemoved = 0;
            }

            #endregion
        }

        protected static void RemoveEdgeIf_EdgesOnly_Test(
            [NotNull] EdgeListGraph<int, IEdge<int>> graph)
        {
            int edgesRemoved = 0;

            // ReSharper disable once ParameterOnlyUsedForPreconditionCheck.Local
            graph.EdgeRemoved += e =>
            {
                Assert.IsNotNull(e);
                // ReSharper disable once AccessToModifiedClosure
                ++edgesRemoved;
            };

            var edge12 = Edge.Create(1, 2);
            var edge13 = Edge.Create(1, 3);
            var edge13Bis = Edge.Create(1, 3);
            var edge14 = Edge.Create(1, 4);
            var edge24 = Edge.Create(2, 4);
            var edge31 = Edge.Create(3, 1);
            var edge33 = Edge.Create(3, 3);
            graph.AddEdgeRange( edge12, edge13, edge13Bis, edge14, edge24, edge31, edge33 );

            Assert.AreEqual(0, graph.RemoveEdgeIf(edge => edge.Target == 5));
            CheckCounter(0);

            Assert.AreEqual(2, graph.RemoveEdgeIf(edge => edge.Source == 3));
            CheckCounter(2);
            graph.AssertHasVertices(new[] { 1, 2, 3, 4 });
            graph.AssertHasEdges(new[] { edge12, edge13, edge13Bis, edge14, edge24 });

            Assert.AreEqual(5, graph.RemoveEdgeIf(_ => true));
            CheckCounter(5);
            AssertEmptyGraph(graph);    // Vertices removed in the same time as edges

            #region Local function

            void CheckCounter(int expectedRemovedEdges)
            {
                Assert.AreEqual(expectedRemovedEdges, edgesRemoved);
                edgesRemoved = 0;
            }

            #endregion
        }

        protected static void RemoveEdgeIf_Clusters_Test(
            [NotNull] ClusteredAdjacencyGraph<int, IEdge<int>> graph)
        {
            var edge12 = Edge.Create(1, 2);
            var edge13 = Edge.Create(1, 3);
            var edge13Bis = Edge.Create(1, 3);
            var edge14 = Edge.Create(1, 4);
            var edge24 = Edge.Create(2, 4);
            var edge31 = Edge.Create(3, 1);
            var edge33 = Edge.Create(3, 3);
            graph.AddVertexRange( 1, 2, 3, 4 );
            graph.AddEdgeRange( edge12, edge13, edge13Bis, edge14, edge24, edge31, edge33 );

            Assert.AreEqual(0, graph.RemoveEdgeIf(edge => edge.Target == 5));

            Assert.AreEqual(2, graph.RemoveEdgeIf(edge => edge.Source == 3));
            graph.AssertHasVertices(new[] { 1, 2, 3, 4 });
            graph.AssertHasEdges(new[] { edge12, edge13, edge13Bis, edge14, edge24 });

            Assert.AreEqual(5, graph.RemoveEdgeIf(_ => true));
            graph.AssertHasVertices(new[] { 1, 2, 3, 4 });
            graph.AssertNoEdge();
        }

        protected static void RemoveEdgeIf_Throws_Test<TVertex, TEdge>(
            [NotNull] IMutableEdgeListGraph<TVertex, TEdge> graph)
            where TEdge : IEdge<TVertex>
        {
            // ReSharper disable once AssignNullToNotNullAttribute
            Assert.Throws<ArgumentNullException>(() => graph.RemoveEdgeIf(null));
        }

        protected static void RemoveEdgeIf_Throws_Clusters_Test<TVertex, TEdge>(
            [NotNull] ClusteredAdjacencyGraph<TVertex, TEdge> graph)
            where TEdge : IEdge<TVertex>
        {
            // ReSharper disable once AssignNullToNotNullAttribute
            Assert.Throws<ArgumentNullException>(() => graph.RemoveEdgeIf(null));
        }

        protected static void RemoveOutEdgeIf_Test(
            [NotNull] IMutableVertexAndEdgeListGraph<int, IEdge<int>> graph)
        {
            int verticesRemoved = 0;
            int edgesRemoved = 0;

            // ReSharper disable once ParameterOnlyUsedForPreconditionCheck.Local
            graph.VertexRemoved += v =>
            {
                Assert.IsNotNull(v);
                ++verticesRemoved;
            };
            // ReSharper disable once ParameterOnlyUsedForPreconditionCheck.Local
            graph.EdgeRemoved += e =>
            {
                Assert.IsNotNull(e);
                // ReSharper disable once AccessToModifiedClosure
                ++edgesRemoved;
            };

            Assert.AreEqual(0, graph.RemoveOutEdgeIf(1, _ => true));
            CheckCounters(0);
            AssertEmptyGraph(graph);

            var edge12 = Edge.Create(1, 2);
            var edge13 = Edge.Create(1, 3);
            var edge13Bis = Edge.Create(1, 3);
            var edge14 = Edge.Create(1, 4);
            var edge24 = Edge.Create(2, 4);
            var edge31 = Edge.Create(3, 1);
            var edge33 = Edge.Create(3, 3);
            graph.AddVerticesAndEdgeRange( edge12, edge13, edge13Bis, edge14, edge24, edge31, edge33 );

            Assert.AreEqual(3, graph.RemoveOutEdgeIf(1, edge => edge.Target >= 3));
            CheckCounters(3);
            graph.AssertHasVertices(new[] { 1, 2, 3, 4 });
            graph.AssertHasEdges(new[] { edge12, edge24, edge31, edge33 });

            Assert.AreEqual(0, graph.RemoveOutEdgeIf(3, edge => edge.Target > 5));
            CheckCounters(0);
            graph.AssertHasVertices(new[] { 1, 2, 3, 4 });
            graph.AssertHasEdges(new[] { edge12, edge24, edge31, edge33 });

            Assert.AreEqual(2, graph.RemoveOutEdgeIf(3, _ => true));
            CheckCounters(2);
            graph.AssertHasVertices(new[] { 1, 2, 3, 4 });
            graph.AssertHasEdges(new[] { edge12, edge24 });

            #region Local function

            void CheckCounters(int expectedRemovedEdges)
            {
                Assert.AreEqual(0, verticesRemoved);
                Assert.AreEqual(expectedRemovedEdges, edgesRemoved);
                edgesRemoved = 0;
            }

            #endregion
        }

        protected static void RemoveOutEdgeIf_ImmutableVertices_Test(
            [NotNull] BidirectionalMatrixGraph<IEdge<int>> graph)
        {
            int edgesRemoved = 0;

            // ReSharper disable once ParameterOnlyUsedForPreconditionCheck.Local
            graph.EdgeRemoved += e =>
            {
                Assert.IsNotNull(e);
                // ReSharper disable once AccessToModifiedClosure
                ++edgesRemoved;
            };

            Assert.AreEqual(0, graph.RemoveOutEdgeIf(6, _ => true));
            CheckCounter(0);
            graph.AssertNoEdge();

            var edge01 = Edge.Create(0, 1);
            var edge02 = Edge.Create(0, 2);
            var edge03 = Edge.Create(0, 3);
            var edge13 = Edge.Create(1, 3);
            var edge20 = Edge.Create(2, 0);
            var edge22 = Edge.Create(2, 2);
            graph.AddEdgeRange( edge01, edge02, edge03, edge13, edge20, edge22 );

            Assert.AreEqual(2, graph.RemoveOutEdgeIf(0, edge => edge.Target >= 2));
            CheckCounter(2);
            graph.AssertHasEdges(new[] { edge01, edge13, edge20, edge22 });

            Assert.AreEqual(0, graph.RemoveOutEdgeIf(2, edge => edge.Target > 4));
            CheckCounter(0);
            graph.AssertHasEdges(new[] { edge01, edge13, edge20, edge22 });

            Assert.AreEqual(2, graph.RemoveOutEdgeIf(2, _ => true));
            CheckCounter(2);
            graph.AssertHasEdges(new[] { edge01, edge13 });

            #region Local function

            void CheckCounter(int expectedRemovedEdges)
            {
                Assert.AreEqual(expectedRemovedEdges, edgesRemoved);
                edgesRemoved = 0;
            }

            #endregion
        }

        protected static void RemoveOutEdgeIf_Clusters_Test(
            [NotNull] ClusteredAdjacencyGraph<int, IEdge<int>> graph)
        {
            Assert.AreEqual(0, graph.RemoveOutEdgeIf(1, _ => true));
            AssertEmptyGraph(graph);

            var edge12 = Edge.Create(1, 2);
            var edge13 = Edge.Create(1, 3);
            var edge13Bis = Edge.Create(1, 3);
            var edge14 = Edge.Create(1, 4);
            var edge24 = Edge.Create(2, 4);
            var edge31 = Edge.Create(3, 1);
            var edge33 = Edge.Create(3, 3);
            graph.AddVerticesAndEdgeRange( edge12, edge13, edge13Bis, edge14, edge24, edge31, edge33 );

            Assert.AreEqual(3, graph.RemoveOutEdgeIf(1, edge => edge.Target >= 3));
            graph.AssertHasVertices(new[] { 1, 2, 3, 4 });
            graph.AssertHasEdges(new[] { edge12, edge24, edge31, edge33 });

            Assert.AreEqual(0, graph.RemoveOutEdgeIf(3, edge => edge.Target > 5));
            graph.AssertHasVertices(new[] { 1, 2, 3, 4 });
            graph.AssertHasEdges(new[] { edge12, edge24, edge31, edge33 });

            Assert.AreEqual(2, graph.RemoveOutEdgeIf(3, _ => true));
            graph.AssertHasVertices(new[] { 1, 2, 3, 4 });
            graph.AssertHasEdges(new[] { edge12, edge24 });
        }

        protected static void RemoveOutEdgeIf_Throws_Test<TEdge>(
            [NotNull] BidirectionalMatrixGraph<TEdge> graph)
            where TEdge : IEdge<int>
        {
            // ReSharper disable once AssignNullToNotNullAttribute
            Assert.Throws<ArgumentNullException>(() => graph.RemoveOutEdgeIf(default, null));
        }

        protected static void RemoveOutEdgeIf_Throws_Test<TVertex, TEdge>(
            [NotNull] IMutableIncidenceGraph<TVertex, TEdge> graph)
            where TVertex : class, new()
            where TEdge : IEdge<TVertex>
        {
            // ReSharper disable AssignNullToNotNullAttribute
            Assert.Throws<ArgumentNullException>(() => graph.RemoveOutEdgeIf(null, _ => true));
            Assert.Throws<ArgumentNullException>(() => graph.RemoveOutEdgeIf(new TVertex(), null));
            Assert.Throws<ArgumentNullException>(() => graph.RemoveOutEdgeIf(null, null));
            // ReSharper restore AssignNullToNotNullAttribute
        }

        protected static void RemoveOutEdgeIf_Throws_Test<TVertex, TEdge>(
            [NotNull] ClusteredAdjacencyGraph<TVertex, TEdge> graph)
            where TVertex : class, new()
            where TEdge : IEdge<TVertex>
        {
            // ReSharper disable AssignNullToNotNullAttribute
            Assert.Throws<ArgumentNullException>(() => graph.RemoveOutEdgeIf(null, _ => true));
            Assert.Throws<ArgumentNullException>(() => graph.RemoveOutEdgeIf(new TVertex(), null));
            Assert.Throws<ArgumentNullException>(() => graph.RemoveOutEdgeIf(null, null));
            // ReSharper restore AssignNullToNotNullAttribute
        }

        protected static void RemoveInEdgeIf_Test(
            [NotNull] IMutableBidirectionalGraph<int, IEdge<int>> graph)
        {
            int verticesRemoved = 0;
            int edgesRemoved = 0;

            // ReSharper disable once ParameterOnlyUsedForPreconditionCheck.Local
            graph.VertexRemoved += v =>
            {
                Assert.IsNotNull(v);
                ++verticesRemoved;
            };
            // ReSharper disable once ParameterOnlyUsedForPreconditionCheck.Local
            graph.EdgeRemoved += e =>
            {
                Assert.IsNotNull(e);
                // ReSharper disable once AccessToModifiedClosure
                ++edgesRemoved;
            };

            Assert.AreEqual(0, graph.RemoveInEdgeIf(1, _ => true));
            CheckCounters(0);
            AssertEmptyGraph(graph);

            var edge12 = Edge.Create(1, 2);
            var edge13 = Edge.Create(1, 3);
            var edge13Bis = Edge.Create(1, 3);
            var edge14 = Edge.Create(1, 4);
            var edge24 = Edge.Create(2, 4);
            var edge31 = Edge.Create(3, 1);
            var edge33 = Edge.Create(3, 3);
            graph.AddVerticesAndEdgeRange( edge12, edge13, edge13Bis, edge14, edge24, edge31, edge33 );

            Assert.AreEqual(2, graph.RemoveInEdgeIf(3, edge => edge.Source == 1));
            CheckCounters(2);
            graph.AssertHasVertices(new[] { 1, 2, 3, 4 });
            graph.AssertHasEdges(new[] { edge12, edge14, edge24, edge31, edge33 });

            Assert.AreEqual(0, graph.RemoveInEdgeIf(3, edge => edge.Target > 5));
            CheckCounters(0);
            graph.AssertHasVertices(new[] { 1, 2, 3, 4 });
            graph.AssertHasEdges(new[] { edge12, edge14, edge24, edge31, edge33 });

            Assert.AreEqual(1, graph.RemoveInEdgeIf(2, _ => true));
            CheckCounters(1);
            graph.AssertHasVertices(new[] { 1, 2, 3, 4 });
            graph.AssertHasEdges(new[] { edge14, edge24, edge31, edge33 });

            #region Local function

            void CheckCounters(int expectedRemovedEdges)
            {
                Assert.AreEqual(0, verticesRemoved);
                Assert.AreEqual(expectedRemovedEdges, edgesRemoved);
                edgesRemoved = 0;
            }

            #endregion
        }

        protected static void RemoveInEdgeIf_ImmutableVertices_Test(
            [NotNull] BidirectionalMatrixGraph<IEdge<int>> graph)
        {
            int edgesRemoved = 0;

            // ReSharper disable once ParameterOnlyUsedForPreconditionCheck.Local
            graph.EdgeRemoved += e =>
            {
                Assert.IsNotNull(e);
                // ReSharper disable once AccessToModifiedClosure
                ++edgesRemoved;
            };

            Assert.AreEqual(0, graph.RemoveInEdgeIf(6, _ => true));
            CheckCounter(0);
            graph.AssertNoEdge();

            var edge01 = Edge.Create(0, 1);
            var edge02 = Edge.Create(0, 2);
            var edge03 = Edge.Create(0, 3);
            var edge13 = Edge.Create(1, 3);
            var edge20 = Edge.Create(2, 0);
            var edge22 = Edge.Create(2, 2);
            graph.AddEdgeRange( edge01, edge02, edge03, edge13, edge20, edge22 );

            Assert.AreEqual(1, graph.RemoveInEdgeIf(2, edge => edge.Source == 0));
            CheckCounter(1);
            graph.AssertHasEdges(new[] { edge01, edge03, edge13, edge20, edge22 });

            Assert.AreEqual(0, graph.RemoveInEdgeIf(2, edge => edge.Target > 4));
            CheckCounter(0);
            graph.AssertHasEdges(new[] { edge01, edge03, edge13, edge20, edge22 });

            Assert.AreEqual(1, graph.RemoveInEdgeIf(1, _ => true));
            CheckCounter(1);
            graph.AssertHasEdges(new[] { edge03, edge13, edge20, edge22 });

            #region Local function

            void CheckCounter(int expectedRemovedEdges)
            {
                Assert.AreEqual(expectedRemovedEdges, edgesRemoved);
                edgesRemoved = 0;
            }

            #endregion
        }

        protected static void RemoveInEdgeIf_Throws_Test<TEdge>(
            [NotNull] BidirectionalMatrixGraph<TEdge> graph)
            where TEdge : IEdge<int>
        {
            // ReSharper disable once AssignNullToNotNullAttribute
            Assert.Throws<ArgumentNullException>(() => graph.RemoveInEdgeIf(default, null));
        }

        protected static void RemoveInEdgeIf_Throws_Test(
            [NotNull] IMutableBidirectionalGraph<TestVertex, IEdge<TestVertex>> graph)
        {
            // ReSharper disable AssignNullToNotNullAttribute
            Assert.Throws<ArgumentNullException>(() => graph.RemoveInEdgeIf(null, _ => true));
            Assert.Throws<ArgumentNullException>(() => graph.RemoveInEdgeIf(new TestVertex("v1"), null));
            Assert.Throws<ArgumentNullException>(() => graph.RemoveInEdgeIf(null, null));
            // ReSharper restore AssignNullToNotNullAttribute
        }

        protected static void RemoveAdjacentEdgeIf_Test(
            [NotNull] IMutableUndirectedGraph<int, IEdge<int>> graph)
        {
            int verticesRemoved = 0;
            int edgesRemoved = 0;

            // ReSharper disable once ParameterOnlyUsedForPreconditionCheck.Local
            graph.VertexRemoved += v =>
            {
                Assert.IsNotNull(v);
                ++verticesRemoved;
            };
            // ReSharper disable once ParameterOnlyUsedForPreconditionCheck.Local
            graph.EdgeRemoved += e =>
            {
                Assert.IsNotNull(e);
                // ReSharper disable once AccessToModifiedClosure
                ++edgesRemoved;
            };

            Assert.AreEqual(0, graph.RemoveAdjacentEdgeIf(1, _ => true));
            CheckCounters(0);
            AssertEmptyGraph(graph);

            var edge12 = Edge.Create(1, 2);
            var edge13 = Edge.Create(1, 3);
            var edge13Bis = Edge.Create(1, 3);
            var edge14 = Edge.Create(1, 4);
            var edge24 = Edge.Create(2, 4);
            var edge31 = Edge.Create(3, 1);
            var edge33 = Edge.Create(3, 3);
            graph.AddVerticesAndEdgeRange( edge12, edge13, edge13Bis, edge14, edge24, edge31, edge33 );

            Assert.AreEqual(4, graph.RemoveAdjacentEdgeIf(1, edge => edge.Source >= 3 || edge.Target >= 3));
            CheckCounters(4);
            graph.AssertHasVertices(new[] { 1, 2, 3, 4 });
            graph.AssertHasEdges(new[] { edge12, edge24, edge33 });

            Assert.AreEqual(0, graph.RemoveAdjacentEdgeIf(3, edge => edge.Target > 5));
            CheckCounters(0);
            graph.AssertHasVertices(new[] { 1, 2, 3, 4 });
            graph.AssertHasEdges(new[] { edge12, edge24, edge33 });

            Assert.AreEqual(1, graph.RemoveAdjacentEdgeIf(3, _ => true));
            CheckCounters(1);
            graph.AssertHasVertices(new[] { 1, 2, 3, 4 });
            graph.AssertHasEdges(new[] { edge12, edge24 });

            #region Local function

            void CheckCounters(int expectedRemovedEdges)
            {
                Assert.AreEqual(0, verticesRemoved);
                Assert.AreEqual(expectedRemovedEdges, edgesRemoved);
                edgesRemoved = 0;
            }

            #endregion
        }

        protected static void RemoveAdjacentEdgeIf_Throws_Test(
            [NotNull] IMutableUndirectedGraph<TestVertex, IEdge<TestVertex>> graph)
        {
            // ReSharper disable AssignNullToNotNullAttribute
            Assert.Throws<ArgumentNullException>(() => graph.RemoveAdjacentEdgeIf(null, _ => true));
            Assert.Throws<ArgumentNullException>(() => graph.RemoveAdjacentEdgeIf(new TestVertex("v1"), null));
            Assert.Throws<ArgumentNullException>(() => graph.RemoveAdjacentEdgeIf(null, null));
            // ReSharper restore AssignNullToNotNullAttribute
        }
    }
}