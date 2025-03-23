using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;

namespace QuikGraph
{
    /// <summary> A delegate-based directed bidirectional graph data structure. </summary>
    /// <typeparam name="TVertex">Vertex type.</typeparam>
    /// <typeparam name="TEdge">Edge type.</typeparam>
    public class DelegateBidirectionalIncidenceGraph<TVertex, TEdge> : DelegateIncidenceGraph<TVertex, TEdge>, IBidirectionalIncidenceGraph<TVertex, TEdge>
        where TEdge : IEdge<TVertex>
    {
        /// <summary>
        /// Initializes a new <see cref="DelegateBidirectionalIncidenceGraph{TVertex,TEdge}"/> class.
        /// </summary>
        /// <param name="tryGetOutEdges">Getter of out-edges.</param>
        /// <param name="tryGetInEdges">Getter of in-edges.</param>
        /// <param name="allowParallelEdges">
        /// Indicates if parallel edges are allowed.
        /// Note that get of edges is delegated so you may have bugs related
        /// to parallel edges due to the delegated implementation.
        /// </param>
        /// <exception cref="T:System.ArgumentNullException"><paramref name="tryGetOutEdges"/> is <see langword="null"/>.</exception>
        /// <exception cref="T:System.ArgumentNullException"><paramref name="tryGetInEdges"/> is <see langword="null"/>.</exception>
        public DelegateBidirectionalIncidenceGraph(
            [NotNull] Func<TVertex, IEnumerable<TEdge>> tryGetOutEdges,
            [NotNull] Func<TVertex, IEnumerable<TEdge>> tryGetInEdges,
            bool allowParallelEdges = true)
            : base(tryGetOutEdges, allowParallelEdges)
        {
            _tryGetInEdgesFunc = tryGetInEdges ?? throw new ArgumentNullException(nameof(tryGetInEdges));
        }

        /// <summary>
        /// Getter of in-edges.
        /// </summary>
        [NotNull]
        private readonly Func<TVertex, IEnumerable<TEdge>> _tryGetInEdgesFunc;

        #region IBidirectionalImplicitGraph<TVertex,TEdge>

        /// <inheritdoc />
        public int? InDegree(TVertex vertex) => InEdges(vertex)?.Count();

        /// <inheritdoc />
        public IEnumerable<TEdge> InEdges(TVertex vertex)
        {
            if (vertex == null)
                throw new ArgumentNullException(nameof(vertex));

            return _tryGetInEdgesFunc(vertex);
        }

        /// <inheritdoc />
        public TEdge InEdge(TVertex vertex, int index)
        {
            var inEdges = InEdges(vertex);

            if (inEdges == null)
            {
                return default(TEdge);
            }

            return inEdges.ElementAt(index);
        }

        /// <inheritdoc />
        public int? Degree(TVertex vertex) => InDegree(vertex) + OutDegree(vertex);

        #endregion
    }
}