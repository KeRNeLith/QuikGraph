using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using JetBrains.Annotations;
using QuikGraph.Algorithms.Observers;
using QuikGraph.Collections;

namespace QuikGraph.Algorithms.ShortestPath
{
    /// <summary>
    /// A single-source K-shortest loopless paths algorithm for graphs
    /// with non negative edge cost.
    /// </summary>
    /// <typeparam name="TVertex">Vertex type.</typeparam>
    /// <typeparam name="TEdge">Edge type.</typeparam>
    public class YenShortestPathsAlgorithm<TVertex, TEdge> where TEdge : IEdge<TVertex>
    {
        /// <summary>
        /// Class representing a sorted path.
        /// </summary>
        public struct SortedPath : IEnumerable<TEdge>, IEquatable<SortedPath>
        {
            [NotNull, ItemNotNull]
            private readonly List<TEdge> _edges;

            /// <summary>
            /// Initializes a new instance of the <see cref="SortedPath"/> struct.
            /// </summary>
            public SortedPath([NotNull, ItemNotNull] IEnumerable<TEdge> edges)
            {
                _edges = edges.ToList();
            }

            /// <summary>
            /// Number of edges in this path.
            /// </summary>
            public int Count => _edges.Count;

            [Pure]
            [NotNull]
            internal TVertex GetVertex(int i)
            {
                Debug.Assert(i >= 0 && i < _edges.Count);

                return _edges[i].Source;
            }

            [Pure]
            [NotNull]
            internal TEdge GetEdge(int i)
            {
                Debug.Assert(i >= 0 && i < _edges.Count);

                return _edges[i];
            }

            [Pure]
            [NotNull, ItemNotNull]
            internal TEdge[] GetEdges(int count)
            {
                if (count > _edges.Count)
                {
                    count = _edges.Count;
                }

                Debug.Assert(count >= 0 && count <= _edges.Count);

                return _edges.GetRange(0, count).ToArray();
            }

            /// <inheritdoc />
            public override bool Equals(object obj)
            {
                return obj is SortedPath path && Equals(path);
            }

            /// <inheritdoc />
            public bool Equals(SortedPath other)
            {
                return _edges.SequenceEqual(other._edges);
            }

            /// <inheritdoc />
            public override int GetHashCode()
            {
                return _edges.GetHashCode();
            }

            /// <inheritdoc />
            public IEnumerator<TEdge> GetEnumerator()
            {
                return _edges.GetEnumerator();
            }

            /// <inheritdoc />
            IEnumerator IEnumerable.GetEnumerator()
            {
                return GetEnumerator();
            }
        }

        private readonly TVertex _sourceVertex;
        private readonly TVertex _targetVertex;

        [NotNull]
        private readonly Func<TEdge, double> _weights;

        [NotNull]
        private readonly Func<IEnumerable<SortedPath>, IEnumerable<SortedPath>> _filter;

        // Limit for the amount of paths
        private readonly int _k;

        [NotNull]
        private readonly IMutableVertexAndEdgeListGraph<TVertex, TEdge> _graph;

        /// <summary>
        /// Initializes a new instance of the <see cref="YenShortestPathsAlgorithm{TVertex,TEdge}"/> class.
        /// </summary>
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
        public YenShortestPathsAlgorithm(
            [NotNull] AdjacencyGraph<TVertex, TEdge> graph,
            [NotNull] TVertex source,
            [NotNull] TVertex target,
            int k,
            [CanBeNull] Func<TEdge, double> edgeWeights,
            [CanBeNull] Func<IEnumerable<SortedPath>, IEnumerable<SortedPath>> filter = null)
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
            if (edgeWeights == null)
                throw new ArgumentNullException(nameof(edgeWeights));

            _sourceVertex = source;
            _targetVertex = target;
            _k = k;
            _graph = graph.Clone();
            _weights = edgeWeights;
            _filter = filter ?? DefaultFilter;
        }

        [Pure]
        [NotNull]
        private static IEnumerable<SortedPath> DefaultFilter([NotNull] IEnumerable<SortedPath> paths)
        {
            return paths;
        }

