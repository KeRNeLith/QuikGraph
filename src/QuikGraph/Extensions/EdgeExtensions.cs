using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
#if !SUPPORTS_TYPE_FULL_FEATURES
using System.Reflection;
#endif
using JetBrains.Annotations;

namespace QuikGraph
{
    /// <summary>
    /// Extensions related to graph edges.
    /// </summary>
    public static class EdgeExtensions
    {
        /// <summary>
        /// Gets a value indicating if the edge is a self edge.
        /// </summary>
        /// <typeparam name="TVertex">Vertex type.</typeparam>
        /// <param name="edge">Edge to check.</param>
        /// <returns>True if edge is a self one, false otherwise.</returns>
        /// <exception cref="T:System.ArgumentNullException"><paramref name="edge"/> is <see langword="null"/>.</exception>
        [Pure]
        public static bool IsSelfEdge<TVertex>([NotNull] this IEdge<TVertex> edge)
        {
            if (edge is null)
                throw new ArgumentNullException(nameof(edge));

            return EqualityComparer<TVertex>.Default.Equals(edge.Source, edge.Target);
        }

        /// <summary>
        /// Given a <paramref name="vertex"/>, returns the other vertex in the edge.
        /// </summary>
        /// <typeparam name="TVertex">Vertex type.</typeparam>
        /// <param name="edge">The edge.</param>
        /// <param name="vertex">The source or target vertex of the <paramref name="edge"/>.</param>
        /// <returns>The other edge vertex.</returns>
        /// <exception cref="T:System.ArgumentNullException"><paramref name="edge"/> is <see langword="null"/>.</exception>
        /// <exception cref="T:System.ArgumentNullException"><paramref name="vertex"/> is <see langword="null"/>.</exception>
        [Pure]
        [NotNull]
        public static TVertex GetOtherVertex<TVertex>([NotNull] this IEdge<TVertex> edge, [NotNull] TVertex vertex)
        {
            if (edge is null)
                throw new ArgumentNullException(nameof(edge));
            if (vertex == null)
                throw new ArgumentNullException(nameof(vertex));

            return EqualityComparer<TVertex>.Default.Equals(edge.Source, vertex) ? edge.Target : edge.Source;
        }

        /// <summary>
        /// Gets a value indicating if the <paramref name="vertex"/> is adjacent to the
        /// <paramref name="edge"/> (is the source or target).
        /// </summary>
        /// <typeparam name="TVertex">Vertex type.</typeparam>
        /// <param name="edge">The edge.</param>
        /// <param name="vertex">Source or target <paramref name="edge"/> vertex.</param>
        /// <returns>True if the <paramref name="vertex"/> is adjacent to this <paramref name="edge"/>, false otherwise.</returns>
        /// <exception cref="T:System.ArgumentNullException"><paramref name="edge"/> is <see langword="null"/>.</exception>
        /// <exception cref="T:System.ArgumentNullException"><paramref name="vertex"/> is <see langword="null"/>.</exception>
        [Pure]
        public static bool IsAdjacent<TVertex>([NotNull] this IEdge<TVertex> edge, [NotNull] TVertex vertex)
        {
            if (edge is null)
                throw new ArgumentNullException(nameof(edge));
            if (vertex == null)
                throw new ArgumentNullException(nameof(vertex));

            return EqualityComparer<TVertex>.Default.Equals(edge.Source, vertex)
                || EqualityComparer<TVertex>.Default.Equals(edge.Target, vertex);
        }

        /// <summary>
        /// Checks if this sequence of edges makes a path.
        /// </summary>
        /// <typeparam name="TVertex">Vertex type.</typeparam>
        /// <typeparam name="TEdge">Edge type.</typeparam>
        /// <param name="path">Sequence of edges.</param>
        /// <returns>True if the set makes a complete path, false otherwise.</returns>
        /// <exception cref="T:System.ArgumentNullException"><paramref name="path"/> is <see langword="null"/>.</exception>
        [Pure]
        public static bool IsPath<TVertex, TEdge>([NotNull, ItemNotNull] this IEnumerable<TEdge> path)
            where TEdge : IEdge<TVertex>
        {
            if (path is null)
                throw new ArgumentNullException(nameof(path));

            bool first = true;
            var lastTarget = default(TVertex);
            foreach (TEdge edge in path)
            {
                if (first)
                {
                    lastTarget = edge.Target;
                    first = false;
                }
                else
                {
                    if (!EqualityComparer<TVertex>.Default.Equals(lastTarget, edge.Source))
                        return false;
                    lastTarget = edge.Target;
                }
            }

            return true;
        }

