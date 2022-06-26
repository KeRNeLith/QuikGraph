using System;
using System.Collections.Generic;
using System.Diagnostics;
using JetBrains.Annotations;
using static QuikGraph.Utils.DisposableHelpers;

namespace QuikGraph.Algorithms.Observers
{
    /// <summary>
    /// Recorder of vertices predecessors (undirected).
    /// </summary>
    /// <typeparam name="TVertex">Vertex type.</typeparam>
    /// <typeparam name="TEdge">Edge type.</typeparam>
    public sealed class VertexPredecessorRecorderObserver<TVertex, TEdge> : IObserver<ITreeBuilderAlgorithm<TVertex, TEdge>>
        where TEdge : IEdge<TVertex>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="VertexPredecessorRecorderObserver{TVertex,TEdge}"/> class.
        /// </summary>
        public VertexPredecessorRecorderObserver()
            : this(new Dictionary<TVertex, TEdge>())
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="VertexPredecessorRecorderObserver{TVertex,TEdge}"/> class.
        /// </summary>
        /// <param name="verticesPredecessors">Vertices predecessors.</param>
        /// <exception cref="T:System.ArgumentNullException"><paramref name="verticesPredecessors"/> is <see langword="null"/>.</exception>
        public VertexPredecessorRecorderObserver(
            [NotNull] IDictionary<TVertex, TEdge> verticesPredecessors)
        {
            VerticesPredecessors = verticesPredecessors ?? throw new ArgumentNullException(nameof(verticesPredecessors));
        }

        /// <summary>
        /// Vertices predecessors.
        /// </summary>
        [NotNull]
        public IDictionary<TVertex, TEdge> VerticesPredecessors { get; }

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

        /// <summary>
        /// Tries to get the predecessor path, if reachable.
        /// </summary>
        /// <param name="vertex">Path ending vertex.</param>
        /// <param name="path">Path to the ending vertex.</param>
        /// <returns>True if a path was found, false otherwise.</returns>
        /// <exception cref="T:System.ArgumentNullException"><paramref name="vertex"/> is <see langword="null"/>.</exception>
        [Pure]
        [ContractAnnotation("=> true, path:notnull;=> false, path:null")]
        public bool TryGetPath([NotNull] TVertex vertex, [ItemNotNull] out IEnumerable<TEdge> path)
        {
            return VerticesPredecessors.TryGetPath(vertex, out path);
        }
    }
}