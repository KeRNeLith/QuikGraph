using System;
using JetBrains.Annotations;
using NUnit.Framework;

namespace QuikGraph.Tests.Structures
{
    internal partial class GraphTestsBase
    {
        #region Contains Edge

        protected static void ContainsEdge_Test(
            [NotNull] IEdgeSet<int, IEdge<int>> graph,
            [NotNull, InstantHandle] Action<IEdge<int>> addVerticesAndEdge)
        {
            var edge1 = Edge.Create(1, 2);
            var edge2 = Edge.Create(1, 3);
            var edge3 = Edge.Create(2, 1);
            var edge4 = Edge.Create(2, 2);
            var otherEdge1 = Edge.Create(1, 2);

            Assert.IsFalse(graph.ContainsEdge(edge1));
            Assert.IsFalse(graph.ContainsEdge(edge2));
            Assert.IsFalse(graph.ContainsEdge(edge3));
            Assert.IsFalse(graph.ContainsEdge(edge4));
            Assert.IsFalse(graph.ContainsEdge(otherEdge1));

            addVerticesAndEdge(edge1);
            Assert.IsTrue(graph.ContainsEdge(edge1));
            Assert.IsFalse(graph.ContainsEdge(edge2));
            Assert.IsFalse(graph.ContainsEdge(edge3));
            Assert.IsFalse(graph.ContainsEdge(edge4));
            Assert.IsFalse(graph.ContainsEdge(otherEdge1));

            addVerticesAndEdge(edge2);
            Assert.IsTrue(graph.ContainsEdge(edge1));
            Assert.IsTrue(graph.ContainsEdge(edge2));
            Assert.IsFalse(graph.ContainsEdge(edge3));
            Assert.IsFalse(graph.ContainsEdge(edge4));
            Assert.IsFalse(graph.ContainsEdge(otherEdge1));

            addVerticesAndEdge(edge3);
            Assert.IsTrue(graph.ContainsEdge(edge1));
            Assert.IsTrue(graph.ContainsEdge(edge2));
            Assert.IsTrue(graph.ContainsEdge(edge3));
            Assert.IsFalse(graph.ContainsEdge(edge4));
            Assert.IsFalse(graph.ContainsEdge(otherEdge1));

            addVerticesAndEdge(edge4);
            Assert.IsTrue(graph.ContainsEdge(edge1));
            Assert.IsTrue(graph.ContainsEdge(edge2));
            Assert.IsTrue(graph.ContainsEdge(edge3));
            Assert.IsTrue(graph.ContainsEdge(edge4));
            Assert.IsFalse(graph.ContainsEdge(otherEdge1));

            addVerticesAndEdge(otherEdge1);
            Assert.IsTrue(graph.ContainsEdge(edge1));
            Assert.IsTrue(graph.ContainsEdge(edge2));
            Assert.IsTrue(graph.ContainsEdge(edge3));
            Assert.IsTrue(graph.ContainsEdge(edge4));
            Assert.IsTrue(graph.ContainsEdge(otherEdge1));

            // Both vertices not in graph
            Assert.IsFalse(graph.ContainsEdge(Edge.Create(0, 10)));
            // Source not in graph
            Assert.IsFalse(graph.ContainsEdge(Edge.Create(0, 1)));
            // Target not in graph
            Assert.IsFalse(graph.ContainsEdge(Edge.Create(1, 0)));
        }

        protected static void ContainsEdge_Test(
            [NotNull] IMutableVertexAndEdgeSet<int, IEdge<int>> graph)
        {
            ContainsEdge_Test(
                graph,
                edge => graph.AddVerticesAndEdge(edge));
        }

        protected static void ContainsEdge_ImmutableGraph_Test(
            [NotNull] IMutableVertexAndEdgeSet<int, IEdge<int>> wrappedGraph,
            [NotNull, InstantHandle] Func<IEdgeSet<int, IEdge<int>>> createGraph)
        {
            IEdgeSet<int, IEdge<int>> graph = createGraph();

            var edge1 = Edge.Create(1, 2);
            var edge2 = Edge.Create(1, 3);
            var edge3 = Edge.Create(2, 1);
            var edge4 = Edge.Create(2, 2);
            var otherEdge1 = Edge.Create(1, 2);

            Assert.IsFalse(graph.ContainsEdge(edge1));
            Assert.IsFalse(graph.ContainsEdge(edge2));
            Assert.IsFalse(graph.ContainsEdge(edge3));
            Assert.IsFalse(graph.ContainsEdge(edge4));
            Assert.IsFalse(graph.ContainsEdge(otherEdge1));

            wrappedGraph.AddVerticesAndEdge(edge1);
            graph = createGraph();
            Assert.IsTrue(graph.ContainsEdge(edge1));
            Assert.IsFalse(graph.ContainsEdge(edge2));
            Assert.IsFalse(graph.ContainsEdge(edge3));
            Assert.IsFalse(graph.ContainsEdge(edge4));
            Assert.IsFalse(graph.ContainsEdge(otherEdge1));

            wrappedGraph.AddVerticesAndEdge(edge2);
            graph = createGraph();
            Assert.IsTrue(graph.ContainsEdge(edge1));
            Assert.IsTrue(graph.ContainsEdge(edge2));
            Assert.IsFalse(graph.ContainsEdge(edge3));
            Assert.IsFalse(graph.ContainsEdge(edge4));
            Assert.IsFalse(graph.ContainsEdge(otherEdge1));

            wrappedGraph.AddVerticesAndEdge(edge3);
            graph = createGraph();
            Assert.IsTrue(graph.ContainsEdge(edge1));
            Assert.IsTrue(graph.ContainsEdge(edge2));
            Assert.IsTrue(graph.ContainsEdge(edge3));
            Assert.IsFalse(graph.ContainsEdge(edge4));
            Assert.IsFalse(graph.ContainsEdge(otherEdge1));

            wrappedGraph.AddVerticesAndEdge(edge4);
            graph = createGraph();
            Assert.IsTrue(graph.ContainsEdge(edge1));
            Assert.IsTrue(graph.ContainsEdge(edge2));
            Assert.IsTrue(graph.ContainsEdge(edge3));
            Assert.IsTrue(graph.ContainsEdge(edge4));
            Assert.IsFalse(graph.ContainsEdge(otherEdge1));

            wrappedGraph.AddVerticesAndEdge(otherEdge1);
            graph = createGraph();
            Assert.IsTrue(graph.ContainsEdge(edge1));
            Assert.IsTrue(graph.ContainsEdge(edge2));
            Assert.IsTrue(graph.ContainsEdge(edge3));
            Assert.IsTrue(graph.ContainsEdge(edge4));
            Assert.IsTrue(graph.ContainsEdge(otherEdge1));

            // Both vertices not in graph
            Assert.IsFalse(graph.ContainsEdge(Edge.Create(0, 10)));
            // Source not in graph
            Assert.IsFalse(graph.ContainsEdge(Edge.Create(0, 1)));
            // Target not in graph
            Assert.IsFalse(graph.ContainsEdge(Edge.Create(1, 0)));
        }

