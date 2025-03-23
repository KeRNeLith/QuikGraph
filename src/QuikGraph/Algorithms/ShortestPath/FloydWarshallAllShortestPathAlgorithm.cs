using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using JetBrains.Annotations;
using QuikGraph.Algorithms.Services;
using QuikGraph.Collections;

namespace QuikGraph.Algorithms.ShortestPath
{
    /// <inheritdoc cref="CreateFloydWarshallAllShortestPathAlgorithm"/>
    public static class FloydWarshallAllShortestPathAlgorithm
    {
        /// <summary> Creates a new <see cref="FloydWarshallAllShortestPathAlgorithm{TVertex,TEdge}"/> class. </summary>
        public static FloydWarshallAllShortestPathAlgorithm<TVertex, TEdge>
            CreateFloydWarshallAllShortestPathAlgorithm<TVertex, TEdge>(
            [NotNull] this IVertexAndEdgeListGraph<TVertex, TEdge> visitedGraph,
            [NotNull] Func<TEdge, double> edgeWeights,
            [CanBeNull] IDistanceRelaxer distanceRelaxer = null,
            [CanBeNull] IAlgorithmComponent host = null) where TEdge : IEdge<TVertex>
            => new FloydWarshallAllShortestPathAlgorithm<TVertex
                , TEdge>(visitedGraph, edgeWeights, distanceRelaxer, host);
    }

