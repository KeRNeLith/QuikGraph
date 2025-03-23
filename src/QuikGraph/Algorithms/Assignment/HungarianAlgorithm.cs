using System;
using System.Collections.Generic;
using JetBrains.Annotations;

namespace QuikGraph.Algorithms.Assignment
{

    /// <summary> A combinatorial optimization algorithm that creates a BiPartite Graph/assignment to minimize the total Sum. </summary>
    /// <remarks>
    /// AKA Kuhn-Munkres-Algorithm.
    /// Finding, in a weighted bipartite graph, a matching
    /// in which the sum of weights of the edges is as large as possible.
    ///
    /// We have to find a Permutation P(N), i.e. assignment of the N jobs to the N workers,
    /// such that each job is assigned to one worker and each worker is assigned one job,
    /// such that the total cost of assignment is minimum.
    ///
    /// Brute Force requires to research N! Permutations. 
    ///
    /// The Algorithm modifies Columns or Rows in ways that
    /// don't change the Maximality, only the maximum Value:
    /// cost[W, J] + R[W] + C[J] has the same maximum Configuration,
    /// but a different maximum Value Tr(cost + R + C) = Tr(cost) + Sum(R) + Sum(C)
    /// because no matter which permutation, we always pick up all values of R and C:
    /// Tr(R[W] + C[J]) = Tr(R[W]) + Tr(C[J]) = Sum(R) + Sum(C)
    /// 
    /// The element cost[W, J] in the W-th row and J-th column represents the cost of
    /// assigning the J-th job to the W-th worker.
    /// 
    /// End-Condition: colsCoveredCount == height
    ///
    /// Übliche Beispiele sind
    /// - das Heiratsproblem möglichst viele Paare bei einer maximalen „Sympathiesumme“ zu finden.
    /// - das Auktionsmodell, bei dem ein maximaler Gesamtpreis zu erzielen.
    /// - das Jobproblem, worin Arbeitsaufträge auf Arbeiter oder Maschinen zu verteilen sind so dass die Kosten minimal sind.
    ///
    /// anders formuliert:
    /// Ordne die Zeilen- und Spaltenvektoren so um,
    /// dass TR(Cost) die Summe der Elemente in der Hauptdiagonale maximal oder minimal wird. 
    /// </remarks>
    public sealed class HungarianAlgorithm
    {
        /// <summary> Hungarian algorithm steps. </summary>
        /// <remarks>See https://en.wikipedia.org/wiki/Hungarian_algorithm </remarks>
        public enum Steps
        {
            /// <summary> Initialization step. </summary>
            Init,

            /// <summary> Step 1.</summary>
            Step1,

            /// <summary> Step 2.</summary>
            Step2,

            /// <summary> Step 3.</summary>
            Step3,

            /// <summary> Step 4.</summary>
            Step4,

            /// <summary> End step. </summary>
            End
        }

        /// <summary> cost[Worker, Job] </summary>
        [NotNull]
        private readonly int[,] _costOfWorkerForJob;

        private int _width;
        private int _height;

        private byte[,] _masks;
        private bool[] _rowsCovered;
        private bool[] _colsCovered;

        private Steps _step;

        /// <summary> Computed assignments, i.e. Agent indexed by the Task </summary>
        public int[] AgentByTaskNo { get; private set; }

        private Location _pathStart;
        private Location[] _path;

        /// <summary> Initializes a new <see cref="HungarianAlgorithm"/> class. </summary>
        /// <param name="costs">Costs matrix.</param>
        /// <exception cref="T:System.ArgumentNullException"><paramref name="costs"/> is <see langword="null"/>.</exception>
        /// <remarks>
        /// The Sum of all cost is to be minimized.
        /// </remarks>
        public HungarianAlgorithm([NotNull] int[,] costs)
        {
            _costOfWorkerForJob = costs ?? throw new ArgumentNullException(nameof(costs));
            _step = Steps.Init;
        }

        /// <summary> Returns assignments (without visualization). </summary>
        [NotNull]
        public int[] Compute()
        {
            while (DoStep() != Steps.End) ;
            return AgentByTaskNo;
        }

        /// <summary> Returns iterations that can be used to visualize the algorithm. </summary>
        /// <returns>An enumerable of algorithm iterations.</returns>
        [Pure]
        [NotNull]
        public IEnumerable<HungarianIteration> GetIterations()
        {
            for (Steps step = Steps.Init; step != Steps.End; )
            {
                step = DoStep();

                yield return new HungarianIteration(
                    (int[,])_costOfWorkerForJob.Clone(),
                    (byte[,])_masks.Clone(),
                    (bool[])_rowsCovered.Clone(),
                    (bool[])_colsCovered.Clone(),
                    step);
            }
        }

