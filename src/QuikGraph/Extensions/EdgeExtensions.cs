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
    /// <summary> Extensions related to graph edges. </summary>
    public static class EdgeExtensions
    {
        /// <summary>
        /// Gets a value indicating if the edge is a self edge.
        /// </summary>
        /// <typeparam name="TVertex">Vertex type.</typeparam>
        /// <returns>True if edge is a self one, false otherwise.</returns>
        /// <exception cref="T:System.ArgumentNullException"><paramref name="edge"/> is <see langword="null"/>.</exception>
        [Pure]
        public static bool IsSelfEdge<TVertex>([NotNull] this IEdge<TVertex> edge
            , Func<TVertex, TVertex, bool> areVerticesEqual = null)
        {
            if (edge is null)
                throw new ArgumentNullException(nameof(edge));

            return (areVerticesEqual ?? EqualityComparer<TVertex>.Default.Equals).Invoke(edge.Source, edge.Target);
        }

        /// <summary>
        /// Given a <paramref name="vertex"/>, returns the other vertex in the edge.
        /// </summary>
        /// <typeparam name="TVertex">Vertex type.</typeparam>
        /// <returns>The other edge vertex.</returns>
        /// <exception cref="T:System.ArgumentNullException"><paramref name="edge"/> is <see langword="null"/>.</exception>
        /// <exception cref="T:System.ArgumentNullException"><paramref name="vertex"/> is <see langword="null"/>.</exception>
        [Pure]
        [NotNull]
        public static TVertex GetOtherVertex<TVertex>([NotNull] this IEdge<TVertex> edge
            , [NotNull] TVertex vertex, Func<TVertex, TVertex, bool> areVerticesEqual = null)
        {
            if (edge is null)
                throw new ArgumentNullException(nameof(edge));
            if (vertex == null)
                throw new ArgumentNullException(nameof(vertex));

            return (areVerticesEqual ?? EqualityComparer<TVertex>.Default.Equals)
                .Invoke(edge.Source, vertex) ? edge.Target : edge.Source;
        }

        /// <summary>
        /// Gets a value indicating if the <paramref name="vertex"/> is adjacent to the
        /// <paramref name="edge"/> (is the source or target).
        /// </summary>
        /// <typeparam name="TVertex">Vertex type.</typeparam>
        /// <returns>True if the <paramref name="vertex"/> is adjacent to this <paramref name="edge"/>, false otherwise.</returns>
        /// <exception cref="T:System.ArgumentNullException"><paramref name="edge"/> is <see langword="null"/>.</exception>
        /// <exception cref="T:System.ArgumentNullException"><paramref name="vertex"/> is <see langword="null"/>.</exception>
        [Pure]
        public static bool IsAdjacent<TVertex>([NotNull] this IEdge<TVertex> edge
            , [NotNull] TVertex vertex, Func<TVertex, TVertex, bool> areVerticesEqual = null)
        {
            if (edge is null)
                throw new ArgumentNullException(nameof(edge));
            if (vertex == null)
                throw new ArgumentNullException(nameof(vertex));

            areVerticesEqual = areVerticesEqual ?? EqualityComparer<TVertex>.Default.Equals;
            return areVerticesEqual(edge.Source, vertex)
                || areVerticesEqual(edge.Target, vertex);
        }

        /// <summary> Checks if the <paramref name="edges"/> form a Circuit/Cycle. </summary>
        /// <returns>True if the set makes a complete path, false otherwise.</returns>
        /// <exception cref="T:System.ArgumentNullException"><paramref name="edges"/> is <see langword="null"/>.</exception>
        [Pure]
        public static bool IsCircuit<TVertex>([NotNull, ItemNotNull] this IEnumerable<IEdge<TVertex>> edges
            , [CanBeNull] Func<TVertex, TVertex, bool> areEqual = null) => IsCircuit(edges.GetEnumerator(), areEqual);

        /// <inheritdoc cref="IsCircuit{TVertex}(IEnumerable{IEdge{TVertex}},Func{TVertex,TVertex,bool})"/>
        [Pure]
        public static bool IsCircuit<TVertex>([NotNull] this IEnumerator<IEdge<TVertex>> edges
            , [CanBeNull] Func<TVertex, TVertex, bool> areEqual = null)
        {
            if (!edges.MoveNext())
            {
                return false; // could also report this as null
            }

            var firstEdge = edges.Current;
            var lastEdge = _IsPath(edges, firstEdge, areEqual);
            if (lastEdge == null)
            {
                return false;
            }

            return (areEqual ?? EqualityComparer<TVertex>.Default.Equals).Invoke(lastEdge.Target, firstEdge.Source);
        }

        /// <summary> Checks if the <paramref name="edges"/> form a path. </summary>
        /// <returns>True if the set makes a complete path, false otherwise.</returns>
        /// <exception cref="T:System.ArgumentNullException"><paramref name="edges"/> is <see langword="null"/>.</exception>
        [Pure]
        public static bool IsPath<TVertex>([NotNull, ItemNotNull] this IEnumerable<IEdge<TVertex>> edges
            , [CanBeNull] Func<TVertex, TVertex, bool> areEqual = null)
            => IsPath(edges.GetEnumerator(), areEqual);

        /// <summary> Checks if the <paramref name="edges"/> form a path. </summary>
        /// <returns>True if the set makes a complete path, false otherwise.</returns>
        /// <exception cref="T:System.ArgumentNullException"><paramref name="edges"/> is <see langword="null"/>.</exception>
        [Pure]
        public static bool IsPath<TVertex>([NotNull] this IEnumerator<IEdge<TVertex>> edges
            , [CanBeNull] Func<TVertex, TVertex, bool> areEqual = null) => _IsPath(edges, areEqual) != null;

        /// <summary> Checks if the <paramref name="edges"/> form a path. </summary>
        /// <returns>
        /// The last Edge if the set makes a complete path, null otherwise.
        /// You can check the <paramref name="edges"/> for the offending Edge if necessary.
        /// </returns>
        /// <exception cref="T:System.ArgumentNullException"><paramref name="edges"/> is <see langword="null"/>.</exception>
        [Pure]
        static IEdge<TVertex> _IsPath<TVertex>([NotNull] this IEnumerator<IEdge<TVertex>> edges
            , [CanBeNull] Func<TVertex, TVertex, bool> areEqual = null)
        {
            if (!edges.MoveNext())
            {
                return null; // could report this as null or as true
            }

            var lastEdge = edges.Current;
            return _IsPath(edges, lastEdge, areEqual);
        }

        private static IEdge<TVertex> _IsPath<TVertex>(IEnumerator<IEdge<TVertex>> edges, IEdge<TVertex> lastEdge
            , [CanBeNull] Func<TVertex, TVertex, bool> areEqual = null)
        {
            areEqual = areEqual ?? EqualityComparer<TVertex>.Default.Equals;
            for (; edges.MoveNext()
                 ; lastEdge = edges.Current)
            {
                if (!areEqual(lastEdge.Target, edges.Current.Source))
                {
                    return null;
                }
            }

            return lastEdge;
        }

        /// <summary> Checks if the <paramref name="path"/> makes a cycle. </summary>
        /// <remarks>Note that this function only work when given a path.</remarks>
        /// <returns>True if the set makes a cycle, false otherwise.</returns>
        /// <exception cref="T:System.ArgumentNullException"><paramref name="path"/> is <see langword="null"/>.</exception>
        [Pure]
        public static bool HasCycles<TVertex>([NotNull, ItemNotNull] this IEnumerable<IEdge<TVertex>> path
            , Func<TVertex, TVertex, bool> areVerticesEqual = null)
        {
            if (path is null)
                throw new ArgumentNullException(nameof(path));

            areVerticesEqual = areVerticesEqual ?? EqualityComparer<TVertex>.Default.Equals;

            var vertices = new Dictionary<TVertex, int>();
            bool first = true;
            foreach (var edge in path)
            {
                if (first)
                {
                    if (edge.IsSelfEdge(areVerticesEqual))
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
        /// Checks if this <paramref name="path"/> of edges does not make a cycle.
        /// </summary>
        /// <returns>True if the path makes a cycle, false otherwise.</returns>
        /// <exception cref="T:System.ArgumentNullException"><paramref name="path"/> is <see langword="null"/>.</exception>
        [Pure]
        public static bool IsPathWithoutCycles<TVertex>([NotNull, ItemNotNull] this IEnumerable<IEdge<TVertex>> path
            , Func<TVertex, TVertex, bool> areVerticesEqual = null)
        {
            if (path is null)
                throw new ArgumentNullException(nameof(path));

            areVerticesEqual = areVerticesEqual ?? EqualityComparer<TVertex>.Default.Equals;

            var vertices = new Dictionary<TVertex, int>();
            bool first = true;
            var lastTarget = default(TVertex);
            foreach (var edge in path)
            {
                if (first)
                {
                    lastTarget = edge.Target;
                    if (edge.IsSelfEdge(areVerticesEqual))
                        return false;
                    vertices.Add(edge.Source, 0);
                    vertices.Add(lastTarget, 0);
                    first = false;
                }
                else
                {
                    if (!areVerticesEqual(lastTarget, edge.Source))
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

        /// <summary> Casts to read only  </summary>
        [Pure]
        [NotNull]
        public static IReadOnlyDictionary<TVertex, TV> AsReadOnly<TVertex, TV>(
            [NotNull] this IDictionary<TVertex, TV> predecessors) => (IReadOnlyDictionary<TVertex, TV>)predecessors;

#if NET35 || NET40
        /// <inheritdoc cref="IsPredecessor{TVertex}(IDictionary{TVertex, IEdge{TVertex}}, TVertex, TVertex, Func{TVertex, TVertex, bool})"/>
        public static bool IsPredecessor<TVertex, TEdge>(
        [NotNull] this IDictionary<TVertex, TEdge> predecessors,
        [NotNull] TVertex root,
        [NotNull] TVertex vertex, Func<TVertex, TVertex, bool> areVerticesEqual = null) where TEdge : IEdge<TVertex>
            => ((IDictionary<TVertex, IEdge<TVertex>>)predecessors).IsPredecessor(root, vertex, areVerticesEqual);
#else
        /// <inheritdoc cref="IsPredecessor{TVertex}(IReadOnlyDictionary{TVertex, IEdge{TVertex}}, TVertex, TVertex, Func{TVertex, TVertex, bool})"/>
        public static bool IsPredecessor<TVertex, TEdge>(
            [NotNull] this IDictionary<TVertex, TEdge> predecessors,
            [NotNull] TVertex root,
            [NotNull] TVertex vertex, Func<TVertex, TVertex, bool> areVerticesEqual = null) where TEdge : IEdge<TVertex>
            => ((IReadOnlyDictionary<TVertex, IEdge<TVertex>>)predecessors).IsPredecessor(root, vertex, areVerticesEqual);
#endif //NET35 || NET40

        ///// <inheritdoc cref="IsPredecessor{TVertex}(IReadOnlyDictionary{TVertex,IEdge{TVertex}},TVertex,TVertex)"/>
        //public static bool IsPredecessor<TVertex>(
        //    [NotNull] this IDictionary<TVertex, IEdge<TVertex>> predecessors,
        //    [NotNull] TVertex root,
        //    [NotNull] TVertex vertex) =>
        //    ((IReadOnlyDictionary<TVertex, IEdge<TVertex>>)predecessors).IsPredecessor(root, vertex);

        /// <summary> Checks that the <paramref name="root"/> is a predecessor of the given <paramref name="vertex"/>. </summary>
        /// <returns>True if the <paramref name="root"/> is a predecessor of the <paramref name="vertex"/>.</returns>
        /// <exception cref="T:System.ArgumentNullException"><paramref name="predecessors"/> is <see langword="null"/>.</exception>
        /// <exception cref="T:System.ArgumentNullException"><paramref name="root"/> is <see langword="null"/>.</exception>
        /// <exception cref="T:System.ArgumentNullException"><paramref name="vertex"/> is <see langword="null"/>.</exception>
        [Pure]
        public static bool IsPredecessor<TVertex>(
#if NET35 || NET40
            [NotNull] this IDictionary<TVertex, IEdge<TVertex>> predecessors,
#else
            [NotNull] this IReadOnlyDictionary<TVertex, IEdge<TVertex>> predecessors,
#endif //NET35 || NET40
            [NotNull] TVertex root,
            [NotNull] TVertex vertex, Func<TVertex, TVertex, bool> areVerticesEqual)
        {
            if (predecessors is null)
                throw new ArgumentNullException(nameof(predecessors));
            if (root == null)
                throw new ArgumentNullException(nameof(root));
            if (vertex == null)
                throw new ArgumentNullException(nameof(vertex));

            areVerticesEqual = areVerticesEqual ?? EqualityComparer<TVertex>.Default.Equals;

            TVertex currentVertex = vertex;
            if (areVerticesEqual(root, currentVertex))
                return true;

            while (predecessors.TryGetValue(currentVertex, out var predecessor))
            {
                TVertex source = predecessor.GetOtherVertex(currentVertex, areVerticesEqual);
                if (areVerticesEqual(currentVertex, source))
                    return false;
                if (areVerticesEqual(source, root))
                    return true;
                currentVertex = source;
            }

            return false;
        }

        /// <summary> Tries to get the predecessor path, if reachable. </summary>
        /// <returns>Path to the ending vertex, if a path was found, null otherwise.</returns>
        /// <exception cref="T:System.ArgumentNullException"><paramref name="predecessors"/> is <see langword="null"/>.</exception>
        /// <exception cref="T:System.ArgumentNullException"><paramref name="vertex"/> is <see langword="null"/>.</exception>
        [Pure]
        public static List<TEdge> GetPath<TVertex, TEdge>(
            [NotNull] this IDictionary<TVertex, TEdge> predecessors,
            [NotNull] TVertex vertex, Func<TVertex, TVertex, bool> areVerticesEqual = null)
            where TEdge : IEdge<TVertex>
        {
            if (predecessors is null)
                throw new ArgumentNullException(nameof(predecessors));
            if (vertex == null)
                throw new ArgumentNullException(nameof(vertex));

            areVerticesEqual = areVerticesEqual ?? EqualityComparer<TVertex>.Default.Equals;
            var computedPath = new List<TEdge>();

            TVertex currentVertex = vertex;
            while (predecessors.TryGetValue(currentVertex, out TEdge edge))
            {
                if (edge.IsSelfEdge(areVerticesEqual))
                    break;

                computedPath.Add(edge);
                currentVertex = GetOtherVertex(edge, currentVertex, areVerticesEqual);
            }

            if (computedPath.Count > 0)
            {
                computedPath.Reverse();
                return computedPath;
            }

            return null;
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
        /// <returns>True if both <paramref name="source"/> and
        /// <paramref name="target"/> match edge vertices, false otherwise.</returns>
        /// <exception cref="T:System.ArgumentNullException"><paramref name="edge"/> is <see langword="null"/>.</exception>
        /// <exception cref="T:System.ArgumentNullException"><paramref name="source"/> is <see langword="null"/>.</exception>
        /// <exception cref="T:System.ArgumentNullException"><paramref name="target"/> is <see langword="null"/>.</exception>
        [Pure]
        public static bool UndirectedVertexEquality<TVertex>(
            [NotNull] this IEdge<TVertex> edge,
            [NotNull] TVertex source,
            [NotNull] TVertex target) => UndirectedVertexEquality(edge, source, target, EqualityComparer<TVertex>.Default.Equals);

        /// <inheritdoc cref="UndirectedVertexEquality{TVertex}(IEdge{TVertex}, TVertex, TVertex)"/>
        [Pure]
        public static bool UndirectedVertexEquality<TVertex>(
            [NotNull] this IEdge<TVertex> edge,
            [NotNull] TVertex source,
            [NotNull] TVertex target, Func<TVertex, TVertex, bool> areVerticesEqual)
        {
            if (edge is null)
                throw new ArgumentNullException(nameof(edge));
            if (source == null)
                throw new ArgumentNullException(nameof(source));
            if (target == null)
                throw new ArgumentNullException(nameof(target));

            return UndirectedVertexEqualityInternal(edge, source, target, areVerticesEqual);
        }

        [Pure]
        internal static bool UndirectedVertexEqualityInternal<TVertex>(
            [NotNull] this IEdge<TVertex> edge,
            [NotNull] TVertex source,
            [NotNull] TVertex target, Func<TVertex, TVertex, bool> areVerticesEqual)
        {
            Debug.Assert(edge != null);
            Debug.Assert(source != null);
            Debug.Assert(target != null);

            return (areVerticesEqual(edge.Source, source)
                        && areVerticesEqual(edge.Target, target))
                   || (areVerticesEqual(edge.Target, source)
                        && areVerticesEqual(edge.Source, target));
        }

        /// <summary>
        /// Indicates if the vertices of this edge match both
        /// <paramref name="source"/> and <paramref name="target"/> vertices.
        /// </summary>
        /// <returns>True if both <paramref name="source"/> and
        /// <paramref name="target"/> match edge vertices, false otherwise.</returns>
        /// <exception cref="T:System.ArgumentNullException"><paramref name="edge"/> is <see langword="null"/>.</exception>
        /// <exception cref="T:System.ArgumentNullException"><paramref name="source"/> is <see langword="null"/>.</exception>
        /// <exception cref="T:System.ArgumentNullException"><paramref name="target"/> is <see langword="null"/>.</exception>
        [Pure]
        public static bool SortedVertexEquality<TVertex>(
            [NotNull] this IEdge<TVertex> edge,
            [NotNull] TVertex source,
            [NotNull] TVertex target) => SortedVertexEquality(edge, source, target, EqualityComparer<TVertex>.Default.Equals);

        /// <inheritdoc cref="SortedVertexEquality{TVertex}(IEdge{TVertex}, TVertex, TVertex)"/>
        [Pure]
        public static bool SortedVertexEquality<TVertex>(
            [NotNull] this IEdge<TVertex> edge,
            [NotNull] TVertex source,
            [NotNull] TVertex target, Func<TVertex, TVertex, bool> areVerticesEqual)
        {
            if (edge is null)
                throw new ArgumentNullException(nameof(edge));
            if (source == null)
                throw new ArgumentNullException(nameof(source));
            if (target == null)
                throw new ArgumentNullException(nameof(target));

            return SortedVertexEqualityInternal(edge, source, target, areVerticesEqual);
        }

        [Pure]
        internal static bool SortedVertexEqualityInternal<TVertex>(
            [NotNull] this IEdge<TVertex> edge,
            [NotNull] TVertex source,
            [NotNull] TVertex target, Func<TVertex, TVertex, bool> areVerticesEqual)
        {
            Debug.Assert(edge != null);
            Debug.Assert(source != null);
            Debug.Assert(target != null);

            return areVerticesEqual(edge.Source, source)
                && areVerticesEqual(edge.Target, target);
        }

        /// <summary> Returns the distinct Set of Vertices for the <paramref name="edges"/> </summary>
        public static HashSet<TVertex> GetVertices<TVertex, TEdge>([NotNull][ItemNotNull] this IEnumerable<TEdge> edges)
            where TEdge : IEdge<TVertex>
        {
            var adjacentVertices = new HashSet<TVertex>();
            foreach (TEdge edge in edges)
            {
                adjacentVertices.Add(edge.Source);
                adjacentVertices.Add(edge.Target);
            }

            return adjacentVertices;
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
            [NotNull, ItemNotNull] this IEnumerable<TEdge> edges)
            where TEdge : IEdge<TVertex>
        {
            if (edges is null)
                throw new ArgumentNullException(nameof(edges));

            return edges.Select(edge => new SReversedEdge<TVertex, TEdge>(edge));
        }
    }
}
