using System.Collections.Generic;
using JetBrains.Annotations;

namespace QuikGraph.Tests.Algorithms
{
    /// <summary>
    /// Class containing information needed for building graphs when testing contracts.
    /// </summary>
    internal sealed class ContractScenario
    {
        /// <summary>
        /// Edges in the graph. These should be converted to compatible edges in the constructor for the graph
        /// and both edges and vertices should be added.
        /// </summary>
        [NotNull, ItemNotNull]
        public IEnumerable<Edge<int>> EdgesInGraph { get; set; } = new Edge<int>[0];

        /// <summary>
        /// Vertices not connected to any other vertices.
        /// </summary>
        [NotNull]
        public IEnumerable<int> SingleVerticesInGraph { get; set; } = new int[0];

        /// <summary>
        /// The vertex that will be used as root vertex in the test.
        /// </summary>
        public int Root { get; set; } = -1;

        /// <summary>
        /// Vertices expected to be accessible from the root, not including the root itself.
        /// </summary>
        [NotNull]
        public IEnumerable<int> AccessibleVerticesFromRoot { get; set; } = new int[0];

        /// <summary>
        /// Flag indicating whether the algorithm should be computed after it has been instantiated.
        /// </summary>
        public bool DoComputation { get; set; } = true;
    }
}