using JetBrains.Annotations;

namespace QuikGraph.Algorithms.Services
{
    /// <summary> Allows to <see cref="GetService{T}"/>s that an algorithm needs. </summary>
    public interface IAlgorithmComponent
    {
        /// <inheritdoc cref="IAlgorithmServices"/>
        [NotNull]
        IAlgorithmServices Services { get; }

        /// <summary> Gets the service with given <typeparamref name="T"/>ype. </summary>
        /// <typeparam name="T">Service type.</typeparam>
        /// <exception cref="T:System.InvalidOperationException">Requested service is not present on algorithm.</exception>
        [Pure]
        [CanBeNull]
        T GetService<T>();

        /// <summary> Tries to get the service with given <typeparamref name="T"/>. </summary>
        /// <typeparam name="T">Service type.</typeparam>
        /// <param name="service">Found service.</param>
        /// <returns>True if the service was found, false otherwise.</returns>
        [Pure]
        [ContractAnnotation("=> true, service:notnull;=> false, service:null")]
        bool TryGetService<T>(out T service);
    }
}