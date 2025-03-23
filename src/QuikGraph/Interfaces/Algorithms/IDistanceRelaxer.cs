using System.Collections.Generic;
using JetBrains.Annotations;

namespace QuikGraph.Algorithms
{
    /// <summary> Represents a distance relaxer, which can aggregate the Distance along a Path. </summary>
    /// <remarks>
    /// During the relaxation process for an edge (u, v) with weight w,
    /// the algorithm checks the current shortest known distance to vertex v
    /// and determines if it can be minimized by taking the path through vertex u.
    ///
    /// Mathematically, it updates the distance d[v] as follows:
    /// if d[v] > d[u]+w
    ///    d[v] = d[u]+w;
    /// </remarks>
    public interface IDistanceRelaxer : IComparer<double>
    {
        /// <summary>
        /// Initial distance.
        /// </summary>
        double InitialDistance { get; }

        /// <summary> Updates the <paramref name="distance"/>, typically by adding the <paramref name="weight"/>. </summary>
        [Pure]
        double Combine(double distance, double weight);
    }
}