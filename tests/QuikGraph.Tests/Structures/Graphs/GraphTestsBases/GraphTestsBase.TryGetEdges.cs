using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using NUnit.Framework;
using static QuikGraph.Tests.GraphTestHelpers;

namespace QuikGraph.Tests.Structures
{
    internal partial class GraphTestsBase
    {
        #region Try Get Edges

        protected static void TryGetEdge_Test(
            [NotNull] IIncidenceGraph<int, IEdge<int>> graph,
            [NotNull, InstantHandle] Action<IEnumerable<IEdge<int>>> addVerticesAndEdgeRange)
        {
            var edge1 = Edge.Create(1, 2);
            var edge2 = Edge.Create(1, 2);
            var edge3 = Edge.Create(1, 3);
            var edge4 = Edge.Create(2, 2);
            var edge5 = Edge.Create(2, 4);
            var edge6 = Edge.Create(3, 1);

            addVerticesAndEdgeRange( new[] {edge1, edge2, edge3, edge4, edge5, edge6 } );

            Assert.IsFalse(graph.TryGetEdge(0, 10, out _));
            Assert.IsFalse(graph.TryGetEdge(0, 1, out _));

            Assert.IsTrue(graph.TryGetEdge(2, 4, out IEdge<int> gotEdge));
            Assert.AreSame(edge5, gotEdge);

            Assert.IsTrue(graph.TryGetEdge(2, 2, out gotEdge));
            Assert.AreSame(edge4, gotEdge);

            Assert.IsTrue(graph.TryGetEdge(1, 2, out gotEdge));
            Assert.AreSame(edge1, gotEdge);

            Assert.IsFalse(graph.TryGetEdge(2, 1, out _));
        }

        protected static void TryGetEdge_Test([NotNull] IMutableVertexAndEdgeListGraph<int, IEdge<int>> graph)
        {
            TryGetEdge_Test(
                graph,
                edges => graph.AddVerticesAndEdgeRange(edges));
        }

        protected static void TryGetEdge_ImmutableGraph_Test(
            [NotNull] IMutableVertexAndEdgeSet<int, IEdge<int>> wrappedGraph,
            [NotNull, InstantHandle] Func<IIncidenceGraph<int, IEdge<int>>> createGraph)
        {
            var edge1 = Edge.Create(1, 2);
            var edge2 = Edge.Create(1, 2);
            var edge3 = Edge.Create(1, 3);
            var edge4 = Edge.Create(2, 2);
            var edge5 = Edge.Create(2, 4);
            var edge6 = Edge.Create(3, 1);

            wrappedGraph.AddVerticesAndEdgeRange( edge1, edge2, edge3, edge4, edge5, edge6 );
            IIncidenceGraph<int, IEdge<int>> graph = createGraph();

            Assert.IsFalse(graph.TryGetEdge(0, 10, out _));
            Assert.IsFalse(graph.TryGetEdge(0, 1, out _));

            Assert.IsTrue(graph.TryGetEdge(2, 4, out IEdge<int> gotEdge));
            Assert.AreSame(edge5, gotEdge);

            Assert.IsTrue(graph.TryGetEdge(2, 2, out gotEdge));
            Assert.AreSame(edge4, gotEdge);

            Assert.IsTrue(graph.TryGetEdge(1, 2, out gotEdge));
            Assert.AreSame(edge1, gotEdge);

            Assert.IsFalse(graph.TryGetEdge(2, 1, out _));
        }

