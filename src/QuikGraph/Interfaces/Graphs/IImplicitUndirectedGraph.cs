using System.Collections.Generic;
using JetBrains.Annotations;
using QuikGraph.Algorithms;

namespace QuikGraph
{
    /// <summary>
    /// An implicit undirected graph with vertices of type <typeparamref name="TVertex"/>
    /// and edges of type <typeparamref name="TEdge"/>.
    /// </summary>
    /// <typeparam name="TVertex">Vertex type.</typeparam>
    /// <typeparam name="TEdge">Edge type.</typeparam>
    public interface IImplicitUndirectedGraph<TVertex, TEdge> : IGraph<TVertex, TEdge>
        where TEdge : IEdge<TVertex>
    {
        /// <summary> Compares edges. </summary>
        [NotNull]
        EdgeEqualityComparer<TVertex> EdgeEqualityComparer { get; }

        /// <summary>  All adjacent Edges (where either Source or Target are <paramref name="vertex"/>. </summary>
        /// <returns>all edges adjacent to the <paramref name="vertex"/></returns>
        [Pure]
        [ItemNotNull] [CanBeNull]
        IEnumerable<TEdge> AdjacentEdges([NotNull] TVertex vertex);

        /// <summary> Gives the adjacent degree of the given <paramref name="vertex"/>. </summary>
        /// <exception cref="T:System.ArgumentNullException"><paramref name="vertex"/> is <see langword="null"/>.</exception>
        /// <exception cref="VertexNotFoundException"><paramref name="vertex"/> is not part of the graph.</exception>
        /// <remarks>
        /// The Degree is the Number of Edges connected to the <paramref name="vertex"/>.
        /// Self-edges from one vertex to itself count twice. 
        /// This is similar to the <see cref="VertexAndEdgeListGraphX.VertexDegree{TVertex,TEdge}"/> for directed Graphs.
        /// </remarks>
        [Pure]
        int? AdjacentDegree([NotNull] TVertex vertex);

        /// <summary>
        /// Gets the <paramref name="index"/>th adjacent edge of the given <paramref name="vertex"/>.
        /// </summary>
        /// <param name="vertex">The vertex.</param>
        /// <param name="index">Index of the adjacent edge requested.</param>
        /// <returns>The adjacent edge.</returns>
        /// <exception cref="T:System.ArgumentNullException"><paramref name="vertex"/> is <see langword="null"/>.</exception>
        /// <exception cref="T:System.ArgumentOutOfRangeException">No vertex at <paramref name="index"/>.</exception>
        /// <exception cref="VertexNotFoundException"><paramref name="vertex"/> is not part of the graph.</exception>
        [Pure]
        [CanBeNull]
        TEdge AdjacentEdge([NotNull] TVertex vertex, int index);

        /// <summary>
        /// Tries to get the edge that link
        /// <paramref name="source"/> and <paramref name="target"/> vertices.
        /// </summary>
        /// <param name="source">Source vertex.</param>
        /// <param name="target">Target vertex.</param>
        /// <param name="edge">Edge found, otherwise <see langword="null"/>.</param>
        /// <returns>True if an edge was found, false otherwise.</returns>
        /// <exception cref="T:System.ArgumentNullException"><paramref name="source"/> is <see langword="null"/>.</exception>
        /// <exception cref="T:System.ArgumentNullException"><paramref name="target"/> is <see langword="null"/>.</exception>
        [Pure]
        [ContractAnnotation("=> true, edge:notnull;=> false, edge:null")]
        bool TryGetEdge([NotNull] TVertex source, [NotNull] TVertex target, out TEdge edge);

        /// <summary>
        /// Checks if this graph contains an edge that link
        /// <paramref name="source"/> and <paramref name="target"/> vertices.
        /// </summary>
        /// <param name="source">Source vertex.</param>
        /// <param name="target">Target vertex.</param>
        /// <returns>True if an edge exists, false otherwise.</returns>
        /// <exception cref="T:System.ArgumentNullException"><paramref name="source"/> is <see langword="null"/>.</exception>
        /// <exception cref="T:System.ArgumentNullException"><paramref name="target"/> is <see langword="null"/>.</exception>
        [Pure]
        bool ContainsEdge([NotNull] TVertex source, [NotNull] TVertex target);
    }
}