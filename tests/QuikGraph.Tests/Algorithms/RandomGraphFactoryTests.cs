using System;
using System.Linq;
using NUnit.Framework;
using QuikGraph.Algorithms;
using QuikGraph.Tests.Structures;
using static QuikGraph.Tests.GraphTestHelpers;

namespace QuikGraph.Tests.Algorithms
{
    /// <summary>
    /// Tests related to <see cref="RandomGraphFactory"/>
    /// </summary>
    [TestFixture]
    internal sealed class RandomGraphFactoryTests : GraphTestsBase
    {
        [Test]
        public void GetVertex()
        {
            var graph = new AdjacencyGraph<int, IEdge<int>>();
            graph.AddVertexRange( 1, 2, 3, 4, 5 );

            int vertex = RandomGraphFactory.GetVertex(graph, new Random(123456));
            Assert.AreEqual(2, vertex);

            vertex = RandomGraphFactory.GetVertex(graph, new Random(456789));
            Assert.AreEqual(5, vertex);

            vertex = RandomGraphFactory.GetVertex(graph.Vertices, graph.VertexCount, new Random(123456));
            Assert.AreEqual(2, vertex);

            vertex = RandomGraphFactory.GetVertex(graph.Vertices, graph.VertexCount, new Random(456789));
            Assert.AreEqual(5, vertex);

            vertex = RandomGraphFactory.GetVertex(graph.Vertices, 3, new Random(123));
            Assert.AreEqual(3, vertex);
        }

        [Test]
        public void GetVertex_Throws()
        {
            var graph = new AdjacencyGraph<int, IEdge<int>>();
            var graph2 = new AdjacencyGraph<TestVertex, IEdge<TestVertex>>();
            var random = new Random();

            // ReSharper disable ReturnValueOfPureMethodIsNotUsed
            // ReSharper disable AssignNullToNotNullAttribute
            Assert.Throws<ArgumentNullException>(() => RandomGraphFactory.GetVertex<int>(null, random));
            Assert.Throws<ArgumentNullException>(() => RandomGraphFactory.GetVertex(graph2, null));
            Assert.Throws<ArgumentNullException>(() => RandomGraphFactory.GetVertex<int>(null, null));

            Assert.Throws<ArgumentNullException>(() => RandomGraphFactory.GetVertex<int>(null, 1, random));
            Assert.Throws<ArgumentNullException>(() => RandomGraphFactory.GetVertex(Enumerable.Empty<int>(), 1, null));
            Assert.Throws<ArgumentNullException>(() => RandomGraphFactory.GetVertex<int>(null, 1, null));
            // ReSharper restore AssignNullToNotNullAttribute
            Assert.Throws<ArgumentOutOfRangeException>(() => RandomGraphFactory.GetVertex(graph, random));

            Assert.Throws<ArgumentOutOfRangeException>(() => RandomGraphFactory.GetVertex(Enumerable.Empty<int>(), -1, random));
            Assert.Throws<ArgumentOutOfRangeException>(() => RandomGraphFactory.GetVertex(Enumerable.Empty<int>(), 0, random));
            Assert.Throws<InvalidOperationException>(() => RandomGraphFactory.GetVertex(Enumerable.Empty<int>(), 1, random));
            Assert.Throws<InvalidOperationException>(() => RandomGraphFactory.GetVertex(new[] { 1, 2 }, 10, new Random(123456)));
            // ReSharper restore ReturnValueOfPureMethodIsNotUsed
        }

        [Test]
        public void GetEdge()
        {
            var graph = new AdjacencyGraph<int, IEdge<int>>();
            var edge12 = Edge.Create(1, 2);
            var edge13 = Edge.Create(1, 3);
            var edge23 = Edge.Create(2, 3);
            var edge24 = Edge.Create(2, 4);
            var edge35 = Edge.Create(3, 5);
            graph.AddVerticesAndEdgeRange(
                edge12, edge13, edge23, edge24, edge35
            );

            IEdge<int> edge = RandomGraphFactory.GetEdge(graph, new Random(123456));
            Assert.AreSame(edge13, edge);

            edge = RandomGraphFactory.GetEdge(graph, new Random(456789));
            Assert.AreSame(edge35, edge);

            edge = RandomGraphFactory.GetEdge<int, IEdge<int>>(graph.Edges, graph.VertexCount, new Random(123456));
            Assert.AreSame(edge13, edge);

            edge = RandomGraphFactory.GetEdge<int, IEdge<int>>(graph.Edges, graph.VertexCount, new Random(456789));
            Assert.AreSame(edge35, edge);

            edge = RandomGraphFactory.GetEdge<int, IEdge<int>>(graph.Edges, 3, new Random(123));
            Assert.AreSame(edge23, edge);
        }

