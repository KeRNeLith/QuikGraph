using System.Diagnostics;
using JetBrains.Annotations;

namespace QuikGraph.Algorithms.Assignment
{
    /// <summary>
    /// State of an iteration of the Hungarian algorithm.
    /// </summary>
    public struct HungarianIteration
    {
        /// <summary>
        /// Costs matrix.
        /// </summary>
        [NotNull]
        public int[,] Matrix { get; }

        /// <summary>
        /// Matrix mask.
        /// </summary>
        [NotNull]
        public byte[,] Mask { get; }

        /// <summary>
        /// Array of treated rows.
        /// </summary>
        [NotNull]
        public bool[] RowsCovered { get; }

        /// <summary>
        /// Array of treated columns.
        /// </summary>
        [NotNull]
        public bool[] ColumnsCovered { get; }

        /// <summary>
        /// <see cref="HungarianAlgorithm.Steps"/> corresponding to this iteration.
        /// </summary>
        public HungarianAlgorithm.Steps Step { get; }

        /// <summary>
        /// Initializes a new <see cref="HungarianIteration"/> struct.
        /// </summary>
        internal HungarianIteration(
            [NotNull] int[,] costs,
            [NotNull] byte[,] mask,
            [NotNull] bool[] rowsCovered,
            [NotNull] bool[] columnsCovered, 
            HungarianAlgorithm.Steps step)
        {
            Debug.Assert(costs != null);
            Debug.Assert(mask != null);
            Debug.Assert(rowsCovered != null);
            Debug.Assert(columnsCovered != null);

            Matrix = costs;
            Mask = mask;
            RowsCovered = rowsCovered;
            ColumnsCovered = columnsCovered;
            Step = step;
        }
    }
}