        private Steps DoStep()
        {
            if (_step == Steps.Init)
                return RunInitStep();

            if (_step != Steps.End)
                return ComputeStep(_step);

            UpdateAgentsTasks();

            return Steps.End;
        }

        private Steps ComputeStep(Steps step)
        {
            switch (step)
            {
                case Steps.Step1:
                {
                    _step = RunStep1(_masks, _colsCovered, _width, _height);
                    return step;
                }
                case Steps.Step2:
                {
                    _step = RunStep2(_costOfWorkerForJob, _masks, _rowsCovered, _colsCovered, _width, _height, ref _pathStart);
                    return step;
                }
                case Steps.Step3:
                {
                    _step = RunStep3(_masks, _rowsCovered, _colsCovered, _width, _height, _path, _pathStart);
                    return step;
                }
                case Steps.Step4:
                {
                    _step = RunStep4(_costOfWorkerForJob, _rowsCovered, _colsCovered, _width, _height);
                    return step;
                }
            }

            return Steps.End;
        }

        private void UpdateAgentsTasks()
        {
            AgentByTaskNo = new int[_height];

            for (int i = 0; i < _height; ++i)
            {
                for (int j = 0; j < _width; ++j)
                {
                    if (_masks[i, j] == 1)
                    {
                        AgentByTaskNo[i] = j;
                        break;
                    }
                }
            }
        }

        private void AssignJobsWith0Cost()
        {
            _masks = new byte[_height, _width];
            _rowsCovered = new bool[_height];
            _colsCovered = new bool[_width];

            for (int i = 0; i < _height; ++i)
            {
                for (int j = 0; j < _width; ++j)
                {
                    if (_costOfWorkerForJob[i, j] == 0 && !_rowsCovered[i] && !_colsCovered[j])
                    {
                        _masks[i, j] = 1;
                        _rowsCovered[i] = true;
                        _colsCovered[j] = true;
                    }
                }
            }
        }

        private Steps RunInitStep()
        {
            _width = _costOfWorkerForJob.GetLength(1);
            _height = _costOfWorkerForJob.GetLength(0);

            // Reduce by rows
            for (int i = 0; i < _height; ++i)
            {
                int min = int.MaxValue;
                for (int j = 0; j < _width; ++j)
                {
                    min = Math.Min(min, _costOfWorkerForJob[i, j]);
                }

                for (int j = 0; j < _width; ++j)
                {
                    _costOfWorkerForJob[i, j] -= min;
                }
            }

            // Set 1 where job assigned
            AssignJobsWith0Cost();

            Fill(_rowsCovered, _height);
            Fill(_colsCovered, _width);

            _path = new Location[_width * _height];
            _pathStart = default(Location);
            _step = Steps.Step1;

            return Steps.Init;
        }

        private static Steps RunStep1(
            [NotNull] byte[,] masks,
            [NotNull] bool[] colsCovered,
            int width,
            int height)
        {
            for (int i = 0; i < height; ++i)
            {
                for (int j = 0; j < width; ++j)
                {
                    if (masks[i, j] == 1)
                    {
                        colsCovered[j] = true;
                    }
                }
            }

            int colsCoveredCount = 0;
            for (int j = 0; j < width; ++j)
            {
                if (colsCovered[j])
                {
                    ++colsCoveredCount;
                }
            }

            return colsCoveredCount == height ? Steps.End : Steps.Step2;
        }

        private static Steps RunStep2(
            [NotNull] int[,] costs,
            [NotNull] byte[,] masks,
            [NotNull] bool[] rowsCovered,
            [NotNull] bool[] colsCovered,
            int width,
            int height,
            ref Location pathStart)
        {
            // Search for another assignment
            Location loc = FindZero(costs, rowsCovered, colsCovered, width, height);

            // If there is not another options we should change matrix
            if (loc.Row == -1)
                return Steps.Step4;

            masks[loc.Row, loc.Column] = 2;
            int starCol = FindStarInRow(masks, width, loc.Row);
            if (starCol != -1)
            {
                rowsCovered[loc.Row] = true;
                colsCovered[starCol] = false;
            }
            else
            {
                pathStart = loc;
                return Steps.Step3;
            }

            return Steps.Step2;
        }

