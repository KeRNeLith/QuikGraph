using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using QuikGraph.Predicates;

namespace QuikGraph.Algorithms.Ranking
{
    /// <inheritdoc cref="CreatePageRankAlgorithm{TVertex,TEdge}"/>
    public static class PageRankAlgorithm
    {
        /// <summary> Create PageRankAlgorithm </summary>
        public static PageRankAlgorithm<TVertex, TEdge> CreatePageRankAlgorithm<TVertex, TEdge>
            ([NotNull] this IBidirectionalGraph<TVertex, TEdge> visitedGraph) where TEdge : IEdge<TVertex>
            => new PageRankAlgorithm<TVertex, TEdge>(visitedGraph);
    }

    /// <summary> Algorithm that computes the page rank of a graph. </summary>
    public sealed class PageRankAlgorithm<TVertex, TEdge> : AlgorithmBase<IBidirectionalGraph<TVertex, TEdge>>
        where TEdge : IEdge<TVertex>
    {
        /// <summary>
        /// Initializes a new <see cref="PageRankAlgorithm{TVertex,TEdge}"/> class.
        /// </summary>
        /// <param name="visitedGraph">Graph to visit.</param>
        /// <exception cref="T:System.ArgumentNullException"><paramref name="visitedGraph"/> is <see langword="null"/>.</exception>
        public PageRankAlgorithm([NotNull] IBidirectionalGraph<TVertex, TEdge> visitedGraph)
            : base(visitedGraph)
        {
        }

        /// <summary>
        /// Ranks per vertices.
        /// </summary>
        [NotNull]
        public IDictionary<TVertex, double> Ranks { get; private set; } = new Dictionary<TVertex, double>();

        private double _damping = 0.85;

        /// <summary>
        /// Gets or sets the damping rate [0-1].
        /// </summary>
        /// <remarks>By default it uses 0.85 which is the value generally used.</remarks>
        /// <exception cref="T:System.ArgumentOutOfRangeException">Value is negative or higher than 1.</exception>
        public double Damping
        {
            get => _damping;
            set
            {
                if (value < 0 || value > 1)
                    throw new ArgumentOutOfRangeException(nameof(value), $"{nameof(Damping)} must be in interval [0-1].");
                _damping = value;
            }
        }

        private double _tolerance = 2 * double.Epsilon;

        /// <summary>
        /// Gets or sets the error tolerance (used to stop the algorithm).
        /// </summary>
        /// <exception cref="T:System.ArgumentOutOfRangeException">Value is negative.</exception>
        public double Tolerance
        {
            get => _tolerance;
            set
            {
                if (value < 0)
                    throw new ArgumentOutOfRangeException(nameof(value), $"{nameof(Tolerance)} must be positive or 0.");
                _tolerance = value;
            }
        }

        private int _maxIterations = 60;

        /// <summary>
        /// Gets or sets the maximum number of iterations.
        /// </summary>
        /// <exception cref="T:System.ArgumentOutOfRangeException">Value is negative or equal 0.</exception>
        public int MaxIterations
        {
            get => _maxIterations;
            set
            {
                if (value <= 0)
                    throw new ArgumentOutOfRangeException(nameof(value), $"{nameof(MaxIterations)} must be positive.");
                _maxIterations = value;
            }
        }

        #region AlgorithmBase<TGraph>

        /// <summary>
        /// Initializes all vertices ranks (1 / VertexCount).
        /// </summary>
        private void InitializeRanks()
        {
            Ranks.Clear();
            double initialRank = 1 / (double)VisitedGraph.VertexCount;
            foreach (TVertex vertex in VisitedGraph.Vertices)
            {
                Ranks.Add(vertex, initialRank);
            }
        }

        /// <inheritdoc />
        protected override void Initialize()
        {
            base.Initialize();

            InitializeRanks();
        }

        /// <inheritdoc />
        protected override void InternalCompute()
        {
            IDictionary<TVertex, double> tempRanks = new Dictionary<TVertex, double>();

            // Create filtered graph
            var filterGraph = new FilteredBidirectionalGraph<TVertex, TEdge, IBidirectionalGraph<TVertex, TEdge>>(
                VisitedGraph,
                new InDictionaryVertexPredicate<TVertex, double>(Ranks).Test,
                edge => true);

            int iteration = 0;
            double error;
            do
            {
                ThrowIfCancellationRequested();

                // Compute page ranks
                error = 0;
                foreach (KeyValuePair<TVertex, double> pair in Ranks)
                {
                    ThrowIfCancellationRequested();

                    TVertex vertex = pair.Key;
                    double rank = pair.Value;

                    // Compute sum of PR(pj)/L(pj)
                    double r = filterGraph.InEdges(vertex)
                        .Select(edge => edge.Source)
                        .Sum(source => Ranks[source] / filterGraph.OutDegree(source).Value);

                    // Add sourceRank and store it
                    double newRank = (1 - Damping) + Damping * r;
                    tempRanks[vertex] = newRank;

                    // Compute deviation
                    error += Math.Abs(newRank - rank);
                }

                // Swap ranks
                IDictionary<TVertex, double> temp = Ranks;
                Ranks = tempRanks;
                tempRanks = temp;

                ++iteration;
            } while (error > Tolerance && iteration < MaxIterations);
        }

        #endregion

        /// <summary>
        /// Gets the sum of all ranks.
        /// </summary>
        /// <returns>Ranks sum.</returns>
        [Pure]
        public double GetRanksSum()
        {
            return Ranks.Values.Sum();
        }

        /// <summary>
        /// Gets the rank average.
        /// </summary>
        /// <returns>Rank average.</returns>
        [Pure]
        public double GetRanksMean()
        {
            return GetRanksSum() / Ranks.Count;
        }
    }
}