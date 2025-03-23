using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using QuikGraph.Algorithms.ConnectedComponents;

namespace QuikGraph.Algorithms.Condensation
{
    /// <inheritdoc cref="CreateCondensationGraphAlgorithm"/>
    public static class CondensationGraphAlgorithm {

        /// <summary> Creates a new <see cref="CondensationGraphAlgorithm{TVertex,TEdge,TGraph}"/> class. </summary>
        /// <remarks>
        /// Cannot determine the TGraph type, as it is not provided in the constructor.
        /// </remarks>
        public static CondensationGraphAlgorithm<TVertex, TEdge, TGraph>
            CreateCondensationGraphAlgorithm<TVertex, TEdge, TGraph>(
            [NotNull] this IVertexAndEdgeListGraph<TVertex, TEdge> visitedGraph, bool stronglyConnected = true)
                where TEdge : IEdge<TVertex>
                where TGraph : IMutableVertexAndEdgeSet<TVertex, TEdge>, new()
                => new CondensationGraphAlgorithm<TVertex, TEdge, TGraph>(visitedGraph, stronglyConnected);
    }

    /// <summary> Condensates the <see cref="AlgorithmBase{TGraph}.VisitedGraph"/> into its <see cref="StronglyConnected"/> components. </summary>
    /// <remarks>
    /// Generates the <see cref="CondensedGraph"/> with the Root Vertices of the Components
    /// and Edges if there is at least one edge in the <see cref="AlgorithmBase{TGraph}.VisitedGraph"/>
    /// between any 2 vertices in their corresponding Components.
    /// 
    /// Each strongly connected component is represented as a single node in the condensed graph.
    /// The edges between SCCs form a directed acyclic graph!
    /// This 
    ///
    /// This is a very important Algorithm to abstract from the Details of a Graph.
    /// </remarks>
    public sealed class CondensationGraphAlgorithm<TVertex, TEdge, TGraph>
        : AlgorithmBase<IVertexAndEdgeListGraph<TVertex, TEdge>>
        where TEdge : IEdge<TVertex>
        where TGraph : IMutableVertexAndEdgeSet<TVertex, TEdge>, new()
    {
        /// <summary> Creates a new <see cref="CondensationGraphAlgorithm{TVertex,TEdge,TGraph}"/> class. </summary>
        /// <exception cref="T:System.ArgumentNullException"><paramref name="visitedGraph"/> is <see langword="null"/>.</exception>
        internal CondensationGraphAlgorithm([NotNull] IVertexAndEdgeListGraph<TVertex, TEdge> visitedGraph
            , bool stronglyConnected = true) : base(visitedGraph)
        {
            StronglyConnected = stronglyConnected;
        }

        /// <summary>
        /// Condensed graph.
        /// </summary>
        public IMutableBidirectionalGraph<TGraph, CondensedEdge<TVertex, TEdge, TGraph>> CondensedGraph { get; private set; }

        /// <summary> Indicates if the algorithm should determine strongly connected components,
        /// considering the Edge-Directions or not, treating the Edges as undirected.
        /// </summary>
        /// <remarks>
        /// for undirected graphs, WCCs are equivalent to connected components.
        /// </remarks>
        public bool StronglyConnected { get; } = true;

        #region AlgorithmBase<TGraph>

        /// <inheritdoc />
        protected override void InternalCompute()
        {
            // Create condensed graph
            CondensedGraph = new BidirectionalGraph<TGraph, CondensedEdge<TVertex, TEdge, TGraph>>(false);
            if (VisitedGraph.VertexCount == 0)
                return;

            // Compute strongly connected components
            var collectComponents = new Dictionary<TVertex, int>(VisitedGraph.VertexCount);
            int componentCount = ComputeComponentCount(collectComponents);

            ThrowIfCancellationRequested();

            // Create vertices list
            var condensedVertices = new Dictionary<int, TGraph>(componentCount);
            for (int i = 0; i < componentCount; ++i)
            {
                var vertex = new TGraph();
                condensedVertices.Add(i, vertex);
                CondensedGraph.AddVertex(vertex);
            }

            // Adding vertices
            foreach (TVertex vertex in VisitedGraph.Vertices)
            {
                condensedVertices[collectComponents[vertex]].AddVertex(vertex);
            }

            ThrowIfCancellationRequested();

            // Condensed edges
            var condensedEdges = new Dictionary<EdgeKey, CondensedEdge<TVertex, TEdge, TGraph>>(componentCount);

            // Iterate over edges and condensate graph
            foreach (TEdge edge in VisitedGraph.Edges)
            {
                // Get component ids
                int sourceID = collectComponents[edge.Source];
                int targetID = collectComponents[edge.Target];

                // Get vertices
                TGraph sources = condensedVertices[sourceID];
                if (sourceID == targetID)
                {
                    sources.AddEdge(edge);
                    continue;
                }

                // At last add edge
                var edgeKey = new EdgeKey(sourceID, targetID);
                if (!condensedEdges.TryGetValue(edgeKey, out CondensedEdge<TVertex, TEdge, TGraph> condensedEdge))
                {
                    TGraph targets = condensedVertices[targetID];

                    condensedEdge = new CondensedEdge<TVertex, TEdge, TGraph>(sources, targets);
                    condensedEdges.Add(edgeKey, condensedEdge);
                    CondensedGraph.AddEdge(condensedEdge);
                }

                condensedEdge.Edges.Add(edge);
            }
        }

        #endregion

        [Pure]
        private int ComputeComponentCount([NotNull] IDictionary<TVertex, int> collectComponents)
            => StronglyConnected
                ? VisitedGraph.ComputeStronglyConnectedComponents(collectComponents, this).ComponentCount
                : VisitedGraph.ComputeWeaklyConnectedComponents(collectComponents, this).ComponentCount;

        private struct EdgeKey : IEquatable<EdgeKey>
        {
            private readonly int _sourceID;
            private readonly int _targetID;

            public EdgeKey(int sourceID, int targetID)
            {
                _sourceID = sourceID;
                _targetID = targetID;
            }

            /// <inheritdoc />
            public bool Equals(EdgeKey other)
            {
                return _sourceID == other._sourceID
                       && _targetID == other._targetID;
            }

            /// <inheritdoc />
            public override bool Equals(object obj)
            {
                return obj is EdgeKey edgeKey
                       && Equals(edgeKey);
            }

            /// <inheritdoc />
            public override int GetHashCode()
            {
                return HashCodeHelpers.Combine(_sourceID, _targetID);
            }
        }
    }
}