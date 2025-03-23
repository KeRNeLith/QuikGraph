using System;
using NUnit.Framework;

namespace QuikGraph.Tests.Structures
{
    /// <summary>
    /// Tests for <see cref="Edge{TVertex}"/>.
    ///</summary>
    [TestFixture]
    internal sealed class EdgeTests : EdgeTestsBase
    {
        [Test]
        public void Construction()
        {
            // Value type
            CheckEdge(Edge.Create(1, 2), 1, 2);
            CheckEdge(Edge.Create(2, 1), 2, 1);
            CheckEdge(Edge.Create(1, 1), 1, 1);

            // Reference type
            var v1 = new TestVertex("v1");
            var v2 = new TestVertex("v2");
            CheckEdge(Edge.Create(v1, v2), v1, v2);
            CheckEdge(Edge.Create(v2, v1), v2, v1);
            CheckEdge(Edge.Create(v1, v1), v1, v1);
        }

        [Test]
        public void Construction_Throws()
        {
            // ReSharper disable ObjectCreationAsStatement
            // ReSharper disable AssignNullToNotNullAttribute
            Assert.Throws<ArgumentNullException>(() => Edge.Create(null, new TestVertex("v1")));
            Assert.Throws<ArgumentNullException>(() => Edge.Create(new TestVertex("v1"), null));
            Assert.Throws<ArgumentNullException>(() => Edge.Create(null, (TestVertex)null));
            // ReSharper restore AssignNullToNotNullAttribute
            // ReSharper restore ObjectCreationAsStatement
        }

        [Test]
        public void Equals()
        {
            var edge1 = Edge.Create(1, 2);
            var edge2 = Edge.Create(1, 2);
            var edge3 = Edge.Create(2, 1);

            Assert.AreEqual(edge1, edge1);

            Assert.AreNotEqual(edge1, edge2);
            Assert.AreNotEqual(edge2, edge1);
            Assert.IsFalse(edge1.Equals(edge2));
            Assert.IsFalse(edge2.Equals(edge1));

            Assert.AreNotEqual(edge1, edge3);
            Assert.AreNotEqual(edge3, edge1);
            Assert.IsFalse(edge1.Equals(edge2));
            Assert.IsFalse(edge2.Equals(edge1));

            Assert.AreNotEqual(null, edge1);
            Assert.IsFalse(edge1.Equals(null));
        }

        [Test]
        public void ObjectToString()
        {
            var edge1 = Edge.Create(1, 2);
            var edge2 = Edge.Create(2, 1);

            Assert.AreEqual("1 -> 2", edge1.ToString());
            Assert.AreEqual("2 -> 1", edge2.ToString());
        }
    }
}