        /// <summary>
        /// Checks if this sequence of edges makes a cycle.
        /// </summary>
        /// <remarks>Note that this function only work when given a path.</remarks>
        /// <typeparam name="TVertex">Vertex type.</typeparam>
        /// <typeparam name="TEdge">Edge type.</typeparam>
        /// <param name="path">Sequence of edges that forms a path.</param>
        /// <returns>True if the set makes a cycle, false otherwise.</returns>
        /// <exception cref="T:System.ArgumentNullException"><paramref name="path"/> is <see langword="null"/>.</exception>
        [Pure]
        public static bool HasCycles<TVertex, TEdge>([NotNull, ItemNotNull] this IEnumerable<TEdge> path)
            where TEdge : IEdge<TVertex>
        {
            if (path is null)
                throw new ArgumentNullException(nameof(path));

            var vertices = new Dictionary<TVertex, int>();
            bool first = true;
            foreach (TEdge edge in path)
            {
                if (first)
                {
                    if (edge.IsSelfEdge())
                        return true;
                    vertices.Add(edge.Source, 0);
                    vertices.Add(edge.Target, 0);
                    first = false;
                }
                else
                {
                    if (vertices.ContainsKey(edge.Target))
                        return true;
                    vertices.Add(edge.Target, 0);
                }
            }

            return false;
        }

        /// <summary>
        /// Checks if this path of edges does not make a cycle.
        /// </summary>
        /// <typeparam name="TVertex">Vertex type.</typeparam>
        /// <typeparam name="TEdge">Edge type.</typeparam>
        /// <param name="path">Path of edges.</param>
        /// <returns>True if the path makes a cycle, false otherwise.</returns>
        /// <exception cref="T:System.ArgumentNullException"><paramref name="path"/> is <see langword="null"/>.</exception>
        [Pure]
        public static bool IsPathWithoutCycles<TVertex, TEdge>([NotNull, ItemNotNull] this IEnumerable<TEdge> path)
            where TEdge : IEdge<TVertex>
        {
            if (path is null)
                throw new ArgumentNullException(nameof(path));

            var vertices = new Dictionary<TVertex, int>();
            bool first = true;
            var lastTarget = default(TVertex);
            foreach (TEdge edge in path)
            {
                if (first)
                {
                    lastTarget = edge.Target;
                    if (edge.IsSelfEdge())
                        return false;
                    vertices.Add(edge.Source, 0);
                    vertices.Add(lastTarget, 0);
                    first = false;
                }
                else
                {
                    if (!EqualityComparer<TVertex>.Default.Equals(lastTarget, edge.Source))
                        return false;
                    if (vertices.ContainsKey(edge.Target))
                        return false;

                    lastTarget = edge.Target;
                    vertices.Add(edge.Target, 0);
                }
            }

            return true;
        }

        /// <summary>
        /// Creates a vertex pair (source, target) from this edge.
        /// </summary>
        /// <typeparam name="TVertex">Vertex type.</typeparam>
        /// <param name="edge">The edge.</param>
        /// <returns>A <see cref="SEquatableEdge{TVertex}"/>.</returns>
        /// <exception cref="T:System.ArgumentNullException"><paramref name="edge"/> is <see langword="null"/>.</exception>
        [Pure]
        public static SEquatableEdge<TVertex> ToVertexPair<TVertex>([NotNull] this IEdge<TVertex> edge)
        {
            if (edge is null)
                throw new ArgumentNullException(nameof(edge));
            return new SEquatableEdge<TVertex>(edge.Source, edge.Target);
        }

