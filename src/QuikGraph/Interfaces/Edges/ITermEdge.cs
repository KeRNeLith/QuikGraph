namespace QuikGraph
{
    /// <summary> Directed edge with terminal indexes. </summary>
    public interface ITermEdge<out TVertex> : IEdge<TVertex>
    {
        /// <summary> Index of terminal on source vertex to which this edge is attached. </summary>
        int SourceTerminal { get; }

        /// <summary> Index of terminal on target vertex to which this edge is attached. </summary>
        int TargetTerminal { get; }
    }
}