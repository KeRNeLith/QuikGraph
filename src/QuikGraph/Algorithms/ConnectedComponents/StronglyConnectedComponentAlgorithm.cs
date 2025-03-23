using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using JetBrains.Annotations;
using QuikGraph.Algorithms.Search;
using QuikGraph.Algorithms.Services;

namespace QuikGraph.Algorithms.ConnectedComponents
{
    /// <inheritdoc cref="ComputeStronglyConnectedComponents"/>
    public static class StronglyConnectedComponentsAlgorithm
    {
        /// <summary> Computes the <see cref="StronglyConnectedComponentsAlgorithm{TVertex,TEdge}"/> </summary>
        public static StronglyConnectedComponentsAlgorithm<TVertex, TEdge> ComputeStronglyConnectedComponents<TVertex, TEdge>([NotNull] this IVertexListGraph<TVertex, TEdge> visitedGraph
                , [CanBeNull] IDictionary<TVertex, int> collectComponents = null, [CanBeNull] IAlgorithmComponent host = null)
            where TEdge : IEdge<TVertex>
        {
            var algorithm = visitedGraph.CreateStronglyConnectedComponentsAlgorithm(collectComponents, host);
            algorithm.Compute();
            return algorithm;
        }

        /// <summary> Creates the <see cref="StronglyConnectedComponentsAlgorithm{TVertex,TEdge}"/> </summary>
        public static StronglyConnectedComponentsAlgorithm<TVertex, TEdge> CreateStronglyConnectedComponentsAlgorithm<TVertex, TEdge>
            (this IVertexListGraph<TVertex, TEdge> visitedGraph
                , [CanBeNull] IDictionary<TVertex, int> collectComponents = null, [CanBeNull] IAlgorithmComponent host = null) where TEdge : IEdge<TVertex>
            => new StronglyConnectedComponentsAlgorithm<TVertex, TEdge>(visitedGraph, collectComponents, host);
    }

