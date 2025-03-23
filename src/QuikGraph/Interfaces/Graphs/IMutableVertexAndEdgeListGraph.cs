using QuikGraph.Algorithms;
using System.Collections.Generic;

namespace QuikGraph
{
    /// <summary>
    /// A mutable vertex and edge list graph with vertices of type
    /// <typeparamref name="TVertex"/> and edges of type <typeparamref name="TEdge"/>.
    /// </summary>
    /// <typeparam name="TVertex">Vertex type.</typeparam>
    /// <typeparam name="TEdge">Edge type.</typeparam>
    public interface IMutableVertexAndEdgeListGraph<TVertex, TEdge>
        : IMutableVertexListGraph<TVertex, TEdge>
        , IMutableVertexAndEdgeSet<TVertex, TEdge>
        , IVertexAndEdgeListGraph<TVertex, TEdge>
        where TEdge : IEdge<TVertex>
    {
    }

    /// <summary> Extension Methods for <see cref="IMutableVertexAndEdgeListGraph{TVertex,TEdge}"/> </summary>
    public static class MutableVertexAndEdgeListGraphX
    {
        /// <summary> Filters those <paramref name="edges"/> that would not create a cycle in <paramref name="graph"/> </summary>
        public static IEnumerable<TEdge> EdgesWithoutCycles<TVertex, TEdge>(this IMutableVertexAndEdgeListGraph<TVertex, TEdge> graph
            , IEnumerable<TEdge> edges) where TEdge : IEdge<TVertex>
        {
            foreach (TEdge edge in edges)
            {
                graph.AddEdge(edge);
                if (!graph.IsDirectedAcyclicGraph())
                {
                    yield return edge;
                }

                graph.RemoveEdge(edge);
            }
        }
    }
}