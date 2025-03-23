using System;
using JetBrains.Annotations;
using NUnit.Framework;
using static QuikGraph.Tests.GraphTestHelpers;

namespace QuikGraph.Tests.Structures
{
    internal partial class GraphTestsBase
    {
        #region Add Edges

        protected static void AddEdge_ParallelEdges_Test<TGraph>([NotNull] TGraph graph)
            where TGraph : IMutableVertexSet<int>, IMutableEdgeListGraph<int, IEdge<int>>
        {
            int edgeAdded = 0;

            graph.AddVertex(1);
            graph.AddVertex(2);

            graph.AssertNoEdge();
            // ReSharper disable once ParameterOnlyUsedForPreconditionCheck.Local
            graph.EdgeAdded += e =>
            {
                Assert.IsNotNull(e);
                ++edgeAdded;
            };

            // Edge 1
            var edge1 = Edge.Create(1, 2);
            Assert.IsTrue(graph.AddEdge(edge1));
            Assert.AreEqual(1, edgeAdded);
            graph.AssertHasEdges(edge1 );

            // Edge 2
            var edge2 = Edge.Create(1, 2);
            Assert.IsTrue(graph.AddEdge(edge2));
            Assert.AreEqual(2, edgeAdded);
            graph.AssertHasEdges(edge1, edge2 );

            // Edge 3
            var edge3 = Edge.Create(2, 1);
            Assert.IsTrue(graph.AddEdge(edge3));
            Assert.AreEqual(3, edgeAdded);
            graph.AssertHasEdges(edge1, edge2, edge3 );

            // Edge 1 bis
            Assert.IsTrue(graph.AddEdge(edge1));
            Assert.AreEqual(4, edgeAdded);
            graph.AssertHasEdges(edge1, edge2, edge3, edge1 );

            // Edge 4 self edge
            var edge4 = Edge.Create(2, 2);
            Assert.IsTrue(graph.AddEdge(edge4));
            Assert.AreEqual(5, edgeAdded);
            graph.AssertHasEdges(edge1, edge2, edge3, edge1, edge4 );
        }

        protected static void AddEdge_ParallelEdges_Clusters_Test(
            [NotNull] ClusteredAdjacencyGraph<int, IEdge<int>> graph1,
            [NotNull] ClusteredAdjacencyGraph<int, IEdge<int>> parent2,
            [NotNull] ClusteredAdjacencyGraph<int, IEdge<int>> graph2)
        {
            // Graph without parent
            graph1.AddVertex(1);
            graph1.AddVertex(2);

            graph1.AssertNoEdge();

            // Edge 1
            var edge1 = Edge.Create(1, 2);
            Assert.IsTrue(graph1.AddEdge(edge1));
            graph1.AssertHasEdges(edge1 );

            // Edge 2
            var edge2 = Edge.Create(1, 2);
            Assert.IsTrue(graph1.AddEdge(edge2));
            graph1.AssertHasEdges(edge1, edge2 );

            // Edge 3
            var edge3 = Edge.Create(2, 1);
            Assert.IsTrue(graph1.AddEdge(edge3));
            graph1.AssertHasEdges(edge1, edge2, edge3 );

            // Edge 1 bis
            Assert.IsTrue(graph1.AddEdge(edge1));
            graph1.AssertHasEdges(edge1, edge2, edge3, edge1 );

            // Edge 4 self edge
            var edge4 = Edge.Create(2, 2);
            Assert.IsTrue(graph1.AddEdge(edge4));
            graph1.AssertHasEdges(edge1, edge2, edge3, edge1, edge4 );


            // Graph with parent
            graph2.AddVertex(1);
            graph2.AddVertex(2);

            parent2.AssertNoEdge();
            graph2.AssertNoEdge();

            // Edge 1
            Assert.IsTrue(graph2.AddEdge(edge1));
            parent2.AssertHasEdges(edge1 );
            graph2.AssertHasEdges(edge1 );

            // Edge 2
            Assert.IsTrue(parent2.AddEdge(edge2));
            parent2.AssertHasEdges(edge1, edge2 );
            graph2.AssertHasEdges(edge1 );

            Assert.IsTrue(graph2.AddEdge(edge2));
            parent2.AssertHasEdges(edge1, edge2 );
            graph2.AssertHasEdges(edge1, edge2 );

            // Edge 3
            Assert.IsTrue(graph2.AddEdge(edge3));
            parent2.AssertHasEdges(edge1, edge2, edge3 );
            graph2.AssertHasEdges(edge1, edge2, edge3 );

            // Edge 1 bis
            Assert.IsTrue(graph2.AddEdge(edge1));
            parent2.AssertHasEdges(edge1, edge2, edge3 );
            graph2.AssertHasEdges(edge1, edge2, edge3, edge1 );

            // Edge 4 self edge
            Assert.IsTrue(graph2.AddEdge(edge4));
            parent2.AssertHasEdges(edge1, edge2, edge3, edge4 );
            graph2.AssertHasEdges(edge1, edge2, edge3, edge1, edge4 );
        }

        protected static void AddEdge_ParallelEdges_EquatableEdge_Test<TGraph>([NotNull] TGraph graph)
            where TGraph : IMutableVertexSet<int>, IMutableEdgeListGraph<int, EquatableEdge<int>>
        {
            int edgeAdded = 0;

            graph.AddVertex(1);
            graph.AddVertex(2);

            graph.AssertNoEdge();
            // ReSharper disable once ParameterOnlyUsedForPreconditionCheck.Local
            graph.EdgeAdded += e =>
            {
                Assert.IsNotNull(e);
                ++edgeAdded;
            };

            // Edge 1
            var edge1 = new EquatableEdge<int>(1, 2);
            Assert.IsTrue(graph.AddEdge(edge1));
            Assert.AreEqual(1, edgeAdded);
            graph.AssertHasEdges(edge1 );

            // Edge 2
            var edge2 = new EquatableEdge<int>(1, 2);
            Assert.IsTrue(graph.AddEdge(edge2));
            Assert.AreEqual(2, edgeAdded);
            graph.AssertHasEdges(edge1, edge2 );

            // Edge 3
            var edge3 = new EquatableEdge<int>(2, 1);
            Assert.IsTrue(graph.AddEdge(edge3));
            Assert.AreEqual(3, edgeAdded);
            graph.AssertHasEdges(edge1, edge2, edge3 );

            // Edge 1 bis
            Assert.IsTrue(graph.AddEdge(edge1));
            Assert.AreEqual(4, edgeAdded);
            graph.AssertHasEdges(edge1, edge2, edge3, edge1 );

            // Edge 4 self edge
            var edge4 = new EquatableEdge<int>(2, 2);
            Assert.IsTrue(graph.AddEdge(edge4));
            Assert.AreEqual(5, edgeAdded);
            graph.AssertHasEdges(edge1, edge2, edge3, edge1, edge4 );
        }

