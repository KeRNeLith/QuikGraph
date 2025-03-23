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
    /// Mutable clustered adjacency graph data structure.
    /// </summary>
    /// <typeparam name="TVertex">Vertex type.</typeparam>
    /// <typeparam name="TEdge">Edge type</typeparam>
#if SUPPORTS_SERIALIZATION
    [Serializable]
#endif
    [DebuggerDisplay("VertexCount = {" + nameof(VertexCount) + "}, EdgeCount = {" + nameof(EdgeCount) + "}")]
    public class ClusteredAdjacencyGraph<TVertex, TEdge>
        : IVertexAndEdgeListGraph<TVertex, TEdge>
        , IEdgeListAndIncidenceGraph<TVertex, TEdge>
        , IClusteredGraph
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
        /// Initializes a new <see cref="ClusteredAdjacencyGraph{TVertex,TEdge}"/> class.
        /// </summary>
        /// <param name="wrappedGraph">Graph to wrap.</param>
        /// <exception cref="T:System.ArgumentNullException"><paramref name="wrappedGraph"/> is <see langword="null"/>.</exception>
        public ClusteredAdjacencyGraph([NotNull] AdjacencyGraph<TVertex, TEdge> wrappedGraph)
        {
            Parent = null;
            Wrapped = wrappedGraph ?? throw new ArgumentNullException(nameof(wrappedGraph));
            Collapsed = false;
        }

        /// <summary>
        /// Initializes a new <see cref="ClusteredAdjacencyGraph{TVertex,TEdge}"/> class.
        /// </summary>
        /// <param name="parentGraph">Parent graph.</param>
        /// <exception cref="T:System.ArgumentNullException"><paramref name="parentGraph"/> is <see langword="null"/>.</exception>
        public ClusteredAdjacencyGraph([NotNull] ClusteredAdjacencyGraph<TVertex, TEdge> parentGraph)
        {
            Parent = parentGraph ?? throw new ArgumentNullException(nameof(parentGraph));
            Wrapped = new AdjacencyGraph<TVertex, TEdge>(parentGraph.AllowParallelEdges);
        }

        /// <summary>
        /// Parent graph.
        /// </summary>
        [CanBeNull]
        public ClusteredAdjacencyGraph<TVertex, TEdge> Parent { get; }

        /// <summary>
        /// Wrapped graph.
        /// </summary>
        [NotNull]
        protected AdjacencyGraph<TVertex, TEdge> Wrapped { get; }

        /// <summary>
        /// Gets or sets the edge capacity.
        /// </summary>
        public int EdgeCapacity
        {
            get => Wrapped.EdgeCapacity;
            set => Wrapped.EdgeCapacity = value;
        }

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
        public bool IsDirected => Wrapped.IsDirected;

        /// <inheritdoc />
        public bool AllowParallelEdges => Wrapped.AllowParallelEdges;

        #endregion

        #region IClusteredGraph

        /// <inheritdoc />
        public bool Collapsed { get; set; }

        [NotNull, ItemNotNull]
        private readonly List<IClusteredGraph> _clusters = new List<IClusteredGraph>();

        /// <inheritdoc />
        public IEnumerable Clusters => _clusters.AsEnumerable();

        /// <inheritdoc />
        public int ClustersCount => _clusters.Count;

        /// <summary>
        /// Adds a new cluster.
        /// </summary>
        /// <returns>The added cluster.</returns>
        public ClusteredAdjacencyGraph<TVertex, TEdge> AddCluster()
        {
            var cluster = new ClusteredAdjacencyGraph<TVertex, TEdge>(this);
            _clusters.Add(cluster);
            return cluster;
        }

        /// <inheritdoc />
        IClusteredGraph IClusteredGraph.AddCluster()
        {
            return AddCluster();
        }

        /// <inheritdoc />
        public void RemoveCluster(IClusteredGraph graph)
        {
            if (graph is null)
                throw new ArgumentNullException(nameof(graph));

            _clusters.Remove(graph);
        }

        #endregion

        #region IVertexSet<TVertex>

        /// <inheritdoc />
        public bool IsVerticesEmpty => Wrapped.IsVerticesEmpty;

        /// <inheritdoc />
        public int VertexCount => Wrapped.VertexCount;

        /// <inheritdoc />
        public virtual IEnumerable<TVertex> Vertices => Wrapped.Vertices;

        /// <inheritdoc />
        public bool ContainsVertex(TVertex vertex)
        {
            return Wrapped.ContainsVertex(vertex);
        }

        #endregion

        #region IEdgeSet<TVertex,TEdge>

        /// <inheritdoc />
        public bool IsEdgesEmpty => Wrapped.IsEdgesEmpty;

        /// <inheritdoc />
        public int EdgeCount => Wrapped.EdgeCount;

        /// <inheritdoc />
        public virtual IEnumerable<TEdge> Edges => Wrapped.Edges;

        /// <inheritdoc />
        public bool ContainsEdge(TEdge edge) => Wrapped.ContainsEdge(edge);

        #endregion

        #region IIncidenceGraph<TVertex,TEdge>

        /// <inheritdoc />
        public bool ContainsEdge(TVertex source, TVertex target) => Wrapped.ContainsEdge(source, target);

        /// <inheritdoc />
        public bool TryGetEdge(TVertex source, TVertex target, out TEdge edge) => Wrapped.TryGetEdge(source, target, out edge);

        /// <inheritdoc />
        public virtual IEnumerable<TEdge> GetEdges(TVertex source, TVertex target) => Wrapped.GetEdges(source, target);

        #endregion

        #region IImplicitGraph<TVertex,TEdge> 

        /// <inheritdoc />
        public int? OutDegree(TVertex vertex) => Wrapped.OutDegree(vertex);

        /// <inheritdoc />
        public virtual IEnumerable<TEdge> OutEdges(TVertex vertex) => Wrapped.OutEdges(vertex);

        /// <inheritdoc />
        public TEdge OutEdge(TVertex vertex, int index) => Wrapped.OutEdge(vertex, index);

        #endregion

        /// <summary> Adds a vertex to this graph. </summary>
        /// <param name="vertex">Vertex to add.</param>
        /// <returns>True if the vertex was added, false otherwise.</returns>
        /// <exception cref="T:System.ArgumentNullException"><paramref name="vertex"/> is <see langword="null"/>.</exception>
        public virtual bool AddVertex([NotNull] TVertex vertex)
        {
            if (vertex == null)
                throw new ArgumentNullException(nameof(vertex));

            if (!(Parent is null || Parent.ContainsVertex(vertex)))
            {
                Parent.AddVertex(vertex);
                return Wrapped.AddVertex(vertex);
            }

            return Wrapped.AddVertex(vertex);
        }

        /// <summary> Adds the <paramref name="vertices"/> to this graph. </summary>
        /// <returns>The number of vertex added.</returns>
        public virtual int AddVertexRange([NotNull, ItemNotNull] params TVertex[] vertices) => AddVertexRange(vertices.AsEnumerable());

        /// <summary> Adds the <paramref name="vertices"/> to this graph. </summary>
        /// <returns>The number of vertex added.</returns>
        public virtual int AddVertexRange([NotNull, ItemNotNull] IEnumerable<TVertex> vertices)
        {
            if (vertices is null)
                throw new ArgumentNullException(nameof(vertices));
            TVertex[] verticesArray = vertices.ToArray();
            if (verticesArray.Any(v => v == null))
                throw new ArgumentNullException(nameof(vertices), "At least one vertex is null.");

            return verticesArray.Count(AddVertex);
        }

        /// <summary>
        /// Removes the given vertex from clusters.
        /// </summary>
        /// <param name="vertex">Vertex to remove.</param>
        private void RemoveChildVertex([NotNull] TVertex vertex)
        {
            Debug.Assert(vertex != null);

            foreach (ClusteredAdjacencyGraph<TVertex, TEdge> cluster in Clusters)
            {
                if (cluster.ContainsVertex(vertex))
                {
                    cluster.Wrapped.RemoveVertex(vertex);
                    cluster.RemoveChildVertex(vertex);
                }
            }
        }

