using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using QuikGraph.Algorithms.Ranking;


namespace QuikGraph.Tests.Algorithms
{
    /// <summary>
    /// Tests for <see cref="PageRankAlgorithm{TVertex,TEdge}"/>.
    /// </summary>
    [TestFixture]
    internal sealed class PageRankAlgorithmTests
    {
        [Test]
        public void Constructor()
        {
            var graph = new BidirectionalGraph<int, IEdge<int>>();
            var algorithm = new PageRankAlgorithm<int, IEdge<int>>(graph);
            AssertAlgorithmProperties(algorithm, graph);

            algorithm = new PageRankAlgorithm<int, IEdge<int>>(graph)
            {
                Damping = 0.96
            };
            AssertAlgorithmProperties(algorithm, graph, 0.96);

            algorithm = new PageRankAlgorithm<int, IEdge<int>>(graph)
            {
                Tolerance = double.Epsilon
            };
            AssertAlgorithmProperties(algorithm, graph, t: double.Epsilon);

            algorithm = new PageRankAlgorithm<int, IEdge<int>>(graph)
            {
                MaxIterations = 12
            };
            AssertAlgorithmProperties(algorithm, graph, iterations: 12);

            algorithm = new PageRankAlgorithm<int, IEdge<int>>(graph)
            {
                Damping = 0.91,
                Tolerance = 3 * double.Epsilon,
                MaxIterations = 50
            };
            AssertAlgorithmProperties(algorithm, graph, 0.91, 3 * double.Epsilon, 50);

            #region Local function

            void AssertAlgorithmProperties<TVertex, TEdge>(
                PageRankAlgorithm<TVertex, TEdge> algo,
                IBidirectionalGraph<TVertex, TEdge> g,
                double d = -1,
                double t = -1,
                int iterations = -1)
                where TEdge : IEdge<TVertex>
            {
                algo.AssertAlgorithmState(g);
                CollectionAssert.IsEmpty(algo.Ranks);
                if (d >= 0)
                {
                    Assert.AreEqual(d, algo.Damping);
                }
                else
                {
                    Assert.GreaterOrEqual(algo.Damping, 0);
                    Assert.LessOrEqual(algo.Damping, 1);
                }
                if (t >= 0)
                {
                    Assert.AreEqual(t, algo.Tolerance);
                }
                else
                {
                    Assert.GreaterOrEqual(algo.Tolerance, 0);
                }
                if (iterations > 0)
                {
                    Assert.AreEqual(iterations, algo.MaxIterations);
                }
                else
                {
                    Assert.Positive(algo.MaxIterations);
                }
            }

            #endregion
        }

        [Test]
        public void Constructor_Throws()
        {
            var graph = new BidirectionalGraph<int, IEdge<int>>();
            // ReSharper disable ObjectCreationAsStatement
            // ReSharper disable once AssignNullToNotNullAttribute
            Assert.Throws<ArgumentNullException>(
                () => new PageRankAlgorithm<int, IEdge<int>>(null));

            var algorithm = new PageRankAlgorithm<int, IEdge<int>>(graph);
            Assert.Throws<ArgumentOutOfRangeException>(() => algorithm.Damping = -10.0);
            Assert.Throws<ArgumentOutOfRangeException>(() => algorithm.Damping = -0.01);
            Assert.Throws<ArgumentOutOfRangeException>(() => algorithm.Damping = 1.01);
            Assert.Throws<ArgumentOutOfRangeException>(() => algorithm.Damping = 10.0);

            Assert.Throws<ArgumentOutOfRangeException>(() => algorithm.Tolerance = -10.0);
            Assert.Throws<ArgumentOutOfRangeException>(() => algorithm.Tolerance = -1);

            Assert.Throws<ArgumentOutOfRangeException>(() => algorithm.MaxIterations = -10);
            Assert.Throws<ArgumentOutOfRangeException>(() => algorithm.MaxIterations = -1);
            // ReSharper restore ObjectCreationAsStatement
        }

        [Test]
        public void PageRank()
        {
            var graph = new BidirectionalGraph<string, IEdge<string>>();
            graph.AddVerticesAndEdgeRange(
                Edge.Create("Amazon", "Twitter"),
                Edge.Create("Amazon", "Microsoft"),
                Edge.Create("Microsoft", "Amazon"),
                Edge.Create("Microsoft", "Facebook"),
                Edge.Create("Microsoft", "Twitter"),
                Edge.Create("Microsoft", "Apple"),
                Edge.Create("Facebook", "Amazon"),
                Edge.Create("Facebook", "Twitter"),
                Edge.Create("Twitter", "Microsoft"),
                Edge.Create("Apple", "Twitter")
            );

            var algorithm = new PageRankAlgorithm<string, IEdge<string>>(graph);
            algorithm.Compute();

            IEnumerable<string> order = algorithm.Ranks.OrderByDescending(pair => pair.Value).Select(pair => pair.Key);
            CollectionAssert.AreEqual(
                new[] { "Microsoft", "Twitter", "Amazon", "Facebook", "Apple" },
                order);
            Assert.Positive(algorithm.GetRanksSum());
            double rankSum = algorithm.Ranks.Sum(pair => pair.Value);
            Assert.AreEqual(rankSum, algorithm.GetRanksSum());

            Assert.Positive(algorithm.GetRanksSum());
            Assert.AreEqual(rankSum / graph.VertexCount, algorithm.GetRanksMean());
        }
    }
}