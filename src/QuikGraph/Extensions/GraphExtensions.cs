using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;

namespace QuikGraph
{
    /// <summary> Extensions for populating graph data structures. </summary>
    public static class GraphExtensions
    {
        #region Delegate graphs

        /// <summary>
        /// Creates an instance of <see cref="DelegateIncidenceGraph{TVertex,TEdge}"/> from this getter of out-edges.
        /// </summary>
        /// <typeparam name="TVertex">Vertex type.</typeparam>
        /// <typeparam name="TEdge">Edge type.</typeparam>
        /// <param name="tryGetOutEdges">Getter of out-edges.</param>
        /// <returns>A corresponding <see cref="DelegateIncidenceGraph{TVertex,TEdge}"/>.</returns>
        /// <exception cref="T:System.ArgumentNullException"><paramref name="tryGetOutEdges"/> is <see langword="null"/>.</exception>
        [Pure]
        [NotNull]
        public static DelegateIncidenceGraph<TVertex, TEdge> ToDelegateIncidenceGraph<TVertex, TEdge>(
            [NotNull] this Func<TVertex, IEnumerable<TEdge>> tryGetOutEdges)
            where TEdge : IEdge<TVertex> => new DelegateIncidenceGraph<TVertex, TEdge>(tryGetOutEdges);

        /// <summary>
        /// Wraps a dictionary into a vertex and edge list graph.
        /// </summary>
        /// <typeparam name="TVertex">Vertex type.</typeparam>
        /// <typeparam name="TEdge">Edge type.</typeparam>
        /// <typeparam name="TEdges">Type of the enumerable of out-edges.</typeparam>
        /// <param name="dictionary">Vertices and edges mapping.</param>
        /// <returns>A corresponding <see cref="DelegateVertexAndEdgeListGraph{TVertex,TEdge}"/>.</returns>
        /// <exception cref="T:System.ArgumentNullException"><paramref name="dictionary"/> is <see langword="null"/>.</exception>
        [Pure]
        [NotNull]
        public static DelegateVertexAndEdgeListGraph<TVertex, TEdge> ToDelegateVertexAndEdgeListGraph<TVertex, TEdge,
            TEdges>(
            [NotNull] this IDictionary<TVertex, TEdges> dictionary)
            where TEdge : IEdge<TVertex>
            where TEdges : IEnumerable<TEdge>
        {
            return ToDelegateVertexAndEdgeListGraph(dictionary, kv => kv.Value);
        }

        /// <summary>
        /// Wraps a dictionary into a <see cref="DelegateVertexAndEdgeListGraph{TVertex,TEdge}"/> with the given edge conversion to get edges.
        /// </summary>
        /// <typeparam name="TVertex">Vertex type.</typeparam>
        /// <typeparam name="TEdge">Edge type.</typeparam>
        /// <typeparam name="TValue">Type of the enumerable of out-edges.</typeparam>
        /// <param name="dictionary">Vertices and edges mapping.</param>
        /// <param name="keyValueToOutEdges">Converter of vertex/edge mapping to enumerable of edges.</param>
        /// <returns>A corresponding <see cref="DelegateVertexAndEdgeListGraph{TVertex,TEdge}"/>.</returns>
        /// <exception cref="T:System.ArgumentNullException"><paramref name="dictionary"/> is <see langword="null"/>.</exception>
        /// <exception cref="T:System.ArgumentNullException"><paramref name="keyValueToOutEdges"/> is <see langword="null"/>.</exception>
        [Pure]
        [NotNull]
        public static DelegateVertexAndEdgeListGraph<TVertex, TEdge> ToDelegateVertexAndEdgeListGraph<TVertex, TEdge,
            TValue>(
            [NotNull] this IDictionary<TVertex, TValue> dictionary,
#if SUPPORTS_CONVERTER
            [NotNull] Converter<KeyValuePair<TVertex, TValue>, IEnumerable<TEdge>> keyValueToOutEdges)
#else
            [NotNull] Func<KeyValuePair<TVertex,TValue>, IEnumerable<TEdge>> keyValueToOutEdges)
#endif
            where TEdge : IEdge<TVertex>
        {
            if (dictionary is null)
                throw new ArgumentNullException(nameof(dictionary));
            if (keyValueToOutEdges is null)
                throw new ArgumentNullException(nameof(keyValueToOutEdges));

            return new DelegateVertexAndEdgeListGraph<TVertex, TEdge>(
                dictionary.Keys,
                (TVertex key) =>
                {
                    if (!dictionary.TryGetValue(key, out TValue value))
                    {
                        return null;
                    }

                    var edges = keyValueToOutEdges(new KeyValuePair<TVertex, TValue>(key, value));
                    return edges;
                });
        }

