using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using NUnit.Framework;
using QuikGraph.Algorithms;
using QuikGraph.Algorithms.ShortestPath;
using QuikGraph.Algorithms.TSP;
using QuikGraph.Tests.Algorithms.ShortestPath;
using QuikGraph.Tests.Algorithms.TSP;

namespace QuikGraph.Tests.Algorithms.Contracts
{
    /// <summary>
    /// Contract tests for <see cref="IDistancesCollection{TVertex}"/>.
    /// </summary>
    [TestFixtureSource(typeof(AlgorithmsProvider), nameof(AlgorithmsProvider.DistanceCollectors))]
    internal abstract class DistancesCollectionContractBase
    {
        [NotNull]
        private readonly Type _testedAlgorithm;

        /// <summary/>
        protected DistancesCollectionContractBase([NotNull] Type algorithmToTest)
        {
            _testedAlgorithm = algorithmToTest;
        }

        [SetUp]
        public void IgnoreIfInterfaceIsNotImplemented()
        {
            if (_testedAlgorithm == typeof(TSP<,,>))
            {
                Assert.Ignore("The TSP algorithm does not implement distance collection functionality.");
            }
        }

        [Pure]
        [NotNull]
        protected IDistancesCollection<T> CreateAlgorithmAndMaybeDoComputation<T>(
            [NotNull] ContractScenario<T> scenario)
        {
            Func<ContractScenario<T>, IDistancesCollection<T>> instantiateAlgorithm = GetAlgorithmFactory<T>();
            return instantiateAlgorithm(scenario);
        }

        [Pure]
        [NotNull]
        private Func<ContractScenario<T>, IDistancesCollection<T>> GetAlgorithmFactory<T>()
        {
            return _testedAlgorithm switch
            {
                _ when _testedAlgorithm == typeof(AStarShortestPathAlgorithm<,>) =>
                    AStartShortestPathAlgorithmTests.CreateAlgorithmAndMaybeDoComputation,

                _ when _testedAlgorithm == typeof(BellmanFordShortestPathAlgorithm<,>) =>
                    BellmanFordShortestPathAlgorithmTests.CreateAlgorithmAndMaybeDoComputation,

                _ when _testedAlgorithm == typeof(DagShortestPathAlgorithm<,>) =>
                    DagShortestPathAlgorithmTests.CreateAlgorithmAndMaybeDoComputation,

                _ when _testedAlgorithm == typeof(DijkstraShortestPathAlgorithm<,>) =>
                    DijkstraShortestPathAlgorithmTests.CreateAlgorithmAndMaybeDoComputation,

                _ when _testedAlgorithm == typeof(TSP<,,>) =>
                    TSPTests.CreateAlgorithmAndMaybeDoComputation,

                _ when _testedAlgorithm == typeof(UndirectedDijkstraShortestPathAlgorithm<,>) =>
                    UndirectedDijkstraShortestPathAlgorithmTests.CreateAlgorithmAndMaybeDoComputation,

                _ => throw new AssertionException($"No test constructor known for {_testedAlgorithm}.")
            };
        }
    }

    internal sealed class TheTryGetDistanceMethod : DistancesCollectionContractBase
    {
        public TheTryGetDistanceMethod([NotNull] Type algorithmToTest) : base(algorithmToTest)
        {
        }

        [Test]
        public void NoDistanceFound_WhenVertexDoesNotExistInGraph()
        {
            var scenario = new ContractScenario<int>
            {
                EdgesInGraph = new[] { Edge.Create(1, 2) },
                AccessibleVerticesFromRoot = new[] { 2 },
                Root = 1,
                DoComputation = true
            };

            IDistancesCollection<int> algorithm = CreateAlgorithmAndMaybeDoComputation(scenario);

            bool distanceFound = algorithm.TryGetDistance(3, out _);
            Assert.False(distanceFound, "No distance should have been found since the vertex does not exist.");
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

            IDistancesCollection<int> algorithm = CreateAlgorithmAndMaybeDoComputation(scenario);

            Assert.Throws<InvalidOperationException>(() => algorithm.TryGetDistance(2, out _));
        }

        [Test]
        public void ExceptionThrown_WhenTargetVertexIsNull()
        {
            var scenario = new ContractScenario<string>
            {
                EdgesInGraph = new[] { Edge.Create("1", "2") },
                SingleVerticesInGraph = new string[0],
                AccessibleVerticesFromRoot = new[] { "2" },
                Root = "1",
                DoComputation = false
            };

            IDistancesCollection<string> algorithm = CreateAlgorithmAndMaybeDoComputation(scenario);

            // ReSharper disable once AssignNullToNotNullAttribute
            Assert.Throws<ArgumentNullException>(() => algorithm.TryGetDistance(null, out _));
        }

