using System;
using System.Collections.Generic;
using System.Reflection;
#if SUPPORTS_CONTRACTS
using System.Diagnostics.Contracts;
#endif
using System.Linq;
using JetBrains.Annotations;
using QuikGraph.Algorithms.Condensation;
using QuikGraph.Algorithms.ConnectedComponents;
using QuikGraph.Algorithms.MaximumFlow;
using QuikGraph.Algorithms.MinimumSpanningTree;
using QuikGraph.Algorithms.Observers;
using QuikGraph.Algorithms.RandomWalks;
using QuikGraph.Algorithms.RankedShortestPath;
using QuikGraph.Algorithms.Search;
using QuikGraph.Algorithms.ShortestPath;
using QuikGraph.Algorithms.TopologicalSort;
using QuikGraph.Collections;
#if !SUPPORTS_TYPE_FULL_FEATURES
using QuikGraph.Utils;
#endif

namespace QuikGraph.Algorithms
{
    /// <summary>
    /// Extensions related to algorithms, to run them.
    /// </summary>
    public static class AlgorithmExtensions
    {
        /// <summary>
        /// Returns the method that implement the access indexer.
        /// </summary>
        /// <typeparam name="TKey">Key type.</typeparam>
        /// <typeparam name="TValue">Value type.</typeparam>
        /// <param name="dictionary">Dictionary on which getting the key access method.</param>
        /// <returns>A function allowing key indexed access.</returns>
#if SUPPORTS_CONTRACTS
        [System.Diagnostics.Contracts.Pure]
#endif
        [JetBrains.Annotations.Pure]
        [NotNull]
        public static Func<TKey, TValue> GetIndexer<TKey, TValue>([NotNull] IDictionary<TKey, TValue> dictionary)
        {
#if SUPPORTS_CONTRACTS
            Contract.Requires(dictionary != null);
            Contract.Ensures(Contract.Result<Func<TKey, TValue>>() != null);
#endif

#if SUPPORTS_TYPE_FULL_FEATURES
            // ReSharper disable once PossibleNullReferenceException, Justification: Dictionary has the [] operator called "Item".
            MethodInfo method = dictionary.GetType().GetProperty("Item").GetGetMethod();
            // ReSharper disable once AssignNullToNotNullAttribute, Justification: Throws if the method is not found.
            return (Func<TKey, TValue>)Delegate.CreateDelegate(typeof(Func<TKey, TValue>), dictionary, method, true);
#else
            return key => dictionary[key];
#endif
        }

        /// <summary>
        /// Gets the vertex identity.
        /// </summary>
        /// <remarks>
        /// Returns more efficient methods for primitive types,
        /// otherwise builds a dictionary.
        /// </remarks>
        /// <typeparam name="TVertex">Vertex type.</typeparam>
        /// <param name="graph">The graph.</param>
        /// <returns>A function that computes a vertex identity for the given <paramref name="graph"/>.</returns>
#if SUPPORTS_CONTRACTS
        [System.Diagnostics.Contracts.Pure]
#endif
        [JetBrains.Annotations.Pure]
        [NotNull]
        public static VertexIdentity<TVertex> GetVertexIdentity<TVertex>([NotNull] this IVertexSet<TVertex> graph)
        {
#if SUPPORTS_CONTRACTS
            Contract.Requires(graph != null);
#endif

            // Simpler identity for primitive types
#if SUPPORTS_TYPE_FULL_FEATURES
            switch (Type.GetTypeCode(typeof(TVertex)))
#else
            switch (TypeUtils.GetTypeCode(typeof(TVertex)))
#endif
            {
                case TypeCode.String:
                case TypeCode.Boolean:
                case TypeCode.Byte:
                case TypeCode.Char:
                case TypeCode.Decimal:
                case TypeCode.Double:
                case TypeCode.Int16:
                case TypeCode.Int32:
                case TypeCode.Int64:
                case TypeCode.SByte:
                case TypeCode.Single:
                case TypeCode.UInt16:
                case TypeCode.UInt32:
                case TypeCode.UInt64:
                    return vertex => vertex.ToString();
            }

            // Create dictionary
            var ids = new Dictionary<TVertex, string>(graph.VertexCount);
            return vertex =>
            {
                if (!ids.TryGetValue(vertex, out string id))
                    ids[vertex] = id = ids.Count.ToString();
                return id;
            };
        }

        /// <summary>
        /// Gets the edge identity.
        /// </summary>
        /// <typeparam name="TVertex">Vertex type.</typeparam>
        /// <typeparam name="TEdge">Edge type.</typeparam>
        /// <param name="graph">The graph.</param>
        /// <returns>A function that computes an edge identity for the given <paramref name="graph"/>.</returns>
#if SUPPORTS_CONTRACTS
        [System.Diagnostics.Contracts.Pure]
#endif
        [JetBrains.Annotations.Pure]
        [NotNull]
        public static EdgeIdentity<TVertex, TEdge> GetEdgeIdentity<TVertex, TEdge>([NotNull] this IEdgeSet<TVertex, TEdge> graph)
            where TEdge : IEdge<TVertex>
        {
#if SUPPORTS_CONTRACTS
            Contract.Requires(graph != null);
#endif

            // Create dictionary
            var ids = new Dictionary<TEdge, string>(graph.EdgeCount);
            return edge =>
            {
                if (!ids.TryGetValue(edge, out string id))
                    ids[edge] = id = ids.Count.ToString();
                return id;
            };
        }

#if SUPPORTS_CONTRACTS
        [System.Diagnostics.Contracts.Pure]
#endif
        [JetBrains.Annotations.Pure]
        [NotNull]
        private static TryFunc<TVertex, IEnumerable<TEdge>> RunDirectedRootedAlgorithm<TVertex, TEdge, TAlgorithm>(
            [NotNull] TVertex source,
            [NotNull] TAlgorithm algorithm)
            where TEdge : IEdge<TVertex>
            where TAlgorithm : RootedAlgorithmBase<TVertex, IVertexListGraph<TVertex, TEdge>>, ITreeBuilderAlgorithm<TVertex, TEdge>
        {
#if SUPPORTS_CONTRACTS
            Contract.Requires(algorithm != null);
#endif

            var predecessorRecorder = new VertexPredecessorRecorderObserver<TVertex, TEdge>();
            using (predecessorRecorder.Attach(algorithm))
                algorithm.Compute(source);

            IDictionary<TVertex, TEdge> predecessors = predecessorRecorder.VertexPredecessors;
            return (TVertex vertex, out IEnumerable<TEdge> edges) => predecessors.TryGetPath(vertex, out edges);
        }

        /// <summary>
        /// Computes a breadth first tree and gets a function that allow to get edges
        /// connected to a vertex in a directed graph.
        /// </summary>
        /// <remarks>Use <see cref="BreadthFirstSearchAlgorithm{TVertex,TEdge}"/> algorithm.</remarks>
        /// <typeparam name="TVertex">Vertex type.</typeparam>
        /// <typeparam name="TEdge">Edge type.</typeparam>
        /// <param name="graph">The graph to visit.</param>
        /// <param name="root">Starting vertex.</param>
        /// <returns>A function that allow to get edges connected to a given vertex.</returns>
#if SUPPORTS_CONTRACTS
        [System.Diagnostics.Contracts.Pure]
#endif
        [JetBrains.Annotations.Pure]
        [NotNull]
        public static TryFunc<TVertex, IEnumerable<TEdge>> TreeBreadthFirstSearch<TVertex, TEdge>(
            [NotNull] this IVertexListGraph<TVertex, TEdge> graph,
            [NotNull] TVertex root)
            where TEdge : IEdge<TVertex>
        {
#if SUPPORTS_CONTRACTS
            Contract.Requires(graph != null);
            Contract.Requires(root != null);
            Contract.Requires(graph.ContainsVertex(root));
            Contract.Ensures(Contract.Result<TryFunc<TVertex, IEnumerable<TEdge>>>() != null);
#endif

            var algorithm = new BreadthFirstSearchAlgorithm<TVertex, TEdge>(graph);
            return RunDirectedRootedAlgorithm<TVertex, TEdge, BreadthFirstSearchAlgorithm<TVertex, TEdge>>(
                root,
                algorithm);
        }