        /// <summary>
        /// Creates an instance of <see cref="DelegateVertexAndEdgeListGraph{TVertex,TEdge}"/>
        /// from given vertices and edge try getter.
        /// </summary>
        /// <typeparam name="TVertex">Vertex type.</typeparam>
        /// <typeparam name="TEdge">Edge type.</typeparam>
        /// <param name="vertices">Enumerable of vertices.</param>
        /// <param name="tryGetOutEdges">Getter of out-edges.</param>
        /// <returns>A corresponding <see cref="DelegateVertexAndEdgeListGraph{TVertex,TEdge}"/>.</returns>
        /// <exception cref="T:System.ArgumentNullException"><paramref name="vertices"/> is <see langword="null"/>.</exception>
        /// <exception cref="T:System.ArgumentNullException"><paramref name="tryGetOutEdges"/> is <see langword="null"/>.</exception>
        [Pure]
        [NotNull]
        public static DelegateVertexAndEdgeListGraph<TVertex, TEdge> ToDelegateVertexAndEdgeListGraph<TVertex, TEdge>(
            [NotNull, ItemNotNull] this IEnumerable<TVertex> vertices,
            [NotNull] Func<TVertex, IEnumerable<TEdge>> tryGetOutEdges)
            where TEdge : IEdge<TVertex>
        {
            return new DelegateVertexAndEdgeListGraph<TVertex, TEdge>(vertices, tryGetOutEdges);
        }

        /// <summary>
        /// Creates an instance of <see cref="DelegateBidirectionalIncidenceGraph{TVertex,TEdge}"/>
        /// from these getters of edges.
        /// </summary>
        /// <typeparam name="TVertex">Vertex type.</typeparam>
        /// <typeparam name="TEdge">Edge type.</typeparam>
        /// <param name="tryGetOutEdges">Getter of out-edges.</param>
        /// <param name="tryGetInEdges">Getter of in-edges.</param>
        /// <returns>A corresponding <see cref="DelegateBidirectionalIncidenceGraph{TVertex,TEdge}"/>.</returns>
        /// <exception cref="T:System.ArgumentNullException"><paramref name="tryGetOutEdges"/> is <see langword="null"/>.</exception>
        /// <exception cref="T:System.ArgumentNullException"><paramref name="tryGetInEdges"/> is <see langword="null"/>.</exception>
        [Pure]
        [NotNull]
        public static DelegateBidirectionalIncidenceGraph<TVertex, TEdge> ToDelegateBidirectionalIncidenceGraph<TVertex,
            TEdge>(
            [NotNull] this Func<TVertex, IEnumerable<TEdge>> tryGetOutEdges,
            [NotNull] Func<TVertex, IEnumerable<TEdge>> tryGetInEdges)
            where TEdge : IEdge<TVertex>
        {
            return new DelegateBidirectionalIncidenceGraph<TVertex, TEdge>(tryGetOutEdges, tryGetInEdges);
        }

