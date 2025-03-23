using System;
using JetBrains.Annotations;
using NUnit.Framework;
using QuikGraph.Algorithms;

namespace QuikGraph.Tests.Algorithms.Contracts
{
    /// <summary>
    /// Tests related to <see cref="IDistancesCollection{TVertex}.GetDistance"/>.
    /// </summary>
    internal sealed class DistancesCollectionGetDistanceContract : DistancesCollectionContractBase
    {
        public DistancesCollectionGetDistanceContract([NotNull] Type algorithmToTest)
            : base(algorithmToTest)
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
}