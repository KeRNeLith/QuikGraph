namespace QuikGraph
{
    /// <summary> Directed graph with efficiently enumerable <typeparamref name="TVertex"/> and <typeparamref name="TEdge"/>. </summary>
    /// <remarks>
    /// Unifies the vertex and edge list graph concepts.
    /// </remarks>
    public interface IVertexAndEdgeListGraph<TVertex, TEdge> 
        : IVertexListGraph<TVertex, TEdge>
        , IEdgeListGraph<TVertex, TEdge>
        where TEdge : IEdge<TVertex>
    {
    }
}