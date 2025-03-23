using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using NUnit.Framework;
using QuikGraph.Algorithms.RandomWalks;

namespace QuikGraph.Tests.Algorithms.RandomWalks
{
    /// <summary>
    /// Tests for <see cref="RandomWalkAlgorithm{TVertex,TEdge}"/>.
    /// </summary>
    [TestFixture]
    internal sealed class EdgeChainsTests
    {
        #region Test helpers

        [Pure]
        [NotNull]
        private static IVertexAndEdgeListGraph<int, EquatableEdge<int>> CreateGraph1()
        {
            var graph = new AdjacencyGraph<int, EquatableEdge<int>>();
            graph.AddVerticesAndEdgeRange(
                new EquatableEdge<int>[] {
                new (1, 2),
                new (1, 3),
                new (2, 3),
                new (2, 5),
                new (4, 3),
                new (4, 5),
                new (4, 7),
                new (5, 6),
                new (6, 7),
                new (7, 4),
                new (8, 3)
            });

            return graph;
        }

        [Pure]
        [NotNull]
        private static IVertexAndEdgeListGraph<int, EquatableEdge<int>> CreateGraph2()
        {
            var graph = new AdjacencyGraph<int, EquatableEdge<int>>();
            graph.AddVerticesAndEdgeRange(
                new EquatableEdge<int>[] {
                new (1, 2),
                new (1, 3),
                new (2, 3),
                new (3, 4),
                new (4, 5),
                new (4, 7),
                new (5, 2),
                new (5, 6),
                new (6, 7),
                new (7, 4),
                new (8, 3)
            });

            return graph;
        }

        #endregion

