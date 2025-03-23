using System;
using NUnit.Framework;

namespace QuikGraph.Tests.Events
{
    /// <summary>
    /// Tests related to <see cref="UndirectedEdgeEventArgs{TVertex,TEdge}"/>.
    /// </summary>
    internal sealed class UndirectedEdgeEventArgsTests
    {
        [Test]
        public void Constructor()
        {
            var edge = Edge.Create(1, 2);

            var args = new UndirectedEdgeEventArgs<int, IEdge<int>>(edge, false);
            Assert.IsFalse(args.Reversed);
            Assert.AreSame(edge, args.Edge);
            Assert.AreEqual(1, args.Source);
            Assert.AreEqual(2, args.Target);

            args = new UndirectedEdgeEventArgs<int, IEdge<int>>(edge, true);
            Assert.IsTrue(args.Reversed);
            Assert.AreSame(edge, args.Edge);
            Assert.AreEqual(2, args.Source);
            Assert.AreEqual(1, args.Target);
        }

        [Test]
        public void Constructor_Throws()
        {
            // ReSharper disable ObjectCreationAsStatement
            // ReSharper disable AssignNullToNotNullAttribute
            Assert.Throws<ArgumentNullException>(
                () => new UndirectedEdgeEventArgs<int, IEdge<int>>(null, false));
            Assert.Throws<ArgumentNullException>(
                () => new UndirectedEdgeEventArgs<int, IEdge<int>>(null, true));
            // ReSharper restore AssignNullToNotNullAttribute
            // ReSharper restore ObjectCreationAsStatement
        }
    }
}