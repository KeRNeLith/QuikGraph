using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using JetBrains.Annotations;
using QuikGraph.Algorithms.Search;
using QuikGraph.Algorithms.Services;
#if !SUPPORTS_SORTEDSET
using QuikGraph.Collections;
#endif

namespace QuikGraph.Algorithms.ConnectedComponents
{
    /// <inheritdoc cref="ComputeWeaklyConnectedComponents"/>
    public static class WeaklyConnectedComponentsAlgorithm
    {
        /// <summary> Computes the <see cref="WeaklyConnectedComponentsAlgorithm{TVertex,TEdge}"/> </summary>
        public static WeaklyConnectedComponentsAlgorithm<TVertex, TEdge> ComputeWeaklyConnectedComponents<TVertex, TEdge>([NotNull] this IVertexListGraph<TVertex, TEdge> visitedGraph
            , [CanBeNull] IDictionary<TVertex, int> collectComponents = null, [CanBeNull] IAlgorithmComponent host = null)
            where TEdge : IEdge<TVertex>
        {
            var algorithm = new WeaklyConnectedComponentsAlgorithm<TVertex, TEdge>(visitedGraph, collectComponents, host);
            algorithm.Compute();
            return algorithm;
        }
    }


    /// <summary>
    /// Algorithm that computes weakly connected components of a graph.
    /// </summary>
    /// <remarks>
    /// A weakly connected component is a maximal sub graph of a graph such that for
    /// every pair of vertices (u,v) in the sub graph, there is an undirected path from u to v
    /// and a directed path from v to u.
    /// </remarks>
    /// <typeparam name="TVertex">Vertex type.</typeparam>
    /// <typeparam name="TEdge">Edge type.</typeparam>
    public sealed class WeaklyConnectedComponentsAlgorithm<TVertex, TEdge>
        : AlgorithmBase<IVertexListGraph<TVertex, TEdge>>
        , IConnectedComponentAlgorithm<TVertex, TEdge, IVertexListGraph<TVertex, TEdge>>
        where TEdge : IEdge<TVertex>
    {
        [NotNull]
        private readonly Dictionary<int, int> _componentEquivalences = new Dictionary<int, int>();

        private int _currentComponent;

        /// <summary>
        /// Initializes a new <see cref="WeaklyConnectedComponentsAlgorithm{TVertex,TEdge}"/> class.
        /// </summary>
        /// <param name="visitedGraph">Graph to visit.</param>
        /// <param name="components">Graph components.</param>
        /// <param name="host">Host to use if set, otherwise use this reference.</param>
        /// <exception cref="T:System.ArgumentNullException"><paramref name="visitedGraph"/> is <see langword="null"/>.</exception>
        /// <exception cref="T:System.ArgumentNullException"><paramref name="components"/> is <see langword="null"/>.</exception>
        internal WeaklyConnectedComponentsAlgorithm([NotNull] IVertexListGraph<TVertex, TEdge> visitedGraph,
            [CanBeNull] IDictionary<TVertex, int> components = null, [CanBeNull] IAlgorithmComponent host = null)
            : base(visitedGraph, host)
        {
            ComponentIndex = components ?? new Dictionary<TVertex, int>(visitedGraph.VertexCount);
        }

        [ItemNotNull]
        private BidirectionalGraph<TVertex, TEdge>[] _graphs;

        /// <summary>
        /// Weakly connected components.
        /// </summary>
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

        #region AlgorithmBase<TGraph>

        /// <inheritdoc />
        protected override void Initialize()
        {
            ComponentCount = 0;
            _currentComponent = 0;
            _componentEquivalences.Clear();
            ComponentIndex.Clear();
        }

        /// <inheritdoc />
        protected override void InternalCompute()
        {
            // Shortcut for empty graph
            if (VisitedGraph.IsVerticesEmpty)
                return;

            var dfs = VisitedGraph.CreateDepthFirstSearchAlgorithm();
            try
            {
                dfs.StartVertex += OnStartVertex;
                dfs.TreeEdge += OnEdgeDiscovered;
                dfs.ForwardOrCrossEdge += OnForwardOrCrossEdge;

                dfs.Compute();
            }
            finally
            {
                dfs.StartVertex -= OnStartVertex;
                dfs.TreeEdge -= OnEdgeDiscovered;
                dfs.ForwardOrCrossEdge -= OnForwardOrCrossEdge;
            }
        }

        /// <inheritdoc />
        protected override void Clean()
        {
            base.Clean();

            // Merge component numbers
            MergeEquivalentComponents();

            _componentEquivalences.Clear();

            // If there are more than one component
            // then it can have some blanks between components indexes
            // Apply a compression to reduce spacing between components
            if (ComponentCount > 1)
            {
                ReduceComponentsIndexes();

                _componentEquivalences.Clear();
            }

            Debug.Assert(ComponentCount >= 0 && ComponentCount <= VisitedGraph.VertexCount);
            Debug.Assert(
                VisitedGraph.Vertices.All(
                    vertex => ComponentIndex[vertex] >= 0 && ComponentIndex[vertex] < ComponentCount));
        }

        private void MergeEquivalentComponents()
        {
            foreach (TVertex vertex in VisitedGraph.Vertices)
            {
                int component = ComponentIndex[vertex];
                int equivalent = GetComponentEquivalence(component);
                if (component != equivalent)
                {
                    ComponentIndex[vertex] = equivalent;
                }
            }
        }

        private void ReduceComponentsIndexes()
        {
            // Extract unique component indexes (sorted)
            var components = new SortedSet<int>();
            foreach (int componentNumber in ComponentIndex.Values)
            {
                components.Add(componentNumber);
            }

            int[] sortedComponents = components.ToArray();

            // Compute component index reduction (via component equivalences)
            for (int i = 0; i < sortedComponents.Length - 1; ++i)
            {
                if (sortedComponents[i + 1] - sortedComponents[i] > 1)
                {
                    int newComponentNumber = sortedComponents[i] + 1;
                    _componentEquivalences.Add(sortedComponents[i + 1], newComponentNumber);
                    sortedComponents[i + 1] = newComponentNumber;
                }
            }

            // Apply the reduction of component indexes
            foreach (TVertex vertex in VisitedGraph.Vertices)
            {
                int component = ComponentIndex[vertex];
                if (_componentEquivalences.TryGetValue(component, out int newComponentValue))
                {
                    ComponentIndex[vertex] = newComponentValue;
                }
            }
        }

        #endregion

        #region IConnectedComponentAlgorithm<TVertex,TEdge,TGraph>

        /// <inheritdoc />
        public int ComponentCount { get; private set; }

        /// <inheritdoc />
        public IDictionary<TVertex, int> ComponentIndex { get; }

        #endregion

        private void OnStartVertex([NotNull] TVertex vertex)
        {
            // We are looking on a new tree
            _currentComponent = _componentEquivalences.Count;
            _componentEquivalences.Add(_currentComponent, _currentComponent);
            ++ComponentCount;
            ComponentIndex.Add(vertex, _currentComponent);
        }

        private void OnEdgeDiscovered([NotNull] TEdge edge)
        {
            // New edge, we store with the current component number
            ComponentIndex.Add(edge.Target, _currentComponent);
        }

        private void OnForwardOrCrossEdge([NotNull] TEdge edge)
        {
            // We have touched another tree, updating count and current component
            int otherComponent = GetComponentEquivalence(ComponentIndex[edge.Target]);
            if (otherComponent != _currentComponent)
            {
                --ComponentCount;

                Debug.Assert(ComponentCount > 0);
                if (_currentComponent > otherComponent)
                {
                    _componentEquivalences[_currentComponent] = otherComponent;
                    _currentComponent = otherComponent;
                }
                else
                {
                    _componentEquivalences[otherComponent] = _currentComponent;
                }
            }
        }

        private int GetComponentEquivalence(int component)
        {
            int equivalent = component;
            int temp = _componentEquivalences[equivalent];
            bool compress = false;
            while (temp != equivalent)
            {
                equivalent = temp;
                temp = _componentEquivalences[equivalent];
                compress = true;
            }

            // Path compression
            if (compress)
            {
                temp = _componentEquivalences[component];
                while (temp != equivalent)
                {
                    temp = _componentEquivalences[component];
                    _componentEquivalences[component] = equivalent;
                }
            }

            return equivalent;
        }
    }
}