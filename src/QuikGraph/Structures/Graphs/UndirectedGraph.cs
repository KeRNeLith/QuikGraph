using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
#if !SUPPORTS_TYPE_FULL_FEATURES
using System.Reflection;
#endif
#if SUPPORTS_AGGRESSIVE_INLINING
using System.Runtime.CompilerServices;
#endif
#if SUPPORTS_SERIALIZATION && NETSTANDARD2_0
using System.Runtime.Serialization;
using System.Security.Permissions;
#endif
using JetBrains.Annotations;
using QuikGraph.Collections;

namespace QuikGraph
{
    /// <summary>
    /// Mutable undirected graph data structure.
    /// </summary>
    /// <typeparam name="TVertex">Vertex type.</typeparam>
    /// <typeparam name="TEdge">Edge type</typeparam>
#if SUPPORTS_SERIALIZATION
    [Serializable]
#endif
    [DebuggerDisplay("VertexCount = {" + nameof(VertexCount) + "}, EdgeCount = {" + nameof(EdgeCount) + "}")]
    public class UndirectedGraph<TVertex, TEdge> : IMutableUndirectedGraph<TVertex, TEdge>, IMutableVertexSet<TVertex>
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
            get => _areVerticesEqual ?? EqualityComparer<TVertex>.Default.Equals;
            set => _areVerticesEqual = value;
        }
        [CanBeNull]
        private Func<TVertex, TVertex, bool> _areVerticesEqual;

        [NotNull]
        private IVertexEdgeDictionary<TVertex, TEdge> _adjacentEdges =
            new VertexEdgeDictionary<TVertex, TEdge>();

        /// <summary>
        /// Initializes a new <see cref="UndirectedGraph{TVertex,TEdge}"/> class.
        /// </summary>
        /// <remarks>Allow parallel edges and gets the best edge equality comparer.</remarks>
        public UndirectedGraph()
            : this(true)
        {
        }

        /// <summary>
        /// Initializes a new <see cref="UndirectedGraph{TVertex,TEdge}"/> class.
        /// </summary>
        /// <remarks>Gets the best edge equality comparer.</remarks>
        /// <param name="allowParallelEdges">Indicates if parallel edges are allowed.</param>
        public UndirectedGraph(bool allowParallelEdges)
            : this(allowParallelEdges, EdgeExtensions.GetUndirectedVertexEquality<TVertex, TEdge>())
        {
            AllowParallelEdges = allowParallelEdges;
        }

        /// <summary> Initializes a new, empty <see cref="UndirectedGraph{TVertex,TEdge}"/> class. </summary>
        /// <param name="allowParallelEdges">Indicates if parallel edges are allowed.</param>
        /// <param name="edgeEqualityComparer">Equality comparer to use to compare edges.</param>
        /// <exception cref="T:System.ArgumentNullException"><paramref name="edgeEqualityComparer"/> is <see langword="null"/>.</exception>
        /// <remarks>by setting <paramref name="allowParallelEdges"/> to false and removing any Self-Edges,
        /// you can create a 'simple' Graph, required for . </remarks>
        public UndirectedGraph(bool allowParallelEdges, [NotNull] EdgeEqualityComparer<TVertex> edgeEqualityComparer)
        {
            AllowParallelEdges = allowParallelEdges;
            EdgeEqualityComparer = edgeEqualityComparer ?? throw new ArgumentNullException(nameof(edgeEqualityComparer));

#if SUPPORTS_TYPE_FULL_FEATURES
            _reorder = typeof(IUndirectedEdge<TVertex>).IsAssignableFrom(typeof(TEdge))
#else
            _reorder = typeof(IUndirectedEdge<TVertex>).GetTypeInfo().IsAssignableFrom(typeof(TEdge).GetTypeInfo())
#endif
                ? (ReorderVertices)((TVertex source, TVertex target, out TVertex orderedSource, out TVertex orderedTarget) =>
                {
                    if (Comparer<TVertex>.Default.Compare(source, target) > 0)
                    {
                        orderedSource = target;
                        orderedTarget = source;
                    }
                    else
                    {
                        orderedSource = source;
                        orderedTarget = target;
                    }
                })
                : (TVertex source, TVertex target, out TVertex orderedSource, out TVertex orderedTarget) =>
                {
                    orderedSource = source;
                    orderedTarget = target;
                };
        }

        private delegate void ReorderVertices(
            [NotNull] TVertex source,
            [NotNull] TVertex target,
            [NotNull] out TVertex orderedSource,
            [NotNull] out TVertex orderedTarget);

        [NotNull]
        private readonly ReorderVertices _reorder;

        /// <inheritdoc />
        public EdgeEqualityComparer<TVertex> EdgeEqualityComparer { get; }

        /// <summary>
        /// Gets or sets the edge capacity.
        /// </summary>
        public int EdgeCapacity { get; set; } = -1;

        #region IGraph<TVertex,TEdge>

        /// <inheritdoc />
        public bool IsDirected => false;

        /// <inheritdoc />
        public bool AllowParallelEdges { get; }

        #endregion

        #region IVertexSet<TVertex>

        /// <inheritdoc />
        public bool IsVerticesEmpty => _adjacentEdges.Count == 0;

        /// <inheritdoc />
        public int VertexCount => _adjacentEdges.Count;

        /// <inheritdoc />
        public IEnumerable<TVertex> Vertices => _adjacentEdges.Keys;

        /// <inheritdoc />
        public bool ContainsVertex(TVertex vertex)
        {
            if (vertex == null)
                throw new ArgumentNullException(nameof(vertex));

            return _adjacentEdges.ContainsKey(vertex);
        }

        #endregion

        #region IEdgeSet<TVertex,TEdge>

        /// <inheritdoc />
        public bool IsEdgesEmpty => EdgeCount == 0;

        /// <inheritdoc />
        public int EdgeCount { get; private set; }

        [NotNull, ItemNotNull]
        private IList<TEdge> _edges = new List<TEdge>();

        /// <inheritdoc />
        public IEnumerable<TEdge> Edges => _edges.AsEnumerable();

        /// <summary> Optional Id for this Graph </summary>
        public string Id { get; set; }

        /// <inheritdoc />
        public override string ToString() => GetType().Name + ": " + Id;

        /// <inheritdoc />
        public bool ContainsEdge(TEdge edge)
        {
            if (edge == null)
                throw new ArgumentNullException(nameof(edge));

            return _adjacentEdges.TryGetValue(edge.Source, out IEdgeList<TEdge> adjacentEdges)
                   && adjacentEdges.Any(adjacentEdge => EqualityComparer<TEdge>.Default.Equals(adjacentEdge, edge));
        }

        /// <inheritdoc />
        public bool ContainsEdge(TVertex source, TVertex target)
        {
            return TryGetEdge(source, target, out _);
        }

        [Pure]
        private bool ContainsEdgeBetweenVertices([NotNull, ItemNotNull] IEnumerable<TEdge> edges, [NotNull] TEdge edge)
        {
            Debug.Assert(edges != null);
            Debug.Assert(edge != null);

            TVertex source = edge.Source;
            TVertex target = edge.Target;
            return edges.Any(e => EdgeEqualityComparer(e, source, target));
        }

        #endregion

        #region IUndirectedGraph<TVertex,TEdge>

        /// <inheritdoc />
        public IEnumerable<TEdge> AdjacentEdges(TVertex vertex)
        {
            if (vertex == null)
                throw new ArgumentNullException(nameof(vertex));

            if (_adjacentEdges.TryGetValue(vertex, out IEdgeList<TEdge> edges))
                return edges.AsEnumerable();

            return null;
        }

        /// <inheritdoc />
        public int? AdjacentDegree(TVertex vertex)
        {
            if (vertex == null)
                throw new ArgumentNullException(nameof(vertex));

            if (_adjacentEdges.TryGetValue(vertex, out IEdgeList<TEdge> edges))
                return edges.Sum(edge => edge.IsSelfEdge(AreVerticesEqual) ? 2 : 1);    // Self edge count twice

            return null;
        }

        /// <inheritdoc />
        public TEdge AdjacentEdge(TVertex vertex, int index)
        {
            if (vertex == null)
                throw new ArgumentNullException(nameof(vertex));

            if (_adjacentEdges.TryGetValue(vertex, out IEdgeList<TEdge> adjacentEdges))
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

            _reorder(source, target, out source, out target);

            if (_adjacentEdges.TryGetValue(source, out IEdgeList<TEdge> adjacentEdges))
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

        #region IMutableGraph<TVertex,TEdge>

        /// <summary>
        /// Trims excess storage allocated for edges.
        /// </summary>
        public void TrimEdgeExcess()
        {
            foreach (IEdgeList<TEdge> edges in _adjacentEdges.Values)
            {
                edges.TrimExcess();
            }
        }

        /// <inheritdoc />
        public void Clear()
        {
            IList<TEdge> edges = _edges;
            _edges = new List<TEdge>();
            IVertexEdgeDictionary<TVertex, TEdge> adjacentEdges = _adjacentEdges;
            _adjacentEdges = new VertexEdgeDictionary<TVertex, TEdge>();
            EdgeCount = 0;

            NotifyEdgesRemoved(edges);
            NotifyVerticesRemoved(adjacentEdges.Keys);
            edges.Clear();
            adjacentEdges.Clear();
        }

        #endregion

        #region IMutableUndirected<TVertex,TEdge>

        /// <inheritdoc />
        public event VertexAction<TVertex> VertexAdded;

        /// <summary>
        /// Called on each added vertex.
        /// </summary>
        /// <param name="vertex">Added vertex.</param>
        protected virtual void OnVertexAdded([NotNull] TVertex vertex)
        {
            Debug.Assert(vertex != null);

            VertexAdded?.Invoke(vertex);
        }

        /// <inheritdoc />
        public bool AddVertex(TVertex vertex)
        {
            if (ContainsVertex(vertex))
                return false;

            _adjacentEdges.Add(
                vertex,
                EdgeCapacity > 0
                    ? new EdgeList<TEdge>(EdgeCapacity)
                    : new EdgeList<TEdge>());

            OnVertexAdded(vertex);

            return true;
        }

        /// <inheritdoc />
        public int AddVertexRange(IEnumerable<TVertex> vertices)
        {
            if (vertices is null)
                throw new ArgumentNullException(nameof(vertices));
            TVertex[] verticesArray = vertices.ToArray();
            if (verticesArray.Any(v => v == null))
                throw new ArgumentNullException(nameof(vertices), "At least one vertex is null.");

            return verticesArray.Count(AddVertex);
        }

        [Pure]
        [NotNull]
        private IEdgeList<TEdge> AddAndReturnEdges([NotNull] TVertex vertex)
        {
            Debug.Assert(vertex != null);

            if (!_adjacentEdges.TryGetValue(vertex, out IEdgeList<TEdge> edges))
            {
                _adjacentEdges[vertex] = edges = EdgeCapacity > 0
                    ? new EdgeList<TEdge>(EdgeCapacity)
                    : new EdgeList<TEdge>();

                OnVertexAdded(vertex);
            }

            return edges;
        }

        /// <inheritdoc />
        public event VertexAction<TVertex> VertexRemoved;

        /// <summary>
        /// Called for each removed vertex.
        /// </summary>
        /// <param name="vertex">Removed vertex.</param>
        protected virtual void OnVertexRemoved([NotNull] TVertex vertex)
        {
            Debug.Assert(vertex != null);

            VertexRemoved?.Invoke(vertex);
        }

#if SUPPORTS_AGGRESSIVE_INLINING
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        private void NotifyVerticesRemoved([NotNull, ItemNotNull] IEnumerable<TVertex> vertices)
        {
            if (VertexRemoved != null) // Lazily notify
            {
                foreach (TVertex vertex in vertices)
                {
                    OnVertexRemoved(vertex);
                }
            }
        }

        /// <inheritdoc />
        public bool RemoveVertex(TVertex vertex)
        {
            ClearAdjacentEdges(vertex);

            bool removed = _adjacentEdges.Remove(vertex);
            if (removed)
            {
                OnVertexRemoved(vertex);
            }

            return removed;
        }

        /// <inheritdoc />
        public int RemoveVertexIf(Func<TVertex, bool> predicate)
        {
            if (predicate is null)
                throw new ArgumentNullException(nameof(predicate));

            var verticesToRemove = new VertexList<TVertex>();
            verticesToRemove.AddRange(Vertices.Where(predicate));

            foreach (TVertex vertex in verticesToRemove)
            {
                ClearAdjacentEdges(vertex);
                _adjacentEdges.Remove(vertex);
            }

            NotifyVerticesRemoved(verticesToRemove);

            return verticesToRemove.Count;
        }

        #endregion

        #region IMutableIncidenceGraph<TVertex,TEdge>

        /// <inheritdoc />
        public int RemoveAdjacentEdgeIf(TVertex vertex, Func<TEdge, bool> predicate)
        {
            if (predicate is null)
                throw new ArgumentNullException(nameof(predicate));

            if (_adjacentEdges.TryGetValue(vertex, out IEdgeList<TEdge> adjacentEdges))
            {
                var edgesToRemove = new VertexList<TEdge>();
                edgesToRemove.AddRange(adjacentEdges.Where(predicate));
                return RemoveEdgesInternal(edgesToRemove);
            }

            return 0;
        }

        /// <inheritdoc />
        public void ClearAdjacentEdges(TVertex vertex)
        {
            if (vertex == null)
                throw new ArgumentNullException(nameof(vertex));

            if (_adjacentEdges.TryGetValue(vertex, out IEdgeList<TEdge> adjacentEdges))
            {
                IEdgeList<TEdge> edgesToRemove = adjacentEdges.Clone();
                adjacentEdges.Clear();
                EdgeCount -= edgesToRemove.Count;
                Debug.Assert(EdgeCount >= 0);

                foreach (TEdge edge in edgesToRemove)
                {
                    if (_adjacentEdges.TryGetValue(edge.Target, out adjacentEdges))
                    {
                        adjacentEdges.Remove(edge);
                    }

                    if (_adjacentEdges.TryGetValue(edge.Source, out adjacentEdges))
                    {
                        adjacentEdges.Remove(edge);
                    }

                    _edges.Remove(edge);
                }

                NotifyEdgesRemoved(edgesToRemove);
            }
        }

        /// <summary>
        /// Clears edges of the given <paramref name="vertex"/>.
        /// </summary>
        /// <param name="vertex">The vertex.</param>
        /// <exception cref="T:System.ArgumentNullException"><paramref name="vertex"/> is <see langword="null"/>.</exception>
        public void ClearEdges([NotNull] TVertex vertex)
        {
            ClearAdjacentEdges(vertex);
        }

        #endregion

        #region IMutableEdgeListGraph<TVertex,TEdge>

        /// <inheritdoc />
        public bool AddVerticesAndEdge(TEdge edge)
        {
            if (edge == null)
                throw new ArgumentNullException(nameof(edge));

            IEdgeList<TEdge> sourceEdges = AddAndReturnEdges(edge.Source);
            IEdgeList<TEdge> targetEdges = AddAndReturnEdges(edge.Target);

            if (!AllowParallelEdges && ContainsEdgeBetweenVertices(sourceEdges, edge))
                return false;

            _edges.Add(edge);
            sourceEdges.Add(edge);
            if (!edge.IsSelfEdge(AreVerticesEqual))
            {
                targetEdges.Add(edge);
            }

            ++EdgeCount;
            OnEdgeAdded(edge);

            return true;
        }

        /// <inheritdoc />
        public int AddVerticesAndEdgeRange(IEnumerable<TEdge> edges)
        {
            if (edges is null)
                throw new ArgumentNullException(nameof(edges));
            TEdge[] edgesArray = edges.ToArray();
            if (edgesArray.Any(e => e == null))
                throw new ArgumentNullException(nameof(edges), "At least one edge is null.");

            return edgesArray.Count(AddVerticesAndEdge);
        }

        /// <inheritdoc />
        public bool AddEdge(TEdge edge)
        {
            if (edge == null)
                throw new ArgumentNullException(nameof(edge));

            if (!_adjacentEdges.TryGetValue(edge.Source, out IEdgeList<TEdge> sourceEdges))
                return false;
            if (!_adjacentEdges.TryGetValue(edge.Target, out IEdgeList<TEdge> targetEdges))
                return false;

            if (!AllowParallelEdges && ContainsEdgeBetweenVertices(sourceEdges, edge))
                return false;

            _edges.Add(edge);
            sourceEdges.Add(edge);
            if (!edge.IsSelfEdge(AreVerticesEqual))
            {
                targetEdges.Add(edge);
            }

            ++EdgeCount;
            OnEdgeAdded(edge);

            return true;
        }

        /// <inheritdoc />
        public int AddEdgeRange(IEnumerable<TEdge> edges)
        {
            if (edges is null)
                throw new ArgumentNullException(nameof(edges));
            TEdge[] edgesArray = edges.ToArray();
            if (edgesArray.Any(e => e == null))
                throw new ArgumentNullException(nameof(edges), "At least one edge is null.");

            return edgesArray.Count(AddEdge);
        }

        /// <inheritdoc />
        public event EdgeAction<TVertex, TEdge> EdgeAdded;

        /// <summary>
        /// Called on each added edge.
        /// </summary>
        /// <param name="edge">Added edge.</param>
        protected virtual void OnEdgeAdded([NotNull] TEdge edge)
        {
            Debug.Assert(edge != null);

            EdgeAdded?.Invoke(edge);
        }

#if SUPPORTS_AGGRESSIVE_INLINING
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        private void NotifyEdgesRemoved([NotNull, ItemNotNull] ICollection<TEdge> edges)
        {
            Debug.Assert(edges != null);

            if (EdgeRemoved != null) // Lazily notify
            {
                foreach (TEdge edge in edges)
                {
                    OnEdgeRemoved(edge);
                }
            }
        }

        /// <inheritdoc />
        public bool RemoveEdge(TEdge edge)
        {
            if (edge == null)
                throw new ArgumentNullException(nameof(edge));

            if (_adjacentEdges.TryGetValue(edge.Source, out IEdgeList<TEdge> adjacentEdges)
                && adjacentEdges.Remove(edge))
            {
                _edges.Remove(edge);

                if (!edge.IsSelfEdge(AreVerticesEqual))
                {
                    _adjacentEdges[edge.Target].Remove(edge);
                }
                --EdgeCount;
                Debug.Assert(EdgeCount >= 0);

                OnEdgeRemoved(edge);
                return true;
            }

            return false;
        }

        /// <inheritdoc />
        public event EdgeAction<TVertex, TEdge> EdgeRemoved;

        /// <summary>
        /// Called on each removed edge.
        /// </summary>
        /// <param name="edge">Removed edge.</param>
        protected virtual void OnEdgeRemoved([NotNull] TEdge edge)
        {
            Debug.Assert(edge != null);

            EdgeRemoved?.Invoke(edge);
        }

        /// <inheritdoc />
        public int RemoveEdgeIf(Func<TEdge, bool> predicate)
        {
            if (predicate is null)
                throw new ArgumentNullException(nameof(predicate));

            return RemoveEdgesInternal(
                Edges.Where(predicate).ToArray());
        }

#if SUPPORTS_AGGRESSIVE_INLINING
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        private int RemoveEdgesInternal([NotNull, ItemNotNull] ICollection<TEdge> edgesToRemove)
        {
            foreach (TEdge edge in edgesToRemove)
            {
                if (_adjacentEdges.TryGetValue(edge.Source, out IEdgeList<TEdge> adjacentEdges)
                    && adjacentEdges.Remove(edge))
                {
                    _edges.Remove(edge);

                    if (!edge.IsSelfEdge(AreVerticesEqual))
                    {
                        _adjacentEdges[edge.Target].Remove(edge);
                    }
                }
            }

            EdgeCount -= edgesToRemove.Count;
            Debug.Assert(EdgeCount >= 0);
            NotifyEdgesRemoved(edgesToRemove);

            return edgesToRemove.Count;
        }

        /// <summary>
        /// Removes the given set of edges.
        /// </summary>
        /// <param name="edges">Edges to remove.</param>
        /// <returns>The number of removed edges.</returns>
        /// <exception cref="T:System.ArgumentNullException">
        /// <paramref name="edges"/> is <see langword="null"/> or at least one of them is <see langword="null"/>.
        /// </exception>
        public int RemoveEdges([NotNull, ItemNotNull] IEnumerable<TEdge> edges)
        {
            if (edges is null)
                throw new ArgumentNullException(nameof(edges));
            TEdge[] edgesArray = edges.ToArray();
            if (edgesArray.Any(e => e == null))
                throw new ArgumentNullException(nameof(edges), "At least one edge is null.");

            return edgesArray.Count(RemoveEdge);
        }

        #endregion

#if SUPPORTS_SERIALIZATION && NETSTANDARD2_0
        #region ISerializable

        /// <summary>
        /// Constructor used during runtime serialization.
        /// </summary>
        protected UndirectedGraph(SerializationInfo info, StreamingContext context)
            : this((bool)info.GetValue("AllowParallelEdges", typeof(bool)))
        {
            EdgeCapacity = (int)info.GetValue("EdgeCapacity", typeof(int));
            _adjacentEdges = (IVertexEdgeDictionary<TVertex, TEdge>)info.GetValue(
                "AdjacentEdges",
                typeof(IVertexEdgeDictionary<TVertex, TEdge>));
            _edges = (IList<TEdge>)info.GetValue("Edges", typeof(IList<TEdge>));
            EdgeCount = _edges.Count;
        }

        /// <inheritdoc />
        [SecurityPermission(SecurityAction.LinkDemand, Flags = SecurityPermissionFlag.SerializationFormatter)]
        void ISerializable.GetObjectData(SerializationInfo info, StreamingContext context)
        {
            GetObjectData(info, context);
        }

        /// <summary>
        /// Gets the data to serialize.
        /// </summary>
        [SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
        protected virtual void GetObjectData([NotNull] SerializationInfo info, StreamingContext context)
        {
            info.AddValue("AllowParallelEdges", AllowParallelEdges);
            info.AddValue("EdgeCapacity", EdgeCapacity);
            info.AddValue("AdjacentEdges", _adjacentEdges);
            info.AddValue("Edges", _edges);
        }

        #endregion
#endif

        #region ICloneable

        private UndirectedGraph(
            [NotNull, ItemNotNull] IList<TEdge> edges,
            [NotNull] IVertexEdgeDictionary<TVertex, TEdge> adjacentEdges,
            [NotNull] EdgeEqualityComparer<TVertex> edgeEqualityComparer,
            int edgeCapacity,
            bool allowParallelEdges)
            : this(allowParallelEdges, edgeEqualityComparer)
        {
            Debug.Assert(edges != null);
            Debug.Assert(adjacentEdges != null);

            _edges = edges;
            _adjacentEdges = adjacentEdges;
            EdgeCount = edges.Count;
            EdgeCapacity = edgeCapacity;
        }

        /// <summary>Clones this graph.</summary>
        [Pure]
        [NotNull]
        public UndirectedGraph<TVertex, TEdge> Clone()
        {
            return new UndirectedGraph<TVertex, TEdge>(
                _edges.ToList(),
                _adjacentEdges.Clone(),
                EdgeEqualityComparer,
                EdgeCapacity,
                AllowParallelEdges);
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