        protected static void AddEdge_ParallelEdges_EquatableEdge_Clusters_Test(
            [NotNull] ClusteredAdjacencyGraph<int, EquatableEdge<int>> graph1,
            [NotNull] ClusteredAdjacencyGraph<int, EquatableEdge<int>> parent2,
            [NotNull] ClusteredAdjacencyGraph<int, EquatableEdge<int>> graph2)
        {
            // Graph without parent
            graph1.AddVertex(1);
            graph1.AddVertex(2);

            graph1.AssertNoEdge();

            // Edge 1
            var edge1 = new EquatableEdge<int>(1, 2);
            Assert.IsTrue(graph1.AddEdge(edge1));
            graph1.AssertHasEdges(edge1 );

            // Edge 2
            var edge2 = new EquatableEdge<int>(1, 2);
            Assert.IsTrue(graph1.AddEdge(edge2));
            graph1.AssertHasEdges(edge1, edge2 );

            // Edge 3
            var edge3 = new EquatableEdge<int>(2, 1);
            Assert.IsTrue(graph1.AddEdge(edge3));
            graph1.AssertHasEdges(edge1, edge2, edge3 );

            // Edge 1 bis
            Assert.IsTrue(graph1.AddEdge(edge1));
            graph1.AssertHasEdges(edge1, edge2, edge3, edge1 );

            // Edge 4 self edge
            var edge4 = new EquatableEdge<int>(2, 2);
            Assert.IsTrue(graph1.AddEdge(edge4));
            graph1.AssertHasEdges(edge1, edge2, edge3, edge1, edge4 );


            // Graph with parent
            graph2.AddVertex(1);
            graph2.AddVertex(2);

            parent2.AssertNoEdge();
            graph2.AssertNoEdge();

            // Edge 1
            Assert.IsTrue(graph2.AddEdge(edge1));
            parent2.AssertHasEdges(edge1 );
            graph2.AssertHasEdges(edge1 );

            // Edge 2
            Assert.IsTrue(parent2.AddEdge(edge2));
            parent2.AssertHasEdges(edge1, edge2 );
            graph2.AssertHasEdges(edge1 );

            Assert.IsTrue(graph2.AddEdge(edge2));
            parent2.AssertHasEdges(edge1, edge2 );
            graph2.AssertHasEdges(edge1, edge2 );

            // Edge 3
            Assert.IsTrue(graph2.AddEdge(edge3));
            parent2.AssertHasEdges(edge1, edge2, edge3 );
            graph2.AssertHasEdges(edge1, edge2, edge3 );

            // Edge 1 bis
            Assert.IsTrue(graph2.AddEdge(edge1));
            parent2.AssertHasEdges(edge1, edge2, edge3 );
            graph2.AssertHasEdges(edge1, edge2, edge3, edge1 );

            // Edge 4 self edge
            Assert.IsTrue(graph2.AddEdge(edge4));
            parent2.AssertHasEdges(edge1, edge2, edge3, edge4 );
            graph2.AssertHasEdges(edge1, edge2, edge3, edge1, edge4 );
        }

        protected static void AddEdge_NoParallelEdges_Test<TGraph>([NotNull] TGraph graph)
            where TGraph : IMutableVertexSet<int>, IMutableEdgeListGraph<int, IEdge<int>>
        {
            int edgeAdded = 0;

            graph.AddVertex(1);
            graph.AddVertex(2);

            graph.AssertNoEdge();
            // ReSharper disable once ParameterOnlyUsedForPreconditionCheck.Local
            graph.EdgeAdded += e =>
            {
                Assert.IsNotNull(e);
                ++edgeAdded;
            };

            // Edge 1
            var edge1 = Edge.Create(1, 2);
            Assert.IsTrue(graph.AddEdge(edge1));
            Assert.AreEqual(1, edgeAdded);
            graph.AssertHasEdges(edge1 );

            // Edge 2
            var edge2 = Edge.Create(1, 2);
            Assert.IsFalse(graph.AddEdge(edge2));
            Assert.AreEqual(1, edgeAdded);
            graph.AssertHasEdges(edge1 );

            // Edge 3
            var edge3 = Edge.Create(2, 1);
            Assert.IsTrue(graph.AddEdge(edge3));
            Assert.AreEqual(2, edgeAdded);
            graph.AssertHasEdges(edge1, edge3 );

            // Edge 1 bis
            Assert.IsFalse(graph.AddEdge(edge1));
            Assert.AreEqual(2, edgeAdded);
            graph.AssertHasEdges(edge1, edge3 );

            // Edge 4 self edge
            var edge4 = Edge.Create(2, 2);
            Assert.IsTrue(graph.AddEdge(edge4));
            Assert.AreEqual(3, edgeAdded);
            graph.AssertHasEdges(edge1, edge3, edge4 );
        }

        protected static void AddEdge_NoParallelEdges_UndirectedGraph_Test<TGraph>([NotNull] TGraph graph)
            where TGraph : IMutableVertexSet<int>, IMutableEdgeListGraph<int, IEdge<int>>
        {
            int edgeAdded = 0;

            graph.AddVertex(1);
            graph.AddVertex(2);

            graph.AssertNoEdge();
            // ReSharper disable once ParameterOnlyUsedForPreconditionCheck.Local
            graph.EdgeAdded += e =>
            {
                Assert.IsNotNull(e);
                ++edgeAdded;
            };

            // Edge 1
            var edge1 = Edge.Create(1, 2);
            Assert.IsTrue(graph.AddEdge(edge1));
            Assert.AreEqual(1, edgeAdded);
            graph.AssertHasEdges(edge1 );

            // Edge 2
            var edge2 = Edge.Create(1, 2);
            Assert.IsFalse(graph.AddEdge(edge2));
            Assert.AreEqual(1, edgeAdded);
            graph.AssertHasEdges(edge1 );

            // Edge 3
            var edge3 = Edge.Create(2, 1);
            Assert.IsFalse(graph.AddEdge(edge3));   // Parallel to edge 1
            Assert.AreEqual(1, edgeAdded);
            graph.AssertHasEdges(edge1 );

            // Edge 1 bis
            Assert.IsFalse(graph.AddEdge(edge1));
            Assert.AreEqual(1, edgeAdded);
            graph.AssertHasEdges(edge1 );

            // Edge 4 self edge
            var edge4 = Edge.Create(2, 2);
            Assert.IsTrue(graph.AddEdge(edge4));
            Assert.AreEqual(2, edgeAdded);
            graph.AssertHasEdges(edge1, edge4 );
        }