        protected static void ContainsEdge_ImmutableGraph_Test(
            [NotNull] IMutableVertexAndEdgeSet<int, IEdge<int>> wrappedGraph,
            [NotNull, InstantHandle] Func<IEdgeSet<int, SEquatableEdge<int>>> createGraph)
        {
            IEdgeSet<int, SEquatableEdge<int>> graph = createGraph();

            var edge1 = Edge.Create(1, 2);
            var equatableEdge1 = new SEquatableEdge<int>(edge1.Source, edge1.Target);
            var edge2 = Edge.Create(1, 3);
            var equatableEdge2 = new SEquatableEdge<int>(edge2.Source, edge2.Target);
            var edge3 = Edge.Create(2, 1);
            var equatableEdge3 = new SEquatableEdge<int>(edge3.Source, edge3.Target);
            var edge4 = Edge.Create(2, 2);
            var equatableEdge4 = new SEquatableEdge<int>(edge4.Source, edge4.Target);
            var otherEdge1 = Edge.Create(1, 2);
            var equatableOtherEdge1 = new SEquatableEdge<int>(otherEdge1.Source, otherEdge1.Target);

            Assert.IsFalse(graph.ContainsEdge(equatableEdge1));
            Assert.IsFalse(graph.ContainsEdge(equatableEdge2));
            Assert.IsFalse(graph.ContainsEdge(equatableEdge3));
            Assert.IsFalse(graph.ContainsEdge(equatableEdge4));
            Assert.IsFalse(graph.ContainsEdge(equatableOtherEdge1));

            wrappedGraph.AddVerticesAndEdge(edge1);
            graph = createGraph();
            Assert.IsTrue(graph.ContainsEdge(equatableEdge1));
            Assert.IsFalse(graph.ContainsEdge(equatableEdge2));
            Assert.IsFalse(graph.ContainsEdge(equatableEdge3));
            Assert.IsFalse(graph.ContainsEdge(equatableEdge4));
            Assert.IsTrue(graph.ContainsEdge(equatableOtherEdge1));

            wrappedGraph.AddVerticesAndEdge(edge2);
            graph = createGraph();
            Assert.IsTrue(graph.ContainsEdge(equatableEdge1));
            Assert.IsTrue(graph.ContainsEdge(equatableEdge2));
            Assert.IsFalse(graph.ContainsEdge(equatableEdge3));
            Assert.IsFalse(graph.ContainsEdge(equatableEdge4));
            Assert.IsTrue(graph.ContainsEdge(equatableOtherEdge1));

            wrappedGraph.AddVerticesAndEdge(edge3);
            graph = createGraph();
            Assert.IsTrue(graph.ContainsEdge(equatableEdge1));
            Assert.IsTrue(graph.ContainsEdge(equatableEdge2));
            Assert.IsTrue(graph.ContainsEdge(equatableEdge3));
            Assert.IsFalse(graph.ContainsEdge(equatableEdge4));
            Assert.IsTrue(graph.ContainsEdge(equatableOtherEdge1));

            wrappedGraph.AddVerticesAndEdge(edge4);
            graph = createGraph();
            Assert.IsTrue(graph.ContainsEdge(equatableEdge1));
            Assert.IsTrue(graph.ContainsEdge(equatableEdge2));
            Assert.IsTrue(graph.ContainsEdge(equatableEdge3));
            Assert.IsTrue(graph.ContainsEdge(equatableEdge4));
            Assert.IsTrue(graph.ContainsEdge(equatableOtherEdge1));

            wrappedGraph.AddVerticesAndEdge(otherEdge1);
            graph = createGraph();
            Assert.IsTrue(graph.ContainsEdge(equatableEdge1));
            Assert.IsTrue(graph.ContainsEdge(equatableEdge2));
            Assert.IsTrue(graph.ContainsEdge(equatableEdge3));
            Assert.IsTrue(graph.ContainsEdge(equatableEdge4));
            Assert.IsTrue(graph.ContainsEdge(equatableOtherEdge1));

            // Both vertices not in graph
            Assert.IsFalse(graph.ContainsEdge(new SEquatableEdge<int>(0, 10)));
            // Source not in graph
            Assert.IsFalse(graph.ContainsEdge(new SEquatableEdge<int>(0, 1)));
            // Target not in graph
            Assert.IsFalse(graph.ContainsEdge(new SEquatableEdge<int>(1, 0)));
        }

        protected static void ContainsEdge_EdgesOnly_Test(
            [NotNull] EdgeListGraph<int, IEdge<int>> graph)
        {
            var edge1 = Edge.Create(1, 2);
            var edge2 = Edge.Create(1, 3);
            var edge3 = Edge.Create(2, 1);
            var edge4 = Edge.Create(2, 2);
            var otherEdge1 = Edge.Create(1, 2);

            Assert.IsFalse(graph.ContainsEdge(edge1));
            Assert.IsFalse(graph.ContainsEdge(edge2));
            Assert.IsFalse(graph.ContainsEdge(edge3));
            Assert.IsFalse(graph.ContainsEdge(edge4));
            Assert.IsFalse(graph.ContainsEdge(otherEdge1));

            graph.AddVerticesAndEdge(edge1);
            Assert.IsTrue(graph.ContainsEdge(edge1));
            Assert.IsFalse(graph.ContainsEdge(edge2));
            Assert.IsFalse(graph.ContainsEdge(edge3));
            Assert.IsFalse(graph.ContainsEdge(edge4));
            Assert.IsFalse(graph.ContainsEdge(otherEdge1));

            graph.AddVerticesAndEdge(edge2);
            Assert.IsTrue(graph.ContainsEdge(edge1));
            Assert.IsTrue(graph.ContainsEdge(edge2));
            Assert.IsFalse(graph.ContainsEdge(edge3));
            Assert.IsFalse(graph.ContainsEdge(edge4));
            Assert.IsFalse(graph.ContainsEdge(otherEdge1));

            graph.AddVerticesAndEdge(edge3);
            Assert.IsTrue(graph.ContainsEdge(edge1));
            Assert.IsTrue(graph.ContainsEdge(edge2));
            Assert.IsTrue(graph.ContainsEdge(edge3));
            Assert.IsFalse(graph.ContainsEdge(edge4));
            Assert.IsFalse(graph.ContainsEdge(otherEdge1));

            graph.AddVerticesAndEdge(edge4);
            Assert.IsTrue(graph.ContainsEdge(edge1));
            Assert.IsTrue(graph.ContainsEdge(edge2));
            Assert.IsTrue(graph.ContainsEdge(edge3));
            Assert.IsTrue(graph.ContainsEdge(edge4));
            Assert.IsFalse(graph.ContainsEdge(otherEdge1));

            graph.AddVerticesAndEdge(otherEdge1);
            Assert.IsTrue(graph.ContainsEdge(edge1));
            Assert.IsTrue(graph.ContainsEdge(edge2));
            Assert.IsTrue(graph.ContainsEdge(edge3));
            Assert.IsTrue(graph.ContainsEdge(edge4));
            Assert.IsTrue(graph.ContainsEdge(otherEdge1));

            // Both vertices not in graph
            Assert.IsFalse(graph.ContainsEdge(Edge.Create(0, 10)));
            // Source not in graph
            Assert.IsFalse(graph.ContainsEdge(Edge.Create(0, 1)));
            // Target not in graph
            Assert.IsFalse(graph.ContainsEdge(Edge.Create(1, 0)));
        }

