using System;
using System.Collections.Generic;
using NUnit.Framework;
using QuikGraph.Predicates;

namespace QuikGraph.Tests.Predicates
{
    /// <summary>
    /// Tests for <see cref="ReversedResidualEdgePredicate{TEdge}"/>.
    ///</summary>
    [TestFixture]
    internal sealed class ReversedResidualEdgePredicateTests
    {
        [Test]
        public void Construction()
        {
            Assert.DoesNotThrow(
                // ReSharper disable once ObjectCreationAsStatement
                () => new ReversedResidualEdgePredicate<Edge<int>>(
                    new Dictionary<Edge<int>, double>(),
                    new Dictionary<Edge<int>, Edge<int>>()));
        }

        [Test]
        public void Construction_Throws()
        {
            // ReSharper disable ObjectCreationAsStatement
            // ReSharper disable AssignNullToNotNullAttribute
            Assert.Throws<ArgumentNullException>(() => new ReversedResidualEdgePredicate<Edge<int>>(null, new Dictionary<Edge<int>, Edge<int>>()));
            Assert.Throws<ArgumentNullException>(() => new ReversedResidualEdgePredicate<Edge<int>>(new Dictionary<Edge<int>, double>(), null));
            Assert.Throws<ArgumentNullException>(() => new ReversedResidualEdgePredicate<Edge<int>>(null, null));
            // ReSharper restore AssignNullToNotNullAttribute
            // ReSharper restore ObjectCreationAsStatement
        }

        [Test]
        public void Predicate()
        {
            var predicate = new ReversedResidualEdgePredicate<IEdge<int>>(
                new Dictionary<IEdge<int>, double>(),
                new Dictionary<IEdge<int>, IEdge<int>>());

            var edge12 = Edge.Create(1, 2);
            var edge21 = Edge.Create(2, 1);
            var edge13 = Edge.Create(1, 3);
            var edge31 = Edge.Create(3, 1);
            predicate.ReversedEdges.Add(edge12, edge21);
            predicate.ReversedEdges.Add(edge21, edge12);
            predicate.ReversedEdges.Add(edge13, edge31);
            predicate.ReversedEdges.Add(edge31, edge13);
            predicate.ResidualCapacities.Add(edge12, -12);
            predicate.ResidualCapacities.Add(edge21, 12);
            predicate.ResidualCapacities.Add(edge13, 0);
            predicate.ResidualCapacities.Add(edge31, 1);

            Assert.IsTrue(predicate.Test(edge12));
            Assert.IsFalse(predicate.Test(edge21));
            Assert.IsTrue(predicate.Test(edge13));
            Assert.IsFalse(predicate.Test(edge31));
        }

        [Test]
        public void Predicate_Throws()
        {
            var predicate = new ReversedResidualEdgePredicate<IEdge<int>>(
                new Dictionary<IEdge<int>, double>(),
                new Dictionary<IEdge<int>, IEdge<int>>());

            // ReSharper disable ReturnValueOfPureMethodIsNotUsed
            // ReSharper disable once AssignNullToNotNullAttribute
            Assert.Throws<ArgumentNullException>(() => predicate.Test(null));

            var edge12 = Edge.Create(1, 2);
            Assert.Throws<KeyNotFoundException>(() => predicate.Test(edge12));

            predicate.ReversedEdges.Add(edge12, Edge.Create(2, 1));
            Assert.Throws<KeyNotFoundException>(() => predicate.Test(edge12));
            // ReSharper restore ReturnValueOfPureMethodIsNotUsed
        }
    }
}