        /// <summary>
        /// Creates an instance of <see cref="DelegateUndirectedGraph{TVertex,TEdge}"/>
        /// from given vertices and edge getter.
        /// </summary>
        /// <typeparam name="TVertex">Vertex type.</typeparam>
        /// <typeparam name="TEdge">Edge type.</typeparam>
        /// <param name="vertices">Enumerable of vertices.</param>
        /// <param name="tryGetAdjacentEdges">Getter of adjacent edges.</param>
        /// <returns>A corresponding <see cref="DelegateUndirectedGraph{TVertex,TEdge}"/>.</returns>
        /// <exception cref="T:System.ArgumentNullException"><paramref name="vertices"/> is <see langword="null"/>.</exception>
        /// <exception cref="T:System.ArgumentNullException"><paramref name="tryGetAdjacentEdges"/> is <see langword="null"/>.</exception>
        [Pure]
        [NotNull]
        public static DelegateUndirectedGraph<TVertex, TEdge> ToDelegateUndirectedGraph<TVertex, TEdge>(
            [NotNull, ItemNotNull] this IEnumerable<TVertex> vertices,
            [NotNull] Func<TVertex, IEnumerable<TEdge>> tryGetAdjacentEdges)
            where TEdge : IEdge<TVertex>
            => new DelegateUndirectedGraph<TVertex, TEdge>(vertices, tryGetAdjacentEdges);

        #endregion

        #region Graphs

        /// <summary>
        /// Converts a raw array of sources and targets (2 columns) vertices into a graph.
        /// </summary>
        /// <typeparam name="TVertex">Vertex type.</typeparam>
        /// <param name="edges">
        /// Array of vertices defining edges.
        /// The first items of each column represents the number of vertices following.
        /// </param>
        /// <returns>A corresponding <see cref="AdjacencyGraph{TVertex,TEdge}"/>.</returns>
        /// <exception cref="T:System.ArgumentNullException">
        /// <paramref name="edges"/>, <paramref name="edges"/>[0] or <paramref name="edges"/>[1] is <see langword="null"/>.
        /// </exception>
        /// <exception cref="T:System.ArgumentException"><paramref name="edges"/> length is different from 2.</exception>
        /// <exception cref="T:System.ArgumentException"><paramref name="edges"/>[0] length is different from <paramref name="edges"/>[1] length.</exception>
        [Pure]
        [NotNull]
        public static AdjacencyGraph<TVertex, SEquatableEdge<TVertex>> ToAdjacencyGraph<TVertex>(
            [NotNull] this TVertex[][] edges)
        {
            if (edges is null)
                throw new ArgumentNullException(nameof(edges));
            if (edges.Length != 2)
                throw new ArgumentException("Must have a length of 2.", nameof(edges));
            if (edges[0] is null)
                throw new ArgumentNullException(nameof(edges));
            if (edges[1] is null)
                throw new ArgumentNullException(nameof(edges));
            if (edges[0].Length != edges[1].Length)
                throw new ArgumentException("Edges columns must have same size.");

            TVertex[] sources = edges[0];
            TVertex[] targets = edges[1];
            int n = sources.Length;
            var edgePairs = new List<SEquatableEdge<TVertex>>(n);
            for (int i = 0; i < n; ++i)
            {
                edgePairs.Add(new SEquatableEdge<TVertex>(sources[i], targets[i]));
            }

            return ToAdjacencyGraph<TVertex, SEquatableEdge<TVertex>>(edgePairs);
        }

        /// <summary>
        /// Converts a set of edges into an <see cref="AdjacencyGraph{TVertex,TEdge}"/>.
        /// </summary>
        /// <typeparam name="TVertex">Vertex type.</typeparam>
        /// <typeparam name="TEdge">Edge type.</typeparam>
        /// <param name="edges">Set of edges to convert.</param>
        /// <param name="allowParallelEdges">Indicates if parallel edges are allowed.</param>
        /// <returns>A corresponding <see cref="AdjacencyGraph{TVertex,TEdge}"/>.</returns>
        /// <exception cref="T:System.ArgumentNullException">
        /// <paramref name="edges"/> is <see langword="null"/> or at least one of them is <see langword="null"/>.
        /// </exception>
        [Pure]
        [NotNull]
        public static AdjacencyGraph<TVertex, TEdge> ToAdjacencyGraph<TVertex, TEdge>(
            [NotNull, ItemNotNull] this IEnumerable<TEdge> edges,
            bool allowParallelEdges = true)
            where TEdge : IEdge<TVertex>
        {
            var graph = new AdjacencyGraph<TVertex, TEdge>(allowParallelEdges);
            graph.AddVerticesAndEdgeRange(edges);
            return graph;
        }