        protected static void ContainsEdge_ForbiddenParallelEdges_ImmutableVertices_Test(
            [NotNull] IMutableEdgeListGraph<int, IEdge<int>> graph)
        {
            var edge1 = Edge.Create(1, 2);
            var edge2 = Edge.Create(1, 3);
            var edge3 = Edge.Create(2, 1);
            var edge4 = Edge.Create(2, 2);
            var otherEdge1 = Edge.Create(1, 2);

            Assert.IsFalse(graph.ContainsEdge(edge1));
            Assert.IsFalse(graph.ContainsEdge(edge2));
            Assert.IsFalse(graph.ContainsEdge(edge3));
            Assert.IsFalse(graph.ContainsEdge(edge4));
            Assert.IsFalse(graph.ContainsEdge(otherEdge1));

            graph.AddEdge(edge1);
            Assert.IsTrue(graph.ContainsEdge(edge1));
            Assert.IsFalse(graph.ContainsEdge(edge2));
            Assert.IsFalse(graph.ContainsEdge(edge3));
            Assert.IsFalse(graph.ContainsEdge(edge4));
            Assert.IsTrue(graph.ContainsEdge(otherEdge1));

            graph.AddEdge(edge2);
            Assert.IsTrue(graph.ContainsEdge(edge1));
            Assert.IsTrue(graph.ContainsEdge(edge2));
            Assert.IsFalse(graph.ContainsEdge(edge3));
            Assert.IsFalse(graph.ContainsEdge(edge4));
            Assert.IsTrue(graph.ContainsEdge(otherEdge1));

            graph.AddEdge(edge3);
            Assert.IsTrue(graph.ContainsEdge(edge1));
            Assert.IsTrue(graph.ContainsEdge(edge2));
            Assert.IsTrue(graph.ContainsEdge(edge3));
            Assert.IsFalse(graph.ContainsEdge(edge4));
            Assert.IsTrue(graph.ContainsEdge(otherEdge1));

            graph.AddEdge(edge4);
            Assert.IsTrue(graph.ContainsEdge(edge1));
            Assert.IsTrue(graph.ContainsEdge(edge2));
            Assert.IsTrue(graph.ContainsEdge(edge3));
            Assert.IsTrue(graph.ContainsEdge(edge4));
            Assert.IsTrue(graph.ContainsEdge(otherEdge1));

            // Both vertices not in graph
            Assert.IsFalse(graph.ContainsEdge(Edge.Create(10, 11)));
            // Source not in graph
            Assert.IsFalse(graph.ContainsEdge(Edge.Create(10, 1)));
            // Target not in graph
            Assert.IsFalse(graph.ContainsEdge(Edge.Create(1, 10)));
        }

        protected static void ContainsEdge_ImmutableGraph_ReversedTest(
            [NotNull] IMutableVertexAndEdgeSet<int, IEdge<int>> wrappedGraph,
            [NotNull, InstantHandle] Func<IEdgeSet<int, SReversedEdge<int, IEdge<int>>>> createGraph)
        {
            IEdgeSet<int, SReversedEdge<int, IEdge<int>>> graph = createGraph();

            var edge1 = Edge.Create(1, 2);
            var reversedEdge1 = new SReversedEdge<int, IEdge<int>>(edge1);
            var edge2 = Edge.Create(1, 3);
            var reversedEdge2 = new SReversedEdge<int, IEdge<int>>(edge2);
            var edge3 = Edge.Create(2, 1);
            var reversedEdge3 = new SReversedEdge<int, IEdge<int>>(edge3);
            var edge4 = Edge.Create(2, 2);
            var reversedEdge4 = new SReversedEdge<int, IEdge<int>>(edge4);
            var otherEdge1 = Edge.Create(1, 2);
            var reversedOtherEdge1 = new SReversedEdge<int, IEdge<int>>(otherEdge1);

            Assert.IsFalse(graph.ContainsEdge(reversedEdge1));
            Assert.IsFalse(graph.ContainsEdge(reversedEdge2));
            Assert.IsFalse(graph.ContainsEdge(reversedEdge3));
            Assert.IsFalse(graph.ContainsEdge(reversedEdge4));
            Assert.IsFalse(graph.ContainsEdge(reversedOtherEdge1));

            wrappedGraph.AddVerticesAndEdge(edge1);
            graph = createGraph();
            Assert.IsTrue(graph.ContainsEdge(reversedEdge1));
            Assert.IsFalse(graph.ContainsEdge(reversedEdge2));
            Assert.IsFalse(graph.ContainsEdge(reversedEdge3));
            Assert.IsFalse(graph.ContainsEdge(reversedEdge4));
            Assert.IsFalse(graph.ContainsEdge(reversedOtherEdge1));

            wrappedGraph.AddVerticesAndEdge(edge2);
            graph = createGraph();
            Assert.IsTrue(graph.ContainsEdge(reversedEdge1));
            Assert.IsTrue(graph.ContainsEdge(reversedEdge2));
            Assert.IsFalse(graph.ContainsEdge(reversedEdge3));
            Assert.IsFalse(graph.ContainsEdge(reversedEdge4));
            Assert.IsFalse(graph.ContainsEdge(reversedOtherEdge1));

            wrappedGraph.AddVerticesAndEdge(edge3);
            graph = createGraph();
            Assert.IsTrue(graph.ContainsEdge(reversedEdge1));
            Assert.IsTrue(graph.ContainsEdge(reversedEdge2));
            Assert.IsTrue(graph.ContainsEdge(reversedEdge3));
            Assert.IsFalse(graph.ContainsEdge(reversedEdge4));
            Assert.IsFalse(graph.ContainsEdge(reversedOtherEdge1));

            wrappedGraph.AddVerticesAndEdge(edge4);
            graph = createGraph();
            Assert.IsTrue(graph.ContainsEdge(reversedEdge1));
            Assert.IsTrue(graph.ContainsEdge(reversedEdge2));
            Assert.IsTrue(graph.ContainsEdge(reversedEdge3));
            Assert.IsTrue(graph.ContainsEdge(reversedEdge4));
            Assert.IsFalse(graph.ContainsEdge(reversedOtherEdge1));

            wrappedGraph.AddVerticesAndEdge(otherEdge1);
            graph = createGraph();
            Assert.IsTrue(graph.ContainsEdge(reversedEdge1));
            Assert.IsTrue(graph.ContainsEdge(reversedEdge2));
            Assert.IsTrue(graph.ContainsEdge(reversedEdge3));
            Assert.IsTrue(graph.ContainsEdge(reversedEdge4));
            Assert.IsTrue(graph.ContainsEdge(reversedOtherEdge1));

            // Both vertices not in graph
            Assert.IsFalse(
                graph.ContainsEdge(
                    new SReversedEdge<int, IEdge<int>>(
                        Edge.Create(0, 10))));
            // Source not in graph
            Assert.IsFalse(
                graph.ContainsEdge(
                    new SReversedEdge<int, IEdge<int>>(
                        Edge.Create(0, 1))));
            // Target not in graph
            Assert.IsFalse(
                graph.ContainsEdge(
                    new SReversedEdge<int, IEdge<int>>(
                        Edge.Create(1, 0))));
        }

