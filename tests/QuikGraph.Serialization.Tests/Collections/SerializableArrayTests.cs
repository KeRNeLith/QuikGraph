#if SUPPORTS_BINARY_GRAPH_SERIALIZATION
using System;
using NUnit.Framework;
using QuikGraph.Serialization.Collections;

namespace QuikGraph.Serialization.Tests
{
    [TestFixture]
    internal sealed class SerializableArrayTests
    {
        [Test]
        public void Constructor()
        {
            // ReSharper disable once ObjectCreationAsStatement
            Assert.DoesNotThrow(() => new SerializableArray<int>(new[]{ 1 }));
        }

        [Test]
        public void Constructor_Throws()
        {
            // ReSharper disable once ObjectCreationAsStatement
            // ReSharper disable once AssignNullToNotNullAttribute
            Assert.Throws<ArgumentNullException>(() => new SerializableArray<int>(null));
        }

        [Test]
        public void ContentEquals()
        {
            var array1 = new TestVertex[0];
            var serializableArray = new SerializableArray<TestVertex>(array1);
            CollectionAssert.IsEmpty(serializableArray);

            var v1 = new TestVertex("1");
            var v2 = new TestVertex("2");
            var v3 = new TestVertex("3");
            TestVertex[] array2 = { v1, v2, v3 };
            TestVertex[] array3 = { v1, v2, v3 };
            CollectionAssert.AreEqual(array2, new SerializableArray<TestVertex>(array2));
            CollectionAssert.AreEqual(array3, new SerializableArray<TestVertex>(array2));

            TestVertex[] array4 = { v2, v1, v3 };
            CollectionAssert.AreEqual(array4, new SerializableArray<TestVertex>(array4));

            TestVertex[] array5 = { v1, null, v3 };
            CollectionAssert.AreEqual(array5, new SerializableArray<TestVertex>(array5));

            CollectionAssert.AreNotEqual(array1, new SerializableArray<TestVertex>(array2));
            CollectionAssert.AreNotEqual(array4, new SerializableArray<TestVertex>(array2));
            CollectionAssert.AreNotEqual(array5, new SerializableArray<TestVertex>(array2));
        }
    }
}
#endif