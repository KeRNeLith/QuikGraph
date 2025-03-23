//using System;
//using NUnit.Framework;
//using QuikGraph.Algorithms;

//namespace QuikGraph.Tests.Algorithms.MaximumFlow
//{
//    /// <summary>
//    /// Tests for <see cref="CentralityApproximationAlgorithm{TVertex,TEdge}"/>.
//    /// </summary>
//    [TestFixture]
//    internal sealed class CentralityApproximationAlgorithmTests : AlgorithmTestsBase
//    {
//        [Test]
//        public void Constructor()
//        {
//            var graph = new AdjacencyGraph<int, IEdge<int>>();
//            Func<Edge<int>, double> distances = edge => 1.0;

//            var algorithm = new CentralityApproximationAlgorithm<int, IEdge<int>>(graph, distances);
//            AssertAlgorithmProperties(algorithm, graph, distances);

//            var random = new Random(123456);
//            algorithm = new CentralityApproximationAlgorithm<int, IEdge<int>>(graph, distances)
//            {
//                Rand = random
//            };
//            AssertAlgorithmProperties(algorithm, graph, distances, random);

//            algorithm = new CentralityApproximationAlgorithm<int, IEdge<int>>(graph, distances)
//            {
//                MaxIterationCount = 12
//            };
//            AssertAlgorithmProperties(algorithm, graph, distances, iterations: 12);

//            #region Local function

//            void AssertAlgorithmProperties<TVertex, TEdge>(
//                CentralityApproximationAlgorithm<TVertex, TEdge> algo,
//                IVertexListGraph<TVertex, TEdge> g,
//                Func<TEdge, double> d,
//                Random r = null,
//                int iterations = -1)
//                where TEdge : IEdge<TVertex>
//            {
//                AssertAlgorithmState(algo, g);
//                Assert.AreSame(d, algo.Distances);
//                if (r is null)
//                    Assert.IsNotNull(algo.Rand);
//                else
//                    Assert.AreSame(r, algo.Rand);
//                if (iterations < 0)
//                    Assert.Positive(algo.MaxIterationCount);
//                else
//                    Assert.AreEqual(iterations, algo.MaxIterationCount);
//            }

//            #endregion
//        }

//        [Test]
//        public void Constructor_Throws()
//        {
//            var graph = new AdjacencyGraph<int, IEdge<int>>();
//            Func<Edge<int>, double> distances = edge => 1.0;

//            // ReSharper disable ObjectCreationAsStatement
//            // ReSharper disable AssignNullToNotNullAttribute
//            Assert.Throws<ArgumentNullException>(
//                () => new CentralityApproximationAlgorithm<int, IEdge<int>>(null, distances));
//            Assert.Throws<ArgumentNullException>(
//                () => new CentralityApproximationAlgorithm<int, IEdge<int>>(graph, null));
//            Assert.Throws<ArgumentNullException>(
//                () => new CentralityApproximationAlgorithm<int, IEdge<int>>(null, null));

//            var algorithm = new CentralityApproximationAlgorithm<int, IEdge<int>>(graph, distances);
//            Assert.Throws<ArgumentOutOfRangeException>(() => algorithm.MaxIterationCount = 0);
//            Assert.Throws<ArgumentOutOfRangeException>(() => algorithm.MaxIterationCount = -1);
//            Assert.Throws<ArgumentOutOfRangeException>(() => algorithm.MaxIterationCount = -10);

//            Assert.Throws<ArgumentNullException>(() => algorithm.Rand = null);
//            // ReSharper restore AssignNullToNotNullAttribute
//            // ReSharper restore ObjectCreationAsStatement
//        }

//        [Test]
//        public void SimpleGraph()
//        {
//            var graph = new AdjacencyGraph<string, IEdge<string>>();
//            graph.AddVerticesAndEdgeRange(new[]
//            {
//                Edge.Create("Mark", "Alice"),
//                Edge.Create("Alice", "Bridget"),
//                Edge.Create("Alice", "Doug"),
//                Edge.Create("Alice", "Charles"),
//                Edge.Create("Charles", "Michael")
//            });
//            Func<Edge<string>, double> distances = edge => 1.0;

//            var random = new Random(123456);
//            var algorithm = new CentralityApproximationAlgorithm<string, IEdge<string>>(graph, distances)
//            {
//                Rand = random,
//                MaxIterationCount = 50
//            };
//            algorithm.Compute();

//            // Do tests
//        }
//    }
//}