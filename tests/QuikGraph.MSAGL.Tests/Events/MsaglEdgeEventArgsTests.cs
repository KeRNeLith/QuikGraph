using System;
using Microsoft.Msagl.Drawing;
using NUnit.Framework;

namespace QuikGraph.MSAGL.Tests
{
    /// <summary>
    /// Tests related to <see cref="MsaglEdgeEventArgs{TVertex,TEdge}"/>.
    /// </summary>
    internal sealed class MsaglEdgeEventArgsTests
    {
        [Test]
        public void Constructor()
        {
            var edge = Edge.Create(1, 2);
            var msaglEdge = new Microsoft.Msagl.Drawing.Edge(new Node("1"), new Node("2"), ConnectionToGraph.Disconnected);
            var args = new MsaglEdgeEventArgs<int, IEdge<int>>(edge, msaglEdge);

            Assert.AreSame(edge, args.Edge);
            Assert.AreSame(msaglEdge, args.MsaglEdge);
        }

        [Test]
        public void Constructor_Throws()
        {
            var edge = Edge.Create(1, 2);
            var msaglEdge = new Microsoft.Msagl.Drawing.Edge(new Node("1"), new Node("2"), ConnectionToGraph.Disconnected);

            // ReSharper disable ObjectCreationAsStatement
            // ReSharper disable AssignNullToNotNullAttribute
            Assert.Throws<ArgumentNullException>(
                () => new MsaglEdgeEventArgs<int, IEdge<int>>(edge, null));
            Assert.Throws<ArgumentNullException>(
                () => new MsaglEdgeEventArgs<int, IEdge<int>>(null, msaglEdge));
            Assert.Throws<ArgumentNullException>(
                () => new MsaglEdgeEventArgs<int, IEdge<int>>(null, null));
            // ReSharper restore AssignNullToNotNullAttribute
            // ReSharper restore ObjectCreationAsStatement
        }
    }
}