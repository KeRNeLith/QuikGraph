using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using JetBrains.Annotations;
using QuikGraph.Algorithms.Observers;
using QuikGraph.Collections;

namespace QuikGraph.Algorithms.ShortestPath
{
    /// <inheritdoc cref="CreateYenShortestPathsAlgorithm"/>
    public static class YenShortestPathsAlgorithm
    {
        /// <summary>Initializes a new <see cref="YenShortestPathsAlgorithm{TVertex}"/> class.</summary>
        public static YenShortestPathsAlgorithm<TVertex> CreateYenShortestPathsAlgorithm<TVertex>(
            [NotNull] this AdjacencyGraph<TVertex, EquatableTaggedEdge<TVertex, double>> graph,
            [NotNull] TVertex source,
            [NotNull] TVertex target,
            int k,
            [CanBeNull] Func<EquatableTaggedEdge<TVertex, double>, double> edgeWeights = null,
            [CanBeNull] Func<IEnumerable<SortedPath<TVertex>>, IEnumerable<SortedPath<TVertex>>> filter = null)
            => new YenShortestPathsAlgorithm<TVertex>(graph, source, target, k, edgeWeights, filter);
    }

    /// <summary> single-source K-shortest loopless paths algorithm for graphs with non negative edge cost.</summary>
    public class YenShortestPathsAlgorithm<TVertex>
    {
        private readonly TVertex _sourceVertex;
        private readonly TVertex _targetVertex;

        [NotNull]
        private readonly Func<EquatableTaggedEdge<TVertex, double>, double> _weights;

        [NotNull]
        private readonly Func<IEnumerable<SortedPath<TVertex>>, IEnumerable<SortedPath<TVertex>>> _filter;

        // Limit for the amount of paths
        private readonly int _k;

        [NotNull]
        private readonly IMutableVertexAndEdgeListGraph<TVertex, EquatableTaggedEdge<TVertex, double>> _graph;

        /// <summary>Initializes a new <see cref="YenShortestPathsAlgorithm{TVertex}"/> class.</summary>
        /// <remarks>
        /// <see cref="T:System.Double"/> for tag type (edge) which comes from Dijkstra’s algorithm, which is used to get one shortest path.
        /// </remarks>
        /// <param name="graph">Graph to visit.</param>
        /// <param name="source">Source vertex.</param>
        /// <param name="target">Target vertex.</param>
        /// <param name="k">Maximum number of path to search.</param>
        /// <param name="edgeWeights">Optional function that computes the weight for a given edge.</param>
        /// <param name="filter">Optional filter of found paths.</param>
        /// <exception cref="T:System.ArgumentNullException"><paramref name="graph"/> is <see langword="null"/>.</exception>
        /// <exception cref="T:System.ArgumentNullException"><paramref name="source"/> is <see langword="null"/>.</exception>
        /// <exception cref="T:System.ArgumentNullException"><paramref name="target"/> is <see langword="null"/>.</exception>
        /// <exception cref="T:System.ArgumentException"><paramref name="source"/> is not part of <paramref name="graph"/>.</exception>
        /// <exception cref="T:System.ArgumentException"><paramref name="target"/> is not part of <paramref name="graph"/>.</exception>
        /// <exception cref="T:System.ArgumentOutOfRangeException"><paramref name="k"/> is lower than 1.</exception>
        internal YenShortestPathsAlgorithm(
            [NotNull] AdjacencyGraph<TVertex, EquatableTaggedEdge<TVertex, double>> graph,
            [NotNull] TVertex source,
            [NotNull] TVertex target,
            int k,
            [CanBeNull] Func<EquatableTaggedEdge<TVertex, double>, double> edgeWeights = null,
            [CanBeNull] Func<IEnumerable<SortedPath<TVertex>>, IEnumerable<SortedPath<TVertex>>> filter = null)
        {
            if (graph is null)
                throw new ArgumentNullException(nameof(graph));
            if (source == null)
                throw new ArgumentNullException(nameof(source));
            if (target == null)
                throw new ArgumentNullException(nameof(target));
            if (!graph.ContainsVertex(source))
                throw new ArgumentException("Source must be in the graph", nameof(source));
            if (!graph.ContainsVertex(target))
                throw new ArgumentException("Target must be in the graph", nameof(source));
            if (k < 1)
                throw new ArgumentOutOfRangeException(nameof(k), "Value must be positive.");

            _sourceVertex = source;
            _targetVertex = target;
            _k = k;
            _graph = graph.Clone();
            _weights = edgeWeights ?? DefaultGetWeights;
            _filter = filter ?? DefaultFilter;
        }