        [Test]
        public void RoundRobinEdgeChain()
        {
            IVertexAndEdgeListGraph<int, EquatableEdge<int>> graph1 = CreateGraph1();

            var chain = new RoundRobinEdgeChain<int, EquatableEdge<int>>();
            Assert.IsTrue(chain.TryGetSuccessor(graph1, 1, out EquatableEdge<int> edge));
            Assert.AreEqual(1, edge.Source);
            Assert.AreEqual(2, edge.Target);
            Assert.IsTrue(chain.TryGetSuccessor(graph1, 2, out edge));
            Assert.AreEqual(2, edge.Source);
            Assert.AreEqual(3, edge.Target);
            Assert.IsFalse(chain.TryGetSuccessor(graph1, 3, out edge));

            chain = new RoundRobinEdgeChain<int, EquatableEdge<int>>();
            EquatableEdge<int>[] edges = graph1.Edges.ToArray();
            Assert.IsTrue(chain.TryGetSuccessor(edges, 1, out edge));
            Assert.AreEqual(1, edge.Source);
            Assert.AreEqual(2, edge.Target);
            Assert.IsTrue(chain.TryGetSuccessor(edges, 2, out edge));
            Assert.AreEqual(1, edge.Source);
            Assert.AreEqual(2, edge.Target);
            Assert.IsTrue(chain.TryGetSuccessor(edges, 2, out edge));
            Assert.AreEqual(1, edge.Source);
            Assert.AreEqual(3, edge.Target);
            Assert.IsTrue(chain.TryGetSuccessor(edges, 3, out edge));
            Assert.AreEqual(1, edge.Source);
            Assert.AreEqual(2, edge.Target);
            Assert.IsTrue(chain.TryGetSuccessor(edges, 2, out edge));
            Assert.AreEqual(2, edge.Source);
            Assert.AreEqual(3, edge.Target);
            Assert.IsTrue(chain.TryGetSuccessor(edges, 3, out edge));
            Assert.AreEqual(1, edge.Source);
            Assert.AreEqual(3, edge.Target);
            Assert.IsTrue(chain.TryGetSuccessor(edges, 3, out edge));
            Assert.AreEqual(2, edge.Source);
            Assert.AreEqual(3, edge.Target);
            Assert.IsTrue(chain.TryGetSuccessor(edges, 3, out edge));
            Assert.AreEqual(2, edge.Source);
            Assert.AreEqual(5, edge.Target);
            // etc.
            Assert.IsFalse(chain.TryGetSuccessor(Enumerable.Empty<EquatableEdge<int>>(), 1, out _));


            IVertexAndEdgeListGraph<int, EquatableEdge<int>> graph2 = CreateGraph2();

            chain = new RoundRobinEdgeChain<int, EquatableEdge<int>>();
            Assert.IsTrue(chain.TryGetSuccessor(graph2, 1, out edge));
            Assert.AreEqual(1, edge.Source);
            Assert.AreEqual(2, edge.Target);
            Assert.IsTrue(chain.TryGetSuccessor(graph2, 2, out edge));
            Assert.AreEqual(2, edge.Source);
            Assert.AreEqual(3, edge.Target);
            Assert.IsTrue(chain.TryGetSuccessor(graph2, 3, out edge));
            Assert.AreEqual(3, edge.Source);
            Assert.AreEqual(4, edge.Target);
            Assert.IsTrue(chain.TryGetSuccessor(graph2, 4, out edge));
            Assert.AreEqual(4, edge.Source);
            Assert.AreEqual(5, edge.Target);
            Assert.IsTrue(chain.TryGetSuccessor(graph2, 5, out edge));
            Assert.AreEqual(5, edge.Source);
            Assert.AreEqual(2, edge.Target);
            Assert.IsTrue(chain.TryGetSuccessor(graph2, 2, out edge));
            Assert.AreEqual(2, edge.Source);
            Assert.AreEqual(3, edge.Target);
            Assert.IsTrue(chain.TryGetSuccessor(graph2, 3, out edge));
            Assert.AreEqual(3, edge.Source);
            Assert.AreEqual(4, edge.Target);
            Assert.IsTrue(chain.TryGetSuccessor(graph2, 4, out edge));
            Assert.AreEqual(4, edge.Source);
            Assert.AreEqual(7, edge.Target);
            // Etc.

            chain = new RoundRobinEdgeChain<int, EquatableEdge<int>>();
            edges = graph2.Edges.ToArray();
            Assert.IsTrue(chain.TryGetSuccessor(edges, 1, out edge));
            Assert.AreEqual(1, edge.Source);
            Assert.AreEqual(2, edge.Target);
            Assert.IsTrue(chain.TryGetSuccessor(edges, 2, out edge));
            Assert.AreEqual(1, edge.Source);
            Assert.AreEqual(2, edge.Target);
            Assert.IsTrue(chain.TryGetSuccessor(edges, 2, out edge));
            Assert.AreEqual(1, edge.Source);
            Assert.AreEqual(3, edge.Target);
            Assert.IsTrue(chain.TryGetSuccessor(edges, 3, out edge));
            Assert.AreEqual(1, edge.Source);
            Assert.AreEqual(2, edge.Target);
            Assert.IsTrue(chain.TryGetSuccessor(edges, 2, out edge));
            Assert.AreEqual(2, edge.Source);
            Assert.AreEqual(3, edge.Target);
            Assert.IsTrue(chain.TryGetSuccessor(edges, 3, out edge));
            Assert.AreEqual(1, edge.Source);
            Assert.AreEqual(3, edge.Target);
            Assert.IsTrue(chain.TryGetSuccessor(edges, 3, out edge));
            Assert.AreEqual(2, edge.Source);
            Assert.AreEqual(3, edge.Target);
            Assert.IsTrue(chain.TryGetSuccessor(edges, 3, out edge));
            Assert.AreEqual(3, edge.Source);
            Assert.AreEqual(4, edge.Target);
            // Etc.
        }

