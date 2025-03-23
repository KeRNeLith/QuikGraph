#if SUPPORTS_CLONEABLE
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using JetBrains.Annotations;
using QuikGraph.Algorithms.Services;

namespace QuikGraph.Algorithms.Exploration
{
    /// <inheritdoc cref="CreateCloneableVertexGraphExplorerAlgorithm"/>
    public static class CloneableVertexGraphExplorerAlgorithm
    {
        /// <summary> Creates a new <see cref="CloneableVertexGraphExplorerAlgorithm{TVertex,TEdge}"/> class. </summary>
        public static CloneableVertexGraphExplorerAlgorithm<TVertex, TEdge>
            CreateCloneableVertexGraphExplorerAlgorithm<TVertex, TEdge>(
            [NotNull] this IMutableVertexAndEdgeSet<TVertex, TEdge> visitedGraph,
            [CanBeNull] IAlgorithmComponent host = null)
                where TVertex : ICloneable
                where TEdge : IEdge<TVertex>
            => new CloneableVertexGraphExplorerAlgorithm<TVertex, TEdge>(visitedGraph, host);

    }
    /// <summary> Explores a graph starting from a given vertex.</summary>
    public sealed class CloneableVertexGraphExplorerAlgorithm<TVertex, TEdge>
        : RootedAlgorithmBase<TVertex, IMutableVertexAndEdgeSet<TVertex, TEdge>>
        , ITreeBuilderAlgorithm<TVertex, TEdge>
        where TVertex : ICloneable
        where TEdge : IEdge<TVertex>
    {
        /// <summary> The processed Graph </summary>
        public IGraph<TVertex, TEdge> VisitededGraph => (IGraph<TVertex, TEdge>)base.VisitedGraph;

        /// <summary> Creates a new <see cref="CloneableVertexGraphExplorerAlgorithm{TVertex,TEdge}"/> class. </summary>
        /// <param name="visitedGraph">Graph to visit.</param>
        /// <param name="host">Host to use if set, otherwise use this reference.</param>
        /// <exception cref="T:System.ArgumentNullException"><paramref name="visitedGraph"/> is <see langword="null"/>.</exception>
        internal CloneableVertexGraphExplorerAlgorithm(
            [NotNull] IMutableVertexAndEdgeSet<TVertex, TEdge> visitedGraph,
            [CanBeNull] IAlgorithmComponent host = null)
            : base(visitedGraph, host)
        {
        }

        /// <summary>
        /// Transitions factories.
        /// </summary>
        [NotNull, ItemNotNull]
        private readonly List<ITransitionFactory<TVertex, TEdge>> _transitionFactories =
            new List<ITransitionFactory<TVertex, TEdge>>();

        [NotNull]
        private Func<TVertex, bool> _addVertexPredicate = vertex => true;

        /// <summary>
        /// Predicate that a vertex must match to be added in the graph.
        /// </summary>
        /// <exception cref="T:System.ArgumentNullException">Set value is <see langword="null"/>.</exception>
        [NotNull]
        public Func<TVertex, bool> AddVertexPredicate
        {
            get => _addVertexPredicate;
            set => _addVertexPredicate = value ?? throw new ArgumentNullException(nameof(value));
        }

        [NotNull]
        private Func<TVertex, bool> _exploreVertexPredicate = vertex => true;

        /// <summary>
        /// Predicate that checks if a given vertex should be explored or ignored.
        /// </summary>
        /// <exception cref="T:System.ArgumentNullException">Set value is <see langword="null"/>.</exception>
        [NotNull]
        public Func<TVertex, bool> ExploreVertexPredicate
        {
            get => _exploreVertexPredicate;
            set => _exploreVertexPredicate = value ?? throw new ArgumentNullException(nameof(value));
        }

        [NotNull]
        private Func<TEdge, bool> _addEdgePredicate = edge => true;

        /// <summary>
        /// Predicate that an edge must match to be added in the graph.
        /// </summary>
        /// <exception cref="T:System.ArgumentNullException">Set value is <see langword="null"/>.</exception>
        [NotNull]
        public Func<TEdge, bool> AddEdgePredicate
        {
            get => _addEdgePredicate;
            set => _addEdgePredicate = value ?? throw new ArgumentNullException(nameof(value));
        }

        [NotNull]
        private Predicate<CloneableVertexGraphExplorerAlgorithm<TVertex, TEdge>> _finishedPredicate =
            new DefaultFinishedPredicate().Test;

        /// <summary>
        /// Predicate that checks if the exploration is finished or not.
        /// </summary>
        /// <exception cref="T:System.ArgumentNullException">Set value is <see langword="null"/>.</exception>
        [NotNull]
        public Predicate<CloneableVertexGraphExplorerAlgorithm<TVertex, TEdge>> FinishedPredicate
        {
            get => _finishedPredicate;
            set => _finishedPredicate = value ?? throw new ArgumentNullException(nameof(value));
        }

        [NotNull, ItemNotNull]
        private readonly Queue<TVertex> _unExploredVertices = new Queue<TVertex>();

        /// <summary>
        /// Gets the enumeration of unexplored vertices.
        /// </summary>
        [NotNull, ItemNotNull]
        public IEnumerable<TVertex> UnExploredVertices => _unExploredVertices.AsEnumerable();

        /// <summary>
        /// Indicates if the algorithm finished successfully or not.
        /// </summary>
        public bool FinishedSuccessfully { get; private set; }

        #region Events

        /// <summary>
        /// Fired when a vertex is discovered.
        /// </summary>
        public event VertexAction<TVertex> DiscoverVertex;

        private void OnVertexDiscovered([NotNull] TVertex vertex)
        {
            Debug.Assert(vertex != null);

            VisitedGraph.AddVertex(vertex);
            _unExploredVertices.Enqueue(vertex);

            DiscoverVertex?.Invoke(vertex);
        }

        /// <summary>
        /// Fired when an edge is encountered.
        /// </summary>
        public event EdgeAction<TVertex, TEdge> TreeEdge;

        private void OnTreeEdge([NotNull] TEdge edge)
        {
            Debug.Assert(edge != null);

            TreeEdge?.Invoke(edge);
        }

        /// <summary>
        /// Fired when a back edge is encountered.
        /// </summary>
        public event EdgeAction<TVertex, TEdge> BackEdge;

        private void OnBackEdge([NotNull] TEdge edge)
        {
            Debug.Assert(edge != null);

            BackEdge?.Invoke(edge);
        }

        /// <summary>
        /// Fired when an edge was skipped from exploration due to failed vertex or edge predicate check.
        /// </summary>
        public event EdgeAction<TVertex, TEdge> EdgeSkipped;

        private void OnEdgeSkipped([NotNull] TEdge edge)
        {
            Debug.Assert(edge != null);

            EdgeSkipped?.Invoke(edge);
        }

        #endregion

        /// <summary>
        /// Adds a new <see cref="ITransitionFactory{TVertex,TEdge}"/> to this algorithm.
        /// </summary>
        /// <param name="transitionFactory">Transition factory to add.</param>
        /// <exception cref="T:System.ArgumentNullException"><paramref name="transitionFactory"/> is <see langword="null"/>.</exception>
        public void AddTransitionFactory([NotNull] ITransitionFactory<TVertex, TEdge> transitionFactory)
        {
            if (transitionFactory is null)
                throw new ArgumentNullException(nameof(transitionFactory));

            _transitionFactories.Add(transitionFactory);
        }

        /// <summary>Adds new <see cref="ITransitionFactory{TVertex,TEdge}"/>s to this algorithm. </summary>
        public void AddTransitionFactories(
            [NotNull, ItemNotNull] params ITransitionFactory<TVertex, TEdge>[] transitionFactories)
        {
            AddTransitionFactories(transitionFactories.AsEnumerable());
        }

        /// <summary>Adds new <see cref="ITransitionFactory{TVertex,TEdge}"/>s to this algorithm. </summary>
        public void AddTransitionFactories(
            [NotNull, ItemNotNull] IEnumerable<ITransitionFactory<TVertex, TEdge>> transitionFactories)
        {
            if (transitionFactories is null)
                throw new ArgumentNullException(nameof(transitionFactories));

            _transitionFactories.AddRange(transitionFactories);
        }

        /// <summary>
        /// Removes the given <paramref name="transitionFactory"/> from this algorithm.
        /// </summary>
        /// <param name="transitionFactory">Transition factory to remove.</param>
        public bool RemoveTransitionFactory([CanBeNull] ITransitionFactory<TVertex, TEdge> transitionFactory)
        {
            return _transitionFactories.Remove(transitionFactory);
        }

        /// <summary>
        /// Clears all <see cref="ITransitionFactory{TVertex,TEdge}"/> from this algorithm.
        /// </summary>
        public void ClearTransitionFactories()
        {
            _transitionFactories.Clear();
        }

        /// <summary>
        /// Checks if this algorithm contains the given <paramref name="transitionFactory"/>.
        /// </summary>
        /// <param name="transitionFactory">Transition factory to check.</param>
        [Pure]
        public bool ContainsTransitionFactory([CanBeNull] ITransitionFactory<TVertex, TEdge> transitionFactory)
        {
            return _transitionFactories.Contains(transitionFactory);
        }

        #region AlgorithmBase<TGraph>

        /// <inheritdoc />
        protected override void InternalCompute()
        {
            if (!TryGetRootVertex(out TVertex root))
                throw new InvalidOperationException("Root vertex not set.");

            VisitedGraph.Clear();
            _unExploredVertices.Clear();
            FinishedSuccessfully = false;

            if (!AddVertexPredicate(root))
                throw new InvalidOperationException($"Starting vertex does not satisfy the {nameof(AddVertexPredicate)}.");
            OnVertexDiscovered(root);

            while (_unExploredVertices.Count > 0)
            {
                // Are we done yet?
                if (!FinishedPredicate(this))
                {
                    FinishedSuccessfully = false;
                    return;
                }

                TVertex current = _unExploredVertices.Dequeue();
                var clone = (TVertex)current.Clone();

                // Let's make sure we want to explore this one
                if (!ExploreVertexPredicate(clone))
                    continue;

                foreach (ITransitionFactory<TVertex, TEdge> transitionFactory in _transitionFactories)
                {
                    GenerateFromTransitionFactory(clone, transitionFactory);
                }
            }

            FinishedSuccessfully = true;
        }

        /// <inheritdoc />
        public override void Compute(TVertex root)
        {
            SetRootVertex(root);
            Compute();
        }

        #endregion

        private void GenerateFromTransitionFactory(
            [NotNull] TVertex current,
            [NotNull] ITransitionFactory<TVertex, TEdge> transitionFactory)
        {
            Debug.Assert(current != null);
            Debug.Assert(transitionFactory != null);

            if (!transitionFactory.IsValid(current))
                return;

            foreach (TEdge transition in transitionFactory.Apply(current))
            {
                if (!AddVertexPredicate(transition.Target)
                    || !AddEdgePredicate(transition))
                {
                    OnEdgeSkipped(transition);
                    continue;
                }

                bool backEdge = VisitedGraph.ContainsVertex(transition.Target);
                if (!backEdge)
                    OnVertexDiscovered(transition.Target);

                VisitedGraph.AddEdge(transition);
                if (backEdge)
                    OnBackEdge(transition);
                else
                    OnTreeEdge(transition);
            }
        }

        /// <summary>
        /// Default implementation of the finished predicate.
        /// </summary>
        public sealed class DefaultFinishedPredicate
        {
            /// <summary>
            /// Initializes a new <see cref="DefaultFinishedPredicate"/> class.
            /// </summary>
            public DefaultFinishedPredicate()
            {
            }

            /// <summary>
            /// Initializes a new <see cref="DefaultFinishedPredicate"/> class.
            /// </summary>
            /// <param name="maxVertexCount">Maximum number of vertices.</param>
            /// <param name="maxEdgeCount">Maximum number of edges.</param>
            public DefaultFinishedPredicate(int maxVertexCount, int maxEdgeCount)
            {
                MaxVertexCount = maxVertexCount;
                MaxEdgeCount = maxEdgeCount;
            }

            /// <summary>
            /// Maximum number of vertices (for exploration).
            /// </summary>
            public int MaxVertexCount { get; set; } = 1000;

            /// <summary>
            /// Maximum number of edges (for exploration).
            /// </summary>
            public int MaxEdgeCount { get; set; } = 1000;

            /// <summary>
            /// Checks if the given <paramref name="algorithm"/> explorer has finished or not.
            /// </summary>
            /// <param name="algorithm">Algorithm explorer to check.</param>
            /// <returns>True if the explorer can continue to explore, false otherwise.</returns>
            /// <exception cref="T:System.ArgumentNullException"><paramref name="algorithm"/> is <see langword="null"/>.</exception>
            [Pure]
            public bool Test([NotNull] CloneableVertexGraphExplorerAlgorithm<TVertex, TEdge> algorithm)
            {
                if (algorithm is null)
                    throw new ArgumentNullException(nameof(algorithm));

                if (algorithm.VisitedGraph.VertexCount > MaxVertexCount)
                    return false;

                if (algorithm.VisitedGraph.EdgeCount > MaxEdgeCount)
                    return false;

                return true;
            }
        }
    }
}
#endif