using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using JetBrains.Annotations;
using static QuikGraph.Utils.DisposableHelpers;

namespace QuikGraph.Algorithms.Observers
{
    /// <summary>
    /// Recorder of edges predecessors.
    /// </summary>
    /// <typeparam name="TVertex">Vertex type.</typeparam>
    /// <typeparam name="TEdge">Edge type.</typeparam>
    public sealed class EdgePredecessorRecorderObserver<TVertex, TEdge> : IObserver<IEdgePredecessorRecorderAlgorithm<TVertex, TEdge>>
        where TEdge : IEdge<TVertex>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="EdgePredecessorRecorderObserver{TVertex,TEdge}"/> class.
        /// </summary>
        public EdgePredecessorRecorderObserver()
            : this(new Dictionary<TEdge, TEdge>())
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="EdgePredecessorRecorderObserver{TVertex,TEdge}"/> class.
        /// </summary>
        /// <param name="edgesPredecessors">Edges predecessors.</param>
        /// <exception cref="T:System.ArgumentNullException"><paramref name="edgesPredecessors"/> is <see langword="null"/>.</exception>
        public EdgePredecessorRecorderObserver(
            [NotNull] IDictionary<TEdge, TEdge> edgesPredecessors)
        {
            EdgesPredecessors = edgesPredecessors ?? throw new ArgumentNullException(nameof(edgesPredecessors));
            EndPathEdges = new List<TEdge>();
        }

        /// <summary>
        /// Edges predecessors.
        /// </summary>
        [NotNull]
        public IDictionary<TEdge, TEdge> EdgesPredecessors { get; }

        /// <summary>
        /// Path ending edges.
        /// </summary>
        [NotNull]
        public ICollection<TEdge> EndPathEdges { get; }

        #region IObserver<TAlgorithm>

        /// <inheritdoc />
        public IDisposable Attach(IEdgePredecessorRecorderAlgorithm<TVertex, TEdge> algorithm)
        {
            if (algorithm is null)
                throw new ArgumentNullException(nameof(algorithm));

            algorithm.DiscoverTreeEdge += OnEdgeDiscovered;
            algorithm.FinishEdge += OnEdgeFinished;

            return Finally(() =>
            {
                algorithm.DiscoverTreeEdge -= OnEdgeDiscovered;
                algorithm.FinishEdge -= OnEdgeFinished;
            });
        }

        #endregion

        /// <summary>
        /// Gets a path starting with <paramref name="startingEdge"/>.
        /// </summary>
        /// <param name="startingEdge">Starting edge.</param>
        /// <returns>Edge path.</returns>
        /// <exception cref="T:System.ArgumentNullException"><paramref name="startingEdge"/> is <see langword="null"/>.</exception>
        [Pure]
        [NotNull, ItemNotNull]
        public ICollection<TEdge> Path([NotNull] TEdge startingEdge)
        {
            if (startingEdge == null)
                throw new ArgumentNullException(nameof(startingEdge));

            var path = new List<TEdge> { startingEdge };

            TEdge currentEdge = startingEdge;
            while (EdgesPredecessors.TryGetValue(currentEdge, out TEdge edge))
            {
                path.Add(edge);
                currentEdge = edge;
            }

            path.Reverse();
            return path;
        }

        /// <summary>
        /// Gets all paths.
        /// </summary>
        /// <returns>Enumerable of paths.</returns>
        [Pure]
        [NotNull, ItemNotNull]
        public IEnumerable<ICollection<TEdge>> AllPaths()
        {
            return EndPathEdges.Select(Path);
        }

        /// <summary>
        /// Merges the path starting at <paramref name="startingEdge"/> with remaining edges.
        /// </summary>
        /// <param name="startingEdge">Starting edge.</param>
        /// <param name="colors">Edges colors mapping.</param>
        /// <returns>Merged path.</returns>
        /// <exception cref="T:System.ArgumentNullException"><paramref name="startingEdge"/> is <see langword="null"/>.</exception>
        /// <exception cref="T:System.ArgumentNullException"><paramref name="colors"/> is <see langword="null"/>.</exception>
        [Pure]
        [NotNull, ItemNotNull]
        public ICollection<TEdge> MergedPath(
            [NotNull] TEdge startingEdge,
            [NotNull] IDictionary<TEdge, GraphColor> colors)
        {
            if (startingEdge == null)
                throw new ArgumentNullException(nameof(startingEdge));
            if (colors is null)
                throw new ArgumentNullException(nameof(colors));

            var path = new List<TEdge>();

            TEdge currentEdge = startingEdge;
            GraphColor color = colors[currentEdge];
            if (color != GraphColor.White)
                return path;
            colors[currentEdge] = GraphColor.Black;

            path.Add(currentEdge);
            while (EdgesPredecessors.TryGetValue(currentEdge, out TEdge edge))
            {
                color = colors[edge];
                if (color != GraphColor.White)
                {
                    path.Reverse();
                    return path;
                }

                colors[edge] = GraphColor.Black;

                path.Add(edge);
                currentEdge = edge;
            }

            path.Reverse();
            return path;
        }

        /// <summary>
        /// Gets all merged path.
        /// </summary>
        /// <returns>Enumerable of merged paths.</returns>
        [Pure]
        [NotNull, ItemNotNull]
        public IEnumerable<ICollection<TEdge>> AllMergedPaths()
        {
            var colors = new Dictionary<TEdge, GraphColor>();

            foreach (KeyValuePair<TEdge, TEdge> pair in EdgesPredecessors)
            {
                colors[pair.Key] = GraphColor.White;
                colors[pair.Value] = GraphColor.White;
            }

            return EndPathEdges.Select(edge => MergedPath(edge, colors));
        }

        private void OnEdgeDiscovered([NotNull] TEdge edge, [NotNull] TEdge targetEdge)
        {
            Debug.Assert(edge != null);
            Debug.Assert(targetEdge != null);

            if (!EqualityComparer<TEdge>.Default.Equals(edge, targetEdge))
            {
                EdgesPredecessors[targetEdge] = edge;
            }
        }

        private void OnEdgeFinished([NotNull] TEdge finishedEdge)
        {
            Debug.Assert(finishedEdge != null);

            if (EdgesPredecessors.Values.Any(edge => EqualityComparer<TEdge>.Default.Equals(finishedEdge, edge)))
                return;

            EndPathEdges.Add(finishedEdge);
        }
    }
}