        /// <summary>
        /// Converts a set of vertex pairs into an <see cref="AdjacencyGraph{TVertex,TEdge}"/>.
        /// </summary>
        /// <typeparam name="TVertex">Vertex type.</typeparam>
        /// <param name="vertexPairs">Set of vertex pairs to convert.</param>
        /// <returns>A corresponding <see cref="AdjacencyGraph{TVertex,TEdge}"/>.</returns>
        /// <exception cref="T:System.ArgumentNullException">
        /// <paramref name="vertexPairs"/> is <see langword="null"/> or at least one of vertex is <see langword="null"/>.
        /// </exception>
        [Pure]
        [NotNull]
        public static AdjacencyGraph<TVertex, SEquatableEdge<TVertex>> ToAdjacencyGraph<TVertex>(
            [NotNull] this IEnumerable<SEquatableEdge<TVertex>> vertexPairs)
        {
            if (vertexPairs is null)
                throw new ArgumentNullException(nameof(vertexPairs));

            var graph = new AdjacencyGraph<TVertex, SEquatableEdge<TVertex>>();
            graph.AddVerticesAndEdgeRange(vertexPairs);
            return graph;
        }

        /// <summary>
        /// Converts a set of vertices into an <see cref="AdjacencyGraph{TVertex,TEdge}"/>
        /// using an edge factory.
        /// </summary>
        /// <typeparam name="TVertex">Vertex type.</typeparam>
        /// <typeparam name="TEdge">Edge type.</typeparam>
        /// <param name="vertices">Set of vertices to convert.</param>
        /// <param name="outEdgesFactory">The out edges factory.</param>
        /// <param name="allowParallelEdges">Indicates if parallel edges are allowed.</param>
        /// <returns>A corresponding <see cref="AdjacencyGraph{TVertex,TEdge}"/>.</returns>
        /// <exception cref="T:System.ArgumentNullException">
        /// <paramref name="vertices"/> is <see langword="null"/> or at least one of them is <see langword="null"/>.
        /// </exception>
        /// <exception cref="T:System.ArgumentNullException">
        /// <paramref name="outEdgesFactory"/> is <see langword="null"/> or creates <see langword="null"/> edge.
        /// </exception>
        [Pure]
        [NotNull]
        public static AdjacencyGraph<TVertex, TEdge> ToAdjacencyGraph<TVertex, TEdge>(
            [NotNull, ItemNotNull] this IEnumerable<TVertex> vertices,
            [NotNull, InstantHandle] Func<TVertex, IEnumerable<TEdge>> outEdgesFactory,
            bool allowParallelEdges = true)
            where TEdge : IEdge<TVertex>
        {
            if (outEdgesFactory is null)
                throw new ArgumentNullException(nameof(outEdgesFactory));

            var graph = new AdjacencyGraph<TVertex, TEdge>(allowParallelEdges);
            graph.AddVertexRange(vertices);

            foreach (TVertex vertex in graph.Vertices)
            {
                graph.AddEdgeRange(outEdgesFactory(vertex));
            }

            return graph;
        }

        /// <summary>
        /// Creates an immutable <see cref="ArrayAdjacencyGraph{TVertex,TEdge}"/> from the input graph.
        /// </summary>
        /// <typeparam name="TVertex">Vertex type.</typeparam>
        /// <typeparam name="TEdge">Edge type.</typeparam>
        /// <param name="graph">Graph to convert.</param>
        /// <returns>A corresponding <see cref="ArrayAdjacencyGraph{TVertex,TEdge}"/>.</returns>
        /// <exception cref="T:System.ArgumentNullException"><paramref name="graph"/> is <see langword="null"/>.</exception>
        [Pure]
        [NotNull]
        public static ArrayAdjacencyGraph<TVertex, TEdge> ToArrayAdjacencyGraph<TVertex, TEdge>(
            [NotNull] this IVertexAndEdgeListGraph<TVertex, TEdge> graph)
            where TEdge : IEdge<TVertex>
        {
            return new ArrayAdjacencyGraph<TVertex, TEdge>(graph);
        }

