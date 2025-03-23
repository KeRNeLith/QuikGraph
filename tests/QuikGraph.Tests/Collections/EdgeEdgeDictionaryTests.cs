using System;
using NUnit.Framework;
using QuikGraph.Collections;
using static QuikGraph.Tests.SerializationTestHelpers;

namespace QuikGraph.Tests.Collections
{
    /// <summary>
    /// Tests for <see cref="EdgeEdgeDictionary{TVertex,TEdge}"/>.
    /// </summary>
    [TestFixture]
    internal sealed class EdgeEdgeDictionaryTests
    {
        [Test]
        public void Constructors()
        {
            // ReSharper disable ObjectCreationAsStatement
            Assert.DoesNotThrow(() => new EdgeEdgeDictionary<int, IEdge<int>>());
            Assert.DoesNotThrow(() => new EdgeEdgeDictionary<int, IEdge<int>>(12));
            // ReSharper restore ObjectCreationAsStatement
        }

        [Test]
        public void Serialization()
        {
            var dictionary = new EdgeEdgeDictionary<int, EquatableEdge<int>>();

            EdgeEdgeDictionary<int, EquatableEdge<int>> deserializedDictionary = SerializeAndDeserialize(dictionary);
            Assert.AreNotSame(dictionary, deserializedDictionary);
            CollectionAssert.IsEmpty(deserializedDictionary);

            dictionary.Add(new EquatableEdge<int>(1, 2), new EquatableEdge<int>(2, 3));
            dictionary.Add(new EquatableEdge<int>(2, 3), new EquatableEdge<int>(3, 4));
            deserializedDictionary = SerializeAndDeserialize(dictionary);
            Assert.AreNotSame(dictionary, deserializedDictionary);
            CollectionAssert.AreEqual(dictionary, deserializedDictionary);
        }

        [Test]
        public void Clone()
        {
            var dictionary = new EdgeEdgeDictionary<int, EquatableEdge<int>>();

            EdgeEdgeDictionary<int, EquatableEdge<int>> clonedDictionary = dictionary.Clone();
            CollectionAssert.IsEmpty(clonedDictionary);

            clonedDictionary = (EdgeEdgeDictionary<int, EquatableEdge<int>>)((ICloneable)dictionary).Clone();
            CollectionAssert.IsEmpty(clonedDictionary);

            dictionary.Add(new EquatableEdge<int>(1, 2), new EquatableEdge<int>(2, 3));
            dictionary.Add(new EquatableEdge<int>(2, 3), new EquatableEdge<int>(3, 4));
            clonedDictionary = dictionary.Clone();
            CollectionAssert.AreEqual(dictionary, clonedDictionary);

            clonedDictionary = (EdgeEdgeDictionary<int, EquatableEdge<int>>)((ICloneable)dictionary).Clone();
            CollectionAssert.AreEqual(dictionary, clonedDictionary);
        }
    }
}