using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using JetBrains.Annotations;

namespace QuikGraph.Algorithms.VertexColoring
{
    /// <summary>
    /// Algorithm that walk through a graph and greedy color vertices. 
    /// </summary>
    /// <typeparam name="TVertex">Vertex type.</typeparam>
    /// <typeparam name="TEdge">Edge type.</typeparam>
    public sealed class GreedyVertexColoringAlgorithm<TVertex, TEdge> : AlgorithmBase<IUndirectedGraph<TVertex, TEdge>>
        where TEdge : IEdge<TVertex>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="GreedyVertexColoringAlgorithm{TVertex,TEdge}"/> class.
        /// </summary>
        /// <param name="visitedGraph">Graph to visit.</param>
        /// <exception cref="T:System.ArgumentNullException"><paramref name="visitedGraph"/> is <see langword="null"/>.</exception>
        public GreedyVertexColoringAlgorithm([NotNull] IUndirectedGraph<TVertex, TEdge> visitedGraph)
            : base(visitedGraph)
        {
        }

        /// <summary>
        /// Vertices colors.
        /// </summary>
        [NotNull]
        public IDictionary<TVertex, int?> Colors { get; } = new Dictionary<TVertex, int?>();

        /// <summary>
        /// Fired when a vertex is colored.
        /// </summary>
        public event VertexAction<TVertex> VertexColored;

        private void OnVertexColored([NotNull] TVertex vertex)
        {
            Debug.Assert(vertex != null);

            VertexColored?.Invoke(vertex);
        }

        #region AlgorithmBase<TGraph>

        /// <inheritdoc />
        protected override void Initialize()
        {
            base.Initialize();

            Colors.Clear();

            // Initialize vertices as unassigned
            foreach (TVertex vertex in VisitedGraph.Vertices)
            {
                Colors[vertex] = null; // No color is assigned to vertex
            }
        }

        /// <inheritdoc />
        protected override void InternalCompute()
        {
            int vertexCount = VisitedGraph.VertexCount;
            if (vertexCount == 0)
                return;
            TVertex firstVertex = VisitedGraph.Vertices.First();

            // Assign the first color to first vertex
            Colors[firstVertex] = 0;
            OnVertexColored(firstVertex);

            // A temporary array to store the available colors. True
            // value of available[usedColor] would mean that the color usedColor is
            // assigned to one of its adjacent vertices
            bool[] available = new bool[vertexCount];
            for (int usingColor = 0; usingColor < vertexCount; ++usingColor)
            {
                available[usingColor] = false;
            }

            // Assign colors to remaining vertexCount-1 vertices
            foreach (TVertex vertex in VisitedGraph.Vertices.Skip(1))
            {
                // Process all adjacent vertices and flag their colors as unavailable
                MarkAdjacentAsUnavailable(vertex, available);

                // Find the first available color
                int usingColor = FindAvailableColor(available);

                // Assign the found color
                Colors[vertex] = usingColor;
                OnVertexColored(vertex);

                // Reset the values back to false for the next iteration
                ResetAdjacentAsAvailable(vertex, available);
            }
        }

        private void MarkAdjacentAsUnavailable([NotNull] TVertex vertex, [NotNull] bool[] available)
        {
            foreach (TEdge adjacentEdges in VisitedGraph.AdjacentEdges(vertex))
            {
                TVertex adjacentVertex = adjacentEdges.GetOtherVertex(vertex);
                if (Colors[adjacentVertex].HasValue)
                {
                    available[Colors[adjacentVertex].Value] = true;
                }
            }
        }

        private static int FindAvailableColor([NotNull] bool[] available)
        {
            int usingColor;
            for (usingColor = 0; usingColor < available.Length; ++usingColor)
            {
                if (!available[usingColor])
                    break;
            }

            return usingColor;
        }

        private void ResetAdjacentAsAvailable([NotNull] TVertex vertex, [NotNull] bool[] available)
        {
            foreach (TEdge adjacentEdges in VisitedGraph.AdjacentEdges(vertex))
            {
                if (Colors[adjacentEdges.GetOtherVertex(vertex)].HasValue)
                {
                    // ReSharper disable once PossibleInvalidOperationException, Justification: Was assigned a color just before
                    int usedColor = Colors[adjacentEdges.GetOtherVertex(vertex)].Value;
                    available[usedColor] = false;
                }
            }
        }

        #endregion
    }
}