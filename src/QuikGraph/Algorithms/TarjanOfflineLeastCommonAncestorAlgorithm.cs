using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using QuikGraph.Algorithms.Search;
using QuikGraph.Algorithms.Services;
using QuikGraph.Collections;

namespace QuikGraph.Algorithms
{
    /// <inheritdoc cref="CreateTarjanOfflineLeastCommonAncestorAlgorithm"/>
    public static class TarjanOfflineLeastCommonAncestorAlgorithm
    {
        /// <summary> Creates a <see cref="TarjanOfflineLeastCommonAncestorAlgorithm{TVertex, TEdge}"/> </summary>
        public static TarjanOfflineLeastCommonAncestorAlgorithm<TVertex, TEdge>
            CreateTarjanOfflineLeastCommonAncestorAlgorithm<TVertex, TEdge>(
            [NotNull] this IVertexListGraph<TVertex, TEdge> visitedGraph,
            [CanBeNull] IAlgorithmComponent host = null) where TEdge : IEdge<TVertex>
            => new TarjanOfflineLeastCommonAncestorAlgorithm<TVertex, TEdge>(visitedGraph);
    }

    /// <summary> Offline least common ancestor in a rooted tree. </summary>
    /// <remarks>
    /// Reference:
    /// Gabow, H. N. and Tarjan, R. E. 1983. A linear-time algorithm for a special case 
    /// of disjoint set union. In Proceedings of the Fifteenth Annual ACM Symposium 
    /// on theory of Computing STOC '83. ACM, New York, NY, 246-251. 
    /// DOI= http://doi.acm.org/10.1145/800061.808753 
    /// </remarks>
    /// <typeparam name="TVertex">Vertex type.</typeparam>
    /// <typeparam name="TEdge">Edge type.</typeparam>
    public sealed class TarjanOfflineLeastCommonAncestorAlgorithm<TVertex, TEdge>
        : RootedAlgorithmBase<TVertex, IVertexListGraph<TVertex, TEdge>>
        where TEdge : IEdge<TVertex>
    {
        [CanBeNull]
        private SEquatableEdge<TVertex>[] _pairs;

        /// <summary>
        /// Ancestors of vertices pairs.
        /// </summary>
        [NotNull]
        public IDictionary<SEquatableEdge<TVertex>, TVertex> Ancestors { get; } = 
            new Dictionary<SEquatableEdge<TVertex>, TVertex>();

        /// <summary>
        /// Initializes a new <see cref="TarjanOfflineLeastCommonAncestorAlgorithm{TVertex,TEdge}"/> class.
        /// </summary>
        /// <param name="visitedGraph">Graph to visit.</param>
        /// <param name="host">Host to use if set, otherwise use this reference.</param>
        /// <exception cref="T:System.ArgumentNullException"><paramref name="visitedGraph"/> is <see langword="null"/>.</exception>
        internal TarjanOfflineLeastCommonAncestorAlgorithm(
            [NotNull] IVertexListGraph<TVertex, TEdge> visitedGraph,
            [CanBeNull] IAlgorithmComponent host = null)
            : base(visitedGraph, host)
        {
        }

        #region AlgorithmBase<TGraph>

        /// <inheritdoc />
        protected override void Initialize()
        {
            base.Initialize();
            Ancestors.Clear();
        }

        /// <inheritdoc />
        protected override void InternalCompute()
        {
            TVertex root = GetAndAssertRootInGraph();
            if (_pairs is null)
                throw new InvalidOperationException("Pairs not set.");

            var graph = _pairs.ToAdjacencyGraph();
            var disjointSet = new ForestDisjointSet<TVertex>();
            var verticesAncestors = new Dictionary<TVertex, TVertex>();
            var dfs = VisitedGraph.CreateDepthFirstSearchAlgorithm(null, null, this);

            dfs.InitializeVertex += vertex => disjointSet.MakeSet(vertex);
            dfs.DiscoverVertex += vertex => verticesAncestors[vertex] = vertex;
            dfs.TreeEdge += edge =>
            {
                disjointSet.Union(edge.Source, edge.Target);
                // ReSharper disable once AssignNullToNotNullAttribute
                // Justification: must be in the set because unioned just before.
                verticesAncestors[disjointSet.FindSet(edge.Source)] = edge.Source;
            };
            dfs.FinishVertex += vertex =>
            {
                foreach (SEquatableEdge<TVertex> edge in graph.OutEdges(vertex))
                {
                    if (dfs.VerticesColors[edge.Target] == GraphColor.Black)
                    {
                        SEquatableEdge<TVertex> pair = edge.ToVertexPair();
                        // ReSharper disable once AssignNullToNotNullAttribute
                        // Justification: must be present in the set.
                        Ancestors[pair] = verticesAncestors[disjointSet.FindSet(edge.Target)];
                    }
                }
            };

            // Run DFS
            dfs.Compute(root);
        }

        #endregion

        /// <summary> The vertex-pairs if set. </summary>
        [Pure]
        [CanBeNull]
        public IEnumerable<SEquatableEdge<TVertex>> VertexPairs() => _pairs;

        /// <summary> Sets vertices pairs. </summary>
        /// <param name="pairs">Vertices pairs.</param>
        /// <exception cref="T:System.ArgumentNullException"><paramref name="pairs"/> is <see langword="null"/>.</exception>
        /// <exception cref="T:System.ArgumentException"><paramref name="pairs"/> is empty or any vertex from pairs is not part of <see cref="AlgorithmBase{TGraph}.VisitedGraph"/>.</exception>
        public void SetVertexPairs([NotNull] params SEquatableEdge<TVertex>[] pairs) => SetVertexPairs(pairs.AsEnumerable());

        /// <summary> Sets vertices pairs. </summary>
        /// <param name="pairs">Vertices pairs.</param>
        /// <exception cref="T:System.ArgumentNullException"><paramref name="pairs"/> is <see langword="null"/>.</exception>
        /// <exception cref="T:System.ArgumentException"><paramref name="pairs"/> is empty or any vertex from pairs is not part of <see cref="AlgorithmBase{TGraph}.VisitedGraph"/>.</exception>
        public void SetVertexPairs([NotNull] IEnumerable<SEquatableEdge<TVertex>> pairs)
        {
            if (pairs is null)
                throw new ArgumentNullException(nameof(pairs));

            _pairs = pairs.ToArray();

            if (_pairs.Length == 0)
                throw new ArgumentException("Must have at least one vertex pair.", nameof(pairs));
            if (_pairs.Any(pair => !VisitedGraph.ContainsVertex(pair.Source)))
                throw new ArgumentException("All pairs sources must be in the graph.", nameof(pairs));
            if (_pairs.Any(pair => !VisitedGraph.ContainsVertex(pair.Target)))
                throw new ArgumentException("All pairs targets must be in the graph.", nameof(pairs));
        }

        /// <summary>
        /// Runs the algorithm with the given <paramref name="root"/> vertex and set of vertices pairs.
        /// </summary>
        /// <param name="root">Root vertex.</param>
        /// <param name="pairs">Vertices pairs.</param>
        /// <exception cref="T:System.ArgumentNullException"><paramref name="root"/> is <see langword="null"/>.</exception>
        /// <exception cref="T:System.ArgumentNullException"><paramref name="pairs"/> is <see langword="null"/>.</exception>
        /// <exception cref="T:System.ArgumentException"><paramref name="root"/> is not part of <see cref="AlgorithmBase{TGraph}.VisitedGraph"/>.</exception>
        /// <exception cref="T:System.ArgumentException"><paramref name="pairs"/> is empty or any vertex from pairs is not part of <see cref="AlgorithmBase{TGraph}.VisitedGraph"/>.</exception>
        public void Compute([NotNull] TVertex root, [NotNull] IEnumerable<SEquatableEdge<TVertex>> pairs)
        {
            SetVertexPairs(pairs);
            Compute(root);
        }
    }
}