        /// <summary>
        /// Wraps a graph (out-edges only) into a bidirectional graph.
        /// </summary>
        /// <remarks>For already bidirectional graph it returns itself.</remarks>
        /// <typeparam name="TVertex">Vertex type.</typeparam>
        /// <typeparam name="TEdge">Edge type.</typeparam>
        /// <param name="graph">Graph to convert.</param>
        /// <returns>A corresponding <see cref="IBidirectionalGraph{TVertex,TEdge}"/>.</returns>
        /// <exception cref="T:System.ArgumentNullException"><paramref name="graph"/> is <see langword="null"/>.</exception>
        [Pure]
        [NotNull]
        public static IBidirectionalGraph<TVertex, TEdge> ToBidirectionalGraph<TVertex, TEdge>(
            [NotNull] this IVertexAndEdgeListGraph<TVertex, TEdge> graph)
            where TEdge : IEdge<TVertex>
        {
            if (graph is null)
                throw new ArgumentNullException(nameof(graph));

            if (graph is IBidirectionalGraph<TVertex, TEdge> self)
                return self;

            return new BidirectionalAdapterGraph<TVertex, TEdge>(graph);
        }

        /// <summary>
        /// Converts a set of edges into a <see cref="BidirectionalGraph{TVertex,TEdge}"/>.
        /// </summary>
        /// <typeparam name="TVertex">Vertex type.</typeparam>
        /// <typeparam name="TEdge">Edge type.</typeparam>
        /// <param name="edges">Set of edges to convert.</param>
        /// <param name="allowParallelEdges">Indicates if parallel edges are allowed.</param>
        /// <returns>A corresponding <see cref="BidirectionalGraph{TVertex,TEdge}"/>.</returns>
        /// <exception cref="T:System.ArgumentNullException">
        /// <paramref name="edges"/> is <see langword="null"/> or at least one of them is <see langword="null"/>.
        /// </exception>
        [Pure]
        [NotNull]
        public static BidirectionalGraph<TVertex, TEdge> ToBidirectionalGraph<TVertex, TEdge>(
            [NotNull, ItemNotNull] this IEnumerable<TEdge> edges,
            bool allowParallelEdges = true)
            where TEdge : IEdge<TVertex>
        {
            var graph = new BidirectionalGraph<TVertex, TEdge>(allowParallelEdges);
            graph.AddVerticesAndEdgeRange(edges);
            return graph;
        }

        /// <summary>
        /// Converts a set of vertex pairs into a <see cref="BidirectionalGraph{TVertex,TEdge}"/>.
        /// </summary>
        /// <typeparam name="TVertex">Vertex type.</typeparam>
        /// <param name="vertexPairs">Set of vertex pairs to convert.</param>
        /// <returns>A corresponding <see cref="BidirectionalGraph{TVertex,TEdge}"/>.</returns>
        /// <exception cref="T:System.ArgumentNullException">
        /// <paramref name="vertexPairs"/> is <see langword="null"/> or at least one of vertex is <see langword="null"/>.
        /// </exception>
        [Pure]
        [NotNull]
        public static BidirectionalGraph<TVertex, SEquatableEdge<TVertex>> ToBidirectionalGraph<TVertex>(
            [NotNull] this IEnumerable<SEquatableEdge<TVertex>> vertexPairs)
        {
            if (vertexPairs is null)
                throw new ArgumentNullException(nameof(vertexPairs));

            var graph = new BidirectionalGraph<TVertex, SEquatableEdge<TVertex>>();
            graph.AddVerticesAndEdgeRange(vertexPairs);
            return graph;
        }

