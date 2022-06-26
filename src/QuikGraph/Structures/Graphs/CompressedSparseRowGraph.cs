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
    [DebuggerDisplay("VertexCount = {" + nameof(VertexCount) + "}, EdgeCount = {" + nameof(EdgeCount) + "}")]
    public sealed class CompressedSparseRowGraph<TVertex> : IVertexListGraph<TVertex, SEquatableEdge<TVertex>>, IEdgeListGraph<TVertex, SEquatableEdge<TVertex>>
#if SUPPORTS_CLONEABLE
        , ICloneable
#endif
    {
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

        private CompressedSparseRowGraph(
            [NotNull, ItemNotNull] TVertex[] outEdgesTargets,
            [NotNull] Dictionary<TVertex, Range> outEdgesStartRanges)
        {
            Debug.Assert(outEdgesTargets != null);
            Debug.Assert(outEdgesStartRanges != null);

            _outEdgesTargets = outEdgesTargets;
            _outEdgesStartRanges = outEdgesStartRanges;
        }

        // Serialization constructor
        internal CompressedSparseRowGraph(
            [NotNull, ItemNotNull] TVertex[] outEdgesTargets,
            [NotNull] IDictionary<TVertex, KeyValuePair<int, int>> outEdgesStartRanges)
        {
            Debug.Assert(outEdgesTargets != null);
            Debug.Assert(outEdgesStartRanges != null);

            _outEdgesTargets = outEdgesTargets;
            _outEdgesStartRanges = new Dictionary<TVertex, Range>(outEdgesStartRanges.Count);
            foreach (KeyValuePair<TVertex, KeyValuePair<int, int>> pair in outEdgesStartRanges)
            {
                _outEdgesStartRanges.Add(pair.Key, new Range(pair.Value.Key, pair.Value.Value));
            }
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
            var outEdgesTargets = new TVertex[graph.EdgeCount];

            int start = 0;
            int index = 0;
            foreach (TVertex vertex in graph.Vertices)
            {
                int end = start + graph.OutDegree(vertex);
                var range = new Range(start, end);
                outEdgeStartRanges.Add(vertex, range);

                foreach (TEdge edge in graph.OutEdges(vertex))
                {
                    outEdgesTargets[index++] = edge.Target;
                }

                start = end;
                Debug.Assert(index == end);
            }

            Debug.Assert(index == outEdgesTargets.Length);

            return new CompressedSparseRowGraph<TVertex>(outEdgesTargets, outEdgeStartRanges);
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
        public int VertexCount => _outEdgesStartRanges.Count;

        /// <inheritdoc />
        public IEnumerable<TVertex> Vertices => _outEdgesStartRanges.Keys.AsEnumerable();

        /// <inheritdoc />
        public bool ContainsVertex(TVertex vertex)
        {
            if (vertex == null)
                throw new ArgumentNullException(nameof(vertex));

            return _outEdgesStartRanges.ContainsKey(vertex);
        }

        #endregion

        #region IEdgeSet<TVertex,TEdge>

        /// <inheritdoc />
        public bool IsEdgesEmpty => EdgeCount == 0;

        /// <inheritdoc />
        public int EdgeCount => _outEdgesTargets.Length;

        [NotNull, ItemNotNull]
        private readonly TVertex[] _outEdgesTargets;
        [NotNull, ItemNotNull]
        internal TVertex[] OutEdgesTargets => _outEdgesTargets;

        [NotNull]
        private readonly Dictionary<TVertex, Range> _outEdgesStartRanges;

        // For Serialization
        [Pure]
        internal KeyValuePair<int, int> GetRangeFor([NotNull] TVertex vertex)
        {
            Range range = _outEdgesStartRanges[vertex];
            return new KeyValuePair<int, int>(range.Start, range.End);
        }

        /// <inheritdoc />
        public IEnumerable<SEquatableEdge<TVertex>> Edges
        {
            get
            {
                foreach (KeyValuePair<TVertex, Range> pair in _outEdgesStartRanges)
                {
                    TVertex source = pair.Key;
                    Range range = pair.Value;
                    for (int i = range.Start; i < range.End; ++i)
                    {
                        TVertex target = _outEdgesTargets[i];
                        yield return new SEquatableEdge<TVertex>(source, target);
                    }
                }
            }
        }

        /// <inheritdoc />
        public bool ContainsEdge(SEquatableEdge<TVertex> edge)
        {
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

            if (_outEdgesStartRanges.TryGetValue(source, out Range range))
            {
                for (int i = range.Start; i < range.End; ++i)
                {
                    if (EqualityComparer<TVertex>.Default.Equals(_outEdgesTargets[i], target))
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

        /// <inheritdoc />
        public bool TryGetEdges(TVertex source, TVertex target, out IEnumerable<SEquatableEdge<TVertex>> edges)
        {
            if (source == null)
                throw new ArgumentNullException(nameof(source));
            if (target == null)
                throw new ArgumentNullException(nameof(target));

            if (_outEdgesStartRanges.TryGetValue(source, out Range range))
            {
                edges = GetEdges();
                return true;
            }

            edges = null;
            return false;

            #region Local function

            IEnumerable<SEquatableEdge<TVertex>> GetEdges()
            {
                for (int i = range.Start; i < range.End; ++i)
                {
                    if (EqualityComparer<TVertex>.Default.Equals(_outEdgesTargets[i], target))
                        yield return new SEquatableEdge<TVertex>(source, target);
                }
            }

            #endregion
        }

        #endregion

        #region IImplicitGraph<TVertex,TEdge>

        /// <inheritdoc />
        public bool IsOutEdgesEmpty(TVertex vertex)
        {
            return OutDegree(vertex) == 0;
        }

        /// <inheritdoc />
        public int OutDegree(TVertex vertex)
        {
            if (vertex == null)
                throw new ArgumentNullException(nameof(vertex));

            if (_outEdgesStartRanges.TryGetValue(vertex, out Range range))
                return range.Length;
            throw new VertexNotFoundException();
        }

        /// <inheritdoc />
        public IEnumerable<SEquatableEdge<TVertex>> OutEdges(TVertex vertex)
        {
            if (vertex == null)
                throw new ArgumentNullException(nameof(vertex));
            return OutEdgesIterator(vertex);
        }

        [Pure]
        [NotNull]
#if SUPPORTS_AGGRESSIVE_INLINING
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        private IEnumerable<SEquatableEdge<TVertex>> OutEdgesIterator([NotNull] TVertex vertex)
        {
            Debug.Assert(vertex != null);

            if (_outEdgesStartRanges.TryGetValue(vertex, out Range range))
            {
                for (int i = range.Start; i < range.End; ++i)
                    yield return new SEquatableEdge<TVertex>(vertex, _outEdgesTargets[i]);
            }
            else
                throw new VertexNotFoundException();
        }

        /// <inheritdoc />
        public bool TryGetOutEdges(TVertex vertex, out IEnumerable<SEquatableEdge<TVertex>> edges)
        {
            if (vertex == null)
                throw new ArgumentNullException(nameof(vertex));

            if (_outEdgesStartRanges.ContainsKey(vertex))
            {
                edges = OutEdgesIterator(vertex);
                return true;
            }

            edges = null;
            return false;
        }

        /// <inheritdoc />
        public SEquatableEdge<TVertex> OutEdge(TVertex vertex, int index)
        {
            if (vertex == null)
                throw new ArgumentNullException(nameof(vertex));

            if (_outEdgesStartRanges.TryGetValue(vertex, out Range range))
            {
                int targetIndex = range.Start + index;
                return new SEquatableEdge<TVertex>(vertex, _outEdgesTargets[targetIndex]);
            }

            throw new VertexNotFoundException();
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
            var outEdgesTargets = (TVertex[])_outEdgesTargets.Clone();
            var ranges = new Dictionary<TVertex, Range>(_outEdgesStartRanges);
            return new CompressedSparseRowGraph<TVertex>(outEdgesTargets, ranges);
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