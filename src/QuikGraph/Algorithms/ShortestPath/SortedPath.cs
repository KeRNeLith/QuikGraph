using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using JetBrains.Annotations;

namespace QuikGraph.Algorithms.ShortestPath
{
    /// <inheritdoc cref="ToSortedPath{TVertex}"/>
    public static class SortedPath
    {
        /// <summary> Converts the edges into a <see cref="SortedPath{TVertex}"/> </summary>
        public static SortedPath<TVertex>? ToSortedPath<TVertex>(this IEnumerable<EquatableTaggedEdge<TVertex, double>> path)
            => (path == null) ? (SortedPath<TVertex>?)null : new SortedPath<TVertex>(path);

    }

    /// <summary> Represents a sorted path with Weights </summary>
    public struct SortedPath<TVertex> : IEnumerable<EquatableTaggedEdge<TVertex, double>>, IEquatable<SortedPath<TVertex>>
    {
        [NotNull, ItemNotNull]
        private readonly List<EquatableTaggedEdge<TVertex, double>> _edges;

        /// <summary>
        /// Initializes a new <see cref="SortedPath{TVertex}"/> struct.
        /// </summary>
        public SortedPath([NotNull, ItemNotNull] IEnumerable<EquatableTaggedEdge<TVertex, double>> edges)
        {
            _edges = edges.ToList();
        }

        /// <summary>
        /// Number of edges in this path.
        /// </summary>
        public int Count => _edges.Count;

        [Pure]
        [NotNull]
        internal TVertex GetVertex(int i)
        {
            Debug.Assert(i >= 0 && i < _edges.Count);

            return _edges[i].Source;
        }

        [Pure]
        [NotNull]
        internal EquatableTaggedEdge<TVertex, double> GetEdge(int i)
        {
            Debug.Assert(i >= 0 && i < _edges.Count);

            return _edges[i];
        }

        [Pure]
        [NotNull, ItemNotNull]
        internal EquatableTaggedEdge<TVertex, double>[] GetEdges(int count)
        {
            if (count > _edges.Count)
            {
                count = _edges.Count;
            }

            Debug.Assert(count >= 0 && count <= _edges.Count);

            return _edges.GetRange(0, count).ToArray();
        }

        /// <inheritdoc />
        public override bool Equals(object obj)
        {
            return obj is SortedPath<TVertex> path && Equals(path);
        }

        /// <inheritdoc />
        public bool Equals(SortedPath<TVertex> other)
        {
            return _edges.SequenceEqual(other._edges);
        }

        /// <inheritdoc />
        public override int GetHashCode()
        {
            return _edges.GetHashCode();
        }

        /// <inheritdoc />
        public IEnumerator<EquatableTaggedEdge<TVertex, double>> GetEnumerator()
        {
            return _edges.GetEnumerator();
        }

        /// <inheritdoc />
        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}