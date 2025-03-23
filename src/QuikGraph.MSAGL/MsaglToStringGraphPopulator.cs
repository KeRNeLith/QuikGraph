using System;
using JetBrains.Annotations;

namespace QuikGraph.MSAGL
{
    /// <summary>
    /// MSAGL graph populator (with string formatting).
    /// </summary>
    /// <typeparam name="TVertex">Vertex type.</typeparam>
    /// <typeparam name="TEdge">Edge type.</typeparam>
    public sealed class MsaglToStringGraphPopulator<TVertex, TEdge> : MsaglDefaultGraphPopulator<TVertex, TEdge>
        where TEdge : IEdge<TVertex>
    {
        /// <summary>
        /// Initializes a new <see cref="MsaglToStringGraphPopulator{TVertex,TEdge}"/> class.
        /// </summary>
        /// <param name="visitedGraph">Graph to convert to MSAGL graph.</param>
        /// <param name="formatProvider">Graph format provider.</param>
        /// <param name="format">Graph format.</param>
        /// <exception cref="T:System.ArgumentNullException"><paramref name="visitedGraph"/> is <see langword="null"/>.</exception>
        public MsaglToStringGraphPopulator(
            [NotNull] IEdgeListGraph<TVertex, TEdge> visitedGraph,
            [CanBeNull] string format = null,
            [CanBeNull] IFormatProvider formatProvider = null)
            : base(visitedGraph)
        {
            FormatProvider = formatProvider;
            Format = string.IsNullOrEmpty(format) ? "{0}" : format;
        }

        /// <summary>
        /// Vertex format provider.
        /// </summary>
        [CanBeNull]
        public IFormatProvider FormatProvider { get; }

        /// <summary>
        /// Vertex id format.
        /// </summary>
        [NotNull]
        public string Format { get; }

        /// <inheritdoc />
        protected override string GetVertexId(TVertex vertex)
        {
            return FormatProvider is null
                ? string.Format(Format, vertex)
                : string.Format(FormatProvider, Format, vertex);
        }
    }
}