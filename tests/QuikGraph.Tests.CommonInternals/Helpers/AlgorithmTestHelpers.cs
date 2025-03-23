using JetBrains.Annotations;
using NUnit.Framework;
using QuikGraph.Algorithms;

namespace QuikGraph.Tests.Algorithms
{
    /// <summary>
    /// Test helpers for algorithms.
    /// </summary>
    internal static class AlgorithmTestHelpers
    {
        public static void AssertAlgorithmState<TGraph>(
            [NotNull] this AlgorithmBase<TGraph> algorithm,
            [NotNull] TGraph treatedGraph,
            ComputationState state = ComputationState.NotRunning)
        {
            Assert.IsNotNull(treatedGraph);
            Assert.AreSame(treatedGraph, algorithm.VisitedGraph);
            Assert.IsNotNull(algorithm.Services);
            Assert.IsNotNull(algorithm.SyncRoot);
            Assert.AreEqual(state, algorithm.State);
        }
    }
}