        protected static void ContainsEdge_EquatableEdge_Test(
            [NotNull] IEdgeSet<int, EquatableEdge<int>> graph,
            [NotNull, InstantHandle] Action<EquatableEdge<int>> addVerticesAndEdge)
        {
            var edge1 = new EquatableEdge<int>(1, 2);
            var edge2 = new EquatableEdge<int>(1, 3);
            var edge3 = new EquatableEdge<int>(2, 1);
            var edge4 = new EquatableEdge<int>(2, 2);
            var otherEdge1 = new EquatableEdge<int>(1, 2);

            Assert.IsFalse(graph.ContainsEdge(edge1));
            Assert.IsFalse(graph.ContainsEdge(edge2));
            Assert.IsFalse(graph.ContainsEdge(edge3));
            Assert.IsFalse(graph.ContainsEdge(edge4));
            Assert.IsFalse(graph.ContainsEdge(otherEdge1));

            addVerticesAndEdge(edge1);
            Assert.IsTrue(graph.ContainsEdge(edge1));
            Assert.IsFalse(graph.ContainsEdge(edge2));
            Assert.IsFalse(graph.ContainsEdge(edge3));
            Assert.IsFalse(graph.ContainsEdge(edge4));
            Assert.IsTrue(graph.ContainsEdge(otherEdge1));

            addVerticesAndEdge(edge2);
            Assert.IsTrue(graph.ContainsEdge(edge1));
            Assert.IsTrue(graph.ContainsEdge(edge2));
            Assert.IsFalse(graph.ContainsEdge(edge3));
            Assert.IsFalse(graph.ContainsEdge(edge4));
            Assert.IsTrue(graph.ContainsEdge(otherEdge1));

            addVerticesAndEdge(edge3);
            Assert.IsTrue(graph.ContainsEdge(edge1));
            Assert.IsTrue(graph.ContainsEdge(edge2));
            Assert.IsTrue(graph.ContainsEdge(edge3));
            Assert.IsFalse(graph.ContainsEdge(edge4));
            Assert.IsTrue(graph.ContainsEdge(otherEdge1));

            addVerticesAndEdge(edge4);
            Assert.IsTrue(graph.ContainsEdge(edge1));
            Assert.IsTrue(graph.ContainsEdge(edge2));
            Assert.IsTrue(graph.ContainsEdge(edge3));
            Assert.IsTrue(graph.ContainsEdge(edge4));
            Assert.IsTrue(graph.ContainsEdge(otherEdge1));

            addVerticesAndEdge(otherEdge1);
            Assert.IsTrue(graph.ContainsEdge(edge1));
            Assert.IsTrue(graph.ContainsEdge(edge2));
            Assert.IsTrue(graph.ContainsEdge(edge3));
            Assert.IsTrue(graph.ContainsEdge(edge4));
            Assert.IsTrue(graph.ContainsEdge(otherEdge1));

            // Both vertices not in graph
            Assert.IsFalse(graph.ContainsEdge(new EquatableEdge<int>(0, 10)));
            // Source not in graph
            Assert.IsFalse(graph.ContainsEdge(new EquatableEdge<int>(0, 1)));
            // Target not in graph
            Assert.IsFalse(graph.ContainsEdge(new EquatableEdge<int>(1, 0)));
        }

        protected static void ContainsEdge_EquatableEdge_Test(
            [NotNull] IMutableVertexAndEdgeSet<int, EquatableEdge<int>> graph)
        {
            ContainsEdge_EquatableEdge_Test(
                graph,
                edge => graph.AddVerticesAndEdge(edge));
        }

        protected static void ContainsEdge_EquatableEdge_ImmutableGraph_Test(
            [NotNull] IMutableVertexAndEdgeSet<int, EquatableEdge<int>> wrappedGraph,
            [NotNull, InstantHandle] Func<IEdgeSet<int, EquatableEdge<int>>> createGraph)
        {
            IEdgeSet<int, EquatableEdge<int>> graph = createGraph();

            var edge1 = new EquatableEdge<int>(1, 2);
            var edge2 = new EquatableEdge<int>(1, 3);
            var edge3 = new EquatableEdge<int>(2, 1);
            var edge4 = new EquatableEdge<int>(2, 2);
            var otherEdge1 = new EquatableEdge<int>(1, 2);

            Assert.IsFalse(graph.ContainsEdge(edge1));
            Assert.IsFalse(graph.ContainsEdge(edge2));
            Assert.IsFalse(graph.ContainsEdge(edge3));
            Assert.IsFalse(graph.ContainsEdge(edge4));
            Assert.IsFalse(graph.ContainsEdge(otherEdge1));

            wrappedGraph.AddVerticesAndEdge(edge1);
            graph = createGraph();
            Assert.IsTrue(graph.ContainsEdge(edge1));
            Assert.IsFalse(graph.ContainsEdge(edge2));
            Assert.IsFalse(graph.ContainsEdge(edge3));
            Assert.IsFalse(graph.ContainsEdge(edge4));
            Assert.IsTrue(graph.ContainsEdge(otherEdge1));

            wrappedGraph.AddVerticesAndEdge(edge2);
            graph = createGraph();
            Assert.IsTrue(graph.ContainsEdge(edge1));
            Assert.IsTrue(graph.ContainsEdge(edge2));
            Assert.IsFalse(graph.ContainsEdge(edge3));
            Assert.IsFalse(graph.ContainsEdge(edge4));
            Assert.IsTrue(graph.ContainsEdge(otherEdge1));

            wrappedGraph.AddVerticesAndEdge(edge3);
            graph = createGraph();
            Assert.IsTrue(graph.ContainsEdge(edge1));
            Assert.IsTrue(graph.ContainsEdge(edge2));
            Assert.IsTrue(graph.ContainsEdge(edge3));
            Assert.IsFalse(graph.ContainsEdge(edge4));
            Assert.IsTrue(graph.ContainsEdge(otherEdge1));

            wrappedGraph.AddVerticesAndEdge(edge4);
            graph = createGraph();
            Assert.IsTrue(graph.ContainsEdge(edge1));
            Assert.IsTrue(graph.ContainsEdge(edge2));
            Assert.IsTrue(graph.ContainsEdge(edge3));
            Assert.IsTrue(graph.ContainsEdge(edge4));
            Assert.IsTrue(graph.ContainsEdge(otherEdge1));

            wrappedGraph.AddVerticesAndEdge(otherEdge1);
            graph = createGraph();
            Assert.IsTrue(graph.ContainsEdge(edge1));
            Assert.IsTrue(graph.ContainsEdge(edge2));
            Assert.IsTrue(graph.ContainsEdge(edge3));
            Assert.IsTrue(graph.ContainsEdge(edge4));
            Assert.IsTrue(graph.ContainsEdge(otherEdge1));

            // Both vertices not in graph
            Assert.IsFalse(graph.ContainsEdge(new EquatableEdge<int>(0, 10)));
            // Source not in graph
            Assert.IsFalse(graph.ContainsEdge(new EquatableEdge<int>(0, 1)));
            // Target not in graph
            Assert.IsFalse(graph.ContainsEdge(new EquatableEdge<int>(1, 0)));
        }