#if SUPPORTS_AGGRESSIVE_INLINING
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        private void RemoveVertexInternal([NotNull] TVertex vertex)
        {
            Debug.Assert(vertex != null);

            RemoveChildVertex(vertex);
            Wrapped.RemoveVertex(vertex);
            Parent?.RemoveVertex(vertex);
        }

        /// <summary>
        /// Removes the given vertex from this graph.
        /// </summary>
        /// <param name="vertex">Vertex to remove.</param>
        /// <returns>True if the vertex was removed, false otherwise.</returns>
        /// <exception cref="T:System.ArgumentNullException"><paramref name="vertex"/> is <see langword="null"/>.</exception>
        public virtual bool RemoveVertex([NotNull] TVertex vertex)
        {
            if (vertex == null)
                throw new ArgumentNullException(nameof(vertex));

            if (!Wrapped.ContainsVertex(vertex))
                return false;

            RemoveVertexInternal(vertex);

            return true;
        }

        /// <summary>
        /// Removes all vertices matching the given <paramref name="predicate"/>.
        /// </summary>
        /// <param name="predicate">Predicate to check on each vertex.</param>
        /// <returns>The number of vertex removed.</returns>
        /// <exception cref="T:System.ArgumentNullException"><paramref name="predicate"/> is <see langword="null"/>.</exception>
        public int RemoveVertexIf([NotNull, InstantHandle] Func<TVertex, bool> predicate)
        {
            if (predicate is null)
                throw new ArgumentNullException(nameof(predicate));

            var verticesToRemove = new VertexList<TVertex>();
            verticesToRemove.AddRange(Vertices.Where(predicate));

            foreach (TVertex vertex in verticesToRemove)
            {
                RemoveVertexInternal(vertex);
            }

            return verticesToRemove.Count;
        }

        /// <summary>
        /// Adds <paramref name="edge"/> and its vertices to this graph.
        /// </summary>
        /// <param name="edge">The edge to add.</param>
        /// <returns>True if the edge was added, false otherwise.</returns>
        /// <exception cref="T:System.ArgumentNullException"><paramref name="edge"/> is <see langword="null"/>.</exception>
        public virtual bool AddVerticesAndEdge([NotNull] TEdge edge)
        {
            if (edge == null)
                throw new ArgumentNullException(nameof(edge));

            AddVertex(edge.Source);
            AddVertex(edge.Target);
            return AddEdge(edge);
        }

        /// <summary>
        /// Adds a set of edges (and it's vertices if necessary).
        /// </summary>
        /// <param name="edges">Edges to add.</param>
        /// <returns>The number of edges added.</returns>
        /// <exception cref="T:System.ArgumentNullException">
        /// <paramref name="edges"/> is <see langword="null"/> or at least one of them is <see langword="null"/>.
        /// </exception>
        public int AddVerticesAndEdgeRange([NotNull, ItemNotNull] params TEdge[] edges)
            => AddVerticesAndEdgeRange(edges.AsEnumerable());

        /// <summary>
        /// Adds a set of edges (and it's vertices if necessary).
        /// </summary>
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

        /// <summary>
        /// Adds the <paramref name="edge"/> to this graph.
        /// </summary>
        /// <param name="edge">An edge.</param>
        /// <returns>True if the edge was added, false otherwise.</returns>
        /// <exception cref="T:System.ArgumentNullException"><paramref name="edge"/> is <see langword="null"/>.</exception>
        public virtual bool AddEdge([NotNull] TEdge edge)
        {
            if (edge == null)
                throw new ArgumentNullException(nameof(edge));

            if (Parent != null && !Parent.ContainsEdge(edge))
            {
                Parent.AddEdge(edge);
            }
            return Wrapped.AddEdge(edge);
        }

        /// <summary> Adds a set of edges to this graph. </summary>
        /// <param name="edges">Edges to add.</param>
        /// <returns>The number of edges successfully added to this graph.</returns>
        /// <exception cref="T:System.ArgumentNullException">
        /// <paramref name="edges"/> is <see langword="null"/> or at least one of them is <see langword="null"/>.
        /// </exception>
        public int AddEdgeRange([NotNull, ItemNotNull] params TEdge[] edges) => AddEdgeRange(edges.AsEnumerable());

        /// <summary> Adds a set of edges to this graph. </summary>
        /// <param name="edges">Edges to add.</param>
        /// <returns>The number of edges successfully added to this graph.</returns>
        /// <exception cref="T:System.ArgumentNullException">
        /// <paramref name="edges"/> is <see langword="null"/> or at least one of them is <see langword="null"/>.
        /// </exception>
        public int AddEdgeRange([NotNull, ItemNotNull] IEnumerable<TEdge> edges)
        {
            if (edges is null)
                throw new ArgumentNullException(nameof(edges));
            TEdge[] edgesArray = edges.ToArray();
            if (edgesArray.Any(e => e == null))
                throw new ArgumentNullException(nameof(edges), "At least one edge is null.");

            return edgesArray.Count(AddEdge);
        }

        private void RemoveChildEdge([NotNull] TEdge edge)
        {
            Debug.Assert(edge != null);

            foreach (ClusteredAdjacencyGraph<TVertex, TEdge> cluster in Clusters)
            {
                if (cluster.ContainsEdge(edge))
                {
                    cluster.Wrapped.RemoveEdge(edge);
                    cluster.RemoveChildEdge(edge);
                }
            }
        }

