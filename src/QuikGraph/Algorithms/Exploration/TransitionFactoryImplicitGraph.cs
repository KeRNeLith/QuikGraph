#if SUPPORTS_CLONEABLE
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using JetBrains.Annotations;
using QuikGraph.Collections;

namespace QuikGraph.Algorithms.Exploration
{
    /// <summary>
    /// Implementation for a graph data structure that support growth
    /// by transitions made by out edges of its vertices.
    /// </summary>
    /// <typeparam name="TVertex">Vertex type.</typeparam>
    /// <typeparam name="TEdge">Edge type</typeparam>
#if SUPPORTS_SERIALIZATION
    [Serializable]
#endif
    public sealed class TransitionFactoryImplicitGraph<TVertex, TEdge> : IImplicitGraph<TVertex, TEdge>
        where TVertex : ICloneable
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

        [NotNull]
        private readonly VertexEdgeDictionary<TVertex, TEdge> _verticesEdgesCache =
            new VertexEdgeDictionary<TVertex, TEdge>();

        [NotNull]
        private readonly Dictionary<TVertex, HashSet<ITransitionFactory<TVertex, TEdge>>> _verticesNotProcessedCache =
            new Dictionary<TVertex, HashSet<ITransitionFactory<TVertex, TEdge>>>();

        /// <summary>
        /// Transitions factories.
        /// </summary>
        [NotNull, ItemNotNull]
        private readonly List<ITransitionFactory<TVertex, TEdge>> _transitionFactories =
            new List<ITransitionFactory<TVertex, TEdge>>();

        /// <summary>
        /// Clears the <see cref="_verticesEdgesCache"/> but keep a trace of explored vertices
        /// in the <see cref="_verticesNotProcessedCache"/>.
        /// </summary>
        private void MoveMainCacheToNotProcessedVertices()
        {
            foreach (TVertex vertex in _verticesEdgesCache.Keys)
            {
                Debug.Assert(!_verticesNotProcessedCache.ContainsKey(vertex));
                _verticesNotProcessedCache.Add(vertex, new HashSet<ITransitionFactory<TVertex, TEdge>>());
            }

            _verticesEdgesCache.Clear();    // Cache no longer valid
        }

        /// <summary>
        /// Adds a new <see cref="ITransitionFactory{TVertex,TEdge}"/> to this graph.
        /// </summary>
        /// <param name="transitionFactory">Transition factory to add.</param>
        /// <exception cref="T:System.ArgumentNullException"><paramref name="transitionFactory"/> is <see langword="null"/>.</exception>
        public void AddTransitionFactory([NotNull] ITransitionFactory<TVertex, TEdge> transitionFactory)
        {
            if (transitionFactory is null)
                throw new ArgumentNullException(nameof(transitionFactory));

            _transitionFactories.Add(transitionFactory);
            MoveMainCacheToNotProcessedVertices();
        }

        /// <summary>
        /// Adds new <see cref="ITransitionFactory{TVertex,TEdge}"/>s to this graph.
        /// </summary>
        /// <param name="transitionFactories">Transition factories to add.</param>
        /// <exception cref="T:System.ArgumentNullException"><paramref name="transitionFactories"/> is <see langword="null"/>.</exception>
        public void AddTransitionFactories(
            [NotNull, ItemNotNull] IEnumerable<ITransitionFactory<TVertex, TEdge>> transitionFactories)
        {
            if (transitionFactories is null)
                throw new ArgumentNullException(nameof(transitionFactories));

            _transitionFactories.AddRange(transitionFactories);
            MoveMainCacheToNotProcessedVertices();
        }

        /// <summary>
        /// Removes the given <paramref name="transitionFactory"/> from this graph.
        /// </summary>
        /// <param name="transitionFactory">Transition factory to remove.</param>
        public bool RemoveTransitionFactory([CanBeNull] ITransitionFactory<TVertex, TEdge> transitionFactory)
        {
            if (_transitionFactories.Remove(transitionFactory))
            {
                _verticesEdgesCache.Clear();    // Cache no longer valid
                CleanNotProcessedCache();
                return true;
            }

            return false;

            #region Local function

            void CleanNotProcessedCache()
            {
                foreach (KeyValuePair<TVertex, HashSet<ITransitionFactory<TVertex, TEdge>>> pair in _verticesNotProcessedCache.ToArray())
                {
                    if (pair.Value.Count == 0 || pair.Value.Contains(transitionFactory))
                    {
                        _verticesNotProcessedCache.Remove(pair.Key);
                    }
                }
            }

            #endregion
        }

        /// <summary>
        /// Clears all <see cref="ITransitionFactory{TVertex,TEdge}"/> from this graph.
        /// </summary>
        public void ClearTransitionFactories()
        {
            _transitionFactories.Clear();
            _verticesEdgesCache.Clear();    // Cache no longer valid
            _verticesNotProcessedCache.Clear();    // Cache no longer valid
        }

        /// <summary>
        /// Checks if this graph contains the given <paramref name="transitionFactory"/>.
        /// </summary>
        /// <param name="transitionFactory">Transition factory to check.</param>
        [Pure]
        public bool ContainsTransitionFactory([CanBeNull] ITransitionFactory<TVertex, TEdge> transitionFactory)
        {
            return _transitionFactories.Contains(transitionFactory);
        }

