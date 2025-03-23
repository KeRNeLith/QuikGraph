using System;
using System.Collections.Generic;
using System.Diagnostics;
using JetBrains.Annotations;
using QuikGraph.Algorithms;
using QuikGraph.Serialization.DirectedGraphML;

namespace QuikGraph.Serialization
{
    /// <summary>
    /// Algorithm that creates a <see cref="DirectedGraphML.DirectedGraph"/>
    /// from a given directed graph.
    /// </summary>
    /// <typeparam name="TVertex">Vertex type.</typeparam>
    /// <typeparam name="TEdge">Edge type.</typeparam>
    [CLSCompliant(false)]
    public sealed class DirectedGraphMLAlgorithm<TVertex, TEdge> : AlgorithmBase<IVertexAndEdgeListGraph<TVertex, TEdge>>
        where TEdge : IEdge<TVertex>
    {
        [NotNull]
        private readonly VertexIdentity<TVertex> _vertexIdentity;

        [NotNull]
        private readonly EdgeIdentity<TVertex, TEdge> _edgeIdentity;

        /// <summary>
        /// Initializes a new <see cref="DirectedGraphMLAlgorithm{TVertex,TEdge}"/> class.
        /// </summary>
        /// <param name="visitedGraph">Graph to visit.</param>
        /// <param name="vertexIdentity">Vertex identity method.</param>
        /// <param name="edgeIdentity">Edge identity method.</param>
        /// <exception cref="T:System.ArgumentNullException"><paramref name="visitedGraph"/> is <see langword="null"/>.</exception>
        /// <exception cref="T:System.ArgumentNullException"><paramref name="vertexIdentity"/> is <see langword="null"/>.</exception>
        /// <exception cref="T:System.ArgumentNullException"><paramref name="edgeIdentity"/> is <see langword="null"/>.</exception>
        public DirectedGraphMLAlgorithm(
            [NotNull] IVertexAndEdgeListGraph<TVertex, TEdge> visitedGraph,
            [NotNull] VertexIdentity<TVertex> vertexIdentity,
            [NotNull] EdgeIdentity<TVertex, TEdge> edgeIdentity)
            : base(visitedGraph)
        {
            _vertexIdentity = vertexIdentity ?? throw new ArgumentNullException(nameof(vertexIdentity));
            _edgeIdentity = edgeIdentity ?? throw new ArgumentNullException(nameof(edgeIdentity));
        }

        /// <summary>
        /// Gets the resulting <see cref="DirectedGraphML.DirectedGraph"/>.
        /// </summary>
        public DirectedGraph DirectedGraph { get; private set; }

        #region AlgorithmBase<TGraph>

        /// <inheritdoc />
        protected override void InternalCompute()
        {
            DirectedGraph = new DirectedGraph();

            var nodes = new List<DirectedGraphNode>(VisitedGraph.VertexCount);
            foreach (TVertex vertex in VisitedGraph.Vertices)
            {
                ThrowIfCancellationRequested();

                var node = new DirectedGraphNode { Id = _vertexIdentity(vertex) };
                OnFormatNode(vertex, node);
                nodes.Add(node);
            }

            DirectedGraph.Nodes = nodes.ToArray();

            var links = new List<DirectedGraphLink>(VisitedGraph.EdgeCount);
            foreach (TEdge edge in VisitedGraph.Edges)
            {
                ThrowIfCancellationRequested();

                var link = new DirectedGraphLink
                {
                    Label = _edgeIdentity(edge),
                    Source = _vertexIdentity(edge.Source),
                    Target = _vertexIdentity(edge.Target)
                };
                OnFormatEdge(edge, link);
                links.Add(link);
            }

            DirectedGraph.Links = links.ToArray();

            OnFormatGraph();
        }

        #endregion

        /// <summary>
        /// Fired when a new node is added to the <see cref="DirectedGraph"/>.
        /// </summary>
        public event Action<TVertex, DirectedGraphNode> FormatNode;

        private void OnFormatNode([NotNull] TVertex vertex, [NotNull] DirectedGraphNode node)
        {
            Debug.Assert(vertex != null);
            Debug.Assert(node != null);

            FormatNode?.Invoke(vertex, node);
        }

        /// <summary>
        /// Fired when a new link is added to the <see cref="DirectedGraph"/>.
        /// </summary>
        public event Action<TEdge, DirectedGraphLink> FormatEdge;

        private void OnFormatEdge([NotNull] TEdge edge, [NotNull] DirectedGraphLink link)
        {
            Debug.Assert(edge != null);
            Debug.Assert(link != null);

            FormatEdge?.Invoke(edge, link);
        }

        /// <summary>
        /// Fired when the graph is about to be returned.
        /// </summary>
        public event Action<IVertexAndEdgeListGraph<TVertex, TEdge>, DirectedGraph> FormatGraph;

        private void OnFormatGraph()
        {
            FormatGraph?.Invoke(VisitedGraph, DirectedGraph);
        }
    }
}