        protected static void AddEdge_NoParallelEdges_Clusters_Test(
            [NotNull] ClusteredAdjacencyGraph<int, IEdge<int>> graph1,
            [NotNull] ClusteredAdjacencyGraph<int, IEdge<int>> parent2,
            [NotNull] ClusteredAdjacencyGraph<int, IEdge<int>> graph2)
        {
            // Graph without parent
            graph1.AddVertex(1);
            graph1.AddVertex(2);

            graph1.AssertNoEdge();

            // Edge 1
            var edge1 = Edge.Create(1, 2);
            Assert.IsTrue(graph1.AddEdge(edge1));
            graph1.AssertHasEdges(edge1 );

            // Edge 2
            var edge2 = Edge.Create(1, 2);
            Assert.IsFalse(graph1.AddEdge(edge2));
            graph1.AssertHasEdges(edge1 );

            // Edge 3
            var edge3 = Edge.Create(2, 1);
            Assert.IsTrue(graph1.AddEdge(edge3));
            graph1.AssertHasEdges(edge1, edge3 );

            // Edge 1 bis
            Assert.IsFalse(graph1.AddEdge(edge1));
            graph1.AssertHasEdges(edge1, edge3 );

            // Edge 4 self edge
            var edge4 = Edge.Create(2, 2);
            Assert.IsTrue(graph1.AddEdge(edge4));
            graph1.AssertHasEdges(edge1, edge3, edge4 );


            // Graph with parent
            graph2.AddVertex(1);
            graph2.AddVertex(2);

            parent2.AssertNoEdge();
            graph2.AssertNoEdge();

            // Edge 1
            Assert.IsTrue(graph2.AddEdge(edge1));
            parent2.AssertHasEdges(edge1 );
            graph2.AssertHasEdges(edge1 );

            // Edge 2
            Assert.IsFalse(graph2.AddEdge(edge2));
            parent2.AssertHasEdges(edge1 );
            graph2.AssertHasEdges(edge1 );

            // Edge 3
            Assert.IsTrue(parent2.AddEdge(edge3));
            parent2.AssertHasEdges(edge1, edge3 );
            graph2.AssertHasEdges(edge1 );

            Assert.IsTrue(graph2.AddEdge(edge3));
            parent2.AssertHasEdges(edge1, edge3 );
            graph2.AssertHasEdges(edge1, edge3 );

            // Edge 1 bis
            Assert.IsFalse(graph2.AddEdge(edge1));
            parent2.AssertHasEdges(edge1, edge3 );
            graph2.AssertHasEdges(edge1, edge3 );

            // Edge 4 self edge
            Assert.IsTrue(graph2.AddEdge(edge4));
            parent2.AssertHasEdges(edge1, edge3, edge4 );
            graph2.AssertHasEdges(edge1, edge3, edge4 );
        }

        protected static void AddEdge_NoParallelEdges_EquatableEdge_Test<TGraph>([NotNull] TGraph graph)
            where TGraph : IMutableVertexSet<int>, IMutableEdgeListGraph<int, EquatableEdge<int>>
        {
            int edgeAdded = 0;

            graph.AddVertex(1);
            graph.AddVertex(2);

            graph.AssertNoEdge();
            // ReSharper disable once ParameterOnlyUsedForPreconditionCheck.Local
            graph.EdgeAdded += e =>
            {
                Assert.IsNotNull(e);
                ++edgeAdded;
            };

            // Edge 1
            var edge1 = new EquatableEdge<int>(1, 2);
            Assert.IsTrue(graph.AddEdge(edge1));
            Assert.AreEqual(1, edgeAdded);
            graph.AssertHasEdges(edge1 );

            // Edge 2
            var edge2 = new EquatableEdge<int>(1, 2);
            Assert.IsFalse(graph.AddEdge(edge2));
            Assert.AreEqual(1, edgeAdded);
            graph.AssertHasEdges(edge1 );

            // Edge 3
            var edge3 = new EquatableEdge<int>(2, 1);
            Assert.IsTrue(graph.AddEdge(edge3));
            Assert.AreEqual(2, edgeAdded);
            graph.AssertHasEdges(edge1, edge3 );

            // Edge 1 bis
            Assert.IsFalse(graph.AddEdge(edge1));
            Assert.AreEqual(2, edgeAdded);
            graph.AssertHasEdges(edge1, edge3 );

            // Edge 4 self edge
            var edge4 = new EquatableEdge<int>(2, 2);
            Assert.IsTrue(graph.AddEdge(edge4));
            Assert.AreEqual(3, edgeAdded);
            graph.AssertHasEdges(edge1, edge3, edge4 );
        }

        protected static void AddEdge_NoParallelEdges_EquatableEdge_UndirectedGraph_Test<TGraph>([NotNull] TGraph graph)
            where TGraph : IMutableVertexSet<int>, IMutableEdgeListGraph<int, EquatableEdge<int>>
        {
            int edgeAdded = 0;

            graph.AddVertex(1);
            graph.AddVertex(2);

            graph.AssertNoEdge();
            // ReSharper disable once ParameterOnlyUsedForPreconditionCheck.Local
            graph.EdgeAdded += e =>
            {
                Assert.IsNotNull(e);
                ++edgeAdded;
            };

            // Edge 1
            var edge1 = new EquatableEdge<int>(1, 2);
            Assert.IsTrue(graph.AddEdge(edge1));
            Assert.AreEqual(1, edgeAdded);
            graph.AssertHasEdges(edge1 );

            // Edge 2
            var edge2 = new EquatableEdge<int>(1, 2);
            Assert.IsFalse(graph.AddEdge(edge2));
            Assert.AreEqual(1, edgeAdded);
            graph.AssertHasEdges(edge1 );

            // Edge 3
            var edge3 = new EquatableEdge<int>(2, 1);
            Assert.IsFalse(graph.AddEdge(edge3));   // Parallel to edge 1
            Assert.AreEqual(1, edgeAdded);
            graph.AssertHasEdges(edge1 );

            // Edge 1 bis
            Assert.IsFalse(graph.AddEdge(edge1));
            Assert.AreEqual(1, edgeAdded);
            graph.AssertHasEdges(edge1 );

            // Edge 4 self edge
            var edge4 = new EquatableEdge<int>(2, 2);
            Assert.IsTrue(graph.AddEdge(edge4));
            Assert.AreEqual(2, edgeAdded);
            graph.AssertHasEdges(edge1, edge4 );
        }

