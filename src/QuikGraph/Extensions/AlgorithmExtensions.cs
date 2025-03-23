using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
#if SUPPORTS_TYPE_FULL_FEATURES
using System.Reflection;
#else
using QuikGraph.Utils;
#endif
#if SUPPORTS_AGGRESSIVE_INLINING
using System.Runtime.CompilerServices;
#endif
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
        /// <exception cref="T:System.ArgumentNullException"><paramref name="dictionary"/> is <see langword="null"/>.</exception>
        [Pure]
        [NotNull]
        public static Func<TKey, TValue> GetIndexer<TKey, TValue>([NotNull] IDictionary<TKey, TValue> dictionary)
        {
            if (dictionary is null)
                throw new ArgumentNullException(nameof(dictionary));

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
        /// <exception cref="T:System.ArgumentNullException"><paramref name="graph"/> is <see langword="null"/>.</exception>
        [Pure]
        [NotNull]
        public static VertexIdentity<TVertex> GetVertexIdentity<TVertex>([NotNull] this IVertexSet<TVertex> graph)
        {
            if (graph is null)
                throw new ArgumentNullException(nameof(graph));

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
                {
                    ids[vertex] = id = ids.Count.ToString();
                }
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
        /// <exception cref="T:System.ArgumentNullException"><paramref name="graph"/> is <see langword="null"/>.</exception>
        [Pure]
        [NotNull]
        public static EdgeIdentity<TVertex, TEdge> GetEdgeIdentity<TVertex, TEdge>([NotNull] this IEdgeSet<TVertex, TEdge> graph)
            where TEdge : IEdge<TVertex>
        {
            if (graph is null)
                throw new ArgumentNullException(nameof(graph));

            // Create dictionary
            var ids = new Dictionary<TEdge, string>(graph.EdgeCount);
            return edge =>
            {
                if (!ids.TryGetValue(edge, out string id))
                {
                    ids[edge] = id = ids.Count.ToString();
                }
                return id;
            };
        }

        [Pure]
        [NotNull]
        private static Func<TVertex, List<TEdge>> RunDirectedRootedAlgorithm<TVertex, TEdge, TAlgorithm>(
            [NotNull] this TAlgorithm algorithm, [NotNull] TVertex source)
            where TEdge : IEdge<TVertex>
            where TAlgorithm : RootedAlgorithmBase<TVertex, IVertexListGraph<TVertex, TEdge>>, ITreeBuilderAlgorithm<TVertex, TEdge>
        {
            Debug.Assert(algorithm != null);

            var predecessorRecorder = new VertexPredecessorRecorderObserver<TVertex, TEdge>(algorithm.VisitedGraph.AreVerticesEqual);
            using (predecessorRecorder.Attach(algorithm))
            {
                algorithm.Compute(source);
            }

            IDictionary<TVertex, TEdge> predecessors = predecessorRecorder.VerticesPredecessors;
            return vertex => predecessors.GetPath(vertex, algorithm.VisitedGraph.AreVerticesEqual);
        }

        /// <summary>
        /// Computes a breadth first tree and gets a function that allow to get edges
        /// connected to a vertex in a directed graph.
        /// </summary>
        /// <remarks>Uses <see cref="BreadthFirstSearchAlgorithm{TVertex,TEdge}"/> algorithm.</remarks>
        /// <typeparam name="TVertex">Vertex type.</typeparam>
        /// <typeparam name="TEdge">Edge type.</typeparam>
        /// <param name="graph">The graph to visit.</param>
        /// <param name="root">Starting vertex.</param>
        /// <returns>A function that allow to get edges connected to a given vertex.</returns>
        /// <exception cref="T:System.ArgumentNullException"><paramref name="graph"/> is <see langword="null"/>.</exception>
        /// <exception cref="T:System.ArgumentNullException"><paramref name="root"/> is <see langword="null"/>.</exception>
        /// <exception cref="T:System.ArgumentException"><paramref name="root"/> is not part of <paramref name="graph"/>.</exception>
        [Pure]
        [NotNull]
        public static Func<TVertex, List<TEdge>> TreeBreadthFirstSearch<TVertex, TEdge>(
            [NotNull] this IVertexListGraph<TVertex, TEdge> graph,
            [NotNull] TVertex root)
            where TEdge : IEdge<TVertex>
        {
            var algorithm = new BreadthFirstSearchAlgorithm<TVertex, TEdge>(graph);
            return algorithm.RunDirectedRootedAlgorithm<TVertex, TEdge, BreadthFirstSearchAlgorithm<TVertex, TEdge>>(root);
        }

        /// <summary>
        /// Computes a depth first tree and gets a function that allow to get edges
        /// connected to a vertex in a directed graph.
        /// </summary>
        /// <remarks>Uses <see cref="DepthFirstSearchAlgorithm{TVertex,TEdge}"/> algorithm.</remarks>
        /// <typeparam name="TVertex">Vertex type.</typeparam>
        /// <typeparam name="TEdge">Edge type.</typeparam>
        /// <param name="graph">The graph to visit.</param>
        /// <param name="root">Starting vertex.</param>
        /// <returns>A function that allow to get edges connected to a given vertex.</returns>
        /// <exception cref="T:System.ArgumentNullException"><paramref name="graph"/> is <see langword="null"/>.</exception>
        /// <exception cref="T:System.ArgumentNullException"><paramref name="root"/> is <see langword="null"/>.</exception>
        /// <exception cref="T:System.ArgumentException"><paramref name="root"/> is not part of <paramref name="graph"/>.</exception>
        [Pure]
        [NotNull]
        public static Func<TVertex, List<TEdge>> TreeDepthFirstSearch<TVertex, TEdge>(
            [NotNull] this IVertexListGraph<TVertex, TEdge> graph,
            [NotNull] TVertex root)
            where TEdge : IEdge<TVertex>
        {
            DepthFirstSearchAlgorithm<TVertex, TEdge> algorithm = graph.CreateDepthFirstSearchAlgorithm();
            return RunDirectedRootedAlgorithm<TVertex, TEdge, DepthFirstSearchAlgorithm<TVertex, TEdge>>(algorithm, root);
        }

        /// <summary>
        /// Computes a cycle popping tree and gets a function that allow to get edges
        /// connected to a vertex in a directed graph.
        /// </summary>
        /// <remarks>Uses <see cref="CyclePoppingRandomTreeAlgorithm{TVertex,TEdge}"/> algorithm and
        /// <see cref="NormalizedMarkovEdgeChain{TVertex,TEdge}"/>.</remarks>
        /// <typeparam name="TVertex">Vertex type.</typeparam>
        /// <typeparam name="TEdge">Edge type.</typeparam>
        /// <param name="graph">The graph to visit.</param>
        /// <param name="root">Starting vertex.</param>
        /// <returns>A function that allow to get edges connected to a given vertex.</returns>
        /// <exception cref="T:System.ArgumentNullException"><paramref name="graph"/> is <see langword="null"/>.</exception>
        /// <exception cref="T:System.ArgumentNullException"><paramref name="root"/> is <see langword="null"/>.</exception>
        /// <exception cref="T:System.ArgumentException"><paramref name="root"/> is not part of <paramref name="graph"/>.</exception>
        [Pure]
        [NotNull]
        public static Func<TVertex, List<TEdge>> TreeCyclePoppingRandom<TVertex, TEdge>(
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
        /// <remarks>Uses <see cref="CyclePoppingRandomTreeAlgorithm{TVertex,TEdge}"/> algorithm.</remarks>
        /// <typeparam name="TVertex">Vertex type.</typeparam>
        /// <typeparam name="TEdge">Edge type.</typeparam>
        /// <param name="graph">The graph to visit.</param>
        /// <param name="root">Starting vertex.</param>
        /// <param name="edgeChain">Markov edge chain.</param>
        /// <returns>A function that allow to get edges connected to a given vertex.</returns>
        /// <exception cref="T:System.ArgumentNullException"><paramref name="graph"/> is <see langword="null"/>.</exception>
        /// <exception cref="T:System.ArgumentNullException"><paramref name="root"/> is <see langword="null"/>.</exception>
        /// <exception cref="T:System.ArgumentNullException"><paramref name="edgeChain"/> is <see langword="null"/>.</exception>
        /// <exception cref="T:System.ArgumentException"><paramref name="root"/> is not part of <paramref name="graph"/>.</exception>
        /// <exception cref="T:System.InvalidOperationException">Something went wrong when running the algorithm.</exception>
        [Pure]
        [NotNull]
        public static Func<TVertex, List<TEdge>> TreeCyclePoppingRandom<TVertex, TEdge>(
            [NotNull] this IVertexListGraph<TVertex, TEdge> graph,
            [NotNull] TVertex root,
            [NotNull] IMarkovEdgeChain<TVertex, TEdge> edgeChain)
            where TEdge : IEdge<TVertex>
        {
            var algorithm = graph.CreateCyclePoppingRandomTreeAlgorithm(edgeChain);
            return RunDirectedRootedAlgorithm<TVertex, TEdge, CyclePoppingRandomTreeAlgorithm<TVertex, TEdge>>(algorithm, root);
        }

        #region Shortest paths

        /// <summary>
        /// Computes shortest path with the Dijkstra algorithm and gets a function that allows
        /// to get paths in a directed graph.
        /// </summary>
        /// <remarks>Uses <see cref="DijkstraShortestPathAlgorithm{TVertex,TEdge}"/> algorithm.</remarks>
        /// <typeparam name="TVertex">Vertex type.</typeparam>
        /// <typeparam name="TEdge">Edge type.</typeparam>
        /// <param name="graph">The graph to visit.</param>
        /// <param name="edgeWeights">Function that computes the weight for a given edge.</param>
        /// <param name="root">Starting vertex.</param>
        /// <returns>A function that allow to get paths starting from <paramref name="root"/> vertex.</returns>
        /// <exception cref="T:System.ArgumentNullException"><paramref name="graph"/> is <see langword="null"/>.</exception>
        /// <exception cref="T:System.ArgumentNullException"><paramref name="edgeWeights"/> is <see langword="null"/>.</exception>
        /// <exception cref="T:System.ArgumentNullException"><paramref name="root"/> is <see langword="null"/>.</exception>
        /// <exception cref="T:System.ArgumentException"><paramref name="root"/> is not part of <paramref name="graph"/>.</exception>
        [Pure]
        [NotNull]
        public static Func<TVertex, List<TEdge>> ShortestPathsDijkstra<TVertex, TEdge>(
            [NotNull] this IVertexAndEdgeListGraph<TVertex, TEdge> graph,
            [NotNull, InstantHandle] Func<TEdge, double> edgeWeights,
            [NotNull] TVertex root)
            where TEdge : IEdge<TVertex>
        {
            var algorithm = graph.CreateDijkstraShortestPathAlgorithm(edgeWeights);
            return RunDirectedRootedAlgorithm<TVertex, TEdge, DijkstraShortestPathAlgorithm<TVertex, TEdge>>(algorithm, root);
        }

        /// <summary>
        /// Computes shortest path with the Dijkstra algorithm and gets a function that allows
        /// to get paths in an undirected graph.
        /// </summary>
        /// <remarks>Uses <see cref="UndirectedDijkstraShortestPathAlgorithm{TVertex,TEdge}"/> algorithm.</remarks>
        /// <typeparam name="TVertex">Vertex type.</typeparam>
        /// <typeparam name="TEdge">Edge type.</typeparam>
        /// <param name="graph">The graph to visit.</param>
        /// <param name="edgeWeights">Function that computes the weight for a given edge.</param>
        /// <param name="root">Starting vertex.</param>
        /// <returns>A function that allow to get paths starting from <paramref name="root"/> vertex.</returns>
        /// <exception cref="T:System.ArgumentNullException"><paramref name="graph"/> is <see langword="null"/>.</exception>
        /// <exception cref="T:System.ArgumentNullException"><paramref name="edgeWeights"/> is <see langword="null"/>.</exception>
        /// <exception cref="T:System.ArgumentNullException"><paramref name="root"/> is <see langword="null"/>.</exception>
        /// <exception cref="T:System.ArgumentException"><paramref name="root"/> is not part of <paramref name="graph"/>.</exception>
        [Pure]
        [NotNull]
        public static Func<TVertex, List<TEdge>> ShortestPathsDijkstra<TVertex, TEdge>(
            [NotNull] this IUndirectedGraph<TVertex, TEdge> graph,
            [NotNull, InstantHandle] Func<TEdge, double> edgeWeights,
            [NotNull] TVertex root)
            where TEdge : IEdge<TVertex>
        {
            var algorithm = graph.CreateUndirectedDijkstraShortestPathAlgorithm(edgeWeights);
            var predecessorRecorder = new UndirectedVertexPredecessorRecorderObserver<TVertex, TEdge>();
            using (predecessorRecorder.Attach(algorithm))
            {
                algorithm.Compute(root);
            }

            IDictionary<TVertex, TEdge> predecessors = predecessorRecorder.VerticesPredecessors;
            return vertex => predecessors.GetPath(vertex, graph.AreVerticesEqual);
        }

        /// <summary>
        /// Computes shortest path with the A* algorithm and gets a function that allows
        /// to get paths in a directed graph.
        /// </summary>
        /// <remarks>Uses <see cref="AStarShortestPathAlgorithm{TVertex,TEdge}"/> algorithm.</remarks>
        /// <typeparam name="TVertex">Vertex type.</typeparam>
        /// <typeparam name="TEdge">Edge type.</typeparam>
        /// <param name="graph">The graph to visit.</param>
        /// <param name="edgeWeights">Function that computes the weight for a given edge.</param>
        /// <param name="costHeuristic">Function that computes a cost for a given vertex.</param>
        /// <param name="root">Starting vertex.</param>
        /// <returns>A function that allow to get paths starting from <paramref name="root"/> vertex.</returns>
        /// <exception cref="T:System.ArgumentNullException"><paramref name="graph"/> is <see langword="null"/>.</exception>
        /// <exception cref="T:System.ArgumentNullException"><paramref name="edgeWeights"/> is <see langword="null"/>.</exception>
        /// <exception cref="T:System.ArgumentNullException"><paramref name="costHeuristic"/> is <see langword="null"/>.</exception>
        /// <exception cref="T:System.ArgumentNullException"><paramref name="root"/> is <see langword="null"/>.</exception>
        /// <exception cref="T:System.ArgumentException"><paramref name="root"/> is not part of <paramref name="graph"/>.</exception>
        [Pure]
        [NotNull]
        public static Func<TVertex, List<TEdge>> ShortestPathsAStar<TVertex, TEdge>(
            [NotNull] this IVertexAndEdgeListGraph<TVertex, TEdge> graph,
            [NotNull, InstantHandle] Func<TEdge, double> edgeWeights,
            [NotNull, InstantHandle] Func<TVertex, double> costHeuristic,
            [NotNull] TVertex root)
            where TEdge : IEdge<TVertex>
        {
            var algorithm = graph.CreateAStarShortestPathAlgorithm(edgeWeights, costHeuristic);
            return RunDirectedRootedAlgorithm<TVertex, TEdge, AStarShortestPathAlgorithm<TVertex, TEdge>>(algorithm, root);
        }

        /// <summary>
        /// Computes shortest path with the Bellman Ford algorithm and gets a function that allows
        /// to get paths in a directed graph.
        /// </summary>
        /// <remarks>Uses <see cref="BellmanFordShortestPathAlgorithm{TVertex,TEdge}"/> algorithm.</remarks>
        /// <typeparam name="TVertex">Vertex type.</typeparam>
        /// <typeparam name="TEdge">Edge type.</typeparam>
        /// <param name="graph">The graph to visit.</param>
        /// <param name="edgeWeights">Function that computes the weight for a given edge.</param>
        /// <param name="root">Starting vertex.</param>
        /// <param name="hasNegativeCycle">Indicates if a negative cycle has been found or not.</param>
        /// <returns>A function that allow to get paths starting from <paramref name="root"/> vertex.</returns>
        /// <exception cref="T:System.ArgumentNullException"><paramref name="graph"/> is <see langword="null"/>.</exception>
        /// <exception cref="T:System.ArgumentNullException"><paramref name="edgeWeights"/> is <see langword="null"/>.</exception>
        /// <exception cref="T:System.ArgumentNullException"><paramref name="root"/> is <see langword="null"/>.</exception>
        /// <exception cref="T:System.ArgumentException"><paramref name="root"/> is not part of <paramref name="graph"/>.</exception>
        [Pure]
        [NotNull]
        public static Func<TVertex, List<TEdge>> ShortestPathsBellmanFord<TVertex, TEdge>(
            [NotNull] this IVertexAndEdgeListGraph<TVertex, TEdge> graph,
            [NotNull, InstantHandle] Func<TEdge, double> edgeWeights,
            [NotNull] TVertex root,
            out bool hasNegativeCycle)
            where TEdge : IEdge<TVertex>
        {
            if (graph is null)
                throw new ArgumentNullException(nameof(graph));
            if (edgeWeights is null)
                throw new ArgumentNullException(nameof(edgeWeights));
            if (root == null)
                throw new ArgumentNullException(nameof(root));

            var algorithm = graph.CreateBellmanFordShortestPathAlgorithm(edgeWeights);
            var predecessorRecorder = new VertexPredecessorRecorderObserver<TVertex, TEdge>(graph.AreVerticesEqual);
            using (predecessorRecorder.Attach(algorithm))
            {
                algorithm.Compute(root);
            }

            hasNegativeCycle = algorithm.FoundNegativeCycle;

            IDictionary<TVertex, TEdge> predecessors = predecessorRecorder.VerticesPredecessors;
            return vertex => predecessors.GetPath(vertex, graph.AreVerticesEqual);
        }

        /// <summary>
        /// Computes shortest path with an algorithm made for DAG (Directed ACyclic graph) and gets a function
        /// that allows to get paths.
        /// </summary>
        /// <remarks>Uses <see cref="DagShortestPathAlgorithm{TVertex,TEdge}"/> algorithm.</remarks>
        /// <typeparam name="TVertex">Vertex type.</typeparam>
        /// <typeparam name="TEdge">Edge type.</typeparam>
        /// <param name="graph">The graph to visit.</param>
        /// <param name="edgeWeights">Function that computes the weight for a given edge.</param>
        /// <param name="root">Starting vertex.</param>
        /// <returns>A function that allow to get paths starting from <paramref name="root"/> vertex.</returns>
        /// <exception cref="T:System.ArgumentNullException"><paramref name="graph"/> is <see langword="null"/>.</exception>
        /// <exception cref="T:System.ArgumentNullException"><paramref name="edgeWeights"/> is <see langword="null"/>.</exception>
        /// <exception cref="T:System.ArgumentNullException"><paramref name="root"/> is <see langword="null"/>.</exception>
        /// <exception cref="T:System.ArgumentException"><paramref name="root"/> is not part of <paramref name="graph"/>.</exception>
        [Pure]
        [NotNull]
        public static Func<TVertex, List<TEdge>> ShortestPathsDag<TVertex, TEdge>(
            [NotNull] this IVertexAndEdgeListGraph<TVertex, TEdge> graph,
            [NotNull, InstantHandle] Func<TEdge, double> edgeWeights,
            [NotNull] TVertex root)
            where TEdge : IEdge<TVertex>
        {
            if (graph is null)
                throw new ArgumentNullException(nameof(graph));
            if (edgeWeights is null)
                throw new ArgumentNullException(nameof(edgeWeights));
            if (root == null)
                throw new ArgumentNullException(nameof(root));

            var algorithm = graph.CreateDagShortestPathAlgorithm(edgeWeights);
            return RunDirectedRootedAlgorithm<TVertex, TEdge, DagShortestPathAlgorithm<TVertex, TEdge>>(algorithm, root);
        }

        #endregion

        #region K-Shortest path

        /// <summary>
        /// Computes k-shortest path with the Hoffman Pavley algorithm and gets those paths.
        /// </summary>
        /// <remarks>Uses <see cref="HoffmanPavleyRankedShortestPathAlgorithm{TVertex,TEdge}"/> algorithm.</remarks>
        /// <typeparam name="TVertex">Vertex type.</typeparam>
        /// <typeparam name="TEdge">Edge type.</typeparam>
        /// <param name="graph">The graph to visit.</param>
        /// <param name="edgeWeights">Function that computes the weight for a given edge.</param>
        /// <param name="root">Starting vertex.</param>
        /// <param name="target">Target vertex.</param>
        /// <param name="maxCount">Maximal number of path to search.</param>
        /// <returns>Enumeration of paths to go from <paramref name="root"/> vertex to <paramref name="target"/>.</returns>
        /// <exception cref="T:System.ArgumentNullException"><paramref name="graph"/> is <see langword="null"/>.</exception>
        /// <exception cref="T:System.ArgumentNullException"><paramref name="edgeWeights"/> is <see langword="null"/>.</exception>
        /// <exception cref="T:System.ArgumentNullException"><paramref name="root"/> is <see langword="null"/>.</exception>
        /// <exception cref="T:System.ArgumentNullException"><paramref name="target"/> is <see langword="null"/>.</exception>
        /// <exception cref="T:System.ArgumentException"><paramref name="root"/> or <paramref name="target"/> are not part of <paramref name="graph"/>.</exception>
        /// <exception cref="T:System.ArgumentOutOfRangeException"><paramref name="maxCount"/> is lower or equal to 1.</exception>
        [Pure]
        [NotNull, ItemNotNull]
        public static IEnumerable<IEnumerable<TEdge>> RankedShortestPathHoffmanPavley<TVertex, TEdge>(
            [NotNull] this IBidirectionalGraph<TVertex, TEdge> graph,
            [NotNull, InstantHandle] Func<TEdge, double> edgeWeights,
            [NotNull] TVertex root,
            [NotNull] TVertex target,
            int maxCount = 3)
            where TEdge : IEdge<TVertex>
        {
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
        /// <exception cref="T:System.ArgumentNullException"><paramref name="graph"/> is <see langword="null"/>.</exception>
        [Pure]
        [NotNull, ItemNotNull]
        public static IEnumerable<TVertex> Sinks<TVertex, TEdge>(
            [NotNull] this IVertexListGraph<TVertex, TEdge> graph)
            where TEdge : IEdge<TVertex>
            => graph.Vertices.Where(FallBackTo<TVertex, bool>(graph.IsOutEdgesEmpty, true));

        /// <summary> Fixes <see langword="null"/> to <paramref name="fallBack"/> </summary>
        public static Func<TK, TV> FallBackTo<TK, TV>(this Func<TK, TV?> isOutEdgesEmpty, TV fallBack) where TV : struct
            => v => isOutEdgesEmpty(v) ?? fallBack;

        /// <summary>
        /// Gets set of root vertices.
        /// </summary>
        /// <typeparam name="TVertex">Vertex type.</typeparam>
        /// <typeparam name="TEdge">Edge type.</typeparam>
        /// <param name="graph">Graph to visit.</param>
        /// <returns>Root vertices.</returns>
        /// <exception cref="T:System.ArgumentNullException"><paramref name="graph"/> is <see langword="null"/>.</exception>
        [Pure]
        [NotNull, ItemNotNull]
        public static IEnumerable<TVertex> Roots<TVertex, TEdge>(
            [NotNull] this IVertexListGraph<TVertex, TEdge> graph)
            where TEdge : IEdge<TVertex>
        {
            var dfs = graph.CreateDepthFirstSearchAlgorithm();
            var notRoots = new Dictionary<TVertex, bool>(graph.VertexCount);
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
        /// <exception cref="T:System.ArgumentNullException"><paramref name="graph"/> is <see langword="null"/>.</exception>
        [Pure]
        [NotNull, ItemNotNull]
        public static IEnumerable<TVertex> Roots<TVertex, TEdge>(
            [NotNull] this IBidirectionalGraph<TVertex, TEdge> graph)
            where TEdge : IEdge<TVertex>
        {
            if (graph is null)
                throw new ArgumentNullException(nameof(graph));
            return graph.Vertices.Where(FallBackTo<TVertex, bool>(graph.IsInEdgesEmpty, true));
        }

        /// <summary>
        /// Gets set of isolated vertices (no incoming nor outcoming vertices).
        /// </summary>
        /// <typeparam name="TVertex">Vertex type.</typeparam>
        /// <typeparam name="TEdge">Edge type.</typeparam>
        /// <param name="graph">Graph to visit.</param>
        /// <returns>Root vertices.</returns>
        /// <exception cref="T:System.ArgumentNullException"><paramref name="graph"/> is <see langword="null"/>.</exception>
        [Pure]
        [NotNull, ItemNotNull]
        public static IEnumerable<TVertex> IsolatedVertices<TVertex, TEdge>(
            [NotNull] this IBidirectionalGraph<TVertex, TEdge> graph)
            where TEdge : IEdge<TVertex>
        {
            if (graph is null)
                throw new ArgumentNullException(nameof(graph));
            return graph.Vertices.Where(vertex => graph.Degree(vertex) == 0);
        }

        #region Topological sorts

        /// <summary>
        /// Creates a topological sort of a directed acyclic graph.
        /// </summary>
        /// <typeparam name="TVertex">Vertex type.</typeparam>
        /// <typeparam name="TEdge">Edge type.</typeparam>
        /// <param name="graph">Graph to visit.</param>
        /// <returns>Sorted vertices (topological sort).</returns>
        /// <exception cref="T:System.ArgumentNullException"><paramref name="graph"/> is <see langword="null"/>.</exception>
        /// <exception cref="CyclicGraphException">If the input graph has a cycle.</exception>
        [Pure]
        [NotNull, ItemNotNull]
        public static IEnumerable<TVertex> TopologicalSort<TVertex, TEdge>(
            [NotNull] this IVertexListGraph<TVertex, TEdge> graph) where TEdge : IEdge<TVertex>
            => graph.ComputeTopologicalSortAlgorithm().SortedVertices.AsEnumerable();

        /// <summary>
        /// Creates a topological sort of an undirected acyclic graph.
        /// </summary>
        /// <typeparam name="TVertex">Vertex type.</typeparam>
        /// <typeparam name="TEdge">Edge type.</typeparam>
        /// <param name="graph">Graph to visit.</param>
        /// <returns>Sorted vertices (topological sort).</returns>
        /// <exception cref="T:System.ArgumentNullException"><paramref name="graph"/> is <see langword="null"/>.</exception>
        /// <exception cref="CyclicGraphException">If the input graph has a cycle.</exception>
        [Pure]
        [NotNull, ItemNotNull]
        public static IEnumerable<TVertex> TopologicalSort<TVertex, TEdge>(
            [NotNull] this IUndirectedGraph<TVertex, TEdge> graph)
            where TEdge : IEdge<TVertex>
        {
            if (graph is null)
                throw new ArgumentNullException(nameof(graph));

            var algorithm = graph.CreateUndirectedTopologicalSortAlgorithm(graph.VertexCount);
            algorithm.Compute();
            return algorithm.SortedVertices.AsEnumerable();
        }

        /// <summary>
        /// Creates a topological sort (source first) of a directed acyclic graph.
        /// </summary>
        /// <typeparam name="TVertex">Vertex type.</typeparam>
        /// <typeparam name="TEdge">Edge type.</typeparam>
        /// <param name="graph">Graph to visit.</param>
        /// <returns>Sorted vertices (topological sort).</returns>
        /// <exception cref="T:System.ArgumentNullException"><paramref name="graph"/> is <see langword="null"/>.</exception>
        /// <exception cref="CyclicGraphException">If the input graph has a cycle.</exception>
        [Pure]
        [NotNull, ItemNotNull]
        public static IEnumerable<TVertex> SourceFirstTopologicalSort<TVertex, TEdge>(
            [NotNull] this IVertexAndEdgeListGraph<TVertex, TEdge> graph)
            where TEdge : IEdge<TVertex>
        {
            if (graph is null)
                throw new ArgumentNullException(nameof(graph));

            var algorithm = new SourceFirstTopologicalSortAlgorithm<TVertex, TEdge>(graph, graph.VertexCount);
            algorithm.Compute();
            return algorithm.SortedVertices.AsEnumerable();
        }

        /// <summary>
        /// Creates a topological sort (source first) of an undirected acyclic graph.
        /// </summary>
        /// <typeparam name="TVertex">Vertex type.</typeparam>
        /// <typeparam name="TEdge">Edge type.</typeparam>
        /// <param name="graph">Graph to visit.</param>
        /// <returns>Sorted vertices (topological sort).</returns>
        /// <exception cref="T:System.ArgumentNullException"><paramref name="graph"/> is <see langword="null"/>.</exception>
        /// <exception cref="CyclicGraphException">If the input graph has a cycle.</exception>
        [Pure]
        [NotNull, ItemNotNull]
        public static IEnumerable<TVertex> SourceFirstTopologicalSort<TVertex, TEdge>(
            [NotNull] this IUndirectedGraph<TVertex, TEdge> graph)
            where TEdge : IEdge<TVertex>
        {
            if (graph is null)
                throw new ArgumentNullException(nameof(graph));

            var algorithm = graph.CreateUndirectedFirstTopologicalSortAlgorithm(false, graph.VertexCount);
            algorithm.Compute();
            return algorithm.SortedVertices.AsEnumerable();
        }

        /// <summary>
        /// Creates a topological sort (source first) of a bidirectional directed acyclic graph.
        /// Uses the <see cref="TopologicalSortDirection.Forward"/> direction.
        /// </summary>
        /// <typeparam name="TVertex">Vertex type.</typeparam>
        /// <typeparam name="TEdge">Edge type.</typeparam>
        /// <param name="graph">Graph to visit.</param>
        /// <returns>Sorted vertices (topological sort).</returns>
        /// <exception cref="T:System.ArgumentNullException"><paramref name="graph"/> is <see langword="null"/>.</exception>
        /// <exception cref="CyclicGraphException">If the input graph has a cycle.</exception>
        [Pure]
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
        /// <param name="direction">Topological sort direction.</param>
        /// <returns>Sorted vertices (topological sort).</returns>
        /// <exception cref="T:System.ArgumentNullException"><paramref name="graph"/> is <see langword="null"/>.</exception>
        /// <exception cref="CyclicGraphException">If the input graph has a cycle.</exception>
        [Pure]
        [NotNull, ItemNotNull]
        public static IEnumerable<TVertex> SourceFirstBidirectionalTopologicalSort<TVertex, TEdge>(
            [NotNull] this IBidirectionalGraph<TVertex, TEdge> graph,
            TopologicalSortDirection direction)
            where TEdge : IEdge<TVertex>
        {
            if (graph is null)
                throw new ArgumentNullException(nameof(graph));

            var algorithm = new SourceFirstBidirectionalTopologicalSortAlgorithm<TVertex, TEdge>(graph, direction, graph.VertexCount);
            algorithm.Compute();
            return algorithm.SortedVertices.AsEnumerable();
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
        /// <exception cref="T:System.ArgumentNullException"><paramref name="graph"/> is <see langword="null"/>.</exception>
        /// <exception cref="T:System.ArgumentNullException"><paramref name="components"/> is <see langword="null"/>.</exception>
        public static int ConnectedComponents<TVertex, TEdge>(
            [NotNull] this IUndirectedGraph<TVertex, TEdge> graph,
            [NotNull] IDictionary<TVertex, int> components)
            where TEdge : IEdge<TVertex>
        {
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
        /// <param name="getComponents">A function retrieve components of the <paramref name="graph"/>.</param>
        /// <returns>A <see cref="T:System.IDisposable"/> of the used algorithm.</returns>
        /// <exception cref="T:System.ArgumentNullException"><paramref name="graph"/> is <see langword="null"/>.</exception>
        [Pure]
        [NotNull]
        public static IDisposable IncrementalConnectedComponents<TVertex, TEdge>(
            [NotNull] this IMutableVertexAndEdgeSet<TVertex, TEdge> graph,
            [NotNull] out Func<KeyValuePair<int, IDictionary<TVertex, int>>> getComponents)
            where TEdge : IEdge<TVertex>
        {
            var incrementalComponents = graph.CreateIncrementalConnectedComponentsAlgorithm();
            incrementalComponents.Compute();
            getComponents = () => incrementalComponents.GetComponents();
            return incrementalComponents;
        }

        /// <summary> Computes the Number of strongly connected components of a directed graph. </summary>
        /// <typeparam name="TVertex">Vertex type.</typeparam>
        /// <typeparam name="TEdge">Edge type.</typeparam>
        /// <param name="graph">Graph to visit.</param>
        /// <param name="components">Found components.</param>
        /// <returns> component found.</returns>
        /// <exception cref="T:System.ArgumentNullException"><paramref name="graph"/> is <see langword="null"/>.</exception>
        /// <exception cref="T:System.ArgumentNullException"><paramref name="components"/> is <see langword="null"/>.</exception>
        public static int StronglyConnectedComponentsCount<TVertex, TEdge>(
            [NotNull] this IVertexListGraph<TVertex, TEdge> graph,
            [NotNull] IDictionary<TVertex, int> components) where TEdge : IEdge<TVertex>
            => graph.ComputeStronglyConnectedComponents(components).ComponentCount;

        /// <summary> Computes the weakly connected components of a directed graph. </summary>
        /// <typeparam name="TVertex">Vertex type.</typeparam>
        /// <typeparam name="TEdge">Edge type.</typeparam>
        /// <param name="graph">Graph to visit.</param>
        /// <param name="components">Found components.</param>
        /// <returns>Number of component found.</returns>
        /// <exception cref="T:System.ArgumentNullException"><paramref name="graph"/> is <see langword="null"/>.</exception>
        /// <exception cref="T:System.ArgumentNullException"><paramref name="components"/> is <see langword="null"/>.</exception>
        public static int WeaklyConnectedComponentsCount<TVertex, TEdge>(
            [NotNull] this IVertexListGraph<TVertex, TEdge> graph,
            [NotNull] IDictionary<TVertex, int> components)
            where TEdge : IEdge<TVertex> => graph.ComputeWeaklyConnectedComponents(components).ComponentCount;

        /// <summary>
        /// Condensates the strongly connected components of a directed graph.
        /// </summary>
        /// <typeparam name="TVertex">Vertex type.</typeparam>
        /// <typeparam name="TEdge">Edge type.</typeparam>
        /// <typeparam name="TGraph">Graph type.</typeparam>
        /// <param name="graph">Graph to visit.</param>
        /// <returns>The condensed graph.</returns>
        /// <exception cref="T:System.ArgumentNullException"><paramref name="graph"/> is <see langword="null"/>.</exception>
        [Pure]
        [NotNull]
        public static IMutableBidirectionalGraph<TGraph, CondensedEdge<TVertex, TEdge, TGraph>> CondensateStronglyConnected<TVertex, TEdge, TGraph>(
            [NotNull] this IVertexAndEdgeListGraph<TVertex, TEdge> graph)
            where TEdge : IEdge<TVertex>
            where TGraph : IMutableVertexAndEdgeSet<TVertex, TEdge>, new()
        {
            var algorithm = new CondensationGraphAlgorithm<TVertex, TEdge, TGraph>(graph, true);
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
        /// <exception cref="T:System.ArgumentNullException"><paramref name="graph"/> is <see langword="null"/>.</exception>
        [Pure]
        [NotNull]
        public static IMutableBidirectionalGraph<TGraph, CondensedEdge<TVertex, TEdge, TGraph>> CondensateWeaklyConnected<TVertex, TEdge, TGraph>(
            [NotNull] this IVertexAndEdgeListGraph<TVertex, TEdge> graph)
            where TEdge : IEdge<TVertex>
            where TGraph : IMutableVertexAndEdgeSet<TVertex, TEdge>, new()
        {
            var algorithm = new CondensationGraphAlgorithm<TVertex, TEdge, TGraph>(graph, false);
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
        /// <exception cref="T:System.ArgumentNullException"><paramref name="graph"/> is <see langword="null"/>.</exception>
        /// <exception cref="T:System.ArgumentNullException"><paramref name="vertexPredicate"/> is <see langword="null"/>.</exception>
        [Pure]
        [NotNull]
        public static IMutableBidirectionalGraph<TVertex, MergedEdge<TVertex, TEdge>> CondensateEdges<TVertex, TEdge>(
            [NotNull] this IBidirectionalGraph<TVertex, TEdge> graph,
            [NotNull] Func<TVertex, bool> vertexPredicate)
            where TEdge : IEdge<TVertex>
        {
            var condensedGraph = new BidirectionalGraph<TVertex, MergedEdge<TVertex, TEdge>>();
            var algorithm = new EdgeMergeCondensationGraphAlgorithm<TVertex, TEdge>(
                graph,
                condensedGraph,
                vertexPredicate);
            algorithm.Compute();
            return condensedGraph;
        }

        #endregion

        private sealed class DirectedCycleTester<TVertex, TEdge>
            where TEdge : IEdge<TVertex>
        {
            private bool _isDag = true;

            [Pure]
            public bool IsDag([NotNull] IVertexListGraph<TVertex, TEdge> graph)
            {
                Debug.Assert(graph != null);

                var dfs = graph.CreateDepthFirstSearchAlgorithm();
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

        [Pure]
#if SUPPORTS_AGGRESSIVE_INLINING
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        private static bool IsDirectedAcyclicGraphInternal<TVertex, TEdge>(
            [NotNull] this IVertexListGraph<TVertex, TEdge> graph)
            where TEdge : IEdge<TVertex>
        {
            return new DirectedCycleTester<TVertex, TEdge>().IsDag(graph);
        }

        /// <summary>
        /// Checks whether the graph is acyclic or not.
        /// </summary>
        /// <remarks>
        /// Builds an <see cref="AdjacencyGraph{TVertex,TEdge}"/> from <paramref name="edges"/>
        /// and performs a depth first search to look for cycles.
        /// </remarks>
        /// <typeparam name="TVertex">Vertex type.</typeparam>
        /// <typeparam name="TEdge">Edge type.</typeparam>
        /// <param name="edges">Edges of forming the graph to visit.</param>
        /// <returns>True if the graph contains a cycle, false otherwise.</returns>
        /// <exception cref="T:System.ArgumentNullException">
        /// <paramref name="edges"/> is <see langword="null"/> or at least one of them is <see langword="null"/>.
        /// </exception>
        [Pure]
        public static bool IsDirectedAcyclicGraph<TVertex, TEdge>(
            [NotNull, ItemNotNull] this IEnumerable<TEdge> edges)
            where TEdge : IEdge<TVertex>
        {
            var graph = new AdjacencyGraph<TVertex, TEdge>();
            graph.AddVerticesAndEdgeRange(edges);
            return IsDirectedAcyclicGraphInternal(graph);
        }

        /// <summary>
        /// Checks whether the <paramref name="graph"/> is acyclic or not.
        /// </summary>
        /// <remarks>
        /// Performs a depth first search to look for cycles.
        /// </remarks>
        /// <typeparam name="TVertex">Vertex type.</typeparam>
        /// <typeparam name="TEdge">Edge type.</typeparam>
        /// <param name="graph">Graph to visit.</param>
        /// <returns>True if the graph contains a cycle, false otherwise.</returns>
        /// <exception cref="T:System.ArgumentNullException"><paramref name="graph"/> is <see langword="null"/>.</exception>
        [Pure]
        public static bool IsDirectedAcyclicGraph<TVertex, TEdge>(
            [NotNull] this IVertexListGraph<TVertex, TEdge> graph)
            where TEdge : IEdge<TVertex>
        {
            if (graph is null)
                throw new ArgumentNullException(nameof(graph));
            return IsDirectedAcyclicGraphInternal(graph);
        }

        private sealed class UndirectedCycleTester<TVertex, TEdge>
            where TEdge : IEdge<TVertex>
        {
            private bool _hasCycle;

            [Pure]
            public bool HasCycle([NotNull] IUndirectedGraph<TVertex, TEdge> graph)
            {
                Debug.Assert(graph != null);

                var dfs = new UndirectedDepthFirstSearchAlgorithm<TVertex, TEdge>(graph);
                try
                {
                    dfs.BackEdge += DfsBackEdge;
                    dfs.Compute();
                    return _hasCycle;
                }
                finally
                {
                    dfs.BackEdge -= DfsBackEdge;
                }
            }

            private void DfsBackEdge([NotNull] object sender, [NotNull] UndirectedEdgeEventArgs<TVertex, TEdge> args)
            {
                _hasCycle = true;
            }
        }

        [Pure]
#if SUPPORTS_AGGRESSIVE_INLINING
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        private static bool IsUndirectedAcyclicGraphInternal<TVertex, TEdge>(
            [NotNull] this IUndirectedGraph<TVertex, TEdge> graph)
            where TEdge : IEdge<TVertex>
        {
            return !new UndirectedCycleTester<TVertex, TEdge>().HasCycle(graph);
        }

        /// <summary>
        /// Checks whether the graph is acyclic or not.
        /// </summary>
        /// <remarks>
        /// Builds an <see cref="UndirectedGraph{TVertex,TEdge}"/> from <paramref name="edges"/>
        /// and performs a depth first search to look for cycles.
        /// </remarks>
        /// <typeparam name="TVertex">Vertex type.</typeparam>
        /// <typeparam name="TEdge">Edge type.</typeparam>
        /// <param name="edges">Edges of forming the graph to visit.</param>
        /// <returns>True if the graph contains a cycle, false otherwise.</returns>
        /// <exception cref="T:System.ArgumentNullException">
        /// <paramref name="edges"/> is <see langword="null"/> or at least one of them is <see langword="null"/>.
        /// </exception>
        [Pure]
        public static bool IsUndirectedAcyclicGraph<TVertex, TEdge>(
            [NotNull, ItemNotNull] this IEnumerable<TEdge> edges)
            where TEdge : IEdge<TVertex>
        {
            var graph = new UndirectedGraph<TVertex, TEdge>();
            graph.AddVerticesAndEdgeRange(edges);
            return IsUndirectedAcyclicGraphInternal(graph);
        }

        /// <summary>
        /// Checks whether the <paramref name="graph"/> is acyclic or not.
        /// </summary>
        /// <typeparam name="TVertex">Vertex type.</typeparam>
        /// <typeparam name="TEdge">Edge type.</typeparam>
        /// <param name="graph">Graph to visit.</param>
        /// <returns>True if the graph contains a cycle, false otherwise.</returns>
        /// <exception cref="T:System.ArgumentNullException"><paramref name="graph"/> is <see langword="null"/>.</exception>
        [Pure]
        public static bool IsUndirectedAcyclicGraph<TVertex, TEdge>(
            [NotNull] this IUndirectedGraph<TVertex, TEdge> graph)
            where TEdge : IEdge<TVertex>
        {
            if (graph is null)
                throw new ArgumentNullException(nameof(graph));
            return IsUndirectedAcyclicGraphInternal(graph);
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
        /// <exception cref="T:System.ArgumentNullException"><paramref name="predecessors"/> is <see langword="null"/>.</exception>
        /// <exception cref="T:System.ArgumentNullException"><paramref name="edgeCosts"/> is <see langword="null"/>.</exception>
        /// <exception cref="T:System.ArgumentNullException"><paramref name="target"/> is <see langword="null"/>.</exception>
        [Pure]
        public static double ComputePredecessorCost<TVertex, TEdge>(
            [NotNull] IDictionary<TVertex, TEdge> predecessors,
            [NotNull] IDictionary<TEdge, double> edgeCosts,
            [NotNull] TVertex target)
            where TEdge : IEdge<TVertex>
        {
            if (predecessors is null)
                throw new ArgumentNullException(nameof(predecessors));
            if (edgeCosts is null)
                throw new ArgumentNullException(nameof(edgeCosts));
            if (target == null)
                throw new ArgumentNullException(nameof(target));

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
        /// <exception cref="T:System.ArgumentNullException"><paramref name="graph"/> is <see langword="null"/>.</exception>
        [Pure]
        [NotNull]
        public static IDisjointSet<TVertex> ComputeDisjointSet<TVertex, TEdge>(
            [NotNull] this IUndirectedGraph<TVertex, TEdge> graph)
            where TEdge : IEdge<TVertex>
        {
            if (graph is null)
                throw new ArgumentNullException(nameof(graph));

            var sets = new ForestDisjointSet<TVertex>(graph.VertexCount);
            foreach (TVertex vertex in graph.Vertices)
            {
                sets.MakeSet(vertex);
            }

            foreach (TEdge edge in graph.Edges)
            {
                sets.Union(edge.Source, edge.Target);
            }

            return sets;
        }

        /// <summary> Computes the minimum spanning tree using Prim algorithm. </summary>
        /// <typeparam name="TVertex">Vertex type.</typeparam>
        /// <typeparam name="TEdge">Edge type.</typeparam>
        /// <param name="graph">Graph to visit.</param>
        /// <param name="edgeWeights">Function that computes the weight for a given edge.</param>
        /// <returns>Edges part of the minimum spanning tree.</returns>
        /// <remarks>
        /// Prim algorithm is simply implemented by calling <see cref="UndirectedDijkstraShortestPathAlgorithm{TVertex,TEdge}"/>
        /// with <see cref="DistanceRelaxers.Prim"/>.
        /// </remarks>
        [Pure]
        [NotNull, ItemNotNull]
        public static IEnumerable<TEdge> MinimumSpanningTreePrim<TVertex, TEdge>(
            [NotNull] this IUndirectedGraph<TVertex, TEdge> graph,
            [NotNull, InstantHandle] Func<TEdge, double> edgeWeights)
            where TEdge : IEdge<TVertex>
        {
            if (graph is null)
                throw new ArgumentNullException(nameof(graph));
            if (edgeWeights is null)
                throw new ArgumentNullException(nameof(edgeWeights));

            if (graph.VertexCount == 0)
                return Enumerable.Empty<TEdge>();

            IDistanceRelaxer distanceRelaxer = DistanceRelaxers.Prim;
            var dijkstra = graph.CreateUndirectedDijkstraShortestPathAlgorithm(edgeWeights, distanceRelaxer);
            var edgeRecorder = new UndirectedVertexPredecessorRecorderObserver<TVertex, TEdge>();
            using (edgeRecorder.Attach(dijkstra))
            {
                dijkstra.Compute();
            }

            return edgeRecorder.VerticesPredecessors.Values;
        }

        /// <summary>
        /// Computes the minimum spanning tree using Kruskal algorithm.
        /// </summary>
        /// <typeparam name="TVertex">Vertex type.</typeparam>
        /// <typeparam name="TEdge">Edge type.</typeparam>
        /// <param name="graph">Graph to visit.</param>
        /// <param name="edgeWeights">Function that computes the weight for a given edge.</param>
        /// <returns>Edges part of the minimum spanning tree.</returns>
        /// <exception cref="T:System.ArgumentNullException"><paramref name="graph"/> is <see langword="null"/>.</exception>
        /// <exception cref="T:System.ArgumentNullException"><paramref name="edgeWeights"/> is <see langword="null"/>.</exception>
        [Pure]
        [NotNull, ItemNotNull]
        public static IEnumerable<TEdge> MinimumSpanningTreeKruskal<TVertex, TEdge>(
            [NotNull] this IUndirectedGraph<TVertex, TEdge> graph,
            [NotNull, InstantHandle] Func<TEdge, double> edgeWeights)
            where TEdge : IEdge<TVertex>
        {
            if (graph is null)
                throw new ArgumentNullException(nameof(graph));
            if (edgeWeights is null)
                throw new ArgumentNullException(nameof(edgeWeights));

            if (graph.VertexCount == 0)
                return Enumerable.Empty<TEdge>();

            var kruskal = new KruskalMinimumSpanningTreeAlgorithm<TVertex, TEdge>(graph, edgeWeights);
            var edgeRecorder = new EdgeRecorderObserver<TVertex, TEdge>();
            using (edgeRecorder.Attach(kruskal))
            {
                kruskal.Compute();
            }

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
        /// <exception cref="T:System.ArgumentNullException"><paramref name="graph"/> is <see langword="null"/>.</exception>
        /// <exception cref="T:System.ArgumentNullException"><paramref name="root"/> is <see langword="null"/>.</exception>
        /// <exception cref="T:System.ArgumentNullException"><paramref name="pairs"/> is <see langword="null"/>.</exception>
        /// <exception cref="T:System.ArgumentException">At least one of <paramref name="pairs"/> vertices is not part of <paramref name="graph"/>.</exception>
        [Pure]
        [NotNull]
        public static TryFunc<SEquatableEdge<TVertex>, TVertex> OfflineLeastCommonAncestor<TVertex, TEdge>(
            [NotNull] this IVertexListGraph<TVertex, TEdge> graph,
            [NotNull] TVertex root,
            [NotNull] IEnumerable<SEquatableEdge<TVertex>> pairs)
            where TEdge : IEdge<TVertex>
        {
            if (graph is null)
                throw new ArgumentNullException(nameof(graph));
            if (pairs is null)
                throw new ArgumentNullException(nameof(pairs));
            SEquatableEdge<TVertex>[] pairsArray = pairs.ToArray();
            if (pairsArray.Any(pair => !graph.ContainsVertex(pair.Source)))
                throw new ArgumentException($"All pairs sources must be in the {nameof(graph)}.", nameof(pairs));
            if (pairsArray.Any(pair => !graph.ContainsVertex(pair.Target)))
                throw new ArgumentException($"All pairs targets must be in the {nameof(graph)}.", nameof(pairs));

            var algorithm = new TarjanOfflineLeastCommonAncestorAlgorithm<TVertex, TEdge>(graph);
            algorithm.Compute(root, pairsArray);

            IDictionary<SEquatableEdge<TVertex>, TVertex> ancestors = algorithm.Ancestors;
            return (SEquatableEdge<TVertex> pair, out TVertex vertex) => ancestors.TryGetValue(pair, out vertex);
        }

        /// <summary>
        /// Computes the maximum flow for a graph with positive capacities and flows
        /// using Edmonds-Karp algorithm.
        /// </summary>
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
        /// <exception cref="T:System.ArgumentNullException"><paramref name="graph"/> is <see langword="null"/>.</exception>
        /// <exception cref="T:System.ArgumentNullException"><paramref name="edgeCapacities"/> is <see langword="null"/>.</exception>
        /// <exception cref="T:System.ArgumentNullException"><paramref name="source"/> is <see langword="null"/>.</exception>
        /// <exception cref="T:System.ArgumentNullException"><paramref name="sink"/> is <see langword="null"/>.</exception>
        /// <exception cref="T:System.ArgumentNullException"><paramref name="edgeFactory"/> is <see langword="null"/>.</exception>
        /// <exception cref="T:System.ArgumentNullException"><paramref name="reversedEdgeAugmentorAlgorithm"/> is <see langword="null"/>.</exception>
        /// <exception cref="T:System.ArgumentException"><paramref name="source"/> and <paramref name="sink"/> are the same vertex.</exception>
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
            if (graph.AreVerticesEqual(source, sink))
                throw new ArgumentException($"{nameof(source)} and {nameof(sink)} must be different.");

            // Compute maximum flow
            var flow = graph.CreateEdmondsKarpMaximumFlowAlgorithm(edgeCapacities, edgeFactory, reversedEdgeAugmentorAlgorithm);
            flow.Compute(source, sink);
            flowPredecessors = flow.Predecessors.TryGetValue;

            return flow.MaxFlow;
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
        /// <exception cref="T:System.ArgumentNullException"><paramref name="graph"/> is <see langword="null"/>.</exception>
        /// <exception cref="T:System.ArgumentNullException"><paramref name="vertexCloner"/> is <see langword="null"/> or creates <see langword="null"/> vertex.</exception>
        /// <exception cref="T:System.ArgumentNullException"><paramref name="edgeCloner"/> is <see langword="null"/> or creates <see langword="null"/> edge.</exception>
        /// <exception cref="T:System.ArgumentNullException"><paramref name="clone"/> is <see langword="null"/>.</exception>
        public static void Clone<TVertex, TEdge>(
            [NotNull] this IVertexAndEdgeListGraph<TVertex, TEdge> graph,
            [NotNull, InstantHandle] Func<TVertex, TVertex> vertexCloner,
            [NotNull, InstantHandle] Func<TEdge, TVertex, TVertex, TEdge> edgeCloner,
            [NotNull] IMutableVertexAndEdgeSet<TVertex, TEdge> clone)
            where TEdge : IEdge<TVertex>
        {
            if (graph is null)
                throw new ArgumentNullException(nameof(graph));
            if (vertexCloner is null)
                throw new ArgumentNullException(nameof(vertexCloner));
            if (edgeCloner is null)
                throw new ArgumentNullException(nameof(edgeCloner));
            if (clone is null)
                throw new ArgumentNullException(nameof(clone));

            clone.Clear();
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