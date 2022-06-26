using System;
using System.Collections.Generic;
using System.Diagnostics;
#if SUPPORTS_AGGRESSIVE_INLINING
using System.Runtime.CompilerServices;
#endif
using JetBrains.Annotations;
using QuikGraph.Algorithms.Services;

namespace QuikGraph.Algorithms
{
    /// <summary>
    /// Base class for all graph algorithm requiring a starting vertex (root).
    /// </summary>
    /// <remarks>Requires a starting vertex (root).</remarks>
    /// <typeparam name="TVertex">Vertex type.</typeparam>
    /// <typeparam name="TGraph">Graph type.</typeparam>
    public abstract class RootedAlgorithmBase<TVertex, TGraph> : AlgorithmBase<TGraph>
        where TGraph : IImplicitVertexSet<TVertex>
    {
        [CanBeNull]
        private TVertex _root;

        private bool _hasRootVertex;

        /// <summary>
        /// Initializes a new instance of the <see cref="RootedAlgorithmBase{TVertex,TGraph}"/> class.
        /// </summary>
        /// <param name="host">Host to use if set, otherwise use this reference.</param>
        /// <param name="visitedGraph">Graph to visit.</param>
        /// <exception cref="T:System.ArgumentNullException"><paramref name="visitedGraph"/> is <see langword="null"/>.</exception>
        protected RootedAlgorithmBase(
            [CanBeNull] IAlgorithmComponent host,
            [NotNull] TGraph visitedGraph)
            : base(host, visitedGraph)
        {
        }

        /// <summary>
        /// Tries to get the root vertex if set.
        /// </summary>
        /// <param name="root">Root vertex if set, otherwise <see langword="null"/>.</param>
        /// <returns>True if the root vertex was set, false otherwise.</returns>
        [Pure]
        [ContractAnnotation("=> true, root:notnull;=> false, root:null")]
        public bool TryGetRootVertex(out TVertex root)
        {
            if (_hasRootVertex)
            {
                root = _root;
                return true;
            }

            root = default(TVertex);
            return false;
        }

        /// <summary>
        /// Sets the root vertex.
        /// </summary>
        /// <param name="root">Root vertex.</param>
        /// <exception cref="T:System.ArgumentNullException"><paramref name="root"/> is <see langword="null"/>.</exception>
        public void SetRootVertex([NotNull] TVertex root)
        {
            if (root == null)
                throw new ArgumentNullException(nameof(root));

            bool changed = !_hasRootVertex || !EqualityComparer<TVertex>.Default.Equals(_root, root);
            _root = root;
            _hasRootVertex = true;

            if (changed)
            {
                OnRootVertexChanged(EventArgs.Empty);
            }
        }

        /// <summary>
        /// Clears the root vertex.
        /// </summary>
        public void ClearRootVertex()
        {
            bool hasRoot = _hasRootVertex;
            _root = default(TVertex);
            _hasRootVertex = false;

            if (hasRoot)
            {
                OnRootVertexChanged(EventArgs.Empty);
            }
        }

        /// <summary>
        /// Fired when the root vertex is changed.
        /// </summary>
        public event EventHandler RootVertexChanged;

        /// <summary>
        /// Called on each root vertex change.
        /// </summary>
        /// <param name="args"><see cref="F:EventArgs.Empty"/>.</param>
        protected virtual void OnRootVertexChanged([NotNull] EventArgs args)
        {
            Debug.Assert(args != null);

            RootVertexChanged?.Invoke(this, args);
        }

        /// <summary>
        /// Gets the root vertex if set and checks it is part of the
        /// <see cref="AlgorithmBase{TGraph}.VisitedGraph"/>.
        /// </summary>
        /// <returns>Root vertex.</returns>
        /// <exception cref="T:System.InvalidOperationException">If the root vertex has not been set.</exception>
        /// <exception cref="VertexNotFoundException">
        /// If the set root vertex is not part of the <see cref="AlgorithmBase{TGraph}.VisitedGraph"/>.
        /// </exception>
        [NotNull]
        protected TVertex GetAndAssertRootInGraph()
        {
            if (!TryGetRootVertex(out TVertex root))
                throw new InvalidOperationException("Root vertex not set.");
            AssertRootInGraph(root);
            return root;
        }

        /// <summary>
        /// Asserts that the given <paramref name="root"/> vertex is in the <see cref="AlgorithmBase{TGraph}.VisitedGraph"/>.
        /// </summary>
        /// <param name="root">Vertex to check.</param>
        /// <exception cref="VertexNotFoundException">
        /// If the set root vertex is not part of the <see cref="AlgorithmBase{TGraph}.VisitedGraph"/>.
        /// </exception>
#if SUPPORTS_AGGRESSIVE_INLINING
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        protected void AssertRootInGraph([NotNull] TVertex root)
        {
            if (!VisitedGraph.ContainsVertex(root))
                throw new VertexNotFoundException("Root vertex is not part of the graph.");
        }

        /// <summary>
        /// Runs the algorithm with the given <paramref name="root"/> vertex.
        /// </summary>
        /// <param name="root">Root vertex.</param>
        /// <exception cref="T:System.ArgumentNullException"><paramref name="root"/> is <see langword="null"/>.</exception>
        /// <exception cref="T:System.ArgumentException"><paramref name="root"/> is not part of <see cref="AlgorithmBase{TGraph}.VisitedGraph"/>.</exception>
        /// <exception cref="T:System.InvalidOperationException">Something went wrong when running the algorithm.</exception>
        public virtual void Compute([NotNull] TVertex root)
        {
            SetRootVertex(root);
            if (!VisitedGraph.ContainsVertex(root))
                throw new ArgumentException("Graph does not contain the provided root vertex.", nameof(root));

            Compute();
        }
    }
}