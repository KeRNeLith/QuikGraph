#if SUPPORTS_SERIALIZATION
using System;
#endif

namespace QuikGraph.Algorithms
{
    /// <summary> Current computation state of a <seealso cref="IComputation"/>. </summary>
#if SUPPORTS_SERIALIZATION
    [Serializable]
#endif
    public enum ComputationState : byte
    {
        /// <summary> The algorithm is not running. </summary>
        NotRunning,

        /// <summary> The algorithm is running. </summary>
        Running,

        /// <summary> An abort has been requested. </summary>
        /// <remarks>
        /// The algorithm is still running and will cancel as soon as it checks the cancellation state.
        /// </remarks>
        PendingAbortion,
        
        /// <summary> The computation is finished successfully. </summary>
        Finished,
        
        /// <summary> The computation was aborted. </summary>
        Aborted
    }
}