        protected static void ContainsEdge_EquatableEdge_EdgesOnly_Test(
            [NotNull] EdgeListGraph<int, EquatableEdge<int>> graph)
        {
            var edge1 = new EquatableEdge<int>(1, 2);
            var edge2 = new EquatableEdge<int>(1, 3);
            var edge3 = new EquatableEdge<int>(2, 1);
            var edge4 = new EquatableEdge<int>(2, 2);
            var otherEdge1 = new EquatableEdge<int>(1, 2);

            Assert.IsFalse(graph.ContainsEdge(edge1));
            Assert.IsFalse(graph.ContainsEdge(edge2));
            Assert.IsFalse(graph.ContainsEdge(edge3));
            Assert.IsFalse(graph.ContainsEdge(edge4));
            Assert.IsFalse(graph.ContainsEdge(otherEdge1));

            graph.AddVerticesAndEdge(edge1);
            Assert.IsTrue(graph.ContainsEdge(edge1));
            Assert.IsFalse(graph.ContainsEdge(edge2));
            Assert.IsFalse(graph.ContainsEdge(edge3));
            Assert.IsFalse(graph.ContainsEdge(edge4));
            Assert.IsTrue(graph.ContainsEdge(otherEdge1));

            graph.AddVerticesAndEdge(edge2);
            Assert.IsTrue(graph.ContainsEdge(edge1));
            Assert.IsTrue(graph.ContainsEdge(edge2));
            Assert.IsFalse(graph.ContainsEdge(edge3));
            Assert.IsFalse(graph.ContainsEdge(edge4));
            Assert.IsTrue(graph.ContainsEdge(otherEdge1));

            graph.AddVerticesAndEdge(edge3);
            Assert.IsTrue(graph.ContainsEdge(edge1));
            Assert.IsTrue(graph.ContainsEdge(edge2));
            Assert.IsTrue(graph.ContainsEdge(edge3));
            Assert.IsFalse(graph.ContainsEdge(edge4));
            Assert.IsTrue(graph.ContainsEdge(otherEdge1));

            graph.AddVerticesAndEdge(edge4);
            Assert.IsTrue(graph.ContainsEdge(edge1));
            Assert.IsTrue(graph.ContainsEdge(edge2));
            Assert.IsTrue(graph.ContainsEdge(edge3));
            Assert.IsTrue(graph.ContainsEdge(edge4));
            Assert.IsTrue(graph.ContainsEdge(otherEdge1));

            graph.AddVerticesAndEdge(otherEdge1);
            Assert.IsTrue(graph.ContainsEdge(edge1));
            Assert.IsTrue(graph.ContainsEdge(edge2));
            Assert.IsTrue(graph.ContainsEdge(edge3));
            Assert.IsTrue(graph.ContainsEdge(edge4));
            Assert.IsTrue(graph.ContainsEdge(otherEdge1));

            // Both vertices not in graph
            Assert.IsFalse(graph.ContainsEdge(new EquatableEdge<int>(0, 10)));
            // Source not in graph
            Assert.IsFalse(graph.ContainsEdge(new EquatableEdge<int>(0, 1)));
            // Target not in graph
            Assert.IsFalse(graph.ContainsEdge(new EquatableEdge<int>(1, 0)));
        }

        protected static void ContainsEdge_EquatableEdges_ForbiddenParallelEdges_ImmutableVertices_Test(
            [NotNull] IMutableEdgeListGraph<int, EquatableEdge<int>> graph)
        {
            var edge1 = new EquatableEdge<int>(1, 2);
            var edge2 = new EquatableEdge<int>(1, 3);
            var edge3 = new EquatableEdge<int>(2, 1);
            var edge4 = new EquatableEdge<int>(2, 2);
            var otherEdge1 = new EquatableEdge<int>(1, 2);

            Assert.IsFalse(graph.ContainsEdge(edge1));
            Assert.IsFalse(graph.ContainsEdge(edge2));
            Assert.IsFalse(graph.ContainsEdge(edge3));
            Assert.IsFalse(graph.ContainsEdge(edge4));
            Assert.IsFalse(graph.ContainsEdge(otherEdge1));

            graph.AddEdge(edge1);
            Assert.IsTrue(graph.ContainsEdge(edge1));
            Assert.IsFalse(graph.ContainsEdge(edge2));
            Assert.IsFalse(graph.ContainsEdge(edge3));
            Assert.IsFalse(graph.ContainsEdge(edge4));
            Assert.IsTrue(graph.ContainsEdge(otherEdge1));

            graph.AddEdge(edge2);
            Assert.IsTrue(graph.ContainsEdge(edge1));
            Assert.IsTrue(graph.ContainsEdge(edge2));
            Assert.IsFalse(graph.ContainsEdge(edge3));
            Assert.IsFalse(graph.ContainsEdge(edge4));
            Assert.IsTrue(graph.ContainsEdge(otherEdge1));

            graph.AddEdge(edge3);
            Assert.IsTrue(graph.ContainsEdge(edge1));
            Assert.IsTrue(graph.ContainsEdge(edge2));
            Assert.IsTrue(graph.ContainsEdge(edge3));
            Assert.IsFalse(graph.ContainsEdge(edge4));
            Assert.IsTrue(graph.ContainsEdge(otherEdge1));

            graph.AddEdge(edge4);
            Assert.IsTrue(graph.ContainsEdge(edge1));
            Assert.IsTrue(graph.ContainsEdge(edge2));
            Assert.IsTrue(graph.ContainsEdge(edge3));
            Assert.IsTrue(graph.ContainsEdge(edge4));
            Assert.IsTrue(graph.ContainsEdge(otherEdge1));

            // Both vertices not in graph
            Assert.IsFalse(graph.ContainsEdge(new EquatableEdge<int>(10, 11)));
            // Source not in graph
            Assert.IsFalse(graph.ContainsEdge(new EquatableEdge<int>(10, 1)));
            // Target not in graph
            Assert.IsFalse(graph.ContainsEdge(new EquatableEdge<int>(1, 10)));
        }

