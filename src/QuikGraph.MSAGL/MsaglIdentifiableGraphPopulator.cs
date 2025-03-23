using System;
using JetBrains.Annotations;
using Microsoft.Msagl.Drawing;

namespace QuikGraph.MSAGL
{
    /// <summary>
    /// MSAGL graph populator (using identifiable vertices).
    /// </summary>
    /// <typeparam name="TVertex">Vertex type.</typeparam>
    /// <typeparam name="TEdge">Edge type.</typeparam>
    public sealed class MsaglIdentifiableGraphPopulator<TVertex, TEdge> : MsaglGraphPopulator<TVertex, TEdge>
        where TEdge : IEdge<TVertex>
    {
        [NotNull]
        private readonly VertexIdentity<TVertex> _vertexIdentity;

        /// <summary>
        /// Initializes a new <see cref="MsaglIdentifiableGraphPopulator{TVertex,TEdge}"/> class.
        /// </summary>
        /// <param name="visitedGraph">Graph to convert to MSAGL graph.</param>
        /// <param name="vertexIdentity">Delegate that given a vertex return its identifier.</param>
        /// <exception cref="T:System.ArgumentNullException"><paramref name="visitedGraph"/> is <see langword="null"/>.</exception>
        /// <exception cref="T:System.ArgumentNullException"><paramref name="vertexIdentity"/> is <see langword="null"/>.</exception>
        public MsaglIdentifiableGraphPopulator(
            [NotNull] IEdgeListGraph<TVertex, TEdge> visitedGraph,
            [NotNull] VertexIdentity<TVertex> vertexIdentity)
            : base(visitedGraph)
        {
            _vertexIdentity = vertexIdentity ?? throw new ArgumentNullException(nameof(vertexIdentity));
        }

        /// <inheritdoc />
        protected override Node AddNode(TVertex vertex)
        {
            return MsaglGraph.AddNode(_vertexIdentity(vertex));
        }

        /// <inheritdoc />
        protected override Microsoft.Msagl.Drawing.Edge AddEdge(TEdge edge)
        {
            return MsaglGraph.AddEdge(
                _vertexIdentity(edge.Source),
                _vertexIdentity(edge.Target));
        }
    }
}