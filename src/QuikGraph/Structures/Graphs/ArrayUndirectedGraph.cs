using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
#if SUPPORTS_SERIALIZATION && NETSTANDARD2_0
using System.Runtime.Serialization;
using System.Security.Permissions;
#endif
using JetBrains.Annotations;

namespace QuikGraph
{
    /// <summary>
    /// Immutable undirected graph data structure.
    /// </summary>
    /// <typeparam name="TVertex">Vertex type.</typeparam>
    /// <typeparam name="TEdge">Edge type</typeparam>
#if SUPPORTS_SERIALIZATION
    [Serializable]
#endif
    [DebuggerDisplay("VertexCount = {" + nameof(VertexCount) + "}, EdgeCount = {" + nameof(EdgeCount) + "}")]
    public sealed class ArrayUndirectedGraph<TVertex, TEdge> : IUndirectedGraph<TVertex, TEdge>
#if SUPPORTS_CLONEABLE
        , ICloneable
#endif
#if SUPPORTS_SERIALIZATION && NETSTANDARD2_0
        , ISerializable
#endif
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

        /// <summary>
        /// Initializes a new <see cref="ArrayUndirectedGraph{TVertex,TEdge}"/> class.
        /// </summary>
        /// <param name="baseGraph">Wrapped graph.</param>
        /// <exception cref="T:System.ArgumentNullException"><paramref name="baseGraph"/> is <see langword="null"/>.</exception>
        public ArrayUndirectedGraph([NotNull] IUndirectedGraph<TVertex, TEdge> baseGraph)
        {
            if (baseGraph is null)
                throw new ArgumentNullException(nameof(baseGraph));

            AllowParallelEdges = baseGraph.AllowParallelEdges;
            EdgeEqualityComparer = baseGraph.EdgeEqualityComparer;
            EdgeCount = baseGraph.EdgeCount;
            _vertexEdges = new Dictionary<TVertex, TEdge[]>(baseGraph.VertexCount);
            foreach (TVertex vertex in baseGraph.Vertices)
            {
                _vertexEdges.Add(
                    vertex,
                    baseGraph.AdjacentEdges(vertex).ToArray());
            }

            _edges = new List<TEdge>(baseGraph.Edges);
        }

        /// <inheritdoc />
        public EdgeEqualityComparer<TVertex> EdgeEqualityComparer { get; }

        #region IGraph<TVertex,TEdge>

        /// <inheritdoc />
        public bool IsDirected => false;

        /// <inheritdoc />
        public bool AllowParallelEdges { get; }

        #endregion

        #region IVertexSet<TVertex>

        /// <inheritdoc />
        public bool IsVerticesEmpty => _vertexEdges.Count == 0;

        /// <inheritdoc />
        public int VertexCount => _vertexEdges.Count;

        [NotNull]
        private readonly IDictionary<TVertex, TEdge[]> _vertexEdges;

        /// <inheritdoc />
        public IEnumerable<TVertex> Vertices => _vertexEdges.Keys.AsEnumerable();

        #endregion

        #region IImplicitVertexSet<TVertex>

        /// <inheritdoc />
        public bool ContainsVertex(TVertex vertex)
        {
            if (vertex == null)
                throw new ArgumentNullException(nameof(vertex));

            return _vertexEdges.ContainsKey(vertex);
        }

        #endregion

        #region IEdgeSet<TVertex,TEdge>

        /// <inheritdoc />
        public bool IsEdgesEmpty => EdgeCount == 0;

        /// <inheritdoc />
        public int EdgeCount { get; }

        [NotNull, ItemNotNull]
        private readonly IList<TEdge> _edges;

        /// <inheritdoc />
        public IEnumerable<TEdge> Edges => _edges.AsEnumerable();

        /// <inheritdoc />
        public bool ContainsEdge(TEdge edge)
        {
            if (edge == null)
                throw new ArgumentNullException(nameof(edge));

            if (_vertexEdges.TryGetValue(edge.Source, out TEdge[] edges))
                return edges.Any(e => EqualityComparer<TEdge>.Default.Equals(e, edge));
            return false;
        }

        /// <inheritdoc />
        public bool ContainsEdge(TVertex source, TVertex target)
        {
            return TryGetEdge(source, target, out _);
        }

        #endregion

        #region IImplicitUndirectedGraph<TVertex,TEdge>

        /// <inheritdoc />
        public IEnumerable<TEdge> AdjacentEdges(TVertex vertex)
        {
            if (vertex == null)
                throw new ArgumentNullException(nameof(vertex));

            if (_vertexEdges.TryGetValue(vertex, out TEdge[] edges))
                return edges.AsEnumerable();

            return null;
        }

        /// <inheritdoc />
        public int? AdjacentDegree(TVertex vertex)
        {
            if (vertex == null)
                throw new ArgumentNullException(nameof(vertex));

            if (_vertexEdges.TryGetValue(vertex, out TEdge[] edges))
                return edges.Sum(edge => edge.IsSelfEdge(AreVerticesEqual) ? 2 : 1);    // Self edge count twice

            return null;
        }

        /// <inheritdoc />
        public TEdge AdjacentEdge(TVertex vertex, int index)
        {
            if (vertex == null)
                throw new ArgumentNullException(nameof(vertex));

            if (_vertexEdges.TryGetValue(vertex, out TEdge[] adjacentEdges))
                return adjacentEdges[index];

            return default(TEdge);
        }

        /// <inheritdoc />
        public bool TryGetEdge(TVertex source, TVertex target, out TEdge edge)
        {
            if (source == null)
                throw new ArgumentNullException(nameof(source));
            if (target == null)
                throw new ArgumentNullException(nameof(target));

            if (Comparer<TVertex>.Default.Compare(source, target) > 0)
            {
                TVertex temp = source;
                source = target;
                target = temp;
            }

            if (_vertexEdges.TryGetValue(source, out TEdge[] adjacentEdges))
            {
                foreach (TEdge adjacentEdge in adjacentEdges.Where(adjacentEdge => EdgeEqualityComparer(adjacentEdge, source, target)))
                {
                    edge = adjacentEdge;
                    return true;
                }
            }

            edge = default(TEdge);
            return false;
        }

        #endregion

#if SUPPORTS_SERIALIZATION && NETSTANDARD2_0
        #region ISerializable

        private ArrayUndirectedGraph(SerializationInfo info, StreamingContext context)
        {
            AllowParallelEdges = (bool)info.GetValue("AllowParallelEdges", typeof(bool));
            _vertexEdges = (IDictionary<TVertex, TEdge[]>)info.GetValue(
                "VertexEdges",
                typeof(IDictionary<TVertex, TEdge[]>));
            _edges = (IList<TEdge>)info.GetValue("Edges", typeof(IList<TEdge>));
            EdgeEqualityComparer = EdgeExtensions.GetUndirectedVertexEquality<TVertex, TEdge>();
            EdgeCount = _edges.Count;
        }

        /// <inheritdoc />
        [SecurityPermission(SecurityAction.LinkDemand, Flags = SecurityPermissionFlag.SerializationFormatter)]
        void ISerializable.GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("AllowParallelEdges", AllowParallelEdges);
            info.AddValue("VertexEdges", _vertexEdges);
            info.AddValue("Edges", _edges);
        }

        #endregion
#endif

        #region ICloneable

        /// <summary>
        /// Clones this graph, returns this instance because this class is immutable.
        /// </summary>
        /// <returns>This graph.</returns>
        [Pure]
        [NotNull]
        public ArrayUndirectedGraph<TVertex, TEdge> Clone()
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