        /// <summary>
        /// Converts a set of vertices into a <see cref="BidirectionalGraph{TVertex,TEdge}"/>
        /// using an edge factory.
        /// </summary>
        /// <typeparam name="TVertex">Vertex type.</typeparam>
        /// <typeparam name="TEdge">Edge type.</typeparam>
        /// <param name="vertices">Set of vertices to convert.</param>
        /// <param name="outEdgesFactory">The out edges factory.</param>
        /// <param name="allowParallelEdges">Indicates if parallel edges are allowed.</param>
        /// <returns>A corresponding <see cref="BidirectionalGraph{TVertex,TEdge}"/>.</returns>
        /// <exception cref="T:System.ArgumentNullException">
        /// <paramref name="vertices"/> is <see langword="null"/> or at least one of them is <see langword="null"/>.
        /// </exception>
        /// <exception cref="T:System.ArgumentNullException">
        /// <paramref name="outEdgesFactory"/> is <see langword="null"/> or creates <see langword="null"/> edge.
        /// </exception>
        [Pure]
        [NotNull]
        public static BidirectionalGraph<TVertex, TEdge> ToBidirectionalGraph<TVertex, TEdge>(
            [NotNull, ItemNotNull] this IEnumerable<TVertex> vertices,
            [NotNull, InstantHandle] Func<TVertex, IEnumerable<TEdge>> outEdgesFactory,
            bool allowParallelEdges = true)
            where TEdge : IEdge<TVertex>
        {
            if (outEdgesFactory is null)
                throw new ArgumentNullException(nameof(outEdgesFactory));

            var graph = new BidirectionalGraph<TVertex, TEdge>(allowParallelEdges);
            graph.AddVertexRange(vertices);

            foreach (TVertex vertex in graph.Vertices)
            {
                graph.AddEdgeRange(outEdgesFactory(vertex));
            }

            return graph;
        }

        /// <summary>
        /// Creates a <see cref="BidirectionalGraph{TVertex,TEdge}"/> from this graph.
        /// </summary>
        /// <param name="graph">Graph to convert.</param>
        /// <returns>A corresponding <see cref="BidirectionalGraph{TVertex,TEdge}"/>.</returns>
        /// <exception cref="T:System.ArgumentNullException"><paramref name="graph"/> is <see langword="null"/>.</exception>
        [NotNull]
        public static BidirectionalGraph<TVertex, TEdge> ToBidirectionalGraph<TVertex, TEdge>(
            [NotNull] this IUndirectedGraph<TVertex, TEdge> graph)
            where TEdge : IEdge<TVertex>
        {
            if (graph is null)
                throw new ArgumentNullException(nameof(graph));

            var newGraph = new BidirectionalGraph<TVertex, TEdge>();

            newGraph.AddVertexRange(graph.Vertices);
            newGraph.AddEdgeRange(graph.Edges);

            return newGraph;
        }

        /// <summary>
        /// Creates an immutable <see cref="ArrayBidirectionalGraph{TVertex,TEdge}"/> from the input graph.
        /// </summary>
        /// <typeparam name="TVertex">Vertex type.</typeparam>
        /// <typeparam name="TEdge">Edge type.</typeparam>
        /// <param name="graph">Graph to convert.</param>
        /// <returns>A corresponding <see cref="ArrayBidirectionalGraph{TVertex,TEdge}"/>.</returns>
        /// <exception cref="T:System.ArgumentNullException"><paramref name="graph"/> is <see langword="null"/>.</exception>
        [Pure]
        [NotNull]
        public static ArrayBidirectionalGraph<TVertex, TEdge> ToArrayBidirectionalGraph<TVertex, TEdge>(
            [NotNull] this IBidirectionalGraph<TVertex, TEdge> graph)
            where TEdge : IEdge<TVertex>
        {
            return new ArrayBidirectionalGraph<TVertex, TEdge>(graph);
        }

        /// <summary>
        /// Converts a sequence of edges into an <see cref="UndirectedGraph{TVertex,TEdge}"/>.
        /// </summary>
        /// <typeparam name="TVertex">Vertex type.</typeparam>
        /// <typeparam name="TEdge">Edge type.</typeparam>
        /// <param name="edges">Set of edges to convert.</param>
        /// <param name="allowParallelEdges">Indicates if parallel edges are allowed.</param>
        /// <returns>A corresponding <see cref="UndirectedGraph{TVertex,TEdge}"/>.</returns>
        /// <exception cref="T:System.ArgumentNullException">
        /// <paramref name="edges"/> is <see langword="null"/> or at least one of them is <see langword="null"/>.
        /// </exception>
        [Pure]
        [NotNull]
        public static UndirectedGraph<TVertex, TEdge> ToUndirectedGraph<TVertex, TEdge>(
            [NotNull, ItemNotNull] this IEnumerable<TEdge> edges,
            bool allowParallelEdges = true)
            where TEdge : IEdge<TVertex>
        {
            var graph = new UndirectedGraph<TVertex, TEdge>(allowParallelEdges);
            graph.AddVerticesAndEdgeRange(edges);
            return graph;
        }

