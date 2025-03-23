using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using NUnit.Framework;
using QuikGraph.Predicates;

namespace QuikGraph.Tests.Predicates
{
    /// <summary>
    /// Tests for <see cref="SinkVertexPredicate{TVertex,TEdge}"/>.
    ///</summary>
    [TestFixture]
    internal sealed class SinkVertexPredicateTests
    {
        [Test]
        public void Construction()
        {
            Assert.DoesNotThrow(
                // ReSharper disable once ObjectCreationAsStatement
                () => new SinkVertexPredicate<int, IEdge<int>>(
                    new AdjacencyGraph<int, IEdge<int>>()));
        }

        [Test]
        public void Construction_Throws()
        {
            // ReSharper disable once ObjectCreationAsStatement
            // ReSharper disable once AssignNullToNotNullAttribute
            Assert.Throws<ArgumentNullException>(() => new SinkVertexPredicate<int, IEdge<int>>(null));
        }

        [NotNull, ItemNotNull]
        private static IEnumerable<TestCaseData> PredicateTestCases
        {
            [UsedImplicitly]
            get
            {
                yield return new TestCaseData(new AdjacencyGraph<int, IEdge<int>>());
                yield return new TestCaseData(new BidirectionalGraph<int, IEdge<int>>());
            }
        }

        [TestCaseSource(nameof(PredicateTestCases))]
        public void Predicate<TGraph>([NotNull] TGraph graph)
            where TGraph 
            : IIncidenceGraph<int, IEdge<int>>
            , IMutableVertexSet<int>
            , IMutableEdgeListGraph<int, IEdge<int>>
        {
            var predicate = new SinkVertexPredicate<int, IEdge<int>>(graph);

            graph.AddVertex(1);
            graph.AddVertex(2);
            Assert.IsTrue(predicate.TestIsOutEdgesEmpty(1));
            Assert.IsTrue(predicate.TestIsOutEdgesEmpty(2));

            graph.AddVertex(3);
            graph.AddEdge(Edge.Create(1, 3));
            Assert.IsFalse(predicate.TestIsOutEdgesEmpty(1));
            Assert.IsTrue(predicate.TestIsOutEdgesEmpty(2));
            Assert.IsTrue(predicate.TestIsOutEdgesEmpty(3));

            graph.AddEdge(Edge.Create(1, 2));
            Assert.IsFalse(predicate.TestIsOutEdgesEmpty(1));
            Assert.IsTrue(predicate.TestIsOutEdgesEmpty(2));
            Assert.IsTrue(predicate.TestIsOutEdgesEmpty(3));

            var edge23 = Edge.Create(2, 3);
            graph.AddEdge(edge23);
            Assert.IsFalse(predicate.TestIsOutEdgesEmpty(1));
            Assert.IsFalse(predicate.TestIsOutEdgesEmpty(2));
            Assert.IsTrue(predicate.TestIsOutEdgesEmpty(3));

            graph.RemoveEdge(edge23);
            Assert.IsFalse(predicate.TestIsOutEdgesEmpty(1));
            Assert.IsTrue(predicate.TestIsOutEdgesEmpty(2));
            Assert.IsTrue(predicate.TestIsOutEdgesEmpty(3));
        }

        [Test]
        public void Predicate_Throws()
        {
            var graph = new AdjacencyGraph<TestVertex, IEdge<TestVertex>>();
            var predicate = new SinkVertexPredicate<TestVertex, IEdge<TestVertex>>(graph);

            // ReSharper disable ReturnValueOfPureMethodIsNotUsed
            Assert.IsTrue(predicate.TestIsOutEdgesEmpty(new TestVertex("1")));
            // ReSharper disable once AssignNullToNotNullAttribute
            Assert.Throws<ArgumentNullException>(() => predicate.TestIsOutEdgesEmpty(null));
            // ReSharper restore ReturnValueOfPureMethodIsNotUsed
        }
    }
}