        protected static void TryGetEdge_ImmutableGraph_Test(
            [NotNull] IMutableVertexAndEdgeSet<int, IEdge<int>> wrappedGraph,
            [NotNull, InstantHandle] Func<IIncidenceGraph<int, SEquatableEdge<int>>> createGraph)
        {
            var edge1 = Edge.Create(1, 2);
            var edge2 = Edge.Create(1, 2);
            var edge3 = Edge.Create(1, 3);
            var edge4 = Edge.Create(2, 2);
            var edge5 = Edge.Create(2, 4);
            var edge6 = Edge.Create(3, 1);

            wrappedGraph.AddVerticesAndEdgeRange( edge1, edge2, edge3, edge4, edge5, edge6 );
            IIncidenceGraph<int, SEquatableEdge<int>> graph = createGraph();

            Assert.IsFalse(graph.TryGetEdge(0, 10, out _));
            Assert.IsFalse(graph.TryGetEdge(0, 1, out _));

            Assert.IsTrue(graph.TryGetEdge(2, 4, out SEquatableEdge<int> gotEdge));
            Assert.AreEqual(new SEquatableEdge<int>(2, 4), gotEdge);

            Assert.IsTrue(graph.TryGetEdge(2, 2, out gotEdge));
            Assert.AreEqual(new SEquatableEdge<int>(2, 2), gotEdge);

            Assert.IsTrue(graph.TryGetEdge(1, 2, out gotEdge));
            Assert.AreEqual(new SEquatableEdge<int>(1, 2), gotEdge);

            Assert.IsFalse(graph.TryGetEdge(2, 1, out _));
        }

        protected static void TryGetEdge_ImmutableVertices_Test([NotNull] BidirectionalMatrixGraph<IEdge<int>> graph)
        {
            var edge1 = Edge.Create(1, 2);
            var edge2 = Edge.Create(1, 3);
            var edge3 = Edge.Create(2, 2);
            var edge4 = Edge.Create(2, 4);
            var edge5 = Edge.Create(3, 1);

            graph.AddEdgeRange( edge1, edge2, edge3, edge4, edge5 );

            Assert.IsFalse(graph.TryGetEdge(6, 10, out _));
            Assert.IsFalse(graph.TryGetEdge(6, 1, out _));

            Assert.IsTrue(graph.TryGetEdge(2, 4, out IEdge<int> gotEdge));
            Assert.AreSame(edge4, gotEdge);

            Assert.IsTrue(graph.TryGetEdge(2, 2, out gotEdge));
            Assert.AreSame(edge3, gotEdge);

            Assert.IsTrue(graph.TryGetEdge(1, 2, out gotEdge));
            Assert.AreSame(edge1, gotEdge);

            Assert.IsFalse(graph.TryGetEdge(2, 1, out _));
        }

        protected static void GetEdge_ImmutableGraph_ReversedTest(
            [NotNull] IMutableVertexAndEdgeSet<int, IEdge<int>> wrappedGraph,
            [NotNull, InstantHandle] Func<IIncidenceGraph<int, SReversedEdge<int, IEdge<int>>>> createGraph)
        {
            var edge1 = Edge.Create(1, 2);
            var edge2 = Edge.Create(1, 2);
            var edge3 = Edge.Create(1, 3);
            var edge4 = Edge.Create(2, 2);
            var edge5 = Edge.Create(2, 4);
            var edge6 = Edge.Create(3, 1);

            wrappedGraph.AddVerticesAndEdgeRange( edge1, edge2, edge3, edge4, edge5, edge6 );
            IIncidenceGraph<int, SReversedEdge<int, IEdge<int>>> graph = createGraph();

            Assert.IsFalse(graph.TryGetEdge(0, 10, out _));
            Assert.IsFalse(graph.TryGetEdge(0, 1, out _));

            Assert.IsTrue(graph.TryGetEdge(4, 2, out SReversedEdge<int, IEdge<int>> gotEdge));
            AssertSameReversedEdge(edge5, gotEdge);

            Assert.IsTrue(graph.TryGetEdge(2, 2, out gotEdge));
            AssertSameReversedEdge(edge4, gotEdge);

            Assert.IsTrue(graph.TryGetEdge(2, 1, out gotEdge));
            AssertSameReversedEdge(edge1, gotEdge);

            Assert.IsFalse(graph.TryGetEdge(1, 2, out _));
        }

