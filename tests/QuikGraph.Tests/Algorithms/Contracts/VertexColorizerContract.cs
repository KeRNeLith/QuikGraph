using System;
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
    [TestFixtureSource(typeof(Feeder), nameof(Feeder.VertexColorizers))]
    public class VertexColorizerContract
    {
        private readonly Type _classUnderTest;

        public VertexColorizerContract(Type testClassType)
        {
            _classUnderTest = testClassType;
        }

        [SetUp]
        public void IgnoreIfInterfaceIsNotImplemented()
        {
            if (_classUnderTest == typeof(TSP<,,>))
                Assert.Ignore("The TSP algorithm does not implement colorizer functionality");
        }

        [Test]
        public void ExceptionThrown_WhenVertexDoesNotExistInGraph()
        {
            var scenario = new ContractScenario
            {
                EdgesInGraph = new[] {new Edge<int>(1, 2)},
                AccessibleVerticesFromRoot = new[] {2},
                Root = 1,
                DoComputation = true
            };

            var algorithm = CreateAlgorithmAndMaybeDoComputation(scenario);

            Assert.Throws<VertexNotFoundException>(() => algorithm.GetVertexColor(3));
        }

        [Test]
        public void ExceptionThrown_WhenAlgorithmHasNotYetBeenComputed()
        {
            var scenario = new ContractScenario
            {
                EdgesInGraph = new[] {new Edge<int>(1, 2)},
                SingleVerticesInGraph = new int[0],
                AccessibleVerticesFromRoot = new[] {2},
                Root = 1,
                DoComputation = false
            };

            var algorithm = CreateAlgorithmAndMaybeDoComputation(scenario);

            var expectedExceptionType = GetExpectedExceptionType();
            Assert.Throws(expectedExceptionType, () => algorithm.GetVertexColor(2));

            Type GetExpectedExceptionType()
            {
                switch (_classUnderTest)
                {
                    case var _ when _classUnderTest == typeof(AStarShortestPathAlgorithm<,>):
                    case var _ when _classUnderTest == typeof(BellmanFordShortestPathAlgorithm<,>):
                    case var _ when _classUnderTest == typeof(DagShortestPathAlgorithm<,>):
                    case var _ when _classUnderTest == typeof(DijkstraShortestPathAlgorithm<,>):
                    case var _ when _classUnderTest == typeof(UndirectedDijkstraShortestPathAlgorithm<,>):
                        return typeof(NullReferenceException);

                    case var _ when _classUnderTest == typeof(BidirectionalDepthFirstSearchAlgorithm<,>):
                    case var _ when _classUnderTest == typeof(BreadthFirstSearchAlgorithm<,>):
                    case var _ when _classUnderTest == typeof(CyclePoppingRandomTreeAlgorithm<,>):
                    case var _ when _classUnderTest == typeof(DepthFirstSearchAlgorithm<,>):
                    case var _ when _classUnderTest == typeof(EdmondsKarpMaximumFlowAlgorithm<,>):
                    case var _ when _classUnderTest == typeof(TSP<,,>):
                    case var _ when _classUnderTest == typeof(UndirectedBreadthFirstSearchAlgorithm<,>):
                    case var _ when _classUnderTest == typeof(UndirectedDepthFirstSearchAlgorithm<,>):
                        return typeof(VertexNotFoundException);

                    default:
                        throw new AssertionException($"No test constructor known for {_classUnderTest}");
                }
            }
        }

        [Test]
        public void ColorReturned_WhenVertexIsAccessibleFromRoot()
        {
            var scenario = new ContractScenario
            {
                EdgesInGraph = new[] {new Edge<int>(1, 2)},
                AccessibleVerticesFromRoot = new[] {2},
                Root = 1,
                DoComputation = true
            };

            var algorithm = CreateAlgorithmAndMaybeDoComputation(scenario);

            Assert.NotNull(algorithm.GetVertexColor(2));
        }

        [Test]
        public void ColorReturned_WhenVertexExistsButIsInaccessibleFromRoot()
        {
            var scenario = new ContractScenario
            {
                EdgesInGraph = new[] {new Edge<int>(1, 2)},
                SingleVerticesInGraph = new[] {3},
                AccessibleVerticesFromRoot = new[] {2},
                Root = 1,
                DoComputation = true
            };

            var algorithm = CreateAlgorithmAndMaybeDoComputation(scenario);

            Assert.NotNull(algorithm.GetVertexColor(3));
        }

        private IVertexColorizerAlgorithm<int> CreateAlgorithmAndMaybeDoComputation(
            ContractScenario scenario)
        {
            var instantiateAlgorithm = GetAlgorithmInstantiator();
            return instantiateAlgorithm(scenario);
        }

        private Func<ContractScenario, IVertexColorizerAlgorithm<int>> GetAlgorithmInstantiator()
        {
            return _classUnderTest switch
            {
                _ when _classUnderTest == typeof(AStarShortestPathAlgorithm<,>) =>
                    AStartShortestPathAlgorithmTests.CreateAlgorithmAndMaybeDoComputation,

                _ when _classUnderTest == typeof(BellmanFordShortestPathAlgorithm<,>) =>
                    BellmanFordShortestPathAlgorithmTests.CreateAlgorithmAndMaybeDoComputation,

                _ when _classUnderTest == typeof(BidirectionalDepthFirstSearchAlgorithm<,>) =>
                    BidirectionalDepthFirstSearchAlgorithmTests.CreateAlgorithmAndMaybeDoComputation,

                _ when _classUnderTest == typeof(BreadthFirstSearchAlgorithm<,>) =>
                    BreadthFirstAlgorithmSearchTests.CreateAlgorithmAndMaybeDoComputation,

                _ when _classUnderTest == typeof(CyclePoppingRandomTreeAlgorithm<,>) =>
                    CyclePoppingRandomTreeAlgorithmTests.CreateAlgorithmAndMaybeDoComputation,

                _ when _classUnderTest == typeof(DagShortestPathAlgorithm<,>) =>
                    DagShortestPathAlgorithmTests.CreateAlgorithmAndMaybeDoComputation,

                _ when _classUnderTest == typeof(DepthFirstSearchAlgorithm<,>) =>
                    DepthFirstAlgorithmSearchTests.CreateAlgorithmAndMaybeDoComputation,

                _ when _classUnderTest == typeof(DijkstraShortestPathAlgorithm<,>) =>
                    DijkstraShortestPathAlgorithmTests.CreateAlgorithmAndMaybeDoComputation,

                _ when _classUnderTest == typeof(EdmondsKarpMaximumFlowAlgorithm<,>) =>
                    EdmondsKarpMaximumFlowAlgorithmTests.CreateAlgorithmAndMaybeDoComputation,

                _ when _classUnderTest == typeof(TSP<,,>) =>
                    TSPTests.CreateAlgorithmAndMaybeDoComputation,

                _ when _classUnderTest == typeof(UndirectedBreadthFirstSearchAlgorithm<,>) =>
                    UndirectedBreadthFirstAlgorithmSearchTests.CreateAlgorithmAndMaybeDoComputation,

                _ when _classUnderTest == typeof(UndirectedDepthFirstSearchAlgorithm<,>) =>
                    UndirectedDepthFirstAlgorithmSearchTests.CreateAlgorithmAndMaybeDoComputation,

                _ when _classUnderTest == typeof(UndirectedDijkstraShortestPathAlgorithm<,>) =>
                    UndirectedDijkstraShortestPathAlgorithmTests.CreateAlgorithmAndMaybeDoComputation,

                _ => throw new AssertionException($"No test constructor known for {_classUnderTest}")
            };
        }
    }
}