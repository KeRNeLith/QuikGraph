using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using NUnit.Framework;
using static QuikGraph.Tests.AssertHelpers;
using static QuikGraph.Tests.GraphTestHelpers;

namespace QuikGraph.Tests.Structures
{
    internal partial class GraphTestsBase
    {
        #region Out Edges

        protected static void OutEdge_Test(
            [NotNull] IImplicitGraph<int, IEdge<int>> graph,
            [NotNull, InstantHandle] Action<IEnumerable<IEdge<int>>> addVerticesAndEdgeRange)
        {
            var edge11 = Edge.Create(1, 1);
            var edge12 = Edge.Create(1, 2);
            var edge13 = Edge.Create(1, 3);
            var edge24 = Edge.Create(2, 4);
            var edge33 = Edge.Create(3, 3);
            var edge41 = Edge.Create(4, 1);

            addVerticesAndEdgeRange(new[] { edge11, edge12, edge13, edge24, edge33, edge41 });

            Assert.AreSame(edge11, graph.OutEdge(1, 0));
            Assert.AreSame(edge13, graph.OutEdge(1, 2));
            Assert.AreSame(edge24, graph.OutEdge(2, 0));
            Assert.AreSame(edge33, graph.OutEdge(3, 0));
            Assert.AreSame(edge41, graph.OutEdge(4, 0));
        }

        protected static void OutEdge_Test(
            [NotNull] IMutableVertexAndEdgeListGraph<int, IEdge<int>> graph)
        {
            OutEdge_Test(
                graph,
                edges => graph.AddVerticesAndEdgeRange(edges));
        }

        protected static void OutEdge_ImmutableGraph_Test(
            [NotNull] IMutableVertexAndEdgeSet<int, IEdge<int>> wrappedGraph,
            [NotNull, InstantHandle] Func<IImplicitGraph<int, IEdge<int>>> createGraph)
        {
            var edge11 = Edge.Create(1, 1);
            var edge12 = Edge.Create(1, 2);
            var edge13 = Edge.Create(1, 3);
            var edge24 = Edge.Create(2, 4);
            var edge33 = Edge.Create(3, 3);
            var edge41 = Edge.Create(4, 1);

            wrappedGraph.AddVerticesAndEdgeRange( edge11, edge12, edge13, edge24, edge33, edge41 );
            IImplicitGraph<int, IEdge<int>> graph = createGraph();

            Assert.AreSame(edge11, graph.OutEdge(1, 0));
            Assert.AreSame(edge13, graph.OutEdge(1, 2));
            Assert.AreSame(edge24, graph.OutEdge(2, 0));
            Assert.AreSame(edge33, graph.OutEdge(3, 0));
            Assert.AreSame(edge41, graph.OutEdge(4, 0));
        }

        protected static void OutEdge_ImmutableGraph_Test(
            [NotNull] IMutableVertexAndEdgeSet<int, IEdge<int>> wrappedGraph,
            [NotNull, InstantHandle] Func<IImplicitGraph<int, SEquatableEdge<int>>> createGraph)
        {
            var edge11 = Edge.Create(1, 1);
            var edge12 = Edge.Create(1, 2);
            var edge13 = Edge.Create(1, 3);
            var edge24 = Edge.Create(2, 4);
            var edge33 = Edge.Create(3, 3);
            var edge41 = Edge.Create(4, 1);

            wrappedGraph.AddVerticesAndEdgeRange( edge11, edge12, edge13, edge24, edge33, edge41 );
            IImplicitGraph<int, SEquatableEdge<int>> graph = createGraph();

            Assert.AreEqual(new SEquatableEdge<int>(1, 1), graph.OutEdge(1, 0));
            Assert.AreEqual(new SEquatableEdge<int>(1, 3), graph.OutEdge(1, 2));
            Assert.AreEqual(new SEquatableEdge<int>(2, 4), graph.OutEdge(2, 0));
            Assert.AreEqual(new SEquatableEdge<int>(3, 3), graph.OutEdge(3, 0));
            Assert.AreEqual(new SEquatableEdge<int>(4, 1), graph.OutEdge(4, 0));
        }

