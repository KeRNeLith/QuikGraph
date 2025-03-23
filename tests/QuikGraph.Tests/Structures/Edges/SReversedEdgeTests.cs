using System;
using NUnit.Framework;

namespace QuikGraph.Tests.Structures
{
    /// <summary>
    /// Tests for <see cref="SReversedEdge{TVertex,TEdge}"/>.
    ///</summary>
    [TestFixture]
    internal sealed class SReversedEdgeTests : EdgeTestsBase
    {
        [Test]
        public void Construction()
        {
            // Value type
            CheckEdge(new SReversedEdge<int, IEdge<int>>(Edge.Create(1, 2)), 2, 1);
            CheckEdge(new SReversedEdge<int, IEdge<int>>(Edge.Create(2, 1)), 1, 2);
            CheckEdge(new SReversedEdge<int, IEdge<int>>(Edge.Create(1, 1)), 1, 1);

            // Struct break the contract with their implicit default constructor
            var defaultEdge = default(SReversedEdge<int, IEdge<int>>);
            // ReSharper disable HeuristicUnreachableCode
            // Justification: Since struct has implicit default constructor it allows initialization of invalid edge
            Assert.IsNull(defaultEdge.OriginalEdge);
            // ReSharper disable  HeuristicUnreachableCode
            Assert.Throws<NullReferenceException>(() => { int _ = defaultEdge.Source; });
            Assert.Throws<NullReferenceException>(() => { int _ = defaultEdge.Target; });
            // ReSharper restore HeuristicUnreachableCode

            // Reference type
            var v1 = new TestVertex("v1");
            var v2 = new TestVertex("v2");
            CheckEdge(new SReversedEdge<TestVertex, IEdge<TestVertex>>(Edge.Create(v1, v2)), v2, v1);
            CheckEdge(new SReversedEdge<TestVertex, IEdge<TestVertex>>(Edge.Create(v2, v1)), v1, v2);
            CheckEdge(new SReversedEdge<TestVertex, IEdge<TestVertex>>(Edge.Create(v1, v1)), v1, v1);

            // Struct break the contract with their implicit default constructor
            var defaultEdge2 = default(SReversedEdge<TestVertex, IEdge<TestVertex>>);
            // ReSharper disable HeuristicUnreachableCode
            // Justification: Since struct has implicit default constructor it allows initialization of invalid edge
            Assert.IsNull(defaultEdge2.OriginalEdge);
            // ReSharper disable  HeuristicUnreachableCode
            Assert.Throws<NullReferenceException>(() => { TestVertex _ = defaultEdge2.Source; });
            Assert.Throws<NullReferenceException>(() => { TestVertex _ = defaultEdge2.Target; });
            // ReSharper restore HeuristicUnreachableCode
        }

        [Test]
        public void Construction_Throws()
        {
            // ReSharper disable once ObjectCreationAsStatement
            // ReSharper disable once AssignNullToNotNullAttribute
            Assert.Throws<ArgumentNullException>(() => new SReversedEdge<TestVertex, IEdge<TestVertex>>(null));
        }

        [Test]
        public void Equals()
        {
            var wrappedEdge = Edge.Create(1, 2);
            var edge1 = new SReversedEdge<int, IEdge<int>>(wrappedEdge);
            var edge2 = new SReversedEdge<int, IEdge<int>>(wrappedEdge);
            var edge3 = new SReversedEdge<int, IEdge<int>>(Edge.Create(1, 2));
            var edge4 = new SReversedEdge<int, IEdge<int>>(Edge.Create(2, 1));

            Assert.AreEqual(edge1, edge1);

            Assert.AreEqual(edge1, edge2);
            Assert.AreEqual(edge2, edge1);
            Assert.IsTrue(edge1.Equals((object)edge2));
            Assert.IsTrue(edge1.Equals(edge2));
            Assert.IsTrue(edge2.Equals(edge1));

            Assert.AreNotEqual(edge1, edge3);
            Assert.AreNotEqual(edge3, edge1);
            Assert.IsFalse(edge1.Equals((object)edge3));
            Assert.IsFalse(edge1.Equals(edge3));
            Assert.IsFalse(edge3.Equals(edge1));

            Assert.AreNotEqual(edge1, edge4);
            Assert.AreNotEqual(edge4, edge1);
            Assert.IsFalse(edge1.Equals((object)edge4));
            Assert.IsFalse(edge1.Equals(edge4));
            Assert.IsFalse(edge4.Equals(edge1));

            Assert.AreNotEqual(null, edge1);
            Assert.IsFalse(edge1.Equals(null));
        }

        [Test]
        public void EqualsDefaultEdge_ReferenceTypeExtremities()
        {
            var edge1 = default(SReversedEdge<int, IEdge<int>>);
            var edge2 = new SReversedEdge<int, IEdge<int>>();

            Assert.AreEqual(edge1, edge2);
            Assert.AreEqual(edge2, edge1);
            Assert.IsTrue(edge1.Equals(edge2));
            Assert.IsTrue(edge2.Equals(edge1));
        }

        [Test]
        public void Equals2()
        {
            var edge1 = new SReversedEdge<int, EquatableEdge<int>>(new EquatableEdge<int>(1, 2));
            var edge2 = new SReversedEdge<int, EquatableEdge<int>>(new EquatableEdge<int>(1, 2));
            var edge3 = new SReversedEdge<int, EquatableEdge<int>>(new EquatableEdge<int>(2, 1));

            Assert.AreEqual(edge1, edge1);
            Assert.AreEqual(edge1, edge2);
            Assert.IsTrue(edge1.Equals((object)edge2));
            Assert.AreNotEqual(edge1, edge3);

            Assert.AreNotEqual(null, edge1);
            Assert.IsFalse(edge1.Equals(null));
        }

        [Test]
        public void Hashcode()
        {
            var wrappedEdge = Edge.Create(1, 2);
            var edge1 = new SReversedEdge<int, IEdge<int>>(wrappedEdge);
            var edge2 = new SReversedEdge<int, IEdge<int>>(wrappedEdge);
            var edge3 = new SReversedEdge<int, IEdge<int>>(Edge.Create(1, 2));
            var edge4 = new SReversedEdge<int, IEdge<int>>(Edge.Create(2, 1));

            Assert.AreEqual(edge1.GetHashCode(), edge2.GetHashCode());
            Assert.AreNotEqual(edge1.GetHashCode(), edge3.GetHashCode());
            Assert.AreNotEqual(edge1.GetHashCode(), edge4.GetHashCode());
        }

        [Test]
        public void HashcodeDefaultEdge_ReferenceTypeExtremities()
        {
            var edge1 = default(SReversedEdge<int, IEdge<int>>);
            var edge2 = new SReversedEdge<int, IEdge<int>>();

            Assert.AreEqual(edge1.GetHashCode(), edge2.GetHashCode());
        }

        [Test]
        public void ObjectToString()
        {
            var edge1 = new SReversedEdge<int, IEdge<int>>(Edge.Create(1, 2));
            var edge2 = new SReversedEdge<int, IEdge<int>>(Edge.Create(2, 1));
            var edge3 = new SReversedEdge<int, UndirectedEdge<int>>(new UndirectedEdge<int>(1, 2));

            Assert.AreEqual("R(1 -> 2)", edge1.ToString());
            Assert.AreEqual("R(2 -> 1)", edge2.ToString());
            Assert.AreEqual("R(1 <-> 2)", edge3.ToString());
        }
    }
}