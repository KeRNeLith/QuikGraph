using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using NUnit.Framework;
using QuikGraph.Algorithms.MaximumFlow;

namespace QuikGraph.Tests.Algorithms.MaximumFlow
{
    /// <summary>
    /// Tests for <see cref="ReversedEdgeAugmentorAlgorithm{TVertex,TEdge}"/>.
    /// </summary>
    [TestFixture]
    internal sealed class ReversedEdgeAugmentorAlgorithmTests
    {
        [Test]
        public void Constructor()
        {
            var graph = new AdjacencyGraph<int, IEdge<int>>();
            EdgeFactory<int, IEdge<int>> edgeFactory = Edge.Create;

            var algorithm = graph.CreateReversedEdgeAugmentorAlgorithm(edgeFactory);
            Assert.AreSame(graph, algorithm.VisitedGraph);
            Assert.AreSame(edgeFactory, algorithm.EdgeFactory);
            Assert.IsFalse(algorithm.Augmented);
            CollectionAssert.IsEmpty(algorithm.AugmentedEdges);
            CollectionAssert.IsEmpty(algorithm.ReversedEdges);
        }

        [Test]
        public void Constructor_Throws()
        {
            AdjacencyGraph<int, IEdge<int>> graphNull = null, graph = new();
            EdgeFactory<int, IEdge<int>> edgeFactory = Edge.Create;

            // ReSharper disable ObjectCreationAsStatement
            // ReSharper disable AssignNullToNotNullAttribute
            Assert.Throws<ArgumentNullException>(() => graphNull.CreateReversedEdgeAugmentorAlgorithm(edgeFactory));
            Assert.Throws<ArgumentNullException>(() => graph.CreateReversedEdgeAugmentorAlgorithm(null));
            Assert.Throws<ArgumentNullException>(() => graphNull.CreateReversedEdgeAugmentorAlgorithm(null));
            // ReSharper restore AssignNullToNotNullAttribute
            // ReSharper restore ObjectCreationAsStatement
        }

        [NotNull, ItemNotNull]
        private static IEnumerable<TestCaseData> AddReversedEdgeTestCases
        {
            [UsedImplicitly]
            get
            {
                EdgeFactory<int, IEdge<int>> edgeFactory1 = Edge.Create;
                yield return new TestCaseData(edgeFactory1);


                EdgeFactory<int, SEdge<int>> edgeFactory2 = (source, target) => new SEdge<int>(source, target);
                yield return new TestCaseData(edgeFactory2);
            }
        }

        [TestCaseSource(nameof(AddReversedEdgeTestCases))]
        public void AddReversedEdges<TEdge>([NotNull] EdgeFactory<int, TEdge> edgeFactory)
            where TEdge : IEdge<int>
        {
            TEdge edge12 = edgeFactory(1, 2);
            TEdge edge13 = edgeFactory(1, 3);
            TEdge edge23 = edgeFactory(2, 3);
            TEdge edge32 = edgeFactory(3, 2);

            var graph = new AdjacencyGraph<int, TEdge>();
            graph.AddVerticesAndEdgeRange(edge12, edge13, edge23, edge32);

            var algorithm = graph.CreateReversedEdgeAugmentorAlgorithm(edgeFactory);

            var reverseEdgesAdded = new List<TEdge>();
            algorithm.ReversedEdgeAdded += reverseEdgesAdded.Add;

            algorithm.AddReversedEdges();

            Assert.IsTrue(algorithm.Augmented);
            CollectionAssert.IsNotEmpty(algorithm.AugmentedEdges);
            TEdge[] augmentedEdges = algorithm.AugmentedEdges.ToArray();
            Assert.AreEqual(2, augmentedEdges.Length);
            Assert.AreEqual(2, augmentedEdges[0].Source);
            Assert.AreEqual(1, augmentedEdges[0].Target);
            Assert.AreEqual(3, augmentedEdges[1].Source);
            Assert.AreEqual(1, augmentedEdges[1].Target);

            CollectionAssert.AreEquivalent(
                new Dictionary<TEdge, TEdge>
                {
                    [edge12] = augmentedEdges[0],
                    [augmentedEdges[0]] = edge12,
                    [edge13] = augmentedEdges[1],
                    [augmentedEdges[1]] = edge13,
                    [edge23] = edge32,
                    [edge32] = edge23,
                },
                algorithm.ReversedEdges);

            CollectionAssert.AreEqual(augmentedEdges, reverseEdgesAdded);
        }

