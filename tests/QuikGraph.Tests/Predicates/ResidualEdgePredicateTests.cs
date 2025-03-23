using System;
using System.Collections.Generic;
using NUnit.Framework;
using QuikGraph.Predicates;

namespace QuikGraph.Tests.Predicates
{
    /// <summary>
    /// Tests for <see cref="ResidualEdgePredicate{TEdge}"/>.
    ///</summary>
    [TestFixture]
    internal sealed class ResidualEdgePredicateTests
    {
        [Test]
        public void Construction()
        {
            Assert.DoesNotThrow(
                // ReSharper disable once ObjectCreationAsStatement
                () => new ResidualEdgePredicate<Edge<int>>(
                    new Dictionary<Edge<int>, double>()));
        }

        [Test]
        public void Construction_Throws()
        {
            // ReSharper disable once ObjectCreationAsStatement
            // ReSharper disable once AssignNullToNotNullAttribute
            Assert.Throws<ArgumentNullException>(() => new ResidualEdgePredicate<Edge<int>>(null));
        }

        [Test]
        public void Predicate()
        {
            var predicate = new ResidualEdgePredicate<IEdge<int>>(
                new Dictionary<IEdge<int>, double>());

            var edge12 = Edge.Create(1, 2);
            var edge13 = Edge.Create(1, 3);
            var edge31 = Edge.Create(3, 1);
            predicate.ResidualCapacities.Add(edge12, -12);
            predicate.ResidualCapacities.Add(edge13, 0);
            predicate.ResidualCapacities.Add(edge31, 1);

            Assert.IsFalse(predicate.Test(edge12));
            Assert.IsFalse(predicate.Test(edge13));
            Assert.IsTrue(predicate.Test(edge31));
        }

        [Test]
        public void Predicate_Throws()
        {
            var predicate = new ResidualEdgePredicate<IEdge<int>>(
                new Dictionary<IEdge<int>, double>());

            // ReSharper disable ReturnValueOfPureMethodIsNotUsed
            // ReSharper disable once AssignNullToNotNullAttribute
            Assert.Throws<ArgumentNullException>(() => predicate.Test(null));

            var edge12 = Edge.Create(1, 2);
            Assert.Throws<KeyNotFoundException>(() => predicate.Test(edge12));
            // ReSharper restore ReturnValueOfPureMethodIsNotUsed
        }
    }
}