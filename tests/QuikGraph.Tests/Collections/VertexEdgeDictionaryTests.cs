using System;
using NUnit.Framework;
using QuikGraph.Collections;

namespace QuikGraph.Tests.Collections
{
    /// <summary>
    /// Tests for <see cref="VertexEdgeDictionary{TVertex,TEdge}"/>.
    /// </summary>
    [TestFixture]
    internal sealed class VertexEdgeDictionaryTests
    {
        [Test]
        public void Constructors()
        {
            // ReSharper disable ObjectCreationAsStatement
            Assert.DoesNotThrow(() => new VertexEdgeDictionary<int, Edge<int>>());
            Assert.DoesNotThrow(() => new VertexEdgeDictionary<int, Edge<int>>(12));
            // ReSharper restore ObjectCreationAsStatement
        }

        [Test]
        public void Clone()
        {
            var dictionary = new VertexEdgeDictionary<int, EquatableEdge<int>>();

            VertexEdgeDictionary<int, EquatableEdge<int>> clonedDictionary = dictionary.Clone();
            CollectionAssert.IsEmpty(clonedDictionary);

            clonedDictionary = (VertexEdgeDictionary<int, EquatableEdge<int>>)((IVertexEdgeDictionary<int, EquatableEdge<int>>)dictionary).Clone();
            CollectionAssert.IsEmpty(clonedDictionary);

            clonedDictionary = (VertexEdgeDictionary<int, EquatableEdge<int>>)((ICloneable)dictionary).Clone();
            CollectionAssert.IsEmpty(clonedDictionary);

            dictionary.Add(1, new EdgeList<int, EquatableEdge<int>> { new EquatableEdge<int>(1, 2) });
            dictionary.Add(2, new EdgeList<int, EquatableEdge<int>> { new EquatableEdge<int>(2, 3) });
            dictionary.Add(3, new EdgeList<int, EquatableEdge<int>>());
            clonedDictionary = dictionary.Clone();
            CollectionAssert.AreEqual(dictionary, clonedDictionary);

            clonedDictionary = (VertexEdgeDictionary<int, EquatableEdge<int>>)((IVertexEdgeDictionary<int, EquatableEdge<int>>)dictionary).Clone();
            CollectionAssert.AreEqual(dictionary, clonedDictionary);

            clonedDictionary = (VertexEdgeDictionary<int, EquatableEdge<int>>)((ICloneable)dictionary).Clone();
            CollectionAssert.AreEqual(dictionary, clonedDictionary);
        }
    }
}