        /// <summary>
        /// Computes a depth first tree and gets a function that allow to get edges
        /// connected to a vertex in a directed graph.
        /// </summary>
        /// <remarks>Use <see cref="DepthFirstSearchAlgorithm{TVertex,TEdge}"/> algorithm.</remarks>
        /// <typeparam name="TVertex">Vertex type.</typeparam>
        /// <typeparam name="TEdge">Edge type.</typeparam>
        /// <param name="graph">The graph to visit.</param>
        /// <param name="root">Starting vertex.</param>
        /// <returns>A function that allow to get edges connected to a given vertex.</returns>
#if SUPPORTS_CONTRACTS
        [System.Diagnostics.Contracts.Pure]
#endif
        [JetBrains.Annotations.Pure]
        [NotNull]
        public static TryFunc<TVertex, IEnumerable<TEdge>> TreeDepthFirstSearch<TVertex, TEdge>(
            [NotNull] this IVertexListGraph<TVertex, TEdge> graph,
            [NotNull] TVertex root)
            where TEdge : IEdge<TVertex>
        {
#if SUPPORTS_CONTRACTS
            Contract.Requires(graph != null);
            Contract.Requires(root != null);
            Contract.Requires(graph.ContainsVertex(root));
            Contract.Ensures(Contract.Result<TryFunc<TVertex, IEnumerable<TEdge>>>() != null);
#endif

            var algorithm = new DepthFirstSearchAlgorithm<TVertex, TEdge>(graph);
            return RunDirectedRootedAlgorithm<TVertex, TEdge, DepthFirstSearchAlgorithm<TVertex, TEdge>>(
                root,
                algorithm);
        }

        /// <summary>
        /// Computes a cycle popping tree and gets a function that allow to get edges
        /// connected to a vertex in a directed graph.
        /// </summary>
        /// <remarks>Use <see cref="CyclePoppingRandomTreeAlgorithm{TVertex,TEdge}"/> algorithm and
        /// <see cref="NormalizedMarkovEdgeChain{TVertex,TEdge}"/>.</remarks>
        /// <typeparam name="TVertex">Vertex type.</typeparam>
        /// <typeparam name="TEdge">Edge type.</typeparam>
        /// <param name="graph">The graph to visit.</param>
        /// <param name="root">Starting vertex.</param>
        /// <returns>A function that allow to get edges connected to a given vertex.</returns>
#if SUPPORTS_CONTRACTS
        [System.Diagnostics.Contracts.Pure]
#endif
        [JetBrains.Annotations.Pure]
        [NotNull]
        public static TryFunc<TVertex, IEnumerable<TEdge>> TreeCyclePoppingRandom<TVertex, TEdge>(
            [NotNull] this IVertexListGraph<TVertex, TEdge> graph,
            [NotNull] TVertex root)
            where TEdge : IEdge<TVertex>
        {
            return TreeCyclePoppingRandom(graph, root, new NormalizedMarkovEdgeChain<TVertex, TEdge>());
        }

        /// <summary>
        /// Computes a cycle popping tree and gets a function that allow to get edges
        /// connected to a vertex in a directed graph.
        /// </summary>
        /// <remarks>Use <see cref="CyclePoppingRandomTreeAlgorithm{TVertex,TEdge}"/> algorithm.</remarks>
        /// <typeparam name="TVertex">Vertex type.</typeparam>
        /// <typeparam name="TEdge">Edge type.</typeparam>
        /// <param name="graph">The graph to visit.</param>
        /// <param name="root">Starting vertex.</param>
        /// <param name="edgeChain">Edge chain handler.</param>
        /// <returns>A function that allow to get edges connected to a given vertex.</returns>
#if SUPPORTS_CONTRACTS
        [System.Diagnostics.Contracts.Pure]
#endif
        [JetBrains.Annotations.Pure]
        [NotNull]
        public static TryFunc<TVertex, IEnumerable<TEdge>> TreeCyclePoppingRandom<TVertex, TEdge>(
            [NotNull] this IVertexListGraph<TVertex, TEdge> graph,
            [NotNull] TVertex root,
            [NotNull] IMarkovEdgeChain<TVertex, TEdge> edgeChain)
            where TEdge : IEdge<TVertex>
        {
#if SUPPORTS_CONTRACTS
            Contract.Requires(graph != null);
            Contract.Requires(root != null);
            Contract.Requires(edgeChain != null);
            Contract.Requires(graph.ContainsVertex(root));
            Contract.Ensures(Contract.Result<TryFunc<TVertex, IEnumerable<TEdge>>>() != null);
#endif

            var algorithm = new CyclePoppingRandomTreeAlgorithm<TVertex, TEdge>(graph, edgeChain);
            return RunDirectedRootedAlgorithm<TVertex, TEdge, CyclePoppingRandomTreeAlgorithm<TVertex, TEdge>>(
                root,
                algorithm);
        }

        #region Shortest paths

        /// <summary>
        /// Computes shortest path with the Dijkstra algorithm and gets a function that allows
        /// to get paths in a directed graph.
        /// </summary>
        /// <remarks>Use <see cref="DijkstraShortestPathAlgorithm{TVertex,TEdge}"/> algorithm.</remarks>
        /// <typeparam name="TVertex">Vertex type.</typeparam>
        /// <typeparam name="TEdge">Edge type.</typeparam>
        /// <param name="graph">The graph to visit.</param>
        /// <param name="edgeWeights">Function that computes the weight for a given edge.</param>
        /// <param name="root">Starting vertex.</param>
        /// <returns>A function that allow to get paths starting from <paramref name="root"/> vertex.</returns>
#if SUPPORTS_CONTRACTS
        [System.Diagnostics.Contracts.Pure]
#endif
        [JetBrains.Annotations.Pure]
        [NotNull]
        public static TryFunc<TVertex, IEnumerable<TEdge>> ShortestPathsDijkstra<TVertex, TEdge>(
            [NotNull] this IVertexAndEdgeListGraph<TVertex, TEdge> graph,
            [NotNull] Func<TEdge, double> edgeWeights,
            [NotNull] TVertex root)
            where TEdge : IEdge<TVertex>
        {
#if SUPPORTS_CONTRACTS
            Contract.Requires(graph != null);
            Contract.Requires(edgeWeights != null);
            Contract.Requires(root != null);
#endif

            var algorithm = new DijkstraShortestPathAlgorithm<TVertex, TEdge>(graph, edgeWeights);
            return RunDirectedRootedAlgorithm<TVertex, TEdge, DijkstraShortestPathAlgorithm<TVertex, TEdge>>(
                root,
                algorithm);
        }

        /// <summary>
        /// Computes shortest path with the Dijkstra algorithm and gets a function that allows
        /// to get paths in an undirected graph.
        /// </summary>
        /// <remarks>Use <see cref="UndirectedDijkstraShortestPathAlgorithm{TVertex,TEdge}"/> algorithm.</remarks>
        /// <typeparam name="TVertex">Vertex type.</typeparam>
        /// <typeparam name="TEdge">Edge type.</typeparam>
        /// <param name="graph">The graph to visit.</param>
        /// <param name="edgeWeights">Function that computes the weight for a given edge.</param>
        /// <param name="root">Starting vertex.</param>
        /// <returns>A function that allow to get paths starting from <paramref name="root"/> vertex.</returns>
#if SUPPORTS_CONTRACTS
        [System.Diagnostics.Contracts.Pure]
#endif
        [JetBrains.Annotations.Pure]
        [NotNull]
        public static TryFunc<TVertex, IEnumerable<TEdge>> ShortestPathsDijkstra<TVertex, TEdge>(
            [NotNull] this IUndirectedGraph<TVertex, TEdge> graph,
            [NotNull] Func<TEdge, double> edgeWeights,
            [NotNull] TVertex root)
            where TEdge : IEdge<TVertex>
        {
#if SUPPORTS_CONTRACTS
            Contract.Requires(graph != null);
            Contract.Requires(edgeWeights != null);
            Contract.Requires(root != null);
#endif

            var algorithm = new UndirectedDijkstraShortestPathAlgorithm<TVertex, TEdge>(graph, edgeWeights);
            var predecessorRecorder = new UndirectedVertexPredecessorRecorderObserver<TVertex, TEdge>();
            using (predecessorRecorder.Attach(algorithm))
                algorithm.Compute(root);

            IDictionary<TVertex, TEdge> predecessors = predecessorRecorder.VertexPredecessors;
            return (TVertex vertex, out IEnumerable<TEdge> edges) => predecessors.TryGetPath(vertex, out edges);
        }

