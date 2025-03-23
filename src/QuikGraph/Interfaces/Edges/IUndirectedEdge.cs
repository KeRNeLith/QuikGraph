namespace QuikGraph
{
    /// <summary> Represents an undirected edge. </summary>
    /// <remarks>
    /// Invariant: to be unique and canonical, <see cref="IEdge{TVertex}.Source"/> must be less or equal to <see cref="IEdge{TVertex}.Target"/> (using the default comparer).
    /// </remarks>
    public interface IUndirectedEdge<out TVertex> : IEdge<TVertex>
    {
    }
}