        [Test]
        public void DistanceReturned_WhenVertexIsAccessibleFromRoot()
        {
            var scenario = new ContractScenario<int>
            {
                EdgesInGraph = new[] { Edge.Create(1, 2) },
                AccessibleVerticesFromRoot = new[] { 2 },
                Root = 1,
                DoComputation = true
            };

            IDistancesCollection<int> algorithm = CreateAlgorithmAndMaybeDoComputation(scenario);

            bool distanceFound = algorithm.TryGetDistance(2, out _);
            Assert.True(distanceFound, "Distance should have been found since the vertex is accessible from root.");
        }

        [Test]
        public void DistanceReturned_WhenVertexExistsButIsInaccessibleFromRoot()
        {
            var scenario = new ContractScenario<int>
            {
                EdgesInGraph = new[] { Edge.Create(1, 2) },
                SingleVerticesInGraph = new[] { 3 },
                AccessibleVerticesFromRoot = new[] { 2 },
                Root = 1,
                DoComputation = true
            };

            IDistancesCollection<int> algorithm = CreateAlgorithmAndMaybeDoComputation(scenario);

            bool distanceFound = algorithm.TryGetDistance(3, out _);
            Assert.True(distanceFound, "Distance should have been found since the vertex exist in the graph.");
        }
    }

    internal sealed class TheGetDistanceMethod : DistancesCollectionContractBase
    {
        public TheGetDistanceMethod([NotNull] Type algorithmToTest) : base(algorithmToTest)
        {
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

            IDistancesCollection<int> algorithm = CreateAlgorithmAndMaybeDoComputation(scenario);

            Assert.IsNaN(algorithm.GetDistance(3));
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

            IDistancesCollection<int> algorithm = CreateAlgorithmAndMaybeDoComputation(scenario);

            Assert.Throws<InvalidOperationException>(() => { double _ = algorithm.GetDistance(2); });
        }

        [Test]
        public void ExceptionThrown_WhenTargetVertexIsNull()
        {
            var scenario = new ContractScenario<string>
            {
                EdgesInGraph = new[] { Edge.Create("1", "2") },
                SingleVerticesInGraph = new string[0],
                AccessibleVerticesFromRoot = new[] { "2" },
                Root = "1",
                DoComputation = false
            };

            IDistancesCollection<string> algorithm = CreateAlgorithmAndMaybeDoComputation(scenario);

            // ReSharper disable once AssignNullToNotNullAttribute
            Assert.Throws<ArgumentNullException>(() => { double _ = algorithm.GetDistance(null); });
        }

        [Test]
        public void NoExceptionThrown_WhenVertexIsAccessibleFromRoot()
        {
            var scenario = new ContractScenario<int>
            {
                EdgesInGraph = new[] { Edge.Create(1, 2) },
                AccessibleVerticesFromRoot = new[] { 2 },
                Root = 1,
                DoComputation = true
            };

            IDistancesCollection<int> algorithm = CreateAlgorithmAndMaybeDoComputation(scenario);

            Assert.DoesNotThrow(() => { double _ = algorithm.GetDistance(2); });
        }

        [Test]
        public void NoExceptionThrown_WhenVertexExistsButIsInaccessibleFromRoot()
        {
            var scenario = new ContractScenario<int>
            {
                EdgesInGraph = new[] { Edge.Create(1, 2) },
                SingleVerticesInGraph = new[] { 3 },
                AccessibleVerticesFromRoot = new[] { 2 },
                Root = 1,
                DoComputation = true
            };

            IDistancesCollection<int> algorithm = CreateAlgorithmAndMaybeDoComputation(scenario);

            Assert.DoesNotThrow(() => { double _ = algorithm.GetDistance(3); });
        }
    }

    internal sealed class TheGetKnownDistancesMethod : DistancesCollectionContractBase
    {
        public TheGetKnownDistancesMethod([NotNull] Type algorithmToTest) : base(algorithmToTest)
        {
        }

        [Test]
        public void DistancesForAllVerticesInGraphReturnedWhenAlgorithmHasBeenRun()
        {
            var scenario = new ContractScenario<int>
            {
                EdgesInGraph = new[] { Edge.Create(1, 2) },
                SingleVerticesInGraph = new[] { 3 },
                AccessibleVerticesFromRoot = new[] { 2 },
                Root = 1,
                DoComputation = true
            };

            IDistancesCollection<int> algorithm = CreateAlgorithmAndMaybeDoComputation(scenario);

            IEnumerable<KeyValuePair<int, double>> distances = algorithm.GetDistances();
            CollectionAssert.AreEquivalent(new[] { 1, 2, 3 }, distances.Select(pair => pair.Key));
        }

        [Test]
        public void EmptyCollectionReturned_WhenAlgorithmHasNotYetBeenRun()
        {
            var scenario = new ContractScenario<int>
            {
                EdgesInGraph = new[] { Edge.Create(1, 2) },
                SingleVerticesInGraph = new[] { 3 },
                AccessibleVerticesFromRoot = new[] { 2 },
                Root = 1,
                DoComputation = false
            };

            IDistancesCollection<int> algorithm = CreateAlgorithmAndMaybeDoComputation(scenario);

            IEnumerable<KeyValuePair<int, double>> distances = algorithm.GetDistances();
            CollectionAssert.IsEmpty(distances);
        }
    }
}