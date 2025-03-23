namespace QuikGraph.Algorithms
{
    /// <summary> An algorithm that exposes events to compute timing with vertices treatment. </summary>
    /// <typeparam name="TVertex">Vertex type.</typeparam>
    public interface IVertexTimeStamperAlgorithm<out TVertex>
    {
        /// <summary>
        /// Fired when a vertex is discovered.
        /// </summary>
        event VertexAction<TVertex> DiscoverVertex;

        /// <summary>
        /// Fired when a vertex is fully treated.
        /// </summary>
        event VertexAction<TVertex> FinishVertex;
    }
}