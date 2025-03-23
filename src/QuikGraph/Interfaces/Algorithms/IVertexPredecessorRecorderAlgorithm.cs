namespace QuikGraph.Algorithms
{
    /// <summary> An algorithm that exposes events to compute vertices predecessors (in directed graph). </summary>
    public interface IVertexPredecessorRecorderAlgorithm<TVertex, TEdge>
        : ITreeBuilderAlgorithm<TVertex, TEdge>
        where TEdge : IEdge<TVertex>
    {
        /// <summary> Fired on a starting vertex once before the start of the search from it. </summary>
        event VertexAction<TVertex> StartVertex;

        /// <summary> Fired when a vertex is fully treated. </summary>
        event VertexAction<TVertex> FinishVertex;
    }
}