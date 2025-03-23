using System;
using JetBrains.Annotations;

namespace QuikGraph.Algorithms
{
    /// <summary> <see cref="Compute"/>s of something with control <see cref="State"/>. </summary>
    /// <remarks>
    /// <see cref="Compute"/> does not run automatically in the Constructor,
    /// so you have a chance to attach <see cref="EventHandler"/>s before the computation starts!
    /// </remarks>
    public interface IComputation
    {
        /// <summary> Synchronizer object usable for managing concurrent Operations. </summary>
        [NotNull]
        object SyncRoot { get; }

        /// <inheritdoc cref="ComputationState"/>
        ComputationState State { get; }

        /// <summary> Runs the computation. </summary>
        void Compute();

        /// <summary> Abort the computation. </summary>
        void Abort();

        /// <summary> Event that fires when the computation state changed. </summary>
        event EventHandler StateChanged;

        /// <summary> Event that fires when the computation starts. </summary>
        event EventHandler Started;

        /// <summary> Event that fires when the computation is finished. </summary>
        event EventHandler Finished;

        /// <summary> Event that fires when the computation is aborted. </summary>
        event EventHandler Aborted;
    }
}