namespace QuikGraph.Algorithms.MinimumSpanningTree
{
    /// <summary> Represents a minimum spanning tree algorithm. </summary>
    /// <remarks>
    /// Generates an undirected Sub-Graph, that minimizes the Sum of Edge-Weights.
    /// 
    /// Similar to the <see cref="TransitiveReductionAlgorithm{TVertex,TEdge}"/> for directed Graphs,
    /// this creates a reduced Graph to simplify Problems.
    /// </remarks>
    public interface IMinimumSpanningTreeAlgorithm<TVertex, TEdge>
        : IAlgorithm<IUndirectedGraph<TVertex, TEdge>>
        , ITreeBuilderAlgorithm<TVertex, TEdge>
        where TEdge : IEdge<TVertex>
    {
    }
}