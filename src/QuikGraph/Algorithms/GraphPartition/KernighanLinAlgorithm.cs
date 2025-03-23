using System;
using System.Collections.Generic;
using System.Diagnostics;
using JetBrains.Annotations;
using System.Linq;
#if !SUPPORTS_SORTEDSET
using QuikGraph.Collections;
#endif

namespace QuikGraph.Algorithms.GraphPartition
{
    /// <inheritdoc cref="CreateKernighanLinAlgorithm"/>
    public static class KernighanLinAlgorithm
    {
        /// <summary> Creates a new <see cref="KernighanLinAlgorithm{TVertex,TEdge}"/> class. </summary>
        public static KernighanLinAlgorithm<TVertex, TEdge> CreateKernighanLinAlgorithm<TVertex, TEdge>(
            [NotNull] this IUndirectedGraph<TVertex, TEdge> visitedGraph,
            int nbIterations) where TEdge : IUndirectedEdge<TVertex>, ITagged<double>
            => new KernighanLinAlgorithm<TVertex, TEdge>(visitedGraph, nbIterations);
    }

    /// <summary> Separates a graph into two disjoint subsets A and B of equal (or nearly equal) size,
    /// in a way that minimizes the sum of the weights of the subset of edges that cross from A to B.
    /// </summary>
    public sealed class KernighanLinAlgorithm<TVertex, TEdge>
        : AlgorithmBase<IUndirectedGraph<TVertex, TEdge>>
        where TEdge : IUndirectedEdge<TVertex>, ITagged<double>
    {
        private readonly int _nbIterations;
        private readonly int _partitionSize;

        private SortedSet<TVertex> _vertexSetA;
        private SortedSet<TVertex> _vertexSetB;

        private SortedSet<TVertex> _unSwappedSetA;
        private SortedSet<TVertex> _unSwappedSetB;

        /// <summary> Creates a new <see cref="KernighanLinAlgorithm{TVertex,TEdge}"/> class. </summary>
        /// <param name="visitedGraph">Graph to visit.</param>
        /// <param name="nbIterations">Number of iterations to perform.</param>
        /// <exception cref="T:System.ArgumentNullException"><paramref name="visitedGraph"/> is <see langword="null"/>.</exception>
        internal KernighanLinAlgorithm(
            [NotNull] IUndirectedGraph<TVertex, TEdge> visitedGraph,
            int nbIterations)
            : base(visitedGraph)
        {
            _nbIterations = nbIterations;
            _partitionSize = visitedGraph.VertexCount / 2;
        }

        /// <summary>
        /// Partition created by the algorithm.
        /// </summary>
        public Partition<TVertex> Partition { get; private set; }

        private sealed class SwapPair
        {
            public TVertex Vertex1 { get; }
            public TVertex Vertex2 { get; }

            public SwapPair([NotNull] TVertex vertex1, [NotNull] TVertex vertex2)
            {
                Debug.Assert(vertex1 != null);
                Debug.Assert(vertex2 != null);

                Vertex1 = vertex1;
                Vertex2 = vertex2;
            }
        }

        private Partition<TVertex> DoAllSwaps()
        {
            var swaps = new List<SwapPair>();
            double minCost = double.MaxValue;
            int minId = -1;

            for (int i = 0; i < _partitionSize; ++i)
            {
                double cost = SingleSwap(swaps);
                if (cost < minCost)
                {
                    minCost = cost;
                    minId = i;
                }
            }

            // Back to swap step with min cut cost
            while (swaps.Count - 1 > minId)
            {
                SwapPair pair = swaps.Last();
                swaps.Remove(pair);
                SwapVertices(_vertexSetA, pair.Vertex2, _vertexSetB, pair.Vertex1);
            }

            return new Partition<TVertex>(_vertexSetA, _vertexSetB, minCost);
        }

        private double SingleSwap([NotNull, ItemNotNull] ICollection<SwapPair> swaps)
        {
            SwapPair maxPair = null;
            double maxGain = double.MinValue;
            foreach (TVertex vertexFromA in _unSwappedSetA)
            {
                foreach (TVertex vertexFromB in _unSwappedSetB)
                {
                    bool foundEdge = FindEdge(vertexFromA, vertexFromB, out TEdge edge);
                    double edgeCost = foundEdge ? edge.Tag : 0.0;

                    double gain = GetVertexCost(vertexFromA) + GetVertexCost(vertexFromB) - 2 * edgeCost;
                    if (gain > maxGain)
                    {
                        maxPair = new SwapPair(vertexFromA, vertexFromB);
                        maxGain = gain;
                    }
                }
            }

            if (maxPair is null)
                throw new InvalidOperationException("Must find a swap.");

            SwapVertices(_vertexSetA, maxPair.Vertex1, _vertexSetB, maxPair.Vertex2);
            swaps.Add(maxPair);
            _unSwappedSetA.Remove(maxPair.Vertex1);
            _unSwappedSetB.Remove(maxPair.Vertex2);

            return GetCutCost();
        }

        [Pure]
        private double GetVertexCost([NotNull] TVertex vertex)
        {
            Debug.Assert(vertex != null);

            double cost = 0;
            bool vertexIsInA = _vertexSetA.Contains(vertex);

            foreach (TVertex neighborVertex in GetNeighbors(vertex))
            {
                bool vertexNeighborIsInA = _vertexSetA.Contains(neighborVertex);
                if (!FindEdge(vertex, neighborVertex, out TEdge edge))
                    continue;

                if (vertexIsInA != vertexNeighborIsInA) // External
                {
                    cost += edge.Tag;
                }
                else
                {
                    cost -= edge.Tag;
                }
            }

            return cost;
        }

        [Pure]
        [NotNull, ItemNotNull]
        private IEnumerable<TVertex> GetNeighbors([NotNull] TVertex vertex)
        {
            Debug.Assert(vertex != null);

            var neighbors = new HashSet<TVertex>();
            foreach (TEdge edge in VisitedGraph.AdjacentEdges(vertex))
            {
                neighbors.Add(edge.Source);
                neighbors.Add(edge.Target);
            }

            if (neighbors.Contains(vertex))
            {
                neighbors.Remove(vertex);
            }

            return neighbors;
        }

        private static void SwapVertices(
            [NotNull, ItemNotNull] ISet<TVertex> setA,
            [NotNull] TVertex vertexA,
            [NotNull, ItemNotNull] ISet<TVertex> setB,
            [NotNull] TVertex vertexB)
        {
            Debug.Assert(setA != null);
            Debug.Assert(vertexA != null);
            Debug.Assert(setB != null);
            Debug.Assert(vertexB != null);

            if (!setA.Contains(vertexA)
                || setA.Contains(vertexB)
                || !setB.Contains(vertexB)
                || setB.Contains(vertexA))
                throw new InvalidOperationException("Invalid swap.");

            setA.Remove(vertexA);
            setA.Add(vertexB);

            setB.Remove(vertexB);
            setB.Add(vertexA);
        }

        [Pure]
        private double GetCutCost()
        {
            double cost = 0;
            foreach (TEdge edge in VisitedGraph.Edges)
            {
                if (_vertexSetA.Contains(edge.Source) != _vertexSetA.Contains(edge.Target))
                {
                    cost += edge.Tag;
                }
            }

            return cost;
        }

        /// <summary>
        /// Searches for an edge that links <paramref name="vertexFromA"/> and <paramref name="vertexFromB"/>.
        /// </summary>
        [Pure]
        private bool FindEdge([NotNull] TVertex vertexFromA, [NotNull] TVertex vertexFromB, out TEdge foundEdge)
        {
            foreach (TEdge edge in VisitedGraph.AdjacentEdges(vertexFromA))
            {
                if (VisitedGraph.AreVerticesEqual(edge.Target, vertexFromB)
                    || VisitedGraph.AreVerticesEqual(edge.Source, vertexFromB))
                {
                    foundEdge = edge;
                    return true;
                }
            }

            foundEdge = default(TEdge);
            return false;
        }

        private void GetStartPartition()
        {
            int i = 0;
            foreach (TVertex vertex in VisitedGraph.Vertices)
            {
                if (i < _partitionSize)
                {
                    _vertexSetA.Add(vertex);
                }
                else
                {
                    _vertexSetB.Add(vertex);
                }

                ++i;
            }
        }

        #region AlgorithmBase<TGraph>

        /// <inheritdoc />
        protected override void InternalCompute()
        {
            _vertexSetA = new SortedSet<TVertex>();
            _vertexSetB = new SortedSet<TVertex>();

            GetStartPartition();

            _unSwappedSetA = new SortedSet<TVertex>(_vertexSetA);
            _unSwappedSetB = new SortedSet<TVertex>(_vertexSetB);

            var bestPartition = new Partition<TVertex>(_vertexSetA, _vertexSetB);
            double minCost = double.MaxValue;

            for (int i = 0; i < _nbIterations; ++i)
            {
                Partition<TVertex> tmpPartition = DoAllSwaps();
                double tmpCutCost = tmpPartition.CutCost;

                _vertexSetA = tmpPartition.VertexSetA;
                _vertexSetB = tmpPartition.VertexSetB;
                _unSwappedSetA = _vertexSetA;
                _unSwappedSetB = _vertexSetB;

                if (tmpCutCost < minCost)
                {
                    bestPartition = tmpPartition;
                    minCost = tmpCutCost;
                }
            }

            Partition = bestPartition;
        }

        #endregion
    }
}