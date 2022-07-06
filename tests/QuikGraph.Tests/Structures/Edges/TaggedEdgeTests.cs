using System;
using NUnit.Framework;

namespace QuikGraph.Tests.Structures
{
    /// <summary>
    /// Tests for <see cref="TaggedEdge{TVertex,TTag}"/>.
    ///</summary>
    [TestFixture]
    internal sealed class TaggedEdgeTests : EdgeTestsBase
    {
        [Test]
        public void Construction()
        {
            var tag = new TestObject(1);

            // Value type
            CheckTaggedEdge(new TaggedEdge<int, TestObject>(1, 2, null), 1, 2, (TestObject)null);
            CheckTaggedEdge(new TaggedEdge<int, TestObject>(2, 1, null), 2, 1, (TestObject)null);
            CheckTaggedEdge(new TaggedEdge<int, TestObject>(1, 1, null), 1, 1, (TestObject)null);
            CheckTaggedEdge(new TaggedEdge<int, TestObject>(1, 2, tag), 1, 2, tag);

            // Reference type
            var v1 = new TestVertex("v1");
            var v2 = new TestVertex("v2");
            CheckTaggedEdge(new TaggedEdge<TestVertex, TestObject>(v1, v2, null), v1, v2, (TestObject)null);
            CheckTaggedEdge(new TaggedEdge<TestVertex, TestObject>(v2, v1, null), v2, v1, (TestObject)null);
            CheckTaggedEdge(new TaggedEdge<TestVertex, TestObject>(v1, v1, null), v1, v1, (TestObject)null);
            CheckTaggedEdge(new TaggedEdge<TestVertex, TestObject>(v1, v2, tag), v1, v2, tag);
        }

        [Test]
        public void Construction_Throws()
        {
            // ReSharper disable ObjectCreationAsStatement
            // ReSharper disable AssignNullToNotNullAttribute
            Assert.Throws<ArgumentNullException>(() => new TaggedEdge<TestVertex, TestObject>(null, new TestVertex("v1"), null));
            Assert.Throws<ArgumentNullException>(() => new TaggedEdge<TestVertex, TestObject>(new TestVertex("v1"), null, null));
            Assert.Throws<ArgumentNullException>(() => new TaggedEdge<TestVertex, TestObject>(null, null, null));
            // ReSharper restore AssignNullToNotNullAttribute
            // ReSharper restore ObjectCreationAsStatement
        }

        [Test]
        public void Equals()
        {
            var tag1 = new TestObject(1);
            var tag2 = new TestObject(2);
            var edge1 = new TaggedEdge<int, TestObject>(1, 2, tag1);
            var edge2 = new TaggedEdge<int, TestObject>(1, 2, tag1);
            var edge3 = new TaggedEdge<int, TestObject>(1, 2, tag2);
            var edge4 = new TaggedEdge<int, TestObject>(1, 2, null);

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

            Assert.AreNotEqual(edge1, edge4);
            Assert.AreNotEqual(edge4, edge1);
            Assert.IsFalse(edge1.Equals(edge4));
            Assert.IsFalse(edge4.Equals(edge1));
            Assert.IsFalse(edge1 == edge4);
            Assert.IsFalse(edge4 == edge1);
            Assert.IsTrue(edge1 != edge4);
            Assert.IsTrue(edge4 != edge1);

            Assert.AreNotEqual(null, edge1);
            Assert.IsFalse(edge1.Equals(null));
            Assert.IsFalse(edge1 == null);
            Assert.IsFalse(null == edge1);
            Assert.IsTrue(edge1 != null);
            Assert.IsTrue(null != edge1);
        }

        [Test]
        public void TagChanged()
        {
            var edge = new TaggedEdge<int, TestObject>(1, 2, null);

            int changeCount = 0;
            edge.TagChanged += (_, _) => ++changeCount;

            edge.Tag = null;
            Assert.AreEqual(0, changeCount);

            var tag1 = new TestObject(1);
            edge.Tag = tag1;
            Assert.AreEqual(1, changeCount);

            edge.Tag = tag1;
            Assert.AreEqual(1, changeCount);

            var tag2 = new TestObject(2);
            edge.Tag = tag2;
            Assert.AreEqual(2, changeCount);

            edge.Tag = tag1;
            Assert.AreEqual(3, changeCount);
        }

        [Test]
        public void ObjectToString()
        {
            var edge1 = new TaggedEdge<int, TestObject>(1, 2, null);
            var edge2 = new TaggedEdge<int, TestObject>(1, 2, new TestObject(42));
            var edge3 = new TaggedEdge<int, TestObject>(2, 1, null);

            Assert.AreEqual("1 -> 2 (no tag)", edge1.ToString());
            Assert.AreEqual("1 -> 2 (42)", edge2.ToString());
            Assert.AreEqual("2 -> 1 (no tag)", edge3.ToString());
        }
    }
}