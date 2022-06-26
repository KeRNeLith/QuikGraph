using System;
using JetBrains.Annotations;

namespace QuikGraph
{
    /// <summary>
    /// Base class for arguments of an event related to an edge.
    /// </summary>
    /// <typeparam name="TVertex">Vertex type.</typeparam>
    /// <typeparam name="TEdge">Edge type.</typeparam>
    public class EdgeEventArgs<TVertex, TEdge> : EventArgs
        where TEdge : IEdge<TVertex>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="EdgeEventArgs{TVertex, TEdge}"/> class.
        /// </summary>
        /// <param name="edge">Concerned edge.</param>
        /// <exception cref="T:System.ArgumentNullException"><paramref name="edge"/> is <see langword="null"/>.</exception>
        public EdgeEventArgs([NotNull] TEdge edge)
        {
            if (edge == null)
                throw new ArgumentNullException(nameof(edge));

            Edge = edge;
        }

        /// <summary>
        /// Edge concerned by the event.
        /// </summary>
        [NotNull]
        public TEdge Edge { get; }
    }
}