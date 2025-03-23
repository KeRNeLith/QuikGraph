using System;
using System.Collections.Generic;
using System.Diagnostics;
using JetBrains.Annotations;
using static QuikGraph.Utils.DisposableHelpers;

namespace QuikGraph.Algorithms.Observers
{
    /// <summary>
    /// A distance recorder for <see cref="IUndirectedTreeBuilderAlgorithm{TVertex,TEdge}"/> algorithms.
    /// </summary>
    /// <typeparam name="TVertex">Vertex type.</typeparam>
    /// <typeparam name="TEdge">Edge type.</typeparam>
    public sealed class UndirectedVertexDistanceRecorderObserver<TVertex, TEdge>
        : IObserver<IUndirectedTreeBuilderAlgorithm<TVertex, TEdge>>
        where TEdge : IEdge<TVertex>
    {
        /// <summary>
        /// Initializes a new <see cref="UndirectedVertexDistanceRecorderObserver{TVertex,TEdge}"/> class.
        /// </summary>
        /// <param name="edgeWeights">Function that computes the weight for a given edge.</param>
        /// <exception cref="T:System.ArgumentNullException"><paramref name="edgeWeights"/> is <see langword="null"/>.</exception>
        public UndirectedVertexDistanceRecorderObserver([NotNull] Func<TEdge, double> edgeWeights)
            : this(edgeWeights, DistanceRelaxers.EdgeShortestDistance, new Dictionary<TVertex, double>())
        {
        }

        /// <summary>
        /// Initializes a new <see cref="UndirectedVertexDistanceRecorderObserver{TVertex,TEdge}"/> class.
        /// </summary>
        /// <param name="edgeWeights">Function that computes the weight for a given edge.</param>
        /// <param name="distanceRelaxer">Distance relaxer.</param>
        /// <param name="distances">Distances per vertex.</param>
        /// <exception cref="T:System.ArgumentNullException"><paramref name="edgeWeights"/> is <see langword="null"/>.</exception>
        /// <exception cref="T:System.ArgumentNullException"><paramref name="distanceRelaxer"/> is <see langword="null"/>.</exception>
        /// <exception cref="T:System.ArgumentNullException"><paramref name="distances"/> is <see langword="null"/>.</exception>
        public UndirectedVertexDistanceRecorderObserver(
            [NotNull] Func<TEdge, double> edgeWeights,
            [NotNull] IDistanceRelaxer distanceRelaxer,
            [NotNull] IDictionary<TVertex, double> distances)
        {
            EdgeWeights = edgeWeights ?? throw new ArgumentNullException(nameof(edgeWeights));
            DistanceRelaxer = distanceRelaxer ?? throw new ArgumentNullException(nameof(distanceRelaxer));
            Distances = distances ?? throw new ArgumentNullException(nameof(distances));
        }

        /// <summary>
        /// Distance relaxer.
        /// </summary>
        [NotNull]
        public IDistanceRelaxer DistanceRelaxer { get; }

        /// <summary>
        /// Function that computes the weight for a given edge.
        /// </summary>
        [NotNull]
        public Func<TEdge, double> EdgeWeights { get; }

        /// <summary>
        /// Distances per vertex.
        /// </summary>
        [NotNull]
        public IDictionary<TVertex, double> Distances { get; }

        #region IObserver<TAlgorithm>

        /// <inheritdoc />
        public IDisposable Attach(IUndirectedTreeBuilderAlgorithm<TVertex, TEdge> algorithm)
        {
            if (algorithm is null)
                throw new ArgumentNullException(nameof(algorithm));

            algorithm.TreeEdge += OnEdgeDiscovered;
            return Finally(() => algorithm.TreeEdge -= OnEdgeDiscovered);
        }

        #endregion

        private void OnEdgeDiscovered([NotNull] object sender, [NotNull] UndirectedEdgeEventArgs<TVertex, TEdge> args)
        {
            Debug.Assert(sender != null);
            Debug.Assert(args != null);

            if (!Distances.TryGetValue(args.Source, out double sourceDistance))
                Distances[args.Source] = sourceDistance = DistanceRelaxer.InitialDistance;
            Distances[args.Target] = DistanceRelaxer.Combine(sourceDistance, EdgeWeights(args.Edge));
        }
    }
}