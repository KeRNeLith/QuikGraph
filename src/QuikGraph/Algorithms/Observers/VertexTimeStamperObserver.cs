using System;
using System.Collections.Generic;
using System.Diagnostics;
using JetBrains.Annotations;
using static QuikGraph.Utils.DisposableHelpers;

namespace QuikGraph.Algorithms.Observers
{
    /// <summary>
    /// Recorder of vertices discover timestamps.
    /// </summary>
    /// <typeparam name="TVertex">Vertex type.</typeparam>
    public sealed class VertexTimeStamperObserver<TVertex> : IObserver<IVertexTimeStamperAlgorithm<TVertex>>
    {
        private int _currentTime;

        /// <summary>
        /// Initializes a new instance of the <see cref="VertexTimeStamperObserver{TVertex}"/> class.
        /// </summary>
        public VertexTimeStamperObserver()
            : this(new Dictionary<TVertex, int>(), new Dictionary<TVertex, int>())
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="VertexTimeStamperObserver{TVertex}"/> class.
        /// </summary>
        /// <param name="discoverTimes">Vertices discover times.</param>
        /// <exception cref="T:System.ArgumentNullException"><paramref name="discoverTimes"/> is <see langword="null"/>.</exception>
        public VertexTimeStamperObserver([NotNull] IDictionary<TVertex, int> discoverTimes)
        {
            DiscoverTimes = discoverTimes ?? throw new ArgumentNullException(nameof(discoverTimes));
            FinishTimes = null;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="VertexTimeStamperObserver{TVertex}"/> class.
        /// </summary>
        /// <param name="discoverTimes">Vertices discover times.</param>
        /// <param name="finishTimes">Vertices fully treated times.</param>
        /// <exception cref="T:System.ArgumentNullException"><paramref name="discoverTimes"/> is <see langword="null"/>.</exception>
        /// <exception cref="T:System.ArgumentNullException"><paramref name="finishTimes"/> is <see langword="null"/>.</exception>
        public VertexTimeStamperObserver(
            [NotNull] IDictionary<TVertex, int> discoverTimes,
            [NotNull] IDictionary<TVertex, int> finishTimes)
        {
            DiscoverTimes = discoverTimes ?? throw new ArgumentNullException(nameof(discoverTimes));
            FinishTimes = finishTimes ?? throw new ArgumentNullException(nameof(finishTimes));
        }

        /// <summary>
        /// Times of vertices discover.
        /// </summary>
        [NotNull]
        public IDictionary<TVertex, int> DiscoverTimes { get; }

        /// <summary>
        /// Times of vertices fully treated.
        /// </summary>
        [CanBeNull]
        public IDictionary<TVertex, int> FinishTimes { get; }

        #region IObserver<TAlgorithm>

        /// <inheritdoc />
        public IDisposable Attach(IVertexTimeStamperAlgorithm<TVertex> algorithm)
        {
            if (algorithm is null)
                throw new ArgumentNullException(nameof(algorithm));

            algorithm.DiscoverVertex += OnVertexDiscovered;
            if (FinishTimes != null)
            {
                algorithm.FinishVertex += OnVertexFinished;
            }

            return Finally(() =>
            {
                algorithm.DiscoverVertex -= OnVertexDiscovered;
                if (FinishTimes != null)
                {
                    algorithm.FinishVertex -= OnVertexFinished;
                }
            });
        }

        #endregion

        private void OnVertexDiscovered([NotNull] TVertex vertex)
        {
            Debug.Assert(vertex != null);

            DiscoverTimes[vertex] = _currentTime++;
        }

        private void OnVertexFinished([NotNull] TVertex vertex)
        {
            Debug.Assert(vertex != null);

            // ReSharper disable once PossibleNullReferenceException, Justification: Not null if the handler is attached
            FinishTimes[vertex] = _currentTime++;
        }
    }
}