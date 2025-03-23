using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using JetBrains.Annotations;

namespace QuikGraph.Algorithms.MaximumFlow
{
    /// <inheritdoc cref="CreateReversedEdgeAugmentorAlgorithm{TVertex,TEdge}"/>
    public static class ReversedEdgeAugmentorAlgorithm
    {
        /// <summary> Creates a new ReversedEdgeAugmentorAlgorithm </summary>
        public static ReversedEdgeAugmentorAlgorithm<TVertex, TEdge> CreateReversedEdgeAugmentorAlgorithm<TVertex, TEdge>(
            [NotNull] this IMutableVertexAndEdgeListGraph<TVertex, TEdge> visitedGraph,
            [NotNull] EdgeFactory<TVertex, TEdge> edgeFactory) where TEdge : IEdge<TVertex>
            => new ReversedEdgeAugmentorAlgorithm<TVertex, TEdge>(visitedGraph, edgeFactory);
    }

    /// <summary>
    /// Routines to add and remove auxiliary edges when using <see cref="EdmondsKarpMaximumFlowAlgorithm{TVertex, TEdge}"/> 
    /// or <see cref="MaximumBipartiteMatchingAlgorithm{TVertex,TEdge}.InternalCompute"/>. 
    /// Remember to call <see cref="RemoveReversedEdges()"/> to remove auxiliary edges.
    /// </summary>
    /// <typeparam name="TVertex">Vertex type.</typeparam>
    /// <typeparam name="TEdge">Edge type.</typeparam>
    public sealed class ReversedEdgeAugmentorAlgorithm<TVertex, TEdge> : IDisposable
        where TEdge : IEdge<TVertex>
    {
        /// <summary>
        /// Initializes a new <see cref="ReversedEdgeAugmentorAlgorithm{TVertex,TEdge}"/> class.
        /// </summary>
        /// <param name="visitedGraph">Graph to visit.</param>
        /// <param name="edgeFactory">Edge factory method.</param>
        /// <exception cref="T:System.ArgumentNullException"><paramref name="visitedGraph"/> is <see langword="null"/>.</exception>
        /// <exception cref="T:System.ArgumentNullException"><paramref name="edgeFactory"/> is <see langword="null"/>.</exception>
        public ReversedEdgeAugmentorAlgorithm(
            [NotNull] IMutableVertexAndEdgeListGraph<TVertex, TEdge> visitedGraph,
            [NotNull] EdgeFactory<TVertex, TEdge> edgeFactory)
        {
            VisitedGraph = visitedGraph ?? throw new ArgumentNullException(nameof(visitedGraph));
            EdgeFactory = edgeFactory ?? throw new ArgumentNullException(nameof(edgeFactory));
        }

        /// <summary>
        /// Gets the graph to visit with this algorithm.
        /// </summary>
        [NotNull]
        public IMutableVertexAndEdgeListGraph<TVertex, TEdge> VisitedGraph { get; }

        /// <summary>
        /// Edge factory method.
        /// </summary>
        [NotNull]
        public EdgeFactory<TVertex, TEdge> EdgeFactory { get; }

        [NotNull, ItemNotNull]
        private readonly List<TEdge> _augmentedEdges = new List<TEdge>();

        /// <summary>
        /// Edges added to the initial graph (augmented ones).
        /// </summary>
        [NotNull, ItemNotNull]
        public IEnumerable<TEdge> AugmentedEdges => _augmentedEdges.AsEnumerable();

        /// <summary>
        /// Edges associated to their reversed edges.
        /// </summary>
        [NotNull]
        public IDictionary<TEdge, TEdge> ReversedEdges { get; } = new Dictionary<TEdge, TEdge>();

        /// <summary>
        /// Gets the state augmented or not of the graph (reversed edges added or not).
        /// </summary>
        public bool Augmented { get; private set; }

        /// <summary>
        /// Fired when a reversed edge is added.
        /// </summary>
        public event EdgeAction<TVertex, TEdge> ReversedEdgeAdded;

        private void OnReservedEdgeAdded([NotNull] TEdge edge)
        {
            Debug.Assert(edge != null);

            ReversedEdgeAdded?.Invoke(edge);
        }

        /// <summary>
        /// Finds the reversed edge of the given one.
        /// </summary>
        /// <param name="edge">Edge to find its corresponding reversed one.</param>
        /// <param name="foundReversedEdge">Found reversed edge.</param>
        /// <returns>True if the reversed edge was found, false otherwise.</returns>
        [Pure]
        private bool FindReversedEdge([NotNull] TEdge edge, out TEdge foundReversedEdge)
        {
            Debug.Assert(edge != null);

            IEnumerable<TEdge> reversedEdges = VisitedGraph
                .OutEdges(edge.Target)
                .Where(reversedEdge => VisitedGraph.AreVerticesEqual(reversedEdge.Target, edge.Source));
            foreach (TEdge reversedEdge in reversedEdges)
            {
                foundReversedEdge = reversedEdge;
                return true;
            }

            foundReversedEdge = default(TEdge);
            return false;
        }

        [Pure]
        [NotNull, ItemNotNull]
        private IEnumerable<TEdge> FindEdgesToReverse()
        {
            foreach (TEdge edge in VisitedGraph.Edges)
            {
                // If reversed already found, continue
                if (ReversedEdges.ContainsKey(edge))
                    continue;

                if (FindReversedEdge(edge, out TEdge reversedEdge))
                {
                    // Setup edge
                    ReversedEdges[edge] = reversedEdge;

                    // Setup reversed if needed
                    if (!ReversedEdges.ContainsKey(reversedEdge))
                    {
                        ReversedEdges[reversedEdge] = edge;
                    }

                    continue;
                }

                // This edge has no reverse
                yield return edge;
            }
        }

        private void AddReversedEdges([NotNull, ItemNotNull] IEnumerable<TEdge> notReversedEdges)
        {
            foreach (TEdge edge in notReversedEdges)
            {
                if (ReversedEdges.ContainsKey(edge))
                    continue;

                // Already been added
                if (FindReversedEdge(edge, out TEdge reversedEdge))
                {
                    ReversedEdges[edge] = reversedEdge;
                    continue;
                }

                // Need to create one
                reversedEdge = EdgeFactory(edge.Target, edge.Source);
                if (!VisitedGraph.AddEdge(reversedEdge))
                    throw new InvalidOperationException("Cannot add the reversed edge, this should not arrive...");

                _augmentedEdges.Add(reversedEdge);
                ReversedEdges[edge] = reversedEdge;
                ReversedEdges[reversedEdge] = edge;

                OnReservedEdgeAdded(reversedEdge);
            }
        }

        /// <summary>
        /// Adds auxiliary edges to <see cref="VisitedGraph"/> to store residual flows.
        /// </summary>
        /// <exception cref="T:System.InvalidOperationException">If the graph is already augmented.</exception>
        public void AddReversedEdges()
        {
            if (Augmented)
                throw new InvalidOperationException("Graph already augmented.");

            // Step 1, find edges that need reversing
            IEnumerable<TEdge> notReversedEdges = FindEdgesToReverse();

            // Step 2, go over each not reversed edge, add reverse
            AddReversedEdges(notReversedEdges);

            Augmented = true;
        }

        /// <summary>
        /// Removes reversed edges that were added.
        /// </summary>
        /// <exception cref="T:System.InvalidOperationException">If the graph was not augmented yet.</exception>
        public void RemoveReversedEdges()
        {
            if (!Augmented)
                throw new InvalidOperationException("Graph is not augmented yet.");

            foreach (TEdge edge in _augmentedEdges)
            {
                VisitedGraph.RemoveEdge(edge);
            }

            _augmentedEdges.Clear();
            ReversedEdges.Clear();

            Augmented = false;
        }

        #region IDisposable

        /// <inheritdoc />
        void IDisposable.Dispose()
        {
            if (Augmented)
            {
                RemoveReversedEdges();
            }
        }

        #endregion
    }
}