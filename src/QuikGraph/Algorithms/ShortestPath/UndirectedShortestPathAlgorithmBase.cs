using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using JetBrains.Annotations;
using QuikGraph.Algorithms.Services;

namespace QuikGraph.Algorithms.ShortestPath
{
    /// <summary>
    /// Base class for all shortest path finder algorithms in undirected graphs.
    /// </summary>
    /// <typeparam name="TVertex">Vertex type.</typeparam>
    /// <typeparam name="TEdge">Edge type.</typeparam>
    public abstract class UndirectedShortestPathAlgorithmBase<TVertex, TEdge>
        : RootedAlgorithmBase<TVertex, IUndirectedGraph<TVertex, TEdge>>
        , IVertexColorizerAlgorithm<TVertex>
        , IUndirectedTreeBuilderAlgorithm<TVertex, TEdge>
        , IDistancesCollection<TVertex>
        where TEdge : IEdge<TVertex>
    {
        /// <summary> The processed Graph </summary>
        public IGraph<TVertex, TEdge> VisitededGraph => base.VisitedGraph;

        /// <summary>
        /// Vertices distances.
        /// </summary>
        private IDictionary<TVertex, double> _distances;

        /// <summary>
        /// Initializes a new <see cref="UndirectedShortestPathAlgorithmBase{TVertex,TEdge}"/> class.
        /// </summary>
        /// <param name="visitedGraph">Graph to visit.</param>
        /// <param name="edgeWeights">Function that computes the weight for a given edge.</param>
        /// <param name="distanceRelaxer">Distance relaxer.</param>
        /// <param name="host">Host to use if set, otherwise use this reference.</param>
        /// <exception cref="T:System.ArgumentNullException"><paramref name="visitedGraph"/> is <see langword="null"/>.</exception>
        /// <exception cref="T:System.ArgumentNullException"><paramref name="edgeWeights"/> is <see langword="null"/>.</exception>
        /// <exception cref="T:System.ArgumentNullException"><paramref name="distanceRelaxer"/> is <see langword="null"/>.</exception>
        protected UndirectedShortestPathAlgorithmBase(
            [NotNull] IUndirectedGraph<TVertex, TEdge> visitedGraph,
            [NotNull] Func<TEdge, double> edgeWeights,
            [CanBeNull] IDistanceRelaxer distanceRelaxer = null,
            [CanBeNull] IAlgorithmComponent host = null)
            : base(visitedGraph, host)
        {
            Weights = edgeWeights ?? throw new ArgumentNullException(nameof(edgeWeights));
            DistanceRelaxer = distanceRelaxer ?? DistanceRelaxers.ShortestDistance;
        }

        /// <summary>
        /// Vertices distances.
        /// </summary>
        [Obsolete("Use methods on " + nameof(IDistancesCollection<object>) + " to interact with the distances instead.")]
        public IDictionary<TVertex, double> Distances => _distances;

        /// <summary>
        /// Gets the distance associated to the given <paramref name="vertex"/>.
        /// </summary>
        /// <param name="vertex">The vertex to get the distance for.</param>
        [Pure]
        protected double GetVertexDistance([NotNull] TVertex vertex)
        {
            return _distances[vertex];
        }

        /// <summary>
        /// Sets the distance associated to the given <paramref name="vertex"/>.
        /// </summary>
        /// <param name="vertex">The vertex to get the distance for.</param>
        /// <param name="distance">The distance.</param>
        protected void SetVertexDistance([NotNull] TVertex vertex, double distance)
        {
            _distances[vertex] = distance;
        }

        /// <inheritdoc />
        /// <exception cref="T:System.InvalidOperationException">Algorithm has not been run.</exception>
        public bool TryGetDistance(TVertex vertex, out double distance)
        {
            if (vertex == null)
                throw new ArgumentNullException(nameof(vertex));
            if (_distances is null)
                throw new InvalidOperationException("Run the algorithm before.");

            return _distances.TryGetValue(vertex, out distance);
        }

        /// <inheritdoc />
        /// <exception cref="T:System.InvalidOperationException">Algorithm has not been run.</exception>
        public double GetDistance(TVertex vertex)
        {
            bool vertexFound = TryGetDistance(vertex, out double distance);
            if (!vertexFound)
                return double.NaN;
            return distance;
        }

        /// <inheritdoc />
        public IEnumerable<KeyValuePair<TVertex, double>> GetDistances()
        {
            return _distances?.Select(pair => pair) ?? Enumerable.Empty<KeyValuePair<TVertex, double>>();
        }

        /// <summary>
        /// Gets the function that gives access to distances from a vertex.
        /// </summary>
        [Pure]
        [NotNull]
        protected Func<TVertex, double> DistancesIndexGetter()
        {
            return AlgorithmExtensions.GetIndexer(_distances);
        }

        /// <summary>
        /// Function that given an edge return the weight of this edge.
        /// </summary>
        [NotNull]
        public Func<TEdge, double> Weights { get; }

        /// <summary>
        /// Distance relaxer.
        /// </summary>
        [NotNull]
        public IDistanceRelaxer DistanceRelaxer { get; }

        #region AlgorithmBase<TGraph>

        /// <inheritdoc />
        protected override void Initialize()
        {
            base.Initialize();

            VerticesColors = new Dictionary<TVertex, GraphColor>(VisitedGraph.VertexCount);
            _distances = new Dictionary<TVertex, double>(VisitedGraph.VertexCount);
        }

        #endregion

        #region IVertexColorizerAlgorithm<TVertex>

        /// <summary>
        /// Stores vertices associated to their colors (treatment state).
        /// </summary>
        public IDictionary<TVertex, GraphColor> VerticesColors { get; private set; }

        /// <inheritdoc />
        public GraphColor? GetVertexColor(TVertex vertex)
        {
            if (VerticesColors.TryGetValue(vertex, out GraphColor color))
                return color;
            return null;
        }

        #endregion

        /// <summary>
        /// Fired when the distance label for the target vertex is decreased.
        /// The edge that participated in the last relaxation for vertex v is
        /// an edge in the shortest paths tree.
        /// </summary>
        public event UndirectedEdgeAction<TVertex, TEdge> TreeEdge;

        /// <summary>
        /// Called on each <see cref="TreeEdge"/> event.
        /// </summary>
        /// <param name="edge">Concerned edge.</param>
        /// <param name="reversed">Indicates if the edge is reversed.</param>
        protected virtual void OnTreeEdge([NotNull] TEdge edge, bool reversed)
        {
            Debug.Assert(edge != null);

            TreeEdge?.Invoke(
                this,
                new UndirectedEdgeEventArgs<TVertex, TEdge>(edge, reversed));
        }

        /// <summary>
        /// Runs the relaxation algorithm on the given <paramref name="edge"/>.
        /// </summary>
        /// <param name="edge">Edge to relax.</param>
        /// <param name="source">Source vertex.</param>
        /// <param name="target">Target vertex.</param>
        /// <returns>True if relaxation decreased the target vertex distance, false otherwise.</returns>
        protected bool Relax([NotNull] TEdge edge, [NotNull] TVertex source, [NotNull] TVertex target)
        {
            Debug.Assert(edge != null);
            Debug.Assert(source != null);
            Debug.Assert(target != null);
            Debug.Assert(
                (VisitedGraph.AreVerticesEqual(edge.Source, source)
                    && VisitedGraph.AreVerticesEqual(edge.Target, target))
                ||
                (VisitedGraph.AreVerticesEqual(edge.Source, target)
                    && VisitedGraph.AreVerticesEqual(edge.Target, source)));

            double du = GetVertexDistance(source);
            double dv = GetVertexDistance(target);
            double we = Weights(edge);

            double duwe = DistanceRelaxer.Combine(du, we);
            if (DistanceRelaxer.Compare(duwe, dv) < 0)
            {
                SetVertexDistance(target, duwe);
                return true;
            }

            return false;
        }
    }
}