        protected static void OutEdge_ImmutableVertices_Test(
            [NotNull] BidirectionalMatrixGraph<IEdge<int>> graph)
        {
            var edge11 = Edge.Create(1, 1);
            var edge12 = Edge.Create(1, 2);
            var edge13 = Edge.Create(1, 3);
            var edge24 = Edge.Create(2, 4);
            var edge33 = Edge.Create(3, 3);
            var edge41 = Edge.Create(4, 1);

            graph.AddEdgeRange( edge11, edge12, edge13, edge24, edge33, edge41 );

            Assert.AreSame(edge11, graph.OutEdge(1, 0));
            Assert.AreSame(edge13, graph.OutEdge(1, 2));
            Assert.AreSame(edge24, graph.OutEdge(2, 0));
            Assert.AreSame(edge33, graph.OutEdge(3, 0));
            Assert.AreSame(edge41, graph.OutEdge(4, 0));
        }

        protected static void OutEdge_ImmutableGraph_ReversedTest(
            [NotNull] IMutableVertexAndEdgeSet<int, IEdge<int>> wrappedGraph,
            [NotNull, InstantHandle] Func<IImplicitGraph<int, SReversedEdge<int, IEdge<int>>>> createGraph)
        {
            var edge11 = Edge.Create(1, 1);
            var edge12 = Edge.Create(1, 2);
            var edge13 = Edge.Create(1, 3);
            var edge21 = Edge.Create(2, 1);
            var edge24 = Edge.Create(2, 4);
            var edge33 = Edge.Create(3, 3);
            var edge41 = Edge.Create(4, 1);

            wrappedGraph.AddVerticesAndEdgeRange( edge11, edge12, edge13, edge21, edge24, edge33, edge41 );
            IImplicitGraph<int, SReversedEdge<int, IEdge<int>>> graph = createGraph();

            AssertSameReversedEdge(edge11, graph.OutEdge(1, 0));
            AssertSameReversedEdge(edge41, graph.OutEdge(1, 2));
            AssertSameReversedEdge(edge12, graph.OutEdge(2, 0));
            AssertSameReversedEdge(edge13, graph.OutEdge(3, 0));
            AssertSameReversedEdge(edge24, graph.OutEdge(4, 0));
        }

        protected static void OutEdge_NullThrows_Test<TVertex, TEdge>(
            [NotNull] IImplicitGraph<TVertex, TEdge> graph)
            where TVertex : class
            where TEdge : IEdge<TVertex>
        {
            // ReSharper disable once ReturnValueOfPureMethodIsNotUsed
            // ReSharper disable once AssignNullToNotNullAttribute
            Assert.Throws<ArgumentNullException>(() => graph.OutEdge(null, 0));
        }

        protected static void OutEdge_Throws_Test(
            [NotNull] IImplicitGraph<int, IEdge<int>> graph,
            [NotNull, InstantHandle] Action<int> addVertex,
            [NotNull, InstantHandle] Action<IEdge<int>> addEdge)
        {
            const int vertex1 = 1;
            const int vertex2 = 2;

            // ReSharper disable ReturnValueOfPureMethodIsNotUsed
            Assert.AreEqual(default, graph.OutEdge(vertex1, 0));

            addVertex(vertex1);
            addVertex(vertex2);
            AssertIndexOutOfRange(() => graph.OutEdge(vertex1, 0));

            addEdge(Edge.Create(1, 2));
            AssertIndexOutOfRange(() => graph.OutEdge(vertex1, 5));
            // ReSharper restore ReturnValueOfPureMethodIsNotUsed
        }

        protected static void OutEdge_Throws_Test(
            [NotNull] IMutableVertexAndEdgeListGraph<int, IEdge<int>> graph)
        {
            OutEdge_Throws_Test(
                graph,
                vertex => graph.AddVertex(vertex),
                edge => graph.AddEdge(edge));
        }

