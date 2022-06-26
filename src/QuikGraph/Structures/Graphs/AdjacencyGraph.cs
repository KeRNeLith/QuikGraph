using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
#if SUPPORTS_AGGRESSIVE_INLINING
using System.Runtime.CompilerServices;
#endif
using JetBrains.Annotations;
using QuikGraph.Collections;

namespace QuikGraph
{
    /// <summary>
    /// Mutable directed graph data structure.
    /// </summary>
    /// <remarks>
    /// It is efficient for sparse graph representation
    /// where out-edge need to be enumerated only.
    /// </remarks>
    /// <typeparam name="TVertex">Vertex type.</typeparam>
    /// <typeparam name="TEdge">Edge type</typeparam>
    [DebuggerDisplay("VertexCount = {" + nameof(VertexCount) + ("}, EdgeCount = {" + nameof(EdgeCount) + "}"))]
    public class AdjacencyGraph<TVertex, TEdge> : IEdgeListAndIncidenceGraph<TVertex, TEdge>, IMutableVertexAndEdgeListGraph<TVertex, TEdge>
#if SUPPORTS_CLONEABLE
        , ICloneable
#endif
        where TEdge : IEdge<TVertex>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AdjacencyGraph{TVertex,TEdge}"/> class.
        /// </summary>
        public AdjacencyGraph()
            : this(true)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="AdjacencyGraph{TVertex,TEdge}"/> class.
        /// </summary>
        /// <param name="allowParallelEdges">Indicates if parallel edges are allowed.</param>
        public AdjacencyGraph(bool allowParallelEdges)
            : this(allowParallelEdges, -1)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="AdjacencyGraph{TVertex,TEdge}"/> class.
        /// </summary>
        /// <param name="allowParallelEdges">Indicates if parallel edges are allowed.</param>
        /// <param name="capacity">Vertex capacity.</param>
        public AdjacencyGraph(bool allowParallelEdges, int capacity)
            : this(allowParallelEdges, capacity, 0)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="AdjacencyGraph{TVertex,TEdge}"/> class.
        /// </summary>
        /// <param name="allowParallelEdges">Indicates if parallel edges are allowed.</param>
        /// <param name="vertexCapacity">Vertex capacity.</param>
        /// <param name="edgeCapacity">Edge capacity.</param>
        public AdjacencyGraph(bool allowParallelEdges, int vertexCapacity, int edgeCapacity)
        {
            AllowParallelEdges = allowParallelEdges;
            _vertexOutEdges = vertexCapacity > -1
                ? new VertexEdgeDictionary<TVertex, TEdge>(vertexCapacity)
                : new VertexEdgeDictionary<TVertex, TEdge>();
            EdgeCapacity = edgeCapacity;
        }

        internal AdjacencyGraph(
            bool allowParallelEdges,
            int edgeCount,
            int edgeCapacity,
            [NotNull] VertexEdgeDictionary<TVertex, TEdge> vertexOutEdges)
        {
            Debug.Assert(edgeCount >= 0);
            Debug.Assert(vertexOutEdges != null);

            AllowParallelEdges = allowParallelEdges;
            EdgeCount = edgeCount;
            EdgeCapacity = edgeCapacity;
            _vertexOutEdges = vertexOutEdges;
        }

        /// <summary>
        /// Gets or sets the edge capacity.
        /// </summary>
        public int EdgeCapacity { get; set; }

        /// <summary>
        /// Gets the type of vertices.
        /// </summary>
        [NotNull]
        public Type VertexType => typeof(TVertex);

        /// <summary>
        /// Gets the type of edges.
        /// </summary>
        [NotNull]
        public Type EdgeType => typeof(TEdge);

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
        private VertexEdgeDictionary<TVertex, TEdge> _vertexOutEdges;
        [NotNull]
        internal VertexEdgeDictionary<TVertex, TEdge> VertexOutEdges => _vertexOutEdges; // For serialization

        /// <inheritdoc />
        public virtual IEnumerable<TVertex> Vertices => _vertexOutEdges.Keys.AsEnumerable();

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
        public int EdgeCount { get; private set; }

        /// <inheritdoc />
        public virtual IEnumerable<TEdge> Edges => _vertexOutEdges.Values.SelectMany(edges => edges);

        /// <inheritdoc />
        public bool ContainsEdge(TEdge edge)
        {
            if (edge == null)
                throw new ArgumentNullException(nameof(edge));

            return _vertexOutEdges.TryGetValue(edge.Source, out IEdgeList<TVertex, TEdge> edges)
                   && edges.Contains(edge);
        }

