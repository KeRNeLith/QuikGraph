using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using JetBrains.Annotations;
using QuikGraph.Algorithms.Observers;
using QuikGraph.Algorithms.Services;
using QuikGraph.Algorithms.ShortestPath;
using QuikGraph.Collections;

namespace QuikGraph.Algorithms.RankedShortestPath
{
    /// <inheritdoc cref="CreateHoffmanPavleyRankedShortestPathAlgorithm"/>
    public static class HoffmanPavleyRankedShortestPathAlgorithm
    {
        /// <summary> Creates a new <see cref="HoffmanPavleyRankedShortestPathAlgorithm{TVertex,TEdge}"/> class. </summary>
        public static HoffmanPavleyRankedShortestPathAlgorithm<TVertex, TEdge> CreateHoffmanPavleyRankedShortestPathAlgorithm<TVertex
            , TEdge>([NotNull] this IBidirectionalGraph<TVertex, TEdge> visitedGraph,
            [NotNull] Func<TEdge, double> edgeWeights,
            [CanBeNull] IDistanceRelaxer distanceRelaxer = null,
            [CanBeNull] IAlgorithmComponent host = null) where TEdge : IEdge<TVertex>
            => new HoffmanPavleyRankedShortestPathAlgorithm<TVertex, TEdge>(visitedGraph, edgeWeights, distanceRelaxer, host);
    }

