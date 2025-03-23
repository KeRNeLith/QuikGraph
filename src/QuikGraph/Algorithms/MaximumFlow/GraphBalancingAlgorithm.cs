using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using JetBrains.Annotations;

namespace QuikGraph.Algorithms.MaximumFlow
{
    /// <inheritdoc cref="CreateGraphBalancerAlgorithm"/>
    public static class GraphBalancerAlgorithm
    {
        /// <summary>Creates a new <see cref="GraphBalancerAlgorithm{TVertex,TEdge}"/> class. </summary>
        public static GraphBalancerAlgorithm<TVertex, TEdge>
            CreateGraphBalancerAlgorithm<TVertex, TEdge>(
            [NotNull] this IMutableBidirectionalGraph<TVertex, TEdge> visitedGraph,
            [NotNull] TVertex source,
            [NotNull] TVertex sink,
            [NotNull] VertexFactory<TVertex> vertexFactory,
            [NotNull] EdgeFactory<TVertex, TEdge> edgeFactory,
            [CanBeNull] IDictionary<TEdge, double> capacities = null)
            where TEdge : IEdge<TVertex>
            => new GraphBalancerAlgorithm<TVertex, TEdge>(visitedGraph,source,sink,vertexFactory,edgeFactory, capacities);

    }
    /// <summary> computes a the graph balancing by finding vertices causing surplus or deficits. </summary>
    public sealed class GraphBalancerAlgorithm<TVertex, TEdge>
        where TEdge : IEdge<TVertex>
    {
        [NotNull]
        private readonly Dictionary<TEdge, int> _preFlow = new Dictionary<TEdge, int>();

        /// <summary>
        /// Initializes a new <see cref="GraphBalancerAlgorithm{TVertex,TEdge}"/> class.
        /// </summary>
        /// <param name="visitedGraph">Graph to visit.</param>
        /// <param name="source">Flow source vertex.</param>
        /// <param name="sink">Flow sink vertex.</param>
        /// <param name="vertexFactory">Vertex factory method.</param>
        /// <param name="edgeFactory">Edge factory method.</param>
        /// <param name="capacities">Edges capacities.</param>
        /// <exception cref="T:System.ArgumentNullException"><paramref name="visitedGraph"/> is <see langword="null"/>.</exception>
        /// <exception cref="T:System.ArgumentNullException"><paramref name="source"/> is <see langword="null"/>.</exception>
        /// <exception cref="T:System.ArgumentNullException"><paramref name="sink"/> is <see langword="null"/>.</exception>
        /// <exception cref="T:System.ArgumentNullException"><paramref name="vertexFactory"/> is <see langword="null"/>.</exception>
        /// <exception cref="T:System.ArgumentNullException"><paramref name="edgeFactory"/> is <see langword="null"/>.</exception>
        /// <exception cref="T:System.ArgumentNullException"><paramref name="capacities"/> is <see langword="null"/>.</exception>
        /// <exception cref="T:System.ArgumentException"><paramref name="visitedGraph"/> does not contain <paramref name="source"/> vertex.</exception>
        /// <exception cref="T:System.ArgumentException"><paramref name="visitedGraph"/> does not contain <paramref name="sink"/> vertex.</exception>
        internal GraphBalancerAlgorithm(
            [NotNull] IMutableBidirectionalGraph<TVertex, TEdge> visitedGraph,
            [NotNull] TVertex source,
            [NotNull] TVertex sink,
            [NotNull] VertexFactory<TVertex> vertexFactory,
            [NotNull] EdgeFactory<TVertex, TEdge> edgeFactory,
            [CanBeNull] IDictionary<TEdge, double> capacities = null)
        {
            if (source == null)
                throw new ArgumentNullException(nameof(source));
            if (sink == null)
                throw new ArgumentNullException(nameof(sink));

            VisitedGraph = visitedGraph ?? throw new ArgumentNullException(nameof(visitedGraph));
            VertexFactory = vertexFactory ?? throw new ArgumentNullException(nameof(vertexFactory));
            EdgeFactory = edgeFactory ?? throw new ArgumentNullException(nameof(edgeFactory));
            Capacities = capacities ?? new Dictionary<TEdge, double>();

            if (!VisitedGraph.ContainsVertex(source))
                throw new ArgumentException("Source must be in the graph", nameof(source));
            if (!VisitedGraph.ContainsVertex(sink))
                throw new ArgumentException("Sink must be in the graph", nameof(sink));
            Source = source;
            Sink = sink;

            // Setting preflow = l(e) = 1
            foreach (TEdge edge in VisitedGraph.Edges)
            {
                if (capacities is null) // Setting capacities = u(e) = +infinity
                    Capacities.Add(edge, double.MaxValue);

                _preFlow.Add(edge, 1);
            }
        }

        /// <summary>
        /// Gets the graph to visit with this algorithm.
        /// </summary>
        [NotNull]
        public IMutableBidirectionalGraph<TVertex, TEdge> VisitedGraph { get; }

        /// <summary>
        /// Vertex factory method.
        /// </summary>
        [NotNull]
        public VertexFactory<TVertex> VertexFactory { get; }

        /// <summary>
        /// Edge factory method.
        /// </summary>
        [NotNull]
        public EdgeFactory<TVertex, TEdge> EdgeFactory { get; }

        /// <summary>
        /// Indicates if the graph has been balanced or not.
        /// </summary>
        public bool Balanced { get; private set; }

        /// <summary>
        /// Flow source vertex.
        /// </summary>
        [NotNull]
        public TVertex Source { get; }

        /// <summary>
        /// Flow sink vertex.
        /// </summary>
        [NotNull]
        public TVertex Sink { get; }

        /// <summary>
        /// Balancing flow source vertex.
        /// </summary>
        /// <remarks>Not <see langword="null"/> if the algorithm has been run (and not reverted).</remarks>
        public TVertex BalancingSource { get; private set; }

        /// <summary>
        /// Balancing source edge (between <see cref="BalancingSource"/> and <see cref="Source"/>).
        /// </summary>
        /// <remarks>Not <see langword="null"/> if the algorithm has been run (and not reverted).</remarks>
        public TEdge BalancingSourceEdge { get; private set; }

        /// <summary>
        /// Balancing flow sink vertex.
        /// </summary>
        /// <remarks>Not <see langword="null"/> if the algorithm has been run (and not reverted).</remarks>
        public TVertex BalancingSink { get; private set; }

        /// <summary>
        /// Balancing sink edge (between <see cref="Sink"/> and <see cref="BalancingSink"/>).
        /// </summary>
        /// <remarks>Not <see langword="null"/> if the algorithm has been run (and not reverted).</remarks>
        public TEdge BalancingSinkEdge { get; private set; }

        [NotNull, ItemNotNull]
        private readonly List<TVertex> _surplusVertices = new List<TVertex>();

        /// <summary>
        /// Enumerable of vertices that add surplus to the graph balance.
        /// </summary>
        [NotNull, ItemNotNull]
        public IEnumerable<TVertex> SurplusVertices => _surplusVertices.AsEnumerable();

        [NotNull, ItemNotNull]
        private readonly List<TEdge> _surplusEdges = new List<TEdge>();

        /// <summary>
        /// Enumerable of edges linked to vertices that add surplus to the graph balance.
        /// </summary>
        [NotNull, ItemNotNull]
        public IEnumerable<TEdge> SurplusEdges => _surplusEdges.AsEnumerable();

        [NotNull, ItemNotNull]
        private readonly List<TVertex> _deficientVertices = new List<TVertex>();

        /// <summary>
        /// Enumerable of vertices that add deficit to the graph balance.
        /// </summary>
        [NotNull, ItemNotNull]
        public IEnumerable<TVertex> DeficientVertices => _deficientVertices.AsEnumerable();

        [NotNull, ItemNotNull]
        private readonly List<TEdge> _deficientEdges = new List<TEdge>();

        /// <summary>
        /// Enumerable of edges linked to vertices that add deficit to the graph balance.
        /// </summary>
        [NotNull, ItemNotNull]
        public IEnumerable<TEdge> DeficientEdges => _deficientEdges.AsEnumerable();

        /// <summary>
        /// Edges capacities.
        /// </summary>
        [NotNull]
        public IDictionary<TEdge, double> Capacities { get; } = new Dictionary<TEdge, double>();

        /// <summary>
        /// Fired when the <see cref="BalancingSource"/> is added to the graph.
        /// </summary>
        public event VertexAction<TVertex> BalancingSourceAdded;

        private void OnBalancingSourceAdded()
        {
            BalancingSourceAdded?.Invoke(Source);
        }

        /// <summary>
        /// Fired when the <see cref="BalancingSink"/> is added to the graph.
        /// </summary>
        public event VertexAction<TVertex> BalancingSinkAdded;

        private void OnBalancingSinkAdded()
        {
            BalancingSinkAdded?.Invoke(Sink);
        }

        /// <summary>
        /// Fired when an edge is added to the graph.
        /// </summary>
        public event EdgeAction<TVertex, TEdge> EdgeAdded;

        private void OnEdgeAdded([NotNull] TEdge edge)
        {
            Debug.Assert(edge != null);

            EdgeAdded?.Invoke(edge);
        }

        /// <summary>
        /// Fired when a vertex adding surplus to the balance is found and added to <see cref="SurplusVertices"/>.
        /// </summary>
        public event VertexAction<TVertex> SurplusVertexAdded;

        private void OnSurplusVertexAdded([NotNull] TVertex vertex)
        {
            Debug.Assert(vertex != null);

            SurplusVertexAdded?.Invoke(vertex);
        }

        /// <summary>
        /// Fired when a vertex adding a deficit to the balance is found and added to <see cref="DeficientVertices"/>.
        /// </summary>
        public event VertexAction<TVertex> DeficientVertexAdded;

        private void OnDeficientVertexAdded([NotNull] TVertex vertex)
        {
            Debug.Assert(vertex != null);

            DeficientVertexAdded?.Invoke(vertex);
        }

        /// <summary>
        /// Gets the balancing index of the given <paramref name="vertex"/>.
        /// </summary>
        /// <param name="vertex">Vertex to get balancing index.</param>
        /// <returns>Balancing index.</returns>
        [Pure]
        public int GetBalancingIndex([NotNull] TVertex vertex)
        {
            if (vertex == null)
                throw new ArgumentNullException(nameof(vertex));

            int balancingIndex = VisitedGraph.OutEdges(vertex).Sum(edge => _preFlow[edge]);

            foreach (TEdge edge in VisitedGraph.InEdges(vertex))
            {
                int preFlow = _preFlow[edge];
                balancingIndex -= preFlow;
            }

            return balancingIndex;
        }

        /// <summary>
        /// Runs the graph balancing algorithm.
        /// </summary>
        /// <exception cref="T:System.InvalidOperationException">If the graph is already balanced.</exception>
        public void Balance()
        {
            if (Balanced)
                throw new InvalidOperationException("Graph already balanced.");

            // Step 0
            // Create new balancing source and sink
            BalancingSource = VertexFactory();
            VisitedGraph.AddVertex(BalancingSource);
            OnBalancingSourceAdded();

            BalancingSink = VertexFactory();
            VisitedGraph.AddVertex(BalancingSink);
            OnBalancingSinkAdded();

            // Step 1
            // Link balancing source to the flow source
            BalancingSourceEdge = EdgeFactory(BalancingSource, Source);
            VisitedGraph.AddEdge(BalancingSourceEdge);
            Capacities.Add(BalancingSourceEdge, double.MaxValue);
            _preFlow.Add(BalancingSourceEdge, 0);
            OnEdgeAdded(BalancingSourceEdge);

            // Link the flow sink to the balancing sink
            BalancingSinkEdge = EdgeFactory(Sink, BalancingSink);
            VisitedGraph.AddEdge(BalancingSinkEdge);
            Capacities.Add(BalancingSinkEdge, double.MaxValue);
            _preFlow.Add(BalancingSinkEdge, 0);
            OnEdgeAdded(BalancingSinkEdge);

            // Step 2
            // For each surplus vertex v, add (source -> v)
            foreach (TVertex vertex in VisitedGraph.Vertices.Where(v => !IsSourceOrSink(v)))
            {
                int balancingIndex = GetBalancingIndex(vertex);
                if (balancingIndex == 0)
                    continue;

                if (balancingIndex < 0)
                {
                    // Surplus vertex
                    TEdge edge = EdgeFactory(BalancingSource, vertex);
                    VisitedGraph.AddEdge(edge);

                    _surplusEdges.Add(edge);
                    _surplusVertices.Add(vertex);

                    _preFlow.Add(edge, 0);

                    Capacities.Add(edge, -balancingIndex);

                    OnSurplusVertexAdded(vertex);
                    OnEdgeAdded(edge);
                }
                else
                {
                    // Deficient vertex
                    TEdge edge = EdgeFactory(vertex, BalancingSink);

                    _deficientEdges.Add(edge);
                    _deficientVertices.Add(vertex);

                    _preFlow.Add(edge, 0);

                    Capacities.Add(edge, balancingIndex);

                    OnDeficientVertexAdded(vertex);
                    OnEdgeAdded(edge);
                }
            }

            Balanced = true;

            #region Local function

            bool IsSourceOrSink(TVertex v)
            {
                return VisitedGraph.AreVerticesEqual(v, BalancingSource)
                       || VisitedGraph.AreVerticesEqual(v, BalancingSink)
                       || VisitedGraph.AreVerticesEqual(v, Source)
                       || VisitedGraph.AreVerticesEqual(v, Sink);
            }

            #endregion
        }

        /// <summary>
        /// Runs the graph unbalancing algorithm.
        /// </summary>
        /// <exception cref="T:System.InvalidOperationException">If the graph is not balanced.</exception>
        public void UnBalance()
        {
            if (!Balanced)
                throw new InvalidOperationException("Graph is not balanced.");

            foreach (TEdge edge in _surplusEdges)
            {
                VisitedGraph.RemoveEdge(edge);
                Capacities.Remove(edge);
                _preFlow.Remove(edge);
            }

            foreach (TEdge edge in _deficientEdges)
            {
                VisitedGraph.RemoveEdge(edge);
                Capacities.Remove(edge);
                _preFlow.Remove(edge);
            }

            Capacities.Remove(BalancingSinkEdge);
            Capacities.Remove(BalancingSourceEdge);

            _preFlow.Remove(BalancingSinkEdge);
            _preFlow.Remove(BalancingSourceEdge);

            VisitedGraph.RemoveEdge(BalancingSourceEdge);
            VisitedGraph.RemoveEdge(BalancingSinkEdge);
            VisitedGraph.RemoveVertex(BalancingSource);
            VisitedGraph.RemoveVertex(BalancingSink);

            BalancingSource = default(TVertex);
            BalancingSink = default(TVertex);
            BalancingSourceEdge = default(TEdge);
            BalancingSinkEdge = default(TEdge);

            _surplusEdges.Clear();
            _deficientEdges.Clear();
            _surplusVertices.Clear();
            _deficientVertices.Clear();

            Balanced = false;
        }
    }
}