        protected static void ContainsEdge_EquatableEdge_ImmutableGraph_ReversedTest(
            [NotNull] IMutableVertexAndEdgeSet<int, EquatableEdge<int>> wrappedGraph,
            [NotNull, InstantHandle] Func<IEdgeSet<int, SReversedEdge<int, EquatableEdge<int>>>> createGraph)
        {
            IEdgeSet<int, SReversedEdge<int, EquatableEdge<int>>> graph = createGraph();

            var edge1 = new EquatableEdge<int>(1, 2);
            var reversedEdge1 = new SReversedEdge<int, EquatableEdge<int>>(edge1);
            var edge2 = new EquatableEdge<int>(1, 3);
            var reversedEdge2 = new SReversedEdge<int, EquatableEdge<int>>(edge2);
            var edge3 = new EquatableEdge<int>(2, 1);
            var reversedEdge3 = new SReversedEdge<int, EquatableEdge<int>>(edge3);
            var edge4 = new EquatableEdge<int>(2, 2);
            var reversedEdge4 = new SReversedEdge<int, EquatableEdge<int>>(edge4);
            var otherEdge1 = new EquatableEdge<int>(1, 2);
            var reversedOtherEdge1 = new SReversedEdge<int, EquatableEdge<int>>(otherEdge1);

            Assert.IsFalse(graph.ContainsEdge(reversedEdge1));
            Assert.IsFalse(graph.ContainsEdge(reversedEdge2));
            Assert.IsFalse(graph.ContainsEdge(reversedEdge3));
            Assert.IsFalse(graph.ContainsEdge(reversedEdge4));
            Assert.IsFalse(graph.ContainsEdge(reversedOtherEdge1));

            wrappedGraph.AddVerticesAndEdge(edge1);
            graph = createGraph();
            Assert.IsTrue(graph.ContainsEdge(reversedEdge1));
            Assert.IsFalse(graph.ContainsEdge(reversedEdge2));
            Assert.IsFalse(graph.ContainsEdge(reversedEdge3));
            Assert.IsFalse(graph.ContainsEdge(reversedEdge4));
            Assert.IsTrue(graph.ContainsEdge(reversedOtherEdge1));

            wrappedGraph.AddVerticesAndEdge(edge2);
            graph = createGraph();
            Assert.IsTrue(graph.ContainsEdge(reversedEdge1));
            Assert.IsTrue(graph.ContainsEdge(reversedEdge2));
            Assert.IsFalse(graph.ContainsEdge(reversedEdge3));
            Assert.IsFalse(graph.ContainsEdge(reversedEdge4));
            Assert.IsTrue(graph.ContainsEdge(reversedOtherEdge1));

            wrappedGraph.AddVerticesAndEdge(edge3);
            graph = createGraph();
            Assert.IsTrue(graph.ContainsEdge(reversedEdge1));
            Assert.IsTrue(graph.ContainsEdge(reversedEdge2));
            Assert.IsTrue(graph.ContainsEdge(reversedEdge3));
            Assert.IsFalse(graph.ContainsEdge(reversedEdge4));
            Assert.IsTrue(graph.ContainsEdge(reversedOtherEdge1));

            wrappedGraph.AddVerticesAndEdge(edge4);
            graph = createGraph();
            Assert.IsTrue(graph.ContainsEdge(reversedEdge1));
            Assert.IsTrue(graph.ContainsEdge(reversedEdge2));
            Assert.IsTrue(graph.ContainsEdge(reversedEdge3));
            Assert.IsTrue(graph.ContainsEdge(reversedEdge4));
            Assert.IsTrue(graph.ContainsEdge(reversedOtherEdge1));

            wrappedGraph.AddVerticesAndEdge(otherEdge1);
            graph = createGraph();
            Assert.IsTrue(graph.ContainsEdge(reversedEdge1));
            Assert.IsTrue(graph.ContainsEdge(reversedEdge2));
            Assert.IsTrue(graph.ContainsEdge(reversedEdge3));
            Assert.IsTrue(graph.ContainsEdge(reversedEdge4));
            Assert.IsTrue(graph.ContainsEdge(reversedOtherEdge1));

            // Both vertices not in graph
            Assert.IsFalse(
                graph.ContainsEdge(
                    new SReversedEdge<int, EquatableEdge<int>>(
                        new EquatableEdge<int>(0, 10))));
            // Source not in graph
            Assert.IsFalse(
                graph.ContainsEdge(
                    new SReversedEdge<int, EquatableEdge<int>>(
                        new EquatableEdge<int>(0, 1))));
            // Target not in graph
            Assert.IsFalse(
                graph.ContainsEdge(
                    new SReversedEdge<int, EquatableEdge<int>>(
                        new EquatableEdge<int>(1, 0))));
        }

        protected static void ContainsEdge_SourceTarget_Test(
            [NotNull] IIncidenceGraph<int, IEdge<int>> graph,
            [NotNull, InstantHandle] Action<IEdge<int>> addVerticesAndEdge)
        {
            var edge1 = Edge.Create(1, 2);
            var edge2 = Edge.Create(1, 3);
            var edge3 = Edge.Create(2, 2);

            Assert.IsFalse(graph.ContainsEdge(1, 2));
            Assert.IsFalse(graph.ContainsEdge(2, 1));

            addVerticesAndEdge(edge1);
            Assert.IsTrue(graph.ContainsEdge(1, 2));
            Assert.IsFalse(graph.ContainsEdge(2, 1));

            addVerticesAndEdge(edge2);
            Assert.IsTrue(graph.ContainsEdge(1, 3));
            Assert.IsFalse(graph.ContainsEdge(3, 1));

            addVerticesAndEdge(edge3);
            Assert.IsTrue(graph.ContainsEdge(2, 2));

            // Vertices is not present in the graph
            Assert.IsFalse(graph.ContainsEdge(0, 4));
            Assert.IsFalse(graph.ContainsEdge(1, 4));
            Assert.IsFalse(graph.ContainsEdge(4, 1));
        }

        protected static void ContainsEdge_SourceTarget_Test(
            [NotNull] IMutableVertexAndEdgeListGraph<int, IEdge<int>> graph)
        {
            ContainsEdge_SourceTarget_Test(
                graph,
                edge => graph.AddVerticesAndEdge(edge));
        }

        protected static void ContainsEdge_SourceTarget_ImmutableGraph_Test<TEdge>(
            [NotNull] IMutableVertexAndEdgeSet<int, IEdge<int>> wrappedGraph,
            [NotNull, InstantHandle] Func<IIncidenceGraph<int, TEdge>> createGraph)
            where TEdge : IEdge<int>
        {
            IIncidenceGraph<int, TEdge> graph = createGraph();

            var edge1 = Edge.Create(1, 2);
            var edge2 = Edge.Create(1, 3);
            var edge3 = Edge.Create(2, 2);

            Assert.IsFalse(graph.ContainsEdge(1, 2));
            Assert.IsFalse(graph.ContainsEdge(2, 1));

            wrappedGraph.AddVerticesAndEdge(edge1);
            graph = createGraph();
            Assert.IsTrue(graph.ContainsEdge(1, 2));
            Assert.IsFalse(graph.ContainsEdge(2, 1));

            wrappedGraph.AddVerticesAndEdge(edge2);
            graph = createGraph();
            Assert.IsTrue(graph.ContainsEdge(1, 3));
            Assert.IsFalse(graph.ContainsEdge(3, 1));

            wrappedGraph.AddVerticesAndEdge(edge3);
            graph = createGraph();
            Assert.IsTrue(graph.ContainsEdge(2, 2));

            // Vertices is not present in the graph
            Assert.IsFalse(graph.ContainsEdge(0, 4));
            Assert.IsFalse(graph.ContainsEdge(1, 4));
            Assert.IsFalse(graph.ContainsEdge(4, 1));
        }

