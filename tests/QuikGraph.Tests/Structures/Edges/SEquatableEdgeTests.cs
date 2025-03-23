using System;
using NUnit.Framework;

namespace QuikGraph.Tests.Structures
{
    /// <summary>
    /// Tests for <see cref="SEquatableEdge{TVertex}"/>.
    ///</summary>
    [TestFixture]
    internal sealed class SEquatableEdgeTests : EdgeTestsBase
    {
        [Test]
        public void Construction()
        {
            // Value type
            CheckEdge(new SEquatableEdge<int>(1, 2), 1, 2);
            CheckEdge(new SEquatableEdge<int>(2, 1), 2, 1);
            CheckEdge(new SEquatableEdge<int>(1, 1), 1, 1);

            // Reference type
            var v1 = new TestVertex("v1");
            var v2 = new TestVertex("v2");
            CheckEdge(new SEquatableEdge<TestVertex>(v1, v2), v1, v2);
            CheckEdge(new SEquatableEdge<TestVertex>(v2, v1), v2, v1);
            CheckEdge(new SEquatableEdge<TestVertex>(v1, v1), v1, v1);
        }

        [Test]
        public void Construction_Throws()
        {
            // ReSharper disable ObjectCreationAsStatement
            // ReSharper disable AssignNullToNotNullAttribute
            Assert.Throws<ArgumentNullException>(() => new SEquatableEdge<TestVertex>(null, new TestVertex("v1")));
            Assert.Throws<ArgumentNullException>(() => new SEquatableEdge<TestVertex>(new TestVertex("v1"), null));
            Assert.Throws<ArgumentNullException>(() => new SEquatableEdge<TestVertex>(null, null));
            // ReSharper restore AssignNullToNotNullAttribute
            // ReSharper restore ObjectCreationAsStatement
        }

        [Test]
        public void Equals()
        {
            var edge1 = new SEquatableEdge<int>(1, 2);
            var edge2 = new SEquatableEdge<int>(1, 2);
            var edge3 = new SEquatableEdge<int>(2, 1);

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

            Assert.AreNotEqual(null, edge1);
            Assert.IsFalse(edge1.Equals(null));
        }

        [Test]
        public void Hashcode()
        {
            var edge1 = new SEquatableEdge<int>(1, 2);
            var edge2 = new SEquatableEdge<int>(1, 2);
            var edge3 = new SEquatableEdge<int>(2, 1);

            Assert.AreEqual(edge1.GetHashCode(), edge2.GetHashCode());
            Assert.AreNotEqual(edge1.GetHashCode(), edge3.GetHashCode());
        }

        [Test]
        public void ObjectToString()
        {
            var edge1 = new SEquatableEdge<int>(1, 2);
            var edge2 = new SEquatableEdge<int>(2, 1);

            Assert.AreEqual("1 -> 2", edge1.ToString());
            Assert.AreEqual("2 -> 1", edge2.ToString());
        }
    }
}