        [Test]
        public void GetEdge_Throws()
        {
            var graph = new AdjacencyGraph<int, IEdge<int>>();
            var graph2 = new AdjacencyGraph<TestVertex, IEdge<TestVertex>>();
            var random = new Random();

            // ReSharper disable ReturnValueOfPureMethodIsNotUsed
            // ReSharper disable AssignNullToNotNullAttribute
            Assert.Throws<ArgumentNullException>(() => RandomGraphFactory.GetEdge<int, IEdge<int>>(null, random));
            Assert.Throws<ArgumentNullException>(() => RandomGraphFactory.GetEdge(graph2, null));
            Assert.Throws<ArgumentNullException>(() => RandomGraphFactory.GetEdge<int, IEdge<int>>(null, null));

            Assert.Throws<ArgumentNullException>(() => RandomGraphFactory.GetEdge<int, IEdge<int>>(null, 1, random));
            Assert.Throws<ArgumentNullException>(() => RandomGraphFactory.GetEdge<int, IEdge<int>>(Enumerable.Empty<IEdge<int>>(), 1, null));
            Assert.Throws<ArgumentNullException>(() => RandomGraphFactory.GetEdge<int, IEdge<int>>(null, 1, null));
            // ReSharper restore AssignNullToNotNullAttribute
            Assert.Throws<ArgumentOutOfRangeException>(() => RandomGraphFactory.GetVertex(graph, random));

            Assert.Throws<ArgumentOutOfRangeException>(() => RandomGraphFactory.GetEdge<int, IEdge<int>>(Enumerable.Empty<IEdge<int>>(), -1, random));
            Assert.Throws<ArgumentOutOfRangeException>(() => RandomGraphFactory.GetEdge<int, IEdge<int>>(Enumerable.Empty<IEdge<int>>(), 0, random));
            Assert.Throws<InvalidOperationException>(() => RandomGraphFactory.GetEdge<int, IEdge<int>>(Enumerable.Empty<IEdge<int>>(), 1, random));
            Assert.Throws<InvalidOperationException>(
                () => RandomGraphFactory.GetEdge<int, IEdge<int>>(
                    new[] { Edge.Create(1, 2), Edge.Create(1, 3) },
                    10,
                    new Random(123456)));
            // ReSharper restore ReturnValueOfPureMethodIsNotUsed
        }

