namespace QuikGraph
{
    /// <summary>
    /// A graph with vertices of type <typeparamref name="TVertex"/>
    /// and edges of type <typeparamref name="TEdge"/>.
    /// </summary>
    /// <typeparam name="TVertex">Vertex type.</typeparam>
    /// <typeparam name="TEdge">Edge type.</typeparam>
    // ReSharper disable once UnusedTypeParameter
    public interface IGraph<TVertex, TEdge> : IImplicitVertexSet<TVertex>
        where TEdge : IEdge<TVertex>
    {
        /// <summary> Flag if the graph is directed </summary>
        bool IsDirected { get; }

        /// <summary> Flag if the graph allows parallel edges </summary>
        bool AllowParallelEdges { get; }
    }
}