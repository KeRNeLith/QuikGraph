using System;
using JetBrains.Annotations;
using NUnit.Framework;

namespace QuikGraph.Tests.Structures
{
    internal partial class GraphTestsBase
    {
        #region Contains Vertex

        protected static void ContainsVertex_Test(
            [NotNull] IMutableVertexSet<TestVertex> graph)
        {
            var vertex1 = new TestVertex("1");
            var vertex2 = new TestVertex("2");
            var otherVertex1 = new TestVertex("1");

            Assert.IsFalse(graph.ContainsVertex(vertex1));
            Assert.IsFalse(graph.ContainsVertex(vertex2));
            Assert.IsFalse(graph.ContainsVertex(otherVertex1));

            graph.AddVertex(vertex1);
            Assert.IsTrue(graph.ContainsVertex(vertex1));
            Assert.IsFalse(graph.ContainsVertex(otherVertex1));

            graph.AddVertex(vertex2);
            Assert.IsTrue(graph.ContainsVertex(vertex2));

            graph.AddVertex(otherVertex1);
            Assert.IsTrue(graph.ContainsVertex(vertex1));
            Assert.IsTrue(graph.ContainsVertex(otherVertex1));
        }

        protected static void ContainsVertex_ImmutableGraph_Test(
            [NotNull] IMutableVertexSet<TestVertex> wrappedGraph,
            [NotNull, InstantHandle] Func<IImplicitVertexSet<TestVertex>> createGraph)
        {
            IImplicitVertexSet<TestVertex> graph = createGraph();

            var vertex1 = new TestVertex("1");
            var vertex2 = new TestVertex("2");
            var otherVertex1 = new TestVertex("1");

            Assert.IsFalse(graph.ContainsVertex(vertex1));
            Assert.IsFalse(graph.ContainsVertex(vertex2));
            Assert.IsFalse(graph.ContainsVertex(otherVertex1));

            wrappedGraph.AddVertex(vertex1);
            graph = createGraph();
            Assert.IsTrue(graph.ContainsVertex(vertex1));
            Assert.IsFalse(graph.ContainsVertex(otherVertex1));

            wrappedGraph.AddVertex(vertex2);
            graph = createGraph();
            Assert.IsTrue(graph.ContainsVertex(vertex2));

            wrappedGraph.AddVertex(otherVertex1);
            graph = createGraph();
            Assert.IsTrue(graph.ContainsVertex(vertex1));
            Assert.IsTrue(graph.ContainsVertex(otherVertex1));
        }

        protected static void ContainsVertex_OnlyEdges_Test(
            [NotNull] EdgeListGraph<TestVertex, IEdge<TestVertex>> graph)
        {
            var vertex1 = new TestVertex("1");
            var toVertex1 = new TestVertex("target 1");
            var vertex2 = new TestVertex("2");
            var toVertex2 = new TestVertex("target 2");
            var otherVertex1 = new TestVertex("1");
            var toOtherVertex1 = new TestVertex("target 1");

            Assert.IsFalse(graph.ContainsVertex(vertex1));
            Assert.IsFalse(graph.ContainsVertex(vertex2));
            Assert.IsFalse(graph.ContainsVertex(otherVertex1));

            graph.AddEdge(Edge.Create(vertex1, toVertex1));
            Assert.IsTrue(graph.ContainsVertex(vertex1));
            Assert.IsFalse(graph.ContainsVertex(otherVertex1));

            graph.AddEdge(Edge.Create(vertex2, toVertex2));
            Assert.IsTrue(graph.ContainsVertex(vertex2));

            graph.AddEdge(Edge.Create(otherVertex1, toOtherVertex1));
            Assert.IsTrue(graph.ContainsVertex(vertex1));
            Assert.IsTrue(graph.ContainsVertex(otherVertex1));
        }

