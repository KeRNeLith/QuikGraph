using System;
using JetBrains.Annotations;
using NUnit.Framework;
using static QuikGraph.Tests.GraphTestHelpers;

namespace QuikGraph.Tests.Structures
{
    internal partial class GraphTestsBase
    {
        #region Add Vertices & Edges

        protected static void AddVerticesAndEdge_Test(
            [NotNull] IMutableVertexAndEdgeSet<int, IEdge<int>> graph)
        {
            int vertexAdded = 0;
            int edgeAdded = 0;

            AssertEmptyGraph(graph);
            // ReSharper disable once ParameterOnlyUsedForPreconditionCheck.Local
            graph.VertexAdded += v =>
            {
                Assert.IsNotNull(v);
                ++vertexAdded;
            };
            // ReSharper disable once ParameterOnlyUsedForPreconditionCheck.Local
            graph.EdgeAdded += e =>
            {
                Assert.IsNotNull(e);
                ++edgeAdded;
            };

            // Edge 1
            var edge1 = Edge.Create(1, 2);
            Assert.IsTrue(graph.AddVerticesAndEdge(edge1));
            Assert.AreEqual(2, vertexAdded);
            Assert.AreEqual(1, edgeAdded);
            graph.AssertHasVertices(new[] { 1, 2 });
            graph.AssertHasEdges(new[] { edge1 });

            // Edge 2
            var edge2 = Edge.Create(1, 3);
            Assert.IsTrue(graph.AddVerticesAndEdge(edge2));
            Assert.AreEqual(3, vertexAdded);
            Assert.AreEqual(2, edgeAdded);
            graph.AssertHasVertices(new[] { 1, 2, 3 });
            graph.AssertHasEdges(new[] { edge1, edge2 });

            // Edge 3
            var edge3 = Edge.Create(2, 3);
            Assert.IsTrue(graph.AddVerticesAndEdge(edge3));
            Assert.AreEqual(3, vertexAdded);
            Assert.AreEqual(3, edgeAdded);
            graph.AssertHasVertices(new[] { 1, 2, 3 });
            graph.AssertHasEdges(new[] { edge1, edge2, edge3 });
        }

        protected static void AddVerticesAndEdge_Clusters_Test(
            [NotNull] ClusteredAdjacencyGraph<int, IEdge<int>> graph1,
            [NotNull] ClusteredAdjacencyGraph<int, IEdge<int>> parent2,
            [NotNull] ClusteredAdjacencyGraph<int, IEdge<int>> graph2)
        {
            // Graph without parent
            AssertEmptyGraph(graph1);

            // Edge 1
            var edge1 = Edge.Create(1, 2);
            Assert.IsTrue(graph1.AddVerticesAndEdge(edge1));
            graph1.AssertHasVertices(new[] { 1, 2 });
            graph1.AssertHasEdges(new[] { edge1 });

            // Edge 2
            var edge2 = Edge.Create(1, 3);
            Assert.IsTrue(graph1.AddVerticesAndEdge(edge2));
            graph1.AssertHasVertices(new[] { 1, 2, 3 });
            graph1.AssertHasEdges(new[] { edge1, edge2 });

            // Edge 3
            var edge3 = Edge.Create(2, 3);
            Assert.IsTrue(graph1.AddVerticesAndEdge(edge3));
            graph1.AssertHasVertices(new[] { 1, 2, 3 });
            graph1.AssertHasEdges(new[] { edge1, edge2, edge3 });


            // Graph with parent
            AssertEmptyGraph(parent2);
            AssertEmptyGraph(graph2);

            // Edge 1
            Assert.IsTrue(graph2.AddVerticesAndEdge(edge1));
            parent2.AssertHasVertices(new[] { 1, 2 });
            graph2.AssertHasVertices(new[] { 1, 2 });
            parent2.AssertHasEdges(new[] { edge1 });
            graph2.AssertHasEdges(new[] { edge1 });

            // Edge 2
            Assert.IsTrue(parent2.AddVerticesAndEdge(edge2));
            parent2.AssertHasVertices(new[] { 1, 2, 3 });
            graph2.AssertHasVertices(new[] { 1, 2 });
            parent2.AssertHasEdges(new[] { edge1, edge2 });
            graph2.AssertHasEdges(new[] { edge1 });

            Assert.IsTrue(graph2.AddVerticesAndEdge(edge2));
            parent2.AssertHasVertices(new[] { 1, 2, 3 });
            graph2.AssertHasVertices(new[] { 1, 2, 3 });
            parent2.AssertHasEdges(new[] { edge1, edge2 });
            graph2.AssertHasEdges(new[] { edge1, edge2 });

            // Edge 3
            Assert.IsTrue(graph2.AddVerticesAndEdge(edge3));
            parent2.AssertHasVertices(new[] { 1, 2, 3 });
            graph2.AssertHasVertices(new[] { 1, 2, 3 });
            parent2.AssertHasEdges(new[] { edge1, edge2, edge3 });
            graph2.AssertHasEdges(new[] { edge1, edge2, edge3 });
        }