        protected static void TryGetEdge_UndirectedGraph_Test(
            [NotNull] IMutableUndirectedGraph<int, IEdge<int>> graph)
        {
            var edge1 = Edge.Create(1, 2);
            var edge2 = Edge.Create(1, 2);
            var edge3 = Edge.Create(1, 3);
            var edge4 = Edge.Create(2, 2);
            var edge5 = Edge.Create(2, 4);
            var edge6 = Edge.Create(3, 1);
            var edge7 = Edge.Create(5, 2);

            graph.AddVerticesAndEdgeRange( edge1, edge2, edge3, edge4, edge5, edge6, edge7 );

            Assert.IsFalse(graph.TryGetEdge(0, 10, out _));
            Assert.IsFalse(graph.TryGetEdge(0, 1, out _));

            Assert.IsTrue(graph.TryGetEdge(2, 4, out IEdge<int> gotEdge));
            Assert.AreSame(edge5, gotEdge);

            Assert.IsTrue(graph.TryGetEdge(1, 2, out gotEdge));
            Assert.AreSame(edge1, gotEdge);

            Assert.IsTrue(graph.TryGetEdge(2, 2, out gotEdge));
            Assert.AreSame(edge4, gotEdge);

            // 1 -> 2 is present in this undirected graph
            Assert.IsTrue(graph.TryGetEdge(2, 1, out gotEdge));
            Assert.AreSame(edge1, gotEdge);

            Assert.IsTrue(graph.TryGetEdge(5, 2, out gotEdge));
            Assert.AreSame(edge7, gotEdge);

            // 5 -> 2 is present in this undirected graph
            Assert.IsTrue(graph.TryGetEdge(2, 5, out gotEdge));
            Assert.AreSame(edge7, gotEdge);
        }

        protected static void TryGetEdge_ImmutableGraph_UndirectedGraph_Test(
            [NotNull] IMutableVertexAndEdgeSet<int, IEdge<int>> wrappedGraph,
            [NotNull, InstantHandle] Func<IImplicitUndirectedGraph<int, IEdge<int>>> createGraph)
        {
            var edge1 = Edge.Create(1, 2);
            var edge2 = Edge.Create(1, 2);
            var edge3 = Edge.Create(1, 3);
            var edge4 = Edge.Create(2, 2);
            var edge5 = Edge.Create(2, 4);
            var edge6 = Edge.Create(3, 1);
            var edge7 = Edge.Create(5, 2);

            wrappedGraph.AddVerticesAndEdgeRange( edge1, edge2, edge3, edge4, edge5, edge6, edge7 );
            IImplicitUndirectedGraph<int, IEdge<int>> graph = createGraph();

            Assert.IsFalse(graph.TryGetEdge(0, 10, out _));
            Assert.IsFalse(graph.TryGetEdge(0, 1, out _));

            Assert.IsTrue(graph.TryGetEdge(2, 4, out IEdge<int> gotEdge));
            Assert.AreSame(edge5, gotEdge);

            Assert.IsTrue(graph.TryGetEdge(1, 2, out gotEdge));
            Assert.AreSame(edge1, gotEdge);

            Assert.IsTrue(graph.TryGetEdge(2, 2, out gotEdge));
            Assert.AreSame(edge4, gotEdge);

            // 1 -> 2 is present in this undirected graph
            Assert.IsTrue(graph.TryGetEdge(2, 1, out gotEdge));
            Assert.AreSame(edge1, gotEdge);

            Assert.IsTrue(graph.TryGetEdge(5, 2, out gotEdge));
            Assert.AreSame(edge7, gotEdge);

            // 5 -> 2 is present in this undirected graph
            Assert.IsTrue(graph.TryGetEdge(2, 5, out gotEdge));
            Assert.AreSame(edge7, gotEdge);
        }

        protected static void TryGetEdge_Throws_Test<TVertex, TEdge>(
            [NotNull] IIncidenceGraph<TVertex, TEdge> graph)
            where TVertex : class, new()
            where TEdge : IEdge<TVertex>
        {
            // ReSharper disable AssignNullToNotNullAttribute
            Assert.Throws<ArgumentNullException>(() => graph.TryGetEdge(null, new TVertex(), out _));
            Assert.Throws<ArgumentNullException>(() => graph.TryGetEdge(new TVertex(), null, out _));
            Assert.Throws<ArgumentNullException>(() => graph.TryGetEdge(null, null, out _));
            // ReSharper restore AssignNullToNotNullAttribute
        }