        protected static void AddEdge_NoParallelEdges_EquatableEdge_Clusters_Test(
            [NotNull] ClusteredAdjacencyGraph<int, EquatableEdge<int>> graph1,
            [NotNull] ClusteredAdjacencyGraph<int, EquatableEdge<int>> parent2,
            [NotNull] ClusteredAdjacencyGraph<int, EquatableEdge<int>> graph2)
        {
            // Graph without parent
            graph1.AddVertex(1);
            graph1.AddVertex(2);

            graph1.AssertNoEdge();

            // Edge 1
            var edge1 = new EquatableEdge<int>(1, 2);
            Assert.IsTrue(graph1.AddEdge(edge1));
            graph1.AssertHasEdges(edge1 );

            // Edge 2
            var edge2 = new EquatableEdge<int>(1, 2);
            Assert.IsFalse(graph1.AddEdge(edge2));
            graph1.AssertHasEdges(edge1 );

            // Edge 3
            var edge3 = new EquatableEdge<int>(2, 1);
            Assert.IsTrue(graph1.AddEdge(edge3));
            graph1.AssertHasEdges(edge1, edge3 );

            // Edge 1 bis
            Assert.IsFalse(graph1.AddEdge(edge1));
            graph1.AssertHasEdges(edge1, edge3 );

            // Edge 4 self edge
            var edge4 = new EquatableEdge<int>(2, 2);
            Assert.IsTrue(graph1.AddEdge(edge4));
            graph1.AssertHasEdges(edge1, edge3, edge4 );


            // Graph with parent
            graph2.AddVertex(1);
            graph2.AddVertex(2);

            parent2.AssertNoEdge();
            graph2.AssertNoEdge();

            // Edge 1
            Assert.IsTrue(graph2.AddEdge(edge1));
            parent2.AssertHasEdges(edge1 );
            graph2.AssertHasEdges(edge1 );

            // Edge 2
            Assert.IsFalse(graph2.AddEdge(edge2));
            parent2.AssertHasEdges(edge1 );
            graph2.AssertHasEdges(edge1 );

            // Edge 3
            Assert.IsTrue(parent2.AddEdge(edge3));
            parent2.AssertHasEdges(edge1, edge3 );
            graph2.AssertHasEdges(edge1 );

            Assert.IsTrue(graph2.AddEdge(edge3));
            parent2.AssertHasEdges(edge1, edge3 );
            graph2.AssertHasEdges(edge1, edge3 );

            // Edge 1 bis
            Assert.IsFalse(graph2.AddEdge(edge1));
            parent2.AssertHasEdges(edge1, edge3 );
            graph2.AssertHasEdges(edge1, edge3 );

            // Edge 4 self edge
            Assert.IsTrue(graph2.AddEdge(edge4));
            parent2.AssertHasEdges(edge1, edge3, edge4 );
            graph2.AssertHasEdges(edge1, edge3, edge4 );
        }

        protected static void AddEdge_ForbiddenParallelEdges_Test(
            [NotNull] BidirectionalMatrixGraph<IEdge<int>> graph)
        {
            int edgeAdded = 0;

            graph.AssertNoEdge();
            // ReSharper disable once ParameterOnlyUsedForPreconditionCheck.Local
            graph.EdgeAdded += e =>
            {
                Assert.IsNotNull(e);
                ++edgeAdded;
            };

            // Edge 1
            var edge1 = Edge.Create(1, 2);
            Assert.IsTrue(graph.AddEdge(edge1));
            Assert.AreEqual(1, edgeAdded);
            graph.AssertHasEdges(edge1 );

            // Edge 2
            var edge2 = Edge.Create(2, 1);
            Assert.IsTrue(graph.AddEdge(edge2));
            Assert.AreEqual(2, edgeAdded);
            graph.AssertHasEdges(edge1, edge2 );

            // Edge 3 self edge
            var edge3 = Edge.Create(2, 2);
            Assert.IsTrue(graph.AddEdge(edge3));
            Assert.AreEqual(3, edgeAdded);
            graph.AssertHasEdges(edge1, edge2, edge3 );
        }

        protected static void AddEdge_EquatableEdge_ForbiddenParallelEdges_Test(
            [NotNull] BidirectionalMatrixGraph<EquatableEdge<int>> graph)
        {
            int edgeAdded = 0;

            graph.AssertNoEdge();
            // ReSharper disable once ParameterOnlyUsedForPreconditionCheck.Local
            graph.EdgeAdded += e =>
            {
                Assert.IsNotNull(e);
                ++edgeAdded;
            };

            // Edge 1
            var edge1 = new EquatableEdge<int>(1, 2);
            Assert.IsTrue(graph.AddEdge(edge1));
            Assert.AreEqual(1, edgeAdded);
            graph.AssertHasEdges(edge1 );

            // Edge 2
            var edge2 = new EquatableEdge<int>(2, 1);
            Assert.IsTrue(graph.AddEdge(edge2));
            Assert.AreEqual(2, edgeAdded);
            graph.AssertHasEdges(edge1, edge2 );

            // Edge 3 self edge
            var edge3 = new EquatableEdge<int>(2, 2);
            Assert.IsTrue(graph.AddEdge(edge3));
            Assert.AreEqual(3, edgeAdded);
            graph.AssertHasEdges(edge1, edge2, edge3 );
        }

        protected static void AddEdge_Throws_EdgesOnly_Test(
            [NotNull] IMutableEdgeListGraph<int, IEdge<int>> graph)
        {
            // ReSharper disable once AssignNullToNotNullAttribute
            Assert.Throws<ArgumentNullException>(() => graph.AddEdge(null));
            graph.AssertNoEdge();
        }

        protected static void AddEdge_Throws_Test<TGraph>([NotNull] TGraph graph)
            where TGraph : IMutableVertexSet<int>, IMutableEdgeListGraph<int, IEdge<int>>
        {
            AddEdge_Throws_EdgesOnly_Test(graph);

            // Both vertices not in graph
            Assert.IsFalse(graph.AddEdge(Edge.Create(0, 1)));
            graph.AssertNoEdge();

            // Source not in graph
            graph.AddVertex(1);
            Assert.IsFalse(graph.AddEdge(Edge.Create(0, 1)));
            graph.AssertNoEdge();

            // Target not in graph
            Assert.IsFalse(graph.AddEdge(Edge.Create(1, 0)));
            graph.AssertNoEdge();
        }

