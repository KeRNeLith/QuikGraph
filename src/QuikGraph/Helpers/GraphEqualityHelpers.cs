using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;

namespace QuikGraph
{
    /// <inheritdoc cref="IsEqualTo"/>
    public static class EquateGraphs
    {
        /// <summary> Checks if both graphs <paramref name="g"/> and <paramref name="h"/> content are equal. </summary>
        /// <remarks>
        /// Uses the provided <paramref name="vertexEquality"/> and <paramref name="edgeEquality"/>
        /// comparer or <see cref="EqualityComparer{T}.Default"/> to respectively compare vertices and edges.
        ///
        /// O(E²+V²), used only for Unit Tests
        /// </remarks>
        [Pure]
        public static bool IsEqualTo<TVertex, TEdge>(
            [CanBeNull] this IEdgeListGraph<TVertex, TEdge> g,
            [CanBeNull] IEdgeListGraph<TVertex, TEdge> h,
            [CanBeNull] IEqualityComparer<TVertex> vertexEquality = null,
            [CanBeNull] IEqualityComparer<TEdge> edgeEquality = null)
            where TEdge : IEdge<TVertex>
        {
            if (vertexEquality is null) vertexEquality = EqualityComparer<TVertex>.Default;
            if (edgeEquality is null) edgeEquality = EqualityComparer<TEdge>.Default;

            if (ReferenceEquals(g, h))
                return true;

            if (g is null || h is null)
                return false;

            if (g.VertexCount != h.VertexCount)
                return false;

            if (g.IsDirected != h.IsDirected)
                return false;

            if (g.EdgeCount != h.EdgeCount)
                return false;

            foreach (TVertex vertex in g.Vertices)
            {
                if (!h.Vertices.Any(v => vertexEquality.Equals(v, vertex)))
                    return false;
            }

            foreach (TEdge edge in g.Edges)
            {
                if (!h.Edges.Any(e => edgeEquality.Equals(e, edge)))
                    return false;
            }

            return true;
        }

    }
}