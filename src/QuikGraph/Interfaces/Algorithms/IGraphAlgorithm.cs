namespace QuikGraph.Algorithms
{
    /// <summary> An algorithm that processes an edge tree </summary>
    /// <typeparam name="TVertex">Vertex type.</typeparam>
    /// <typeparam name="TEdge">Edge type.</typeparam>
    public interface IGraphAlgorithm<TVertex, TEdge>
        where TEdge : IEdge<TVertex>
    {
        /// <summary> Reference to the Graph visited </summary>
        IGraph<TVertex, TEdge> VisitededGraph { get; }

    }
}