using System;
using System.Collections.Generic;
using System.Diagnostics;
using JetBrains.Annotations;
using static QuikGraph.Utils.DisposableHelpers;

namespace QuikGraph.Algorithms.Observers
{
    /// <summary> Recorder of vertices predecessors (undirected) by observing Edges. </summary>
#if SUPPORTS_SERIALIZATION
    [Serializable]
#endif
    public sealed class VertexPredecessorRecorderObserver<TVertex, TEdge>
        : IObserver<ITreeBuilderAlgorithm<TVertex, TEdge>> where TEdge : IEdge<TVertex>
    {
        /// <inheritdoc cref="VertexPredecessorRecorderObserver{TVertex, TEdge}"/>
        public VertexPredecessorRecorderObserver()
            : this(new Dictionary<TVertex, TEdge>())
        {
        }

        /// <inheritdoc cref="VertexPredecessorRecorderObserver{TVertex, TEdge}"/>
        public VertexPredecessorRecorderObserver(IEqualityComparer<TVertex> equality)
            : this(new Dictionary<TVertex, TEdge>(equality), equality.Equals)
        {
        }

        /// <inheritdoc cref="VertexPredecessorRecorderObserver{TVertex, TEdge}"/>
        public VertexPredecessorRecorderObserver(Func<TVertex, TVertex, bool> equality)
            : this(new Dictionary<TVertex, TEdge>(), equality)
        {
        }

        /// <inheritdoc cref="VertexPredecessorRecorderObserver{TVertex, TEdge}"/>
        public VertexPredecessorRecorderObserver([NotNull] IDictionary<TVertex, TEdge> verticesPredecessors
            , Func<TVertex, TVertex, bool> equals = null)
        {
            VerticesPredecessors = verticesPredecessors ?? throw new ArgumentNullException(nameof(verticesPredecessors));
            AreVerticesEqual = equals;
        }

        /// <summary>
        /// Vertices predecessors.
        /// </summary>
        [NotNull]
        public IDictionary<TVertex, TEdge> VerticesPredecessors { get; }

        /// <summary>
        /// 
        /// </summary>
        public Func<TVertex, TVertex, bool> AreVerticesEqual { get; }

        #region IObserver<TAlgorithm>

        /// <inheritdoc />
        public IDisposable Attach(ITreeBuilderAlgorithm<TVertex, TEdge> algorithm)
        {
            if (algorithm is null)
                throw new ArgumentNullException(nameof(algorithm));

            algorithm.TreeEdge += OnEdgeDiscovered;
            return Finally(() => algorithm.TreeEdge -= OnEdgeDiscovered);
        }

        #endregion

        private void OnEdgeDiscovered([NotNull] TEdge edge)
        {
            Debug.Assert(edge != null);

            VerticesPredecessors[edge.Target] = edge;
        }

        /// <summary> Tries to get the predecessor path, if reachable. </summary>
        /// <param name="terminal">Path ending vertex.</param>
        /// <returns>Path to the ending vertex, null if no path was found.</returns>
        /// <exception cref="T:System.ArgumentNullException"><paramref name="terminal"/> is <see langword="null"/>.</exception>
        [Pure]
        public List<TEdge> GetPath([NotNull] TVertex terminal) => VerticesPredecessors.GetPath(terminal, AreVerticesEqual);
    }
}