        /// <summary>
        /// Computes shortest path with the A* algorithm and gets a function that allows
        /// to get paths in a directed graph.
        /// </summary>
        /// <remarks>Use <see cref="AStarShortestPathAlgorithm{TVertex,TEdge}"/> algorithm.</remarks>
        /// <typeparam name="TVertex">Vertex type.</typeparam>
        /// <typeparam name="TEdge">Edge type.</typeparam>
        /// <param name="graph">The graph to visit.</param>
        /// <param name="edgeWeights">Function that computes the weight for a given edge.</param>
        /// <param name="costHeuristic">Function that computes a cost for a given vertex.</param>
        /// <param name="root">Starting vertex.</param>
        /// <returns>A function that allow to get paths starting from <paramref name="root"/> vertex.</returns>
#if SUPPORTS_CONTRACTS
        [System.Diagnostics.Contracts.Pure]
#endif
        [JetBrains.Annotations.Pure]
        [NotNull]
        public static TryFunc<TVertex, IEnumerable<TEdge>> ShortestPathsAStar<TVertex, TEdge>(
            [NotNull] this IVertexAndEdgeListGraph<TVertex, TEdge> graph,
            [NotNull] Func<TEdge, double> edgeWeights,
            [NotNull] Func<TVertex, double> costHeuristic,
            [NotNull] TVertex root)
            where TEdge : IEdge<TVertex>
        {
#if SUPPORTS_CONTRACTS
            Contract.Requires(graph != null);
            Contract.Requires(edgeWeights != null);
            Contract.Requires(costHeuristic != null);
            Contract.Requires(root != null);
#endif

            var algorithm = new AStarShortestPathAlgorithm<TVertex, TEdge>(graph, edgeWeights, costHeuristic);
            return RunDirectedRootedAlgorithm<TVertex, TEdge, AStarShortestPathAlgorithm<TVertex, TEdge>>(
                root,
                algorithm);
        }

        /// <summary>
        /// Computes shortest path with the Bellman Ford algorithm and gets a function that allows
        /// to get paths in a directed graph.
        /// </summary>
        /// <remarks>Use <see cref="BellmanFordShortestPathAlgorithm{TVertex,TEdge}"/> algorithm.</remarks>
        /// <typeparam name="TVertex">Vertex type.</typeparam>
        /// <typeparam name="TEdge">Edge type.</typeparam>
        /// <param name="graph">The graph to visit.</param>
        /// <param name="edgeWeights">Function that computes the weight for a given edge.</param>
        /// <param name="root">Starting vertex.</param>
        /// <returns>A function that allow to get paths starting from <paramref name="root"/> vertex.</returns>
#if SUPPORTS_CONTRACTS
        [System.Diagnostics.Contracts.Pure]
#endif
        [JetBrains.Annotations.Pure]
        [NotNull]
        public static TryFunc<TVertex, IEnumerable<TEdge>> ShortestPathsBellmanFord<TVertex, TEdge>(
            [NotNull] this IVertexAndEdgeListGraph<TVertex, TEdge> graph,
            [NotNull] Func<TEdge, double> edgeWeights,
            [NotNull] TVertex root)
            where TEdge : IEdge<TVertex>
        {
#if SUPPORTS_CONTRACTS
            Contract.Requires(graph != null);
            Contract.Requires(edgeWeights != null);
            Contract.Requires(root != null);
#endif

            var algorithm = new BellmanFordShortestPathAlgorithm<TVertex, TEdge>(graph, edgeWeights);
            var predecessorRecorder = new VertexPredecessorRecorderObserver<TVertex, TEdge>();
            using (predecessorRecorder.Attach(algorithm))
                algorithm.Compute(root);

            IDictionary<TVertex, TEdge> predecessors = predecessorRecorder.VertexPredecessors;
            return (TVertex vertex, out IEnumerable<TEdge> edges) => predecessors.TryGetPath(vertex, out edges);
        }

        /// <summary>
        /// Computes shortest path with an algorithm made for DAG (Directed ACyclic graph) and gets a function
        /// that allows to get paths.
        /// </summary>
        /// <remarks>Use <see cref="DagShortestPathAlgorithm{TVertex,TEdge}"/> algorithm.</remarks>
        /// <typeparam name="TVertex">Vertex type.</typeparam>
        /// <typeparam name="TEdge">Edge type.</typeparam>
        /// <param name="graph">The graph to visit.</param>
        /// <param name="edgeWeights">Function that computes the weight for a given edge.</param>
        /// <param name="root">Starting vertex.</param>
        /// <returns>A function that allow to get paths starting from <paramref name="root"/> vertex.</returns>
#if SUPPORTS_CONTRACTS
        [System.Diagnostics.Contracts.Pure]
#endif
        [JetBrains.Annotations.Pure]
        [NotNull]
        public static TryFunc<TVertex, IEnumerable<TEdge>> ShortestPathsDag<TVertex, TEdge>(
            [NotNull] this IVertexAndEdgeListGraph<TVertex, TEdge> graph,
            [NotNull] Func<TEdge, double> edgeWeights,
            [NotNull] TVertex root)
            where TEdge : IEdge<TVertex>
        {
#if SUPPORTS_CONTRACTS
            Contract.Requires(graph != null);
            Contract.Requires(edgeWeights != null);
            Contract.Requires(root != null);
#endif

            var algorithm = new DagShortestPathAlgorithm<TVertex, TEdge>(graph, edgeWeights);
            return RunDirectedRootedAlgorithm<TVertex, TEdge, DagShortestPathAlgorithm<TVertex, TEdge>>(
                root,
                algorithm);
        }

        #endregion

        #region K-Shortest path

        /// <summary>
        /// Computes k-shortest path with the Hoffman Pavley algorithm and gets those paths.
        /// </summary>
        /// <remarks>Use <see cref="HoffmanPavleyRankedShortestPathAlgorithm{TVertex,TEdge}"/> algorithm.</remarks>
        /// <typeparam name="TVertex">Vertex type.</typeparam>
        /// <typeparam name="TEdge">Edge type.</typeparam>
        /// <param name="graph">The graph to visit.</param>
        /// <param name="edgeWeights">Function that computes the weight for a given edge.</param>
        /// <param name="root">Starting vertex.</param>
        /// <param name="target">Target vertex.</param>
        /// <param name="maxCount">Maximal number of path to search.</param>
        /// <returns>ENumeration of paths to go from <paramref name="root"/> vertex to <paramref name="target"/>.</returns>
#if SUPPORTS_CONTRACTS
        [System.Diagnostics.Contracts.Pure]
#endif
        [JetBrains.Annotations.Pure]
        [NotNull, ItemNotNull]
        public static IEnumerable<IEnumerable<TEdge>> RankedShortestPathHoffmanPavley<TVertex, TEdge>(
            [NotNull] this IBidirectionalGraph<TVertex, TEdge> graph,
            [NotNull] Func<TEdge, double> edgeWeights,
            [NotNull] TVertex root,
            [NotNull] TVertex target,
            int maxCount)
            where TEdge : IEdge<TVertex>
        {
#if SUPPORTS_CONTRACTS
            Contract.Requires(graph != null);
            Contract.Requires(edgeWeights != null);
            Contract.Requires(root != null && graph.ContainsVertex(root));
            Contract.Requires(target != null && graph.ContainsVertex(target));
            Contract.Requires(maxCount > 1);
#endif

            var algorithm = new HoffmanPavleyRankedShortestPathAlgorithm<TVertex, TEdge>(graph, edgeWeights)
            {
                ShortestPathCount = maxCount
            };
            algorithm.Compute(root, target);

            return algorithm.ComputedShortestPaths;
        }

