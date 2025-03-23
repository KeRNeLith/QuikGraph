using System;
using JetBrains.Annotations;
using NUnit.Framework;
using QuikGraph.Algorithms.Cliques;
using QuikGraph.Algorithms.Services;


namespace QuikGraph.Tests.Algorithms.Cliques
{
    /// <summary>
    /// Tests for <see cref="MaximumCliqueAlgorithmBase{TVertex,TEdge}"/>.
    /// </summary>
    [TestFixture]
    internal sealed class MaximumCliqueAlgorithmTests
    {
        private class TestMaximumCliqueAlgorithm<TVertex, TEdge> : MaximumCliqueAlgorithmBase<TVertex, TEdge>
            where TEdge : IEdge<TVertex>
        {
            public TestMaximumCliqueAlgorithm(
                [NotNull] IUndirectedGraph<TVertex, TEdge> visitedGraph,
                [CanBeNull] IAlgorithmComponent host = null)
                : base(visitedGraph, host)
            {
            }

            /// <inheritdoc />
            protected override void InternalCompute()
            {
                throw new NotImplementedException();
            }
        }

        [Test]
        public void Constructor()
        {
            var graph = new UndirectedGraph<int, IEdge<int>>();
            var algorithm = new TestMaximumCliqueAlgorithm<int, IEdge<int>>(graph);
            algorithm.AssertAlgorithmState(graph);

            algorithm = new TestMaximumCliqueAlgorithm<int, IEdge<int>>(graph, null);
            algorithm.AssertAlgorithmState(graph);
        }

        [Test]
        public void Constructor_Throws()
        {
            // ReSharper disable ObjectCreationAsStatement
            // ReSharper disable AssignNullToNotNullAttribute
            Assert.Throws<ArgumentNullException>(
                () => new TestMaximumCliqueAlgorithm<int, IEdge<int>>(null));

            Assert.Throws<ArgumentNullException>(
                () => new TestMaximumCliqueAlgorithm<int, IEdge<int>>(null, null));
            // ReSharper restore AssignNullToNotNullAttribute
            // ReSharper restore ObjectCreationAsStatement
        }
    }
}