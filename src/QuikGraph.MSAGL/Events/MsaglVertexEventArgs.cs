using System;
using JetBrains.Annotations;
using Microsoft.Msagl.Drawing;

namespace QuikGraph.MSAGL
{
    /// <summary>
    /// Arguments of an event related to an MSAGL vertex.
    /// </summary>
    /// <typeparam name="TVertex">Vertex type.</typeparam>
    public class MsaglVertexEventArgs<TVertex> : VertexEventArgs<TVertex>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MsaglVertexEventArgs{TVertex}"/> class.
        /// </summary>
        /// <param name="vertex">Concerned vertex.</param>
        /// <param name="node">Concerned <see cref="T:Microsoft.Msagl.Drawing.Node"/>.</param>
        /// <exception cref="T:System.ArgumentNullException"><paramref name="vertex"/> is <see langword="null"/>.</exception>
        /// <exception cref="T:System.ArgumentNullException"><paramref name="node"/> is <see langword="null"/>.</exception>
        public MsaglVertexEventArgs([NotNull] TVertex vertex, [NotNull] Node node)
            : base(vertex)
        {
            Node = node ?? throw new ArgumentNullException(nameof(node));
        }

        /// <summary>
        /// <see cref="T:Microsoft.Msagl.Drawing.Node"/> concerned by the event.
        /// </summary>
        [NotNull]
        public Node Node { get; }
    }
}