        #endregion

        #region IIncidenceGraph<TVertex,TEdge>

        /// <inheritdoc />
        public bool ContainsEdge(TVertex source, TVertex target)
        {
            if (target == null)
                throw new ArgumentNullException(nameof(target));

            if (TryGetOutEdges(source, out IEnumerable<TEdge> outEdges))
                return outEdges.Any(edge => EqualityComparer<TVertex>.Default.Equals(edge.Target, target));
            return false;
        }

        /// <inheritdoc />
        public bool TryGetEdge(TVertex source, TVertex target, out TEdge edge)
        {
            if (source == null)
                throw new ArgumentNullException(nameof(source));
            if (target == null)
                throw new ArgumentNullException(nameof(target));

            if (_vertexOutEdges.TryGetValue(source, out IEdgeList<TVertex, TEdge> outEdges)
                && outEdges.Count > 0)
            {
                foreach (TEdge outEdge in outEdges.Where(outEdge => EqualityComparer<TVertex>.Default.Equals(outEdge.Target, target)))
                {
                    edge = outEdge;
                    return true;
                }
            }

            edge = default(TEdge);
            return false;
        }

        /// <inheritdoc />
        public virtual bool TryGetEdges(TVertex source, TVertex target, out IEnumerable<TEdge> edges)
        {
            if (source == null)
                throw new ArgumentNullException(nameof(source));
            if (target == null)
                throw new ArgumentNullException(nameof(target));

            if (_vertexOutEdges.TryGetValue(source, out IEdgeList<TVertex, TEdge> outEdges))
            {
                edges = outEdges.Where(edge => EqualityComparer<TVertex>.Default.Equals(edge.Target, target));
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
            return OutDegree(vertex) == 0;
        }

        /// <inheritdoc />
        public int OutDegree(TVertex vertex)
        {
            if (vertex == null)
                throw new ArgumentNullException(nameof(vertex));

            if (_vertexOutEdges.TryGetValue(vertex, out IEdgeList<TVertex, TEdge> outEdges))
                return outEdges.Count;
            throw new VertexNotFoundException();
        }

        /// <inheritdoc />
        public virtual IEnumerable<TEdge> OutEdges(TVertex vertex)
        {
            if (vertex == null)
                throw new ArgumentNullException(nameof(vertex));

            if (_vertexOutEdges.TryGetValue(vertex, out IEdgeList<TVertex, TEdge> outEdges))
                return outEdges.AsEnumerable();
            throw new VertexNotFoundException();
        }

        /// <inheritdoc />
        public virtual bool TryGetOutEdges(TVertex vertex, out IEnumerable<TEdge> edges)
        {
            if (vertex == null)
                throw new ArgumentNullException(nameof(vertex));

            if (_vertexOutEdges.TryGetValue(vertex, out IEdgeList<TVertex, TEdge> outEdges))
            {
                edges = outEdges.AsEnumerable();
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

            if (_vertexOutEdges.TryGetValue(vertex, out IEdgeList<TVertex, TEdge> outEdges))
                return outEdges[index];
            throw new VertexNotFoundException();
        }

        #endregion

        #region IMutableVertexAndEdgeSet<TVertex,TEdge>

        /// <inheritdoc />
        public virtual bool AddVertex(TVertex vertex)
        {
            if (ContainsVertex(vertex))
                return false;

            _vertexOutEdges.Add(
                vertex,
                EdgeCapacity > 0
                    ? new EdgeList<TVertex, TEdge>(EdgeCapacity)
                    : new EdgeList<TVertex, TEdge>());

            OnVertexAdded(vertex);

            return true;
        }

        /// <inheritdoc />
        public virtual int AddVertexRange(IEnumerable<TVertex> vertices)
        {
            if (vertices is null)
                throw new ArgumentNullException(nameof(vertices));
            TVertex[] verticesArray = vertices.ToArray();
            if (verticesArray.Any(v => v == null))
                throw new ArgumentNullException(nameof(vertices), "At least one vertex is null.");

            return verticesArray.Count(AddVertex);
        }

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

#if SUPPORTS_AGGRESSIVE_INLINING
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        private void NotifyVerticesRemoved([NotNull, ItemNotNull] ICollection<TVertex> vertices)
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
        public virtual bool RemoveVertex(TVertex vertex)
        {
            if (!ContainsVertex(vertex))
                return false;

            // Remove out edges
            IEdgeList<TVertex, TEdge> edgesToRemove = _vertexOutEdges[vertex];
            _vertexOutEdges.Remove(vertex);
            EdgeCount -= edgesToRemove.Count;
            Debug.Assert(EdgeCount >= 0);

            // Remove in edges (Run over edges and remove each edge touching the vertex)
            RemoveInEdges(v => EqualityComparer<TVertex>.Default.Equals(v, vertex));

            NotifyEdgesRemoved(edgesToRemove);
            OnVertexRemoved(vertex);

            return true;
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

        /// <inheritdoc />
        public int RemoveVertexIf(VertexPredicate<TVertex> predicate)
        {
            if (predicate is null)
                throw new ArgumentNullException(nameof(predicate));

            var verticesToRemove = new VertexList<TVertex>();
            verticesToRemove.AddRange(Vertices.Where(vertex => predicate(vertex)));

            // Remove out edges
            var verticesEdgesRemoved = new VertexEdgeDictionary<TVertex, TEdge>(verticesToRemove.Count);
            foreach (TVertex vertex in verticesToRemove)
            {
                verticesEdgesRemoved[vertex] = _vertexOutEdges[vertex];
                _vertexOutEdges.Remove(vertex);
            }
            EdgeCount -= verticesEdgesRemoved.Sum(pair => pair.Value.Count);
            Debug.Assert(EdgeCount >= 0);

            // Remove in edges (Run over edges and remove each edge touching vertices to remove)
            RemoveInEdges(v => verticesToRemove.Contains(v, EqualityComparer<TVertex>.Default));

            NotifyEdgesRemoved(verticesEdgesRemoved.Values.SelectMany(edges => edges));
            NotifyVerticesRemoved(verticesToRemove);

            return verticesToRemove.Count;
        }

        /// <inheritdoc />
        public virtual bool AddVerticesAndEdge(TEdge edge)
        {
            if (edge == null)
                throw new ArgumentNullException(nameof(edge));

            AddVertex(edge.Source);
            AddVertex(edge.Target);
            return AddEdgeInternal(edge);
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

        #endregion

        #region IMutableEdgeListGraph<TVertex,TEdge>

        [Pure]
#if SUPPORTS_AGGRESSIVE_INLINING
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        private bool AddEdgeInternal([NotNull] TEdge edge)
        {
            Debug.Assert(edge != null);

            if (!AllowParallelEdges && ContainsEdge(edge.Source, edge.Target))
                return false;

            _vertexOutEdges[edge.Source].Add(edge);
            ++EdgeCount;

            OnEdgeAdded(edge);

            return true;
        }

        /// <inheritdoc />
        public virtual bool AddEdge(TEdge edge)
        {
            if (edge == null)
                throw new ArgumentNullException(nameof(edge));
            if (!ContainsVertex(edge.Source) || !ContainsVertex(edge.Target))
                throw new VertexNotFoundException();

            return AddEdgeInternal(edge);
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
        private void RemoveInEdges([NotNull, InstantHandle] Predicate<TVertex> shouldRemove)
        {
            Debug.Assert(shouldRemove != null);

            // Run over edges and remove each edge touching the vertices to remove
            foreach (IEdgeList<TVertex, TEdge> outEdges in _vertexOutEdges.Select(pair => pair.Value))
            {
                // Collect indexes of edges to remove
                var indexesToRemove = new List<int>();
                var edgesToRemove = new List<TEdge>();
                for (int i = 0; i < outEdges.Count; ++i)
                {
                    TEdge outEdge = outEdges[i];
                    if (shouldRemove(outEdge.Target))
                    {
                        indexesToRemove.Add(i);
                        edgesToRemove.Add(outEdge);
                    }
                }

                // Remove collected edges
                for (int i = indexesToRemove.Count - 1; i >= 0; --i)
                {
                    outEdges.RemoveAt(indexesToRemove[i]);
                }

                EdgeCount -= indexesToRemove.Count;
                Debug.Assert(EdgeCount >= 0);
                NotifyEdgesRemoved(edgesToRemove);
            }
        }

#if SUPPORTS_AGGRESSIVE_INLINING
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        private void NotifyEdgesRemoved([NotNull, ItemNotNull] IEnumerable<TEdge> edges)
        {
            Debug.Assert(edges != null);

            if (EdgeRemoved != null) // Lazily notify
            {
                // Enumeration is only made on safe enumerable
                foreach (TEdge edge in edges)
                {
                    OnEdgeRemoved(edge);
                }
            }
        }

        /// <inheritdoc />
        public virtual bool RemoveEdge(TEdge edge)
        {
            if (edge == null)
                throw new ArgumentNullException(nameof(edge));

            if (_vertexOutEdges.TryGetValue(edge.Source, out IEdgeList<TVertex, TEdge> outEdges)
                && outEdges.Remove(edge))
            {
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

#if SUPPORTS_AGGRESSIVE_INLINING
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        private int RemoveEdgesInternal([NotNull] ICollection<TEdge> edgesToRemove)
        {
            foreach (TEdge edge in edgesToRemove)
            {
                _vertexOutEdges[edge.Source].Remove(edge);
            }

            EdgeCount -= edgesToRemove.Count;
            Debug.Assert(EdgeCount >= 0);
            NotifyEdgesRemoved(edgesToRemove);

            return edgesToRemove.Count;
        }

        /// <inheritdoc />
        public int RemoveEdgeIf(EdgePredicate<TVertex, TEdge> predicate)
        {
            if (predicate is null)
                throw new ArgumentNullException(nameof(predicate));

            var edgesToRemove = new EdgeList<TVertex, TEdge>();
            edgesToRemove.AddRange(Edges.Where(edge => predicate(edge)));

            return RemoveEdgesInternal(edgesToRemove);
        }

        #endregion

        #region IMutableIncidenceGraph<TVertex,TEdge>

        /// <inheritdoc />
        public int RemoveOutEdgeIf(TVertex vertex, EdgePredicate<TVertex, TEdge> predicate)
        {
            if (predicate is null)
                throw new ArgumentNullException(nameof(predicate));

            if (_vertexOutEdges.TryGetValue(vertex, out IEdgeList<TVertex, TEdge> outEdges))
            {
                var edgesToRemove = new EdgeList<TVertex, TEdge>();
                edgesToRemove.AddRange(outEdges.Where(edge => predicate(edge)));
                return RemoveEdgesInternal(edgesToRemove);
            }

            return 0;
        }

        #endregion

        #region IMutableIncidenceGraph<TVertex,TEdge>

        /// <inheritdoc />
        public void ClearOutEdges(TVertex vertex)
        {
            if (vertex == null)
                throw new ArgumentNullException(nameof(vertex));

            if (_vertexOutEdges.TryGetValue(vertex, out IEdgeList<TVertex, TEdge> outEdges))
            {
                _vertexOutEdges[vertex] = new EdgeList<TVertex, TEdge>();
                EdgeCount -= outEdges.Count;
                Debug.Assert(EdgeCount >= 0);
                NotifyEdgesRemoved(outEdges);
                outEdges.Clear();
            }
        }

        /// <summary>
        /// Clears edges of the given <paramref name="vertex"/>.
        /// </summary>
        /// <param name="vertex">The vertex.</param>
        /// <exception cref="T:System.ArgumentNullException"><paramref name="vertex"/> is <see langword="null"/>.</exception>
        public void ClearEdges([NotNull] TVertex vertex)
        {
            ClearOutEdges(vertex);
        }

        /// <inheritdoc />
        public void TrimEdgeExcess()
        {
            foreach (IEdgeList<TVertex, TEdge> edges in _vertexOutEdges.Values)
            {
                edges.TrimExcess();
            }
        }

        #endregion

        #region IMutableGraph<TVertex,TEdge>

        /// <inheritdoc />
        public void Clear()
        {
            VertexEdgeDictionary<TVertex, TEdge> vertexEdges = _vertexOutEdges;
            _vertexOutEdges = new VertexEdgeDictionary<TVertex, TEdge>();
            EdgeCount = 0;

            NotifyEdgesRemoved(vertexEdges.SelectMany(edges => edges.Value).Distinct());
            NotifyVerticesRemoved(vertexEdges.Keys);
            vertexEdges.Clear();
        }

        #endregion

        #region ICloneable

        /// <summary>
        /// Clones this graph.
        /// </summary>
        /// <returns>Cloned graph.</returns>
        [Pure]
        [NotNull]
        public AdjacencyGraph<TVertex, TEdge> Clone()
        {
            return new AdjacencyGraph<TVertex, TEdge>(
                AllowParallelEdges,
                EdgeCount,
                EdgeCapacity,
                _vertexOutEdges.Clone());
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