        [Pure]
        [NotNull]
        private static IEnumerable<SortedPath<TVertex>> DefaultFilter([NotNull] IEnumerable<SortedPath<TVertex>> paths)
        {
            return paths;
        }

        [Pure]
        private static double DefaultGetWeights([NotNull] EquatableTaggedEdge<TVertex, double> edge)
        {
            return edge.Tag;
        }

        [Pure]
        private double GetPathDistance([ItemNotNull] SortedPath<TVertex> edges)
        {
            return edges.Sum(edge => _weights(edge));
        }

        [Pure]
        private SortedPath<TVertex> GetInitialShortestPath()
        {
            // Find the first shortest way from source to target
            SortedPath<TVertex>? shortestPath = GetShortestPathInGraph(_graph, _sourceVertex, _targetVertex);
            // In case of Dijkstra’s algorithm couldn't find any ways
            if (!shortestPath.HasValue)
                throw new NoPathFoundException();

            return shortestPath.Value;
        }

        [Pure]
        [CanBeNull]
        private SortedPath<TVertex>? GetShortestPathInGraph(
            [NotNull] IVertexListGraph<TVertex, EquatableTaggedEdge<TVertex, double>> graph,
            [NotNull] TVertex source,
            [NotNull] TVertex target)
        {
            Debug.Assert(graph != null);
            Debug.Assert(source != null);
            Debug.Assert(target != null);

            // Compute distances between the start vertex and other
            var algorithm = graph.CreateDijkstraShortestPathAlgorithm(_weights);
            var recorder = new VertexPredecessorRecorderObserver<TVertex, EquatableTaggedEdge<TVertex, double>>(graph.AreVerticesEqual);

            using (recorder.Attach(algorithm))
            {
                algorithm.Compute(source);
            }

            // Get shortest path from start (source) vertex to target
            var path = recorder.GetPath(target);
            return path.ToSortedPath();
        }

        [Pure]
        [CanBeNull]
        private static SortedPath<TVertex>? ExtractShortestPathCandidate(
            [NotNull] List<SortedPath<TVertex>> shortestPaths,
            [NotNull] IQueue<SortedPath<TVertex>> shortestPathCandidates)
        {
            bool isNewPath = false;
            SortedPath<TVertex>? newPath = null;
            while (shortestPathCandidates.Count > 0 && !isNewPath)
            {
                newPath = shortestPathCandidates.Dequeue();
                isNewPath = true;
                // Check to see if this candidate path duplicates a previously found path
                if (shortestPaths.Any(path => newPath.Value.Equals(path)))
                {
                    isNewPath = false;
                    newPath = null;
                }
            }

            return newPath;
        }

