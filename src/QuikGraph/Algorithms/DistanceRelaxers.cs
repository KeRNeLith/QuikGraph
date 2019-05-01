﻿namespace QuikGraph.Algorithms
{
    /// <summary>
    /// Implementation of several distance relaxers.
    /// </summary>
    public static class DistanceRelaxers
    {
        /// <summary>
        /// Shortest distance relaxer.
        /// </summary>
        public static readonly IDistanceRelaxer ShortestDistance = new ShortestDistanceRelaxer();

        private sealed class ShortestDistanceRelaxer : IDistanceRelaxer
        {
            /// <inheritdoc />
            public double InitialDistance => double.MaxValue;

            /// <inheritdoc />
            public int Compare(double a, double b)
            {
                return a.CompareTo(b);
            }

            /// <inheritdoc />
            public double Combine(double distance, double weight)
            {
                return distance + weight;
            }
        }

        /// <summary>
        /// Critical distance relaxer.
        /// </summary>
        public static readonly IDistanceRelaxer CriticalDistance = new CriticalDistanceRelaxer();

        private sealed class CriticalDistanceRelaxer : IDistanceRelaxer
        {
            /// <inheritdoc />
            public double InitialDistance => double.MinValue;

            /// <inheritdoc />
            public int Compare(double a, double b)
            {
                return -a.CompareTo(b);
            }

            /// <inheritdoc />
            public double Combine(double distance, double weight)
            {
                return distance + weight;
            }
        }

        /// <summary>
        /// Edge shortest distance relaxer.
        /// </summary>
        public static readonly IDistanceRelaxer EdgeShortestDistance = new EdgeDistanceRelaxer();

        private sealed class EdgeDistanceRelaxer : IDistanceRelaxer
        {
            /// <inheritdoc />
            public double InitialDistance => 0;

            /// <inheritdoc />
            public int Compare(double a, double b)
            {
                return a.CompareTo(b);
            }

            /// <inheritdoc />
            public double Combine(double distance, double weight)
            {
                return distance + weight;
            }
        }
    }
}
