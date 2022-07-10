using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using JetBrains.Annotations;

namespace QuikGraph
{
    /// <summary>
    /// Mutable reversed bidirectional graph data structure.
    /// </summary>
    /// <remarks>It is mutable via the original graph.</remarks>
    /// <typeparam name="TVertex">Vertex type.</typeparam>
    /// <typeparam name="TEdge">Edge type</typeparam>
#if SUPPORTS_SERIALIZATION
    [Serializable]
#endif
    [DebuggerDisplay("VertexCount = {" + nameof(VertexCount) + "}, EdgeCount = {" + nameof(EdgeCount) + "}")]
    public sealed class ReversedBidirectionalGraph<TVertex, TEdge> : IBidirectionalGraph<TVertex, SReversedEdge<TVertex, TEdge>>
        where TEdge : IEdge<TVertex>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ReversedBidirectionalGraph{TVertex,TEdge}"/> class.
        /// </summary>
        /// <param name="originalGraph">Original graph to reverse.</param>
        /// <exception cref="T:System.ArgumentNullException"><paramref name="originalGraph"/> is <see langword="null"/>.</exception>
        public ReversedBidirectionalGraph([NotNull] IBidirectionalGraph<TVertex, TEdge> originalGraph)
        {
            OriginalGraph = originalGraph ?? throw new ArgumentNullException(nameof(originalGraph));
        }

        /// <summary>
        /// Original graph.
        /// </summary>
        [NotNull]
        public IBidirectionalGraph<TVertex, TEdge> OriginalGraph { get; }

        #region IGraph<TVertex,TEdge>

        /// <inheritdoc />
        public bool IsDirected => OriginalGraph.IsDirected;

        /// <inheritdoc />
        public bool AllowParallelEdges => OriginalGraph.AllowParallelEdges;

        #endregion

        #region IVertexSet<TVertex>

        /// <inheritdoc />
        public bool IsVerticesEmpty => OriginalGraph.IsVerticesEmpty;

        /// <inheritdoc />
        public int VertexCount => OriginalGraph.VertexCount;

        /// <inheritdoc />
        public IEnumerable<TVertex> Vertices => OriginalGraph.Vertices;

        /// <inheritdoc />
        public bool ContainsVertex(TVertex vertex)
        {
            return OriginalGraph.ContainsVertex(vertex);
        }

        #endregion

        #region IEdgeSet<TVertex,TEdge>

        /// <inheritdoc />
        public bool IsEdgesEmpty => OriginalGraph.IsEdgesEmpty;

        /// <inheritdoc />
        public int EdgeCount => OriginalGraph.EdgeCount;

        /// <inheritdoc />
        public IEnumerable<SReversedEdge<TVertex, TEdge>> Edges =>
            OriginalGraph.Edges.Select(edge => new SReversedEdge<TVertex, TEdge>(edge));

        /// <inheritdoc />
        public bool ContainsEdge(SReversedEdge<TVertex, TEdge> edge)
        {
            return OriginalGraph.ContainsEdge(edge.OriginalEdge);
        }

        #endregion

        #region IIncidenceGraph<TVertex,TEdge>

        /// <inheritdoc />
        public bool ContainsEdge(TVertex source, TVertex target)
        {
            return OriginalGraph.ContainsEdge(target, source);
        }

        /// <inheritdoc />
        public bool TryGetEdge(TVertex source, TVertex target, out SReversedEdge<TVertex, TEdge> edge)
        {
            if (OriginalGraph.TryGetEdge(target, source, out TEdge originalEdge))
            {
                edge = new SReversedEdge<TVertex, TEdge>(originalEdge);
                return true;
            }

            edge = default(SReversedEdge<TVertex, TEdge>);
            return false;
        }

        /// <inheritdoc />
        public bool TryGetEdges(TVertex source, TVertex target, out IEnumerable<SReversedEdge<TVertex, TEdge>> edges)
        {
            if (OriginalGraph.TryGetEdges(target, source, out IEnumerable<TEdge> originalEdges)
                && ContainsVertex(source))
            {
                edges = originalEdges.Select(edge => new SReversedEdge<TVertex, TEdge>(edge));
                return true;
            }

            edges = null;
            return false;
        }

        #endregion

        #region IImplicitGraph<TVertex,TEdge>

        /// <inheritdoc />
        public bool IsOutEdgesEmpty(TVertex vertex)
        {
            return OriginalGraph.IsInEdgesEmpty(vertex);
        }

        /// <inheritdoc />
        public int OutDegree(TVertex vertex)
        {
            return OriginalGraph.InDegree(vertex);
        }

        /// <inheritdoc />
        public IEnumerable<SReversedEdge<TVertex, TEdge>> OutEdges(TVertex vertex)
        {
            return EdgeExtensions.ReverseEdges<TVertex, TEdge>(OriginalGraph.InEdges(vertex));
        }

        /// <inheritdoc />
        public bool TryGetOutEdges(TVertex vertex, out IEnumerable<SReversedEdge<TVertex, TEdge>> edges)
        {
            if (OriginalGraph.TryGetInEdges(vertex, out IEnumerable<TEdge> inEdges))
            {
                edges = EdgeExtensions.ReverseEdges<TVertex, TEdge>(inEdges);
                return true;
            }

            edges = null;
            return false;
        }

        /// <inheritdoc />
        public SReversedEdge<TVertex, TEdge> OutEdge(TVertex vertex, int index)
        {
            return new SReversedEdge<TVertex, TEdge>(
                OriginalGraph.InEdge(vertex, index));
        }

        #endregion

        #region IBidirectionalIncidenceGraph<TVertex,TEdge>

        /// <inheritdoc />
        public IEnumerable<SReversedEdge<TVertex, TEdge>> InEdges(TVertex vertex)
        {
            return EdgeExtensions.ReverseEdges<TVertex, TEdge>(OriginalGraph.OutEdges(vertex));
        }

        /// <inheritdoc />
        public SReversedEdge<TVertex, TEdge> InEdge(TVertex vertex, int index)
        {
            return new SReversedEdge<TVertex, TEdge>(
                OriginalGraph.OutEdge(vertex, index));
        }

        /// <inheritdoc />
        public bool IsInEdgesEmpty(TVertex vertex)
        {
            return OriginalGraph.IsOutEdgesEmpty(vertex);
        }

        /// <inheritdoc />
        public int InDegree(TVertex vertex)
        {
            return OriginalGraph.OutDegree(vertex);
        }

        /// <inheritdoc />
        public bool TryGetInEdges(TVertex vertex, out IEnumerable<SReversedEdge<TVertex, TEdge>> edges)
        {
            if (OriginalGraph.TryGetOutEdges(vertex, out IEnumerable<TEdge> outEdges))
            {
                edges = EdgeExtensions.ReverseEdges<TVertex, TEdge>(outEdges);
                return true;
            }

            edges = null;
            return false;
        }

        /// <inheritdoc />
        public int Degree(TVertex vertex)
        {
            return OriginalGraph.Degree(vertex);
        }

        #endregion
    }
}