        protected static void ContainsVertex_EquatableVertex_Test(
            [NotNull] IMutableVertexSet<EquatableTestVertex> graph)
        {
            var vertex1 = new EquatableTestVertex("1");
            var vertex2 = new EquatableTestVertex("2");
            var otherVertex1 = new EquatableTestVertex("1");

            Assert.IsFalse(graph.ContainsVertex(vertex1));
            Assert.IsFalse(graph.ContainsVertex(vertex2));
            Assert.IsFalse(graph.ContainsVertex(otherVertex1));

            graph.AddVertex(vertex1);
            Assert.IsTrue(graph.ContainsVertex(vertex1));
            Assert.IsTrue(graph.ContainsVertex(otherVertex1));

            graph.AddVertex(vertex2);
            Assert.IsTrue(graph.ContainsVertex(vertex2));

            graph.AddVertex(otherVertex1);
            Assert.IsTrue(graph.ContainsVertex(vertex1));
            Assert.IsTrue(graph.ContainsVertex(otherVertex1));
        }

        protected static void ContainsVertex_EquatableVertex_ImmutableGraph_Test(
            [NotNull] IMutableVertexSet<EquatableTestVertex> wrappedGraph,
            [NotNull, InstantHandle] Func<IImplicitVertexSet<EquatableTestVertex>> createGraph)
        {
            IImplicitVertexSet<EquatableTestVertex> graph = createGraph();

            var vertex1 = new EquatableTestVertex("1");
            var vertex2 = new EquatableTestVertex("2");
            var otherVertex1 = new EquatableTestVertex("1");

            Assert.IsFalse(graph.ContainsVertex(vertex1));
            Assert.IsFalse(graph.ContainsVertex(vertex2));
            Assert.IsFalse(graph.ContainsVertex(otherVertex1));

            wrappedGraph.AddVertex(vertex1);
            graph = createGraph();
            Assert.IsTrue(graph.ContainsVertex(vertex1));
            Assert.IsTrue(graph.ContainsVertex(otherVertex1));

            wrappedGraph.AddVertex(vertex2);
            graph = createGraph();
            Assert.IsTrue(graph.ContainsVertex(vertex2));

            wrappedGraph.AddVertex(otherVertex1);
            graph = createGraph();
            Assert.IsTrue(graph.ContainsVertex(vertex1));
            Assert.IsTrue(graph.ContainsVertex(otherVertex1));
        }

        protected static void ContainsVertex_EquatableVertex_OnlyEdges_Test(
            [NotNull] EdgeListGraph<EquatableTestVertex, IEdge<EquatableTestVertex>> graph)
        {
            var vertex1 = new EquatableTestVertex("1");
            var toVertex1 = new EquatableTestVertex("target 1");
            var vertex2 = new EquatableTestVertex("2");
            var toVertex2 = new EquatableTestVertex("target 2");
            var otherVertex1 = new EquatableTestVertex("1");
            var toOtherVertex1 = new EquatableTestVertex("target 1");

            Assert.IsFalse(graph.ContainsVertex(vertex1));
            Assert.IsFalse(graph.ContainsVertex(vertex2));
            Assert.IsFalse(graph.ContainsVertex(otherVertex1));

            graph.AddEdge(Edge.Create(vertex1, toVertex1));
            Assert.IsTrue(graph.ContainsVertex(vertex1));
            Assert.IsTrue(graph.ContainsVertex(otherVertex1));

            graph.AddEdge(Edge.Create(vertex2, toVertex2));
            Assert.IsTrue(graph.ContainsVertex(vertex2));

            graph.AddEdge(Edge.Create(otherVertex1, toOtherVertex1));
            Assert.IsTrue(graph.ContainsVertex(vertex1));
            Assert.IsTrue(graph.ContainsVertex(otherVertex1));
        }

        protected static void ContainsVertex_Throws_Test<TVertex>(
            [NotNull] IImplicitVertexSet<TVertex> graph)
            where TVertex : class
        {
            // ReSharper disable once AssignNullToNotNullAttribute
            // ReSharper disable once ReturnValueOfPureMethodIsNotUsed
            Assert.Throws<ArgumentNullException>(() => graph.ContainsVertex(null));
        }

        #endregion
    }
}