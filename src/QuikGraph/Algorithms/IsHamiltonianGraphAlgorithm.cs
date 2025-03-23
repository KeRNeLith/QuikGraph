using System.Linq;
using System.Collections.Generic;
using JetBrains.Annotations;

namespace QuikGraph.Algorithms
{
    /// <summary> Algorithm that checks if a graph is Hamiltonian </summary>
    /// <remarks>
    /// (has a path that links all vertices and pass one and only one time by each vertex).
    /// </remarks>
    public static class IsHamiltonianGraphAlgorithm
    {
        /// <summary> Creates a new <see cref="IsHamiltonianGraphAlgorithm"/> </summary>
        public static IsHamiltonianGraphAlgorithm<TVertex, TEdge> CreateHamiltonianGraphAlgorithm<TVertex
            , TEdge>(this IUndirectedGraph<TVertex, TEdge> graph)
            where TEdge : IUndirectedEdge<TVertex>
            => new IsHamiltonianGraphAlgorithm<TVertex, TEdge>(graph);

        /// <inheritdoc cref="GraphExtensions.IsSimpleAndHamiltonian{TVertex,TEdge}"/>
        [Pure]
        public static bool IsHamiltonian<TVertex, TEdge>(
            [NotNull] this IUndirectedGraph<TVertex, TEdge> graph)
            where TEdge : IUndirectedEdge<TVertex>
            => CreateHamiltonianGraphAlgorithm(graph).IsHamiltonian();
    }

    /// <inheritdoc cref="IsHamiltonianGraphAlgorithm{TVertex, TEdge}(IUndirectedGraph{TVertex, TEdge})"/>
    public class IsHamiltonianGraphAlgorithm<TVertex, TEdge>
        where TEdge : IUndirectedEdge<TVertex>
    {
        [NotNull]
        private readonly UndirectedGraph<TVertex, TEdge> _simpleGraph;

        /// <summary> checks if the <see cref="_simpleGraph"/> is Hamiltonian
        /// (has a path that links all vertices and pass one and only one time by each vertex).
        /// </summary>
        /// <exception cref="T:System.ArgumentNullException"><paramref name="graph"/> is <see langword="null"/>.</exception>
        internal IsHamiltonianGraphAlgorithm([NotNull] IUndirectedGraph<TVertex, TEdge> graph)
            => _simpleGraph = graph.RemoveParallelAndSelfEdges();

        /// <summary> Gets all vertices permutations. </summary>
        [Pure]
        [NotNull, ItemNotNull]
        public List<List<TVertex>> GetAllVertexPermutations() => _simpleGraph.Vertices.ToList().GetAllPermutations();

        /// <inheritdoc cref="GraphExtensions.IsSimpleAndHamiltonian{TVertex,TEdge}"/>
        public bool IsHamiltonian() => _simpleGraph.IsSimpleAndHamiltonian();
    }
}