    /// <summary> Floyd-Warshall all shortest path algorithm.</summary>
    public class FloydWarshallAllShortestPathAlgorithm<TVertex, TEdge>
        : AlgorithmBase<IVertexAndEdgeListGraph<TVertex, TEdge>>
        where TEdge : IEdge<TVertex>
    {
        [NotNull]
        private readonly Func<TEdge, double> _weights;

        [NotNull]
        private readonly IDistanceRelaxer _distanceRelaxer;

        [NotNull]
        private readonly Dictionary<SEquatableEdge<TVertex>, VertexData> _data;

        private struct VertexData
        {
            public double Distance { get; }

            private readonly TVertex _predecessor;
            private readonly TEdge _edge;

            private readonly bool _edgeStored;

            // Null edge for self edge data
            public VertexData(double distance, [CanBeNull] TEdge edge)
            {
                Distance = distance;
                _predecessor = default(TVertex);
                _edge = edge;
                _edgeStored = true;
            }

            public VertexData(double distance, [NotNull] TVertex predecessor)
            {
                Debug.Assert(predecessor != null);

                Distance = distance;
                _predecessor = predecessor;
                _edge = default(TEdge);
                _edgeStored = false;
            }

            [Pure]
            public bool TryGetPredecessor(out TVertex predecessor)
            {
                predecessor = _predecessor;
                return !_edgeStored;
            }

            [Pure]
            public bool TryGetEdge(out TEdge edge)
            {
                edge = _edge;
                return _edgeStored;
            }

            public override string ToString()
            {
                if (_edgeStored)
                    return $"e:{Distance}-{_edge}";
                return $"p:{Distance}-{_predecessor}";
            }
        }

        /// <summary>
        /// Initializes a new <see cref="FloydWarshallAllShortestPathAlgorithm{TVertex,TEdge}"/> class.
        /// </summary>
        /// <param name="visitedGraph">Graph to visit.</param>
        /// <param name="edgeWeights">Function that computes the weight for a given edge.</param>
        /// <param name="distanceRelaxer">Distance relaxer.</param>
        /// <param name="host">Host to use if set, otherwise use this reference.</param>
        /// <exception cref="T:System.ArgumentNullException"><paramref name="visitedGraph"/> is <see langword="null"/>.</exception>
        /// <exception cref="T:System.ArgumentNullException"><paramref name="edgeWeights"/> is <see langword="null"/>.</exception>
        /// <exception cref="T:System.ArgumentNullException"><paramref name="distanceRelaxer"/> is <see langword="null"/>.</exception>
        internal FloydWarshallAllShortestPathAlgorithm(
            [NotNull] IVertexAndEdgeListGraph<TVertex, TEdge> visitedGraph,
            [NotNull] Func<TEdge, double> edgeWeights,
            [CanBeNull] IDistanceRelaxer distanceRelaxer = null,
            [CanBeNull] IAlgorithmComponent host = null)
            : base(visitedGraph, host)
        {
            _weights = edgeWeights ?? throw new ArgumentNullException(nameof(edgeWeights));
            _distanceRelaxer = distanceRelaxer ?? DistanceRelaxers.ShortestDistance;
            _data = new Dictionary<SEquatableEdge<TVertex>, VertexData>();
        }

        /// <summary>
        /// Tries to get the distance (<paramref name="distance"/>) between
        /// <paramref name="source"/> and <paramref name="target"/>.
        /// </summary>
        /// <param name="source">Source vertex.</param>
        /// <param name="target">Target vertex.</param>
        /// <param name="distance">Associated distance (cost).</param>
        /// <returns>True if the distance was found, false otherwise.</returns>
        /// <exception cref="T:System.ArgumentNullException"><paramref name="source"/> is <see langword="null"/>.</exception>
        /// <exception cref="T:System.ArgumentNullException"><paramref name="target"/> is <see langword="null"/>.</exception>
        public bool TryGetDistance([NotNull] TVertex source, [NotNull] TVertex target, out double distance)
        {
            if (source == null)
                throw new ArgumentNullException(nameof(source));
            if (target == null)
                throw new ArgumentNullException(nameof(target));

            if (_data.TryGetValue(new SEquatableEdge<TVertex>(source, target), out VertexData data))
            {
                distance = data.Distance;
                return true;
            }

            distance = -1;
            return false;
        }

        /// <summary> Tries to get the path that links both <paramref name="source"/> and <paramref name="target"/> vertices. </summary>
        /// <returns>The path linking both vertices, if found, otherwise <see langword="null"/>.</returns>
        /// <exception cref="T:System.ArgumentNullException"><paramref name="source"/> is <see langword="null"/>.</exception>
        /// <exception cref="T:System.ArgumentNullException"><paramref name="target"/> is <see langword="null"/>.</exception>
        /// <exception cref="T:System.InvalidOperationException">Failed to find a predecessor vertex while getting path.</exception>
        [CanBeNull]
        public IEnumerable<TEdge> GetPath(
            [NotNull] TVertex source,
            [NotNull] TVertex target)
        {
            if (source == null)
                throw new ArgumentNullException(nameof(source));
            if (target == null)
                throw new ArgumentNullException(nameof(target));

            if (VisitedGraph.AreVerticesEqual(source, target))
            {
                return null;
            }

            return GetPathInternal(source, target);
        }

        [ContractAnnotation("=> true, path:notnull;=> false, path:null")]
        private IEnumerable<TEdge> GetPathInternal(
            [NotNull] TVertex source,
            [NotNull] TVertex target)
        {
#if DEBUG && !NET20
            var set = new HashSet<TVertex> { source, target };
#endif

            var edges = new EdgeList<TEdge>();
            var todo = new Stack<SEquatableEdge<TVertex>>();
            todo.Push(new SEquatableEdge<TVertex>(source, target));
            while (todo.Count > 0)
            {
                SEquatableEdge<TVertex> current = todo.Pop();

                Debug.Assert(!VisitedGraph.AreVerticesEqual(current.Source, current.Target));

                if (_data.TryGetValue(current, out VertexData data))
                {
                    if (data.TryGetEdge(out TEdge edge))
                    {
                        edges.Add(edge);
                    }
                    else
                    {
                        if (data.TryGetPredecessor(out TVertex intermediate))
                        {
#if DEBUG && !NET20
                            Debug.Assert(set.Add(intermediate));
#endif

                            todo.Push(new SEquatableEdge<TVertex>(intermediate, current.Target));
                            todo.Push(new SEquatableEdge<TVertex>(current.Source, intermediate));
                        }
                        else
                        {
                            throw new InvalidOperationException("Cannot find predecessor.");
                        }
                    }
                }
                else
                {
                    // No path found
                    return null;
                }
            }

            Debug.Assert(todo.Count == 0);
            Debug.Assert(edges.Count > 0);

            return edges;
        }

        #region AlgorithmBase<TGraph>

        /// <inheritdoc />
        protected override void Initialize()
        {
            base.Initialize();

            // Matrix i,j -> path
            _data.Clear();

            // Prepare the matrix with initial costs
            // Walk each edge and add entry in cost dictionary
            foreach (TEdge edge in VisitedGraph.Edges)
            {
                SEquatableEdge<TVertex> ij = edge.ToVertexPair();
                double cost = _weights(edge);
                if (!_data.TryGetValue(ij, out VertexData data))
                {
                    _data[ij] = new VertexData(cost, edge);
                }
                else if (cost < data.Distance)
                {
                    _data[ij] = new VertexData(cost, edge);
                }
            }
        }

        /// <inheritdoc />
        protected override void InternalCompute()
        {
            ThrowIfCancellationRequested();

            TVertex[] vertices = VisitedGraph.Vertices.ToArray();

            // Walk each vertices and make sure cost self-cost 0
            foreach (TVertex vertex in vertices)
            {
                _data[new SEquatableEdge<TVertex>(vertex, vertex)] = new VertexData(0, default(TEdge));
            }

            ThrowIfCancellationRequested();

            // Iterate k, i, j
            foreach (TVertex vk in vertices)
            {
                ThrowIfCancellationRequested();

                FillIData(vertices, vk);
            }

            // Check negative cycles
            CheckNegativeCycles(vertices);
        }

        private void FillIData([NotNull, ItemNotNull] TVertex[] vertices, [NotNull] TVertex vk)
        {
            foreach (TVertex vi in vertices)
            {
                var ik = new SEquatableEdge<TVertex>(vi, vk);
                if (_data.TryGetValue(ik, out VertexData pathIk))
                {
                    FillJData(vertices, vi, vk, pathIk);
                }
            }
        }

        private void FillJData(
            [NotNull, ItemNotNull] IEnumerable<TVertex> vertices,
            [NotNull] TVertex vi,
            [NotNull] TVertex vk,
            VertexData pathIk)
        {
            foreach (TVertex vj in vertices)
            {
                var kj = new SEquatableEdge<TVertex>(vk, vj);
                if (_data.TryGetValue(kj, out VertexData pathKj))
                {
                    double combined = _distanceRelaxer.Combine(
                        pathIk.Distance,
                        pathKj.Distance);

                    var ij = new SEquatableEdge<TVertex>(vi, vj);
                    if (_data.TryGetValue(ij, out VertexData pathIj))
                    {
                        if (_distanceRelaxer.Compare(combined, pathIj.Distance) < 0)
                            _data[ij] = new VertexData(combined, vk);
                    }
                    else
                    {
                        _data[ij] = new VertexData(combined, vk);
                    }
                }
            }
        }

        private void CheckNegativeCycles([NotNull, ItemNotNull] IEnumerable<TVertex> vertices)
        {
            foreach (TVertex vi in vertices)
            {
                var ii = new SEquatableEdge<TVertex>(vi, vi);
                if (_data.TryGetValue(ii, out VertexData data) && data.Distance < 0)
                    throw new NegativeCycleGraphException();
            }
        }

        #endregion

        /// <summary>
        /// Dumps current data state to stream <paramref name="writer"/>.
        /// </summary>
        [Conditional("DEBUG")]
        public void Dump([NotNull] TextWriter writer)
        {
            writer.WriteLine("data:");
            foreach (KeyValuePair<SEquatableEdge<TVertex>, VertexData> kv in _data)
            {
                writer.WriteLine(
                    $"{kv.Key.Source}->{kv.Key.Target}: {kv.Value.ToString()}");
            }
        }
    }
}