#if SUPPORTS_AGGRESSIVE_INLINING
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        private void RemoveEdgeInternal([NotNull] TEdge edge)
        {
            Debug.Assert(edge != null);

            RemoveChildEdge(edge);
            Wrapped.RemoveEdge(edge);
            Parent?.RemoveEdge(edge);
        }

        /// <summary>
        /// Removes the <paramref name="edge"/> from this graph.
        /// </summary>
        /// <param name="edge">Edge to remove.</param>
        /// <returns>True if the <paramref name="edge"/> was successfully removed, false otherwise.</returns>
        /// <exception cref="T:System.ArgumentNullException"><paramref name="edge"/> is <see langword="null"/>.</exception>
        public virtual bool RemoveEdge([NotNull] TEdge edge)
        {
            if (edge == null)
                throw new ArgumentNullException(nameof(edge));

            if (!Wrapped.ContainsEdge(edge))
                return false;

            RemoveEdgeInternal(edge);

            return true;
        }

        /// <summary>
        /// Removes all edges that match the given <paramref name="predicate"/>.
        /// </summary>
        /// <param name="predicate">Predicate to check if an edge should be removed.</param>
        /// <returns>The number of edges removed.</returns>
        /// <exception cref="T:System.ArgumentNullException"><paramref name="predicate"/> is <see langword="null"/>.</exception>
        public int RemoveEdgeIf([NotNull, InstantHandle] Func<TEdge, bool> predicate)
        {
            if (predicate is null)
                throw new ArgumentNullException(nameof(predicate));

            var edgesToRemove = new EdgeList<TEdge>();
            edgesToRemove.AddRange(Edges.Where(predicate));

            foreach (TEdge edge in edgesToRemove)
            {
                RemoveEdgeInternal(edge);
            }

            return edgesToRemove.Count;
        }

        /// <summary>
        /// Removes all out-edges of the <paramref name="vertex"/>
        /// where the <paramref name="predicate"/> is evaluated to true.
        /// </summary>
        /// <param name="vertex">The vertex.</param>
        /// <param name="predicate">Predicate to remove edges.</param>
        /// <returns>The number of removed edges.</returns>
        /// <exception cref="T:System.ArgumentNullException"><paramref name="vertex"/> is <see langword="null"/>.</exception>
        /// <exception cref="T:System.ArgumentNullException"><paramref name="predicate"/> is <see langword="null"/>.</exception>
        public int RemoveOutEdgeIf([NotNull] TVertex vertex, [NotNull, InstantHandle] Func<TEdge, bool> predicate)
        {
            if (vertex == null)
                throw new ArgumentNullException(nameof(vertex));
            if (predicate is null)
                throw new ArgumentNullException(nameof(predicate));

            int edgeToRemoveCount = Wrapped.RemoveOutEdgeIf(vertex, predicate);
            Parent?.RemoveOutEdgeIf(vertex, predicate);

            return edgeToRemoveCount;
        }

        /// <summary>
        /// Clears the out-edges of the given <paramref name="vertex"/>
        /// </summary>
        /// <param name="vertex">The vertex.</param>
        /// <exception cref="T:System.ArgumentNullException"><paramref name="vertex"/> is <see langword="null"/>.</exception>
        public void ClearOutEdges([NotNull] TVertex vertex)
        {
            if (vertex == null)
                throw new ArgumentNullException(nameof(vertex));

            Wrapped.ClearOutEdges(vertex);
        }

        /// <summary>
        /// Clears the vertex and edges.
        /// </summary>
        public void Clear()
        {
            Wrapped.Clear();
            _clusters.Clear();
        }
    }
}