        #endregion

        /// <summary>
        /// Gets set of sink vertices.
        /// </summary>
        /// <typeparam name="TVertex">Vertex type.</typeparam>
        /// <typeparam name="TEdge">Edge type.</typeparam>
        /// <param name="graph">Graph to visit.</param>
        /// <returns>Sink vertices.</returns>
#if SUPPORTS_CONTRACTS
        [System.Diagnostics.Contracts.Pure]
#endif
        [JetBrains.Annotations.Pure]
        [NotNull, ItemNotNull]
        public static IEnumerable<TVertex> Sinks<TVertex, TEdge>(
            [NotNull] this IVertexListGraph<TVertex, TEdge> graph)
            where TEdge : IEdge<TVertex>
        {
#if SUPPORTS_CONTRACTS
            Contract.Requires(graph != null);
#endif

            return graph.Vertices.Where(graph.IsOutEdgesEmpty);
        }

        /// <summary>
        /// Gets set of root vertices.
        /// </summary>
        /// <typeparam name="TVertex">Vertex type.</typeparam>
        /// <typeparam name="TEdge">Edge type.</typeparam>
        /// <param name="graph">Graph to visit.</param>
        /// <returns>Root vertices.</returns>
#if SUPPORTS_CONTRACTS
        [System.Diagnostics.Contracts.Pure]
#endif
        [JetBrains.Annotations.Pure]
        [NotNull, ItemNotNull]
        public static IEnumerable<TVertex> Roots<TVertex, TEdge>(
            [NotNull] this IVertexListGraph<TVertex, TEdge> graph)
            where TEdge : IEdge<TVertex>
        {
#if SUPPORTS_CONTRACTS
            Contract.Requires(graph != null);
#endif

            var notRoots = new Dictionary<TVertex, bool>(graph.VertexCount);
            var dfs = new DepthFirstSearchAlgorithm<TVertex, TEdge>(graph);
            dfs.ExamineEdge += edge => notRoots[edge.Target] = false;
            dfs.Compute();

            foreach (TVertex vertex in graph.Vertices)
            {
                if (!notRoots.TryGetValue(vertex, out _))
                    yield return vertex;
            }
        }

        /// <summary>
        /// Gets set of root vertices.
        /// </summary>
        /// <typeparam name="TVertex">Vertex type.</typeparam>
        /// <typeparam name="TEdge">Edge type.</typeparam>
        /// <param name="graph">Graph to visit.</param>
        /// <returns>Root vertices.</returns>
#if SUPPORTS_CONTRACTS
        [System.Diagnostics.Contracts.Pure]
#endif
        [JetBrains.Annotations.Pure]
        [NotNull, ItemNotNull]
        public static IEnumerable<TVertex> Roots<TVertex, TEdge>(
            [NotNull] this IBidirectionalGraph<TVertex, TEdge> graph)
            where TEdge : IEdge<TVertex>
        {
#if SUPPORTS_CONTRACTS
            Contract.Requires(graph != null);
#endif

            return graph.Vertices.Where(graph.IsInEdgesEmpty);
        }

        /// <summary>
        /// Gets set of isolated vertices (no incoming nor outcoming vertices).
        /// </summary>
        /// <typeparam name="TVertex">Vertex type.</typeparam>
        /// <typeparam name="TEdge">Edge type.</typeparam>
        /// <param name="graph">Graph to visit.</param>
        /// <returns>Root vertices.</returns>
#if SUPPORTS_CONTRACTS
        [System.Diagnostics.Contracts.Pure]
#endif
        [JetBrains.Annotations.Pure]
        [NotNull, ItemNotNull]
        public static IEnumerable<TVertex> IsolatedVertices<TVertex, TEdge>(
            [NotNull] this IBidirectionalGraph<TVertex, TEdge> graph)
            where TEdge : IEdge<TVertex>
        {
#if SUPPORTS_CONTRACTS
            Contract.Requires(graph != null);
#endif

            return graph.Vertices.Where(vertex => graph.Degree(vertex) == 0);
        }

        #region Topological sorts

        /// <summary>
        /// Creates a topological sort of an undirected acyclic graph.
        /// </summary>
        /// <typeparam name="TVertex">Vertex type.</typeparam>
        /// <typeparam name="TEdge">Edge type.</typeparam>
        /// <param name="graph">Graph to visit.</param>
        /// <returns>Sorted vertices (topological sort)</returns>
        /// <exception cref="NonAcyclicGraphException">If the input graph has a cycle.</exception>
#if SUPPORTS_CONTRACTS
        [System.Diagnostics.Contracts.Pure]
#endif
        [JetBrains.Annotations.Pure]
        [NotNull, ItemNotNull]
        public static IEnumerable<TVertex> TopologicalSort<TVertex, TEdge>(
            [NotNull] this IUndirectedGraph<TVertex, TEdge> graph)
            where TEdge : IEdge<TVertex>
        {
#if SUPPORTS_CONTRACTS
            Contract.Requires(graph != null);
#endif

            var vertices = new List<TVertex>(graph.VertexCount);
            TopologicalSort(graph, vertices);
            return vertices.AsEnumerable();
        }

        /// <summary>
        /// Creates a topological sort of an undirected acyclic graph.
        /// </summary>
        /// <typeparam name="TVertex">Vertex type.</typeparam>
        /// <typeparam name="TEdge">Edge type.</typeparam>
        /// <param name="graph">Graph to visit.</param>
        /// <param name="vertices">Collection in which sorted vertices will be put.</param>
        /// <exception cref="NonAcyclicGraphException">If the input graph has a cycle.</exception>
        public static void TopologicalSort<TVertex, TEdge>(
            this IUndirectedGraph<TVertex, TEdge> graph,
            [NotNull, ItemNotNull] IList<TVertex> vertices)
            where TEdge : IEdge<TVertex>
        {
#if SUPPORTS_CONTRACTS
            Contract.Requires(graph != null);
            Contract.Requires(vertices != null);
#endif

            var algorithm = new UndirectedTopologicalSortAlgorithm<TVertex, TEdge>(graph);
            algorithm.Compute(vertices);
        }

        /// <summary>
        /// Creates a topological sort of a directed acyclic graph.
        /// </summary>
        /// <typeparam name="TVertex">Vertex type.</typeparam>
        /// <typeparam name="TEdge">Edge type.</typeparam>
        /// <param name="graph">Graph to visit.</param>
        /// <returns>Sorted vertices (topological sort)</returns>
        /// <exception cref="NonAcyclicGraphException">If the input graph has a cycle.</exception>
#if SUPPORTS_CONTRACTS
        [System.Diagnostics.Contracts.Pure]
#endif
        [JetBrains.Annotations.Pure]
        [NotNull, ItemNotNull]
        public static IEnumerable<TVertex> TopologicalSort<TVertex, TEdge>(
            [NotNull] this IVertexListGraph<TVertex, TEdge> graph)
            where TEdge : IEdge<TVertex>
        {
#if SUPPORTS_CONTRACTS
            Contract.Requires(graph != null);
#endif

            var vertices = new List<TVertex>(graph.VertexCount);
            TopologicalSort(graph, vertices);
            return vertices.AsEnumerable();
        }

        /// <summary>
        /// Creates a topological sort of a directed acyclic graph.
        /// </summary>
        /// <typeparam name="TVertex">Vertex type.</typeparam>
        /// <typeparam name="TEdge">Edge type.</typeparam>
        /// <param name="graph">Graph to visit.</param>
        /// <param name="vertices">Collection in which sorted vertices will be put.</param>
        /// <exception cref="NonAcyclicGraphException">If the input graph has a cycle.</exception>
        public static void TopologicalSort<TVertex, TEdge>(
            [NotNull] this IVertexListGraph<TVertex, TEdge> graph,
            [NotNull, ItemNotNull] IList<TVertex> vertices)
            where TEdge : IEdge<TVertex>
        {
#if SUPPORTS_CONTRACTS
            Contract.Requires(graph != null);
            Contract.Requires(vertices != null);
#endif

            var algorithm = new TopologicalSortAlgorithm<TVertex, TEdge>(graph);
            algorithm.Compute(vertices);
        }