        protected static void AddVerticesAndEdge_Throws_Test<TVertex, TEdge>(
            [NotNull] IMutableVertexAndEdgeSet<TVertex, TEdge> graph)
            where TEdge : IEdge<TVertex>
        {
            // ReSharper disable once AssignNullToNotNullAttribute
            Assert.Throws<ArgumentNullException>(() => graph.AddVerticesAndEdge(default));
            AssertEmptyGraph(graph);
        }

        protected static void AddVerticesAndEdge_Throws_EdgesOnly_Test<TVertex, TEdge>(
            [NotNull] EdgeListGraph<TVertex, TEdge> graph)
            where TEdge : IEdge<TVertex>
        {
            // ReSharper disable once AssignNullToNotNullAttribute
            Assert.Throws<ArgumentNullException>(() => graph.AddVerticesAndEdge(default));
            AssertEmptyGraph(graph);
        }

        protected static void AddVerticesAndEdge_Throws_Clusters_Test<TVertex, TEdge>(
            [NotNull] ClusteredAdjacencyGraph<TVertex, TEdge> graph)
            where TEdge : IEdge<TVertex>
        {
            // ReSharper disable once AssignNullToNotNullAttribute
            Assert.Throws<ArgumentNullException>(() => graph.AddVerticesAndEdge(default));
            AssertEmptyGraph(graph);
        }

        protected static void AddVerticesAndEdgeRange_Test(
            [NotNull] IMutableVertexAndEdgeSet<int, IEdge<int>> graph)
        {
            int vertexAdded = 0;
            int edgeAdded = 0;

            AssertEmptyGraph(graph);
            // ReSharper disable once ParameterOnlyUsedForPreconditionCheck.Local
            graph.VertexAdded += v =>
            {
                Assert.IsNotNull(v);
                ++vertexAdded;
            };
            // ReSharper disable once ParameterOnlyUsedForPreconditionCheck.Local
            graph.EdgeAdded += e =>
            {
                Assert.IsNotNull(e);
                ++edgeAdded;
            };

            // Edge 1, 2
            var edge1 = Edge.Create(1, 2);
            var edge2 = Edge.Create(1, 3);
            Assert.AreEqual(2, graph.AddVerticesAndEdgeRange( edge1, edge2));
            Assert.AreEqual(3, vertexAdded);
            Assert.AreEqual(2, edgeAdded);
            graph.AssertHasVertices(new[] { 1, 2, 3 });
            graph.AssertHasEdges(new[] { edge1, edge2 });

            // Edge 1, 3
            var edge3 = Edge.Create(2, 3);
            Assert.AreEqual(1, graph.AddVerticesAndEdgeRange( edge1, edge3)); // Showcase the add of only one edge
            Assert.AreEqual(3, vertexAdded);
            Assert.AreEqual(3, edgeAdded);
            graph.AssertHasVertices(new[] { 1, 2, 3 });
            graph.AssertHasEdges(new[] { edge1, edge2, edge3 });
        }

        protected static void AddVerticesAndEdgeRange_EdgesOnly_Test(
            [NotNull] EdgeListGraph<int, IEdge<int>> graph)
        {
            int edgeAdded = 0;

            AssertEmptyGraph(graph);
            // ReSharper disable once ParameterOnlyUsedForPreconditionCheck.Local
            graph.EdgeAdded += e =>
            {
                Assert.IsNotNull(e);
                ++edgeAdded;
            };

            // Edge 1, 2
            var edge1 = Edge.Create(1, 2);
            var edge2 = Edge.Create(1, 3);
            Assert.AreEqual(2, graph.AddVerticesAndEdgeRange( edge1, edge2 ));
            Assert.AreEqual(2, edgeAdded);
            graph.AssertHasVertices(new[] { 1, 2, 3 });
            graph.AssertHasEdges(new[] { edge1, edge2 });

            // Edge 1, 3
            var edge3 = Edge.Create(2, 3);
            Assert.AreEqual(1, graph.AddVerticesAndEdgeRange( edge1, edge3 )); // Showcase the add of only one edge
            Assert.AreEqual(3, edgeAdded);
            graph.AssertHasVertices(new[] { 1, 2, 3 });
            graph.AssertHasEdges(new[] { edge1, edge2, edge3 });
        }