        protected static void OutEdge_Throws_ImmutableGraph_Test<TEdge>(
            [NotNull] IMutableVertexAndEdgeSet<int, IEdge<int>> wrappedGraph,
            [NotNull, InstantHandle] Func<IImplicitGraph<int, TEdge>> createGraph)
            where TEdge : IEdge<int>
        {
            IImplicitGraph<int, TEdge> graph = createGraph();

            const int vertex1 = 1;
            const int vertex2 = 2;

            // ReSharper disable ReturnValueOfPureMethodIsNotUsed
            Assert.AreEqual(default, graph.OutEdge(vertex1, 0));

            wrappedGraph.AddVertex(vertex1);
            wrappedGraph.AddVertex(vertex2);
            graph = createGraph();
            AssertIndexOutOfRange(() => graph.OutEdge(vertex1, 0));

            wrappedGraph.AddEdge(Edge.Create(1, 2));
            graph = createGraph();
            AssertIndexOutOfRange(() => graph.OutEdge(vertex1, 5));
            // ReSharper restore ReturnValueOfPureMethodIsNotUsed
        }

        protected static void OutEdge_Throws_ImmutableVertices_Test(
            [NotNull] BidirectionalMatrixGraph<IEdge<int>> graph)
        {
            // ReSharper disable ReturnValueOfPureMethodIsNotUsed
            Assert.Throws<VertexNotFoundException>(() => graph.OutEdge(-1, 0));
            Assert.Throws<VertexNotFoundException>(() => graph.OutEdge(4, 0));

            graph.AddEdge(Edge.Create(1, 2));
            AssertIndexOutOfRange(() => graph.OutEdge(1, 5));
            // ReSharper restore ReturnValueOfPureMethodIsNotUsed
        }

        protected static void OutEdge_Throws_ImmutableGraph_ReversedTest(
            [NotNull] IMutableVertexAndEdgeSet<int, IEdge<int>> wrappedGraph,
            [NotNull, InstantHandle] Func<IImplicitGraph<int, SReversedEdge<int, IEdge<int>>>> createGraph)
        {
            IImplicitGraph<int, SReversedEdge<int, IEdge<int>>> graph = createGraph();

            const int vertex1 = 1;
            const int vertex2 = 2;

            // ReSharper disable ReturnValueOfPureMethodIsNotUsed
            //Assert.AreEqual(null, graph.OutEdge(vertex1, 0));

            wrappedGraph.AddVertex(vertex1);
            wrappedGraph.AddVertex(vertex2);
            graph = createGraph();
            AssertIndexOutOfRange(() => graph.OutEdge(vertex1, 0));

            wrappedGraph.AddEdge(Edge.Create(1, 2));
            graph = createGraph();
            AssertIndexOutOfRange(() => graph.OutEdge(vertex1, 5));
            // ReSharper restore ReturnValueOfPureMethodIsNotUsed
        }

        protected static void OutEdges_Test(
            [NotNull] IImplicitGraph<int, IEdge<int>> graph,
            [NotNull, InstantHandle] Action<int> addVertex,
            [NotNull, InstantHandle] Action<IEnumerable<IEdge<int>>> addVerticesAndEdgeRange)
        {
            var edge12 = Edge.Create(1, 2);
            var edge13 = Edge.Create(1, 3);
            var edge14 = Edge.Create(1, 4);
            var edge24 = Edge.Create(2, 4);
            var edge31 = Edge.Create(3, 1);
            var edge33 = Edge.Create(3, 3);

            addVertex(1);
            AssertNoOutEdge(graph, 1);

            addVerticesAndEdgeRange(new[] { edge12, edge13, edge14, edge24, edge31, edge33 });

            AssertHasOutEdges(graph, 1, new[] { edge12, edge13, edge14 });
            AssertHasOutEdges(graph, 2, new[] { edge24 });
            AssertHasOutEdges(graph, 3, new[] { edge31, edge33 });
            AssertNoOutEdge(graph, 4);
        }