        [Test]
        public void Create()
        {
            var graph = new AdjacencyGraph<int , EquatableEdge<int>>();

            // With isolated vertices only
            int v = 0;
            RandomGraphFactory.Create(
                graph,
                () => ++v,
                (source, target) => new EquatableEdge<int>(source, target),
                new Random(123456),
                2,
                0,
                true);
            graph.AssertHasVertices(1, 2 );
            graph.AssertNoEdge();

            // With self edge
            v = 0;
            RandomGraphFactory.Create(
                graph,
                () => ++v,
                (source, target) => new EquatableEdge<int>(source, target),
                new Random(123456),
                5,
                10,
                true);
            graph.AssertHasVertices(1, 2, 3, 4, 5 );
            graph.AssertHasEdges(new EquatableEdge<int>[]
                {
                    new (1, 2),
                    new (1, 2),
                    new (1, 2),
                    new (2, 1),
                    new (3, 5),
                    new (3, 1),
                    new (4, 5),
                    new (4, 4),
                    new (4, 1),
                    new (5, 3)
                });

            // Without self edge
            graph.Clear();
            v = 0;
            RandomGraphFactory.Create(
                graph,
                () => ++v,
                (source, target) => new EquatableEdge<int>(source, target),
                new Random(123456),
                5,
                10,
                false);
            graph.AssertHasVertices(1, 2, 3, 4, 5 );
            graph.AssertHasEdges(
                new EquatableEdge<int>[]
                {
                    new (1, 2),
                    new (1, 2),
                    new (1, 2),
                    new (2, 1),
                    new (3, 5),
                    new (3, 1),
                    new (3, 1),
                    new (4, 5),
                    new (4, 1),
                    new (5, 3)
                });

            // Different seed change generated graph
            graph.Clear();
            v = 0;
            RandomGraphFactory.Create(
                graph,
                () => ++v,
                (source, target) => new EquatableEdge<int>(source, target),
                new Random(456789),
                5,
                10,
                true);
            graph.AssertHasVertices(1, 2, 3, 4, 5 );
            graph.AssertHasEdges(
                new EquatableEdge<int>[]
                {
                    new(1, 2),
                    new(2, 2),
                    new(2, 5),
                    new(3, 4),
                    new(3, 2),
                    new(4, 5),
                    new(4, 2),
                    new(4, 2),
                    new(5, 2),
                    new(5, 3)
                });

            // On non-empty graph, keep existing stuff
            graph.Clear();
            graph.AddVerticesAndEdge(new EquatableEdge<int>(6, 7));
            v = 0;
            RandomGraphFactory.Create(
                graph,
                () => ++v,
                (source, target) => new EquatableEdge<int>(source, target),
                new Random(123456),
                5,
                10,
                true);
            graph.AssertHasVertices(1, 2, 3, 4, 5, 6, 7 );
            graph.AssertHasEdges(
                new EquatableEdge<int>[]
                {
                    new (6, 7),
                    new (1, 2),
                    new (1, 2),
                    new (1, 2),
                    new (2, 1),
                    new (3, 5),
                    new (3, 1),
                    new (4, 5),
                    new (4, 4),
                    new (4, 1),
                    new (5, 3)
                });
        }

        [Test]
        public void Create_Throws()
        {
            var graph = new AdjacencyGraph<int, IEdge<int>>();
            var random = new Random();

            // ReSharper disable AssignNullToNotNullAttribute
            Assert.Throws<ArgumentNullException>(() => RandomGraphFactory.Create((IMutableVertexAndEdgeListGraph<int, IEdge<int>>)null, () => 1, Edge.Create, random, 1, 1, false));
            Assert.Throws<ArgumentNullException>(() => RandomGraphFactory.Create(graph, null, Edge.Create, random, 1, 1, false));
            Assert.Throws<ArgumentNullException>(() => RandomGraphFactory.Create(graph, () => 1, null, random, 1, 1, false));
            Assert.Throws<ArgumentNullException>(() => RandomGraphFactory.Create(graph, () => 1, Edge.Create, null, 1, 1, false));
            Assert.Throws<ArgumentNullException>(() => RandomGraphFactory.Create((IMutableVertexAndEdgeListGraph<int, IEdge<int>>)null, null, Edge.Create, random, 1, 1, false));
            Assert.Throws<ArgumentNullException>(() => RandomGraphFactory.Create((IMutableVertexAndEdgeListGraph<int, IEdge<int>>)null, () => 1, null, random, 1, 1, false));
            Assert.Throws<ArgumentNullException>(() => RandomGraphFactory.Create((IMutableVertexAndEdgeListGraph<int, IEdge<int>>)null, () => 1, Edge.Create, null, 1, 1, false));
            Assert.Throws<ArgumentNullException>(() => RandomGraphFactory.Create(graph, null, null, random, 1, 1, false));
            Assert.Throws<ArgumentNullException>(() => RandomGraphFactory.Create(graph, null, Edge.Create, null, 1, 1, false));
            Assert.Throws<ArgumentNullException>(() => RandomGraphFactory.Create(graph, () => 1, null, null, 1, 1, false));
            Assert.Throws<ArgumentNullException>(() => RandomGraphFactory.Create(graph, null, null, null, 1, 1, false));
            Assert.Throws<ArgumentNullException>(() => RandomGraphFactory.Create((IMutableVertexAndEdgeListGraph<int, IEdge<int>>)null, () => 1, null, null, 1, 1, false));
            Assert.Throws<ArgumentNullException>(() => RandomGraphFactory.Create((IMutableVertexAndEdgeListGraph<int, IEdge<int>>)null, null, Edge.Create, null, 1, 1, false));
            Assert.Throws<ArgumentNullException>(() => RandomGraphFactory.Create((IMutableVertexAndEdgeListGraph<int, IEdge<int>>)null, null, null, random, 1, 1, false));
            Assert.Throws<ArgumentNullException>(() => RandomGraphFactory.Create((IMutableVertexAndEdgeListGraph<int, IEdge<int>>)null, null, null, null, 1, 1, false));
            // ReSharper restore AssignNullToNotNullAttribute

            Assert.Throws<ArgumentOutOfRangeException>(() => RandomGraphFactory.Create(graph, () => 1, Edge.Create, random, -1, 1, false));
            Assert.Throws<ArgumentOutOfRangeException>(() => RandomGraphFactory.Create(graph, () => 1, Edge.Create, random, 0, 1, false));
            Assert.Throws<ArgumentOutOfRangeException>(() => RandomGraphFactory.Create(graph, () => 1, Edge.Create, random, 1, -1, false));
            Assert.Throws<ArgumentOutOfRangeException>(() => RandomGraphFactory.Create(graph, () => 1, Edge.Create, random, 0, 0, false));
            Assert.Throws<ArgumentOutOfRangeException>(() => RandomGraphFactory.Create(graph, () => 1, Edge.Create, random, -1, -1, false));
        }