        /// <summary>
        /// Creates a topological sort (source first) of a directed acyclic graph.
        /// </summary>
        /// <typeparam name="TVertex">Vertex type.</typeparam>
        /// <typeparam name="TEdge">Edge type.</typeparam>
        /// <param name="graph">Graph to visit.</param>
        /// <returns>Sorted vertices (topological sort)</returns>
        /// <exception cref="NonAcyclicGraphException">If the input graph has a cycle.</exception>
#if SUPPORTS_CONTRACTS
        [System.Diagnostics.Contracts.Pure]
#endif
        [JetBrains.Annotations.Pure]
        [NotNull, ItemNotNull]
        public static IEnumerable<TVertex> SourceFirstTopologicalSort<TVertex, TEdge>(
            [NotNull] this IVertexAndEdgeListGraph<TVertex, TEdge> graph)
            where TEdge : IEdge<TVertex>
        {
#if SUPPORTS_CONTRACTS
            Contract.Requires(graph != null);
#endif

            var vertices = new List<TVertex>(graph.VertexCount);
            SourceFirstTopologicalSort(graph, vertices);
            return vertices.AsEnumerable();
        }

        /// <summary>
        /// Creates a topological sort (source first) of a directed acyclic graph.
        /// </summary>
        /// <typeparam name="TVertex">Vertex type.</typeparam>
        /// <typeparam name="TEdge">Edge type.</typeparam>
        /// <param name="graph">Graph to visit.</param>
        /// <param name="vertices">Collection in which sorted vertices will be put.</param>
        /// <exception cref="NonAcyclicGraphException">If the input graph has a cycle.</exception>
        public static void SourceFirstTopologicalSort<TVertex, TEdge>(
            [NotNull] this IVertexAndEdgeListGraph<TVertex, TEdge> graph,
            [NotNull, ItemNotNull] IList<TVertex> vertices)
            where TEdge : IEdge<TVertex>
        {
#if SUPPORTS_CONTRACTS
            Contract.Requires(graph != null);
            Contract.Requires(vertices != null);
#endif

            var algorithm = new SourceFirstTopologicalSortAlgorithm<TVertex, TEdge>(graph);
            algorithm.Compute(vertices);
        }

        /// <summary>
        /// Creates a topological sort (source first) of a bidirectional directed acyclic graph.
        /// </summary>
        /// <typeparam name="TVertex">Vertex type.</typeparam>
        /// <typeparam name="TEdge">Edge type.</typeparam>
        /// <param name="graph">Graph to visit.</param>
        /// <param name="direction">Topological sort direction.</param>
        /// <returns>Sorted vertices (topological sort)</returns>
        /// <exception cref="NonAcyclicGraphException">If the input graph has a cycle.</exception>
#if SUPPORTS_CONTRACTS
        [System.Diagnostics.Contracts.Pure]
#endif
        [JetBrains.Annotations.Pure]
        [NotNull, ItemNotNull]
        public static IEnumerable<TVertex> SourceFirstBidirectionalTopologicalSort<TVertex, TEdge>(
            [NotNull] this IBidirectionalGraph<TVertex, TEdge> graph,
            TopologicalSortDirection direction)
            where TEdge : IEdge<TVertex>
        {
#if SUPPORTS_CONTRACTS
            Contract.Requires(graph != null);
#endif

            var vertices = new List<TVertex>(graph.VertexCount);
            SourceFirstBidirectionalTopologicalSort(graph, vertices, direction);
            return vertices.AsEnumerable();
        }

        /// <summary>
        /// Creates a topological sort (source first) of a bidirectional directed acyclic graph.
        /// Uses the <see cref="TopologicalSortDirection.Forward"/> direction.
        /// </summary>
        /// <typeparam name="TVertex">Vertex type.</typeparam>
        /// <typeparam name="TEdge">Edge type.</typeparam>
        /// <param name="graph">Graph to visit.</param>
        /// <returns>Sorted vertices (topological sort)</returns>
        /// <exception cref="NonAcyclicGraphException">If the input graph has a cycle.</exception>
#if SUPPORTS_CONTRACTS
        [System.Diagnostics.Contracts.Pure]
#endif
        [JetBrains.Annotations.Pure]
        public static IEnumerable<TVertex> SourceFirstBidirectionalTopologicalSort<TVertex, TEdge>(
            [NotNull] this IBidirectionalGraph<TVertex, TEdge> graph)
            where TEdge : IEdge<TVertex>
        {
            return SourceFirstBidirectionalTopologicalSort(graph, TopologicalSortDirection.Forward);
        }

        /// <summary>
        /// Creates a topological sort (source first) of a bidirectional directed acyclic graph.
        /// </summary>
        /// <typeparam name="TVertex">Vertex type.</typeparam>
        /// <typeparam name="TEdge">Edge type.</typeparam>
        /// <param name="graph">Graph to visit.</param>
        /// <param name="vertices">Collection in which sorted vertices will be put.</param>
        /// <param name="direction">Topological sort direction.</param>
        /// <exception cref="NonAcyclicGraphException">If the input graph has a cycle.</exception>
        public static void SourceFirstBidirectionalTopologicalSort<TVertex, TEdge>(
            [NotNull] this IBidirectionalGraph<TVertex, TEdge> graph,
            [NotNull, ItemNotNull] IList<TVertex> vertices,
            TopologicalSortDirection direction)
            where TEdge : IEdge<TVertex>
        {
#if SUPPORTS_CONTRACTS
            Contract.Requires(graph != null);
            Contract.Requires(vertices != null);
#endif

            var algorithm = new SourceFirstBidirectionalTopologicalSortAlgorithm<TVertex, TEdge>(graph, direction);
            algorithm.Compute(vertices);
        }

        /// <summary>
        /// Creates a topological sort (source first) of a bidirectional directed acyclic graph.
        /// Uses the <see cref="TopologicalSortDirection.Forward"/> direction.
        /// </summary>
        /// <typeparam name="TVertex">Vertex type.</typeparam>
        /// <typeparam name="TEdge">Edge type.</typeparam>
        /// <param name="graph">Graph to visit.</param>
        /// <param name="vertices">Collection in which sorted vertices will be put.</param>
        /// <exception cref="NonAcyclicGraphException">If the input graph has a cycle.</exception>
        public static void SourceFirstBidirectionalTopologicalSort<TVertex, TEdge>(
            [NotNull] this IBidirectionalGraph<TVertex, TEdge> graph,
            [NotNull, ItemNotNull] IList<TVertex> vertices)
            where TEdge : IEdge<TVertex>
        {
            SourceFirstBidirectionalTopologicalSort(graph, vertices, TopologicalSortDirection.Forward);
        }

        #endregion

        #region Connected components

        /// <summary>
        /// Computes the connected components of an undirected graph.
        /// </summary>
        /// <typeparam name="TVertex">Vertex type.</typeparam>
        /// <typeparam name="TEdge">Edge type.</typeparam>
        /// <param name="graph">Graph to visit.</param>
        /// <param name="components">Found components.</param>
        /// <returns>Number of component found.</returns>
        public static int ConnectedComponents<TVertex, TEdge>(
            [NotNull] this IUndirectedGraph<TVertex, TEdge> graph,
            [NotNull] IDictionary<TVertex, int> components)
            where TEdge : IEdge<TVertex>
        {
#if SUPPORTS_CONTRACTS
            Contract.Requires(graph != null);
            Contract.Requires(components != null);
#endif

            var algorithm = new ConnectedComponentsAlgorithm<TVertex, TEdge>(graph, components);
            algorithm.Compute();
            return algorithm.ComponentCount;
        }

