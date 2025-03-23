using System;
using JetBrains.Annotations;
using NUnit.Framework;
using static QuikGraph.Tests.AssertHelpers;
using static QuikGraph.Tests.GraphTestHelpers;

namespace QuikGraph.Tests.Structures
{
    internal partial class GraphTestsBase
    {
        #region Adjacent Edges

        protected static void AdjacentEdge_Test(
            [NotNull] IMutableUndirectedGraph<int, IEdge<int>> graph)
        {
            var edge11 = Edge.Create(1, 1);
            var edge12 = Edge.Create(1, 2);
            var edge13 = Edge.Create(1, 3);
            var edge24 = Edge.Create(2, 4);
            var edge33 = Edge.Create(3, 3);
            var edge41 = Edge.Create(4, 1);

            graph.AddVerticesAndEdgeRange( edge11, edge12, edge13, edge24, edge33, edge41 );

            Assert.AreSame(edge11, graph.AdjacentEdge(1, 0));
            Assert.AreSame(edge13, graph.AdjacentEdge(1, 2));
            Assert.AreSame(edge41, graph.AdjacentEdge(1, 3));
            Assert.AreSame(edge13, graph.AdjacentEdge(3, 0));
            Assert.AreSame(edge33, graph.AdjacentEdge(3, 1));
            Assert.AreSame(edge24, graph.AdjacentEdge(4, 0));
        }

        protected static void AdjacentEdge_ImmutableGraph_Test(
            [NotNull] IMutableVertexAndEdgeSet<int, IEdge<int>> wrappedGraph,
            [NotNull, InstantHandle] Func<IImplicitUndirectedGraph<int, IEdge<int>>> createGraph)
        {
            var edge11 = Edge.Create(1, 1);
            var edge12 = Edge.Create(1, 2);
            var edge13 = Edge.Create(1, 3);
            var edge24 = Edge.Create(2, 4);
            var edge33 = Edge.Create(3, 3);
            var edge41 = Edge.Create(4, 1);

            wrappedGraph.AddVerticesAndEdgeRange( edge11, edge12, edge13, edge24, edge33, edge41 );
            IImplicitUndirectedGraph<int, IEdge<int>> graph = createGraph();

            Assert.AreSame(edge11, graph.AdjacentEdge(1, 0));
            Assert.AreSame(edge13, graph.AdjacentEdge(1, 2));
            Assert.AreSame(edge41, graph.AdjacentEdge(1, 3));
            Assert.AreSame(edge13, graph.AdjacentEdge(3, 0));
            Assert.AreSame(edge33, graph.AdjacentEdge(3, 1));
            Assert.AreSame(edge24, graph.AdjacentEdge(4, 0));
        }

        protected static void AdjacentEdge_NullThrows_Test<TVertex, TEdge>(
            [NotNull] IImplicitUndirectedGraph<TVertex, TEdge> graph)
            where TVertex : class
            where TEdge : IEdge<TVertex>
        {
            // ReSharper disable once ReturnValueOfPureMethodIsNotUsed
            // ReSharper disable once AssignNullToNotNullAttribute
            Assert.Throws<ArgumentNullException>(() => graph.AdjacentEdge(null, 0));
        }

        protected static void AdjacentEdge_Throws_Test(
            [NotNull] IMutableUndirectedGraph<int, IEdge<int>> graph)
        {
            const int vertex1 = 1;
            const int vertex2 = 2;

            // ReSharper disable ReturnValueOfPureMethodIsNotUsed
            Assert.IsNull(graph.AdjacentEdge(vertex1, 0));

            graph.AddVertex(vertex1);
            graph.AddVertex(vertex2);
            AssertIndexOutOfRange(() => graph.AdjacentEdge(vertex1, 0));

            graph.AddEdge(Edge.Create(1, 2));
            AssertIndexOutOfRange(() => graph.AdjacentEdge(vertex1, 5));
            // ReSharper restore ReturnValueOfPureMethodIsNotUsed
        }

        protected static void AdjacentEdge_Throws_ImmutableGraph_Test(
            [NotNull] IMutableVertexAndEdgeSet<int, IEdge<int>> wrappedGraph,
            [NotNull, InstantHandle] Func<IImplicitUndirectedGraph<int, IEdge<int>>> createGraph)
        {
            const int vertex1 = 1;
            const int vertex2 = 2;

            IImplicitUndirectedGraph<int, IEdge<int>> graph = createGraph();
            // ReSharper disable ReturnValueOfPureMethodIsNotUsed
            Assert.IsNull(graph.AdjacentEdge(vertex1, 0));

            wrappedGraph.AddVertex(vertex1);
            wrappedGraph.AddVertex(vertex2);
            graph = createGraph();
            AssertIndexOutOfRange(() => graph.AdjacentEdge(vertex1, 0));

            wrappedGraph.AddEdge(Edge.Create(1, 2));
            graph = createGraph();
            AssertIndexOutOfRange(() => graph.AdjacentEdge(vertex1, 5));
            // ReSharper restore ReturnValueOfPureMethodIsNotUsed
        }

