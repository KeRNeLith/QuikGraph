#if SUPPORTS_SERIALIZATION
using System;
#endif
using JetBrains.Annotations;

namespace QuikGraph
{
    /// <summary>
    /// Base class for arguments of an event related to an undirected edge.
    /// </summary>
    /// <typeparam name="TVertex">Vertex type.</typeparam>
    /// <typeparam name="TEdge">Edge type.</typeparam>
    public class UndirectedEdgeEventArgs<TVertex, TEdge> : EdgeEventArgs<TVertex, TEdge>
        where TEdge : IEdge<TVertex>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="UndirectedEdgeEventArgs{TVertex, TEdge}"/> class.
        /// </summary>
        /// <param name="edge">The edge.</param>
        /// <param name="reversed">Indicates if the edge should be reversed or not.</param>
        /// <exception cref="T:System.ArgumentNullException"><paramref name="edge"/> is <see langword="null"/>.</exception>
        public UndirectedEdgeEventArgs([NotNull] TEdge edge, bool reversed)
            : base(edge)
        {
            Reversed = reversed;
        }

        /// <summary>
        /// Indicates if the edge vertices are reversed or not.
        /// </summary>
        public bool Reversed { get; }

        /// <summary>
        /// Edge source.
        /// </summary>
        [NotNull]
        public TVertex Source => Reversed ? Edge.Target : Edge.Source;

        /// <summary>
        /// Edge target.
        /// </summary>
        [NotNull]
        public TVertex Target => Reversed ? Edge.Source : Edge.Target;
    }
}