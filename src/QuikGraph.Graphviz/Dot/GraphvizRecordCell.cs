using System;
using System.Text;
using JetBrains.Annotations;
using static QuikGraph.Graphviz.DotEscapers;

namespace QuikGraph.Graphviz.Dot
{
    /// <summary>
    /// Graphviz record cell.
    /// </summary>
    public class GraphvizRecordCell
    {
        [NotNull, ItemNotNull]
        private GraphvizRecordCellCollection _cells = new GraphvizRecordCellCollection();

        /// <summary>
        /// Record cells.
        /// </summary>
        /// <exception cref="T:System.ArgumentNullException">Set value is <see langword="null"/>.</exception>
        [NotNull, ItemNotNull]
        public GraphvizRecordCellCollection Cells
        {
            get => _cells;
            set => _cells = value ?? throw new ArgumentNullException(nameof(value));
        }

        /// <summary>
        /// Indicates if record has port.
        /// </summary>
        public bool HasPort => !string.IsNullOrEmpty(Port);

        /// <summary>
        /// Port.
        /// </summary>
        public string Port { get; set; }

        /// <summary>
        /// Indicates if record has text.
        /// </summary>
        public bool HasText => !string.IsNullOrEmpty(Text);

        /// <summary>
        /// Text.
        /// </summary>
        public string Text { get; set; }

        /// <summary>
        /// Converts this record cell to DOT.
        /// </summary>
        /// <returns>Record cell as DOT.</returns>
        [Pure]
        [NotNull]
        public string ToDot()
        {
            var builder = new StringBuilder();

            if (HasPort)
            {
                builder.Append($"<{EscapePort(Port)}> ");
            }

            if (HasText)
            {
                builder.Append(EscapeRecord(Text));
            }

            if (Cells.Count > 0)
            {
                // Case when using a cell with both pattern at the same time
                // field = fieldId AND '{' rlabel '}'
                // with fieldId = <Port> Text
                // where rlabel = field ( '|' field )
                if (HasPort || HasText)
                {
                    builder.Append(" | ");
                }

                builder.Append("{ ");
                bool flag = false;
                foreach (GraphvizRecordCell cell in Cells)
                {
                    if (flag)
                    {
                        builder.Append($" | {cell.ToDot()}");
                        continue;
                    }
                    builder.Append(cell.ToDot());
                    flag = true;
                }
                builder.Append(" }");
            }

            return builder.ToString();
        }

        /// <inheritdoc />
        public override string ToString()
        {
            return ToDot();
        }
    }
}