using System;
using System.Collections.Generic;
using JetBrains.Annotations;

namespace QuikGraph.Algorithms.Condensation
{
    /// <summary> An edge that merge several other edges. </summary>
    /// <typeparam name="TVertex">Vertex type.</typeparam>
    /// <typeparam name="TEdge">Edge type.</typeparam>
#if SUPPORTS_SERIALIZATION
    [Serializable]
#endif
    public sealed class MergedEdge<TVertex, TEdge> : Edge<TVertex>
        where TEdge : IEdge<TVertex>
    {
        /// <summary>
        /// Initializes a new <see cref="MergedEdge{TVertex,TEdge}"/> class.
        /// </summary>
        /// <param name="source">The source vertex.</param>
        /// <param name="target">The target vertex.</param>
        /// <exception cref="T:System.ArgumentNullException"><paramref name="source"/> is <see langword="null"/>.</exception>
        /// <exception cref="T:System.ArgumentNullException"><paramref name="target"/> is <see langword="null"/>.</exception>
        public MergedEdge([NotNull] TVertex source, [NotNull] TVertex target)
            : base(source, target)
        {
        }

        [NotNull, ItemNotNull]
        private List<TEdge> _edges = new List<TEdge>();

        /// <summary>
        /// Merged edges.
        /// </summary>
        [NotNull, ItemNotNull]
        public List<TEdge> Edges => _edges;

    }

    /// <summary>
    /// Helpers for <see cref="MergedEdge{TVertex,TEdge}"/>.
    /// </summary>
    public static class MergedEdge
    {
        /// <summary> Merges the <paramref name="mergedInEdge"/> and the <paramref name="mergedOutEdge"/>. </summary>
        [Pure]
        [NotNull]
        public static MergedEdge<TVertex, TEdge> Merge<TVertex, TEdge>(
            [NotNull] this MergedEdge<TVertex, TEdge> mergedInEdge,
            [NotNull] MergedEdge<TVertex, TEdge> mergedOutEdge) where TEdge : IEdge<TVertex>
        {
            if (mergedInEdge is null)
                throw new ArgumentNullException(nameof(mergedInEdge));
            if (mergedOutEdge is null)
                throw new ArgumentNullException(nameof(mergedOutEdge));

            var newEdge = new MergedEdge<TVertex, TEdge>(mergedInEdge.Source, mergedOutEdge.Target)
            {
                //_edges = new List<TEdge>(mergedInEdge.Edges.Count + mergedOutEdge.Edges.Count)
            };
            newEdge.Edges.AddRange(mergedInEdge.Edges);
            newEdge.Edges.AddRange(mergedOutEdge.Edges);

            return newEdge;
        }
    }
}