        [NotNull]
        private Func<TVertex, bool> _vertexPredicate = vertex => true;

        /// <summary>
        /// Predicate that a vertex must match to be the successor (target) of an edge.
        /// </summary>
        /// <exception cref="T:System.ArgumentNullException">Set value is <see langword="null"/>.</exception>
        [NotNull]
        public Func<TVertex, bool> SuccessorVertexPredicate
        {
            get => _vertexPredicate;
            set
            {
                _vertexPredicate = value ?? throw new ArgumentNullException(nameof(value));
                _verticesEdgesCache.Clear();    // Cache is no longer valid
            }
        }

        [NotNull]
        private Func<TEdge, bool> _edgePredicate = edge => true;

        /// <summary>
        /// Predicate that an edge must match to be the successor of a source vertex.
        /// </summary>
        /// <exception cref="T:System.ArgumentNullException">Set value is <see langword="null"/>.</exception>
        [NotNull]
        public Func<TEdge, bool> SuccessorEdgePredicate
        {
            get => _edgePredicate;
            set
            {
                _edgePredicate = value ?? throw new ArgumentNullException(nameof(value));
                _verticesEdgesCache.Clear();    // Cache is no longer valid
            }
        }

        #region IGraph<TVertex,TEdge>

        /// <inheritdoc />
        public bool IsDirected => true;

        /// <inheritdoc />
        public bool AllowParallelEdges => true;

        #endregion

        /// <inheritdoc />
        public bool ContainsVertex(TVertex vertex)
        {
            if (vertex == null)
                throw new ArgumentNullException(nameof(vertex));

            return _verticesEdgesCache.ContainsKey(vertex)
                || _verticesNotProcessedCache.ContainsKey(vertex);
        }

        #region IImplicitGraph<TVertex,TEdge>

        /// <inheritdoc />
        public int? OutDegree(TVertex vertex) => OutEdges(vertex)?.Count();

        private void AddToNotProcessedCacheIfNecessary(
            [NotNull] TVertex vertex,
            [NotNull] ITransitionFactory<TVertex, TEdge> transitionFactory)
        {
            Debug.Assert(vertex != null);
            Debug.Assert(transitionFactory != null);

            if (!_verticesEdgesCache.ContainsKey(vertex))
            {
                if (_verticesNotProcessedCache.TryGetValue(
                    vertex,
                    out HashSet<ITransitionFactory<TVertex, TEdge>> factories))
                {
                    factories.Add(transitionFactory);
                }
                else
                {
                    _verticesNotProcessedCache.Add(
                        vertex,
                        new HashSet<ITransitionFactory<TVertex, TEdge>> { transitionFactory });
                }
            }
        }

        [Pure]
        [CanBeNull, ItemNotNull]
        private IEdgeList<TEdge> ExploreFactoriesForVertex([NotNull] TVertex vertex)
        {
            Debug.Assert(vertex != null);

            IEdgeList<TEdge> edges = null;
            foreach (ITransitionFactory<TVertex, TEdge> transitionFactory in _transitionFactories)
            {
                if (!transitionFactory.IsValid(vertex))
                    continue;

                if (edges is null)
                {
                    edges = new EdgeList<TEdge>();
                }

                foreach (TEdge edge in transitionFactory.Apply(vertex).Where(edge => SuccessorVertexPredicate(edge.Target)))
                {
                    AddToNotProcessedCacheIfNecessary(edge.Target, transitionFactory);

                    if (SuccessorEdgePredicate(edge))
                    {
                        edges.Add(edge);
                    }
                }
            }

            return edges;
        }

        /// <inheritdoc />
        public IEnumerable<TEdge> OutEdges(TVertex vertex)
        {
            if (vertex == null)
                throw new ArgumentNullException(nameof(vertex));

            bool wasNotProcessed = _verticesNotProcessedCache.Remove(vertex);

            if (_verticesEdgesCache.TryGetValue(vertex, out IEdgeList<TEdge> edgeList))
            {
                return edgeList; //.AsEnumerable();
                //return Empty;
            }

            edgeList = ExploreFactoriesForVertex(vertex);

            if (edgeList is null)
            {
                if (!wasNotProcessed)
                {
                    return null;
                    //return Empty;
                }

                // Vertex has no out edges
                edgeList = new EdgeList<TEdge>();
            }

            _verticesEdgesCache[vertex] = edgeList;

            return edgeList;//.AsEnumerable();
        }

        /// <summary> Returns an empty Edge-Set </summary>
        public IEnumerable<TEdge> Empty => Edge.Empty<TEdge>();

        /// <inheritdoc />
        public TEdge OutEdge(TVertex vertex, int index)
        {
            int i = 0;
            IEnumerable<TEdge> outEdges = OutEdges(vertex);
            if (outEdges is null)
            {
                return default(TEdge);
            }

            foreach (TEdge edge in outEdges)
            {
                if (i++ == index)
                    return edge;
            }

            throw new ArgumentOutOfRangeException(nameof(index));
        }

        #endregion
    }
}
#endif