        protected static void TryGetEdge_Throws_UndirectedGraph_Test<TVertex, TEdge>(
            [NotNull] IImplicitUndirectedGraph<TVertex, TEdge> graph)
            where TVertex : class, new()
            where TEdge : IEdge<TVertex>
        {
            // ReSharper disable AssignNullToNotNullAttribute
            Assert.Throws<ArgumentNullException>(() => graph.TryGetEdge(null, new TVertex(), out _));
            Assert.Throws<ArgumentNullException>(() => graph.TryGetEdge(new TVertex(), null, out _));
            Assert.Throws<ArgumentNullException>(() => graph.TryGetEdge(null, null, out _));
            // ReSharper restore AssignNullToNotNullAttribute
        }

        protected static void GetEdges_Test(
            [NotNull] IIncidenceGraph<int, IEdge<int>> graph,
            [NotNull, InstantHandle] Action<IEnumerable<IEdge<int>>> addVerticesAndEdgeRange)
        {
            var edge1 = Edge.Create(1, 2);
            var edge2 = Edge.Create(1, 2);
            var edge3 = Edge.Create(1, 3);
            var edge4 = Edge.Create(2, 2);
            var edge5 = Edge.Create(2, 4);
            var edge6 = Edge.Create(3, 1);

            addVerticesAndEdgeRange(new[] { edge1, edge2, edge3, edge4, edge5, edge6 });

            Assert.IsEmpty(graph.GetEdges(0, 10));
            Assert.IsEmpty(graph.GetEdges(0, 1));

            var gotEdges = graph.GetEdges(2, 2);
            CollectionAssert.AreEqual(new[] { edge4 }, gotEdges);

            gotEdges = graph.GetEdges(2, 4);
            CollectionAssert.AreEqual(new[] { edge5 }, gotEdges);

            gotEdges = graph.GetEdges(1, 2);
            CollectionAssert.AreEqual(new[] { edge1, edge2 }, gotEdges);

            gotEdges = graph.GetEdges(2, 1);
            CollectionAssert.IsEmpty(gotEdges);
        }

        protected static void GetEdges_Test(
            [NotNull] IMutableVertexAndEdgeListGraph<int, IEdge<int>> graph)
            => GetEdges_Test(graph,edges => graph.AddVerticesAndEdgeRange(edges));

        protected static void GetEdges_ImmutableGraph_Test(
            [NotNull] IMutableVertexAndEdgeSet<int, IEdge<int>> wrappedGraph,
            [NotNull, InstantHandle] Func<IIncidenceGraph<int, IEdge<int>>> createGraph)
        {
            var edge1 = Edge.Create(1, 2);
            var edge2 = Edge.Create(1, 2);
            var edge3 = Edge.Create(1, 3);
            var edge4 = Edge.Create(2, 2);
            var edge5 = Edge.Create(2, 4);
            var edge6 = Edge.Create(3, 1);

            wrappedGraph.AddVerticesAndEdgeRange( edge1, edge2, edge3, edge4, edge5, edge6 );
            IIncidenceGraph<int, IEdge<int>> graph = createGraph();

            Assert.IsEmpty(graph.GetEdges(0, 10));
            Assert.IsEmpty(graph.GetEdges(0, 1));

            var gotEdges = graph.GetEdges(2, 2);
            CollectionAssert.AreEqual(new[] { edge4 }, gotEdges);

            gotEdges = graph.GetEdges(2, 4);
            CollectionAssert.AreEqual(new[] { edge5 }, gotEdges);

            gotEdges = graph.GetEdges(1, 2);
            CollectionAssert.AreEqual(new[] { edge1, edge2 }, gotEdges);

            gotEdges = graph.GetEdges(2, 1);
            CollectionAssert.IsEmpty(gotEdges);
        }