        /// <summary> Creates a new, 'simple' graph without parallel edges from the <paramref name="multiGraph"/>.</summary>
        public static UndirectedGraph<TVertex, TEdge> RemoveParallelAndSelfEdges<TVertex, TEdge>(
            this IUndirectedGraph<TVertex, TEdge> multiGraph) where TEdge : IEdge<TVertex>
        {
            var simpleGraph = new UndirectedGraph<TVertex, TEdge>(false, multiGraph?.EdgeEqualityComparer);
            simpleGraph.AddVertexRange(multiGraph.Vertices);
            simpleGraph.AddEdgeRange(multiGraph.Edges);
            simpleGraph.RemoveEdgeIf(edge => edge.IsSelfEdge(multiGraph.AreVerticesEqual));
            return simpleGraph;
        }

        /// <summary> Checks if the <paramref name="vertex"/> satisfies Dirac's theorem that deg(vertex) >= (|vertices| / 2). </summary>  
        /// <remarks>
        /// According to Dirac's theorem, a graph with |vertices| >= 3
        /// that SatisfiesDiracTheorem for any vertex is Hamiltonian.
        /// </remarks>
        [Pure]
        public static bool SatisfiesDiracTheorem<TVertex, TEdge>(this IUndirectedGraph<TVertex, TEdge> graph
            , [NotNull] TVertex vertex) where TEdge : IEdge<TVertex>
            => graph.AdjacentDegree(vertex) * 2 >= graph.VertexCount;

        /// <summary> Checks if the <paramref name="simpleGraph"/> is Hamiltonian,
        /// i.e. has a path that links all vertices and passes each only once. </summary>
        /// <remarks>
        /// The simpleGraph is a pre-processed Graph, to remove any duplicate and Self-Edges.
        /// 
        /// First checks Dirac's theorem, that a graph with |vertices| >= 3
        /// that SatisfiesDiracTheorem for any vertex is Hamiltonian.
        ///
        /// If that fails it performs a brute-force Test for all Permutations. 
        /// </remarks>
        [Pure]
        public static bool IsSimpleAndHamiltonian<TVertex, TEdge>(this IUndirectedGraph<TVertex, TEdge> simpleGraph) where TEdge : IEdge<TVertex>
        {
            var graphVertices = simpleGraph.Vertices.ToList();
            int vertexCount = simpleGraph.VertexCount;
            return vertexCount == 1
                   || (vertexCount >= 3 && graphVertices.All(simpleGraph.SatisfiesDiracTheorem))
                   || graphVertices.GetAllPermutations().Any(simpleGraph.ContainsPath);
        }

        /// <summary> Returns true if the <paramref name="graph"/> contains the <paramref name="path"/>. </summary>
        [Pure]
        public static bool ContainsPath<TVertex, TEdge>(this IImplicitUndirectedGraph<TVertex, TEdge> graph
            , [NotNull, ItemNotNull] List<TVertex> path) where TEdge : IEdge<TVertex>
        {
            if (path.Count > 1)
            {
                path.Add(path[0]);      // Make cycle, not simple path
            }

            for (int i = 0; i < path.Count - 1; i++)
            {
                if (true != graph.AdjacentVertices(path[i])?.Contains(path[i + 1]))
                {
                    return false;
                }
            }

            return true;
        }

        /// <summary> Returns the set of vertices adjacent to the given <paramref name="vertex"/>, except for itself. </summary>
        /// <exception cref="T:System.ArgumentNullException"><paramref name="vertex"/> is <see langword="null"/>.</exception>
        [Pure]
        [ItemNotNull]
        [CanBeNull]
        public static HashSet<TVertex> AdjacentVertices<TVertex, TEdge>(this IImplicitUndirectedGraph<TVertex, TEdge> graph
            , TVertex vertex) where TEdge : IEdge<TVertex>
        {
            var adjacentEdges = graph.AdjacentEdges(vertex);
            if (adjacentEdges is null)
            {
                return null;
            }

            var adjacentVertices = adjacentEdges.GetVertices<TVertex, TEdge>();
            adjacentVertices.Remove(vertex);
            return adjacentVertices;
        }

