using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using JetBrains.Annotations;

namespace QuikGraph
{
    /// <inheritdoc cref="CreateReversedBidirectionalGraph{TVertex, TEdge}"/>
    public static class ReversedBidirectionalGraph
    {
        /// <summary> Creates a new <see cref="ReversedBidirectionalGraph{TVertex,TEdge}"/> class. </summary>
        public static ReversedBidirectionalGraph<TVertex, TEdge> CreateReversedBidirectionalGraph<TVertex, TEdge>
            ([NotNull] this IBidirectionalGraph<TVertex, TEdge> originalGraph) where TEdge : IEdge<TVertex>
            => new ReversedBidirectionalGraph<TVertex, TEdge>(originalGraph);
    }

    /// <summary> Mutable reversed bidirectional <see cref="OriginalGraph"/>. </summary>
    /// <remarks>It is mutable via the original graph.</remarks>
#if SUPPORTS_SERIALIZATION
    [Serializable]
#endif
    [DebuggerDisplay("VertexCount = {" + nameof(VertexCount) + "}, EdgeCount = {" + nameof(EdgeCount) + "}")]
    public sealed class ReversedBidirectionalGraph<TVertex, TEdge> : IBidirectionalGraph<TVertex, SReversedEdge<TVertex, TEdge>>
        where TEdge : IEdge<TVertex>
    {
        /// <inheritdoc />
        public Func<TVertex, TVertex, bool> AreVerticesEqual
        {
            get => areVerticesEqual ?? EqualityComparer<TVertex>.Default.Equals;
            set => areVerticesEqual = value;
        }
        [CanBeNull]
        private Func<TVertex, TVertex, bool> areVerticesEqual;

        /// <summary> Initializes a new <see cref="ReversedBidirectionalGraph{TVertex,TEdge}"/> class. </summary>
        /// <param name="originalGraph">Original graph to reverse.</param>
        /// <exception cref="T:System.ArgumentNullException"><paramref name="originalGraph"/> is <see langword="null"/>.</exception>
        internal ReversedBidirectionalGraph([NotNull] IBidirectionalGraph<TVertex, TEdge> originalGraph)
        {
            OriginalGraph = originalGraph ?? throw new ArgumentNullException(nameof(originalGraph));
        }

        /// <summary> Wrapped original graph. </summary>
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
        public bool ContainsVertex(TVertex vertex) => OriginalGraph.ContainsVertex(vertex);

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
        public bool ContainsEdge(TVertex source, TVertex target) => OriginalGraph.ContainsEdge(target, source);

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


        /// <summary> Returns an empty Edge-Set </summary>
        public IEnumerable<SReversedEdge<TVertex, TEdge>> Empty => Edge.Empty<SReversedEdge<TVertex, TEdge>>();

        /// <inheritdoc />
        public IEnumerable<SReversedEdge<TVertex, TEdge>> GetEdges(TVertex source, TVertex target)
        {
            if (target == null)
                throw new ArgumentNullException(nameof(target));

            if (!ContainsVertex(source))
            {
                return Empty;
            }

            var originalEdges = OriginalGraph.GetEdges(target, source);
            var edges = originalEdges?.Select(edge => new SReversedEdge<TVertex, TEdge>(edge));
            return edges;

        }

        #endregion

        #region IImplicitGraph<TVertex,TEdge>

        /// <inheritdoc />
        public int? OutDegree(TVertex vertex) => OriginalGraph.InDegree(vertex);

        /// <inheritdoc />
        public IEnumerable<SReversedEdge<TVertex, TEdge>> OutEdges(TVertex vertex)
            => OriginalGraph.InEdges(vertex)?.ReverseEdges<TVertex, TEdge>();

        /// <inheritdoc />
        public SReversedEdge<TVertex, TEdge> OutEdge(TVertex vertex, int index)
            => new SReversedEdge<TVertex, TEdge>(OriginalGraph.InEdge(vertex, index));

        #endregion

        #region IBidirectionalIncidenceGraph<TVertex,TEdge>

        /// <inheritdoc />
        public IEnumerable<SReversedEdge<TVertex, TEdge>> InEdges(TVertex vertex)
            => OriginalGraph.OutEdges(vertex)?.ReverseEdges<TVertex, TEdge>();

        /// <inheritdoc />
        public SReversedEdge<TVertex, TEdge> InEdge(TVertex vertex, int index)
            => new SReversedEdge<TVertex, TEdge>(OriginalGraph.OutEdge(vertex, index));

        /// <inheritdoc />
        public int? InDegree(TVertex vertex) => OriginalGraph.OutDegree(vertex);

        /// <inheritdoc />
        public int? Degree(TVertex vertex) => OriginalGraph.Degree(vertex);

        #endregion
    }
}