        [Pure]
        private bool SearchAndAddKthShortestPath(
            SortedPath<TVertex> previousPath,
            [NotNull] List<SortedPath<TVertex>> shortestPaths,
            [NotNull] IQueue<SortedPath<TVertex>> shortestPathCandidates)
        {
            // Iterate over all of the nodes in the (k-1)st shortest path except for the target node
            // For each node (up to) one new candidate path is generated by temporarily modifying
            // the graph and then running Dijkstra's algorithm to find the shortest path between
            // the node and the target in the modified graph
            for (int i = 0; i < previousPath.Count; ++i)
            {
                // Spur node is retrieved from the previous k-shortest path = currently visited vertex in the previous path
                TVertex spurVertex = previousPath.GetVertex(i);

                // The sequence of nodes from the source to the spur node of the previous k-shortest path
                EquatableTaggedEdge<TVertex, double>[] rootPath = previousPath.GetEdges(i);

                foreach (SortedPath<TVertex> path in shortestPaths.Where(path => rootPath.SequenceEqual(path.GetEdges(i))))
                {
                    // Remove the links that are part of the previous shortest paths which share the same root path
                    EquatableTaggedEdge<TVertex, double> edgeToRemove = path.GetEdge(i);
                    _edgesToRestore.Add(edgeToRemove);
                    _graph.RemoveEdge(edgeToRemove);
                }

                var verticesToRestore = new List<TVertex>();
                foreach (TVertex source in rootPath.Select(rootPathEdge => rootPathEdge.Source))
                {
                    if (!_graph.AreVerticesEqual(spurVertex, source))
                    {
                        verticesToRestore.Add(source);

                        _graph.EdgeRemoved += OnGraphEdgeRemoved;
                        _graph.RemoveVertex(source);
                        _graph.EdgeRemoved -= OnGraphEdgeRemoved;
                    }
                }

                SortedPath<TVertex>? spurPath = GetShortestPathInGraph(_graph, spurVertex, _targetVertex);
                if (spurPath.HasValue)
                {
                    // Entire path is made up of the root path and spur path
                    var totalPath = new SortedPath<TVertex>(previousPath.GetEdges(i).Concat(spurPath.Value));

                    // Add the potential k-shortest path to the heap
                    if (!shortestPathCandidates.Contains(totalPath))
                    {
                        shortestPathCandidates.Enqueue(totalPath);
                    }
                }

                // Add back the edges and nodes that were removed from the graph
                _graph.AddVertexRange(verticesToRestore);
                _graph.AddEdgeRange(_edgesToRestore);
                _edgesToRestore.Clear();
            }

            // Identify the candidate path with the shortest cost
            SortedPath<TVertex>? newPath = ExtractShortestPathCandidate(shortestPaths, shortestPathCandidates);
            if (newPath is null)
            {
                // This handles the case of there being no spur paths, or no spur paths left.
                // This could happen if the spur paths have already been exhausted (added to A),
                // or there are no spur paths at all - such as when both the source and sink vertices
                // lie along a "dead end".
                return false;
            }

            // Add the best, non-duplicate candidate identified as the k shortest path
            shortestPaths.Add(newPath.Value);
            return true;
        }

        /// <summary>
        /// Runs the algorithm.
        /// </summary>
        /// <returns>Found paths.</returns>
        /// <exception cref="NoPathFoundException">No shortest path was found.</exception>
        [Pure]
        [NotNull]
        public IEnumerable<SortedPath<TVertex>> Execute()
        {
            SortedPath<TVertex> initialPath = GetInitialShortestPath();
            var shortestPaths = new List<SortedPath<TVertex>> { initialPath };

            // Initialize the set to store the potential k-th shortest path
            var shortestPathCandidates = new BinaryQueue<SortedPath<TVertex>, double>(GetPathDistance);

            for (int k = 1; k < _k; ++k)
            {
                SortedPath<TVertex> previousPath = shortestPaths[k - 1];

                if (!SearchAndAddKthShortestPath(previousPath, shortestPaths, shortestPathCandidates))
                    break;
            }

            return _filter(shortestPaths);
        }

        [NotNull, ItemNotNull]
        private readonly List<EquatableTaggedEdge<TVertex, double>> _edgesToRestore =
            new List<EquatableTaggedEdge<TVertex, double>>();

        private void OnGraphEdgeRemoved([NotNull] EquatableTaggedEdge<TVertex, double> edge)
        {
            _edgesToRestore.Add(edge);
        }
    }
}