    /// <summary> Hoffman and Pavley K-shortest path algorithm. </summary>
    /// <remarks>
    /// Reference:
    /// Hoffman, W. and Pavley, R. 1959. A Method for the Solution of the Nth Best Path Problem. 
    /// J. ACM 6, 4 (Oct. 1959), 506-514. DOI= http://doi.acm.org/10.1145/320998.321004
    /// </remarks>
    public sealed class HoffmanPavleyRankedShortestPathAlgorithm<TVertex, TEdge>
        : RankedShortestPathAlgorithmBase<TVertex, TEdge, IBidirectionalGraph<TVertex, TEdge>>
        where TEdge : IEdge<TVertex>
    {
        private TVertex _target;
        private bool _hasTargetVertex;

        [NotNull]
        private readonly Func<TEdge, double> _edgeWeights;

        /// <summary>
        /// Initializes a new <see cref="HoffmanPavleyRankedShortestPathAlgorithm{TVertex,TEdge}"/> class.
        /// </summary>
        /// <param name="visitedGraph">Graph to visit.</param>
        /// <param name="edgeWeights">Function that for a given edge provide its weight.</param>
        /// <param name="distanceRelaxer">Distance relaxer.</param>
        /// <param name="host">Host to use if set, otherwise use this reference.</param>
        /// <exception cref="T:System.ArgumentNullException"><paramref name="visitedGraph"/> is <see langword="null"/>.</exception>
        /// <exception cref="T:System.ArgumentNullException"><paramref name="edgeWeights"/> is <see langword="null"/>.</exception>
        /// <exception cref="T:System.ArgumentNullException"><paramref name="distanceRelaxer"/> is <see langword="null"/>.</exception>
        internal HoffmanPavleyRankedShortestPathAlgorithm(
            [NotNull] IBidirectionalGraph<TVertex, TEdge> visitedGraph,
            [NotNull] Func<TEdge, double> edgeWeights,
            [CanBeNull] IDistanceRelaxer distanceRelaxer = null,
            [CanBeNull] IAlgorithmComponent host = null)
            : base(visitedGraph, distanceRelaxer ?? DistanceRelaxers.ShortestDistance, host)
        {
            _edgeWeights = edgeWeights ?? throw new ArgumentNullException(nameof(edgeWeights));
        }

        /// <summary>
        /// Sets the target vertex.
        /// </summary>
        /// <param name="target">Target vertex.</param>
        /// <exception cref="T:System.ArgumentNullException"><paramref name="target"/> is <see langword="null"/>.</exception>
        public void SetTargetVertex([NotNull] TVertex target)
        {
            if (target == null)
                throw new ArgumentNullException(nameof(target));

            _target = target;
            _hasTargetVertex = true;
        }

        /// <summary>
        /// Tries to get the target vertex if set.
        /// </summary>
        /// <param name="target">Target vertex if set, otherwise <see langword="null"/>.</param>
        /// <returns>True if the target vertex was set, false otherwise.</returns>
        [Pure]
        [ContractAnnotation("=> true, target:notnull;=> false, target:null")]
        public bool TryGetTargetVertex(out TVertex target)
        {
            if (_hasTargetVertex)
            {
                target = _target;
                return true;
            }

            target = default(TVertex);
            return false;
        }

        /// <summary>
        /// Runs the algorithm with the given <paramref name="root"/> vertex.
        /// </summary>
        /// <param name="root">Root vertex.</param>
        /// <param name="target">Target vertex.</param>
        /// <exception cref="T:System.ArgumentNullException"><paramref name="root"/> is <see langword="null"/>.</exception>
        /// <exception cref="T:System.ArgumentNullException"><paramref name="target"/> is <see langword="null"/>.</exception>
        /// <exception cref="T:System.ArgumentException"><paramref name="root"/> is not part of <see cref="AlgorithmBase{TGraph}.VisitedGraph"/>.</exception>
        /// <exception cref="T:System.ArgumentException"><paramref name="target"/> is not part of <see cref="AlgorithmBase{TGraph}.VisitedGraph"/>.</exception>
        /// <exception cref="T:System.InvalidOperationException">Something went wrong when running the algorithm.</exception>
        public void Compute([NotNull] TVertex root, [NotNull] TVertex target)
        {
            if (root == null)
                throw new ArgumentNullException(nameof(root));
            SetTargetVertex(target);
            if (!VisitedGraph.ContainsVertex(target))
                throw new ArgumentException("Graph does not contain the provided target vertex.", nameof(target));
            Compute(root);
        }

        #region AlgorithmBase<TGraph>

        /// <inheritdoc />
        protected override void InternalCompute()
        {
            TVertex root = GetAndAssertRootInGraph();
            if (!TryGetTargetVertex(out TVertex target))
                throw new InvalidOperationException("Target vertex not set.");
            if (!VisitedGraph.ContainsVertex(target))
                throw new Exception("Target vertex is not part of the graph.");

            // Start by building the minimum tree starting from the target vertex.
            ComputeMinimumTree(
                target,
                out IDictionary<TVertex, TEdge> successors,
                out IDictionary<TVertex, double> distances);

            ThrowIfCancellationRequested();

            var queue = new FibonacciQueue<DeviationPath, double>(deviationPath => deviationPath.Weight);
            int vertexCount = VisitedGraph.VertexCount;

            // First shortest path
            EnqueueFirstShortestPath(queue, successors, distances, root);

            while (queue.Count > 0
                   && ComputedShortestPathCount < ShortestPathCount)
            {
                ThrowIfCancellationRequested();

                DeviationPath deviation = queue.Dequeue();

                // Turn into path
                var path = new List<TEdge>();
                for (int i = 0; i < deviation.DeviationIndex; ++i)
                    path.Add(deviation.ParentPath[i]);
                path.Add(deviation.DeviationEdge);

                int startEdge = path.Count;
                AppendShortestPath(path, successors, deviation.DeviationEdge.Target);

                Debug.Assert(Math.Abs(deviation.Weight - path.Sum(e => _edgeWeights(e))) < float.Epsilon);
                Debug.Assert(path.Count > 0);

                // Add to list, if has no cycle
                if (!path.Cast<IEdge<TVertex>>().HasCycles(VisitedGraph.AreVerticesEqual))
                    AddComputedShortestPath(path);

                // Append new deviation paths
                if (path.Count < vertexCount)
                {
                    EnqueueDeviationPaths(
                        queue,
                        root,
                        distances,
                        path.ToArray(),
                        startEdge);
                }
            }
        }

        #endregion

        private void EnqueueFirstShortestPath(
            [NotNull] IQueue<DeviationPath> queue,
            [NotNull] IDictionary<TVertex, TEdge> successors,
            [NotNull] IDictionary<TVertex, double> distances,
            [NotNull] TVertex root)
        {
            Debug.Assert(queue != null);
            Debug.Assert(queue.Count == 0);
            Debug.Assert(successors != null);
            Debug.Assert(distances != null);
            Debug.Assert(root != null);

            var path = new List<TEdge>();
            AppendShortestPath(path, successors, root);

            if (path.Count == 0)
                return; // Unreachable vertices

            if (!path.Cast<IEdge<TVertex>>().HasCycles(VisitedGraph.AreVerticesEqual))
                AddComputedShortestPath(path);

            // Create deviation paths
            EnqueueDeviationPaths(
                queue,
                root,
                distances,
                path.ToArray(),
                0);
        }

        private void ComputeMinimumTree(
            [NotNull] TVertex target,
            out IDictionary<TVertex, TEdge> successors,
            out IDictionary<TVertex, double> distances)
        {
            Debug.Assert(target != null);

            var reversedGraph = VisitedGraph.CreateReversedBidirectionalGraph();
            var successorsObserver =
                new VertexPredecessorRecorderObserver<TVertex, SReversedEdge<TVertex, TEdge>>(VisitedGraph.AreVerticesEqual);
            var distancesObserver =
                new VertexDistanceRecorderObserver<TVertex, SReversedEdge<TVertex, TEdge>>(ReversedEdgeWeight);
            var shortestPath =
                reversedGraph.CreateDijkstraShortestPathAlgorithm(
                    ReversedEdgeWeight,
                    DistanceRelaxer,
                    this);

            using (successorsObserver.Attach(shortestPath))
            using (distancesObserver.Attach(shortestPath))
                shortestPath.Compute(target);

            successors = new Dictionary<TVertex, TEdge>();
            foreach (KeyValuePair<TVertex, SReversedEdge<TVertex, TEdge>> pair in successorsObserver.VerticesPredecessors)
            {
                successors.Add(pair.Key, pair.Value.OriginalEdge);
            }

            distances = distancesObserver.Distances;

            double ReversedEdgeWeight(SReversedEdge<TVertex, TEdge> edge) => _edgeWeights(edge.OriginalEdge);
        }

        private void EnqueueDeviationPaths(
            [NotNull] IQueue<DeviationPath> queue,
            [NotNull] TVertex root,
            [NotNull] IDictionary<TVertex, double> distances,
            [NotNull, ItemNotNull] TEdge[] path,
            int startEdge)
        {
            Debug.Assert(queue != null);
            Debug.Assert(root != null);
            Debug.Assert(distances != null);
            Debug.Assert(path != null);
            Debug.Assert(path[0].IsAdjacent(root, VisitedGraph.AreVerticesEqual));
            Debug.Assert(0 <= startEdge && startEdge < path.Length);

            TVertex previousVertex = root;
            double previousWeight = 0;
            var pathVertices = new Dictionary<TVertex, int>(path.Length);
            for (int edgeIndex = 0; edgeIndex < path.Length; ++edgeIndex)
            {
                TEdge edge = path[edgeIndex];
                if (edgeIndex >= startEdge)
                {
                    EnqueueDeviationPaths(
                        queue,
                        distances,
                        path,
                        edgeIndex,
                        previousVertex,
                        previousWeight);
                }

                // Update counter
                previousVertex = edge.Target;
                previousWeight += _edgeWeights(edge);

                // Detection of loops
                if (edgeIndex == 0)
                {
                    pathVertices[edge.Source] = 0;
                }

                // We should really allow only one key
                if (pathVertices.ContainsKey(edge.Target))
                    break;

                pathVertices[edge.Target] = 0;
            }
        }

        private void EnqueueDeviationPaths(
            [NotNull] IQueue<DeviationPath> queue,
            [NotNull] IDictionary<TVertex, double> distances,
            [NotNull, ItemNotNull] TEdge[] path,
            int edgeIndex,
            [NotNull] TVertex previousVertex,
            double previousWeight)
        {
            Debug.Assert(queue != null);
            Debug.Assert(distances != null);
            Debug.Assert(path != null);
            Debug.Assert(previousVertex != null);

            TEdge edge = path[edgeIndex];
            foreach (TEdge deviationEdge in VisitedGraph.OutEdges(previousVertex))
            {
                // Skip self edges and equal edges
                if (EqualityComparer<TEdge>.Default.Equals(deviationEdge, edge) || deviationEdge.IsSelfEdge(VisitedGraph.AreVerticesEqual))
                    continue;

                // Any edge obviously creating a loop
                TVertex target = deviationEdge.Target;
                if (distances.TryGetValue(target, out double distance))
                {
                    double deviationWeight = DistanceRelaxer.Combine(
                        previousWeight,
                        DistanceRelaxer.Combine(
                            _edgeWeights(deviationEdge),
                            distance));

                    var deviation = new DeviationPath(
                        path,
                        edgeIndex,
                        deviationEdge,
                        deviationWeight);

                    queue.Enqueue(deviation);
                }
            }
        }

        private void AppendShortestPath(
            [NotNull, ItemNotNull] ICollection<TEdge> path,
            [NotNull] IDictionary<TVertex, TEdge> successors,
            [NotNull] TVertex startVertex)
        {
            Debug.Assert(path != null);
            Debug.Assert(successors != null);
            Debug.Assert(startVertex != null);

            TVertex current = startVertex;
            while (successors.TryGetValue(current, out TEdge edge))
            {
                path.Add(edge);
                current = edge.Target;
            }

            Debug.Assert(path.Count == 0 || VisitedGraph.AreVerticesEqual(path.ElementAt(path.Count - 1).Target, _target));
        }

        [DebuggerDisplay("Weight = {" + nameof(Weight) + "}, Index = {" + nameof(DeviationIndex) + "}, Edge = {" + nameof(DeviationEdge) + "}")]
        private struct DeviationPath
        {
            [NotNull, ItemNotNull]
            public TEdge[] ParentPath { get; }

            public int DeviationIndex { get; }

            [NotNull]
            public TEdge DeviationEdge { get; }

            public double Weight { get; }

            public DeviationPath(
                [NotNull, ItemNotNull] TEdge[] parentPath,
                int deviationIndex,
                [NotNull] TEdge deviationEdge,
                double weight)
            {
                Debug.Assert(parentPath != null);
                Debug.Assert(0 <= deviationIndex && deviationIndex < parentPath.Length);
                Debug.Assert(deviationEdge != null);
                Debug.Assert(weight >= 0);

                ParentPath = parentPath;
                DeviationIndex = deviationIndex;
                DeviationEdge = deviationEdge;
                Weight = weight;
            }

            /// <inheritdoc />
            public override string ToString()
            {
                return $"{Weight} at {DeviationIndex} ({DeviationEdge})";
            }
        }
    }
}