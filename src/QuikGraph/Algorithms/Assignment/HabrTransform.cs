namespace QuikGraph.Algorithms.Assignment
{
    using System;
    using System.IO;
    using System.Linq;

    /// <inheritdoc cref="ModifyMatrixSquare(double[,])"/>
    public static class HabrTransform
    {
        /// <summary>In-Place Habr-Transform </summary>
        /// <remarks>
        /// The Habr Transform is a matrix transformation that modifies a matrix in-place,
        /// so that the sum of each row and each column is zero.
        /// </remarks>
        public static void ModifyMatrixSquare(this double[,] matrix)
        {
            int rows = matrix.GetLength(0);
            int cols = matrix.GetLength(1);

            double[] rowAverages = new double[rows];

            // Calculate row sums and overall sum
            for (int i = 0; i < rows; i++)
            {
                double rowSum = 0;
                for (int j = 0; j < cols; j++)
                {
                    rowSum += matrix[i, j];
                }
                rowAverages[i] = rowSum / cols;
            }

            // Calculate column averages
            double[] colAverages = new double[cols];
            for (int j = 0; j < cols; j++)
            {
                double colSum = 0;
                for (int i = 0; i < rows; i++)
                {
                    colSum += matrix[i, j];
                }
                colAverages[j] = colSum / rows;
            }

            // Calculate overall average
            double overallSum = rowAverages.Sum();
            double overallAverage = overallSum / rows;

            // Modify the matrix
            for (int i = 0; i < rows; i++)
            {
                double rowAverage = rowAverages[i] - overallAverage;
                for (int j = 0; j < cols; j++)
                {
                    matrix[i, j] -= colAverages[j] + rowAverage;
                }
            }
        }

        /// <inheritdoc cref="ModifyMatrixSquare(double[,])"/>
        public static void ModifyMatrix(this double[][] matrix)
        {
            int rows = matrix.Length;
            int cols = matrix[0].Length;

            double[] rowAverages = new double[rows];

            // Calculate row sums and overall sum
            for (int i = 0; i < rows; i++)
            {
                double[] matrix_i = matrix[i];
                double rowSum = 0;
                for (int j = 0; j < cols; j++)
                {
                    rowSum += matrix_i[j];
                }
                rowAverages[i] = rowSum / cols;
            }

            // Calculate column averages
            double[] colAverages = new double[cols];
            for (int j = 0; j < cols; j++)
            {
                double colSum = 0;
                for (int i = 0; i < rows; i++)
                {
                    colSum += matrix[i][j];
                }
                colAverages[j] = colSum / rows;
            }

            // Calculate overall average
            double overallSum = rowAverages.Sum();
            double overallAverage = overallSum / rows;

            // Modify the matrix
            for (int i = 0; i < rows; i++)
            {
                double[] matrix_i = matrix[i];
                double rowAverage = rowAverages[i] + overallAverage;
                for (int j = 0; j < cols; j++)
                {
                    matrix_i[j] -= colAverages[j] + rowAverage;
                }
            }
        }

        /// <summary> Print a 2D matrix to the console. </summary>
        public static void PrintMatrix(this double[,] matrix, Action<string> write = null)
        {
            write = write ?? Console.Write;
            int rows = matrix.GetLength(0);
            int cols = matrix.GetLength(1);

            for (int i = 0; i < rows; i++)
            {
                for (int j = 0; j < cols; j++)
                {
                    write($"{matrix[i, j]:0.00}\t");
                }
                write("\n");
            }
        }
    }
}