        protected static void AdjacentEdges_Test(
            [NotNull] IMutableUndirectedGraph<int, IEdge<int>> graph)
        {
            var edge12 = Edge.Create(1, 2);
            var edge13 = Edge.Create(1, 3);
            var edge14 = Edge.Create(1, 4);
            var edge24 = Edge.Create(2, 4);
            var edge31 = Edge.Create(3, 1);
            var edge33 = Edge.Create(3, 3);

            graph.AddVertex(1);
            AssertNoAdjacentEdge(graph, 1);

            graph.AddVertex(5);
            graph.AddVerticesAndEdgeRange( edge12, edge13, edge14, edge24, edge31, edge33 );

            graph.AssertHasAdjacentEdges(1, new[] { edge12, edge13, edge14, edge31 });
            graph.AssertHasAdjacentEdges(2, new[] { edge12, edge24 });
            graph.AssertHasAdjacentEdges(3, new[] { edge13, edge31, edge33 }, 4);  // Has self edge counting twice
            graph.AssertHasAdjacentEdges(4, new[] { edge14, edge24 });
            AssertNoAdjacentEdge(graph, 5);
        }

        protected static void AdjacentEdges_ImmutableGraph_Test(
            [NotNull] IMutableVertexAndEdgeSet<int, IEdge<int>> wrappedGraph,
            [NotNull, InstantHandle] Func<IImplicitUndirectedGraph<int, IEdge<int>>> createGraph)
        {
            var edge12 = Edge.Create(1, 2);
            var edge13 = Edge.Create(1, 3);
            var edge14 = Edge.Create(1, 4);
            var edge24 = Edge.Create(2, 4);
            var edge31 = Edge.Create(3, 1);
            var edge33 = Edge.Create(3, 3);

            wrappedGraph.AddVertex(1);
            IImplicitUndirectedGraph<int, IEdge<int>> graph = createGraph();
            AssertNoAdjacentEdge(graph, 1);

            wrappedGraph.AddVertex(5);
            wrappedGraph.AddVerticesAndEdgeRange( edge12, edge13, edge14, edge24, edge31, edge33 );
            graph = createGraph();

            graph.AssertHasAdjacentEdges(1, new[] { edge12, edge13, edge14, edge31 });
            graph.AssertHasAdjacentEdges(2, new[] { edge12, edge24 });
            graph.AssertHasAdjacentEdges(3, new[] { edge13, edge31, edge33 }, 4);  // Has self edge counting twice
            graph.AssertHasAdjacentEdges(4, new[] { edge14, edge24 });
            AssertNoAdjacentEdge(graph, 5);
        }

        protected static void AdjacentEdges_NullThrows_Test<TVertex, TEdge>(
            [NotNull] IImplicitUndirectedGraph<TVertex, TEdge> graph)
            where TVertex : class
            where TEdge : IEdge<TVertex>
        {
            // ReSharper disable ReturnValueOfPureMethodIsNotUsed
            // ReSharper disable AssignNullToNotNullAttribute
            Assert.Throws<ArgumentNullException>(() => graph.IsAdjacentEdgesEmpty(null));
            Assert.Throws<ArgumentNullException>(() => graph.AdjacentDegree(null));
            Assert.Throws<ArgumentNullException>(() => graph.AdjacentEdges(null));
            // ReSharper restore AssignNullToNotNullAttribute
            // ReSharper restore ReturnValueOfPureMethodIsNotUsed
        }

        protected static void AdjacentEdges_Throws_Test<TVertex>(
            [NotNull] IImplicitUndirectedGraph<TVertex, Edge<TVertex>> graph)
            where TVertex : class, IEquatable<TVertex>, new()
        {
            AdjacentEdges_NullThrows_Test(graph);

            // ReSharper disable ReturnValueOfPureMethodIsNotUsed
            var vertex = new TVertex();
            Assert.IsNull(graph.IsAdjacentEdgesEmpty(vertex));
            Assert.IsNull(graph.AdjacentDegree(vertex));;
            Assert.IsNull(graph.AdjacentEdges(vertex));
            // ReSharper restore ReturnValueOfPureMethodIsNotUsed
        }

        #endregion
    }
}