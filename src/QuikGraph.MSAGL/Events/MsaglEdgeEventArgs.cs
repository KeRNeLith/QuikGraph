using System;
using JetBrains.Annotations;

namespace QuikGraph.MSAGL
{
    /// <summary>
    /// Arguments of an event related to an MSAGL edge.
    /// </summary>
    /// <typeparam name="TVertex">Vertex type.</typeparam>
    /// <typeparam name="TEdge">Edge type.</typeparam>
    [Serializable]
    public class MsaglEdgeEventArgs<TVertex, TEdge> : EdgeEventArgs<TVertex, TEdge>
        where TEdge : IEdge<TVertex>
    {
        /// <summary>
        /// Initializes a new <see cref="MsaglEdgeEventArgs{TVertex, TEdge}"/> class.
        /// </summary>
        /// <param name="edge">Concerned edge.</param>
        /// <param name="msaglEdge">Concerned <see cref="T:Microsoft.Msagl.Drawing.Edge"/>.</param>
        /// <exception cref="T:System.ArgumentNullException"><paramref name="edge"/> is <see langword="null"/>.</exception>
        /// <exception cref="T:System.ArgumentNullException"><paramref name="msaglEdge"/> is <see langword="null"/>.</exception>
        public MsaglEdgeEventArgs([NotNull] TEdge edge, [NotNull] Microsoft.Msagl.Drawing.Edge msaglEdge)
            : base(edge)
        {
            MsaglEdge = msaglEdge ?? throw new ArgumentNullException(nameof(msaglEdge));
        }

        /// <summary>
        /// <see cref="T:Microsoft.Msagl.Drawing.Edge"/> concerned by the event.
        /// </summary>
        [NotNull]
        public Microsoft.Msagl.Drawing.Edge MsaglEdge { get; }
    }
}