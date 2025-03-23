using System;
using System.Data;
using JetBrains.Annotations;
using QuikGraph.Algorithms;

namespace QuikGraph.Data
{
    /// <summary>
    /// Algorithm that take a <see cref="T:System.Data.DataSet"/> and convert it as a graph representation.
    /// </summary>
    public sealed class DataSetGraphPopulatorAlgorithm : AlgorithmBase<IMutableVertexAndEdgeSet<DataTable, DataRelationEdge>>
    {
        /// <summary>
        /// <see cref="T:System.Data.DataSet"/> to represent as a graph.
        /// </summary>
        [NotNull]
        public DataSet DataSet { get; }

        /// <summary>
        /// Initializes a new <see cref="DataSetGraphPopulatorAlgorithm"/> class.
        /// </summary>
        /// <param name="visitedGraph">Graph to fill from <paramref name="dataSet"/>.</param>
        /// <param name="dataSet"><see cref="T:System.Data.DataSet"/> to use to fill <paramref name="visitedGraph"/>.</param>
        /// <exception cref="T:System.ArgumentNullException"><paramref name="visitedGraph"/> is <see langword="null"/>.</exception>
        /// <exception cref="T:System.ArgumentNullException"><paramref name="dataSet"/> is <see langword="null"/>.</exception>
        public DataSetGraphPopulatorAlgorithm(
            [NotNull] IMutableVertexAndEdgeSet<DataTable, DataRelationEdge> visitedGraph,
            [NotNull] DataSet dataSet)
            : base(visitedGraph)
        {
            DataSet = dataSet ?? throw new ArgumentNullException(nameof(dataSet));
        }

        #region AlgorithmBase<TGraph>

        /// <inheritdoc />
        protected override void InternalCompute()
        {
            foreach (DataTable table in DataSet.Tables)
            {
                VisitedGraph.AddVertex(table);
            }

            foreach (DataRelation relation in DataSet.Relations)
            {
                VisitedGraph.AddEdge(new DataRelationEdge(relation));
            }
        }

        #endregion
    }
}