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
            CheckEdge(new SReversedEdge<int, Edge<int>>(new Edge<int>(1, 2)), 2, 1);
            CheckEdge(new SReversedEdge<int, Edge<int>>(new Edge<int>(2, 1)), 1, 2);
            CheckEdge(new SReversedEdge<int, Edge<int>>(new Edge<int>(1, 1)), 1, 1);

            // Struct break the contract with their implicit default constructor
            var defaultEdge = default(SReversedEdge<int, Edge<int>>);
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
            CheckEdge(new SReversedEdge<TestVertex, Edge<TestVertex>>(new Edge<TestVertex>(v1, v2)), v2, v1);
            CheckEdge(new SReversedEdge<TestVertex, Edge<TestVertex>>(new Edge<TestVertex>(v2, v1)), v1, v2);
            CheckEdge(new SReversedEdge<TestVertex, Edge<TestVertex>>(new Edge<TestVertex>(v1, v1)), v1, v1);

            // Struct break the contract with their implicit default constructor
            var defaultEdge2 = default(SReversedEdge<TestVertex, Edge<TestVertex>>);
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
            Assert.Throws<ArgumentNullException>(() => new SReversedEdge<TestVertex, Edge<TestVertex>>(null));
        }

        [Test]
        public void Equals()
        {
            var wrappedEdge = new Edge<int>(1, 2);
            var edge1 = new SReversedEdge<int, Edge<int>>(wrappedEdge);
            var edge2 = new SReversedEdge<int, Edge<int>>(wrappedEdge);
            var edge3 = new SReversedEdge<int, Edge<int>>(new Edge<int>(1, 2));
            var edge4 = new SReversedEdge<int, Edge<int>>(new Edge<int>(2, 1));

            Assert.AreEqual(edge1, edge1);

            Assert.AreEqual(edge1, edge2);
            Assert.AreEqual(edge2, edge1);
            Assert.IsTrue(edge1.Equals((object)edge2));
            Assert.IsTrue(edge1.Equals(edge2));
            Assert.IsTrue(edge2.Equals(edge1));
            Assert.IsTrue(edge1 == edge2);
            Assert.IsTrue(edge2 == edge1);
            Assert.IsFalse(edge1 != edge2);
            Assert.IsFalse(edge2 != edge1);

            Assert.AreNotEqual(edge1, edge3);
            Assert.AreNotEqual(edge3, edge1);
            Assert.IsFalse(edge1.Equals((object)edge3));
            Assert.IsFalse(edge1.Equals(edge3));
            Assert.IsFalse(edge3.Equals(edge1));
            Assert.IsFalse(edge1 == edge3);
            Assert.IsFalse(edge3 == edge1);
            Assert.IsTrue(edge1 != edge3);
            Assert.IsTrue(edge3 != edge1);

            Assert.AreNotEqual(edge1, edge4);
            Assert.AreNotEqual(edge4, edge1);
            Assert.IsFalse(edge1.Equals((object)edge4));
            Assert.IsFalse(edge1.Equals(edge4));
            Assert.IsFalse(edge4.Equals(edge1));
            Assert.IsFalse(edge1 == edge4);
            Assert.IsFalse(edge4 == edge1);
            Assert.IsTrue(edge1 != edge4);
            Assert.IsTrue(edge4 != edge1);

            Assert.AreNotEqual(null, edge1);
            Assert.IsFalse(edge1.Equals(null));
        }

        [Test]
        public void EqualsDefaultEdge_ReferenceTypeExtremities()
        {
            var edge1 = default(SReversedEdge<int, Edge<int>>);
            var edge2 = new SReversedEdge<int, Edge<int>>();

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
            var wrappedEdge = new Edge<int>(1, 2);
            var edge1 = new SReversedEdge<int, Edge<int>>(wrappedEdge);
            var edge2 = new SReversedEdge<int, Edge<int>>(wrappedEdge);
            var edge3 = new SReversedEdge<int, Edge<int>>(new Edge<int>(1, 2));
            var edge4 = new SReversedEdge<int, Edge<int>>(new Edge<int>(2, 1));

            Assert.AreEqual(edge1.GetHashCode(), edge2.GetHashCode());
            Assert.AreNotEqual(edge1.GetHashCode(), edge3.GetHashCode());
            Assert.AreNotEqual(edge1.GetHashCode(), edge4.GetHashCode());
        }

        [Test]
        public void HashcodeDefaultEdge_ReferenceTypeExtremities()
        {
            var edge1 = default(SReversedEdge<int, Edge<int>>);
            var edge2 = new SReversedEdge<int, Edge<int>>();

            Assert.AreEqual(edge1.GetHashCode(), edge2.GetHashCode());
        }

        [Test]
        public void ObjectToString()
        {
            var edge1 = new SReversedEdge<int, Edge<int>>(new Edge<int>(1, 2));
            var edge2 = new SReversedEdge<int, Edge<int>>(new Edge<int>(2, 1));
            var edge3 = new SReversedEdge<int, UndirectedEdge<int>>(new UndirectedEdge<int>(1, 2));

            Assert.AreEqual("R(1 -> 2)", edge1.ToString());
            Assert.AreEqual("R(2 -> 1)", edge2.ToString());
            Assert.AreEqual("R(1 <-> 2)", edge3.ToString());
        }
    }
}