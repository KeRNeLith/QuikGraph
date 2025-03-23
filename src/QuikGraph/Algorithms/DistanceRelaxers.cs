using JetBrains.Annotations;

namespace QuikGraph.Algorithms
{
    /// <summary> Different Singleton Implementations of <see cref="IDistanceRelaxer"/>. </summary>
    public static class DistanceRelaxers
    {
        /// <summary> Shortest <see cref="IDistanceRelaxer"/>. </summary>
        [NotNull]
        public static readonly IDistanceRelaxer ShortestDistance = new ShortestDistanceRelaxer();

        /// <inheritdoc cref="ShortestDistance"/>
        private sealed class ShortestDistanceRelaxer : IDistanceRelaxer
        {
            /// <inheritdoc />
            public double InitialDistance => double.MaxValue;

            /// <inheritdoc />
            public int Compare(double x, double y) => x.CompareTo(y);

            /// <inheritdoc />
            public double Combine(double distance, double weight) => distance + weight;
        }

        /// <summary> Critical <see cref="IDistanceRelaxer"/>. </summary>
        [NotNull]
        public static readonly IDistanceRelaxer CriticalDistance = new CriticalDistanceRelaxer();

        /// <inheritdoc cref="CriticalDistance"/>
        private sealed class CriticalDistanceRelaxer : IDistanceRelaxer
        {
            /// <inheritdoc />
            public double InitialDistance => double.MinValue;

            /// <inheritdoc />
            public int Compare(double x, double y) => -x.CompareTo(y);

            /// <inheritdoc />
            public double Combine(double distance, double weight) => distance + weight;
        }

        /// <summary> Edge shortest <see cref="IDistanceRelaxer"/>. </summary>
        [NotNull]
        public static readonly IDistanceRelaxer EdgeShortestDistance = new EdgeDistanceRelaxer();

        /// <inheritdoc cref="EdgeShortestDistance"/>
        private sealed class EdgeDistanceRelaxer : IDistanceRelaxer
        {
            /// <inheritdoc />
            public double InitialDistance => 0;

            /// <inheritdoc />
            public int Compare(double x, double y) => x.CompareTo(y);

            /// <inheritdoc />
            public double Combine(double distance, double weight) => distance + weight;
        }

        /// <summary> Prim <see cref="IDistanceRelaxer"/>. </summary>
        [NotNull]
        public static readonly IDistanceRelaxer Prim = new PrimRelaxer();

        /// <inheritdoc cref="Prim"/>
        private sealed class PrimRelaxer : IDistanceRelaxer
        {
            public double InitialDistance => double.MaxValue;

            public int Compare(double x, double y) => x.CompareTo(y);

            public double Combine(double distance, double weight) => weight;
        }
    }
}