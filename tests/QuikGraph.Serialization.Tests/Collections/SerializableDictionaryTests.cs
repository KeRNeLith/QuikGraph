#if SUPPORTS_BINARY_GRAPH_SERIALIZATION
using System;
using NUnit.Framework;
using QuikGraph.Serialization.Collections;
#if SUPPORTS_SERIALIZATION
using static QuikGraph.Tests.SerializationTestHelpers;
#endif

namespace QuikGraph.Serialization.Tests
{
    [TestFixture]
    internal sealed class SerializableDictionaryTests
    {
        [Test]
        public void Constructor()
        {
            // ReSharper disable ObjectCreationAsStatement
            Assert.DoesNotThrow(() => new SerializableDictionary<int, Edge<int>>());
            Assert.DoesNotThrow(() => new SerializableDictionary<int, Edge<int>>(12));
            // ReSharper restore ObjectCreationAsStatement
        }

        [Test]
        public void Constructor_Throws()
        {
            // ReSharper disable once ObjectCreationAsStatement
            // ReSharper disable once AssignNullToNotNullAttribute
            Assert.Throws<ArgumentOutOfRangeException>(() => new SerializableDictionary<int, Edge<int>>(-5));
        }

#if SUPPORTS_SERIALIZATION
        [Test]
        public void Serialization()
        {
            var dictionary = new SerializableDictionary<int, EquatableEdge<int>>();

            SerializableDictionary<int, EquatableEdge<int>> deserializedDictionary = SerializeAndDeserialize(dictionary);
            Assert.AreNotSame(dictionary, deserializedDictionary);
            CollectionAssert.IsEmpty(deserializedDictionary);

            dictionary.Add(2, new EquatableEdge<int>(2, 3));
            dictionary.Add(3, new EquatableEdge<int>(3, 4));
            deserializedDictionary = SerializeAndDeserialize(dictionary);
            Assert.AreNotSame(dictionary, deserializedDictionary);
            CollectionAssert.AreEqual(dictionary, deserializedDictionary);
        }
#endif
    }
}
#endif