        protected static void GetEdges_ImmutableGraph_Test(
            [NotNull] IMutableVertexAndEdgeSet<int, IEdge<int>> wrappedGraph,
            [NotNull, InstantHandle] Func<IIncidenceGraph<int, SEquatableEdge<int>>> createGraph)
        {
            var edge1 = Edge.Create(1, 2);
            var edge2 = Edge.Create(1, 2);
            var edge3 = Edge.Create(1, 3);
            var edge4 = Edge.Create(2, 2);
            var edge5 = Edge.Create(2, 4);
            var edge6 = Edge.Create(3, 1);

            wrappedGraph.AddVerticesAndEdgeRange( edge1, edge2, edge3, edge4, edge5, edge6 );
            IIncidenceGraph<int, SEquatableEdge<int>> graph = createGraph();

            Assert.IsEmpty(graph.GetEdges(0, 10));
            Assert.IsEmpty(graph.GetEdges(0, 1));

            var gotEdges = graph.GetEdges(2, 2);
            CollectionAssert.AreEqual(
                new[] { new SEquatableEdge<int>(2, 2) },
                gotEdges);

            gotEdges = graph.GetEdges(2, 4);
            CollectionAssert.AreEqual(
                new[] { new SEquatableEdge<int>(2, 4), },
                gotEdges);

            gotEdges = graph.GetEdges(1, 2);
            CollectionAssert.AreEqual(
                new[]
                {
                    new SEquatableEdge<int>(1, 2),
                    new SEquatableEdge<int>(1, 2)
                },
                gotEdges);

            gotEdges = graph.GetEdges(2, 1);
            CollectionAssert.IsEmpty(gotEdges);
        }

        protected static void GetEdges_ImmutableVertices_Test(
            [NotNull] BidirectionalMatrixGraph<IEdge<int>> graph)
        {
            var edge1 = Edge.Create(1, 2);
            var edge2 = Edge.Create(1, 3);
            var edge3 = Edge.Create(2, 2);
            var edge4 = Edge.Create(2, 4);
            var edge5 = Edge.Create(3, 1);

            graph.AddEdgeRange( edge1, edge2, edge3, edge4, edge5 );

            Assert.IsEmpty(graph.GetEdges(6, 10));
            Assert.IsEmpty(graph.GetEdges(6, 1));

            var gotEdges = graph.GetEdges(2, 2);
            CollectionAssert.AreEqual(new[] { edge3 }, gotEdges);

            gotEdges = graph.GetEdges(2, 4);
            CollectionAssert.AreEqual(new[] { edge4 }, gotEdges);

            gotEdges = graph.GetEdges(1, 2);
            CollectionAssert.AreEqual(new[] { edge1 }, gotEdges);

            gotEdges = graph.GetEdges(2, 1);
            CollectionAssert.IsEmpty(gotEdges);
        }

        protected static void GetEdges_ImmutableGraph_ReversedTest(
            [NotNull] IMutableVertexAndEdgeSet<int, IEdge<int>> wrappedGraph,
            [NotNull, InstantHandle] Func<IIncidenceGraph<int, SReversedEdge<int, IEdge<int>>>> createGraph)
        {
            var edge1 = Edge.Create(1, 2);
            var edge2 = Edge.Create(1, 2);
            var edge3 = Edge.Create(1, 3);
            var edge4 = Edge.Create(2, 2);
            var edge5 = Edge.Create(2, 4);
            var edge6 = Edge.Create(3, 1);

            wrappedGraph.AddVerticesAndEdgeRange( edge1, edge2, edge3, edge4, edge5, edge6 );
            IIncidenceGraph<int, SReversedEdge<int, IEdge<int>>> graph = createGraph();

            Assert.IsEmpty(graph.GetEdges(0, 10));
            Assert.IsEmpty(graph.GetEdges(0, 1));

            var gotEdges = graph.GetEdges(2, 2);
            AssertSameReversedEdges(new[] { edge4 }, gotEdges);

            gotEdges = graph.GetEdges(4, 2);
            AssertSameReversedEdges(new[] { edge5 }, gotEdges);

            gotEdges = graph.GetEdges(2, 1); 
            AssertSameReversedEdges(new[] { edge1, edge2 }, gotEdges);

            gotEdges = graph.GetEdges(1, 2);
            CollectionAssert.IsEmpty(gotEdges);
        }

        protected static void GetEdges_Throws_Test<TEdge>(
            [NotNull] IIncidenceGraph<TestVertex, TEdge> graph)
            where TEdge : IEdge<TestVertex>
        {
            // ReSharper disable AssignNullToNotNullAttribute
            Assert.Throws<ArgumentNullException>(() => graph.GetEdges(null, new TestVertex("v2")));
            Assert.Throws<ArgumentNullException>(() => graph.GetEdges(new TestVertex("v1"), null));
            Assert.Throws<ArgumentNullException>(() => graph.GetEdges(null, null));
            // ReSharper restore AssignNullToNotNullAttribute
        }

