using System.Data;
using System.Diagnostics;
using System.Text;
using JetBrains.Annotations;
using QuikGraph.Graphviz;
using QuikGraph.Graphviz.Dot;

namespace QuikGraph.Data
{
    /// <summary>
    /// An algorithm that renders a DataSet graph to the Graphviz DOT format.
    /// </summary>
    public class DataSetGraphvizAlgorithm : GraphvizAlgorithm<DataTable, DataRelationEdge>
    {
        /// <summary>
        /// Initializes a new <see cref="DataSetGraphvizAlgorithm"/> class.
        /// </summary>
        /// <param name="visitedGraph">Graph to convert to DOT.</param>
        /// <exception cref="T:System.ArgumentNullException"><paramref name="visitedGraph"/> is <see langword="null"/>.</exception>
        public DataSetGraphvizAlgorithm([NotNull] DataSetGraph visitedGraph)
            : base(visitedGraph)
        {
            InitializeFormat();
        }

        /// <summary>
        /// Initializes a new <see cref="DataSetGraphvizAlgorithm"/> class.
        /// </summary>
        /// <param name="visitedGraph">Graph to convert to DOT.</param>
        /// <param name="imageType">Target output image type.</param>
        /// <exception cref="T:System.ArgumentNullException"><paramref name="visitedGraph"/> is <see langword="null"/>.</exception>
        public DataSetGraphvizAlgorithm(
            [NotNull] DataSetGraph visitedGraph,
            GraphvizImageType imageType)
            : base(visitedGraph, imageType)
        {
            InitializeFormat();
        }

        private void InitializeFormat()
        {
            FormatVertex += FormatTable;
            FormatEdge += FormatRelation;

            CommonVertexFormat.Style = GraphvizVertexStyle.Solid;
            CommonVertexFormat.Shape = GraphvizVertexShape.Record;
        }

        /// <summary>
        /// Formats a <see cref="T:System.Data.DataTable"/> (a table).
        /// </summary>
        /// <param name="sender">The <see cref="GraphvizAlgorithm{TVertex,TEdge}"/> performing the formatting.</param>
        /// <param name="args">Vertex event arguments.</param>
        protected virtual void FormatTable([NotNull] object sender, [NotNull] FormatVertexEventArgs<DataTable> args)
        {
            Debug.Assert(sender != null);
            Debug.Assert(args != null);

            DataTable vertex = args.Vertex;
            GraphvizVertex format = args.VertexFormat;
            format.Shape = GraphvizVertexShape.Record;

            // Create a record with a title and a list of columns
            var title = new GraphvizRecordCell
            {
                Text = vertex.TableName
            };

            var builder = new StringBuilder();
            bool flag = true;
            foreach (DataColumn column in vertex.Columns)
            {
                if (flag)
                {
                    flag = false;
                }
                else
                {
                    builder.AppendLine();
                }

                builder.Append($"+ {column.ColumnName} : {column.DataType.Name}");
                if (column.Unique)
                {
                    builder.Append(" unique");
                }
            }
            var columns = new GraphvizRecordCell
            {
                Text = builder.ToString().TrimEnd()
            };

            format.Record.Cells.Add(title);
            format.Record.Cells.Add(columns);
        }

        /// <summary>
        /// Formats a <see cref="DataRelationEdge"/> (a relation between tables).
        /// </summary>
        /// <param name="sender">The <see cref="GraphvizAlgorithm{TVertex,TEdge}"/> performing the formatting.</param>
        /// <param name="args">Edge event arguments.</param>
        protected virtual void FormatRelation([NotNull] object sender, [NotNull] FormatEdgeEventArgs<DataTable, DataRelationEdge> args)
        {
            Debug.Assert(sender != null);
            Debug.Assert(args != null);

            GraphvizEdge format = args.EdgeFormat;
            format.Label.Value = args.Edge.Relation.RelationName;
        }
    }
}