        /// <summary>
        /// Computes the incremental connected components for a growing graph (edge added only).
        /// Each call to the delegate re-computes the component dictionary. The returned dictionary
        /// is shared across multiple calls of the method.
        /// </summary>
        /// <typeparam name="TVertex">Vertex type.</typeparam>
        /// <typeparam name="TEdge">Edge type.</typeparam>
        /// <param name="graph">Graph to visit.</param>
        /// <returns>A function retrieve components of the <paramref name="graph"/>.</returns>
        [NotNull]
        public static Func<KeyValuePair<int, IDictionary<TVertex, int>>> IncrementalConnectedComponents<TVertex, TEdge>(
            [NotNull] this IMutableVertexAndEdgeSet<TVertex, TEdge> graph)
            where TEdge : IEdge<TVertex>
        {
#if SUPPORTS_CONTRACTS
            Contract.Requires(graph != null);
#endif

            var incrementalComponents = new IncrementalConnectedComponentsAlgorithm<TVertex, TEdge>(graph);
            incrementalComponents.Compute();

            return () => incrementalComponents.GetComponents();
        }

        /// <summary>
        /// Computes the strongly connected components of a directed graph.
        /// </summary>
        /// <typeparam name="TVertex">Vertex type.</typeparam>
        /// <typeparam name="TEdge">Edge type.</typeparam>
        /// <param name="graph">Graph to visit.</param>
        /// <param name="components">Found components.</param>
        /// <returns>Number of component found.</returns>
        public static int StronglyConnectedComponents<TVertex, TEdge>(
            [NotNull] this IVertexListGraph<TVertex, TEdge> graph,
            [NotNull] IDictionary<TVertex, int> components)
            where TEdge : IEdge<TVertex>
        {
#if SUPPORTS_CONTRACTS
            Contract.Requires(graph != null);
            Contract.Ensures(components != null);
#endif

            var algorithm = new StronglyConnectedComponentsAlgorithm<TVertex, TEdge>(graph, components);
            algorithm.Compute();
            return algorithm.ComponentCount;
        }

        /// <summary>
        /// Computes the weakly connected components of a directed graph.
        /// </summary>
        /// <typeparam name="TVertex">Vertex type.</typeparam>
        /// <typeparam name="TEdge">Edge type.</typeparam>
        /// <param name="graph">Graph to visit.</param>
        /// <param name="components">Found components.</param>
        /// <returns>Number of component found.</returns>
        public static int WeaklyConnectedComponents<TVertex, TEdge>(
            [NotNull] this IVertexListGraph<TVertex, TEdge> graph,
            [NotNull] IDictionary<TVertex, int> components)
            where TEdge : IEdge<TVertex>
        {
#if SUPPORTS_CONTRACTS
            Contract.Requires(graph != null);
            Contract.Requires(components != null);
#endif

            var algorithm = new WeaklyConnectedComponentsAlgorithm<TVertex, TEdge>(graph, components);
            algorithm.Compute();
            return algorithm.ComponentCount;
        }

        /// <summary>
        /// Condensates the strongly connected components of a directed graph.
        /// </summary>
        /// <typeparam name="TVertex">Vertex type.</typeparam>
        /// <typeparam name="TEdge">Edge type.</typeparam>
        /// <typeparam name="TGraph">Graph type.</typeparam>
        /// <param name="graph">Graph to visit.</param>
        /// <returns>The condensed graph.</returns>
        [NotNull]
        public static IMutableBidirectionalGraph<TGraph, CondensedEdge<TVertex, TEdge, TGraph>> CondensateStronglyConnected<TVertex, TEdge, TGraph>(
            [NotNull] this IVertexAndEdgeListGraph<TVertex, TEdge> graph)
            where TEdge : IEdge<TVertex>
            where TGraph : IMutableVertexAndEdgeSet<TVertex, TEdge>, new()
        {
#if SUPPORTS_CONTRACTS
            Contract.Requires(graph != null);
#endif

            var algorithm = new CondensationGraphAlgorithm<TVertex, TEdge, TGraph>(graph)
            {
                StronglyConnected = true
            };
            algorithm.Compute();
            return algorithm.CondensedGraph;
        }

        /// <summary>
        /// Condensates the weakly connected components of a directed graph.
        /// </summary>
        /// <typeparam name="TVertex">Vertex type.</typeparam>
        /// <typeparam name="TEdge">Edge type.</typeparam>
        /// <typeparam name="TGraph">Graph type.</typeparam>
        /// <param name="graph">Graph to visit.</param>
        /// <returns>The condensed graph.</returns>
        [NotNull]
        public static IMutableBidirectionalGraph<TGraph, CondensedEdge<TVertex, TEdge, TGraph>> CondensateWeaklyConnected<TVertex, TEdge, TGraph>(
            [NotNull] this IVertexAndEdgeListGraph<TVertex, TEdge> graph)
            where TEdge : IEdge<TVertex>
            where TGraph : IMutableVertexAndEdgeSet<TVertex, TEdge>, new()
        {
#if SUPPORTS_CONTRACTS
            Contract.Requires(graph != null);
#endif

            var algorithm = new CondensationGraphAlgorithm<TVertex, TEdge, TGraph>(graph)
            {
                StronglyConnected = false
            };
            algorithm.Compute();
            return algorithm.CondensedGraph;
        }

        /// <summary>
        /// Condensates the given bidirectional directed graph.
        /// </summary>
        /// <typeparam name="TVertex">Vertex type.</typeparam>
        /// <typeparam name="TEdge">Edge type.</typeparam>
        /// <param name="graph">Graph to visit.</param>
        /// <param name="vertexPredicate">Vertex predicate used to filter the vertices to put in the condensed graph.</param>
        /// <returns>The condensed graph.</returns>
        [NotNull]
        public static IMutableBidirectionalGraph<TVertex, MergedEdge<TVertex, TEdge>> CondensateEdges<TVertex, TEdge>(
            [NotNull] this IBidirectionalGraph<TVertex, TEdge> graph,
            [NotNull] VertexPredicate<TVertex> vertexPredicate)
            where TEdge : IEdge<TVertex>
        {
#if SUPPORTS_CONTRACTS
            Contract.Requires(graph != null);
            Contract.Requires(vertexPredicate != null);
#endif

            var condensedGraph = new BidirectionalGraph<TVertex, MergedEdge<TVertex, TEdge>>();
            var algorithm = new EdgeMergeCondensationGraphAlgorithm<TVertex, TEdge>(
                graph,
                condensedGraph,
                vertexPredicate);
            algorithm.Compute();

            return condensedGraph;
        }

        #endregion

        /// <summary>
        /// Gets odd vertices of the given <paramref name="graph"/>.
        /// </summary>
        /// <param name="graph">Graph to visit.</param>
        /// <returns>Enumerable of odd vertices.</returns>
#if SUPPORTS_CONTRACTS
        [System.Diagnostics.Contracts.Pure]
#endif
        [JetBrains.Annotations.Pure]
        [NotNull, ItemNotNull]
        public static IEnumerable<TVertex> OddVertices<TVertex, TEdge>(
            [NotNull] this IVertexAndEdgeListGraph<TVertex, TEdge> graph)
            where TEdge : IEdge<TVertex>
        {
#if SUPPORTS_CONTRACTS
            Contract.Requires(graph != null);
#endif

            var counts = new Dictionary<TVertex, int>(graph.VertexCount);
            foreach (TVertex vertex in graph.Vertices)
                counts.Add(vertex, 0);

            foreach (TEdge edge in graph.Edges)
            {
                ++counts[edge.Source];
                --counts[edge.Target];
            }

            // Odds
            return counts
                .Where(pair => pair.Value % 2 == 0)
                .Select(pair => pair.Key);
        }