        protected static void GetOutEdges_Test(
            [NotNull] IImplicitGraph<int, IEdge<int>> graph,
            [NotNull, InstantHandle] Action<IEnumerable<IEdge<int>>> addVerticesAndEdgeRange)
        {
            var edge1 = Edge.Create(1, 2);
            var edge2 = Edge.Create(1, 2);
            var edge3 = Edge.Create(1, 3);
            var edge4 = Edge.Create(2, 2);
            var edge5 = Edge.Create(2, 4);
            var edge6 = Edge.Create(3, 1);
            var edge7 = Edge.Create(4, 5);

            addVerticesAndEdgeRange(new[] { edge1, edge2, edge3, edge4, edge5, edge6, edge7 });

            Assert.IsNull(graph.OutEdges(0));

            var gotEdges = graph.OutEdges(5);
            CollectionAssert.IsEmpty(gotEdges);

            gotEdges = graph.OutEdges(3);
            CollectionAssert.AreEqual(new[] { edge6 }, gotEdges);

            gotEdges = graph.OutEdges(1);
            CollectionAssert.AreEqual(new[] { edge1, edge2, edge3 }, gotEdges);
        }

        protected static void GetOutEdges_Test(
            [NotNull] IMutableVertexAndEdgeListGraph<int, IEdge<int>> graph)
        {
            GetOutEdges_Test(graph, edges => graph.AddVerticesAndEdgeRange(edges));
        }

        protected static void GetOutEdges_ImmutableGraph_Test(
            [NotNull] IMutableVertexAndEdgeSet<int, IEdge<int>> wrappedGraph,
            [NotNull, InstantHandle] Func<IImplicitGraph<int, IEdge<int>>> createGraph)
        {
            var edge1 = Edge.Create(1, 2);
            var edge2 = Edge.Create(1, 2);
            var edge3 = Edge.Create(1, 3);
            var edge4 = Edge.Create(2, 2);
            var edge5 = Edge.Create(2, 4);
            var edge6 = Edge.Create(3, 1);
            var edge7 = Edge.Create(4, 5);

            wrappedGraph.AddVerticesAndEdgeRange( edge1, edge2, edge3, edge4, edge5, edge6, edge7 );
            IImplicitGraph<int, IEdge<int>> graph = createGraph();

            Assert.IsNull(graph.OutEdges(0));

            var gotEdges = graph.OutEdges(5);
            CollectionAssert.IsEmpty(gotEdges);

            gotEdges = graph.OutEdges(3);
            CollectionAssert.AreEqual(new[] { edge6 }, gotEdges);

            gotEdges = graph.OutEdges(1);
            CollectionAssert.AreEqual(new[] { edge1, edge2, edge3 }, gotEdges);
        }

        protected static void GetOutEdges_ImmutableGraph_Test(
            [NotNull] IMutableVertexAndEdgeSet<int, IEdge<int>> wrappedGraph,
            [NotNull, InstantHandle] Func<IImplicitGraph<int, SEquatableEdge<int>>> createGraph)
        {
            var edge1 = Edge.Create(1, 2);
            var edge2 = Edge.Create(1, 2);
            var edge3 = Edge.Create(1, 3);
            var edge4 = Edge.Create(2, 2);
            var edge5 = Edge.Create(2, 4);
            var edge6 = Edge.Create(3, 1);
            var edge7 = Edge.Create(4, 5);

            wrappedGraph.AddVerticesAndEdgeRange( edge1, edge2, edge3, edge4, edge5, edge6, edge7 );
            IImplicitGraph<int, SEquatableEdge<int>> graph = createGraph();

            Assert.IsEmpty(graph.OutEdges(0));

            var gotEdges = graph.OutEdges(5);
            CollectionAssert.IsEmpty(gotEdges);

            gotEdges = graph.OutEdges(3);
            CollectionAssert.AreEqual(
                new[] { new SEquatableEdge<int>(3, 1) },
                gotEdges);

            gotEdges = graph.OutEdges(1);
            CollectionAssert.AreEqual(
                new[]
                {
                    new SEquatableEdge<int>(1, 2),
                    new SEquatableEdge<int>(1, 2),
                    new SEquatableEdge<int>(1, 3)
                },
                gotEdges);
        }

