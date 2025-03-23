using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
#if SUPPORTS_AGGRESSIVE_INLINING
using System.Runtime.CompilerServices;
#endif
using JetBrains.Annotations;

namespace QuikGraph
{
    /// <summary>
    /// Directed graph data structure using a compressed sparse row representation.
    /// (http://www.cs.utk.edu/~dongarra/etemplates/node373.html)
    /// </summary>
    /// <typeparam name="TVertex">Vertex type.</typeparam>
#if SUPPORTS_SERIALIZATION
    [Serializable]
#endif
    [DebuggerDisplay("VertexCount = {" + nameof(VertexCount) + "}, EdgeCount = {" + nameof(EdgeCount) + "}")]
    public sealed class CompressedSparseRowGraph<TVertex> : IVertexListGraph<TVertex, SEquatableEdge<TVertex>>, IEdgeListGraph<TVertex, SEquatableEdge<TVertex>>
#if SUPPORTS_CLONEABLE
        , ICloneable
#endif
    {
#if SUPPORTS_SERIALIZATION
        [Serializable]
#endif
        private struct Range
        {
            public readonly int Start;
            public readonly int End;

            public Range(int start, int end)
            {
                Debug.Assert(start >= 0, "Must be positive");
                Debug.Assert(start <= end, $"Must be less that {nameof(start)} ({start}).");

                Start = start;
                End = end;
            }

            public int Length => End - Start;
        }

        /// <inheritdoc />
        public Func<TVertex, TVertex, bool> AreVerticesEqual
        {
            get => areVerticesEqual ?? EqualityComparer<TVertex>.Default.Equals;
            set => areVerticesEqual = value;
        }
        [CanBeNull]
        private Func<TVertex, TVertex, bool> areVerticesEqual;

        private CompressedSparseRowGraph(
            [NotNull] Dictionary<TVertex, Range> outEdgeStartRanges,
            [NotNull, ItemNotNull] TVertex[] outEdges)
        {
            Debug.Assert(outEdgeStartRanges != null);
            Debug.Assert(outEdges != null);

            _outEdgeStartRanges = outEdgeStartRanges;
            _outEdges = outEdges;
        }

        /// <summary>
        /// Converts the given <paramref name="graph"/> to a <see cref="CompressedSparseRowGraph{TVertex}"/>.
        /// </summary>
        /// <param name="graph">Graph to convert.</param>
        /// <typeparam name="TEdge">Edge type.</typeparam>
        /// <returns>A corresponding <see cref="CompressedSparseRowGraph{TVertex}"/>.</returns>
        /// <exception cref="T:System.ArgumentNullException"><paramref name="graph"/> is <see langword="null"/>.</exception>
        [NotNull]
        public static CompressedSparseRowGraph<TVertex> FromGraph<TEdge>(
            [NotNull] IVertexAndEdgeListGraph<TVertex, TEdge> graph)
            where TEdge : IEdge<TVertex>
        {
            if (graph is null)
                throw new ArgumentNullException(nameof(graph));

            var outEdgeStartRanges = new Dictionary<TVertex, Range>(graph.VertexCount);
            var outEdges = new TVertex[graph.EdgeCount];

            int start = 0;
            int index = 0;
            foreach (TVertex vertex in graph.Vertices)
            {
                int end = start + (graph.OutDegree(vertex) ?? 0);
                var range = new Range(start, end);
                outEdgeStartRanges.Add(vertex, range);

                foreach (TEdge edge in graph.OutEdges(vertex))
                {
                    outEdges[index++] = edge.Target;
                }

                start = end;
                Debug.Assert(index == end);
            }

            Debug.Assert(index == outEdges.Length);

            return new CompressedSparseRowGraph<TVertex>(outEdgeStartRanges, outEdges);
        }

        #region IGraph<TVertex,TEdge>

        /// <inheritdoc />
        public bool IsDirected => true;

        /// <inheritdoc />
        public bool AllowParallelEdges => false;

        #endregion

        #region IVertexSet<TVertex,TEdge>

        /// <inheritdoc />
        public bool IsVerticesEmpty => VertexCount == 0;

        /// <inheritdoc />
        public int VertexCount => _outEdgeStartRanges.Count;

        /// <inheritdoc />
        public IEnumerable<TVertex> Vertices => _outEdgeStartRanges.Keys.AsEnumerable();

        /// <inheritdoc />
        public bool ContainsVertex(TVertex vertex)
        {
            if (vertex == null)
                throw new ArgumentNullException(nameof(vertex));

            return _outEdgeStartRanges.ContainsKey(vertex);
        }

        #endregion

        #region IEdgeSet<TVertex,TEdge>

        /// <inheritdoc />
        public bool IsEdgesEmpty => EdgeCount == 0;

        /// <inheritdoc />
        public int EdgeCount => _outEdges.Length;

        [NotNull, ItemNotNull]
        private readonly TVertex[] _outEdges;

        [NotNull]
        private readonly Dictionary<TVertex, Range> _outEdgeStartRanges;

        /// <inheritdoc />
        public IEnumerable<SEquatableEdge<TVertex>> Edges
        {
            get
            {
                foreach (KeyValuePair<TVertex, Range> pair in _outEdgeStartRanges)
                {
                    TVertex source = pair.Key;
                    Range range = pair.Value;
                    for (int i = range.Start; i < range.End; ++i)
                    {
                        TVertex target = _outEdges[i];
                        yield return new SEquatableEdge<TVertex>(source, target);
                    }
                }
            }
        }

        /// <inheritdoc />
        public bool ContainsEdge(SEquatableEdge<TVertex> edge)
        {
            if (edge is null)
            {
                throw new ArgumentNullException(nameof(edge));
            }
            return ContainsEdge(edge.Source, edge.Target);
        }

        #endregion

        #region IIncidenceGraph<TVertex,TEdge>

        /// <inheritdoc />
        public bool ContainsEdge(TVertex source, TVertex target)
        {
            if (source == null)
                throw new ArgumentNullException(nameof(source));
            if (target == null)
                throw new ArgumentNullException(nameof(target));

            if (_outEdgeStartRanges.TryGetValue(source, out Range range))
            {
                for (int i = range.Start; i < range.End; ++i)
                {
                    if (AreVerticesEqual(_outEdges[i], target))
                        return true;
                }
            }

            return false;
        }

        /// <inheritdoc />
        public bool TryGetEdge(TVertex source, TVertex target, out SEquatableEdge<TVertex> edge)
        {
            if (ContainsEdge(source, target))
            {
                edge = new SEquatableEdge<TVertex>(source, target);
                return true;
            }

            edge = default(SEquatableEdge<TVertex>);
            return false;
        }

        /// <summary> Returns an empty Edge-Set </summary>
        public IEnumerable<SEquatableEdge<TVertex>> Empty => Edge.Empty<SEquatableEdge<TVertex>>();


        /// <inheritdoc />
        public IEnumerable<SEquatableEdge<TVertex>> GetEdges(TVertex source, TVertex target)
        {
            if (source == null)
                throw new ArgumentNullException(nameof(source));
            if (target == null)
                throw new ArgumentNullException(nameof(target));

            if (_outEdgeStartRanges.TryGetValue(source, out Range range))
            {
                return _GetEdges_();
            }

            return Empty;

            IEnumerable<SEquatableEdge<TVertex>> _GetEdges_()
            {
                for (int i = range.Start; i < range.End; ++i)
                {
                    if (AreVerticesEqual(_outEdges[i], target))
                        yield return new SEquatableEdge<TVertex>(source, target);
                }
            }
        }

        #endregion

        #region IImplicitGraph<TVertex,TEdge>

        /// <inheritdoc />
        public int? OutDegree(TVertex vertex)
        {
            if (vertex == null)
                throw new ArgumentNullException(nameof(vertex));

            if (_outEdgeStartRanges.TryGetValue(vertex, out Range range))
                return range.Length;

            return null;
        }

        /// <inheritdoc />
        public IEnumerable<SEquatableEdge<TVertex>> OutEdges(TVertex vertex)
        {
            if (vertex == null)
                throw new ArgumentNullException(nameof(vertex));
            return OutEdgesIterator(vertex);
        }

        [Pure]
        [CanBeNull]
#if SUPPORTS_AGGRESSIVE_INLINING
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        private IEnumerable<SEquatableEdge<TVertex>> OutEdgesIterator([NotNull] TVertex vertex)
        {
            Debug.Assert(vertex != null);

            if (_outEdgeStartRanges.TryGetValue(vertex, out Range range))
                for (int i = range.Start; i < range.End; ++i)
                    yield return new SEquatableEdge<TVertex>(vertex, _outEdges[i]);
        }

        /// <inheritdoc />
        public SEquatableEdge<TVertex> OutEdge(TVertex vertex, int index)
        {
            if (vertex == null)
                throw new ArgumentNullException(nameof(vertex));

            if (_outEdgeStartRanges.TryGetValue(vertex, out Range range))
            {
                int targetIndex = range.Start + index;
                return new SEquatableEdge<TVertex>(vertex, _outEdges[targetIndex]);
            }

            return default(SEquatableEdge<TVertex>);
        }

        #endregion

        #region ICloneable

        /// <summary>
        /// Clones this graph.
        /// </summary>
        /// <returns>Cloned graph.</returns>
        [Pure]
        [NotNull]
        public CompressedSparseRowGraph<TVertex> Clone()
        {
            var ranges = new Dictionary<TVertex, Range>(_outEdgeStartRanges);
            var edges = (TVertex[])_outEdges.Clone();
            return new CompressedSparseRowGraph<TVertex>(ranges, edges);
        }

#if SUPPORTS_CLONEABLE
        /// <inheritdoc />
        object ICloneable.Clone()
        {
            return Clone();
        }
#endif

        #endregion
    }
}