using System.Linq;
using JetBrains.Annotations;
using QuikGraph.Algorithms.ConnectedComponents;

namespace QuikGraph.Algorithms
{
    /// <inheritdoc cref="IsEulerian{TVertex,TEdge}"/>
    public static class IsEulerianGraphAlgorithm
    {
        /// <summary> Creates a new <see cref="IsEulerianGraphAlgorithm{TVertex,TEdge}"/> </summary>
        public static IsEulerianGraphAlgorithm<TVertex, TEdge> CreateEulerianGraphAlgorithm<TVertex, TEdge>(
            this IUndirectedGraph<TVertex, TEdge> graph)
            where TEdge : IUndirectedEdge<TVertex> => new IsEulerianGraphAlgorithm<TVertex, TEdge>(graph);

        /// <summary> Returns true if the <paramref name="graph"/> is Eulerian, otherwise false. </summary>
        /// <inheritdoc cref="IsEulerianGraphAlgorithm{TVertex,TEdge}.IsEulerian"/>
        [Pure]
        public static bool IsEulerian<TVertex, TEdge>(
            [NotNull] this IUndirectedGraph<TVertex, TEdge> graph)
            where TEdge : IUndirectedEdge<TVertex> => CreateEulerianGraphAlgorithm(graph).IsEulerian();
    }

    /// <inheritdoc cref="IsEulerianGraphAlgorithm{TVertex, TEdge}(IUndirectedGraph{TVertex, TEdge})"/>
    public class IsEulerianGraphAlgorithm<TVertex, TEdge>
        where TEdge : IUndirectedEdge<TVertex>
    {
        [NotNull]
        private readonly UndirectedGraph<TVertex, TEdge> _simpleGraph;

        /// <summary> Algorithm that checks if the undirected <see cref="_simpleGraph"/> is Eulerian.
        /// (i.e. has a path using all edges one and only one time).
        /// </summary>
        /// <param name="graph">Graph to check; is not modified, but copied.</param>
        /// <exception cref="T:System.ArgumentNullException"><paramref name="graph"/> is <see langword="null"/>.</exception>
        internal IsEulerianGraphAlgorithm([NotNull] IUndirectedGraph<TVertex, TEdge> graph)
            => _simpleGraph = graph.RemoveParallelAndSelfEdges();

        /// <summary> Gets the components except for single Nodes in the current graph. </summary>
        [Pure]
        public int[] NumVerticesInComponent()
        {
            var undirectedGraph = _simpleGraph;
            var connectedComponentsAlgorithm = undirectedGraph.CreateConnectedComponentsAlgorithm();
            connectedComponentsAlgorithm.Compute();

            return connectedComponentsAlgorithm.NumVerticesInComponent();
        }

        [Pure]
        private bool HasEvenDegree([NotNull] TVertex vertex) => _simpleGraph.AdjacentDegree(vertex) % 2 == 0;

        /// <summary> A graph is an Eulerian circuit,
        /// if it has a single Component, where each Vertex <see cref="HasEvenDegree"/>. </summary>
        /// <returns>True if the graph is Eulerian, false otherwise.</returns>
        [Pure]
        public bool IsEulerian()
        {
            if (!_simpleGraph.Vertices.All(HasEvenDegree))
            {
                return false;
            }
            var components = NumVerticesInComponent().Where(num => num > 1).Take(2).ToList();
            switch (components.Count)
            {
                case 0: return _simpleGraph.VertexCount == 1;
                case 1: return true;
                default: return false; // Many components
            }
        }
    }
}