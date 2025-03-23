using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
#if SUPPORTS_AGGRESSIVE_INLINING
using System.Runtime.CompilerServices;
#endif
using JetBrains.Annotations;
using QuikGraph.Graphviz.Dot;

namespace QuikGraph.Graphviz
{
    /// <summary>
    /// An algorithm that renders a graph to the Graphviz DOT format.
    /// </summary>
    /// <typeparam name="TVertex">Vertex type.</typeparam>
    /// <typeparam name="TEdge">Edge type.</typeparam>
    public class GraphvizAlgorithm<TVertex, TEdge>
        where TEdge : IEdge<TVertex>
    {
        [NotNull]
        private readonly Dictionary<TVertex, int> _verticesIds = new Dictionary<TVertex, int>();

        /// <summary>
        /// Initializes a new <see cref="GraphvizAlgorithm{TVertex,TEdge}"/> class.
        /// </summary>
        /// <param name="graph">Graph to convert to DOT.</param>
        /// <exception cref="T:System.ArgumentNullException"><paramref name="graph"/> is <see langword="null"/>.</exception>
        public GraphvizAlgorithm([NotNull] IEdgeListGraph<TVertex, TEdge> graph)
            : this(graph, GraphvizImageType.Png)
        {
        }

        /// <summary>
        /// Initializes a new <see cref="GraphvizAlgorithm{TVertex,TEdge}"/> class.
        /// </summary>
        /// <param name="graph">Graph to convert to DOT.</param>
        /// <param name="imageType">Target output image type.</param>
        /// <exception cref="T:System.ArgumentNullException"><paramref name="graph"/> is <see langword="null"/>.</exception>
        public GraphvizAlgorithm(
            [NotNull] IEdgeListGraph<TVertex, TEdge> graph,
            GraphvizImageType imageType)
        {
            ClusterCount = 0;
            _visitedGraph = graph ?? throw new ArgumentNullException(nameof(graph));
            ImageType = imageType;
            GraphFormat = new GraphvizGraph();
            CommonVertexFormat = new GraphvizVertex();
            CommonEdgeFormat = new GraphvizEdge();
        }

        /// <summary>
        /// Graph format.
        /// </summary>
        [NotNull]
        public GraphvizGraph GraphFormat { get; }

        /// <summary>
        /// Common vertex format.
        /// </summary>
        [NotNull]
        public GraphvizVertex CommonVertexFormat { get; }

        /// <summary>
        /// Common edge format.
        /// </summary>
        [NotNull]
        public GraphvizEdge CommonEdgeFormat { get; }

        [NotNull]
        private IEdgeListGraph<TVertex, TEdge> _visitedGraph;

        /// <summary>
        /// Graph to convert.
        /// </summary>
        /// <exception cref="T:System.ArgumentNullException">Set value is <see langword="null"/>.</exception>
        [NotNull]
        public IEdgeListGraph<TVertex, TEdge> VisitedGraph
        {
            get => _visitedGraph;
            set => _visitedGraph = value ?? throw new ArgumentNullException(nameof(value));
        }

        /// <summary>
        /// Dot output stream.
        /// </summary>
        /// <remarks>Not <see langword="null"/> after a run of <see cref="Generate()"/> or <see cref="Generate(IDotEngine,string)"/>.</remarks>
        public StringWriter Output { get; private set; }

        /// <summary>
        /// Current image output type.
        /// </summary>
        public GraphvizImageType ImageType { get; set; }

        internal int ClusterCount { get; set; }

        /// <summary>
        /// Fired when formatting a clustered graph.
        /// </summary>
        public event FormatClusterEventHandler<TVertex, TEdge> FormatCluster;

        private void OnFormatCluster([NotNull] IVertexAndEdgeListGraph<TVertex, TEdge> cluster)
        {
            Debug.Assert(cluster != null);

            FormatClusterEventHandler<TVertex, TEdge> formatCluster = FormatCluster;
            if (formatCluster is null)
                return;

            var args = new FormatClusterEventArgs<TVertex, TEdge>(cluster, new GraphvizGraph());
            formatCluster(this, args);
            string dot = args.GraphFormat.ToDot();
            if (dot.Length != 0)
            {
                Output.WriteLine(dot);
            }
        }

        /// <summary>
        /// Fired when formatting a vertex.
        /// </summary>
        public event FormatVertexEventHandler<TVertex> FormatVertex;

        private void OnFormatVertex([NotNull] TVertex vertex)
        {
            Debug.Assert(vertex != null);

            Output.Write($"{_verticesIds[vertex]}");
            FormatVertexEventHandler<TVertex> formatVertex = FormatVertex;
            if (formatVertex != null)
            {
                var vertexFormat = new GraphvizVertex();
                formatVertex(this, new FormatVertexEventArgs<TVertex>(vertex, vertexFormat));

                string dot = vertexFormat.InternalToDot(CommonVertexFormat);
                if (dot.Length != 0)
                {
                    Output.Write($" [{dot}]");
                }
            }
            Output.WriteLine(";");
        }

        /// <summary>
        /// Fired when formatting an edge.
        /// </summary>
        public event FormatEdgeAction<TVertex, TEdge> FormatEdge;

        private void OnFormatEdge([NotNull] TEdge edge)
        {
            Debug.Assert(edge != null);

            FormatEdgeAction<TVertex, TEdge> formatEdge = FormatEdge;
            if (formatEdge != null)
            {
                var edgeFormat = new GraphvizEdge();
                formatEdge(this, new FormatEdgeEventArgs<TVertex, TEdge>(edge, edgeFormat));

                string dot = edgeFormat.ToDot();
                if (dot.Length != 0)
                {
                    Output.Write($" [{dot}]");
                }
            }
            Output.WriteLine(";");
        }

        /// <summary>
        /// Generates the DOT corresponding to <see cref="VisitedGraph"/>.
        /// </summary>
        /// <returns>DOT serialization of <see cref="VisitedGraph"/>.</returns>
        [Pure]
        [NotNull]
        public string Generate()
        {
            ClusterCount = 0;
            _verticesIds.Clear();
            Output = new StringWriter();
            // Build vertex id map
            int i = 0;
            var vertices = new HashSet<TVertex>(VisitedGraph.Vertices);
            foreach (TVertex vertex in VisitedGraph.Vertices)
            {
                _verticesIds.Add(vertex, i++);
            }

            var edges = new HashSet<TEdge>(VisitedGraph.Edges);

            Output.Write(VisitedGraph.IsDirected ? "digraph " : "graph ");
            Output.Write(GraphFormat.Name);
            Output.WriteLine(" {");

            string graphFormat = GraphFormat.ToDot();
            if (graphFormat.Length > 0)
            {
                Output.WriteLine(graphFormat);
            }
            string vertexFormat = CommonVertexFormat.ToDot();
            if (vertexFormat.Length > 0)
            {
                Output.WriteLine($"node [{vertexFormat}];");
            }
            string edgeFormat = CommonEdgeFormat.ToDot();
            if (edgeFormat.Length > 0)
            {
                Output.WriteLine($"edge [{edgeFormat}];");
            }

            if (VisitedGraph is IClusteredGraph clusteredGraph)
            {
                WriteClusters(vertices, edges, clusteredGraph);
            }

            WriteVertices(vertices);
            WriteEdges(edges);

            Output.Write("}");
            return Output.ToString();
        }

        /// <summary>
        /// Generates the DOT corresponding to <see cref="VisitedGraph"/> using <paramref name="dot"/> engine
        /// and puts result in <paramref name="outputFilePath"/>.
        /// </summary>
        /// <returns>File path containing DOT serialization of <see cref="VisitedGraph"/>.</returns>
        /// <exception cref="T:System.ArgumentNullException"><paramref name="dot"/> is <see langword="null"/>.</exception>
        /// <exception cref="T:System.ArgumentException"><paramref name="outputFilePath"/> is <see langword="null"/> or empty.</exception>
        [NotNull]
        public string Generate([NotNull] IDotEngine dot, [NotNull] string outputFilePath)
        {
            if (dot is null)
                throw new ArgumentNullException(nameof(dot));
            if (string.IsNullOrEmpty(outputFilePath))
                throw new ArgumentException("Output file path cannot be null or empty.", nameof(outputFilePath));

            return dot.Run(ImageType, Generate(), outputFilePath);
        }

        private void WriteClusters(
            [NotNull, ItemNotNull] ICollection<TVertex> remainingVertices, 
            [NotNull, ItemNotNull] ICollection<TEdge> remainingEdges, 
            [NotNull] IClusteredGraph parent)
        {
            Debug.Assert(remainingVertices != null);
            Debug.Assert(remainingEdges != null);
            Debug.Assert(parent != null);

            ++ClusterCount;
            foreach (IVertexAndEdgeListGraph<TVertex, TEdge> subGraph in parent.Clusters)
            {
                Output.Write($"subgraph cluster{ClusterCount}");
                Output.WriteLine(" {");
                OnFormatCluster(subGraph);
                if (subGraph is IClusteredGraph clusteredGraph)
                {
                    WriteClusters(remainingVertices, remainingEdges, clusteredGraph);
                }

                if (parent.Collapsed)
                {
                    foreach (TVertex vertex in subGraph.Vertices)
                    {
                        remainingVertices.Remove(vertex);
                    }

                    foreach (TEdge edge in subGraph.Edges)
                    {
                        remainingEdges.Remove(edge);
                    }
                }
                else
                {
                    WriteVertices(remainingVertices, subGraph.Vertices);
                    WriteEdges(remainingEdges, subGraph.Edges);
                }
                Output.WriteLine("}");
            }
        }

#if SUPPORTS_AGGRESSIVE_INLINING
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        private void WriteVertex([NotNull] TVertex vertex)
        {
            Debug.Assert(vertex != null);

            OnFormatVertex(vertex);
        }

        private void WriteVertices([NotNull, ItemNotNull] IEnumerable<TVertex> vertices)
        {
            Debug.Assert(vertices != null);

            foreach (TVertex vertex in vertices)
            {
                WriteVertex(vertex);
            }
        }

        private void WriteVertices(
            [NotNull, ItemNotNull] ICollection<TVertex> remainingVertices,
            [NotNull, ItemNotNull] IEnumerable<TVertex> vertices)
        {
            Debug.Assert(remainingVertices != null);
            Debug.Assert(vertices != null);

            foreach (TVertex vertex in vertices.Where(remainingVertices.Contains))
            {
                WriteVertex(vertex);
                remainingVertices.Remove(vertex);
            }
        }

#if SUPPORTS_AGGRESSIVE_INLINING
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        private void WriteEdge([NotNull] TEdge edge)
        {
            Debug.Assert(edge != null);

            Output.Write(VisitedGraph.IsDirected
                ? $"{_verticesIds[edge.Source]} -> {_verticesIds[edge.Target]}"
                : $"{_verticesIds[edge.Source]} -- {_verticesIds[edge.Target]}");

            OnFormatEdge(edge);
        }

        private void WriteEdges([NotNull, ItemNotNull] IEnumerable<TEdge> edges)
        {
            Debug.Assert(edges != null);

            foreach (TEdge edge in edges)
            {
                WriteEdge(edge);
            }
        }


        private void WriteEdges(
            [NotNull, ItemNotNull] ICollection<TEdge> remainingEdges,
            [NotNull, ItemNotNull] IEnumerable<TEdge> edges)
        {
            Debug.Assert(remainingEdges != null);
            Debug.Assert(edges != null);

            foreach (TEdge edge in edges.Where(remainingEdges.Contains))
            {
                WriteEdge(edge);
                remainingEdges.Remove(edge);
            }
        }
    }
}