using System;
using JetBrains.Annotations;
using NUnit.Framework;

namespace QuikGraph.Tests.Structures
{
    internal partial class GraphTestsBase
    {
        protected static void Degree_Test(
            [NotNull] IMutableBidirectionalGraph<int, IEdge<int>> graph)
        {
            var edge1 = Edge.Create(1, 2);
            var edge2 = Edge.Create(1, 3);
            var edge3 = Edge.Create(1, 4);
            var edge4 = Edge.Create(2, 4);
            var edge5 = Edge.Create(3, 2);
            var edge6 = Edge.Create(3, 3);

            graph.AddVerticesAndEdgeRange( edge1, edge2, edge3, edge4, edge5, edge6 );
            graph.AddVertex(5);

            Assert.AreEqual(3, graph.Degree(1));
            Assert.AreEqual(3, graph.Degree(2));
            Assert.AreEqual(4, graph.Degree(3)); // Self edge
            Assert.AreEqual(2, graph.Degree(4));
            Assert.AreEqual(0, graph.Degree(5));
        }

        protected static void Degree_ImmutableGraph_Test(
            [NotNull] IMutableVertexAndEdgeSet<int, IEdge<int>> wrappedGraph,
            [NotNull, InstantHandle] Func<IBidirectionalIncidenceGraph<int, IEdge<int>>> createGraph)
        {
            var edge1 = Edge.Create(1, 2);
            var edge2 = Edge.Create(1, 3);
            var edge3 = Edge.Create(1, 4);
            var edge4 = Edge.Create(2, 4);
            var edge5 = Edge.Create(3, 2);
            var edge6 = Edge.Create(3, 3);

            wrappedGraph.AddVerticesAndEdgeRange( edge1, edge2, edge3, edge4, edge5, edge6 );
            wrappedGraph.AddVertex(5);
            IBidirectionalIncidenceGraph<int, IEdge<int>> graph = createGraph();

            Assert.AreEqual(3, graph.Degree(1));
            Assert.AreEqual(3, graph.Degree(2));
            Assert.AreEqual(4, graph.Degree(3)); // Self edge
            Assert.AreEqual(2, graph.Degree(4));
            Assert.AreEqual(0, graph.Degree(5));
        }

        protected static void Degree_ImmutableVertices_Test(
            [NotNull] BidirectionalMatrixGraph<IEdge<int>> graph)
        {
            var edge1 = Edge.Create(1, 2);
            var edge2 = Edge.Create(1, 3);
            var edge3 = Edge.Create(1, 4);
            var edge4 = Edge.Create(2, 4);
            var edge5 = Edge.Create(3, 2);
            var edge6 = Edge.Create(3, 3);

            graph.AddEdgeRange( edge1, edge2, edge3, edge4, edge5, edge6 );

            Assert.AreEqual(0, graph.Degree(0));
            Assert.AreEqual(3, graph.Degree(1));
            Assert.AreEqual(3, graph.Degree(2));
            Assert.AreEqual(4, graph.Degree(3)); // Self edge
            Assert.AreEqual(2, graph.Degree(4));
        }

        protected static void Degree_ImmutableGraph_ReversedTest(
            [NotNull] IMutableVertexAndEdgeSet<int, IEdge<int>> wrappedGraph,
            [NotNull, InstantHandle] Func<IBidirectionalIncidenceGraph<int, SReversedEdge<int, IEdge<int>>>> createGraph)
        {
            var edge1 = Edge.Create(1, 2);
            var edge2 = Edge.Create(1, 3);
            var edge3 = Edge.Create(1, 4);
            var edge4 = Edge.Create(2, 4);
            var edge5 = Edge.Create(3, 2);
            var edge6 = Edge.Create(3, 3);

            wrappedGraph.AddVerticesAndEdgeRange( edge1, edge2, edge3, edge4, edge5, edge6 );
            wrappedGraph.AddVertex(5);
            IBidirectionalIncidenceGraph<int, SReversedEdge<int, IEdge<int>>> graph = createGraph();

            Assert.AreEqual(3, graph.Degree(1));
            Assert.AreEqual(3, graph.Degree(2));
            Assert.AreEqual(4, graph.Degree(3)); // Self edge
            Assert.AreEqual(2, graph.Degree(4));
            Assert.AreEqual(0, graph.Degree(5));
        }

        protected static void Degree_Throws_Test<TVertex, TEdge>(
            [NotNull] IBidirectionalIncidenceGraph<TVertex, TEdge> graph)
            where TVertex : class, IEquatable<TVertex>, new()
            where TEdge : IEdge<TVertex>
        {
            // ReSharper disable ReturnValueOfPureMethodIsNotUsed
            // ReSharper disable once AssignNullToNotNullAttribute
            Assert.Throws<ArgumentNullException>(() => graph.Degree(null));
            Assert.IsNull(graph.Degree(new TVertex()));
            // ReSharper restore ReturnValueOfPureMethodIsNotUsed
        }

        protected static void Degree_Throws_Matrix_Test<TEdge>(
            [NotNull] BidirectionalMatrixGraph<TEdge> graph)
            where TEdge : IEdge<int>
        {
            // ReSharper disable ReturnValueOfPureMethodIsNotUsed
            Assert.AreEqual(default, graph.Degree(-1));
            Assert.AreEqual(default(int?), graph.Degree(10));
            // ReSharper restore ReturnValueOfPureMethodIsNotUsed
        }
    }
}