        private static Steps RunStep3(
            [NotNull] byte[,] masks,
            [NotNull] bool[] rowsCovered,
            [NotNull] bool[] colsCovered,
            int width,
            int height,
            [NotNull] Location[] path,
            Location pathStart)
        {
            int pathIndex = 0;
            path[0] = pathStart;
            int row = FindStarInColumn(masks, height, path[pathIndex].Column);
            while (row != -1)
            {
                ++pathIndex;
                path[pathIndex] = new Location(row, path[pathIndex - 1].Column);
                int col = FindPrimeInRow(masks, width, path[pathIndex].Row);

                ++pathIndex;
                path[pathIndex] = new Location(path[pathIndex - 1].Row, col);
                row = FindStarInColumn(masks, height, path[pathIndex].Column);
            }

            ConvertPath(masks, path, pathIndex + 1);
            Fill(rowsCovered);
            Fill(colsCovered);
            ClearPrimes(masks, width, height);

            return Steps.Step1;
        }

        private static Steps RunStep4(
            [NotNull] int[,] costs,
            [NotNull] bool[] rowsCovered,
            [NotNull] bool[] colsCovered,
            int width,
            int height)
        {
            int minValue = FindMinimum(costs, rowsCovered, colsCovered, width, height);
            for (int i = 0; i < height; ++i)
            {
                for (int j = 0; j < width; ++j)
                {
                    if (rowsCovered[i])
                    {
                        costs[i, j] += minValue;
                    }

                    if (!colsCovered[j])
                    {
                        costs[i, j] -= minValue;
                    }
                }
            }

            return Steps.Step2;
        }

        private static void ConvertPath(
            [NotNull] byte[,] masks,
            [NotNull] Location[] path,
            int pathLength)
        {
            for (int i = 0; i < pathLength; ++i)
            {
                switch (masks[path[i].Row, path[i].Column])
                {
                    case 1:
                        masks[path[i].Row, path[i].Column] = 0;
                        break;
                    case 2:
                        masks[path[i].Row, path[i].Column] = 1;
                        break;
                }
            }
        }

        private static Location FindZero(
            [NotNull] int[,] costs,
            [NotNull] bool[] rowsCovered,
            [NotNull] bool[] colsCovered,
            int width,
            int height)
        {
            for (int i = 0; i < height; ++i)
            {
                for (int j = 0; j < width; ++j)
                {
                    if (costs[i, j] == 0
                        && !rowsCovered[i]
                        && !colsCovered[j])
                        return new Location(i, j);
                }
            }

            return Location.InvalidLocation;
        }

        private static int FindMinimum(
            [NotNull] int[,] costs,
            [NotNull] bool[] rowsCovered,
            [NotNull] bool[] colsCovered,
            int width,
            int height)
        {
            int minValue = int.MaxValue;
            for (int i = 0; i < height; ++i)
            {
                for (int j = 0; j < width; ++j)
                {
                    if (!rowsCovered[i] &&
                        !colsCovered[j])
                    {
                        minValue = Math.Min(minValue, costs[i, j]);
                    }
                }
            }

            return minValue;
        }

        private static int FindStarInRow(
            [NotNull] byte[,] masks,
            int width,
            int row)
        {
            for (int j = 0; j < width; ++j)
            {
                if (masks[row, j] == 1)
                    return j;
            }

            return -1;
        }

        private static int FindStarInColumn(
            [NotNull] byte[,] masks,
            int height,
            int column)
        {
            for (int i = 0; i < height; ++i)
            {
                if (masks[i, column] == 1)
                    return i;
            }

            return -1;
        }

        private static int FindPrimeInRow(
            [NotNull] byte[,] masks,
            int width,
            int row)
        {
            for (int j = 0; j < width; ++j)
            {
                if (masks[row, j] == 2)
                    return j;
            }

            return -1;
        }

        /// <summary> Fills the <paramref name="array"/> with the <paramref name="value"/> </summary>
        public static void Fill<T>([NotNull] T[] array, int? stop = null, T value = default(T), int start = 0)
        {
            for (int i = stop ?? array.Length; --i >= start;)
            {
                array[i] = value;
            }
        }

        private static void ClearPrimes(
            [NotNull] byte[,] masks,
            int width,
            int height)
        {
            for (int i = 0; i < height; ++i)
            {
                for (int j = 0; j < width; ++j)
                {
                    if (masks[i, j] == 2)
                    {
                        masks[i, j] = 0;
                    }
                }
            }
        }

        ///<summary>
        /// Represents coordinates: raw and column number.
        /// </summary>
        private struct Location
        {
            public static readonly Location InvalidLocation = new Location(-1, -1);

            public int Row { get; }
            public int Column { get; }

            public Location(int row, int col)
            {
                Row = row;
                Column = col;
            }
        }
    }
}