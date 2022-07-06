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
            CheckEdge(new Edge<int>(1, 2), 1, 2);
            CheckEdge(new Edge<int>(2, 1), 2, 1);
            CheckEdge(new Edge<int>(1, 1), 1, 1);

            // Reference type
            var v1 = new TestVertex("v1");
            var v2 = new TestVertex("v2");
            CheckEdge(new Edge<TestVertex>(v1, v2), v1, v2);
            CheckEdge(new Edge<TestVertex>(v2, v1), v2, v1);
            CheckEdge(new Edge<TestVertex>(v1, v1), v1, v1);
        }

        [Test]
        public void Construction_Throws()
        {
            // ReSharper disable ObjectCreationAsStatement
            // ReSharper disable AssignNullToNotNullAttribute
            Assert.Throws<ArgumentNullException>(() => new Edge<TestVertex>(null, new TestVertex("v1")));
            Assert.Throws<ArgumentNullException>(() => new Edge<TestVertex>(new TestVertex("v1"), null));
            Assert.Throws<ArgumentNullException>(() => new Edge<TestVertex>(null, null));
            // ReSharper restore AssignNullToNotNullAttribute
            // ReSharper restore ObjectCreationAsStatement
        }

        [Test]
        public void Equals()
        {
            var edge1 = new Edge<int>(1, 2);
            var edge2 = new Edge<int>(1, 2);
            var edge3 = new Edge<int>(2, 1);

            Assert.AreEqual(edge1, edge1);

            Assert.AreNotEqual(edge1, edge2);
            Assert.AreNotEqual(edge2, edge1);
            Assert.IsFalse(edge1.Equals(edge2));
            Assert.IsFalse(edge2.Equals(edge1));
            Assert.IsFalse(edge1 == edge2);
            Assert.IsFalse(edge2 == edge1);
            Assert.IsTrue(edge1 != edge2);
            Assert.IsTrue(edge2 != edge1);

            Assert.AreNotEqual(edge1, edge3);
            Assert.AreNotEqual(edge3, edge1);
            Assert.IsFalse(edge1.Equals(edge3));
            Assert.IsFalse(edge3.Equals(edge1));
            Assert.IsFalse(edge1 == edge3);
            Assert.IsFalse(edge3 == edge1);
            Assert.IsTrue(edge1 != edge3);
            Assert.IsTrue(edge3 != edge1);

            Assert.AreNotEqual(null, edge1);
            Assert.IsFalse(edge1.Equals(null));
            Assert.IsFalse(edge1 == null);
            Assert.IsFalse(null == edge1);
            Assert.IsTrue(edge1 != null);
            Assert.IsTrue(null != edge1);
        }

        [Test]
        public void ObjectToString()
        {
            var edge1 = new Edge<int>(1, 2);
            var edge2 = new Edge<int>(2, 1);

            Assert.AreEqual("1 -> 2", edge1.ToString());
            Assert.AreEqual("2 -> 1", edge2.ToString());
        }
    }
}