using System;
using NUnit.Framework;
using QuikGraph.Collections;

namespace QuikGraph.Tests.Collections
{
    /// <summary> Tests for <see cref="EdgeList{TEdge}"/>. </summary>
    [TestFixture]
    internal sealed class EdgeListTests
    {
        [Test]
        public void Constructors()
        {
            // ReSharper disable ObjectCreationAsStatement
            Assert.DoesNotThrow(() => new EdgeList<IEdge<int>>());
            Assert.DoesNotThrow(() => new EdgeList<IEdge<int>>(12));
            var list = new EdgeList<EquatableEdge<int>>
            {
                new(1, 2),
                new(2, 3)
            };
            var otherList = new EdgeList<EquatableEdge<int>>(list);
            CollectionAssert.AreEqual(list, otherList);
            // ReSharper restore ObjectCreationAsStatement
        }

        [Test]
        public void Clone()
        {
            var list = new EdgeList<EquatableEdge<int>>();

            EdgeList<EquatableEdge<int>> clonedList = list.Clone();
            CollectionAssert.IsEmpty(clonedList);

            clonedList = (EdgeList<EquatableEdge<int>>)((IEdgeList<EquatableEdge<int>>)list).Clone();
            CollectionAssert.IsEmpty(clonedList);

            clonedList = (EdgeList<EquatableEdge<int>>)((ICloneable)list).Clone();
            CollectionAssert.IsEmpty(clonedList);

            list.AddRange(new[]
            {
                new EquatableEdge<int>(1, 2),
                new EquatableEdge<int>(2, 3)
            });
            clonedList = list.Clone();
            CollectionAssert.AreEqual(list, clonedList);

            clonedList = (EdgeList<EquatableEdge<int>>)((IEdgeList<EquatableEdge<int>>)list).Clone();
            CollectionAssert.AreEqual(list, clonedList);

            clonedList = (EdgeList<EquatableEdge<int>>)((ICloneable)list).Clone();
            CollectionAssert.AreEqual(list, clonedList);
        }
    }
}