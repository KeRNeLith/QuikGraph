using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using QuikGraph.Algorithms;
using static QuikGraph.Tests.TestHelpers;

namespace QuikGraph.Tests.Algorithms
{
    /// <summary>
    /// Tests for <see cref="IsHamiltonianGraphAlgorithm{TVertex,TEdge}"/>.
    /// </summary>
    [TestFixture]
    internal sealed class HamiltonianGraphAlgorithmTests
    {
        [Test]
        public void IsHamiltonianEmpty()
        {
            var graph = CreateUndirectedGraph();

            Assert.IsFalse(graph.IsHamiltonian());
        }

        [Test]
        public void IsHamiltonian()
        {
            // Hamiltonian
            var graph = CreateUndirectedGraph(
                Edge.CreateUndirected(1, 2),
                Edge.CreateUndirected(2, 3),
                Edge.CreateUndirected(1, 3),
                Edge.CreateUndirected(2, 4),
                Edge.CreateUndirected(3, 4));

            Assert.IsTrue(graph.IsHamiltonian());

            // Not Hamiltonian
            graph = CreateUndirectedGraph(
                Edge.CreateUndirected(1, 2),
                Edge.CreateUndirected(2, 3),
                Edge.CreateUndirected(2, 4),
                Edge.CreateUndirected(3, 4)
            );

            Assert.IsFalse(graph.IsHamiltonian());
        }

        [Test]
        public void IsHamiltonianOneVertexWithCycle()
        {
            var graph = CreateUndirectedGraph(Edge.CreateUndirected(1, 1));

            Assert.IsTrue(graph.IsHamiltonian());
        }

        [Test]
        public void IsHamiltonianTwoVertices()
        {
            // Hamiltonian
            var graph = CreateUndirectedGraph(Edge.CreateUndirected(1, 2));

            Assert.IsTrue(graph.IsHamiltonian());

            // Not Hamiltonian
            graph = CreateUndirectedGraph(
                Edge.CreateUndirected(1, 1),
                Edge.CreateUndirected(2, 2));

            Assert.IsFalse(graph.IsHamiltonian());
        }

        [Test]
        public void IsHamiltonianWithLoops()
        {
            var graph = CreateUndirectedGraph(
                Edge.CreateUndirected(1, 1),
                Edge.CreateUndirected(1, 1),
                Edge.CreateUndirected(2, 2),
                Edge.CreateUndirected(2, 2),
                Edge.CreateUndirected(2, 2),
                Edge.CreateUndirected(3, 3),
                Edge.CreateUndirected(3, 3)
            );

            Assert.IsFalse(graph.IsHamiltonian());
        }

        [Test]
        public void IsHamiltonianWithParallelEdges()
        {
            var graph = CreateUndirectedGraph(
                Edge.CreateUndirected(1, 2),
                Edge.CreateUndirected(1, 2),
                Edge.CreateUndirected(3, 4),
                Edge.CreateUndirected(3, 4));

            Assert.IsFalse(graph.IsHamiltonian());
        }

        [Test]
        public void IsHamiltonianDiracsTheorem()
        {
            // This graph is Hamiltonian and satisfies Dirac's theorem. This test should work faster
            var graph = CreateUndirectedGraph(
                Edge.CreateUndirected(1, 2),
                Edge.CreateUndirected(1, 3),
                Edge.CreateUndirected(1, 4),
                Edge.CreateUndirected(1, 7),
                Edge.CreateUndirected(1, 8),
                Edge.CreateUndirected(1, 10),
                Edge.CreateUndirected(2, 6),
                Edge.CreateUndirected(2, 9),
                Edge.CreateUndirected(2, 4),
                Edge.CreateUndirected(2, 5),
                Edge.CreateUndirected(3, 4),
                Edge.CreateUndirected(3, 6),
                Edge.CreateUndirected(3, 7),
                Edge.CreateUndirected(3, 8),
                Edge.CreateUndirected(3, 8),
                Edge.CreateUndirected(4, 6),
                Edge.CreateUndirected(4, 5),
                Edge.CreateUndirected(4, 7),
                Edge.CreateUndirected(5, 7),
                Edge.CreateUndirected(5, 6),
                Edge.CreateUndirected(5, 9),
                Edge.CreateUndirected(5, 10),
                Edge.CreateUndirected(6, 9),
                Edge.CreateUndirected(6, 10),
                Edge.CreateUndirected(6, 7),
                Edge.CreateUndirected(7, 8),
                Edge.CreateUndirected(8, 9),
                Edge.CreateUndirected(8, 10),
                Edge.CreateUndirected(9, 10));

            Assert.IsTrue(graph.IsHamiltonian());
        }