        protected static void ContainsEdge_SourceTarget_ForbiddenParallelEdges_Test(
            [NotNull] BidirectionalMatrixGraph<IEdge<int>> graph)
        {
            var edge1 = Edge.Create(1, 2);
            var edge2 = Edge.Create(1, 3);
            var edge3 = Edge.Create(2, 2);

            Assert.IsFalse(graph.ContainsEdge(1, 2));
            Assert.IsFalse(graph.ContainsEdge(2, 1));

            graph.AddEdge(edge1);
            Assert.IsTrue(graph.ContainsEdge(1, 2));
            Assert.IsFalse(graph.ContainsEdge(2, 1));

            graph.AddEdge(edge2);
            Assert.IsTrue(graph.ContainsEdge(1, 3));
            Assert.IsFalse(graph.ContainsEdge(3, 1));

            graph.AddEdge(edge3);
            Assert.IsTrue(graph.ContainsEdge(2, 2));

            // Vertices is not present in the graph
            Assert.IsFalse(graph.ContainsEdge(4, 5));
            Assert.IsFalse(graph.ContainsEdge(1, 4));
            Assert.IsFalse(graph.ContainsEdge(4, 1));
        }

        protected static void ContainsEdge_SourceTarget_ImmutableGraph_ReversedTest(
            [NotNull] IMutableVertexAndEdgeSet<int, IEdge<int>> wrappedGraph,
            [NotNull, InstantHandle] Func<IIncidenceGraph<int, SReversedEdge<int, IEdge<int>>>> createGraph)
        {
            IIncidenceGraph<int, SReversedEdge<int, IEdge<int>>> graph = createGraph();

            var edge1 = Edge.Create(1, 2);
            var edge2 = Edge.Create(1, 3);
            var edge3 = Edge.Create(2, 2);

            Assert.IsFalse(graph.ContainsEdge(1, 2));
            Assert.IsFalse(graph.ContainsEdge(2, 1));

            wrappedGraph.AddVerticesAndEdge(edge1);
            graph = createGraph();
            Assert.IsFalse(graph.ContainsEdge(1, 2));
            Assert.IsTrue(graph.ContainsEdge(2, 1));

            wrappedGraph.AddVerticesAndEdge(edge2);
            graph = createGraph();
            Assert.IsFalse(graph.ContainsEdge(1, 3));
            Assert.IsTrue(graph.ContainsEdge(3, 1));

            wrappedGraph.AddVerticesAndEdge(edge3);
            graph = createGraph();
            Assert.IsTrue(graph.ContainsEdge(2, 2));

            // Vertices is not present in the graph
            Assert.IsFalse(graph.ContainsEdge(0, 4));
            Assert.IsFalse(graph.ContainsEdge(1, 4));
            Assert.IsFalse(graph.ContainsEdge(4, 1));
        }

        protected static void ContainsEdge_SourceTarget_UndirectedGraph_Test(
            [NotNull] IMutableUndirectedGraph<int, IEdge<int>> graph)
        {
            var edge1 = Edge.Create(1, 2);
            var edge2 = Edge.Create(1, 3);
            var edge3 = Edge.Create(2, 2);

            Assert.IsFalse(graph.ContainsEdge(1, 2));
            Assert.IsFalse(graph.ContainsEdge(2, 1));

            graph.AddVerticesAndEdge(edge1);
            Assert.IsTrue(graph.ContainsEdge(1, 2));
            Assert.IsTrue(graph.ContainsEdge(2, 1));

            graph.AddVerticesAndEdge(edge2);
            Assert.IsTrue(graph.ContainsEdge(1, 3));
            Assert.IsTrue(graph.ContainsEdge(3, 1));

            graph.AddVerticesAndEdge(edge3);
            Assert.IsTrue(graph.ContainsEdge(2, 2));

            // Vertices is not present in the graph
            Assert.IsFalse(graph.ContainsEdge(0, 4));
            Assert.IsFalse(graph.ContainsEdge(1, 4));
            Assert.IsFalse(graph.ContainsEdge(4, 1));
        }

        protected static void ContainsEdge_SourceTarget_ImmutableGraph_UndirectedGraph_Test(
            [NotNull] IMutableVertexAndEdgeSet<int, IEdge<int>> wrappedGraph,
            [NotNull, InstantHandle] Func<IImplicitUndirectedGraph<int, IEdge<int>>> createGraph)
        {
            var edge1 = Edge.Create(1, 2);
            var edge2 = Edge.Create(1, 3);
            var edge3 = Edge.Create(2, 2);

            IImplicitUndirectedGraph<int, IEdge<int>> graph = createGraph();
            Assert.IsFalse(graph.ContainsEdge(1, 2));
            Assert.IsFalse(graph.ContainsEdge(2, 1));

            wrappedGraph.AddVerticesAndEdge(edge1);
            graph = createGraph();
            Assert.IsTrue(graph.ContainsEdge(1, 2));
            Assert.IsTrue(graph.ContainsEdge(2, 1));

            wrappedGraph.AddVerticesAndEdge(edge2);
            graph = createGraph();
            Assert.IsTrue(graph.ContainsEdge(1, 3));
            Assert.IsTrue(graph.ContainsEdge(3, 1));

            wrappedGraph.AddVerticesAndEdge(edge3);
            graph = createGraph();
            Assert.IsTrue(graph.ContainsEdge(2, 2));

            // Vertices is not present in the graph
            Assert.IsFalse(graph.ContainsEdge(0, 4));
            Assert.IsFalse(graph.ContainsEdge(1, 4));
            Assert.IsFalse(graph.ContainsEdge(4, 1));
        }

        protected static void ContainsEdge_NullThrows_Test<TVertex, TEdge>(
            [NotNull] IEdgeSet<TVertex, TEdge> graph)
            where TEdge : IEdge<TVertex>
        {
            // ReSharper disable once AssignNullToNotNullAttribute
            // ReSharper disable once ReturnValueOfPureMethodIsNotUsed
            Assert.Throws<ArgumentNullException>(() => graph.ContainsEdge(default));
        }

        protected static void ContainsEdge_DefaultNullThrows_Test<TVertex>(
            [NotNull] IEdgeSet<TVertex, SEquatableEdge<TVertex>> graph)
        {
            // ReSharper disable once AssignNullToNotNullAttribute
            // ReSharper disable once ReturnValueOfPureMethodIsNotUsed
            Assert.Throws<ArgumentNullException>(() => graph.ContainsEdge(default));
        }

        protected static void ContainsEdge_NullThrows_ReversedTest<TVertex, TEdge>(
            [NotNull] IEdgeSet<TVertex, SReversedEdge<TVertex, TEdge>> graph)
            where TEdge : IEdge<TVertex>
        {
            // ReSharper disable once AssignNullToNotNullAttribute
            // ReSharper disable once ReturnValueOfPureMethodIsNotUsed
            Assert.Throws<ArgumentNullException>(() => graph.ContainsEdge(default));
        }

