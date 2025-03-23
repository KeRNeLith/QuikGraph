using System;
using JetBrains.Annotations;
using NUnit.Framework;
using QuikGraph.Algorithms;
using QuikGraph.Algorithms.MaximumFlow;
using QuikGraph.Algorithms.RandomWalks;
using QuikGraph.Algorithms.Search;
using QuikGraph.Algorithms.ShortestPath;
using QuikGraph.Algorithms.TSP;
using QuikGraph.Tests.Algorithms.MaximumFlow;
using QuikGraph.Tests.Algorithms.RandomWalks;
using QuikGraph.Tests.Algorithms.Search;
using QuikGraph.Tests.Algorithms.ShortestPath;
using QuikGraph.Tests.Algorithms.TSP;

// ReSharper disable ReturnValueOfPureMethodIsNotUsed

namespace QuikGraph.Tests.Algorithms.Contracts
{
    /// <summary>
    /// Contract tests for <see cref="IVertexColorizerAlgorithm{TVertex}"/>.
    /// </summary>
    [TestFixtureSource(typeof(AlgorithmsProvider), nameof(AlgorithmsProvider.VertexColorizers))]
    internal sealed class VertexColorizerContract
    {
        [NotNull]
        private readonly Type _testedAlgorithm;

        /// <summary/>
        public VertexColorizerContract([NotNull] Type algorithmToTest)
        {
            _testedAlgorithm = algorithmToTest;
        }

        [SetUp]
        public void IgnoreIfInterfaceIsNotImplemented()
        {
            if (_testedAlgorithm == typeof(TSP<,,>))
            {
                Assert.Ignore("The TSP algorithm does not implement colorizer functionality.");
            }
        }

        [Test]
        public void ExceptionThrown_WhenVertexDoesNotExistInGraph()
        {
            var scenario = new ContractScenario<int>
            {
                EdgesInGraph = new[] { Edge.Create(1, 2) },
                AccessibleVerticesFromRoot = new[] { 2 },
                Root = 1,
                DoComputation = true
            };

            IVertexColorizerAlgorithm<int> algorithm = CreateAlgorithmAndMaybeDoComputation(scenario);

            Assert.IsNull(algorithm.GetVertexColor(3));
        }

        [Test]
        public void ExceptionThrown_WhenAlgorithmHasNotYetBeenComputed()
        {
            var scenario = new ContractScenario<int>
            {
                EdgesInGraph = new[] { Edge.Create(1, 2) },
                SingleVerticesInGraph = new int[0],
                AccessibleVerticesFromRoot = new[] { 2 },
                Root = 1,
                DoComputation = false
            };

            IVertexColorizerAlgorithm<int> algorithm = CreateAlgorithmAndMaybeDoComputation(scenario);

            Type expectedExceptionType = GetExpectedExceptionType();
            if (expectedExceptionType == null)
            {
                Assert.IsNull(algorithm.GetVertexColor(2));
            }
            else
            {
                Assert.Throws(expectedExceptionType, () => algorithm.GetVertexColor(2));
            }

            #region Local function

            Type GetExpectedExceptionType()
            {
                switch (_testedAlgorithm)
                {
                    case var _ when _testedAlgorithm == typeof(AStarShortestPathAlgorithm<,>):
                    case var _ when _testedAlgorithm == typeof(BellmanFordShortestPathAlgorithm<,>):
                    case var _ when _testedAlgorithm == typeof(DagShortestPathAlgorithm<,>):
                    case var _ when _testedAlgorithm == typeof(DijkstraShortestPathAlgorithm<,>):
                    case var _ when _testedAlgorithm == typeof(UndirectedDijkstraShortestPathAlgorithm<,>):
                        return typeof(NullReferenceException);
                }

                return null; //typeof(Exception);
            }

            #endregion
        }

        [Test]
        public void ColorReturned_WhenVertexIsAccessibleFromRoot()
        {
            var scenario = new ContractScenario<int>
            {
                EdgesInGraph = new[] { Edge.Create(1, 2) },
                AccessibleVerticesFromRoot = new[] { 2 },
                Root = 1,
                DoComputation = true
            };

            IVertexColorizerAlgorithm<int> algorithm = CreateAlgorithmAndMaybeDoComputation(scenario);

            Assert.IsNotNull(algorithm.GetVertexColor(2));
        }