        protected static void AddEdge_Throws_Clusters_Test(
            [NotNull] ClusteredAdjacencyGraph<int, IEdge<int>> graph)
        {
            // ReSharper disable once AssignNullToNotNullAttribute
            Assert.Throws<ArgumentNullException>(() => graph.AddEdge(null));
            graph.AssertNoEdge();

            // Both vertices not in graph
            Assert.IsFalse(graph.AddEdge(Edge.Create(0, 1)));;
            graph.AssertNoEdge();

            // Source not in graph
            graph.AddVertex(1);
            Assert.IsFalse(graph.AddEdge(Edge.Create(0, 1)));
            graph.AssertNoEdge();

            // Target not in graph
            Assert.IsFalse(graph.AddEdge(Edge.Create(1, 0)));
            graph.AssertNoEdge();
        }

        protected static void AddEdgeRange_EdgesOnly_Test(
            [NotNull] IMutableEdgeListGraph<int, IEdge<int>> graph)
        {
            int edgeAdded = 0;

            graph.AssertNoEdge();
            // ReSharper disable once ParameterOnlyUsedForPreconditionCheck.Local
            graph.EdgeAdded += e =>
            {
                Assert.IsNotNull(e);
                ++edgeAdded;
            };

            // Edge 1, 2, 3
            var edge1 = Edge.Create(1, 2);
            var edge2 = Edge.Create(1, 3);
            var edge3 = Edge.Create(2, 3);
            Assert.AreEqual(3, graph.AddEdgeRange( edge1, edge2, edge3 ));
            Assert.AreEqual(3, edgeAdded);
            graph.AssertHasEdges(edge1, edge2, edge3 );

            // Edge 1, 4
            var edge4 = Edge.Create(2, 2);
            Assert.AreEqual(1, graph.AddEdgeRange( edge1, edge4 )); // Showcase the add of only one edge
            Assert.AreEqual(4, edgeAdded);
            graph.AssertHasEdges(edge1, edge2, edge3, edge4 );
        }

        protected static void AddEdgeRange_Test<TGraph>([NotNull] TGraph graph)
            where TGraph : IMutableVertexSet<int>, IMutableEdgeListGraph<int, IEdge<int>>
        {
            graph.AddVertex(1);
            graph.AddVertex(2);
            graph.AddVertex(3);

            AddEdgeRange_EdgesOnly_Test(graph);
        }

        protected static void AddEdgeRange_ForbiddenParallelEdges_Test()
        {
            int edgeAdded = 0;
            var graph = new BidirectionalMatrixGraph<IEdge<int>>(3);

            graph.AssertNoEdge();
            // ReSharper disable once ParameterOnlyUsedForPreconditionCheck.Local
            graph.EdgeAdded += e =>
            {
                Assert.IsNotNull(e);
                ++edgeAdded;
            };

            // Edge 1, 2, 3
            var edge1 = Edge.Create(0, 1);
            var edge2 = Edge.Create(0, 2);
            var edge3 = Edge.Create(1, 2);
            Assert.AreEqual(3, graph.AddEdgeRange( edge1, edge2, edge3 ));
            Assert.AreEqual(3, edgeAdded);
            graph.AssertHasEdges(edge1, edge2, edge3 );

            // Edge 4
            var edge4 = Edge.Create(2, 2);
            Assert.AreEqual(1, graph.AddEdgeRange( edge4 ));
            Assert.AreEqual(4, edgeAdded);
            graph.AssertHasEdges(edge1, edge2, edge3, edge4 );
        }

        protected static void AddEdgeRange_Clusters_Test(
            [NotNull] ClusteredAdjacencyGraph<int, IEdge<int>> graph1,
            [NotNull] ClusteredAdjacencyGraph<int, IEdge<int>> parent2,
            [NotNull] ClusteredAdjacencyGraph<int, IEdge<int>> graph2)
        {
            // Graph without parent
            graph1.AddVertex(1);
            graph1.AddVertex(2);
            graph1.AddVertex(3);

            graph1.AssertNoEdge();

            // Edge 1, 2, 3
            var edge1 = Edge.Create(1, 2);
            var edge2 = Edge.Create(1, 3);
            var edge3 = Edge.Create(2, 3);
            Assert.AreEqual(3, graph1.AddEdgeRange( edge1, edge2, edge3 ));
            graph1.AssertHasEdges(edge1, edge2, edge3 );

            // Edge 1, 4
            var edge4 = Edge.Create(2, 2);
            Assert.AreEqual(1, graph1.AddEdgeRange( edge1, edge4 )); // Showcase the add of only one edge
            graph1.AssertHasEdges(edge1, edge2, edge3, edge4 );


            // Graph with parent
            graph2.AddVertex(1);
            graph2.AddVertex(2);
            graph2.AddVertex(3);

            parent2.AssertNoEdge();
            graph2.AssertNoEdge();

            // Edge 1, 2, 3
            Assert.AreEqual(3, graph2.AddEdgeRange( edge1, edge2, edge3 ));
            parent2.AssertHasEdges(edge1, edge2, edge3 );
            graph2.AssertHasEdges(edge1, edge2, edge3 );

            // Edge 1, 4
            Assert.AreEqual(1, parent2.AddEdgeRange( edge1, edge4 )); // Showcase the add of only one edge
            parent2.AssertHasEdges(edge1, edge2, edge3, edge4 );
            graph2.AssertHasEdges(edge1, edge2, edge3 );

            Assert.AreEqual(1, graph2.AddEdgeRange( edge1, edge4 )); // Showcase the add of only one edge
            parent2.AssertHasEdges(edge1, edge2, edge3, edge4 );
            graph2.AssertHasEdges(edge1, edge2, edge3, edge4 );
        }

        protected static void AddEdgeRange_Throws_EdgesOnly_Test(
            [NotNull] IMutableEdgeListGraph<int, IEdge<int>> graph)
        {
            int edgeAdded = 0;

            graph.AssertNoEdge();
            // ReSharper disable once ParameterOnlyUsedForPreconditionCheck.Local
            graph.EdgeAdded += e =>
            {
                Assert.IsNotNull(e);
                ++edgeAdded;
            };

            // ReSharper disable once AssignNullToNotNullAttribute
            Assert.Throws<ArgumentNullException>(() => graph.AddEdgeRange(null));
            graph.AssertNoEdge();
            Assert.AreEqual(0, edgeAdded);

            // Edge 1, 2, 3
            var edge1 = Edge.Create(1, 2);
            var edge3 = Edge.Create(2, 3);
            Assert.Throws<ArgumentNullException>(() => graph.AddEdgeRange( edge1, null, edge3 ));
            Assert.AreEqual(0, edgeAdded);
            graph.AssertNoEdge();
        }