        [Pure]
        private double GetPathDistance([ItemNotNull] SortedPath edges)
        {
            return edges.Sum(edge => _weights(edge));
        }

        [Pure]
        private SortedPath GetInitialShortestPath()
        {
            // Find the first shortest way from source to target
            SortedPath? shortestPath = GetShortestPathInGraph(_graph, _sourceVertex, _targetVertex);
            // In case of Dijkstra’s algorithm couldn't find any ways
            if (!shortestPath.HasValue)
                throw new NoPathFoundException();

            return shortestPath.Value;
        }

        [Pure]
        [CanBeNull]
        private SortedPath? GetShortestPathInGraph(
            [NotNull] IVertexListGraph<TVertex, TEdge> graph,
            [NotNull] TVertex source,
            [NotNull] TVertex target)
        {
            Debug.Assert(graph != null);
            Debug.Assert(source != null);
            Debug.Assert(target != null);

            // Compute distances between the start vertex and other
            var algorithm = new DijkstraShortestPathAlgorithm<TVertex, TEdge>(graph, _weights);
            var recorder = new VertexPredecessorRecorderObserver<TVertex, TEdge>();

            using (recorder.Attach(algorithm))
            {
                algorithm.Compute(source);
            }

            // Get shortest path from start (source) vertex to target
            return recorder.TryGetPath(target, out IEnumerable<TEdge> path)
                ? new SortedPath(path)
                : (SortedPath?)null;
        }

        [Pure]
        [CanBeNull]
        private static SortedPath? ExtractShortestPathCandidate(
            [NotNull] List<SortedPath> shortestPaths,
            [NotNull] IQueue<SortedPath> shortestPathCandidates)
        {
            bool isNewPath = false;
            SortedPath? newPath = null;
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
            SortedPath previousPath,
            [NotNull] List<SortedPath> shortestPaths,
            [NotNull] IQueue<SortedPath> shortestPathCandidates)
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
                TEdge[] rootPath = previousPath.GetEdges(i);

                foreach (SortedPath path in shortestPaths.Where(path => rootPath.SequenceEqual(path.GetEdges(i))))
                {
                    // Remove the links that are part of the previous shortest paths which share the same root path
                    TEdge edgeToRemove = path.GetEdge(i);
                    _edgesToRestore.Add(edgeToRemove);
                    _graph.RemoveEdge(edgeToRemove);
                }

                var verticesToRestore = new List<TVertex>();
                foreach (TVertex source in rootPath.Select(rootPathEdge => rootPathEdge.Source))
                {
                    if (!EqualityComparer<TVertex>.Default.Equals(spurVertex, source))
                    {
                        verticesToRestore.Add(source);

                        _graph.EdgeRemoved += OnGraphEdgeRemoved;
                        _graph.RemoveVertex(source);
                        _graph.EdgeRemoved -= OnGraphEdgeRemoved;
                    }
                }

                SortedPath? spurPath = GetShortestPathInGraph(_graph, spurVertex, _targetVertex);
                if (spurPath.HasValue)
                {
                    // Entire path is made up of the root path and spur path
                    var totalPath = new SortedPath(previousPath.GetEdges(i).Concat(spurPath.Value));

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
            SortedPath? newPath = ExtractShortestPathCandidate(shortestPaths, shortestPathCandidates);
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
        public IEnumerable<SortedPath> Execute()
        {
            SortedPath initialPath = GetInitialShortestPath();
            var shortestPaths = new List<SortedPath> { initialPath };

            // Initialize the set to store the potential k-th shortest path
            var shortestPathCandidates = new BinaryQueue<SortedPath, double>(GetPathDistance);

            for (int k = 1; k < _k; ++k)
            {
                SortedPath previousPath = shortestPaths[k - 1];

                if (!SearchAndAddKthShortestPath(previousPath, shortestPaths, shortestPathCandidates))
                    break;
            }

            return _filter(shortestPaths);
        }

        [NotNull, ItemNotNull]
        private readonly List<TEdge> _edgesToRestore =
            new List<TEdge>();

        private void OnGraphEdgeRemoved([NotNull] TEdge edge)
        {
            _edgesToRestore.Add(edge);
        }
    }
}