        protected static void AddVerticesAndEdgeRange_Clusters_Test(
            [NotNull] ClusteredAdjacencyGraph<int, IEdge<int>> graph1,
            [NotNull] ClusteredAdjacencyGraph<int, IEdge<int>> parent2,
            [NotNull] ClusteredAdjacencyGraph<int, IEdge<int>> graph2)
        {
            // Graph without parent
            AssertEmptyGraph(graph1);

            // Edge 1, 2
            var edge1 = Edge.Create(1, 2);
            var edge2 = Edge.Create(1, 3);
            Assert.AreEqual(2, graph1.AddVerticesAndEdgeRange( edge1, edge2 ));
            graph1.AssertHasVertices(new[] { 1, 2, 3 });
            graph1.AssertHasEdges(new[] { edge1, edge2 });

            // Edge 1, 3
            var edge3 = Edge.Create(2, 3);
            Assert.AreEqual(1, graph1.AddVerticesAndEdgeRange( edge1, edge3 )); // Showcase the add of only one edge
            graph1.AssertHasVertices(new[] { 1, 2, 3 });
            graph1.AssertHasEdges(new[] { edge1, edge2, edge3 });


            // Graph with parent
            AssertEmptyGraph(parent2);
            AssertEmptyGraph(graph2);

            // Edge 1, 2
            Assert.AreEqual(2, graph2.AddVerticesAndEdgeRange( edge1, edge2 ));
            parent2.AssertHasVertices(new[] { 1, 2, 3 });
            graph2.AssertHasVertices(new[] { 1, 2, 3 });
            parent2.AssertHasEdges(new[] { edge1, edge2 });
            graph2.AssertHasEdges(new[] { edge1, edge2 });

            // Edge 1, 3
            Assert.AreEqual(1, parent2.AddVerticesAndEdgeRange( edge1, edge3)); // Showcase the add of only one edge
            parent2.AssertHasVertices(new[] { 1, 2, 3 });
            graph2.AssertHasVertices(new[] { 1, 2, 3 });
            parent2.AssertHasEdges(new[] { edge1, edge2, edge3 });
            graph2.AssertHasEdges(new[] { edge1, edge2 });

            Assert.AreEqual(1, graph2.AddVerticesAndEdgeRange( edge1, edge3)); // Showcase the add of only one edge
            parent2.AssertHasVertices(new[] { 1, 2, 3 });
            graph2.AssertHasVertices(new[] { 1, 2, 3 });
            parent2.AssertHasEdges(new[] { edge1, edge2, edge3 });
            graph2.AssertHasEdges(new[] { edge1, edge2, edge3 });
        }

        protected static void AddVerticesAndEdgeRange_Throws_Test(
            [NotNull] IMutableVertexAndEdgeSet<int, IEdge<int>> graph)
        {
            int vertexAdded = 0;
            int edgeAdded = 0;

            AssertEmptyGraph(graph);
            // ReSharper disable once ParameterOnlyUsedForPreconditionCheck.Local
            graph.VertexAdded += v =>
            {
                Assert.IsNotNull(v);
                ++vertexAdded;
            };
            // ReSharper disable once ParameterOnlyUsedForPreconditionCheck.Local
            graph.EdgeAdded += e =>
            {
                Assert.IsNotNull(e);
                ++edgeAdded;
            };

            // ReSharper disable once AssignNullToNotNullAttribute
            Assert.Throws<ArgumentNullException>(() => graph.AddVerticesAndEdgeRange(null));

            // Edge 1, 2, 3
            var edge1 = Edge.Create(1, 2);
            var edge3 = Edge.Create(1, 3);
            Assert.Throws<ArgumentNullException>(() => graph.AddVerticesAndEdgeRange( edge1, null, edge3 ));
            Assert.AreEqual(0, vertexAdded);
            Assert.AreEqual(0, edgeAdded);
            AssertEmptyGraph(graph);
        }

        protected static void AddVerticesAndEdgeRange_Throws_EdgesOnly_Test(
            [NotNull] EdgeListGraph<int, IEdge<int>> graph)
        {
            int edgeAdded = 0;

            AssertEmptyGraph(graph);
            // ReSharper disable once ParameterOnlyUsedForPreconditionCheck.Local
            graph.EdgeAdded += e =>
            {
                Assert.IsNotNull(e);
                ++edgeAdded;
            };

            // ReSharper disable once AssignNullToNotNullAttribute
            Assert.Throws<ArgumentNullException>(() => graph.AddVerticesAndEdgeRange(null));

            // Edge 1, 2, 3
            var edge1 = Edge.Create(1, 2);
            var edge3 = Edge.Create(1, 3);
            Assert.Throws<ArgumentNullException>(() => graph.AddVerticesAndEdgeRange( edge1, null, edge3 ));
            Assert.AreEqual(0, edgeAdded);
            AssertEmptyGraph(graph);
        }

        protected static void AddVerticesAndEdgeRange_Throws_Clusters_Test(
            [NotNull] ClusteredAdjacencyGraph<int, IEdge<int>> graph)
        {
            AssertEmptyGraph(graph);

            // ReSharper disable once AssignNullToNotNullAttribute
            Assert.Throws<ArgumentNullException>(() => graph.AddVerticesAndEdgeRange(null));

            // Edge 1, 2, 3
            var edge1 = Edge.Create(1, 2);
            var edge3 = Edge.Create(1, 3);
            Assert.Throws<ArgumentNullException>(() => graph.AddVerticesAndEdgeRange( edge1, null, edge3 ));
            AssertEmptyGraph(graph);
        }

        #endregion
    }
}