        protected static void AddEdgeRange_Throws_Test<TGraph>([NotNull] TGraph graph)
            where TGraph : IMutableVertexSet<int>, IMutableEdgeListGraph<int, IEdge<int>>
        {
            graph.AddVertex(1);
            graph.AddVertex(2);
            graph.AddVertex(3);

            AddEdgeRange_Throws_EdgesOnly_Test(graph);
        }

        protected static void AddEdgeRange_Throws_Clusters_Test(
            [NotNull] ClusteredAdjacencyGraph<int, IEdge<int>> graph)
        {
            graph.AddVertex(1);
            graph.AddVertex(2);
            graph.AddVertex(3);

            graph.AssertNoEdge();

            // ReSharper disable once AssignNullToNotNullAttribute
            Assert.Throws<ArgumentNullException>(() => graph.AddEdgeRange(null));
            graph.AssertNoEdge();

            // Edge 1, 2, 3
            var edge1 = Edge.Create(1, 2);
            var edge3 = Edge.Create(2, 3);
            Assert.Throws<ArgumentNullException>(() => graph.AddEdgeRange( edge1, null, edge3 ));
            graph.AssertNoEdge();
        }

        protected static void AddEdgeRange_ForbiddenParallelEdges_Throws_Test(
            [NotNull] BidirectionalMatrixGraph<IEdge<int>> graph)
        {
            int edgeAdded = 0;

            graph.AssertNoEdge();
            // ReSharper disable once ParameterOnlyUsedForPreconditionCheck.Local
            graph.EdgeAdded += e =>
            {
                Assert.IsNotNull(e);
                ++edgeAdded;
            };

            // ReSharper disable once AssignNullToNotNullAttribute
            Assert.Throws<ArgumentNullException>(() => graph.AddEdgeRange(null));
            graph.AssertNoEdge();
            Assert.AreEqual(0, edgeAdded);

            // Edge 1, 2, 3
            var edge1 = Edge.Create(0, 1);
            var edge3 = Edge.Create(1, 2);
            Assert.Throws<ArgumentNullException>(() => graph.AddEdgeRange( edge1, null, edge3 ));
            Assert.AreEqual(0, edgeAdded);
            graph.AssertNoEdge();

            // Edge 1, 3, 4
            var edge4 = Edge.Create(0, 1);
            Assert.Throws<ParallelEdgeNotAllowedException>(() => graph.AddEdgeRange( edge1, edge3, edge4 ));
            Assert.AreEqual(2, edgeAdded);
            graph.AssertHasEdges(edge1, edge3 );

            // Out of range => vertex not found
            Assert.Throws<VertexNotFoundException>(() => graph.AddEdgeRange( Edge.Create(4, 5)));
            Assert.AreEqual(2, edgeAdded);
            graph.AssertHasEdges(edge1, edge3 );
        }


        protected static void AddEdge_ParallelEdges_EdgesOnly_Test(
            [NotNull] EdgeListGraph<int, IEdge<int>> directedGraph,
            [NotNull] EdgeListGraph<int, IEdge<int>> undirectedGraph,
            [NotNull, InstantHandle] Func<
                EdgeListGraph<int, IEdge<int>>,
                IEdge<int>,
                bool> addEdge)
        {
            if (!directedGraph.IsDirected && directedGraph.AllowParallelEdges)
                throw new InvalidOperationException("Graph must be directed and allow parallel edges.");
            if (undirectedGraph.IsDirected && undirectedGraph.AllowParallelEdges)
                throw new InvalidOperationException("Graph must be undirected and allow parallel edges.");

            int directedEdgeAdded = 0;
            int undirectedEdgeAdded = 0;

            directedGraph.AssertNoEdge();
            // ReSharper disable once ParameterOnlyUsedForPreconditionCheck.Local
            directedGraph.EdgeAdded += e =>
            {
                Assert.IsNotNull(e);
                ++directedEdgeAdded;
            };

            undirectedGraph.AssertNoEdge();
            // ReSharper disable once ParameterOnlyUsedForPreconditionCheck.Local
            undirectedGraph.EdgeAdded += e =>
            {
                Assert.IsNotNull(e);
                ++undirectedEdgeAdded;
            };

            // Edge 1
            var edge1 = Edge.Create(1, 2);
            Assert.IsTrue(addEdge(directedGraph, edge1));
            Assert.AreEqual(1, directedEdgeAdded);
            directedGraph.AssertHasEdges(edge1 );

            Assert.IsTrue(addEdge(undirectedGraph, edge1));
            Assert.AreEqual(1, undirectedEdgeAdded);
            undirectedGraph.AssertHasEdges(edge1 );

            // Edge 2
            var edge2 = Edge.Create(1, 2);
            Assert.IsTrue(addEdge(directedGraph, edge2));
            Assert.AreEqual(2, directedEdgeAdded);
            directedGraph.AssertHasEdges(edge1, edge2 );

            Assert.IsTrue(addEdge(undirectedGraph, edge2));
            Assert.AreEqual(2, undirectedEdgeAdded);
            undirectedGraph.AssertHasEdges(edge1, edge2 );

            // Edge 3
            var edge3 = Edge.Create(2, 1);
            Assert.IsTrue(addEdge(directedGraph, edge3));
            Assert.AreEqual(3, directedEdgeAdded);
            directedGraph.AssertHasEdges(edge1, edge2, edge3 );

            Assert.IsTrue(addEdge(undirectedGraph, edge3));
            Assert.AreEqual(3, undirectedEdgeAdded);
            undirectedGraph.AssertHasEdges(edge1, edge2, edge3 );

            // Edge 1 bis
            Assert.IsFalse(addEdge(directedGraph, edge1));
            Assert.AreEqual(3, directedEdgeAdded);
            directedGraph.AssertHasEdges(edge1, edge2, edge3 );

            Assert.IsFalse(addEdge(undirectedGraph, edge1));
            Assert.AreEqual(3, undirectedEdgeAdded);
            undirectedGraph.AssertHasEdges(edge1, edge2, edge3 );

            // Edge 4 self edge
            var edge4 = Edge.Create(2, 2);
            Assert.IsTrue(addEdge(directedGraph, edge4));
            Assert.AreEqual(4, directedEdgeAdded);
            directedGraph.AssertHasEdges(edge1, edge2, edge3, edge4 );

            Assert.IsTrue(addEdge(undirectedGraph, edge4));
            Assert.AreEqual(4, undirectedEdgeAdded);
            undirectedGraph.AssertHasEdges(edge1, edge2, edge3, edge4 );
        }

