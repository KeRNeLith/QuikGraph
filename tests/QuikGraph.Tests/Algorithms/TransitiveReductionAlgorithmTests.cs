using System;
using System.Collections.Generic;
using NUnit.Framework;
using QuikGraph.Algorithms;

using static QuikGraph.Tests.GraphTestHelpers;

namespace QuikGraph.Tests.Algorithms
{
    /// <summary>
    /// Tests for <see cref="TransitiveReductionAlgorithm{TVertex,TEdge}"/>.
    /// </summary>
    [TestFixture]
    internal sealed class TransitiveReductionAlgorithmTests
    {
        [Test]
        public void Constructor()
        {
            var graph = new AdjacencyGraph<int, IEdge<int>>();
            var algorithm = new TransitiveReductionAlgorithm<int, IEdge<int>>(graph);
            algorithm.AssertAlgorithmState(graph);
            Assert.IsNotNull(algorithm.TransitiveReduction);
        }

        [Test]
        public void Constructor_Throws()
        {
            // ReSharper disable once ObjectCreationAsStatement
            // ReSharper disable once AssignNullToNotNullAttribute
            Assert.Throws<ArgumentNullException>(
                () => new TransitiveReductionAlgorithm<int, IEdge<int>>(null));
        }

        [Test]
        public void TransitiveReduction_ValueType()
        {
            // Test 1
            var edge12 = new SEdge<int>(1, 2);
            var edge13 = new SEdge<int>(1, 3);
            var edge14 = new SEdge<int>(1, 4);
            var edge15 = new SEdge<int>(1, 5);
            var edge24 = new SEdge<int>(2, 4);
            var edge34 = new SEdge<int>(3, 4);
            var edge35 = new SEdge<int>(3, 5);
            var edge45 = new SEdge<int>(4, 5);
            var graph = new AdjacencyGraph<int, SEdge<int>>();
            graph.AddVerticesAndEdgeRange(
                edge12, edge13, edge14, edge15,
                edge24, edge34, edge35, edge45
            );

            var result = graph.ComputeTransitiveReduction();
            result.AssertHasVertices(new List<int>
            {
                1,
                2,
                3,
                4,
                5
            }.AsReadOnly());
            List<SEdge<int>> expected = new()
            {
                edge12,
                edge13,
                edge24,
                edge34,
                edge45
            };
            result.AssertHasEdges(expected);
            Assert.AreEqual(expected.Count, result.EdgeCount);

            // Idempotency:
            var result2 = result.ComputeTransitiveReduction();
            result2.AssertHasEdges(expected);
        }

        [Test]
        public void TransitiveReduction_ValueType2()
        {
            var edge01 = new SEdge<int>(0, 1);
            var edge02 = new SEdge<int>(0, 2);
            var edge03 = new SEdge<int>(0, 3);
            var edge23 = new SEdge<int>(2, 3);
            var edge24 = new SEdge<int>(2, 4);
            var edge25 = new SEdge<int>(2, 5);
            var edge35 = new SEdge<int>(3, 5);
            var edge45 = new SEdge<int>(4, 5);
            var edge65 = new SEdge<int>(6, 5);
            var edge67 = new SEdge<int>(6, 7);
            var edge74 = new SEdge<int>(7, 4);
            var graph = new AdjacencyGraph<int, SEdge<int>>();
            graph.AddVerticesAndEdgeRange(
                edge01, edge02, edge03, edge23,
                edge24, edge25, edge35, edge45,
                edge65, edge67, edge74
            );

            var result = graph.ComputeTransitiveReduction();
            result.AssertHasVertices(new List<int>
            {
                0,
                1,
                2,
                3,
                4,
                5,
                6,
                7
            }.AsReadOnly());
            var expected = new List<SEdge<int>>
            {
                edge01,
                edge02,
                edge23,
                edge24,
                edge35,
                edge45,
                edge67,
                edge74
            }.AsReadOnly();
            result.AssertHasEdges(expected);

            // Idempotency:
            var result2 = result.ComputeTransitiveReduction();
            result2.AssertHasEdges(expected);
        }

        [Test]
        public void TransitiveReduction_ReferenceType()
        {
            // Test 1
            var edge12 = Edge.Create(1, 2);
            var edge13 = Edge.Create(1, 3);
            var edge14 = Edge.Create(1, 4);
            var edge15 = Edge.Create(1, 5);
            var edge24 = Edge.Create(2, 4);
            var edge34 = Edge.Create(3, 4);
            var edge35 = Edge.Create(3, 5);
            var edge45 = Edge.Create(4, 5);
            var graph = new AdjacencyGraph<int, IEdge<int>>();
            graph.AddVerticesAndEdgeRange(
                edge12, edge13, edge14, edge15,
                edge24, edge34, edge35, edge45
            );

            var result = graph.ComputeTransitiveReduction();
            result.AssertHasVertices(1, 2, 3, 4, 5 );
            var expected = new[] { edge12, edge13, edge24, edge34, edge45 };

            result.AssertHasEdges(expected);

            // Idempotency:
            var result2 = result.ComputeTransitiveReduction();
            result2.AssertHasEdges(expected);
        }

        [Test]
        public void TransitiveReduction_ReferenceType2()
        {
            var edge01 = Edge.Create(0, 1);
            var edge02 = Edge.Create(0, 2);
            var edge03 = Edge.Create(0, 3);
            var edge23 = Edge.Create(2, 3);
            var edge24 = Edge.Create(2, 4);
            var edge25 = Edge.Create(2, 5);
            var edge35 = Edge.Create(3, 5);
            var edge45 = Edge.Create(4, 5);
            var edge65 = Edge.Create(6, 5);
            var edge67 = Edge.Create(6, 7);
            var edge74 = Edge.Create(7, 4);
            var graph = new AdjacencyGraph<int, IEdge<int>>();
            graph.AddVerticesAndEdgeRange(
                edge01, edge02, edge03, edge23, edge24, edge25, edge35, edge45, edge65, edge67, edge74
            );

            var result = graph.ComputeTransitiveReduction();
            result.AssertHasVertices(0, 1, 2, 3, 4, 5, 6, 7 );
            var expected = new[] { edge01, edge02, edge23, edge24, edge35, edge45, edge67, edge74 };
            result.AssertHasEdges(expected);

            // Idempotency:
            var result2 = result.ComputeTransitiveReduction();
            result2.AssertHasEdges(expected);
        }

        [Test]
        public void TransitiveReduction_IsolatedVertices()
        {
            const string vertex1 = "/test";
            const string vertex2 = "/test/123";
            const string vertex3 = "/test/notlinked";
            var edge12 = Edge.Create(vertex1, vertex2);

            var graph = new AdjacencyGraph<string, IEdge<string>>();
            graph.AddVertexRange( vertex1, vertex2, vertex3 );
            graph.AddEdge(edge12);

            BidirectionalGraph<string, IEdge<string>> result = graph.ComputeTransitiveReduction();
            result.AssertHasVertices(new List<string> { vertex1, vertex2, vertex3 }.AsReadOnly());
            var expected = new List<IEdge<string>> { edge12 }.AsReadOnly();
            result.AssertHasEdges(expected);

            // Idempotency:
            var result2 = result.ComputeTransitiveReduction();
            result2.AssertHasEdges(expected);
        }
    }
}