        protected static void GetOutEdges_ImmutableVertices_Test(
            [NotNull] BidirectionalMatrixGraph<IEdge<int>> graph)
        {
            var edge1 = Edge.Create(1, 2);
            var edge2 = Edge.Create(1, 3);
            var edge3 = Edge.Create(2, 2);
            var edge4 = Edge.Create(2, 4);
            var edge5 = Edge.Create(3, 1);
            var edge6 = Edge.Create(4, 5);

            graph.AddEdgeRange( edge1, edge2, edge3, edge4, edge5, edge6 );

            Assert.IsNull(graph.OutEdges(6));

            var gotEdges = graph.OutEdges(5);
            CollectionAssert.IsEmpty(gotEdges);

            gotEdges = graph.OutEdges(3);
            CollectionAssert.AreEqual(new[] { edge5 }, gotEdges);

            gotEdges = graph.OutEdges(1);
            CollectionAssert.AreEqual(new[] { edge1, edge2 }, gotEdges);
        }

        protected static void GetOutEdges_ImmutableGraph_ReversedTest(
            [NotNull] IMutableVertexAndEdgeSet<int, IEdge<int>> wrappedGraph,
            [NotNull, InstantHandle] Func<IImplicitGraph<int, SReversedEdge<int, IEdge<int>>>> createGraph)
        {
            var edge1 = Edge.Create(1, 2);
            var edge2 = Edge.Create(1, 2);
            var edge3 = Edge.Create(1, 3);
            var edge4 = Edge.Create(2, 2);
            var edge5 = Edge.Create(2, 4);
            var edge6 = Edge.Create(3, 1);
            var edge7 = Edge.Create(5, 4);

            wrappedGraph.AddVerticesAndEdgeRange( edge1, edge2, edge3, edge4, edge5, edge6, edge7 );
            IImplicitGraph<int, SReversedEdge<int, IEdge<int>>> graph = createGraph();

            Assert.IsNull(graph.OutEdges(0));

            var gotEdges = graph.OutEdges(5);
            CollectionAssert.IsEmpty(gotEdges);

            gotEdges = graph.OutEdges(3);
            AssertSameReversedEdges(new[] { edge3 }, gotEdges);

            gotEdges = graph.OutEdges(2);
            AssertSameReversedEdges(new[] { edge1, edge2, edge4 }, gotEdges);
        }

        protected static void GetOutEdges_Throws_Test<TVertex, TEdge>(
            [NotNull] IImplicitGraph<TVertex, TEdge> graph)
            where TVertex : class
            where TEdge : IEdge<TVertex>
        {
            // ReSharper disable once AssignNullToNotNullAttribute
            Assert.Throws<ArgumentNullException>(() => graph.OutEdges(null));
        }

        protected static void GetInEdges_Test(
            [NotNull] IBidirectionalIncidenceGraph<int, IEdge<int>> graph,
            [NotNull, InstantHandle] Action<IEnumerable<IEdge<int>>> addVerticesAndEdgeRange)
        {
            var edge1 = Edge.Create(1, 2);
            var edge2 = Edge.Create(1, 2);
            var edge3 = Edge.Create(1, 3);
            var edge4 = Edge.Create(2, 2);
            var edge5 = Edge.Create(2, 4);
            var edge6 = Edge.Create(3, 1);
            var edge7 = Edge.Create(5, 3);

            addVerticesAndEdgeRange(new[] { edge1, edge2, edge3, edge4, edge5, edge6, edge7 });

            Assert.IsNull(graph.InEdges(0));

            var gotEdges = graph.InEdges(5);
            CollectionAssert.IsEmpty(gotEdges);

            gotEdges = graph.InEdges(4);
            CollectionAssert.AreEqual(new[] { edge5 }, gotEdges);

            gotEdges = graph.InEdges(2);
            CollectionAssert.AreEqual(new[] { edge1, edge2, edge4 }, gotEdges);
        }

        protected static void GetInEdges_Test(
            [NotNull] IMutableBidirectionalGraph<int, IEdge<int>> graph)
        {
            GetInEdges_Test(
                graph,
                edges => graph.AddVerticesAndEdgeRange(edges));
        }

