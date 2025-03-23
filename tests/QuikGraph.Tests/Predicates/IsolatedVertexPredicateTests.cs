using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using NUnit.Framework;
using QuikGraph.Predicates;

namespace QuikGraph.Tests.Predicates
{
    /// <summary>
    /// Tests for <see cref="IsolatedVertexPredicate{TVertex,TEdge}"/>.
    ///</summary>
    [TestFixture]
    internal sealed class IsolatedVertexPredicateTests
    {
        [Test]
        public void Construction()
        {
            Assert.DoesNotThrow(
                // ReSharper disable once ObjectCreationAsStatement
                () => new IsolatedVertexPredicate<int, IEdge<int>>(
                    new BidirectionalGraph<int, IEdge<int>>()));
        }

        [Test]
        public void Construction_Throws()
        {
            // ReSharper disable once ObjectCreationAsStatement
            // ReSharper disable once AssignNullToNotNullAttribute
            Assert.Throws<ArgumentNullException>(() => new IsolatedVertexPredicate<int, IEdge<int>>(null));
        }

        [NotNull, ItemNotNull]
        private static IEnumerable<TestCaseData> PredicateTestCases
        {
            [UsedImplicitly]
            get
            {
                yield return new TestCaseData(new BidirectionalGraph<int, IEdge<int>>());
            }
        }

        [TestCaseSource(nameof(PredicateTestCases))]
        public void Predicate<TGraph>([NotNull] TGraph graph)
            where TGraph 
            : IBidirectionalGraph<int, IEdge<int>>
            , IMutableVertexSet<int>
            , IMutableEdgeListGraph<int, IEdge<int>>
        {
            var predicate = new IsolatedVertexPredicate<int, IEdge<int>>(graph);

            graph.AddVertex(1);
            graph.AddVertex(2);
            Assert.IsTrue(predicate.TestIsEdgesEmpty(1));
            Assert.IsTrue(predicate.TestIsEdgesEmpty(2));

            graph.AddVertex(3);
            var edge13 = Edge.Create(1, 3);
            graph.AddEdge(edge13);
            Assert.IsFalse(predicate.TestIsEdgesEmpty(1));
            Assert.IsTrue(predicate.TestIsEdgesEmpty(2));
            Assert.IsFalse(predicate.TestIsEdgesEmpty(3));

            var edge12 = Edge.Create(1, 2);
            graph.AddEdge(edge12);
            Assert.IsFalse(predicate.TestIsEdgesEmpty(1));
            Assert.IsFalse(predicate.TestIsEdgesEmpty(2));
            Assert.IsFalse(predicate.TestIsEdgesEmpty(3));

            var edge23 = Edge.Create(2, 3);
            graph.AddEdge(edge23);
            Assert.IsFalse(predicate.TestIsEdgesEmpty(1));
            Assert.IsFalse(predicate.TestIsEdgesEmpty(2));
            Assert.IsFalse(predicate.TestIsEdgesEmpty(3));

            graph.RemoveEdge(edge23);
            Assert.IsFalse(predicate.TestIsEdgesEmpty(1));
            Assert.IsFalse(predicate.TestIsEdgesEmpty(2));
            Assert.IsFalse(predicate.TestIsEdgesEmpty(3));

            graph.RemoveEdge(edge12);
            Assert.IsFalse(predicate.TestIsEdgesEmpty(1));
            Assert.IsTrue(predicate.TestIsEdgesEmpty(2));
            Assert.IsFalse(predicate.TestIsEdgesEmpty(3));

            graph.RemoveEdge(edge13);
            Assert.IsTrue(predicate.TestIsEdgesEmpty(1));
            Assert.IsTrue(predicate.TestIsEdgesEmpty(2));
            Assert.IsTrue(predicate.TestIsEdgesEmpty(3));
        }

        [Test]
        public void Predicate_Throws()
        {
            var graph = new BidirectionalGraph<TestVertex, IEdge<TestVertex>>();
            var predicate = new IsolatedVertexPredicate<TestVertex, IEdge<TestVertex>>(graph);

            // ReSharper disable ReturnValueOfPureMethodIsNotUsed
            Assert.IsTrue(predicate.TestIsEdgesEmpty(new TestVertex("1")));
            // ReSharper disable once AssignNullToNotNullAttribute
            Assert.Throws<ArgumentNullException>(() => predicate.TestIsEdgesEmpty(null));
            // ReSharper restore ReturnValueOfPureMethodIsNotUsed
        }
    }
}