        [Test]
        public void NormalizedMarkovEdgeChain()
        {
            IVertexAndEdgeListGraph<int, EquatableEdge<int>> graph1 = CreateGraph1();

            var chain = new NormalizedMarkovEdgeChain<int, EquatableEdge<int>>
            {
                Rand = new Random(123456)
            };
            Assert.IsTrue(chain.TryGetSuccessor(graph1, 1, out EquatableEdge<int> edge));
            Assert.AreEqual(1, edge.Source);
            Assert.AreEqual(2, edge.Target);
            Assert.IsTrue(chain.TryGetSuccessor(graph1, 2, out edge));
            Assert.AreEqual(2, edge.Source);
            Assert.AreEqual(3, edge.Target);
            Assert.IsFalse(chain.TryGetSuccessor(graph1, 3, out edge));

            chain = new NormalizedMarkovEdgeChain<int, EquatableEdge<int>>
            {
                Rand = new Random(123456)
            };
            EquatableEdge<int>[] edges = graph1.Edges.ToArray();
            Assert.IsTrue(chain.TryGetSuccessor(edges, 1, out edge));
            Assert.AreEqual(2, edge.Source);
            Assert.AreEqual(3, edge.Target);
            Assert.IsTrue(chain.TryGetSuccessor(edges, 3, out edge));
            Assert.AreEqual(1, edge.Source);
            Assert.AreEqual(3, edge.Target);
            Assert.IsTrue(chain.TryGetSuccessor(edges, 3, out edge));
            Assert.AreEqual(1, edge.Source);
            Assert.AreEqual(2, edge.Target);
            // etc.
            Assert.IsFalse(chain.TryGetSuccessor(Enumerable.Empty<EquatableEdge<int>>(), 1, out _));


            IVertexAndEdgeListGraph<int, EquatableEdge<int>> graph2 = CreateGraph2();

            chain = new NormalizedMarkovEdgeChain<int, EquatableEdge<int>>
            {
                Rand = new Random(123456)
            };
            Assert.IsTrue(chain.TryGetSuccessor(graph2, 1, out edge));
            Assert.AreEqual(1, edge.Source);
            Assert.AreEqual(2, edge.Target);
            Assert.IsTrue(chain.TryGetSuccessor(graph2, 2, out edge));
            Assert.AreEqual(2, edge.Source);
            Assert.AreEqual(3, edge.Target);
            Assert.IsTrue(chain.TryGetSuccessor(graph2, 3, out edge));
            Assert.AreEqual(3, edge.Source);
            Assert.AreEqual(4, edge.Target);
            Assert.IsTrue(chain.TryGetSuccessor(graph2, 4, out edge));
            Assert.AreEqual(4, edge.Source);
            Assert.AreEqual(5, edge.Target);
            Assert.IsTrue(chain.TryGetSuccessor(graph2, 5, out edge));
            Assert.AreEqual(5, edge.Source);
            Assert.AreEqual(6, edge.Target);
            Assert.IsTrue(chain.TryGetSuccessor(graph2, 6, out edge));
            Assert.AreEqual(6, edge.Source);
            Assert.AreEqual(7, edge.Target);
            Assert.IsTrue(chain.TryGetSuccessor(graph2, 7, out edge));
            Assert.AreEqual(7, edge.Source);
            Assert.AreEqual(4, edge.Target);
            Assert.IsTrue(chain.TryGetSuccessor(graph2, 4, out edge));
            // Etc.

            chain = new NormalizedMarkovEdgeChain<int, EquatableEdge<int>>
            {
                Rand = new Random(123456)
            };
            edges = graph2.Edges.ToArray();
            Assert.IsTrue(chain.TryGetSuccessor(edges, 1, out edge));
            Assert.AreEqual(2, edge.Source);
            Assert.AreEqual(3, edge.Target);
            Assert.IsTrue(chain.TryGetSuccessor(edges, 3, out edge));
            Assert.AreEqual(1, edge.Source);
            Assert.AreEqual(3, edge.Target);
            Assert.IsTrue(chain.TryGetSuccessor(edges, 3, out edge));
            Assert.AreEqual(1, edge.Source);
            Assert.AreEqual(2, edge.Target);
            // Etc.
        }

        [Test]
        public void Constructor_WeightedMarkovEdgeChains()
        {
            var weights = new Dictionary<IEdge<int>, double>();
            var chain1 = new WeightedMarkovEdgeChain<int, IEdge<int>>(weights);
            Assert.AreSame(weights, chain1.Weights);

            var chain2 = new VanishingWeightedMarkovEdgeChain<int, IEdge<int>>(weights);
            Assert.AreSame(weights, chain2.Weights);

            chain2 = new VanishingWeightedMarkovEdgeChain<int, IEdge<int>>(weights, 2.0);
            Assert.AreSame(weights, chain2.Weights);
        }

        [Test]
        public void Constructor_WeightedMarkovEdgeChains_Throws()
        {
            // ReSharper disable ObjectCreationAsStatement
            // ReSharper disable AssignNullToNotNullAttribute
            Assert.Throws<ArgumentNullException>(
                () => new WeightedMarkovEdgeChain<int, IEdge<int>>(null));
            Assert.Throws<ArgumentNullException>(
                () => new VanishingWeightedMarkovEdgeChain<int, IEdge<int>>(null));
            Assert.Throws<ArgumentNullException>(
                () => new VanishingWeightedMarkovEdgeChain<int, IEdge<int>>(null, 2.0));
            // ReSharper restore AssignNullToNotNullAttribute
            // ReSharper restore ObjectCreationAsStatement
        }