        protected static void AddEdge_ParallelEdges_EquatableEdge_EdgesOnly_Test(
            [NotNull] EdgeListGraph<int, EquatableEdge<int>> directedGraph,
            [NotNull] EdgeListGraph<int, EquatableEdge<int>> undirectedGraph,
            [NotNull, InstantHandle] Func<
                EdgeListGraph<int, EquatableEdge<int>>,
                EquatableEdge<int>,
                bool> addEdge)
        {
            if (!directedGraph.IsDirected && directedGraph.AllowParallelEdges)
                throw new InvalidOperationException("Graph must be directed and allow parallel edges.");
            if (undirectedGraph.IsDirected && undirectedGraph.AllowParallelEdges)
                throw new InvalidOperationException("Graph must be undirected and allow parallel edges.");

            int directedEdgeAdded = 0;
            int undirectedEdgeAdded = 0;

            directedGraph.AssertNoEdge();
            // ReSharper disable once ParameterOnlyUsedForPreconditionCheck.Local
            directedGraph.EdgeAdded += e =>
            {
                Assert.IsNotNull(e);
                ++directedEdgeAdded;
            };

            undirectedGraph.AssertNoEdge();
            // ReSharper disable once ParameterOnlyUsedForPreconditionCheck.Local
            undirectedGraph.EdgeAdded += e =>
            {
                Assert.IsNotNull(e);
                ++undirectedEdgeAdded;
            };

            // Edge 1
            var edge1 = new EquatableEdge<int>(1, 2);
            Assert.IsTrue(addEdge(directedGraph, edge1));
            Assert.AreEqual(1, directedEdgeAdded);
            directedGraph.AssertHasEdges(edge1 );

            Assert.IsTrue(addEdge(undirectedGraph, edge1));
            Assert.AreEqual(1, undirectedEdgeAdded);
            undirectedGraph.AssertHasEdges(edge1 );

            // Edge 2
            var edge2 = new EquatableEdge<int>(1, 2);
            Assert.IsFalse(addEdge(directedGraph, edge2));
            Assert.AreEqual(1, directedEdgeAdded);
            directedGraph.AssertHasEdges(edge1 );

            Assert.IsFalse(addEdge(undirectedGraph, edge2));
            Assert.AreEqual(1, undirectedEdgeAdded);
            undirectedGraph.AssertHasEdges(edge1 );

            // Edge 3
            var edge3 = new EquatableEdge<int>(2, 1);
            Assert.IsTrue(addEdge(directedGraph, edge3));
            Assert.AreEqual(2, directedEdgeAdded);
            directedGraph.AssertHasEdges(edge1, edge3 );

            Assert.IsTrue(addEdge(undirectedGraph, edge3));
            Assert.AreEqual(2, undirectedEdgeAdded);
            undirectedGraph.AssertHasEdges(edge1, edge3 );

            // Edge 1 bis
            Assert.IsFalse(addEdge(directedGraph, edge1));
            Assert.AreEqual(2, directedEdgeAdded);
            directedGraph.AssertHasEdges(edge1, edge3 );

            Assert.IsFalse(addEdge(undirectedGraph, edge1));
            Assert.AreEqual(2, undirectedEdgeAdded);
            undirectedGraph.AssertHasEdges(edge1, edge3 );

            // Edge 4 self edge
            var edge4 = new EquatableEdge<int>(2, 2);
            Assert.IsTrue(addEdge(directedGraph, edge4));
            Assert.AreEqual(3, directedEdgeAdded);
            directedGraph.AssertHasEdges(edge1, edge3, edge4 );

            Assert.IsTrue(addEdge(undirectedGraph, edge4));
            Assert.AreEqual(3, undirectedEdgeAdded);
            undirectedGraph.AssertHasEdges(edge1, edge3, edge4 );
        }

        protected static void AddEdge_NoParallelEdges_EdgesOnly_Test(
            [NotNull] EdgeListGraph<int, IEdge<int>> directedGraph,
            [NotNull] EdgeListGraph<int, IEdge<int>> undirectedGraph,
            [NotNull, InstantHandle] Func<
                EdgeListGraph<int, IEdge<int>>,
                IEdge<int>,
                bool> addEdge)
        {
            if (!directedGraph.IsDirected && !directedGraph.AllowParallelEdges)
                throw new InvalidOperationException("Graph must be directed and not allow parallel edges.");
            if (undirectedGraph.IsDirected && !undirectedGraph.AllowParallelEdges)
                throw new InvalidOperationException("Graph must be undirected and not allow parallel edges.");

            int directedEdgeAdded = 0;
            int undirectedEdgeAdded = 0;

            directedGraph.AssertNoEdge();
            // ReSharper disable once ParameterOnlyUsedForPreconditionCheck.Local
            directedGraph.EdgeAdded += e =>
            {
                Assert.IsNotNull(e);
                ++directedEdgeAdded;
            };

            undirectedGraph.AssertNoEdge();
            // ReSharper disable once ParameterOnlyUsedForPreconditionCheck.Local
            undirectedGraph.EdgeAdded += e =>
            {
                Assert.IsNotNull(e);
                ++undirectedEdgeAdded;
            };

            // Edge 1
            var edge1 = Edge.Create(1, 2);
            Assert.IsTrue(addEdge(directedGraph, edge1));
            Assert.AreEqual(1, directedEdgeAdded);
            directedGraph.AssertHasEdges(edge1 );

            Assert.IsTrue(addEdge(undirectedGraph, edge1));
            Assert.AreEqual(1, undirectedEdgeAdded);
            undirectedGraph.AssertHasEdges(edge1 );

            // Edge 2
            var edge2 = Edge.Create(1, 2);
            Assert.IsFalse(addEdge(directedGraph, edge2));
            Assert.AreEqual(1, directedEdgeAdded);
            directedGraph.AssertHasEdges(edge1 );

            Assert.IsFalse(addEdge(undirectedGraph, edge2));
            Assert.AreEqual(1, undirectedEdgeAdded);
            undirectedGraph.AssertHasEdges(edge1 );

            // Edge 3
            var edge3 = Edge.Create(2, 1);
            Assert.IsTrue(addEdge(directedGraph, edge3));
            Assert.AreEqual(2, directedEdgeAdded);
            directedGraph.AssertHasEdges(edge1, edge3 );

            Assert.IsFalse(addEdge(undirectedGraph, edge3));
            Assert.AreEqual(1, undirectedEdgeAdded);
            undirectedGraph.AssertHasEdges(edge1 );

            // Edge 1 bis
            Assert.IsFalse(addEdge(directedGraph, edge1));
            Assert.AreEqual(2, directedEdgeAdded);
            directedGraph.AssertHasEdges(edge1, edge3 );

            Assert.IsFalse(addEdge(undirectedGraph, edge1));
            Assert.AreEqual(1, undirectedEdgeAdded);
            undirectedGraph.AssertHasEdges(edge1 );

            // Edge 4 self edge
            var edge4 = Edge.Create(2, 2);
            Assert.IsTrue(addEdge(directedGraph, edge4));
            Assert.AreEqual(3, directedEdgeAdded);
            directedGraph.AssertHasEdges(edge1, edge3, edge4 );

            Assert.IsTrue(addEdge(undirectedGraph, edge4));
            Assert.AreEqual(2, undirectedEdgeAdded);
            undirectedGraph.AssertHasEdges(edge1, edge4 );
        }

