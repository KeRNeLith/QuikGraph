using System;
using JetBrains.Annotations;
using NUnit.Framework;
using static QuikGraph.Tests.GraphTestHelpers;

namespace QuikGraph.Tests.Structures
{
    internal partial class GraphTestsBase
    {
        #region Remove Vertices

        protected static void RemoveVertex_Test(
            [NotNull] IMutableVertexAndEdgeSet<int, IEdge<int>> graph)
        {
            int verticesRemoved = 0;
            int edgesRemoved = 0;

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

            var edge12 = Edge.Create(1, 2);
            var edge13 = Edge.Create(1, 3);
            var edge14 = Edge.Create(1, 4);
            var edge24 = Edge.Create(2, 4);
            var edge31 = Edge.Create(3, 1);
            var edge33 = Edge.Create(3, 3);
            graph.AddVerticesAndEdgeRange( edge12, edge13, edge14, edge24, edge31, edge33 );

            Assert.IsFalse(graph.RemoveVertex(5));
            CheckCounters(0, 0);

            Assert.IsTrue(graph.RemoveVertex(3));
            CheckCounters(1, 3);
            graph.AssertHasVertices(new[] { 1, 2, 4 });
            graph.AssertHasEdges(new[] { edge12, edge14, edge24 });

            Assert.IsTrue(graph.RemoveVertex(1));
            CheckCounters(1, 2);
            graph.AssertHasVertices(new[] { 2, 4 });
            graph.AssertHasEdges(new[] { edge24 });

            Assert.IsTrue(graph.RemoveVertex(2));
            CheckCounters(1, 1);
            graph.AssertHasVertices(new[] { 4 });
            graph.AssertNoEdge();

            Assert.IsTrue(graph.RemoveVertex(4));
            CheckCounters(1, 0);
            AssertEmptyGraph(graph);

            #region Local function

            void CheckCounters(int expectedRemovedVertices, int expectedRemovedEdges)
            {
                Assert.AreEqual(expectedRemovedVertices, verticesRemoved);
                Assert.AreEqual(expectedRemovedEdges, edgesRemoved);
                verticesRemoved = 0;
                edgesRemoved = 0;
            }

            #endregion
        }

        protected static void RemoveVertex_Clusters_Test(
            [NotNull] ClusteredAdjacencyGraph<int, IEdge<int>> graph)
        {
            var edge12 = Edge.Create(1, 2);
            var edge13 = Edge.Create(1, 3);
            var edge14 = Edge.Create(1, 4);
            var edge24 = Edge.Create(2, 4);
            var edge31 = Edge.Create(3, 1);
            var edge33 = Edge.Create(3, 3);
            graph.AddVerticesAndEdgeRange( edge12, edge13, edge14, edge24, edge31, edge33 );

            Assert.IsFalse(graph.RemoveVertex(5));

            Assert.IsTrue(graph.RemoveVertex(3));
            graph.AssertHasVertices(new[] { 1, 2, 4 });
            graph.AssertHasEdges(new[] { edge12, edge14, edge24 });

            Assert.IsTrue(graph.RemoveVertex(1));
            graph.AssertHasVertices(new[] { 2, 4 });
            graph.AssertHasEdges(new[] { edge24 });

            Assert.IsTrue(graph.RemoveVertex(2));
            graph.AssertHasVertices(new[] { 4 });
            graph.AssertNoEdge();

            Assert.IsTrue(graph.RemoveVertex(4));
            AssertEmptyGraph(graph);


            // With cluster
            ClusteredAdjacencyGraph<int, IEdge<int>> cluster1 = graph.AddCluster();
            ClusteredAdjacencyGraph<int, IEdge<int>> cluster2 = graph.AddCluster();
            ClusteredAdjacencyGraph<int, IEdge<int>> cluster3 = graph.AddCluster();

            cluster1.AddVertexRange( 1, 2 );
            cluster1.AssertHasVertices(new[] { 1, 2 });

            cluster2.AddVertexRange( 1, 2, 4 );
            cluster2.AssertHasVertices(new[] { 1, 2, 4 });

            cluster3.AddVertex(2);
            cluster3.AssertHasVertices(new[] { 2 });

            graph.AddVertexRange( 1, 2, 3, 4 );
            graph.AssertHasVertices(new[] { 1, 2, 3, 4 });


            graph.RemoveVertex(2);
            graph.AssertHasVertices(new[] { 1, 3, 4 });
            cluster1.AssertHasVertices(new[] { 1 });
            cluster2.AssertHasVertices(new[] { 1, 4 });
            AssertNoVertex(cluster3);

            graph.RemoveVertex(1);
            graph.AssertHasVertices(new[] { 3, 4 });
            AssertNoVertex(cluster1);
            cluster2.AssertHasVertices(new[] { 4 });
            AssertNoVertex(cluster3);

            graph.RemoveVertex(3);
            graph.AssertHasVertices(new[] { 4 });
            AssertNoVertex(cluster1);
            cluster2.AssertHasVertices(new[] { 4 });
            AssertNoVertex(cluster3);

            graph.RemoveVertex(4);
            AssertNoVertex(graph);
            AssertNoVertex(cluster1);
            AssertNoVertex(cluster2);
            AssertNoVertex(cluster3);
        }

        protected static void RemoveVertex_Throws_Test<TVertex>(
            [NotNull] IMutableVertexSet<TVertex> graph)
            where TVertex : class
        {
            // ReSharper disable once AssignNullToNotNullAttribute
            Assert.Throws<ArgumentNullException>(() => graph.RemoveVertex(null));
        }

