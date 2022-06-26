using System.Diagnostics;
using JetBrains.Annotations;
using QuikGraph.Graphviz.Dot;

namespace QuikGraph.Graphviz
{
    /// <summary>
    /// Arguments of an event related to the formatting of an edge.
    /// </summary>
    public sealed class FormatEdgeEventArgs<TVertex, TEdge> : EdgeEventArgs<TVertex, TEdge>
        where TEdge : IEdge<TVertex>
    {
        /// <summary />
        internal FormatEdgeEventArgs([NotNull] TEdge edge, [NotNull] GraphvizEdge edgeFormat)
            : base(edge)
        {
            Debug.Assert(edgeFormat != null);

            EdgeFormat = edgeFormat;
        }

        /// <summary>
        /// Edge format.
        /// </summary>
        public GraphvizEdge EdgeFormat { get; }
    }
}