        [Test]
        public void ColorReturned_WhenVertexExistsButIsInaccessibleFromRoot()
        {
            var scenario = new ContractScenario<int>
            {
                EdgesInGraph = new[] { Edge.Create(1, 2) },
                SingleVerticesInGraph = new[] { 3 },
                AccessibleVerticesFromRoot = new[] { 2 },
                Root = 1,
                DoComputation = true
            };

            IVertexColorizerAlgorithm<int> algorithm = CreateAlgorithmAndMaybeDoComputation(scenario);

            Assert.IsNotNull(algorithm.GetVertexColor(3));
        }

        [Pure]
        [NotNull]
        private IVertexColorizerAlgorithm<T> CreateAlgorithmAndMaybeDoComputation<T>(
            [NotNull] ContractScenario<T> scenario)
        {
            var instantiateAlgorithm = GetAlgorithmFactory<T>();
            return instantiateAlgorithm(scenario);
        }

        [Pure]
        [NotNull]
        private Func<ContractScenario<T>, IVertexColorizerAlgorithm<T>> GetAlgorithmFactory<T>()
        {
            return _testedAlgorithm switch
            {
                _ when _testedAlgorithm == typeof(AStarShortestPathAlgorithm<,>) =>
                    AStartShortestPathAlgorithmTests.CreateAlgorithmAndMaybeDoComputation,

                _ when _testedAlgorithm == typeof(BellmanFordShortestPathAlgorithm<,>) =>
                    BellmanFordShortestPathAlgorithmTests.CreateAlgorithmAndMaybeDoComputation,

                _ when _testedAlgorithm == typeof(BidirectionalDepthFirstSearchAlgorithm<,>) =>
                    BidirectionalDepthFirstSearchAlgorithmTests.CreateAlgorithmAndMaybeDoComputation,

                _ when _testedAlgorithm == typeof(BreadthFirstSearchAlgorithm<,>) =>
                    BreadthFirstAlgorithmSearchTests.CreateAlgorithmAndMaybeDoComputation,

                _ when _testedAlgorithm == typeof(CyclePoppingRandomTreeAlgorithm<,>) =>
                    CyclePoppingRandomTreeAlgorithmTests.CreateAlgorithmAndMaybeDoComputation,

                _ when _testedAlgorithm == typeof(DagShortestPathAlgorithm<,>) =>
                    DagShortestPathAlgorithmTests.CreateAlgorithmAndMaybeDoComputation,

                _ when _testedAlgorithm == typeof(DepthFirstSearchAlgorithm<,>) =>
                    DepthFirstAlgorithmSearchTests.CreateAlgorithmAndMaybeDoComputation,

                _ when _testedAlgorithm == typeof(DijkstraShortestPathAlgorithm<,>) =>
                    DijkstraShortestPathAlgorithmTests.CreateAlgorithmAndMaybeDoComputation,

                _ when _testedAlgorithm == typeof(EdmondsKarpMaximumFlowAlgorithm<,>) =>
                    EdmondsKarpMaximumFlowAlgorithmTests.CreateAlgorithmAndMaybeDoComputation,

                _ when _testedAlgorithm == typeof(TSP<,,>) =>
                    TSPTests.CreateAlgorithmAndMaybeDoComputation,

                _ when _testedAlgorithm == typeof(UndirectedBreadthFirstSearchAlgorithm<,>) =>
                    UndirectedBreadthFirstAlgorithmSearchTests.CreateAlgorithmAndMaybeDoComputation,

                _ when _testedAlgorithm == typeof(UndirectedDepthFirstSearchAlgorithm<,>) =>
                    UndirectedDepthFirstAlgorithmSearchTests.CreateAlgorithmAndMaybeDoComputation,

                _ when _testedAlgorithm == typeof(UndirectedDijkstraShortestPathAlgorithm<,>) =>
                    UndirectedDijkstraShortestPathAlgorithmTests.CreateAlgorithmAndMaybeDoComputation,

                _ => throw new AssertionException($"No test constructor known for {_testedAlgorithm}.")
            };
        }
    }
}