using System;
using System.Collections.Generic;
using System.Diagnostics;
using JetBrains.Annotations;
using static QuikGraph.Utils.DisposableHelpers;

namespace QuikGraph.Algorithms.Observers
{
    /// <summary> Recorder of vertices predecessors (for undirected Graphs). </summary>
    /// <remarks> Records Edges In-Order, as they are discovered. </remarks>
#if SUPPORTS_SERIALIZATION
    [Serializable]
#endif
    public sealed class UndirectedVertexPredecessorRecorderObserver<TVertex, TEdge> :
        IObserver<IUndirectedTreeBuilderAlgorithm<TVertex, TEdge>>
        where TEdge : IEdge<TVertex>
    {
        /// <summary> Compares Vertices for Equality </summary>
        public Func<TVertex, TVertex, bool> AreVerticesEqual { get; private set; }

        /// <summary> Initializes <see cref="VerticesPredecessors"/>  empty. </summary>
        public UndirectedVertexPredecessorRecorderObserver()
            : this(new Dictionary<TVertex, TEdge>())
        {
        }

        /// <summary> Initializes with <paramref name="verticesPredecessors"/>. </summary>
        public UndirectedVertexPredecessorRecorderObserver(
            [NotNull] IDictionary<TVertex, TEdge> verticesPredecessors)
        {
            VerticesPredecessors = verticesPredecessors ?? throw new ArgumentNullException(nameof(verticesPredecessors));
        }

        /// <summary> predecessor-Edges indexed by the Vertices they are leading to </summary>
        [NotNull]
        public IDictionary<TVertex, TEdge> VerticesPredecessors { get; }

        #region IObserver<TAlgorithm>

        /// <inheritdoc />
        public IDisposable Attach(IUndirectedTreeBuilderAlgorithm<TVertex, TEdge> algorithm)
        {
            if (algorithm is null)
                throw new ArgumentNullException(nameof(algorithm));

            AreVerticesEqual = AreVerticesEqual ?? algorithm.VisitededGraph.AreVerticesEqual;

            //algorithm.VisitededGraph.AreVerticesEqual
            algorithm.TreeEdge += OnEdgeDiscovered;
            return Finally(() => algorithm.TreeEdge -= OnEdgeDiscovered);
        }

        #endregion

        private void OnEdgeDiscovered([NotNull] object sender, [NotNull] UndirectedEdgeEventArgs<TVertex, TEdge> args)
        {
            Debug.Assert(sender != null);
            Debug.Assert(args != null);

            VerticesPredecessors[args.Target] = args.Edge;
        }

        /// <summary> Tries to get the predecessor path, if reachable. </summary>
        /// <returns>Path to the ending vertex, null if a path was not found.</returns>
        /// <exception cref="T:System.ArgumentNullException"><paramref name="vertex"/> is <see langword="null"/>.</exception>
        [Pure]
        public List<TEdge> GetPath([NotNull] TVertex vertex)
            => VerticesPredecessors.GetPath(vertex, AreVerticesEqual);
    }
}