        protected static void ContainsEdge_SourceTarget_Throws_Test<TVertex, TEdge>(
            [NotNull] IIncidenceGraph<TVertex, TEdge> graph)
            where TVertex : class, new()
            where TEdge : IEdge<TVertex>
        {
            // ReSharper disable ReturnValueOfPureMethodIsNotUsed
            // ReSharper disable AssignNullToNotNullAttribute
            Assert.Throws<ArgumentNullException>(() => graph.ContainsEdge(new TVertex(), null));
            Assert.Throws<ArgumentNullException>(() => graph.ContainsEdge(null, new TVertex()));
            Assert.Throws<ArgumentNullException>(() => graph.ContainsEdge(null, null));
            // ReSharper restore AssignNullToNotNullAttribute
            // ReSharper restore ReturnValueOfPureMethodIsNotUsed
        }

        protected static void ContainsEdge_SourceTarget_Throws_UndirectedGraph_Test<TVertex, TEdge>(
            [NotNull] IImplicitUndirectedGraph<TVertex, TEdge> graph)
            where TVertex : class, new()
            where TEdge : IEdge<TVertex>
        {
            // ReSharper disable ReturnValueOfPureMethodIsNotUsed
            // ReSharper disable AssignNullToNotNullAttribute
            Assert.Throws<ArgumentNullException>(() => graph.ContainsEdge(new TVertex(), null));
            Assert.Throws<ArgumentNullException>(() => graph.ContainsEdge(null, new TVertex()));
            Assert.Throws<ArgumentNullException>(() => graph.ContainsEdge(null, null));
            // ReSharper restore AssignNullToNotNullAttribute
            // ReSharper restore ReturnValueOfPureMethodIsNotUsed
        }


        protected static void ContainsEdge_UndirectedEdge_UndirectedGraph_Test(
            [NotNull] IMutableUndirectedGraph<int, EquatableEdge<int>> graph1,
            [NotNull] IMutableUndirectedGraph<int, EquatableUndirectedEdge<int>> graph2)
        {
            ///////////////////////////////////
            // ContainsEdge => Source/Target //
            ///////////////////////////////////
            // Equatable Edge
            var equatableEdge1 = new EquatableEdge<int>(1, 2);
            var equatableEdge2 = new EquatableEdge<int>(1, 3);

            Assert.IsFalse(graph1.ContainsEdge(1, 2));
            Assert.IsFalse(graph1.ContainsEdge(2, 1));

            graph1.AddVerticesAndEdge(equatableEdge1);
            Assert.IsTrue(graph1.ContainsEdge(1, 2));
            Assert.IsTrue(graph1.ContainsEdge(2, 1));

            graph1.AddVerticesAndEdge(equatableEdge2);
            Assert.IsTrue(graph1.ContainsEdge(1, 3));
            Assert.IsTrue(graph1.ContainsEdge(3, 1));

            // Vertices is not present in the graph
            Assert.IsFalse(graph1.ContainsEdge(0, 4));
            Assert.IsFalse(graph1.ContainsEdge(1, 4));
            Assert.IsFalse(graph1.ContainsEdge(4, 1));



            // Undirected equatable edge
            var equatableUndirectedEdge1 = new EquatableUndirectedEdge<int>(1, 2);
            var equatableUndirectedEdge2 = new EquatableUndirectedEdge<int>(1, 3);

            Assert.IsFalse(graph2.ContainsEdge(1, 2));
            Assert.IsFalse(graph2.ContainsEdge(2, 1));

            graph2.AddVerticesAndEdge(equatableUndirectedEdge1);
            Assert.IsTrue(graph2.ContainsEdge(1, 2));
            Assert.IsTrue(graph2.ContainsEdge(2, 1));

            graph2.AddVerticesAndEdge(equatableUndirectedEdge2);
            Assert.IsTrue(graph2.ContainsEdge(1, 3));
            Assert.IsTrue(graph2.ContainsEdge(3, 1));

            // Vertices is not present in the graph
            Assert.IsFalse(graph2.ContainsEdge(0, 4));
            Assert.IsFalse(graph2.ContainsEdge(1, 4));
            Assert.IsFalse(graph2.ContainsEdge(4, 1));
        }

        protected static void ContainsEdge_UndirectedEdge_ImmutableGraph_UndirectedGraph_Test(
            [NotNull] IMutableVertexAndEdgeSet<int, EquatableEdge<int>> wrappedGraph1,
            [NotNull, InstantHandle] Func<IImplicitUndirectedGraph<int, EquatableEdge<int>>> createEquatableEdgeGraph,
            [NotNull] IMutableVertexAndEdgeSet<int, EquatableUndirectedEdge<int>> wrappedGraph2,
            [NotNull, InstantHandle] Func<IImplicitUndirectedGraph<int, EquatableUndirectedEdge<int>>> createEquatableUndirectedEdgeGraph)
        {
            ///////////////////////////////////
            // ContainsEdge => Source/Target //
            ///////////////////////////////////
            // Equatable Edge
            var equatableEdge1 = new EquatableEdge<int>(1, 2);
            var equatableEdge2 = new EquatableEdge<int>(1, 3);

            IImplicitUndirectedGraph<int, EquatableEdge<int>> graph1 = createEquatableEdgeGraph();
            Assert.IsFalse(graph1.ContainsEdge(1, 2));
            Assert.IsFalse(graph1.ContainsEdge(2, 1));

            wrappedGraph1.AddVerticesAndEdge(equatableEdge1);
            graph1 = createEquatableEdgeGraph();
            Assert.IsTrue(graph1.ContainsEdge(1, 2));
            Assert.IsTrue(graph1.ContainsEdge(2, 1));

            wrappedGraph1.AddVerticesAndEdge(equatableEdge2);
            graph1 = createEquatableEdgeGraph();
            Assert.IsTrue(graph1.ContainsEdge(1, 3));
            Assert.IsTrue(graph1.ContainsEdge(3, 1));

            // Vertices is not present in the graph
            Assert.IsFalse(graph1.ContainsEdge(0, 4));
            Assert.IsFalse(graph1.ContainsEdge(1, 4));
            Assert.IsFalse(graph1.ContainsEdge(4, 1));



            // Undirected equatable edge
            var equatableUndirectedEdge1 = new EquatableUndirectedEdge<int>(1, 2);
            var equatableUndirectedEdge2 = new EquatableUndirectedEdge<int>(1, 3);

            IImplicitUndirectedGraph<int, EquatableUndirectedEdge<int>> graph2 = createEquatableUndirectedEdgeGraph();
            Assert.IsFalse(graph2.ContainsEdge(1, 2));
            Assert.IsFalse(graph2.ContainsEdge(2, 1));

            wrappedGraph2.AddVerticesAndEdge(equatableUndirectedEdge1);
            graph2 = createEquatableUndirectedEdgeGraph();
            Assert.IsTrue(graph2.ContainsEdge(1, 2));
            Assert.IsTrue(graph2.ContainsEdge(2, 1));

            wrappedGraph2.AddVerticesAndEdge(equatableUndirectedEdge2);
            graph2 = createEquatableUndirectedEdgeGraph();
            Assert.IsTrue(graph2.ContainsEdge(1, 3));
            Assert.IsTrue(graph2.ContainsEdge(3, 1));

            // Vertices is not present in the graph
            Assert.IsFalse(graph2.ContainsEdge(0, 4));
            Assert.IsFalse(graph2.ContainsEdge(1, 4));
            Assert.IsFalse(graph2.ContainsEdge(4, 1));
        }

        #endregion
    }
}