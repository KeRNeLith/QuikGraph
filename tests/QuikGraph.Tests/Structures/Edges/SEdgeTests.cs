using System;
using NUnit.Framework;

namespace QuikGraph.Tests.Structures
{
    /// <summary>
    /// Tests for <see cref="SEdge{TVertex}"/>.
    ///</summary>
    [TestFixture]
    internal sealed class SEdgeTests : EdgeTestsBase
    {
        [Test]
        public void Construction()
        {
            // Value type
            CheckEdge(new SEdge<int>(1, 2), 1, 2);
            CheckEdge(new SEdge<int>(2, 1), 2, 1);
            CheckEdge(new SEdge<int>(1, 1), 1, 1);
            //CheckEdge(default(SEdge<int>), 0, 0);

            // Reference type
            var v1 = new TestVertex("v1");
            var v2 = new TestVertex("v2");
            CheckEdge(new SEdge<TestVertex>(v1, v2), v1, v2);
            CheckEdge(new SEdge<TestVertex>(v2, v1), v2, v1);
            CheckEdge(new SEdge<TestVertex>(v1, v1), v1, v1);
        }

        [Test]
        public void Construction_Throws()
        {
            // ReSharper disable ObjectCreationAsStatement
            // ReSharper disable AssignNullToNotNullAttribute
            Assert.Throws<ArgumentNullException>(() => new SEdge<TestVertex>(null, new TestVertex("v1")));
            Assert.Throws<ArgumentNullException>(() => new SEdge<TestVertex>(new TestVertex("v1"), null));
            Assert.Throws<ArgumentNullException>(() => new SEdge<TestVertex>(null, null));
            // ReSharper restore AssignNullToNotNullAttribute
            // ReSharper restore ObjectCreationAsStatement
        }

        [Test]
        public void Equals()
        {
            var edge1 = new SEdge<int>(0, 0); //default(SEdge<int>);
            var edge2 = new SEdge<int>(0, 0);
            var edge3 = new SEdge<int>(1, 2);
            var edge4 = new SEdge<int>(1, 2);
            var edge5 = new SEdge<int>(2, 1);

            Assert.AreEqual(edge1, edge1);

            Assert.AreEqual(edge1, edge2);  // Is equatable
            Assert.AreEqual(edge2, edge1);  // Is equatable
            Assert.IsTrue(edge1.Equals(edge2));  // Is equatable
            Assert.IsTrue(edge2.Equals(edge1));  // Is equatable

            Assert.AreNotEqual(edge1, edge3);
            Assert.AreNotEqual(edge3, edge1);
            Assert.IsFalse(edge1.Equals(edge3));
            Assert.IsFalse(edge3.Equals(edge1));

            Assert.AreEqual(edge3, edge4);
            Assert.AreEqual(edge4, edge3);
            Assert.IsTrue(edge3.Equals(edge4));
            Assert.IsTrue(edge4.Equals(edge3));

            Assert.AreNotEqual(edge3, edge5);
            Assert.AreNotEqual(edge5, edge3);
            Assert.IsFalse(edge3.Equals(edge5));
            Assert.IsFalse(edge5.Equals(edge3));

            Assert.AreNotEqual(null, edge1);
            Assert.IsFalse(edge1.Equals(null));
        }

        [Test]
        public void ObjectToString()
        {
            var edge1 = new SEdge<int>(1, 2);
            var edge2 = new SEdge<int>(2, 1);

            Assert.AreEqual("1 -> 2", edge1.ToString());
            Assert.AreEqual("2 -> 1", edge2.ToString());
        }
    }
}