        [Test]
        public void AddReversedEdges_Throws()
        {
            var graph = new AdjacencyGraph<int, IEdge<int>>();
            EdgeFactory<int, IEdge<int>> edgeFactory = Edge.Create;

            var algorithm = graph.CreateReversedEdgeAugmentorAlgorithm(edgeFactory);
            Assert.DoesNotThrow(algorithm.AddReversedEdges);
            Assert.Throws<InvalidOperationException>(algorithm.AddReversedEdges);
        }

        [Test]
        public void RemoveReversedEdges()
        {
            var edge12 = Edge.Create(1, 2);
            var edge13 = Edge.Create(1, 3);
            var edge23 = Edge.Create(2, 3);
            var edge32 = Edge.Create(3, 2);

            var graph = new AdjacencyGraph<int, IEdge<int>>();
            graph.AddVerticesAndEdgeRange(edge12, edge13, edge23, edge32);

            var algorithm = graph.CreateReversedEdgeAugmentorAlgorithm(Edge.Create);
            algorithm.AddReversedEdges();

            Assert.IsTrue(algorithm.Augmented);
            CollectionAssert.IsNotEmpty(algorithm.AugmentedEdges);
            foreach (IEdge<int> edge in algorithm.AugmentedEdges)
            {
                CollectionAssert.Contains(algorithm.VisitedGraph.Edges, edge);
            }
            CollectionAssert.IsNotEmpty(algorithm.ReversedEdges);

            algorithm.RemoveReversedEdges();

            Assert.IsFalse(algorithm.Augmented);
            CollectionAssert.IsEmpty(algorithm.AugmentedEdges);
            foreach (var edge in algorithm.AugmentedEdges)
            {
                CollectionAssert.DoesNotContain(algorithm.VisitedGraph.Edges, edge);
            }
            CollectionAssert.IsEmpty(algorithm.ReversedEdges);
        }

        [Test]
        public void RemoveReversedEdges_Throws()
        {
            var graph = new AdjacencyGraph<int, IEdge<int>>();

            var algorithm = graph.CreateReversedEdgeAugmentorAlgorithm(Edge.Create);
            Assert.Throws<InvalidOperationException>(algorithm.RemoveReversedEdges);
        }

        [Test]
        public void Dispose()
        {
            var graph = new AdjacencyGraph<int, IEdge<int>>();

            var algorithm = graph.CreateReversedEdgeAugmentorAlgorithm(Edge.Create);
            CollectionAssert.IsEmpty(algorithm.AugmentedEdges);
            CollectionAssert.IsEmpty(algorithm.ReversedEdges);
            ((IDisposable)algorithm).Dispose();
            CollectionAssert.IsEmpty(algorithm.AugmentedEdges);
            CollectionAssert.IsEmpty(algorithm.ReversedEdges);

            graph.AddVerticesAndEdgeRange(
                Edge.Create(1, 2),
                Edge.Create(1, 3),
                Edge.Create(2, 3),
                Edge.Create(3, 2)
            );
            algorithm = graph.CreateReversedEdgeAugmentorAlgorithm(Edge.Create);
            algorithm.AddReversedEdges();
            CollectionAssert.IsNotEmpty(algorithm.AugmentedEdges);
            CollectionAssert.IsNotEmpty(algorithm.ReversedEdges);
            ((IDisposable)algorithm).Dispose();
            CollectionAssert.IsEmpty(algorithm.AugmentedEdges);
            CollectionAssert.IsEmpty(algorithm.ReversedEdges);
        }
    }
}