        /// <summary>
        /// Checks that the <paramref name="root"/> is a predecessor of the given <paramref name="vertex"/>.
        /// </summary>
        /// <typeparam name="TVertex">Vertex type.</typeparam>
        /// <typeparam name="TEdge">Edge type.</typeparam>
        /// <param name="predecessors">Predecessors map.</param>
        /// <param name="root">Root vertex.</param>
        /// <param name="vertex">Ending vertex.</param>
        /// <returns>True if the <paramref name="root"/> is a predecessor of the <paramref name="vertex"/>.</returns>
        /// <exception cref="T:System.ArgumentNullException"><paramref name="predecessors"/> is <see langword="null"/>.</exception>
        /// <exception cref="T:System.ArgumentNullException"><paramref name="root"/> is <see langword="null"/>.</exception>
        /// <exception cref="T:System.ArgumentNullException"><paramref name="vertex"/> is <see langword="null"/>.</exception>
        [Pure]
        public static bool IsPredecessor<TVertex, TEdge>(
            [NotNull] this IDictionary<TVertex, TEdge> predecessors,
            [NotNull] TVertex root,
            [NotNull] TVertex vertex)
            where TEdge : IEdge<TVertex>
        {
            if (predecessors is null)
                throw new ArgumentNullException(nameof(predecessors));
            if (root == null)
                throw new ArgumentNullException(nameof(root));
            if (vertex == null)
                throw new ArgumentNullException(nameof(vertex));

            TVertex currentVertex = vertex;
            if (EqualityComparer<TVertex>.Default.Equals(root, currentVertex))
                return true;

            while (predecessors.TryGetValue(currentVertex, out TEdge predecessor))
            {
                TVertex source = GetOtherVertex(predecessor, currentVertex);
                if (EqualityComparer<TVertex>.Default.Equals(currentVertex, source))
                    return false;
                if (EqualityComparer<TVertex>.Default.Equals(source, root))
                    return true;
                currentVertex = source;
            }

            return false;
        }

        /// <summary>
        /// Tries to get the predecessor path, if reachable.
        /// </summary>
        /// <typeparam name="TVertex">Vertex type.</typeparam>
        /// <typeparam name="TEdge">Edge type.</typeparam>
        /// <param name="predecessors">Predecessors map.</param>
        /// <param name="vertex">Path ending vertex.</param>
        /// <param name="path">Path to the ending vertex.</param>
        /// <returns>True if a path was found, false otherwise.</returns>
        /// <exception cref="T:System.ArgumentNullException"><paramref name="predecessors"/> is <see langword="null"/>.</exception>
        /// <exception cref="T:System.ArgumentNullException"><paramref name="vertex"/> is <see langword="null"/>.</exception>
        [Pure]
        [ContractAnnotation("=> true, path:notnull;=> false, path:null")]
        public static bool TryGetPath<TVertex, TEdge>(
            [NotNull] this IDictionary<TVertex, TEdge> predecessors,
            [NotNull] TVertex vertex,
            [ItemNotNull] out IEnumerable<TEdge> path)
            where TEdge : IEdge<TVertex>
        {
            if (predecessors is null)
                throw new ArgumentNullException(nameof(predecessors));
            if (vertex == null)
                throw new ArgumentNullException(nameof(vertex));

            var computedPath = new List<TEdge>();

            TVertex currentVertex = vertex;
            while (predecessors.TryGetValue(currentVertex, out TEdge edge))
            {
                if (edge.IsSelfEdge())
                    break;

                computedPath.Add(edge);
                currentVertex = GetOtherVertex(edge, currentVertex);
            }

            if (computedPath.Count > 0)
            {
                computedPath.Reverse();
                path = computedPath;
                return true;
            }

            path = null;
            return false;
        }

        /// <summary>
        /// Returns the most efficient comparer for the particular type of <typeparamref name="TEdge"/>.
        /// If <typeparamref name="TEdge"/> implements <see cref="IUndirectedEdge{TVertex}"/>, then only
        /// the (source, target) pair has to be compared; if not, (source, target) and (target, source)
        /// have to be compared.
        /// </summary>
        /// <typeparam name="TVertex">Vertex type.</typeparam>
        /// <typeparam name="TEdge">Edge type.</typeparam>
        /// <returns>The best edge equality comparer.</returns>
        [Pure]
        [NotNull]
        public static EdgeEqualityComparer<TVertex> GetUndirectedVertexEquality<TVertex, TEdge>()
        {
#if SUPPORTS_TYPE_FULL_FEATURES
            if (typeof(IUndirectedEdge<TVertex>).IsAssignableFrom(typeof(TEdge)))
#else
            if (typeof(IUndirectedEdge<TVertex>).GetTypeInfo().IsAssignableFrom(typeof(TEdge).GetTypeInfo()))
#endif
                return SortedVertexEquality;
            return UndirectedVertexEquality;
        }