        /// <summary>
        /// Checks whether the graph is acyclic or not.
        /// </summary>
        /// <remarks>
        /// Performs a depth first search to look for cycles.
        /// </remarks>
        /// <typeparam name="TVertex">Vertex type.</typeparam>
        /// <typeparam name="TEdge">Edge type.</typeparam>
        /// <param name="graph">Graph to visit.</param>
        /// <returns>True if the graph contains a cycle, false otherwise.</returns>
#if SUPPORTS_CONTRACTS
        [System.Diagnostics.Contracts.Pure]
#endif
        [JetBrains.Annotations.Pure]
        public static bool IsDirectedAcyclicGraph<TVertex, TEdge>(this IVertexListGraph<TVertex, TEdge> graph)
            where TEdge : IEdge<TVertex>
        {
#if SUPPORTS_CONTRACTS
            Contract.Requires(graph != null);
#endif

            return new DagTester<TVertex, TEdge>().IsDag(graph);
        }

        private class DagTester<TVertex, TEdge>
            where TEdge : IEdge<TVertex>
        {
            private bool _isDag = true;

            [JetBrains.Annotations.Pure]
            public bool IsDag([NotNull] IVertexListGraph<TVertex, TEdge> graph)
            {
                var dfs = new DepthFirstSearchAlgorithm<TVertex, TEdge>(graph);
                try
                {
                    dfs.BackEdge += DfsBackEdge;
                    _isDag = true;
                    dfs.Compute();
                    return _isDag;
                }
                finally
                {
                    dfs.BackEdge -= DfsBackEdge;
                }
            }

            private void DfsBackEdge([NotNull] TEdge edge)
            {
                _isDag = false;
            }
        }

        /// <summary>
        /// Given a edge cost map, computes the corresponding predecessor costs.
        /// </summary>
        /// <typeparam name="TVertex">Vertex type.</typeparam>
        /// <typeparam name="TEdge">Edge type.</typeparam>
        /// <param name="predecessors">Predecessors map.</param>
        /// <param name="edgeCosts">Costs map.</param>
        /// <param name="target">Target vertex.</param>
        /// <returns>The predecessors cost.</returns>
#if SUPPORTS_CONTRACTS
        [System.Diagnostics.Contracts.Pure]
#endif
        [JetBrains.Annotations.Pure]
        public static double ComputePredecessorCost<TVertex, TEdge>(
            [NotNull] IDictionary<TVertex, TEdge> predecessors,
            [NotNull] IDictionary<TEdge, double> edgeCosts,
            [NotNull] TVertex target)
            where TEdge : IEdge<TVertex>
        {
#if SUPPORTS_CONTRACTS
            Contract.Requires(predecessors != null);
            Contract.Requires(edgeCosts != null);
            Contract.Requires(target != null);
#endif

            double cost = 0;
            TVertex current = target;
            while (predecessors.TryGetValue(current, out TEdge edge))
            {
                cost += edgeCosts[edge];
                current = edge.Source;
            }

            return cost;
        }

        /// <summary>
        /// Computes disjoint sets of the given <paramref name="graph"/>.
        /// </summary>
        /// <typeparam name="TVertex">Vertex type.</typeparam>
        /// <typeparam name="TEdge">Edge type.</typeparam>
        /// <param name="graph">Graph to visit.</param>
        /// <returns>Found disjoint sets.</returns>
#if SUPPORTS_CONTRACTS
        [System.Diagnostics.Contracts.Pure]
#endif
        [JetBrains.Annotations.Pure]
        [NotNull]
        public static IDisjointSet<TVertex> ComputeDisjointSet<TVertex, TEdge>(
            [NotNull] this IUndirectedGraph<TVertex, TEdge> graph)
            where TEdge : IEdge<TVertex>
        {
#if SUPPORTS_CONTRACTS
            Contract.Requires(graph != null);
#endif

            var sets = new ForestDisjointSet<TVertex>(graph.VertexCount);
            foreach (TVertex vertex in graph.Vertices)
                sets.MakeSet(vertex);
            foreach (TEdge edge in graph.Edges)
                sets.Union(edge.Source, edge.Target);

            return sets;
        }

        /// <summary>
        /// Computes the minimum spanning tree using Prim algorithm.
        /// </summary>
        /// <remarks>Prim algorithm is simply implemented by calling Dijkstra shortest path.</remarks>
        /// <typeparam name="TVertex">Vertex type.</typeparam>
        /// <typeparam name="TEdge">Edge type.</typeparam>
        /// <param name="graph">Graph to visit.</param>
        /// <param name="edgeWeights">Function that computes the weight for a given edge.</param>
        /// <returns>Edges part of the minimum spanning tree.</returns>
#if SUPPORTS_CONTRACTS
        [System.Diagnostics.Contracts.Pure]
#endif
        [JetBrains.Annotations.Pure]
        [NotNull, ItemNotNull]
        public static IEnumerable<TEdge> MinimumSpanningTreePrim<TVertex, TEdge>(
            [NotNull] this IUndirectedGraph<TVertex, TEdge> graph,
            [NotNull] Func<TEdge, double> edgeWeights)
            where TEdge : IEdge<TVertex>
        {
#if SUPPORTS_CONTRACTS
            Contract.Requires(graph != null);
            Contract.Requires(edgeWeights != null);
#endif

            if (graph.VertexCount == 0)
                return Enumerable.Empty<TEdge>();

            IDistanceRelaxer distanceRelaxer = DistanceRelaxers.Prim;
            var dijkstra = new UndirectedDijkstraShortestPathAlgorithm<TVertex, TEdge>(graph, edgeWeights, distanceRelaxer);
            var edgeRecorder = new UndirectedVertexPredecessorRecorderObserver<TVertex, TEdge>();
            using (edgeRecorder.Attach(dijkstra))
                dijkstra.Compute();

            return edgeRecorder.VertexPredecessors.Values;
        }

        /// <summary>
        /// Computes the minimum spanning tree using Kruskal algorithm.
        /// </summary>
        /// <typeparam name="TVertex">Vertex type.</typeparam>
        /// <typeparam name="TEdge">Edge type.</typeparam>
        /// <param name="graph">Graph to visit.</param>
        /// <param name="edgeWeights">Function that computes the weight for a given edge.</param>
        /// <returns>Edges part of the minimum spanning tree.</returns>
#if SUPPORTS_CONTRACTS
        [System.Diagnostics.Contracts.Pure]
#endif
        [JetBrains.Annotations.Pure]
        [NotNull, ItemNotNull]
        public static IEnumerable<TEdge> MinimumSpanningTreeKruskal<TVertex, TEdge>(
            [NotNull] this IUndirectedGraph<TVertex, TEdge> graph,
            [NotNull] Func<TEdge, double> edgeWeights)
            where TEdge : IEdge<TVertex>
        {
#if SUPPORTS_CONTRACTS
            Contract.Requires(graph != null);
            Contract.Requires(edgeWeights != null);
#endif

            if (graph.VertexCount == 0)
                return Enumerable.Empty<TEdge>();

            var kruskal = new KruskalMinimumSpanningTreeAlgorithm<TVertex, TEdge>(graph, edgeWeights);
            var edgeRecorder = new EdgeRecorderObserver<TVertex, TEdge>();
            using (edgeRecorder.Attach(kruskal))
                kruskal.Compute();

            return edgeRecorder.Edges;
        }

