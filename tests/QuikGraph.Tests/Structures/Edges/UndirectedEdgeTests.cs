using System;
using NUnit.Framework;

namespace QuikGraph.Tests.Structures
{
    /// <summary>
    /// Tests for <see cref="UndirectedEdge{TVertex}"/>.
    ///</summary>
    [TestFixture]
    internal sealed class UndirectedEdgeTests : EdgeTestsBase
    {
        [Test]
        public void Construction()
        {
            // Value type
            CheckEdge(new UndirectedEdge<int>(1, 2), 1, 2);
            CheckEdge(new UndirectedEdge<int>(1, 1), 1, 1);

            // Reference type
            var v1 = new ComparableTestVertex("v1");
            var v2 = new ComparableTestVertex("v2");
            CheckEdge(new UndirectedEdge<ComparableTestVertex>(v1, v2), v1, v2);
            CheckEdge(new UndirectedEdge<ComparableTestVertex>(v1, v1), v1, v1);

            // Order correctly on creation
            CheckEdge(new UndirectedEdge<int>(2, 1), 1, 2);

            // Comparable
            var comparableV1 = new ComparableTestVertex("v1");
            var comparableV2 = new ComparableTestVertex("v2");
            CheckEdge(new UndirectedEdge<ComparableTestVertex>(comparableV2, comparableV1), comparableV1, comparableV2);
        }

        [Test]
        public void Construction_Throws()
        {
            // ReSharper disable ObjectCreationAsStatement
            // ReSharper disable AssignNullToNotNullAttribute
            Assert.Throws<ArgumentNullException>(() => new UndirectedEdge<TestVertex>(null, new TestVertex("v1")));
            Assert.Throws<ArgumentNullException>(() => new UndirectedEdge<TestVertex>(new TestVertex("v1"), null));
            Assert.Throws<ArgumentNullException>(() => new UndirectedEdge<TestVertex>(null, null));
            // ReSharper restore AssignNullToNotNullAttribute


            // Not comparable
            var v1 = new TestVertex("v1");
            var v2 = new TestVertex("v2");
            Assert.Throws<ArgumentException>(() => new UndirectedEdge<TestVertex>(v1, v2));
            // ReSharper restore ObjectCreationAsStatement
        }

        [Test]
        public void Equals()
        {
            var edge1 = new UndirectedEdge<int>(1, 2);
            var edge2 = new UndirectedEdge<int>(1, 2);

            Assert.AreEqual(edge1, edge1);

            Assert.AreNotEqual(edge1, edge2);
            Assert.AreNotEqual(edge2, edge1);
            Assert.IsFalse(edge1.Equals(edge2));
            Assert.IsFalse(edge2.Equals(edge1));

            Assert.AreNotEqual(null, edge1);
            Assert.IsFalse(edge1.Equals(null));
        }

        [Test]
        public void ObjectToString()
        {
            var edge = new UndirectedEdge<int>(1, 2);

            Assert.AreEqual("1 <-> 2", edge.ToString());
        }
    }
}