        protected static void GetInEdges_ImmutableGraph_Test(
            [NotNull] IMutableVertexAndEdgeSet<int, IEdge<int>> wrappedGraph,
            [NotNull, InstantHandle] Func<IBidirectionalIncidenceGraph<int, IEdge<int>>> createGraph)
        {
            var edge1 = Edge.Create(1, 2);
            var edge2 = Edge.Create(1, 2);
            var edge3 = Edge.Create(1, 3);
            var edge4 = Edge.Create(2, 2);
            var edge5 = Edge.Create(2, 4);
            var edge6 = Edge.Create(3, 1);
            var edge7 = Edge.Create(5, 3);

            wrappedGraph.AddVerticesAndEdgeRange( edge1, edge2, edge3, edge4, edge5, edge6, edge7 );
            IBidirectionalIncidenceGraph<int, IEdge<int>> graph = createGraph();

            Assert.IsNull(graph.InEdges(0));

            var gotEdges = graph.InEdges(5);
            CollectionAssert.IsEmpty(gotEdges);

            gotEdges = graph.InEdges(4);
            CollectionAssert.AreEqual(new[] { edge5 }, gotEdges);

            gotEdges = graph.InEdges(2);
            CollectionAssert.AreEqual(new[] { edge1, edge2, edge4 }, gotEdges);
        }

        protected static void GetInEdges_ImmutableVertices_Test(
            [NotNull] BidirectionalMatrixGraph<IEdge<int>> graph)
        {
            var edge1 = Edge.Create(1, 2);
            var edge2 = Edge.Create(1, 3);
            var edge3 = Edge.Create(2, 2);
            var edge4 = Edge.Create(2, 4);
            var edge5 = Edge.Create(3, 1);
            var edge6 = Edge.Create(5, 3);

            graph.AddEdgeRange( edge1, edge2, edge3, edge4, edge5, edge6 );

            Assert.IsNull(graph.InEdges(6));

            var gotEdges = graph.InEdges(5);
            CollectionAssert.IsEmpty(gotEdges);

            gotEdges = graph.InEdges(4);
            CollectionAssert.AreEqual(new[] { edge4 }, gotEdges);

            gotEdges = graph.InEdges(2);
            CollectionAssert.AreEqual(new[] { edge1, edge3 }, gotEdges);
        }

        protected static void GetInEdges_ImmutableGraph_ReversedTest(
            [NotNull] IMutableVertexAndEdgeSet<int, IEdge<int>> wrappedGraph,
            [NotNull, InstantHandle] Func<IBidirectionalIncidenceGraph<int, SReversedEdge<int, IEdge<int>>>> createGraph)
        {
            var edge1 = Edge.Create(1, 2);
            var edge2 = Edge.Create(1, 2);
            var edge3 = Edge.Create(1, 3);
            var edge4 = Edge.Create(1, 1);
            var edge5 = Edge.Create(2, 4);
            var edge6 = Edge.Create(3, 1);
            var edge7 = Edge.Create(4, 5);

            wrappedGraph.AddVerticesAndEdgeRange( edge1, edge2, edge3, edge4, edge5, edge6, edge7 );
            IBidirectionalIncidenceGraph<int, SReversedEdge<int, IEdge<int>>> graph = createGraph();

            Assert.IsNull(graph.InEdges(0));

            var gotEdges = graph.InEdges(5);
            CollectionAssert.IsEmpty(gotEdges);

            gotEdges = graph.InEdges(4);
            AssertSameReversedEdges(new[] { edge7 }, gotEdges);

            gotEdges = graph.InEdges(1);
            AssertSameReversedEdges(new[] { edge1, edge2, edge3, edge4 }, gotEdges);
        }

        protected static void GetInEdges_Throws_Test<TVertex, TEdge>(
            [NotNull] IBidirectionalIncidenceGraph<TVertex, TEdge> graph)
            where TVertex : class
            where TEdge : IEdge<TVertex>
        {
            // ReSharper disable once AssignNullToNotNullAttribute
            Assert.Throws<ArgumentNullException>(() => graph.InEdges(null));
        }

        #endregion
    }
}