        [Test]
        public void WeightedMarkovEdgeChain()
        {
            var weights = new Dictionary<EquatableEdge<int>, double>
            {
                [new EquatableEdge<int>(1, 2)] = 1.0,
                [new EquatableEdge<int>(1, 3)] = 2.0,
                [new EquatableEdge<int>(2, 3)] = -1.0,
                [new EquatableEdge<int>(2, 5)] = 10.0,
                [new EquatableEdge<int>(3, 4)] = 5.0,
                [new EquatableEdge<int>(4, 3)] = 25.0,
                [new EquatableEdge<int>(4, 5)] = 2.0,
                [new EquatableEdge<int>(4, 7)] = 1.0,
                [new EquatableEdge<int>(5, 2)] = 3.0,
                [new EquatableEdge<int>(5, 6)] = 1.0,
                [new EquatableEdge<int>(6, 7)] = 1.5,
                [new EquatableEdge<int>(7, 4)] = 0.0,
                [new EquatableEdge<int>(8, 3)] = 1.0
            };
            IVertexAndEdgeListGraph<int, EquatableEdge<int>> graph1 = CreateGraph1();

            var chain = new WeightedMarkovEdgeChain<int, EquatableEdge<int>>(weights)
            {
                Rand = new Random(123456)
            };
            Assert.IsTrue(chain.TryGetSuccessor(graph1, 1, out EquatableEdge<int> edge));
            Assert.AreEqual(1, edge.Source);
            Assert.AreEqual(2, edge.Target);
            Assert.IsTrue(chain.TryGetSuccessor(graph1, 2, out edge));
            Assert.AreEqual(2, edge.Source);
            Assert.AreEqual(5, edge.Target);
            Assert.IsTrue(chain.TryGetSuccessor(graph1, 5, out edge));
            Assert.AreEqual(5, edge.Source);
            Assert.AreEqual(6, edge.Target);
            Assert.IsTrue(chain.TryGetSuccessor(graph1, 6, out edge));
            Assert.AreEqual(6, edge.Source);
            Assert.AreEqual(7, edge.Target);
            Assert.IsTrue(chain.TryGetSuccessor(graph1, 7, out edge));
            Assert.AreEqual(7, edge.Source);
            Assert.AreEqual(4, edge.Target);
            Assert.IsTrue(chain.TryGetSuccessor(graph1, 4, out edge));
            Assert.AreEqual(4, edge.Source);
            Assert.AreEqual(3, edge.Target);
            Assert.IsTrue(chain.TryGetSuccessor(graph1, 3, out edge));

            chain = new WeightedMarkovEdgeChain<int, EquatableEdge<int>>(weights)
            {
                Rand = new Random(123456)
            };
            EquatableEdge<int>[] edges = graph1.Edges.ToArray();
            Assert.IsTrue(chain.TryGetSuccessor(edges, 1, out edge));
            Assert.AreEqual(2, edge.Source);
            Assert.AreEqual(5, edge.Target);
            Assert.IsTrue(chain.TryGetSuccessor(edges, 5, out edge));
            Assert.AreEqual(2, edge.Source);
            Assert.AreEqual(5, edge.Target);
            Assert.IsTrue(chain.TryGetSuccessor(edges, 5, out edge));
            Assert.AreEqual(1, edge.Source);
            Assert.AreEqual(3, edge.Target);
            // etc.
            Assert.IsFalse(chain.TryGetSuccessor(Enumerable.Empty<EquatableEdge<int>>(), 1, out _));


            IVertexAndEdgeListGraph<int, EquatableEdge<int>> graph2 = CreateGraph2();

            chain = new WeightedMarkovEdgeChain<int, EquatableEdge<int>>(weights)
            {
                Rand = new Random(123456)
            };
            Assert.IsTrue(chain.TryGetSuccessor(graph2, 1, out edge));
            Assert.AreEqual(1, edge.Source);
            Assert.AreEqual(2, edge.Target);
            Assert.IsFalse(chain.TryGetSuccessor(graph2, 2, out edge));

            chain = new WeightedMarkovEdgeChain<int, EquatableEdge<int>>(weights)
            {
                Rand = new Random(123456)
            };
            edges = graph2.Edges.ToArray();
            Assert.IsTrue(chain.TryGetSuccessor(edges, 1, out edge));
            Assert.AreEqual(3, edge.Source);
            Assert.AreEqual(4, edge.Target);
            Assert.IsTrue(chain.TryGetSuccessor(edges, 4, out edge));
            Assert.AreEqual(1, edge.Source);
            Assert.AreEqual(3, edge.Target);
            Assert.IsTrue(chain.TryGetSuccessor(edges, 1, out edge));
            Assert.AreEqual(1, edge.Source);
            Assert.AreEqual(2, edge.Target);
            // Etc.
        }

