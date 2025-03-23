using System.Collections.Generic;
using JetBrains.Annotations;

namespace QuikGraph.Algorithms
{
    /// <summary> An algorithm computing graph connected components (strongly or weakly). </summary>
    public interface IConnectedComponentAlgorithm<TVertex, TEdge, out TGraph> : IAlgorithm<TGraph>
        where TGraph : IGraph<TVertex, TEdge>
        where TEdge : IEdge<TVertex>
    {
        /// <summary> Number of components, i.e. distinct Values of <see cref="ComponentIndex"/>. </summary>
        int ComponentCount { get; }

        /// <summary> 0-based Component-Index for each <typeparamref name="TVertex"/>. </summary>
        [NotNull]
        IDictionary<TVertex, int> ComponentIndex { get; }
    }
}