        /// <summary>
        /// Gets a value indicating if the vertices of this edge match
        /// <paramref name="source"/> and <paramref name="target"/>
        /// or <paramref name="target"/> and <paramref name="source"/> vertices.
        /// </summary>
        /// <typeparam name="TVertex">Vertex type.</typeparam>
        /// <param name="edge">The edge.</param>
        /// <param name="source">Source vertex.</param>
        /// <param name="target">Target vertex.</param>
        /// <returns>True if both <paramref name="source"/> and
        /// <paramref name="target"/> match edge vertices, false otherwise.</returns>
        /// <exception cref="T:System.ArgumentNullException"><paramref name="edge"/> is <see langword="null"/>.</exception>
        /// <exception cref="T:System.ArgumentNullException"><paramref name="source"/> is <see langword="null"/>.</exception>
        /// <exception cref="T:System.ArgumentNullException"><paramref name="target"/> is <see langword="null"/>.</exception>
        [Pure]
        public static bool UndirectedVertexEquality<TVertex>(
            [NotNull] this IEdge<TVertex> edge,
            [NotNull] TVertex source,
            [NotNull] TVertex target)
        {
            if (edge is null)
                throw new ArgumentNullException(nameof(edge));
            if (source == null)
                throw new ArgumentNullException(nameof(source));
            if (target == null)
                throw new ArgumentNullException(nameof(target));

            return UndirectedVertexEqualityInternal(edge, source, target);
        }

        [Pure]
        internal static bool UndirectedVertexEqualityInternal<TVertex>(
            [NotNull] this IEdge<TVertex> edge,
            [NotNull] TVertex source,
            [NotNull] TVertex target)
        {
            Debug.Assert(edge != null);
            Debug.Assert(source != null);
            Debug.Assert(target != null);

            return (EqualityComparer<TVertex>.Default.Equals(edge.Source, source)
                        && EqualityComparer<TVertex>.Default.Equals(edge.Target, target))
                   || (EqualityComparer<TVertex>.Default.Equals(edge.Target, source)
                        && EqualityComparer<TVertex>.Default.Equals(edge.Source, target));
        }

        /// <summary>
        /// Gets a value indicating if the vertices of this edge match
        /// <paramref name="source"/> and <paramref name="target"/> vertices.
        /// </summary>
        /// <typeparam name="TVertex">Vertex type.</typeparam>
        /// <param name="edge">The edge.</param>
        /// <param name="source">Source vertex.</param>
        /// <param name="target">Target vertex.</param>
        /// <returns>True if both <paramref name="source"/> and
        /// <paramref name="target"/> match edge vertices, false otherwise.</returns>
        /// <exception cref="T:System.ArgumentNullException"><paramref name="edge"/> is <see langword="null"/>.</exception>
        /// <exception cref="T:System.ArgumentNullException"><paramref name="source"/> is <see langword="null"/>.</exception>
        /// <exception cref="T:System.ArgumentNullException"><paramref name="target"/> is <see langword="null"/>.</exception>
        [Pure]
        public static bool SortedVertexEquality<TVertex>(
            [NotNull] this IEdge<TVertex> edge,
            [NotNull] TVertex source,
            [NotNull] TVertex target)
        {
            if (edge is null)
                throw new ArgumentNullException(nameof(edge));
            if (source == null)
                throw new ArgumentNullException(nameof(source));
            if (target == null)
                throw new ArgumentNullException(nameof(target));

            return SortedVertexEqualityInternal(edge, source, target);
        }

        [Pure]
        internal static bool SortedVertexEqualityInternal<TVertex>(
            [NotNull] this IEdge<TVertex> edge,
            [NotNull] TVertex source,
            [NotNull] TVertex target)
        {
            Debug.Assert(edge != null);
            Debug.Assert(source != null);
            Debug.Assert(target != null);

            return EqualityComparer<TVertex>.Default.Equals(edge.Source, source)
                && EqualityComparer<TVertex>.Default.Equals(edge.Target, target);
        }

        /// <summary>
        /// Returns an enumeration of reversed edges.
        /// </summary>
        /// <typeparam name="TVertex">Vertex type.</typeparam>
        /// <typeparam name="TEdge">Edge type.</typeparam>
        /// <param name="edges">Edges to reversed.</param>
        /// <returns>Reversed edges.</returns>
        /// <exception cref="T:System.ArgumentNullException"><paramref name="edges"/> is <see langword="null"/>.</exception>
        [Pure]
        [NotNull]
        public static IEnumerable<SReversedEdge<TVertex, TEdge>> ReverseEdges<TVertex, TEdge>(
            [NotNull, ItemNotNull] IEnumerable<TEdge> edges)
            where TEdge : IEdge<TVertex>
        {
            if (edges is null)
                throw new ArgumentNullException(nameof(edges));

            return edges.Select(edge => new SReversedEdge<TVertex, TEdge>(edge));
        }
    }
}