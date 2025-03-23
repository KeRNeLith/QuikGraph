using System;
using System.Collections;
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
    /// Mutable edge list graph data structure.
    /// </summary>
    /// <remarks>Only mutable by its edges, vertices are not stored but computed on demand.</remarks>
    /// <typeparam name="TVertex">Vertex type.</typeparam>
    /// <typeparam name="TEdge">Edge type</typeparam>
#if SUPPORTS_SERIALIZATION
    [Serializable]
#endif
    [DebuggerDisplay("EdgeCount = {" + nameof(EdgeCount) + "}")]
    public class EdgeListGraph<TVertex, TEdge> : IMutableEdgeListGraph<TVertex, TEdge>
#if SUPPORTS_CLONEABLE
        , ICloneable
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
        /// Initializes a new <see cref="EdgeListGraph{TVertex,TEdge}"/> class.
        /// </summary>
        public EdgeListGraph()
        {
        }

        /// <summary>
        /// Initializes a new <see cref="EdgeListGraph{TVertex,TEdge}"/> class.
        /// </summary>
        /// <param name="isDirected">Indicates if the graph is directed.</param>
        /// <param name="allowParallelEdges">Indicates if parallel edges are allowed.</param>
        public EdgeListGraph(bool isDirected, bool allowParallelEdges)
        {
            IsDirected = isDirected;
            AllowParallelEdges = allowParallelEdges;
        }

        #region IGraph<TVertex,TEdge>

        /// <inheritdoc />
        public bool IsDirected { get; } = true;

        /// <inheritdoc />
        public bool AllowParallelEdges { get; } = true;

        #endregion

        #region IVertexSet<TVertex>

        /// <inheritdoc />
        public bool IsVerticesEmpty => _edges.Count == 0;

        /// <inheritdoc />
        public int VertexCount => GetVertices().Count;

        /// <inheritdoc />
        public IEnumerable<TVertex> Vertices => GetVertices().AsEnumerable();

        [Pure]
        [NotNull, ItemNotNull]
        private HashSet<TVertex> GetVertices()
        {
            var vertices = new HashSet<TVertex>();
            foreach (TEdge edge in Edges)
            {
                vertices.Add(edge.Source);
                vertices.Add(edge.Target);
            }

            return vertices;
        }

        /// <inheritdoc />
        public bool ContainsVertex(TVertex vertex)
        {
            if (vertex == null)
                throw new ArgumentNullException(nameof(vertex));

            return Edges.Any(
                edge => AreVerticesEqual(edge.Source, vertex) || AreVerticesEqual(edge.Target, vertex));
        }

        #endregion

        #region IEdgeSet<TVertex,TEdge>

        [NotNull]
        private EdgeEdgeDictionary<TVertex, TEdge> _edges = new EdgeEdgeDictionary<TVertex, TEdge>();

        /// <inheritdoc />
        public bool IsEdgesEmpty => _edges.Count == 0;

        /// <inheritdoc />
        public int EdgeCount => _edges.Count;

        /// <inheritdoc />
        public IEnumerable<TEdge> Edges => _edges.Keys.AsEnumerable();

        /// <inheritdoc />
        public bool ContainsEdge(TEdge edge)
        {
            if (edge == null)
                throw new ArgumentNullException(nameof(edge));

            return _edges.ContainsKey(edge);
        }

        [Pure]
        private bool ContainsEdge(TVertex source, TVertex target)
        {
            Debug.Assert(source != null);
            Debug.Assert(target != null);

            return _edges.Keys
                .Any(e => IsDirected
                    ? e.SortedVertexEqualityInternal(source, target, AreVerticesEqual)
                    : e.UndirectedVertexEqualityInternal(source, target, AreVerticesEqual));
        }

        #endregion

        #region IMutableEdgeListGraph<TVertex,TEdge>

        /// <summary>
        /// Adds <paramref name="edge"/> and its vertices to this graph.
        /// </summary>
        /// <param name="edge">The edge to add.</param>
        /// <returns>True if the edge was added, false otherwise.</returns>
        /// <exception cref="T:System.ArgumentNullException"><paramref name="edge"/> is <see langword="null"/>.</exception>
        public bool AddVerticesAndEdge([NotNull] TEdge edge) => AddEdge(edge);

        /// <summary> Adds a set of edges (and it's vertices if necessary). </summary>
        /// <param name="edges">Edges to add.</param>
        /// <returns>The number of edges added.</returns>
        /// <exception cref="T:System.ArgumentNullException">
        /// <paramref name="edges"/> is <see langword="null"/> or at least one of them is <see langword="null"/>.
        /// </exception>
        public int AddVerticesAndEdgeRange([NotNull, ItemNotNull] params TEdge[] edges)
            => AddVerticesAndEdgeRange(edges.AsEnumerable());

        /// <summary> Adds a set of edges (and it's vertices if necessary). </summary>
        /// <param name="edges">Edges to add.</param>
        /// <returns>The number of edges added.</returns>
        /// <exception cref="T:System.ArgumentNullException">
        /// <paramref name="edges"/> is <see langword="null"/> or at least one of them is <see langword="null"/>.
        /// </exception>
        public int AddVerticesAndEdgeRange([NotNull, ItemNotNull] IEnumerable<TEdge> edges)
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
            if (AllowParallelEdges)
            {
                if (_edges.ContainsKey(edge))
                    return false;
            }
            else
            {
                if (ContainsEdge(edge.Source, edge.Target))
                    return false;
            }

            _edges.Add(edge, edge);
            OnEdgeAdded(edge);

            return true;
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

            if (_edges.Remove(edge))
            {
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

            var edgesToRemove = new EdgeList<TEdge>();
            edgesToRemove.AddRange(Edges.Where(predicate));

            foreach (TEdge edge in edgesToRemove)
            {
                _edges.Remove(edge);
            }

            NotifyEdgesRemoved(edgesToRemove);

            return edgesToRemove.Count;
        }

        #endregion

        /// <inheritdoc />
        public void Clear()
        {
            EdgeEdgeDictionary<TVertex, TEdge> edges = _edges;
            _edges = new EdgeEdgeDictionary<TVertex, TEdge>();
            
            NotifyEdgesRemoved(edges.Keys);
            edges.Clear();
        }

        #region ICloneable

        private EdgeListGraph(
            bool isDirected,
            bool allowParallelEdges,
            [NotNull] EdgeEdgeDictionary<TVertex, TEdge> edges)
        {
            Debug.Assert(edges != null);

            IsDirected = isDirected;
            AllowParallelEdges = allowParallelEdges;
            _edges = edges;
        }

        /// <summary>
        /// Clones this graph.
        /// </summary>
        /// <returns>Cloned graph.</returns>
        [Pure]
        [NotNull]
        public EdgeListGraph<TVertex, TEdge> Clone()
        {
            return new EdgeListGraph<TVertex, TEdge>(
                IsDirected,
                AllowParallelEdges,
                _edges.Clone());
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