        [Test]
        public void Create_Undirected()
        {
            var graph = new UndirectedGraph<int, EquatableEdge<int>>();

            // With isolated vertices only
            int v = 0;
            RandomGraphFactory.Create(
                graph,
                () => ++v,
                (source, target) => new EquatableEdge<int>(source, target),
                new Random(123456),
                2,
                0,
                true);
            graph.AssertHasVertices(1, 2 );
            graph.AssertNoEdge();

            // With self edge
            v = 0;
            RandomGraphFactory.Create(
                graph,
                () => ++v,
                (source, target) => new EquatableEdge<int>(source, target),
                new Random(123456),
                5,
                10,
                true);
            graph.AssertHasVertices(1, 2, 3, 4, 5 );
            graph.AssertHasEdges(
                new EquatableEdge<int>[]
                {
                    new (1, 2),
                    new (1, 2),
                    new (1, 2),
                    new (2, 1),
                    new (3, 5),
                    new (3, 1),
                    new (4, 5),
                    new (4, 4),
                    new (4, 1),
                    new (5, 3)
                });

            // Without self edge
            graph.Clear();
            v = 0;
            RandomGraphFactory.Create(
                graph,
                () => ++v,
                (source, target) => new EquatableEdge<int>(source, target),
                new Random(123456),
                5,
                10,
                false);
            graph.AssertHasVertices(1, 2, 3, 4, 5 );
            graph.AssertHasEdges(
                new EquatableEdge<int>[]
                {
                    new(1, 2),
                    new(1, 2),
                    new(1, 2),
                    new(2, 1),
                    new(3, 5),
                    new(3, 1),
                    new(3, 1),
                    new(4, 5),
                    new(4, 1),
                    new(5, 3)
                });

            // Different seed change generated graph
            graph.Clear();
            v = 0;
            RandomGraphFactory.Create(
                graph,
                () => ++v,
                (source, target) => new EquatableEdge<int>(source, target),
                new Random(456789),
                5,
                10,
                true);
            graph.AssertHasVertices(1, 2, 3, 4, 5 );
            graph.AssertHasEdges(
                new EquatableEdge<int>[]
                {
                    new(1, 2),
                    new(2, 2),
                    new(2, 5),
                    new(3, 4),
                    new(3, 2),
                    new(4, 5),
                    new(4, 2),
                    new(4, 2),
                    new(5, 2),
                    new(5, 3)
                });

            // On non empty graph, keep existing stuff
            graph.Clear();
            graph.AddVerticesAndEdge(new EquatableEdge<int>(6, 7));
            v = 0;
            RandomGraphFactory.Create(
                graph,
                () => ++v,
                (source, target) => new EquatableEdge<int>(source, target),
                new Random(123456),
                5,
                10,
                true);
            graph.AssertHasVertices(1, 2, 3, 4, 5, 6, 7 );
            graph.AssertHasEdges(
                new EquatableEdge<int>[]
                {
                    new(6, 7),
                    new(1, 2),
                    new(1, 2),
                    new(1, 2),
                    new(2, 1),
                    new(3, 5),
                    new(3, 1),
                    new(4, 5),
                    new(4, 4),
                    new(4, 1),
                    new(5, 3)
                });
        }

