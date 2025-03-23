namespace QuikGraph.Algorithms
{
    /// <summary> An algorithm that exposes an event to build an edge tree. </summary>
    public interface ITreeBuilderAlgorithm<TVertex, TEdge> : IGraphAlgorithm<TVertex, TEdge>
        where TEdge : IEdge<TVertex>
    {
        /// <summary> Fired when an edge is encountered. </summary>
        event EdgeAction<TVertex, TEdge> TreeEdge;
    }
}