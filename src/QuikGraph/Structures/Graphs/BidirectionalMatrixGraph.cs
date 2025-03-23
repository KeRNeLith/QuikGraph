using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
#if SUPPORTS_AGGRESSIVE_INLINING
using System.Runtime.CompilerServices;
#endif
using JetBrains.Annotations;

namespace QuikGraph
{
    /// <summary>
    /// Mutable bidirectional graph data structure based on a matrix.
    /// </summary>
    /// <typeparam name="TEdge">Edge type</typeparam>
#if SUPPORTS_SERIALIZATION
    [Serializable]
#endif
    [DebuggerDisplay("VertexCount = {" + nameof(VertexCount) + "}, EdgeCount = {" + nameof(EdgeCount) + "}")]
    public class BidirectionalMatrixGraph<TEdge> : IBidirectionalGraph<int, TEdge>, IMutableEdgeListGraph<int, TEdge>
#if SUPPORTS_CLONEABLE
        , ICloneable
#endif
        where TEdge : IEdge<int>
    {
        /// <inheritdoc />
        public Func<int, int, bool> AreVerticesEqual
        {
            get => areVerticesEqual ?? EqualityComparer<int>.Default.Equals;
            set => areVerticesEqual = value;
        }
        [CanBeNull]
        private Func<int, int, bool> areVerticesEqual;

        /// <summary>
        /// Initializes a new <see cref="BidirectionalMatrixGraph{TEdge}"/> class.
        /// </summary>
        /// <param name="vertexCount">Number of vertices.</param>
        /// <exception cref="T:System.ArgumentException"><paramref name="vertexCount"/> is is negative or equal to 0.</exception>
        public BidirectionalMatrixGraph(int vertexCount)
        {
            if (vertexCount <= 0)
                throw new ArgumentException("Must be positive.", nameof(vertexCount));

            VertexCount = vertexCount;
            EdgeCount = 0;
            _edges = new TEdge[vertexCount, vertexCount];
        }

        #region Helpers

        [Pure]
        private bool IsInGraph(int vertex) => vertex >= 0 && vertex < VertexCount;

        [Pure]
        private bool AreInGraph(int source, int target) => IsInGraph(source) && IsInGraph(target);

        // ReSharper disable once ParameterOnlyUsedForPreconditionCheck.Local
        private void AssertIsInGraph(int vertex)
        {
            if (!IsInGraph(vertex))
                throw new VertexNotFoundException($"Vertex must be in [0, {VertexCount - 1}].");
        }

        private void AssertAreInGraph(int source, int target)
        {
            AssertIsInGraph(source);
            AssertIsInGraph(target);
        }

        #endregion

        #region IGraph<int,TEdge>


        /// <inheritdoc />
        public bool IsDirected => true;

        /// <inheritdoc />
        public bool AllowParallelEdges => false;

        #endregion

        #region IVertexSet<int>

        /// <inheritdoc />
        public bool IsVerticesEmpty => VertexCount == 0;

        /// <inheritdoc />
        public int VertexCount { get; }

        /// <inheritdoc />
        public IEnumerable<int> Vertices => Enumerable.Range(0, VertexCount);

        /// <inheritdoc />
        public bool ContainsVertex(int vertex)
        {
            return vertex >= 0 && vertex < VertexCount;
        }

        #endregion

        #region IEdgeSet<int,TEdge>

        /// <inheritdoc />
        public bool IsEdgesEmpty => EdgeCount == 0;

        /// <inheritdoc />
        public int EdgeCount { get; private set; }

        [NotNull] [ItemCanBeNull] private readonly TEdge[,] _edges;

        /// <inheritdoc />
        public IEnumerable<TEdge> Edges
        {
            get
            {
                for (int i = 0; i < VertexCount; ++i)
                {
                    for (int j = 0; j < VertexCount; ++j)
                    {
                        TEdge edge = _edges[i, j];
                        if (edge != null)
                            yield return edge;
                    }
                }
            }
        }

        /// <inheritdoc />
        public bool ContainsEdge(TEdge edge)
        {
            if (edge == null)
                throw new ArgumentNullException(nameof(edge));
            if (!AreInGraph(edge.Source, edge.Target))
                return false;

            return _edges[edge.Source, edge.Target] != null;
        }

        #endregion

        #region IIncidenceGraph<int,TEdge>

        /// <inheritdoc />
        public bool ContainsEdge(int source, int target)
        {
            if (!AreInGraph(source, target))
                return false;
            return _edges[source, target] != null;
        }

        /// <inheritdoc />
        public bool TryGetEdge(int source, int target, out TEdge edge)
        {
            if (AreInGraph(source, target))
            {
                edge = _edges[source, target];
                return edge != null;
            }

            edge = default(TEdge);
            return false;
        }

        /// <summary> Returns an empty Edge-Set </summary>
        public IEnumerable<TEdge> Empty => Edge.Empty<TEdge>();

        /// <inheritdoc />
        public IEnumerable<TEdge> GetEdges(int source, int target)
        {
            if (!AreInGraph(source, target))
            {
                return Empty;
            }

            var edge = _edges[source, target];
            var edges = edge == null
                ? Enumerable.Empty<TEdge>()
                : new[] { edge };
            return edges;
        }

        #endregion

        #region IImplicitGraph<int,TEdge>

        /// <inheritdoc />
        public int? OutDegree(int vertex)
        {
            //AssertIsInGraph(vertex);
            if (!IsInGraph(vertex))
                return null;

            int count = 0;
            for (int j = 0; j < VertexCount; ++j)
            {
                if (_edges[vertex, j] != null)
                {
                    ++count;
                }
            }

            return count;
        }

        /// <inheritdoc />
        public IEnumerable<TEdge> OutEdges(int vertex)
        {
            //AssertIsInGraph(vertex);
            if (!IsInGraph(vertex))
                return null;

            return _OutEdges();
            IEnumerable<TEdge> _OutEdges()
            {
                for (int j = 0; j < VertexCount; ++j)
                {
                    TEdge edge = _edges[vertex, j];
                    if (edge != null)
                        yield return edge;
                }
            }
        }

        /// <inheritdoc />
        public TEdge OutEdge(int vertex, int index)
        {
            AssertIsInGraph(vertex);

            int count = 0;
            for (int j = 0; j < VertexCount; ++j)
            {
                TEdge edge = _edges[vertex, j];
                if (edge != null)
                {
                    if (count == index)
                        return edge;
                    ++count;
                }
            }

            throw new ArgumentOutOfRangeException(nameof(index));
        }

        #endregion

        #region IBidirectionalGraph<int,TEdge>

        /// <inheritdoc />
        public int? InDegree(int vertex)
        {
            //AssertIsInGraph(vertex);
            if (!IsInGraph(vertex))
            {
                return null;
            }

            int count = 0;
            for (int i = 0; i < VertexCount; ++i)
            {
                if (_edges[i, vertex] != null)
                {
                    ++count;
                }
            }

            return count;
        }

        /// <inheritdoc />
        public IEnumerable<TEdge> InEdges(int vertex)
        {
            if (!IsInGraph(vertex))
            {
                return null;
            }

            return InEdges();

            IEnumerable<TEdge> InEdges() {
                for (int i = 0; i < VertexCount; ++i)
                {
                    TEdge edge = _edges[i, vertex];
                    if (edge != null)
                        yield return edge;
                }
            }
        }

        /// <inheritdoc />
        public TEdge InEdge(int vertex, int index)
        {
            AssertIsInGraph(vertex);

            int count = 0;
            for (int i = 0; i < VertexCount; ++i)
            {
                TEdge edge = _edges[i, vertex];
                if (edge != null)
                {
                    if (count == index)
                        return edge;
                    ++count;
                }
            }

            throw new ArgumentOutOfRangeException(nameof(index));
        }

        /// <inheritdoc />
        public int? Degree(int vertex) => InDegree(vertex) + OutDegree(vertex);

        #endregion

        #region IMutableGraph<int,TEdge>

        /// <inheritdoc />
        public void Clear()
        {
            EdgeCount = 0;
            for (int i = 0; i < VertexCount; ++i)
            {
                for (int j = 0; j < VertexCount; ++j)
                {
                    TEdge edge = _edges[i, j];
                    _edges[i, j] = default(TEdge);

                    if (edge != null)
                    {
                        OnEdgeRemoved(edge);
                    }
                }
            }
        }

        #endregion

        #region IMutableBidirectionalGraph<int,TEdge>

        /// <summary>
        /// Removes in-edges of the given <paramref name="vertex"/> that match
        /// predicate <paramref name="predicate"/>.
        /// </summary>
        /// <param name="vertex">The vertex.</param>
        /// <param name="predicate">Edge predicate.</param>
        /// <returns>Number of edges removed.</returns>
        /// <exception cref="T:System.ArgumentNullException"><paramref name="predicate"/> is <see langword="null"/>.</exception>
        public int RemoveInEdgeIf(int vertex, [NotNull, InstantHandle] Func<TEdge, bool> predicate)
        {
            if (predicate is null)
                throw new ArgumentNullException(nameof(predicate));
            if (!IsInGraph(vertex))
                return 0;

            int count = 0;
            for (int i = 0; i < VertexCount; ++i)
            {
                TEdge edge = _edges[i, vertex];
                if (edge != null && predicate(edge))
                {
                    RemoveEdgeInternal(edge);
                    ++count;
                }
            }

            return count;
        }

#if SUPPORTS_AGGRESSIVE_INLINING
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        private void ClearInEdgesInternal(int vertex)
        {
            for (int i = 0; i < VertexCount; ++i)
            {
                TEdge edge = _edges[i, vertex];
                if (edge != null)
                {
                    RemoveEdgeInternal(edge);
                }
            }
        }

        /// <summary>
        /// Clears the in-edges of the given <paramref name="vertex"/>.
        /// </summary>
        /// <param name="vertex">The vertex.</param>
        public void ClearInEdges(int vertex)
        {
            if (!IsInGraph(vertex))
                return;

            ClearInEdgesInternal(vertex);
        }

        /// <summary>
        /// Clears the in-edges and out-edges of the given <paramref name="vertex"/>.
        /// </summary>
        /// <param name="vertex">The vertex.</param>
        public void ClearEdges(int vertex)
        {
            if (!IsInGraph(vertex))
                return;

            ClearInEdgesInternal(vertex);
            ClearOutEdgesInternal(vertex);
        }

        #endregion

        #region IMutableIncidenceGraph<int,TEdge>

        /// <summary>
        /// Removes all out-edges of the <paramref name="vertex"/>
        /// where the <paramref name="predicate"/> is evaluated to true.
        /// </summary>
        /// <param name="vertex">The vertex.</param>
        /// <param name="predicate">Predicate to remove edges.</param>
        /// <returns>The number of removed edges.</returns>
        /// <exception cref="T:System.ArgumentNullException"><paramref name="predicate"/> is <see langword="null"/>.</exception>
        public int RemoveOutEdgeIf(int vertex, [NotNull, InstantHandle] Func<TEdge, bool> predicate)
        {
            if (predicate is null)
                throw new ArgumentNullException(nameof(predicate));
            if (!IsInGraph(vertex))
                return 0;

            int count = 0;
            for (int j = 0; j < VertexCount; ++j)
            {
                TEdge edge = _edges[vertex, j];
                if (edge != null && predicate(edge))
                {
                    RemoveEdgeInternal(edge);
                    ++count;
                }
            }

            return count;
        }

#if SUPPORTS_AGGRESSIVE_INLINING
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        private void ClearOutEdgesInternal(int vertex)
        {
            for (int j = 0; j < VertexCount; ++j)
            {
                TEdge edge = _edges[vertex, j];
                if (edge != null)
                {
                    RemoveEdgeInternal(edge);
                }
            }
        }

        /// <summary>
        /// Clears the out-edges of the given <paramref name="vertex"/>
        /// </summary>
        /// <param name="vertex">The vertex.</param>
        public void ClearOutEdges(int vertex)
        {
            if (!IsInGraph(vertex))
                return;

            ClearOutEdgesInternal(vertex);
        }

        #endregion

        #region IMutableEdgeListGraph<int,TEdge>

        /// <inheritdoc />
        /// <exception cref="ParallelEdgeNotAllowedException"><paramref name="edge"/> is already present in graph.</exception>
        public bool AddEdge(TEdge edge)
        {
            if (edge == null)
                throw new ArgumentNullException(nameof(edge));
            AssertAreInGraph(edge.Source, edge.Target);

            if (_edges[edge.Source, edge.Target] == null)
            {
                _edges[edge.Source, edge.Target] = edge;
                ++EdgeCount;
                OnEdgeAdded(edge);
                return true;
            }

            throw new ParallelEdgeNotAllowedException();
        }

        /// <inheritdoc />
        public int AddEdgeRange(params TEdge[] edges) => AddEdgeRange(edges.AsEnumerable());

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
        public event EdgeAction<int, TEdge> EdgeAdded;

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
        private void RemoveEdgeInternal([NotNull] TEdge edge)
        {
            Debug.Assert(edge != null);
            Debug.Assert(_edges[edge.Source, edge.Target] != null);

            _edges[edge.Source, edge.Target] = default(TEdge);
            --EdgeCount;
            Debug.Assert(EdgeCount >= 0);
            OnEdgeRemoved(edge);
        }

        /// <inheritdoc />
        public bool RemoveEdge(TEdge edge)
        {
            if (edge == null)
                throw new ArgumentNullException(nameof(edge));
            if (!AreInGraph(edge.Source, edge.Target))
                return false;
            TEdge edgeToRemove = _edges[edge.Source, edge.Target];
            if (edgeToRemove == null)
                return false;

            RemoveEdgeInternal(edgeToRemove);

            return true;
        }

        /// <inheritdoc />
        public event EdgeAction<int, TEdge> EdgeRemoved;

        /// <summary>
        /// Called on each removed edge.
        /// </summary>
        /// <param name="edge">Removed edge.</param>
        protected virtual void OnEdgeRemoved([NotNull] TEdge edge)
        {
            Debug.Assert(edge != null);

            EdgeRemoved?.Invoke(edge);
        }

        /// <summary>
        /// <see cref="RemoveEdgeIf"/> is not implemented for this kind of graph.
        /// </summary>
        /// <exception cref="T:System.NotSupportedException">This method is not supported.</exception>
        public int RemoveEdgeIf(Func<TEdge, bool> predicate)
        {
            throw new NotSupportedException();
        }

        #endregion

        #region ICloneable

        private BidirectionalMatrixGraph(
            int vertexCount,
            int edgeCount,
            [NotNull] TEdge[,] edges)
        {
            Debug.Assert(vertexCount > 0);
            Debug.Assert(edgeCount >= 0);
            Debug.Assert(edges != null);
            Debug.Assert(vertexCount == edges.GetLength(0));
            Debug.Assert(vertexCount == edges.GetLength(1));

            VertexCount = vertexCount;
            EdgeCount = edgeCount;
            _edges = edges;
        }

        /// <summary>
        /// Clones this graph.
        /// </summary>
        /// <returns>Cloned graph.</returns>
        [Pure]
        [NotNull]
        public BidirectionalMatrixGraph<TEdge> Clone()
        {
            return new BidirectionalMatrixGraph<TEdge>(
                VertexCount,
                EdgeCount,
                (TEdge[,])_edges.Clone());
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