    /// <summary> Computes and stores strongly connected components of a graph. </summary>
    /// <remarks>
    /// A strongly connected component
    /// is a sub graph with a path from every vertex to every other vertices.
    /// </remarks>
    public sealed class StronglyConnectedComponentsAlgorithm<TVertex, TEdge>
        : AlgorithmBase<IVertexListGraph<TVertex, TEdge>>
        , IConnectedComponentAlgorithm<TVertex, TEdge, IVertexListGraph<TVertex, TEdge>>
        where TEdge : IEdge<TVertex>
    {
        [NotNull]
        private readonly Stack<TVertex> _stack;

        private int _dfsTime;

        /// <summary>
        /// Initializes a new <see cref="StronglyConnectedComponentsAlgorithm{TVertex,TEdge}"/> class.
        /// </summary>
        /// <param name="visitedGraph">Graph to visit.</param>
        /// <param name="components">pre-determined Graph components.</param>
        /// <param name="host">Host to use if set, otherwise use this reference.</param>
        /// <exception cref="T:System.ArgumentNullException"><paramref name="visitedGraph"/> is <see langword="null"/>.</exception>
        /// <exception cref="T:System.ArgumentNullException"><paramref name="components"/> is <see langword="null"/>.</exception>
        internal StronglyConnectedComponentsAlgorithm([NotNull] IVertexListGraph<TVertex, TEdge> visitedGraph,
            [CanBeNull] IDictionary<TVertex, int> components = null, [CanBeNull] IAlgorithmComponent host = null)
            : base(visitedGraph, host)
        {
            ComponentIndex = components ?? new Dictionary<TVertex, int>(visitedGraph.VertexCount);
            Roots = new Dictionary<TVertex, TVertex>();
            DiscoverTimes = new Dictionary<TVertex, int>();
            _stack = new Stack<TVertex>();
            ComponentCount = 0;
            _dfsTime = 0;
        }

        /// <summary> Root vertices associated to their representative (minimal, first) linked vertex. </summary>
        /// <remarks>
        /// <see cref="ComponentIndex"/> is similar, but tracks a counting Index instead of a representative Vertex.
        /// </remarks>
        [NotNull]
        public IDictionary<TVertex, TVertex> Roots { get; }

        /// <summary> Discrete Times of vertices discover in DFS. </summary>
        [NotNull]
        public IDictionary<TVertex, int> DiscoverTimes { get; }

        /// <summary> Number of steps spent. </summary>
        public int Steps { get; private set; }

        /// <summary> Number of components discovered per step. </summary>
        public List<int> ComponentsPerStep { get; private set; }

        /// <summary> Vertices treated per step. </summary>
        public List<TVertex> VerticesPerStep { get; private set; }

        [ItemNotNull]
        private BidirectionalGraph<TVertex, TEdge>[] _graphs;

        /// <summary> Strongly connected component Sub-Graphs. </summary>
        [NotNull, ItemNotNull]
        public BidirectionalGraph<TVertex, TEdge>[] Graphs
        {
            get
            {
                _graphs = new BidirectionalGraph<TVertex, TEdge>[ComponentCount];
                for (int i = 0; i < ComponentCount; ++i)
                {
                    _graphs[i] = new BidirectionalGraph<TVertex, TEdge>();
                }

                foreach (TVertex componentName in ComponentIndex.Keys)
                {
                    _graphs[ComponentIndex[componentName]].AddVertex(componentName);
                }

                foreach (TVertex vertex in VisitedGraph.Vertices)
                {
                    foreach (TEdge edge in VisitedGraph.OutEdges(vertex))
                    {

                        if (ComponentIndex[vertex] == ComponentIndex[edge.Target])
                        {
                            _graphs[ComponentIndex[vertex]].AddEdge(edge);
                        }
                    }
                }

                return _graphs;
            }
        }

        [Pure]
        [NotNull]
        private TVertex MinDiscoverTime([NotNull] TVertex u, [NotNull] TVertex v)
        {
            Debug.Assert(u != null);
            Debug.Assert(v != null);

            // Min vertex
            return DiscoverTimes[u] < DiscoverTimes[v]
                ? u
                : v;
        }

        #region AlgorithmBase<TGraph>

        /// <inheritdoc />
        protected override void Initialize()
        {
            base.Initialize();

            ComponentsPerStep = new List<int>();
            VerticesPerStep = new List<TVertex>();

            ComponentIndex.Clear();
            Roots.Clear();
            DiscoverTimes.Clear();
            _stack.Clear();
            ComponentCount = 0;
            _dfsTime = 0;
        }

        /// <inheritdoc />
        protected override void InternalCompute()
        {
            DepthFirstSearchAlgorithm<TVertex, TEdge> dfs = null;
            try
            {
                dfs = VisitedGraph.CreateDepthFirstSearchAlgorithm(host: this);
                dfs.DiscoverVertex += OnVertexDiscovered;
                dfs.FinishVertex += OnVertexFinished;

                dfs.Compute();
            }
            finally
            {
                if (dfs != null)
                {
                    dfs.DiscoverVertex -= OnVertexDiscovered;
                    dfs.FinishVertex -= OnVertexFinished;
                }
            }

            Debug.Assert(ComponentCount >= 0);
            Debug.Assert(VisitedGraph.VertexCount >= 0 || ComponentCount == 0);
            Debug.Assert(VisitedGraph.Vertices.All(v => ComponentIndex.ContainsKey(v)));
            Debug.Assert(VisitedGraph.VertexCount == ComponentIndex.Count);
            Debug.Assert(ComponentIndex.Values.All(c => c <= ComponentCount));
        }

        #endregion

        #region IConnectedComponentAlgorithm<TVertex,TEdge,TGraph>

        /// <inheritdoc />
        public int ComponentCount { get; private set; }

        /// <inheritdoc />
        /// <remarks>
        /// <see cref="Roots"/> is similar, but maps every Vertex to their representative (minimal, first) linked vertex.
        /// </remarks>
        public IDictionary<TVertex, int> ComponentIndex { get; }

        #endregion

        private void OnVertexDiscovered([NotNull] TVertex vertex)
        {
            Roots[vertex] = vertex;
            ComponentIndex[vertex] = int.MaxValue;

            ComponentsPerStep.Add(ComponentCount);
            VerticesPerStep.Add(vertex);
            ++Steps;

            DiscoverTimes[vertex] = _dfsTime++;
            _stack.Push(vertex);
        }

        private void OnVertexFinished([NotNull] TVertex vertex)
        {
            foreach (TVertex target in VisitedGraph.OutEdges(vertex).Select(edge => edge.Target))
            {
                if (ComponentIndex[target] == int.MaxValue)
                {
                    Roots[vertex] = MinDiscoverTime(Roots[vertex], Roots[target]);
                }
            }

            if (VisitedGraph.AreVerticesEqual(Roots[vertex], vertex))
            {
                TVertex w;
                do
                {
                    w = _stack.Pop();
                    ComponentIndex[w] = ComponentCount;

                    ComponentsPerStep.Add(ComponentCount);
                    VerticesPerStep.Add(w);
                    ++Steps;
                }
                while (!VisitedGraph.AreVerticesEqual(w, vertex));

                ++ComponentCount;
            }
        }
    }
}