        protected static void RemoveVertex_Throws_Clusters_Test<TVertex, TEdge>(
            [NotNull] ClusteredAdjacencyGraph<TVertex, TEdge> graph)
            where TVertex : class
            where TEdge : IEdge<TVertex>
        {
            // ReSharper disable once AssignNullToNotNullAttribute
            Assert.Throws<ArgumentNullException>(() => graph.RemoveVertex(null));
        }

        protected static void RemoveVertexIf_Test(
            [NotNull] IMutableVertexAndEdgeSet<int, IEdge<int>> graph)
        {
            int verticesRemoved = 0;
            int edgesRemoved = 0;

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

            var edge12 = Edge.Create(1, 2);
            var edge13 = Edge.Create(1, 3);
            var edge14 = Edge.Create(1, 4);
            var edge24 = Edge.Create(2, 4);
            var edge31 = Edge.Create(3, 1);
            var edge33 = Edge.Create(3, 3);
            graph.AddVerticesAndEdgeRange( edge12, edge13, edge14, edge24, edge31, edge33 );

            Assert.AreEqual(0, graph.RemoveVertexIf(vertex => vertex > 10));
            CheckCounters(0, 0);

            Assert.AreEqual(2, graph.RemoveVertexIf(vertex => vertex > 2));
            CheckCounters(2, 5);
            graph.AssertHasVertices(new[] { 1, 2 });
            graph.AssertHasEdges(new[] { edge12 });

            Assert.AreEqual(2, graph.RemoveVertexIf(_ => true));
            CheckCounters(2, 1);
            AssertEmptyGraph(graph);

            #region Local function

            void CheckCounters(int expectedRemovedVertices, int expectedRemovedEdges)
            {
                Assert.AreEqual(expectedRemovedVertices, verticesRemoved);
                Assert.AreEqual(expectedRemovedEdges, edgesRemoved);
                verticesRemoved = 0;
                edgesRemoved = 0;
            }

            #endregion
        }

        protected static void RemoveVertexIf_Test2(
            [NotNull] IMutableVertexAndEdgeSet<int, IEdge<int>> graph)
        {
            int verticesRemoved = 0;
            int edgesRemoved = 0;

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

            var edge11 = Edge.Create(1, 1);
            var edge13 = Edge.Create(1, 3);
            var edge24 = Edge.Create(2, 4);
            var edge31 = Edge.Create(3, 1);
            var edge32 = Edge.Create(3, 2);
            var edge34 = Edge.Create(3, 4);
            graph.AddVerticesAndEdgeRange( edge11, edge13, edge24, edge31, edge32, edge34 );

            Assert.AreEqual(2, graph.RemoveVertexIf(vertex => vertex == 1 || vertex  == 3));
            CheckCounters(2, 5);
            graph.AssertHasVertices(new[] { 2, 4 });
            graph.AssertHasEdges(new[] { edge24 });

            #region Local function

            void CheckCounters(int expectedRemovedVertices, int expectedRemovedEdges)
            {
                Assert.AreEqual(expectedRemovedVertices, verticesRemoved);
                Assert.AreEqual(expectedRemovedEdges, edgesRemoved);
                verticesRemoved = 0;
                edgesRemoved = 0;
            }

            #endregion
        }

        protected static void RemoveVertexIf_Clusters_Test(
            [NotNull] ClusteredAdjacencyGraph<int, IEdge<int>> graph)
        {
            var edge12 = Edge.Create(1, 2);
            var edge13 = Edge.Create(1, 3);
            var edge14 = Edge.Create(1, 4);
            var edge24 = Edge.Create(2, 4);
            var edge31 = Edge.Create(3, 1);
            var edge33 = Edge.Create(3, 3);
            graph.AddVerticesAndEdgeRange( edge12, edge13, edge14, edge24, edge31, edge33 );

            Assert.AreEqual(0, graph.RemoveVertexIf(vertex => vertex > 10));

            Assert.AreEqual(2, graph.RemoveVertexIf(vertex => vertex > 2));
            graph.AssertHasVertices(new[] { 1, 2 });
            graph.AssertHasEdges(new[] { edge12 });

            Assert.AreEqual(2, graph.RemoveVertexIf(_ => true));
            AssertEmptyGraph(graph);
        }

        protected static void RemoveVertexIf_Clusters_Test2(
            [NotNull] ClusteredAdjacencyGraph<int, IEdge<int>> graph)
        {
            var edge11 = Edge.Create(1, 1);
            var edge13 = Edge.Create(1, 3);
            var edge24 = Edge.Create(2, 4);
            var edge31 = Edge.Create(3, 1);
            var edge32 = Edge.Create(3, 2);
            var edge34 = Edge.Create(3, 4);
            graph.AddVerticesAndEdgeRange( edge11, edge13, edge24, edge31, edge32, edge34 );

            Assert.AreEqual(2, graph.RemoveVertexIf(vertex => vertex == 1 || vertex == 3));
            graph.AssertHasVertices(new[] { 2, 4 });
            graph.AssertHasEdges(new[] { edge24 });
        }

        protected static void RemoveVertexIf_Throws_Test<TVertex>(
            [NotNull] IMutableVertexSet<TVertex> graph)
            where TVertex : class
        {
            // ReSharper disable once AssignNullToNotNullAttribute
            Assert.Throws<ArgumentNullException>(() => graph.RemoveVertexIf(null));
        }

        protected static void RemoveVertexIf_Throws_Clusters_Test<TVertex, TEdge>(
            [NotNull] ClusteredAdjacencyGraph<TVertex, TEdge> graph)
            where TVertex : class
            where TEdge : IEdge<TVertex>
        {
            // ReSharper disable once AssignNullToNotNullAttribute
            Assert.Throws<ArgumentNullException>(() => graph.RemoveVertexIf(null));
        }

        #endregion
    }
}