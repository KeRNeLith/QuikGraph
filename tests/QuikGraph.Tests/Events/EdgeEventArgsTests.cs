using System;
using NUnit.Framework;

namespace QuikGraph.Tests.Events
{
    /// <summary>
    /// Tests related to <see cref="EdgeEventArgs{TVertex,TEdge}"/>.
    /// </summary>
    internal sealed class EdgeEventArgsTests
    {
        [Test]
        public void Constructor()
        {
            var edge = Edge.Create(1, 2);
            var args = new EdgeEventArgs<int, IEdge<int>>(edge);

            Assert.AreSame(edge, args.Edge);
        }

        [Test]
        public void Constructor_Throws()
        {
            // ReSharper disable once ObjectCreationAsStatement
            // ReSharper disable once AssignNullToNotNullAttribute
            Assert.Throws<ArgumentNullException>(
                () => new EdgeEventArgs<int, IEdge<int>>(null));
        }
    }
}