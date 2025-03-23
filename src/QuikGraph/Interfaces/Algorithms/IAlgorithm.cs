using JetBrains.Annotations;

namespace QuikGraph.Algorithms
{
    /// <summary> An algorithm to run on the <see cref="VisitedGraph"/>. </summary>
    public interface IAlgorithm<out TGraph> : IComputation
    {
        /// <summary> The graph to visit/transform with this algorithm. </summary>
        [NotNull]
        TGraph VisitedGraph { get; }
    }
}