        protected static void OutEdges_Test(
            [NotNull] IMutableVertexAndEdgeListGraph<int, IEdge<int>> graph)
        {
            OutEdges_Test(
                graph,
                vertex => graph.AddVertex(vertex),
                edges => graph.AddVerticesAndEdgeRange(edges));
        }

        protected static void OutEdges_ImmutableGraph_Test(
            [NotNull] IMutableVertexAndEdgeSet<int, IEdge<int>> wrappedGraph,
            [NotNull, InstantHandle] Func<IImplicitGraph<int, IEdge<int>>> createGraph)
        {
            var edge12 = Edge.Create(1, 2);
            var edge13 = Edge.Create(1, 3);
            var edge14 = Edge.Create(1, 4);
            var edge24 = Edge.Create(2, 4);
            var edge31 = Edge.Create(3, 1);
            var edge33 = Edge.Create(3, 3);

            wrappedGraph.AddVertex(1);
            IImplicitGraph<int, IEdge<int>> graph = createGraph();
            AssertNoOutEdge(graph, 1);

            wrappedGraph.AddVerticesAndEdgeRange( edge12, edge13, edge14, edge24, edge31, edge33 );
            graph = createGraph();

            AssertHasOutEdges(graph, 1, new[] { edge12, edge13, edge14 });
            AssertHasOutEdges(graph, 2, new[] { edge24 });
            AssertHasOutEdges(graph, 3, new[] { edge31, edge33 });
            AssertNoOutEdge(graph, 4);
        }

        protected static void OutEdges_ImmutableGraph_Test(
            [NotNull] IMutableVertexAndEdgeSet<int, IEdge<int>> wrappedGraph,
            [NotNull, InstantHandle] Func<IImplicitGraph<int, SEquatableEdge<int>>> createGraph)
        {
            var edge12 = Edge.Create(1, 2);
            var edge13 = Edge.Create(1, 3);
            var edge14 = Edge.Create(1, 4);
            var edge24 = Edge.Create(2, 4);
            var edge31 = Edge.Create(3, 1);
            var edge33 = Edge.Create(3, 3);

            wrappedGraph.AddVertex(1);
            IImplicitGraph<int, SEquatableEdge<int>> graph = createGraph();
            AssertNoOutEdge(graph, 1);

            wrappedGraph.AddVerticesAndEdgeRange( edge12, edge13, edge14, edge24, edge31, edge33 );
            graph = createGraph();

            AssertHasOutEdges(
                graph, 
                1, 
                new[]
                {
                    new SEquatableEdge<int>(1, 2),
                    new SEquatableEdge<int>(1, 3),
                    new SEquatableEdge<int>(1, 4)
                });
            AssertHasOutEdges(
                graph, 
                2, 
                new[] { new SEquatableEdge<int>(2, 4) });
            AssertHasOutEdges(
                graph, 
                3, 
                new[]
                {
                    new SEquatableEdge<int>(3, 1),
                    new SEquatableEdge<int>(3, 3)
                });
            AssertNoOutEdge(graph, 4);
        }

        protected static void OutEdges_ImmutableVertices_Test(
            [NotNull] BidirectionalMatrixGraph<IEdge<int>> graph)
        {
            var edge01 = Edge.Create(0, 1);
            var edge02 = Edge.Create(0, 2);
            var edge03 = Edge.Create(0, 3);
            var edge13 = Edge.Create(1, 3);
            var edge20 = Edge.Create(2, 0);
            var edge22 = Edge.Create(2, 2);

            AssertNoOutEdge(graph, 1);

            graph.AddEdgeRange( edge01, edge02, edge03, edge13, edge20, edge22 );

            AssertHasOutEdges(graph, 0, new[] { edge01, edge02, edge03 });
            AssertHasOutEdges(graph, 1, new[] { edge13 });
            AssertHasOutEdges(graph, 2, new[] { edge20, edge22 });
            AssertNoOutEdge(graph, 3);
        }

