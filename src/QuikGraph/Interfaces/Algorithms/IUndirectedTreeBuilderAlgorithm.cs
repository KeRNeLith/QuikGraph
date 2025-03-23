namespace QuikGraph.Algorithms
{
    /// <summary>
    /// An algorithm that exposes an event to build an edge tree (in undirected graph).
    /// </summary>
    /// <typeparam name="TVertex">Vertex type.</typeparam>
    /// <typeparam name="TEdge">Edge type.</typeparam>
    public interface IUndirectedTreeBuilderAlgorithm<TVertex, TEdge> : IGraphAlgorithm<TVertex, TEdge>
        where TEdge : IEdge<TVertex>
    {
        /// <summary> Fired when an edge is encountered. </summary>
        event UndirectedEdgeAction<TVertex, TEdge> TreeEdge;
    }
}