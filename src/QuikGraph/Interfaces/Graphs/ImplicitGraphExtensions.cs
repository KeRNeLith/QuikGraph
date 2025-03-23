using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using QuikGraph.Collections;

namespace QuikGraph
{
    /// <summary> Extension Methods on <see cref="IImplicitGraph{TVertex,TEdge}"/> </summary>
    public static class ImplicitGraphExtensions
    {
        private static readonly Dictionary<Type, Array> EmptyArrays = new Dictionary<Type, Array>();

        /// <summary> Returns whether the <paramref name="items"/> is null or empty </summary>
        public static bool IsNullOrEmpty<T>([CanBeNull] this IEnumerable<T> items)
            => items is null || (items is IReadOnlyCollection<T> coll ? coll.Count <= 0 : !items.Any());

        /// <summary> Returns an empty Enumeration when the <paramref name="items"/> is null </summary>
        public static IEnumerable<T> AsNotNull<T>([CanBeNull] this IEnumerable<T> items) => items ?? Enumerable.Empty<T>();

        /// <summary> Returns an empty Array when the <paramref name="items"/> is null </summary>
        public static EdgeList<T> AsList<T>([CanBeNull] this IEnumerable<T> items) => new EdgeList<T>(items);

        /// <summary> Returns an empty Array when the <paramref name="items"/> is null </summary>
        public static T[] AsArray<T>([CanBeNull] this IEnumerable<T> items)
        {
            if (!(items is null) && (!(items is IReadOnlyCollection<T> coll) || coll.Count > 0)) //.IsNullOrEmpty())
            {
                return items.ToArray();
            }

            if (EmptyArrays.TryGetValue(typeof(T), out Array ret))
            {
                return (T[])ret;
            }

            var array = new T[0];
            EmptyArrays[typeof(T)] = array;
            return array;

        }

        /// <summary> Determines whether <paramref name="vertex"/> has no out-edges. </summary>
        /// <returns>True if <paramref name="vertex"/> has no out-edges, false otherwise.</returns>
        [Pure]
        public static bool? IsOutEdgesEmpty<TVertex, TEdge>(this IImplicitGraph<TVertex, TEdge> graph
            , TVertex vertex) where TEdge : IEdge<TVertex> => !graph.OutEdges(vertex)?.Any(); //OutDegree(vertex) == 0;

        /// <summary> Flag that the given <paramref name="vertex"/> has no adjacent edge. </summary>
        /// <returns><see langword="null"/> when the <paramref name="vertex"/> is unknown to the <paramref name="graph"/></returns>
        [Pure]
        public static bool? IsAdjacentEdgesEmpty<TVertex, TEdge>(this IImplicitUndirectedGraph<TVertex, TEdge> graph
            , TVertex vertex) where TEdge : IEdge<TVertex> => !graph.AdjacentEdges(vertex)?.Any();

        /// <summary> Determines whether <paramref name="vertex"/> has no in-edges. </summary>
        /// <returns>True if <paramref name="vertex"/> has no in-edges, false otherwise.</returns>
        /// <exception cref="T:System.ArgumentNullException"><paramref name="vertex"/> is <see langword="null"/>.</exception>
        /// <exception cref="VertexNotFoundException"><paramref name="vertex"/> is not part of the graph.</exception>
        [Pure]
        public static bool? IsInEdgesEmpty<TVertex, TEdge>(this IBidirectionalIncidenceGraph<TVertex, TEdge> graph
            , TVertex vertex) where TEdge : IEdge<TVertex> => !graph.InEdges(vertex)?.Any(); //InDegree(vertex) == 0;
    }
}