        protected static void AddEdge_NoParallelEdges_EquatableEdge_EdgesOnly_Test(
            [NotNull] EdgeListGraph<int, EquatableEdge<int>> directedGraph,
            [NotNull] EdgeListGraph<int, EquatableEdge<int>> undirectedGraph,
            [NotNull, InstantHandle] Func<
                EdgeListGraph<int, EquatableEdge<int>>,
                EquatableEdge<int>,
                bool> addEdge)
        {
            if (!directedGraph.IsDirected && !directedGraph.AllowParallelEdges)
                throw new InvalidOperationException("Graph must be directed and not allow parallel edges.");
            if (undirectedGraph.IsDirected && !undirectedGraph.AllowParallelEdges)
                throw new InvalidOperationException("Graph must be undirected and not allow parallel edges.");

            int directedEdgeAdded = 0;
            int undirectedEdgeAdded = 0;

            directedGraph.AssertNoEdge();
            // ReSharper disable once ParameterOnlyUsedForPreconditionCheck.Local
            directedGraph.EdgeAdded += e =>
            {
                Assert.IsNotNull(e);
                ++directedEdgeAdded;
            };

            undirectedGraph.AssertNoEdge();
            // ReSharper disable once ParameterOnlyUsedForPreconditionCheck.Local
            undirectedGraph.EdgeAdded += e =>
            {
                Assert.IsNotNull(e);
                ++undirectedEdgeAdded;
            };

            // Edge 1
            var edge1 = new EquatableEdge<int>(1, 2);
            Assert.IsTrue(addEdge(directedGraph, edge1));
            Assert.AreEqual(1, directedEdgeAdded);
            directedGraph.AssertHasEdges(edge1 );

            Assert.IsTrue(addEdge(undirectedGraph, edge1));
            Assert.AreEqual(1, undirectedEdgeAdded);
            undirectedGraph.AssertHasEdges(edge1 );

            // Edge 2
            var edge2 = new EquatableEdge<int>(1, 2);
            Assert.IsFalse(addEdge(directedGraph, edge2));
            Assert.AreEqual(1, directedEdgeAdded);
            directedGraph.AssertHasEdges(edge1 );

            Assert.IsFalse(addEdge(undirectedGraph, edge2));
            Assert.AreEqual(1, undirectedEdgeAdded);
            undirectedGraph.AssertHasEdges(edge1 );

            // Edge 3
            var edge3 = new EquatableEdge<int>(2, 1);
            Assert.IsTrue(addEdge(directedGraph, edge3));
            Assert.AreEqual(2, directedEdgeAdded);
            directedGraph.AssertHasEdges(edge1, edge3 );

            Assert.IsFalse(addEdge(undirectedGraph, edge3));
            Assert.AreEqual(1, undirectedEdgeAdded);
            undirectedGraph.AssertHasEdges(edge1 );

            // Edge 1 bis
            Assert.IsFalse(addEdge(directedGraph, edge1));
            Assert.AreEqual(2, directedEdgeAdded);
            directedGraph.AssertHasEdges(edge1, edge3 );

            Assert.IsFalse(addEdge(undirectedGraph, edge1));
            Assert.AreEqual(1, undirectedEdgeAdded);
            undirectedGraph.AssertHasEdges(edge1 );

            // Edge 4 self edge
            var edge4 = new EquatableEdge<int>(2, 2);
            Assert.IsTrue(addEdge(directedGraph, edge4));
            Assert.AreEqual(3, directedEdgeAdded);
            directedGraph.AssertHasEdges(edge1, edge3, edge4 );

            Assert.IsTrue(addEdge(undirectedGraph, edge4));
            Assert.AreEqual(2, undirectedEdgeAdded);
            undirectedGraph.AssertHasEdges(edge1, edge4 );
        }


        protected static void AddEdge_ImmutableGraph_NoUpdate<TGraph>(
            [NotNull] TGraph wrappedGraph,
            [NotNull, InstantHandle] Func<IEdgeSet<int, IEdge<int>>> createGraph)
            where TGraph : IMutableVertexSet<int>, IMutableEdgeListGraph<int, IEdge<int>>
        {
            IEdgeSet<int, IEdge<int>> graph = createGraph();

            var edge = Edge.Create(1, 2);
            wrappedGraph.AddVertex(1);
            wrappedGraph.AddVertex(2);
            wrappedGraph.AddEdge(edge);

            graph.AssertNoEdge();  // Graph is not updated
        }

        protected static void AddEdge_ImmutableGraph_WithUpdate<TGraph>(
            [NotNull] TGraph wrappedGraph,
            [NotNull, InstantHandle] Func<IEdgeSet<int, IEdge<int>>> createGraph)
            where TGraph : IMutableVertexSet<int>, IMutableEdgeListGraph<int, IEdge<int>>
        {
            IEdgeSet<int, IEdge<int>> graph = createGraph();

            var edge = Edge.Create(1, 2);
            wrappedGraph.AddVertex(1);
            wrappedGraph.AddVertex(2);
            wrappedGraph.AddEdge(edge);

            graph.AssertHasEdges(edge );  // Graph is updated
        }

        #endregion
    }
}