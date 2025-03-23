using System;
using System.Collections.Generic;
using JetBrains.Annotations;

namespace QuikGraph.Predicates
{
    /// <summary>
    /// Predicate that tests if an edge is residual.
    /// </summary>
    /// <typeparam name="TEdge">Edge type.</typeparam>
    public sealed class ResidualEdgePredicate<TEdge>
    {
        /// <summary>
        /// Initializes a new <see cref="ResidualEdgePredicate{TEdge}"/> class.
        /// </summary>
        /// <param name="residualCapacities">Residual capacities per edge.</param>
        /// <exception cref="T:System.ArgumentNullException"><paramref name="residualCapacities"/> is <see langword="null"/>.</exception>
        public ResidualEdgePredicate([NotNull] IDictionary<TEdge, double> residualCapacities)
        {
            ResidualCapacities = residualCapacities ?? throw new ArgumentNullException(nameof(residualCapacities));
        }

        /// <summary>
        /// Residual capacities map.
        /// </summary>
        [NotNull]
        public IDictionary<TEdge, double> ResidualCapacities { get; }

        /// <summary>
        /// Checks if the given <paramref name="edge"/> is residual.
        /// </summary>
        /// <remarks>Check if the implemented predicate is matched.</remarks>
        /// <param name="edge">Edge to use in predicate.</param>
        /// <returns>True if the edge is residual, false otherwise.</returns>
        /// <exception cref="T:System.ArgumentNullException"><paramref name="edge"/> is <see langword="null"/>.</exception>
        [Pure]
        public bool Test([NotNull] TEdge edge)
        {
            if (edge == null)
                throw new ArgumentNullException(nameof(edge));
            return 0 < ResidualCapacities[edge];
        }
    }
}