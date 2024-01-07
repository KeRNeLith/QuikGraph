using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using JetBrains.Annotations;

namespace QuikGraph
{
    /// <summary>
    /// Immutable directed graph data structure.
    /// </summary>
    /// <remarks>
    /// It is efficient for large sparse graph representation
    /// where out-edge need to be enumerated only.
    /// </remarks>
    /// <typeparam name="TVertex">Vertex type.</typeparam>
    /// <typeparam name="TEdge">Edge type</typeparam>
#if SUPPORTS_SERIALIZATION
    [Serializable]
#endif
    [DebuggerDisplay("VertexCount = {" + nameof(VertexCount) + "}, EdgeCount = {" + nameof(EdgeCount) + "}")]
    public sealed class ArrayAdjacencyGraph<TVertex, TEdge> : IVertexAndEdgeListGraph<TVertex, TEdge>
#if SUPPORTS_CLONEABLE
        , ICloneable
#endif
        where TEdge : IEdge<TVertex>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ArrayAdjacencyGraph{TVertex,TEdge}"/> class.
        /// </summary>
        /// <param name="baseGraph">Wrapped graph.</param>
        /// <exception cref="T:System.ArgumentNullException"><paramref name="baseGraph"/> is <see langword="null"/>.</exception>
        public ArrayAdjacencyGraph([NotNull] IVertexAndEdgeListGraph<TVertex, TEdge> baseGraph)
        {
            if (baseGraph is null)
                throw new ArgumentNullException(nameof(baseGraph));

            AllowParallelEdges = baseGraph.AllowParallelEdges;
            _vertexOutEdges = new Dictionary<TVertex, TEdge[]>(baseGraph.VertexCount);
            EdgeCount = baseGraph.EdgeCount;
            foreach (TVertex vertex in baseGraph.Vertices)
            {
                _vertexOutEdges.Add(
                    vertex,
                    baseGraph.OutEdges(vertex).ToArray());
            }
        }

        #region IGraph<TVertex,TEdge>

        /// <inheritdoc />
        public bool IsDirected => true;

        /// <inheritdoc />
        public bool AllowParallelEdges { get; }

        #endregion

        #region IVertexSet<TVertex>

        /// <inheritdoc />
        public bool IsVerticesEmpty => VertexCount == 0;

        /// <inheritdoc />
        public int VertexCount => _vertexOutEdges.Count;

        [NotNull]
        private readonly Dictionary<TVertex, TEdge[]> _vertexOutEdges;

        /// <inheritdoc />
        public IEnumerable<TVertex> Vertices => _vertexOutEdges.Keys;

        /// <inheritdoc />
        public bool ContainsVertex(TVertex vertex)
        {
            if (vertex == null)
                throw new ArgumentNullException(nameof(vertex));

            return _vertexOutEdges.ContainsKey(vertex);
        }

        #endregion

        #region IEdgeSet<TVertex,TEdge>

        /// <inheritdoc />
        public bool IsEdgesEmpty => EdgeCount == 0;

        /// <inheritdoc />
        public int EdgeCount { get; }

        /// <inheritdoc />
        public IEnumerable<TEdge> Edges => _vertexOutEdges.Values.SelectMany(edges => edges);

        /// <inheritdoc />
        public bool ContainsEdge(TEdge edge)
        {
            if (edge == null)
                throw new ArgumentNullException(nameof(edge));

            if (_vertexOutEdges.TryGetValue(edge.Source, out TEdge[] outEdges))
                return Array.Exists(outEdges, outEdge => EqualityComparer<TEdge>.Default.Equals(outEdge, edge));
            return false;
        }

        /// <inheritdoc />
        public bool ContainsEdge(TVertex source, TVertex target)
        {
            return TryGetEdge(source, target, out _);
        }

        #endregion

        #region IImplicitGraph<TVertex,TEdge>

        /// <inheritdoc />
        public bool IsOutEdgesEmpty(TVertex vertex)
        {
            return OutDegree(vertex) == 0;
        }

        /// <inheritdoc />
        public int OutDegree(TVertex vertex)
        {
            if (vertex == null)
                throw new ArgumentNullException(nameof(vertex));

            if (_vertexOutEdges.TryGetValue(vertex, out TEdge[] edges))
                return edges.Length;
            throw new VertexNotFoundException();
        }

        /// <inheritdoc />
        public IEnumerable<TEdge> OutEdges(TVertex vertex)
        {
            if (vertex == null)
                throw new ArgumentNullException(nameof(vertex));

            if (_vertexOutEdges.TryGetValue(vertex, out TEdge[] edges))
                return edges;
            throw new VertexNotFoundException();
        }

        /// <inheritdoc />
        public bool TryGetOutEdges(TVertex vertex, out IEnumerable<TEdge> edges)
        {
            if (vertex == null)
                throw new ArgumentNullException(nameof(vertex));

            if (_vertexOutEdges.TryGetValue(vertex, out TEdge[] outEdges))
            {
                edges = outEdges;
                return true;
            }

            edges = null;
            return false;
        }

        /// <inheritdoc />
        public TEdge OutEdge(TVertex vertex, int index)
        {
            if (vertex == null)
                throw new ArgumentNullException(nameof(vertex));

            if (_vertexOutEdges.TryGetValue(vertex, out TEdge[] outEdges))
                return outEdges[index];
            throw new VertexNotFoundException();
        }

        #endregion

        #region IIncidenceGraph<TVertex,TEdge>

        /// <inheritdoc />
        public bool TryGetEdge(TVertex source, TVertex target, out TEdge edge)
        {
            if (source == null)
                throw new ArgumentNullException(nameof(source));
            if (target == null)
                throw new ArgumentNullException(nameof(target));

            if (_vertexOutEdges.TryGetValue(source, out TEdge[] outEdges))
            {
                foreach (TEdge outEdge in outEdges)
                {
                    if (EqualityComparer<TVertex>.Default.Equals(outEdge.Target, target))
                    {
                        edge = outEdge;
                        return true;
                    }
                }
            }

            edge = default(TEdge);
            return false;
        }

        /// <inheritdoc />
        public bool TryGetEdges(TVertex source, TVertex target, out IEnumerable<TEdge> edges)
        {
            if (source == null)
                throw new ArgumentNullException(nameof(source));
            if (target == null)
                throw new ArgumentNullException(nameof(target));

            if (_vertexOutEdges.TryGetValue(source, out TEdge[] outEdges))
            {
                edges = outEdges.Where(edge => EqualityComparer<TVertex>.Default.Equals(edge.Target, target));
                return true;
            }

            edges = null;
            return false;
        }

        #endregion

        #region ICloneable

        /// <summary>
        /// Clones this graph, returns this instance because this class is immutable.
        /// </summary>
        /// <returns>This graph.</returns>
        [Pure]
        [NotNull]
        public ArrayAdjacencyGraph<TVertex, TEdge> Clone()
        {
            return this;
        }

#if SUPPORTS_CLONEABLE
        /// <inheritdoc />
        object ICloneable.Clone()
        {
            return Clone();
        }
#endif

        #endregion
    }
}