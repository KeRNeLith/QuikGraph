using JetBrains.Annotations;

namespace QuikGraph.Algorithms.Services
{
    /// <summary> Provides common services available to algorithm instances. </summary>
    public interface IAlgorithmServices
    {
        /// <summary> Algorithm cancel manager. </summary>
        [NotNull]
        ICancelManager CancelManager { get; }
    }
}