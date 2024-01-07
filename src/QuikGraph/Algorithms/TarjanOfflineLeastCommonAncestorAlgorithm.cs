using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using QuikGraph.Algorithms.Search;
using QuikGraph.Algorithms.Services;
using QuikGraph.Collections;

namespace QuikGraph.Algorithms
{
    /// <summary>
    /// Offline least common ancestor in a rooted tree.
    /// </summary>
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
        /// Initializes a new instance of the <see cref="TarjanOfflineLeastCommonAncestorAlgorithm{TVertex,TEdge}"/> class.
        /// </summary>
        /// <param name="visitedGraph">Graph to visit.</param>
        /// <exception cref="T:System.ArgumentNullException"><paramref name="visitedGraph"/> is <see langword="null"/>.</exception>
        public TarjanOfflineLeastCommonAncestorAlgorithm(
            [NotNull] IVertexListGraph<TVertex, TEdge> visitedGraph)
            : this(null, visitedGraph)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="TarjanOfflineLeastCommonAncestorAlgorithm{TVertex,TEdge}"/> class.
        /// </summary>
        /// <param name="host">Host to use if set, otherwise use this reference.</param>
        /// <param name="visitedGraph">Graph to visit.</param>
        /// <exception cref="T:System.ArgumentNullException"><paramref name="visitedGraph"/> is <see langword="null"/>.</exception>
        public TarjanOfflineLeastCommonAncestorAlgorithm(
            [CanBeNull] IAlgorithmComponent host,
            [NotNull] IVertexListGraph<TVertex, TEdge> visitedGraph)
            : base(host, visitedGraph)
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
            var dfs = new DepthFirstSearchAlgorithm<TVertex, TEdge>(
                this, 
                VisitedGraph, 
                new Dictionary<TVertex, GraphColor>(VisitedGraph.VertexCount));

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

        /// <summary>
        /// Tries to get vertices pairs if set.
        /// </summary>
        /// <param name="pairs">Vertices pairs if set.</param>
        /// <returns>True if vertex pairs were set, false otherwise.</returns>
        [Pure]
        [ContractAnnotation("=> true, pairs:notnull;=> false, pairs:null")]
        public bool TryGetVertexPairs(out IEnumerable<SEquatableEdge<TVertex>> pairs)
        {
            pairs = _pairs;
            return pairs != null;
        }

        /// <summary>
        /// Sets vertices pairs.
        /// </summary>
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
            if (Array.Exists(_pairs, pair => !VisitedGraph.ContainsVertex(pair.Source)))
                throw new ArgumentException("All pairs sources must be in the graph.", nameof(pairs));
            if (Array.Exists(_pairs, pair => !VisitedGraph.ContainsVertex(pair.Target)))
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