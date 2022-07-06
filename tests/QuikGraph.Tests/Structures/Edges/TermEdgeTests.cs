using System;
using NUnit.Framework;

namespace QuikGraph.Tests.Structures
{
    /// <summary>
    /// Tests for <see cref="TermEdge{TVertex}"/>.
    ///</summary>
    [TestFixture]
    internal sealed class TermEdgeTests : EdgeTestsBase
    {
        [Test]
        public void Construction()
        {
            // Value type
            CheckEdge(new TermEdge<int>(1, 2), 1, 2);
            CheckEdge(new TermEdge<int>(2, 1), 2, 1);
            CheckEdge(new TermEdge<int>(1, 1), 1, 1);

            CheckTermEdge(new TermEdge<int>(1, 2, 0, 1), 1, 2, 0, 1);
            CheckTermEdge(new TermEdge<int>(2, 1, 1, 0), 2, 1, 1, 0);
            CheckTermEdge(new TermEdge<int>(1, 1, 0, 0), 1, 1, 0, 0);

            // Reference type
            var v1 = new TestVertex("v1");
            var v2 = new TestVertex("v2");
            CheckEdge(new TermEdge<TestVertex>(v1, v2), v1, v2);
            CheckEdge(new TermEdge<TestVertex>(v2, v1), v2, v1);
            CheckEdge(new TermEdge<TestVertex>(v1, v1), v1, v1);

            CheckTermEdge(new TermEdge<TestVertex>(v1, v2, 0, 1), v1, v2, 0, 1);
            CheckTermEdge(new TermEdge<TestVertex>(v2, v1, 1, 0), v2, v1, 1, 0);
            CheckTermEdge(new TermEdge<TestVertex>(v1, v1, 0, 0), v1, v1, 0, 0);
        }

        [Test]
        public void Construction_Throws()
        {
            var v1 = new TestVertex("v1");
            var v2 = new TestVertex("v2");
            // ReSharper disable ObjectCreationAsStatement
            // ReSharper disable AssignNullToNotNullAttribute
            Assert.Throws<ArgumentNullException>(() => new TermEdge<TestVertex>(null, v1));
            Assert.Throws<ArgumentNullException>(() => new TermEdge<TestVertex>(v1, null));
            Assert.Throws<ArgumentNullException>(() => new TermEdge<TestVertex>(null, null));

            Assert.Throws<ArgumentNullException>(() => new TermEdge<TestVertex>(null, v1, 0, 1));
            Assert.Throws<ArgumentNullException>(() => new TermEdge<TestVertex>(v1, null, 0, 1));
            Assert.Throws<ArgumentNullException>(() => new TermEdge<TestVertex>(null, null, 0, 1));
            // ReSharper restore AssignNullToNotNullAttribute

            Assert.Throws<ArgumentException>(() => new TermEdge<TestVertex>(v1, v2, -1, 0));
            Assert.Throws<ArgumentException>(() => new TermEdge<TestVertex>(v1, v2, 0, -1));
            Assert.Throws<ArgumentException>(() => new TermEdge<TestVertex>(v1, v2, -1, -1));
            // ReSharper restore ObjectCreationAsStatement
        }

        [Test]
        public void Equals()
        {
            var edge1 = new TermEdge<int>(1, 2);
            var edge2 = new TermEdge<int>(1, 2);
            var edge3 = new TermEdge<int>(1, 2, 0, 0);
            var edge4 = new TermEdge<int>(1, 2, 0, 0);
            var edge5 = new TermEdge<int>(1, 2, 0, 1);
            var edge6 = new TermEdge<int>(1, 2, 0, 1);

            Assert.AreEqual(edge1, edge1);
            Assert.AreEqual(edge3, edge3);
            Assert.AreEqual(edge5, edge5);

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

            Assert.AreNotEqual(edge1, edge5);
            Assert.AreNotEqual(edge5, edge1);
            Assert.IsFalse(edge1.Equals(edge5));
            Assert.IsFalse(edge5.Equals(edge1));
            Assert.IsFalse(edge1 == edge5);
            Assert.IsFalse(edge5 == edge1);
            Assert.IsTrue(edge1 != edge5);
            Assert.IsTrue(edge5 != edge1);

            Assert.AreNotEqual(edge3, edge4);
            Assert.AreNotEqual(edge4, edge3);
            Assert.IsFalse(edge3.Equals(edge4));
            Assert.IsFalse(edge4.Equals(edge3));
            Assert.IsFalse(edge3 == edge4);
            Assert.IsFalse(edge4 == edge3);
            Assert.IsTrue(edge3 != edge4);
            Assert.IsTrue(edge4 != edge3);

            Assert.AreNotEqual(edge5, edge6);
            Assert.AreNotEqual(edge6, edge5);
            Assert.IsFalse(edge5.Equals(edge6));
            Assert.IsFalse(edge6.Equals(edge5));
            Assert.IsFalse(edge5 == edge6);
            Assert.IsFalse(edge6 == edge5);
            Assert.IsTrue(edge5 != edge6);
            Assert.IsTrue(edge6 != edge5);

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
            var edge1 = new TermEdge<int>(1, 2);
            var edge2 = new TermEdge<int>(1, 2, 1, 5);
            var edge3 = new TermEdge<int>(2, 1);

            Assert.AreEqual("1 (0) -> 2 (0)", edge1.ToString());
            Assert.AreEqual("1 (1) -> 2 (5)", edge2.ToString());
            Assert.AreEqual("2 (0) -> 1 (0)", edge3.ToString());
        }
    }
}