        /// <summary>
        /// Computes the offline least common ancestor between pairs of vertices in a
        /// rooted tree using Tarjan algorithm.
        /// </summary>
        /// <remarks>
        /// Reference:
        /// Gabow, H. N. and Tarjan, R. E. 1983. A linear-time algorithm for a special case of disjoint set union.
        /// In Proceedings of the Fifteenth Annual ACM Symposium on theory of Computing STOC '83. ACM, New York, NY, 246-251.
        /// DOI= http://doi.acm.org/10.1145/800061.808753 
        /// </remarks>
        /// <typeparam name="TVertex">Vertex type.</typeparam>
        /// <typeparam name="TEdge">Edge type.</typeparam>
        /// <param name="graph">Graph to visit.</param>
        /// <param name="root">Starting vertex.</param>
        /// <param name="pairs">Vertices pairs.</param>
        /// <returns>A function that allow to get least common ancestor for a pair of vertices.</returns>
#if SUPPORTS_CONTRACTS
        [System.Diagnostics.Contracts.Pure]
#endif
        [JetBrains.Annotations.Pure]
        [NotNull]
        public static TryFunc<SEquatableEdge<TVertex>, TVertex> OfflineLeastCommonAncestor<TVertex, TEdge>(
            [NotNull] this IVertexListGraph<TVertex, TEdge> graph,
            [NotNull] TVertex root,
            [NotNull] IEnumerable<SEquatableEdge<TVertex>> pairs)
            where TEdge : IEdge<TVertex>
        {
#if SUPPORTS_CONTRACTS
            Contract.Requires(graph != null);
            Contract.Requires(root != null);
            Contract.Requires(pairs != null);
            Contract.Requires(graph.ContainsVertex(root));
            Contract.Requires(pairs.All(pair => graph.ContainsVertex(pair.Source)));
            Contract.Requires(pairs.All(pair => graph.ContainsVertex(pair.Target)));
#endif

            var algorithm = new TarjanOfflineLeastCommonAncestorAlgorithm<TVertex, TEdge>(graph);
            algorithm.Compute(root, pairs);

            var ancestors = algorithm.Ancestors;
            return (SEquatableEdge<TVertex> pair, out TVertex vertex) => ancestors.TryGetValue(pair, out vertex);
        }

        /// <summary>
        /// Computes the maximum flow for a graph with positive capacities and flows
        /// using Edmonds-Karp algorithm.
        /// </summary>
        /// <remarks>
        /// Will throw an exception in <see cref="ReversedEdgeAugmentorAlgorithm{TVertex,TEdge}.AddReversedEdges()"/> if TEdge is a value type,
        /// e.g. <see cref="SEdge{TVertex}"/>.
        /// <seealso href="https://github.com/YaccConstructor/QuickGraph/issues/183#issue-377613647"/>.
        /// </remarks>
        /// <typeparam name="TVertex">Vertex type.</typeparam>
        /// <typeparam name="TEdge">Edge type.</typeparam>
        /// <param name="graph">Graph to visit.</param>
        /// <param name="edgeCapacities">Function that given an edge return the capacity of this edge.</param>
        /// <param name="source">The source vertex.</param>
        /// <param name="sink">The sink vertex.</param>
        /// <param name="flowPredecessors">Function that allow to retrieve flow predecessors.</param>
        /// <param name="edgeFactory">Edge factory method.</param>
        /// <param name="reversedEdgeAugmentorAlgorithm">Algorithm that is in of charge of augmenting the graph (creating missing reversed edges).</param>
        /// <returns>The maximum flow.</returns>
#if SUPPORTS_CONTRACTS
        [System.Diagnostics.Contracts.Pure]
#endif
        [JetBrains.Annotations.Pure]
        public static double MaximumFlow<TVertex, TEdge>(
            [NotNull] this IMutableVertexAndEdgeListGraph<TVertex, TEdge> graph,
            [NotNull] Func<TEdge, double> edgeCapacities,
            [NotNull] TVertex source,
            [NotNull] TVertex sink,
            [NotNull] out TryFunc<TVertex, TEdge> flowPredecessors,
            [NotNull] EdgeFactory<TVertex, TEdge> edgeFactory,
            [NotNull] ReversedEdgeAugmentorAlgorithm<TVertex, TEdge> reversedEdgeAugmentorAlgorithm)
            where TEdge : IEdge<TVertex>
        {
#if SUPPORTS_CONTRACTS
            Contract.Requires(graph != null);
            Contract.Requires(edgeCapacities != null);
            Contract.Requires(source != null);
            Contract.Requires(sink != null);
            Contract.Requires(!source.Equals(sink));
            Contract.Requires(edgeFactory != null);
            Contract.Requires(reversedEdgeAugmentorAlgorithm != null);
#endif

            // Compute maximum flow
            var flow = new EdmondsKarpMaximumFlowAlgorithm<TVertex, TEdge>(
                graph,
                edgeCapacities,
                edgeFactory,
                reversedEdgeAugmentorAlgorithm);
            flow.Compute(source, sink);
            flowPredecessors = flow.Predecessors.TryGetValue;

            return flow.MaxFlow;
        }

        /// <summary>
        /// Computes the transitive reduction of the given <paramref name="graph"/>.
        /// </summary>
        /// <typeparam name="TVertex">Vertex type.</typeparam>
        /// <typeparam name="TEdge">Edge type.</typeparam>
        /// <param name="graph">Graph to compute the reduction.</param>
        /// <returns>Transitive graph reduction.</returns>
#if SUPPORTS_CONTRACTS
        [System.Diagnostics.Contracts.Pure]
#endif
        [JetBrains.Annotations.Pure]
        [NotNull]
        public static BidirectionalGraph<TVertex, TEdge> ComputeTransitiveReduction<TVertex, TEdge>(
            [NotNull] this BidirectionalGraph<TVertex, TEdge> graph)
            where TEdge : IEdge<TVertex>
        {
            var algorithm = new TransitiveReductionAlgorithm<TVertex, TEdge>(graph);
            algorithm.Compute();
            return algorithm.TransitiveReduction;
        }

        /// <summary>
        /// Computes the transitive close of the given <paramref name="graph"/>.
        /// </summary>
        /// <typeparam name="TVertex">Vertex type.</typeparam>
        /// <typeparam name="TEdge">Edge type.</typeparam>
        /// <param name="graph">Graph to compute the closure.</param>
        /// <param name="createEdge">Function that create an edge between the 2 given vertices.</param>
        /// <returns>Transitive graph closure.</returns>
#if SUPPORTS_CONTRACTS
        [System.Diagnostics.Contracts.Pure]
#endif
        [JetBrains.Annotations.Pure]
        [NotNull]
        public static BidirectionalGraph<TVertex, TEdge> ComputeTransitiveClosure<TVertex, TEdge>(
            [NotNull] this BidirectionalGraph<TVertex, TEdge> graph,
            [NotNull] Func<TVertex, TVertex, TEdge> createEdge)
            where TEdge : IEdge<TVertex>
        {
            var algorithm = new TransitiveClosureAlgorithm<TVertex, TEdge>(graph, createEdge);
            algorithm.Compute();
            return algorithm.TransitiveClosure;
        }

        /// <summary>
        /// Clones a graph to another graph.
        /// </summary>
        /// <typeparam name="TVertex">Vertex type.</typeparam>
        /// <typeparam name="TEdge">Edge type.</typeparam>
        /// <param name="graph">Graph to clone.</param>
        /// <param name="vertexCloner">Delegate to clone a vertex.</param>
        /// <param name="edgeCloner">Delegate to clone an edge.</param>
        /// <param name="clone">Cloned graph.</param>
        public static void Clone<TVertex, TEdge>(
            [NotNull] this IVertexAndEdgeListGraph<TVertex, TEdge> graph,
            [NotNull, InstantHandle] Func<TVertex, TVertex> vertexCloner,
            [NotNull, InstantHandle] Func<TEdge, TVertex, TVertex, TEdge> edgeCloner,
            IMutableVertexAndEdgeSet<TVertex, TEdge> clone)
            where TEdge : IEdge<TVertex>
        {
#if SUPPORTS_CONTRACTS
            Contract.Requires(graph != null);
            Contract.Requires(vertexCloner != null);
            Contract.Requires(edgeCloner != null);
            Contract.Requires(clone != null);
#endif

            var vertexClones = new Dictionary<TVertex, TVertex>(graph.VertexCount);
            foreach (TVertex vertex in graph.Vertices)
            {
                TVertex clonedVertex = vertexCloner(vertex);
                clone.AddVertex(clonedVertex);
                vertexClones.Add(vertex, clonedVertex);
            }

            foreach (TEdge edge in graph.Edges)
            {
                TEdge clonedEdge = edgeCloner(
                    edge,
                    vertexClones[edge.Source],
                    vertexClones[edge.Target]);
                clone.AddEdge(clonedEdge);
            }
        }
    }
}