        protected static void OutEdges_ImmutableGraph_ReversedTest(
            [NotNull] IMutableVertexAndEdgeSet<int, IEdge<int>> wrappedGraph,
            [NotNull, InstantHandle] Func<IImplicitGraph<int, SReversedEdge<int, IEdge<int>>>> createGraph)
        {
            var edge12 = Edge.Create(1, 2);
            var edge13 = Edge.Create(1, 3);
            var edge14 = Edge.Create(1, 4);
            var edge24 = Edge.Create(2, 4);
            var edge33 = Edge.Create(3, 3);
            var edge34 = Edge.Create(3, 4);

            wrappedGraph.AddVertex(1);
            IImplicitGraph<int, SReversedEdge<int, IEdge<int>>> graph = createGraph();
            AssertNoOutEdge(graph, 1);

            wrappedGraph.AddVerticesAndEdgeRange( edge12, edge13, edge14, edge24, edge33, edge34 );
            graph = createGraph();

            AssertNoOutEdge(graph, 1);
            AssertHasReversedOutEdges(graph, 2, new[] { edge12 });
            AssertHasReversedOutEdges(graph, 3, new[] { edge13, edge33 });
            AssertHasReversedOutEdges(graph, 4, new[] { edge14, edge24, edge34 });
        }

        protected static void OutEdges_NullThrows_Test<TVertex, TEdge>(
            [NotNull] IImplicitGraph<TVertex, TEdge> graph)
            where TVertex : class
            where TEdge : IEdge<TVertex>
        {
            // ReSharper disable ReturnValueOfPureMethodIsNotUsed
            // ReSharper disable AssignNullToNotNullAttribute
            Assert.Throws<ArgumentNullException>(() => graph.IsOutEdgesEmpty(null));
            Assert.Throws<ArgumentNullException>(() => graph.OutDegree(null));
            Assert.Throws<ArgumentNullException>(() => graph.OutEdges(null).ToArray());
            // ReSharper restore AssignNullToNotNullAttribute
            // ReSharper restore ReturnValueOfPureMethodIsNotUsed
        }

        protected static void OutEdges_Throws_Test<TVertex, TEdge>(
            [NotNull] IImplicitGraph<TVertex, TEdge> graph)
            where TVertex : IEquatable<TVertex>, new()
            where TEdge : IEdge<TVertex>
        {
            // ReSharper disable ReturnValueOfPureMethodIsNotUsed
            // ReSharper disable AssignNullToNotNullAttribute
            var vertex = new TVertex();
            Assert.AreEqual(default, graph.IsOutEdgesEmpty(vertex));
            Assert.AreEqual(default, graph.OutDegree(vertex));
            Assert.AreEqual(default, graph.OutEdges(vertex)?.ToArray());
            // ReSharper restore AssignNullToNotNullAttribute
            // ReSharper restore ReturnValueOfPureMethodIsNotUsed
        }

        protected static void OutEdges_Throws_Matrix_Test<TEdge>(
            [NotNull] BidirectionalMatrixGraph<TEdge> graph)
            where TEdge : IEdge<int>
        {
            const int vertex = 10;
            // ReSharper disable ReturnValueOfPureMethodIsNotUsed
            // ReSharper disable AssignNullToNotNullAttribute
            Assert.AreEqual(default, graph.IsOutEdgesEmpty(vertex));
            Assert.AreEqual(default, graph.OutDegree(vertex));
            Assert.AreEqual(default, graph.OutEdges(vertex)?.ToArray());
            // ReSharper restore AssignNullToNotNullAttribute
            // ReSharper restore ReturnValueOfPureMethodIsNotUsed
            //Assert.Throws<VertexNotFoundException>(() => graph.OutDegree(vertex));
            //Assert.Throws<VertexNotFoundException>(() => graph.OutEdges(vertex).ToArray());
            // ReSharper restore AssignNullToNotNullAttribute
            // ReSharper restore ReturnValueOfPureMethodIsNotUsed
        }

        #endregion
    }
}