        [Test]
        public void IsHamiltonianNotDiracsTheorem()
        {
            // This graph is Hamiltonian but don't satisfy Dirac's theorem. This test should work slowlier
            var graph = CreateUndirectedGraph(
                Edge.CreateUndirected(1, 2),
                Edge.CreateUndirected(1, 3),
                Edge.CreateUndirected(1, 4),
                Edge.CreateUndirected(1, 7),
                Edge.CreateUndirected(1, 8),
                Edge.CreateUndirected(1, 10),
                Edge.CreateUndirected(2, 6),
                Edge.CreateUndirected(2, 9),
                Edge.CreateUndirected(2, 4),
                Edge.CreateUndirected(3, 4),
                Edge.CreateUndirected(3, 6),
                Edge.CreateUndirected(3, 7),
                Edge.CreateUndirected(3, 8),
                Edge.CreateUndirected(4, 6),
                Edge.CreateUndirected(4, 5),
                Edge.CreateUndirected(4, 7),
                Edge.CreateUndirected(5, 7),
                Edge.CreateUndirected(5, 6),
                Edge.CreateUndirected(5, 9),
                Edge.CreateUndirected(5, 10),
                Edge.CreateUndirected(6, 9),
                Edge.CreateUndirected(6, 10),
                Edge.CreateUndirected(6, 7),
                Edge.CreateUndirected(7, 8),
                Edge.CreateUndirected(8, 9),
                Edge.CreateUndirected(8, 10),
                Edge.CreateUndirected(9, 10));

            Assert.IsTrue(graph.IsHamiltonian());
        }

        #region Test helpers

        private class SequenceComparer<T>
#if SUPPORTS_ENUMERABLE_COVARIANT
            : IEqualityComparer<IEnumerable<T>>
#else
            : IEqualityComparer<List<T>>
#endif
        {
#if SUPPORTS_ENUMERABLE_COVARIANT
            public bool Equals(IEnumerable<T> seq1, IEnumerable<T> seq2)
#else
            public bool Equals(List<T> seq1, List<T> seq2)
#endif
            {
                if (seq1 is null)
                    return seq2 is null;
                if (seq2 is null)
                    return false;
                return seq1.SequenceEqual(seq2);
            }

#if SUPPORTS_ENUMERABLE_COVARIANT
            public int GetHashCode(IEnumerable<T> seq)
#else
            public int GetHashCode(List<T> seq)
#endif
            {
                int hash = 1234567;
                foreach (T elem in seq)
                    hash = hash * 37 + elem.GetHashCode();
                return hash;
            }
        }

        private static int Factorial(int i)
        {
            if (i <= 1)
                return 1;
            return i * Factorial(i - 1);
        }

        #endregion

        [Test]
        public void IsHamiltonianCyclesBuilder()
        {
            var graph = CreateUndirectedGraph(
                Edge.CreateUndirected(1, 2),
                Edge.CreateUndirected(1, 3),
                Edge.CreateUndirected(1, 4),
                Edge.CreateUndirected(1, 7),
                Edge.CreateUndirected(1, 8),
                Edge.CreateUndirected(1, 10),
                Edge.CreateUndirected(2, 6),
                Edge.CreateUndirected(2, 9),
                Edge.CreateUndirected(2, 4),
                Edge.CreateUndirected(3, 4),
                Edge.CreateUndirected(3, 6),
                Edge.CreateUndirected(3, 7),
                Edge.CreateUndirected(3, 8),
                Edge.CreateUndirected(4, 6),
                Edge.CreateUndirected(4, 5),
                Edge.CreateUndirected(4, 7),
                Edge.CreateUndirected(5, 7),
                Edge.CreateUndirected(5, 6),
                Edge.CreateUndirected(5, 9),
                Edge.CreateUndirected(5, 10),
                Edge.CreateUndirected(6, 9),
                Edge.CreateUndirected(6, 10),
                Edge.CreateUndirected(6, 7),
                Edge.CreateUndirected(7, 8),
                Edge.CreateUndirected(8, 9),
                Edge.CreateUndirected(8, 10),
                Edge.CreateUndirected(9, 10));

            var algorithm = graph.CreateHamiltonianGraphAlgorithm();

            var hashSet = new HashSet<List<int>>(new SequenceComparer<int>());
            hashSet.UnionWith(algorithm.GetAllVertexPermutations());

            Assert.AreEqual(hashSet.Count, Factorial(graph.VertexCount));
        }

        [Test]
        public void IsHamiltonian_Throws()
        {
            IUndirectedGraph<int, UndirectedEdge<int>> nullGraph = null;

            // ReSharper disable ObjectCreationAsStatement
            // ReSharper disable AssignNullToNotNullAttribute
            Assert.Throws<ArgumentNullException>(() => nullGraph.CreateHamiltonianGraphAlgorithm());

            // ReSharper disable once ReturnValueOfPureMethodIsNotUsed
            Assert.Throws<ArgumentNullException>(() => nullGraph.IsHamiltonian());

            // ReSharper restore AssignNullToNotNullAttribute
            // ReSharper restore ObjectCreationAsStatement
        }
    }
}