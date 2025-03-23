using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;

namespace QuikGraph.Algorithms.TSP
{
    /// <summary> Minimizes the <see cref="MinCost"/> for the <see cref="Path"/> </summary>
    internal sealed class Task<TVertex, TEdge>
        where TEdge : EquatableEdge<TVertex>
    {
        [NotNull]
        private readonly BidirectionalGraph<TVertex, TEdge> _graph;

        /// <summary> The Cost for each edge </summary>
        [NotNull]
        private readonly IDictionary<EquatableEdge<TVertex>, double> _weight;

        /// <summary> The Path to optimize </summary>
        [NotNull]
        public BidirectionalGraph<TVertex, TEdge> Path { get; }

        [NotNull]
        public string TaskName { get; }

        public double MinCost { get; private set; }

        [NotNull]
        public TaskPriority Priority { get; }

        public bool ConnectsAllVertices() => Path.EdgeCount == _graph.VertexCount;

        public Task(
            [NotNull] BidirectionalGraph<TVertex, TEdge> graph,
            [NotNull] IDictionary<EquatableEdge<TVertex>, double> weights,
            [NotNull] BidirectionalGraph<TVertex, TEdge> path, double cost,
            [NotNull] string taskName = "Init")
        {
            TaskName = taskName;
            _graph = new BidirectionalGraph<TVertex, TEdge>(graph);
            _weight = new Dictionary<EquatableEdge<TVertex>, double>(weights);
            Path = path;
            BuildInitialPath();
            MinCost = cost + Reduce();
            Priority = new TaskPriority(MinCost, path.EdgeCount);
        }

        private void BuildInitialPath()
        {
            if (CanAddSingleEdgeWithoutCycle())
                return;

            foreach(var edge in Path.EdgesWithoutCycles(_graph.Edges).ToList())
            {
                _graph.RemoveEdge(edge);
                _weight.Remove(edge);
            };
        }

        private bool CanAddSingleEdgeWithoutCycle()
        {
            if (_graph.EdgeCount != 1)
            {
                return false;
            }

            var singleEdge = _graph.Edges.First();
            Path.AddEdge(singleEdge);
            if (Path.IsDirectedAcyclicGraph())
            {
                Path.RemoveEdge(singleEdge);
                return false;
            }

            return true;

        }

        private double Reduce()
        {
            if (_graph.IsEdgesEmpty)
            {
                return double.PositiveInfinity;
            }

            double sum = ReduceOutEdges();
            sum += ReduceInEdges();

            return sum;
        }

        private double ReduceOutEdges()
        {
            double sum = 0;
            foreach (TVertex vertex in _graph.Vertices)
            {
                TEdge[] outEdgesArray = _graph.OutEdges(vertex).AsArray();
                if (outEdgesArray.Length <= 0)
                    continue;

                double min = outEdgesArray.Min(edge => _weight[edge]);
                foreach (TEdge edge in outEdgesArray)
                {
                    _weight[edge] -= min;
                }

                sum += min;
            }

            return sum;
        }

        private double ReduceInEdges()
        {
            double sum = 0;
            foreach (TVertex vertex in _graph.Vertices)
            {
                var inEdges = _graph.InEdges(vertex);
                //if (inEdges.IsNullOrEmpty())
                //    continue;

                //if (!_graph.InEdges(vertex, out IEnumerable<TEdge> inEdges))
                //    continue;

                TEdge[] inEdgesArray = inEdges.AsArray(); // ?? Array.Empty<TEdge>();
                if (inEdgesArray.Length <= 0)
                    continue;

                double min = inEdgesArray.Min(edge => _weight[edge]);
                foreach (TEdge edge in inEdgesArray)
                {
                    _weight[edge] -= min;
                }

                sum += min;
            }

            return sum;
        }

        [NotNull, ItemNotNull]
        private IEnumerable<TEdge> GetZeroEdges(double threshold = double.Epsilon)
        {
            var zeros = new List<TEdge>();
            foreach (TVertex vertex in _graph.Vertices)
            {
                var outEdges = _graph.OutEdges(vertex);
                if (outEdges != null)
                {
                    zeros.AddRange(outEdges.Where(edge => Math.Abs(_weight[edge]) < threshold));
                }
            }

            return zeros;
        }

        [Pure]
        private double ComputeMaxCandidate(
            [NotNull, ItemNotNull] IEnumerable<TEdge> row,
            [NotNull, ItemNotNull] IEnumerable<TEdge> column,
            [NotNull] TVertex source,
            [NotNull] TVertex target)
        {
            return
                row.Where(edge => !_graph.AreVerticesEqual(edge.Target, target))
                    .DefaultIfEmpty(null)
                    .Min(edge => edge is null
                        ? double.PositiveInfinity
                        : _weight[edge])
                +
                column.Where(edge => !_graph.AreVerticesEqual(edge.Source, source))
                    .DefaultIfEmpty(null)
                    .Min(edge => edge is null
                        ? double.PositiveInfinity
                        : _weight[edge]);
        }

        private TEdge ChooseEdgeForSplit()
        {
            TEdge edgeForSplit = null;
            double max = double.NegativeInfinity;
            foreach (TEdge edge in GetZeroEdges())
            {
                TVertex v1 = edge.Source;
                TVertex v2 = edge.Target;

                var row = _graph.OutEdges(v1);
                var column = _graph.InEdges(v2);
                {
                    double maxCandidate = ComputeMaxCandidate(row, column, v1, v2);

                    if (max < maxCandidate)
                    {
                        max = maxCandidate;
                        edgeForSplit = edge;
                    }
                }
            }

            return edgeForSplit;
        }

        private bool CanSplit() => MinCost < double.PositiveInfinity;

        public bool Split(out Task<TVertex, TEdge> taskTake, out Task<TVertex, TEdge> taskDrop)
        {
            if (!CanSplit())
            {
                taskTake = null;
                taskDrop = null;
                return false;
            }

            TEdge edgeForSplit = ChooseEdgeForSplit();
            TVertex v1 = edgeForSplit.Source;
            TVertex v2 = edgeForSplit.Target;

            BidirectionalGraph<TVertex, TEdge> graphTake = _graph.Clone();
            var weightsTake = new Dictionary<EquatableEdge<TVertex>, double>(_weight);
            var reverseEdge = new EquatableEdge<TVertex>(edgeForSplit.Target, edgeForSplit.Source);
            weightsTake.Remove(reverseEdge);
            graphTake.RemoveEdgeIf(edge => reverseEdge.Equals(edge));

            foreach (TEdge outEdge in graphTake.OutEdges(v1))
            {
                weightsTake.Remove(outEdge);
            }

            foreach (TEdge inEdge in graphTake.InEdges(v2))
            {
                weightsTake.Remove(inEdge);
            }

            graphTake.ClearOutEdges(v1);
            graphTake.ClearInEdges(v2);
            var pathTake = new BidirectionalGraph<TVertex, TEdge>(Path);
            pathTake.AddEdge(edgeForSplit);
            taskTake = new Task<TVertex, TEdge>(graphTake, weightsTake, pathTake, MinCost, $"Take{edgeForSplit}");

            BidirectionalGraph<TVertex, TEdge> graphDrop = _graph.Clone();
            var weightsDrop = new Dictionary<EquatableEdge<TVertex>, double>(_weight);
            weightsDrop.Remove(edgeForSplit);
            graphDrop.RemoveEdge(edgeForSplit);
            taskDrop = new Task<TVertex, TEdge>(graphDrop, weightsDrop, new BidirectionalGraph<TVertex, TEdge>(Path), MinCost, $"Drop{edgeForSplit}");

            return true;
        }
    }
}