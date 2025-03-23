using System;
using System.Collections.Generic;
using NUnit.Framework;
using QuikGraph.Algorithms;

using static QuikGraph.Tests.GraphTestHelpers;

namespace QuikGraph.Tests.Algorithms
{
    /// <summary>
    /// Tests for <see cref="TransitiveClosureAlgorithm{TVertex,TEdge}"/>.
    /// </summary>
    [TestFixture]
    internal sealed class TransitiveClosureAlgorithmTests
    {
        [Test]
        public void Constructor()
        {
            var graph = new AdjacencyGraph<int, IEdge<int>>();
            var algorithm = graph.CreateTransitiveClosureAlgorithm(Edge.Create);
            algorithm.AssertAlgorithmState(graph);
            Assert.IsNotNull(algorithm.TransitiveClosure);
        }

        [Test]
        public void Constructor_Throws()
        {
            AdjacencyGraph<int, IEdge<int>> graph = new (), nullGraph = null;
            // ReSharper disable ObjectCreationAsStatement
            // ReSharper disable AssignNullToNotNullAttribute
            Assert.Throws<ArgumentNullException>(() => nullGraph.CreateTransitiveClosureAlgorithm(Edge.Create));
            Assert.Throws<ArgumentNullException>(() => nullGraph.CreateTransitiveClosureAlgorithm(null));
            Assert.Throws<ArgumentNullException>(() => graph.CreateTransitiveClosureAlgorithm(null));
            // ReSharper restore AssignNullToNotNullAttribute
            // ReSharper restore ObjectCreationAsStatement
        }

        [Test]
        public void TransitiveClosure_ValueType()
        {
            // Test 1
            var graph = new AdjacencyGraph<int, SEquatableEdge<int>>();
            graph.AddVerticesAndEdgeRange(
                new SEquatableEdge<int>(1, 2),
                new SEquatableEdge<int>(2, 3)
            );

            var result = graph.ComputeTransitiveClosure((u, v) => new SEquatableEdge<int>(u, v));
            result.AssertHasVertices(1, 2, 3);
            var expected = new SEquatableEdge<int>[]{
                    new(1, 2),
                    new(1, 3),
                    new(2, 3)
            };
            result.AssertHasEdges(expected);

            var result2 = result.ComputeTransitiveClosure((u, v) => new SEquatableEdge<int>(u, v));
            result2.AssertHasEdges(expected);
        }

        [Test]
        public void TransitiveClosure_ValueType2()
        {
            var graph = new AdjacencyGraph<int, SEquatableEdge<int>>();
            graph.AddVerticesAndEdgeRange(
                new List<SEquatableEdge<int>>
                {
                    new(1, 2),
                    new(2, 3),
                    new(3, 4),
                    new(3, 5)
                }.AsReadOnly());

            var result = graph.ComputeTransitiveClosure((u, v)
                => new SEquatableEdge<int>(u, v));
            result.AssertHasVertices(new List<int> { 1, 2, 3, 4, 5 }.AsReadOnly());
            var expected = new SEquatableEdge<int>[] {
                new(1, 2),
                new(1, 3),
                new(1, 4),
                new(1, 5),
                new(2, 3),
                new(2, 4),
                new(2, 5),
                new(3, 4),
                new(3, 5)
            };
            result.AssertHasEdges(expected);

            // Idempotency:
            var result2 = result.ComputeTransitiveClosure((u, v)
                => new SEquatableEdge<int>(u, v));
            result2.AssertHasEdges(expected);
        }

        [Test]
        public void TransitiveClosure_ReferenceType()
        {
            var graph = new AdjacencyGraph<int, EquatableEdge<int>>();
            graph.AddVerticesAndEdgeRange(
                new EquatableEdge<int>(1, 2),
                new EquatableEdge<int>(2, 3)
            );

            BidirectionalGraph<int, EquatableEdge<int>> result = graph.ComputeTransitiveClosure((u, v) => new EquatableEdge<int>(u, v));
            result.AssertHasVertices(1, 2, 3 );
            var expected = new List<EquatableEdge<int>>
            {
                new(1, 2),
                new(1, 3),
                new(2, 3)
            }.AsReadOnly();
            result.AssertHasEdges(expected);

            // Idempotency:
            var result2 = result.ComputeTransitiveClosure((u, v)
                => new EquatableEdge<int>(u, v));
            result2.AssertHasEdges(expected);
        }

        [Test]
        public void TransitiveClosure_ReferenceType2()
        {
            var graph = new AdjacencyGraph<int, EquatableEdge<int>>();
            graph.AddVerticesAndEdgeRange(
                new List<EquatableEdge<int>>
                {
                    new(1, 2),
                    new(2, 3),
                    new(3, 4),
                    new(3, 5)
                }.AsReadOnly());

            var result = graph.ComputeTransitiveClosure((u, v) => new EquatableEdge<int>(u, v));
            result.AssertHasVertices(1, 2, 3, 4, 5 );
            var expected = new List<EquatableEdge<int>>
            {
                new(1, 2),
                new(1, 3),
                new(1, 4),
                new(1, 5),
                new(2, 3),
                new(2, 4),
                new(2, 5),
                new(3, 4),
                new(3, 5)
            };

            result.AssertHasEdges(expected);

            // Idempotency:
            var result2 = result.ComputeTransitiveClosure((u, v)
                => new EquatableEdge<int>(u, v));
            result2.AssertHasEdges(expected);
        }

        [Test]
        public void TransitiveClosure_IsolatedVertices()
        {
            const string vertex1 = "/test";
            const string vertex2 = "/test/123";
            const string vertex3 = "/test/456";
            const string vertex4 = "/test/notlinked";
            var edge12 = new EquatableEdge<string>(vertex1, vertex2);
            var edge23 = new EquatableEdge<string>(vertex2, vertex3);

            var graph = new AdjacencyGraph<string, EquatableEdge<string>>();
            graph.AddVertexRange( vertex1, vertex2, vertex3, vertex4 );
            graph.AddEdgeRange( edge12, edge23 );

            BidirectionalGraph<string, EquatableEdge<string>> result = graph.ComputeTransitiveClosure((u, v) => new EquatableEdge<string>(u, v));
            result.AssertHasVertices(vertex1, vertex2, vertex3, vertex4 );
            result.AssertHasEdges(edge12, new EquatableEdge<string>(vertex1, vertex3), edge23);
        }
    }
}