        /// <summary>
        /// Converts a set of vertex pairs into an <see cref="UndirectedGraph{TVertex,TEdge}"/>.
        /// </summary>
        /// <typeparam name="TVertex">Vertex type.</typeparam>
        /// <param name="vertexPairs">Set of vertex pairs to convert.</param>
        /// <returns>A corresponding <see cref="UndirectedGraph{TVertex,TEdge}"/>.</returns>
        /// <exception cref="T:System.ArgumentNullException"><paramref name="vertexPairs"/> is <see langword="null"/>.</exception>
        [Pure]
        [NotNull]
        public static UndirectedGraph<TVertex, SEquatableEdge<TVertex>> ToUndirectedGraph<TVertex>(
            [NotNull] this IEnumerable<SEquatableEdge<TVertex>> vertexPairs)
        {
            if (vertexPairs is null)
                throw new ArgumentNullException(nameof(vertexPairs));

            var graph = new UndirectedGraph<TVertex, SEquatableEdge<TVertex>>();
            graph.AddVerticesAndEdgeRange(vertexPairs);
            return graph;
        }

        /// <summary>
        /// Creates an immutable <see cref="ArrayUndirectedGraph{TVertex,TEdge}"/> from the input graph.
        /// </summary>
        /// <typeparam name="TVertex">Vertex type.</typeparam>
        /// <typeparam name="TEdge">Edge type.</typeparam>
        /// <param name="graph">Graph to convert.</param>
        /// <returns>A corresponding <see cref="ArrayUndirectedGraph{TVertex,TEdge}"/>.</returns>
        /// <exception cref="T:System.ArgumentNullException"><paramref name="graph"/> is <see langword="null"/>.</exception>
        [Pure]
        [NotNull]
        public static ArrayUndirectedGraph<TVertex, TEdge> ToArrayUndirectedGraph<TVertex, TEdge>(
            [NotNull] this IUndirectedGraph<TVertex, TEdge> graph)
            where TEdge : IEdge<TVertex>
        {
            return new ArrayUndirectedGraph<TVertex, TEdge>(graph);
        }

        /// <summary>
        /// Creates an immutable compressed row graph representation of the <paramref name="graph"/>.
        /// </summary>
        /// <typeparam name="TVertex">Vertex type.</typeparam>
        /// <typeparam name="TEdge">Edge type.</typeparam>
        /// <param name="graph">Graph to visit.</param>
        /// <returns>A corresponding <see cref="CompressedSparseRowGraph{TVertex}"/>.</returns>
        /// <exception cref="T:System.ArgumentNullException"><paramref name="graph"/> is <see langword="null"/>.</exception>
        [Pure]
        [NotNull]
        public static CompressedSparseRowGraph<TVertex> ToCompressedRowGraph<TVertex, TEdge>(
            [NotNull] this IVertexAndEdgeListGraph<TVertex, TEdge> graph)
            where TEdge : IEdge<TVertex>
        {
            return CompressedSparseRowGraph<TVertex>.FromGraph(graph);
        }

        #endregion

        /// <summary> Params makes it easier to add Nodes via Code </summary>
        public static int AddVerticesAndEdgeRange<TVertex, TEdge>(this IMutableVertexAndEdgeSet<TVertex, IEdge<TVertex>> graph
            , params TEdge[] edges)
            where TEdge : IEdge<TVertex> => graph.AddVerticesAndEdgeRange((IEnumerable<IEdge<TVertex>>)edges.AsEnumerable());

        /// <summary> Params makes it easier to add Nodes via Code </summary>
        public static int AddVerticesAndEdgeRange<TVertex, TEdge>(this IMutableVertexAndEdgeSet<TVertex, TEdge> graph
            , params TEdge[] edges)
            where TEdge : IEdge<TVertex> => graph.AddVerticesAndEdgeRange(edges.AsEnumerable());

    }
}