        [Test]
        public void Create_Undirected_Throws()
        {
            var graph = new UndirectedGraph<int, IEdge<int>>();
            var random = new Random();

            // ReSharper disable AssignNullToNotNullAttribute
            Assert.Throws<ArgumentNullException>(() => RandomGraphFactory.Create((IMutableUndirectedGraph<int, IEdge<int>>)null, () => 1, Edge.Create, random, 1, 1, false));
            Assert.Throws<ArgumentNullException>(() => RandomGraphFactory.Create(graph, null, Edge.Create, random, 1, 1, false));
            Assert.Throws<ArgumentNullException>(() => RandomGraphFactory.Create(graph, () => 1, null, random, 1, 1, false));
            Assert.Throws<ArgumentNullException>(() => RandomGraphFactory.Create(graph, () => 1, Edge.Create, null, 1, 1, false));
            Assert.Throws<ArgumentNullException>(() => RandomGraphFactory.Create((IMutableUndirectedGraph<int, IEdge<int>>)null, null, Edge.Create, random, 1, 1, false));
            Assert.Throws<ArgumentNullException>(() => RandomGraphFactory.Create((IMutableUndirectedGraph<int, IEdge<int>>)null, () => 1, null, random, 1, 1, false));
            Assert.Throws<ArgumentNullException>(() => RandomGraphFactory.Create((IMutableUndirectedGraph<int, IEdge<int>>)null, () => 1, Edge.Create, null, 1, 1, false));
            Assert.Throws<ArgumentNullException>(() => RandomGraphFactory.Create(graph, null, null, random, 1, 1, false));
            Assert.Throws<ArgumentNullException>(() => RandomGraphFactory.Create(graph, null, Edge.Create, null, 1, 1, false));
            Assert.Throws<ArgumentNullException>(() => RandomGraphFactory.Create(graph, () => 1, null, null, 1, 1, false));
            Assert.Throws<ArgumentNullException>(() => RandomGraphFactory.Create(graph,null, null, null, 1, 1, false));
            Assert.Throws<ArgumentNullException>(() => RandomGraphFactory.Create((IMutableUndirectedGraph<int, IEdge<int>>)null, () => 1, null, null, 1, 1, false));
            Assert.Throws<ArgumentNullException>(() => RandomGraphFactory.Create((IMutableUndirectedGraph<int, IEdge<int>>)null, null, Edge.Create, null, 1, 1, false));
            Assert.Throws<ArgumentNullException>(() => RandomGraphFactory.Create((IMutableUndirectedGraph<int, IEdge<int>>)null, null, null, random, 1, 1, false));
            Assert.Throws<ArgumentNullException>(() => RandomGraphFactory.Create((IMutableUndirectedGraph<int, IEdge<int>>)null, null, null, null, 1, 1, false));

            // ReSharper restore AssignNullToNotNullAttribute
            Assert.Throws<ArgumentOutOfRangeException>(() => RandomGraphFactory.Create(graph, () => 1, Edge.Create, random, -1, 1, false));
            Assert.Throws<ArgumentOutOfRangeException>(() => RandomGraphFactory.Create(graph, () => 1, Edge.Create, random, 0, 1, false));
            Assert.Throws<ArgumentOutOfRangeException>(() => RandomGraphFactory.Create(graph, () => 1, Edge.Create, random, 1, -1, false));
            Assert.Throws<ArgumentOutOfRangeException>(() => RandomGraphFactory.Create(graph, () => 1, Edge.Create, random, 0, 0, false));
            Assert.Throws<ArgumentOutOfRangeException>(() => RandomGraphFactory.Create(graph, () => 1, Edge.Create, random, -1, -1, false));
        }
    }
}