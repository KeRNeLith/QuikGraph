using System.Text;
using JetBrains.Annotations;
using QuikGraph.Algorithms.Condensation;

namespace QuikGraph.Graphviz
{
    /// <summary>
    /// Condensation graph to DOT renderer.
    /// </summary>
    /// <typeparam name="TVertex">Vertex type.</typeparam>
    /// <typeparam name="TEdge">Edge type.</typeparam>
    /// <typeparam name="TGraph">Graph type.</typeparam>
    public class CondensatedGraphRenderer<TVertex, TEdge, TGraph> : GraphRendererBase<TGraph, CondensedEdge<TVertex, TEdge, TGraph>>
        where TEdge : IEdge<TVertex>
        where TGraph : IMutableVertexAndEdgeSet<TVertex, TEdge>, new()
    {
        /// <summary>
        /// Initializes a new <see cref="CondensatedGraphRenderer{TVertex,TEdge,TGraph}"/> class.
        /// </summary>
        /// <param name="graph">Graph to convert to DOT.</param>
        /// <exception cref="T:System.ArgumentNullException"><paramref name="graph"/> is <see langword="null"/>.</exception>
        public CondensatedGraphRenderer(
            [NotNull] IEdgeListGraph<TGraph, CondensedEdge<TVertex, TEdge, TGraph>> graph)
            : base(graph)
        {
        }

        /// <inheritdoc />
        protected override void Initialize()
        {
            base.Initialize();

            Graphviz.FormatVertex += OnFormatVertex;
            Graphviz.FormatEdge += OnFormatEdge;
        }

        /// <inheritdoc />
        protected override void Clean()
        {
            Graphviz.FormatEdge -= OnFormatEdge;
            Graphviz.FormatVertex -= OnFormatVertex;

            base.Clean();
        }

        private static void OnFormatVertex([NotNull] object sender, [NotNull] FormatVertexEventArgs<TGraph> args)
        {
            var builder = new StringBuilder();
            builder.AppendLine($"{args.Vertex.VertexCount}-{args.Vertex.EdgeCount}");

            foreach (TVertex vertex in args.Vertex.Vertices)
            {
                builder.AppendLine($"  {vertex}");
            }
            foreach (TEdge edge in args.Vertex.Edges)
            {
                builder.AppendLine($"  {edge}");
            }

            args.VertexFormat.Label = builder.ToString();
        }

        private static void OnFormatEdge(
            [NotNull] object sender,
            [NotNull] FormatEdgeEventArgs<TGraph, CondensedEdge<TVertex, TEdge, TGraph>> args)
        {
            var builder = new StringBuilder();
            builder.AppendLine(args.Edge.Edges.Count.ToString());

            foreach (TEdge edge in args.Edge.Edges)
            {
                builder.AppendLine($"  {edge}");
            }

            args.EdgeFormat.Label.Value = builder.ToString();
        }
    }
}