        [Test]
        public void VanishingWeightedMarkovEdgeChain()
        {
            var weights = new Dictionary<EquatableEdge<int>, double>
            {
                [new EquatableEdge<int>(1, 2)] = 1.0,
                [new EquatableEdge<int>(1, 3)] = 2.0,
                [new EquatableEdge<int>(2, 3)] = -1.0,
                [new EquatableEdge<int>(2, 5)] = 10.0,
                [new EquatableEdge<int>(3, 4)] = 5.0,
                [new EquatableEdge<int>(4, 3)] = 25.0,
                [new EquatableEdge<int>(4, 5)] = 2.0,
                [new EquatableEdge<int>(4, 7)] = 1.0,
                [new EquatableEdge<int>(5, 2)] = 3.0,
                [new EquatableEdge<int>(5, 6)] = 1.0,
                [new EquatableEdge<int>(6, 7)] = 1.5,
                [new EquatableEdge<int>(7, 4)] = 0.25,
                [new EquatableEdge<int>(8, 3)] = 1.0
            };
            IVertexAndEdgeListGraph<int, EquatableEdge<int>> graph1 = CreateGraph1();

            var chain = new VanishingWeightedMarkovEdgeChain<int, EquatableEdge<int>>(weights)
            {
                Rand = new Random(123456)
            };
            Assert.IsTrue(chain.TryGetSuccessor(graph1, 1, out EquatableEdge<int> edge));
            Assert.AreEqual(1, edge.Source);
            Assert.AreEqual(2, edge.Target);
            Assert.IsTrue(chain.TryGetSuccessor(graph1, 2, out edge));
            Assert.AreEqual(2, edge.Source);
            Assert.AreEqual(5, edge.Target);
            Assert.IsTrue(chain.TryGetSuccessor(graph1, 5, out edge));
            Assert.AreEqual(5, edge.Source);
            Assert.AreEqual(6, edge.Target);
            Assert.IsTrue(chain.TryGetSuccessor(graph1, 6, out edge));
            Assert.AreEqual(6, edge.Source);
            Assert.AreEqual(7, edge.Target);
            Assert.IsTrue(chain.TryGetSuccessor(graph1, 7, out edge));
            Assert.AreEqual(7, edge.Source);
            Assert.AreEqual(4, edge.Target);
            Assert.IsTrue(chain.TryGetSuccessor(graph1, 4, out edge));
            Assert.AreEqual(4, edge.Source);
            Assert.AreEqual(3, edge.Target);
            Assert.IsTrue(chain.TryGetSuccessor(graph1, 3, out edge));

            weights = new Dictionary<EquatableEdge<int>, double>
            {
                [new EquatableEdge<int>(1, 2)] = 1.0,
                [new EquatableEdge<int>(1, 3)] = 2.0,
                [new EquatableEdge<int>(2, 3)] = -1.0,
                [new EquatableEdge<int>(2, 5)] = 10.0,
                [new EquatableEdge<int>(3, 4)] = 5.0,
                [new EquatableEdge<int>(4, 3)] = 25.0,
                [new EquatableEdge<int>(4, 5)] = 2.0,
                [new EquatableEdge<int>(4, 7)] = 1.0,
                [new EquatableEdge<int>(5, 2)] = 3.0,
                [new EquatableEdge<int>(5, 6)] = 1.0,
                [new EquatableEdge<int>(6, 7)] = 1.5,
                [new EquatableEdge<int>(7, 4)] = 0.25,
                [new EquatableEdge<int>(8, 3)] = 1.0
            };
            chain = new VanishingWeightedMarkovEdgeChain<int, EquatableEdge<int>>(weights)
            {
                Rand = new Random(123456)
            };
            EquatableEdge<int>[] edges = graph1.Edges.ToArray();
            Assert.IsTrue(chain.TryGetSuccessor(edges, 1, out edge));
            Assert.AreEqual(2, edge.Source);
            Assert.AreEqual(5, edge.Target);
            Assert.IsTrue(chain.TryGetSuccessor(edges, 5, out edge));
            Assert.AreEqual(2, edge.Source);
            Assert.AreEqual(5, edge.Target);
            Assert.IsTrue(chain.TryGetSuccessor(edges, 5, out edge));
            Assert.AreEqual(1, edge.Source);
            Assert.AreEqual(3, edge.Target);
            // etc.
            Assert.IsFalse(chain.TryGetSuccessor(Enumerable.Empty<EquatableEdge<int>>(), 1, out _));


            IVertexAndEdgeListGraph<int, EquatableEdge<int>> graph2 = CreateGraph2();

            weights = new Dictionary<EquatableEdge<int>, double>
            {
                [new EquatableEdge<int>(1, 2)] = 1.0,
                [new EquatableEdge<int>(1, 3)] = 2.0,
                [new EquatableEdge<int>(2, 3)] = -1.0,
                [new EquatableEdge<int>(2, 5)] = 10.0,
                [new EquatableEdge<int>(3, 4)] = 5.0,
                [new EquatableEdge<int>(4, 3)] = 25.0,
                [new EquatableEdge<int>(4, 5)] = 2.0,
                [new EquatableEdge<int>(4, 7)] = 1.0,
                [new EquatableEdge<int>(5, 2)] = 3.0,
                [new EquatableEdge<int>(5, 6)] = 1.0,
                [new EquatableEdge<int>(6, 7)] = 1.5,
                [new EquatableEdge<int>(7, 4)] = 0.25,
                [new EquatableEdge<int>(8, 3)] = 1.0
            };
            chain = new VanishingWeightedMarkovEdgeChain<int, EquatableEdge<int>>(weights)
            {
                Rand = new Random(123456)
            };
            Assert.IsTrue(chain.TryGetSuccessor(graph2, 1, out edge));
            Assert.AreEqual(1, edge.Source);
            Assert.AreEqual(2, edge.Target);
            Assert.IsFalse(chain.TryGetSuccessor(graph2, 2, out edge));

            weights = new Dictionary<EquatableEdge<int>, double>
            {
                [new EquatableEdge<int>(1, 2)] = 1.0,
                [new EquatableEdge<int>(1, 3)] = 2.0,
                [new EquatableEdge<int>(2, 3)] = -1.0,
                [new EquatableEdge<int>(2, 5)] = 10.0,
                [new EquatableEdge<int>(3, 4)] = 5.0,
                [new EquatableEdge<int>(4, 3)] = 25.0,
                [new EquatableEdge<int>(4, 5)] = 2.0,
                [new EquatableEdge<int>(4, 7)] = 1.0,
                [new EquatableEdge<int>(5, 2)] = 3.0,
                [new EquatableEdge<int>(5, 6)] = 1.0,
                [new EquatableEdge<int>(6, 7)] = 1.5,
                [new EquatableEdge<int>(7, 4)] = 0.25,
                [new EquatableEdge<int>(8, 3)] = 1.0
            };
            chain = new VanishingWeightedMarkovEdgeChain<int, EquatableEdge<int>>(weights)
            {
                Rand = new Random(123456)
            };
            edges = graph2.Edges.ToArray();
            Assert.IsTrue(chain.TryGetSuccessor(edges, 1, out edge));
            Assert.AreEqual(3, edge.Source);
            Assert.AreEqual(4, edge.Target);
            Assert.IsTrue(chain.TryGetSuccessor(edges, 4, out edge));
            Assert.AreEqual(1, edge.Source);
            Assert.AreEqual(3, edge.Target);
            Assert.IsTrue(chain.TryGetSuccessor(edges, 3, out edge));
            Assert.AreEqual(1, edge.Source);
            Assert.AreEqual(2, edge.Target);
            Assert.IsTrue(chain.TryGetSuccessor(edges, 2, out edge));
            Assert.AreEqual(5, edge.Source);
            Assert.AreEqual(2, edge.Target);
            // Etc.
        }
    }
}