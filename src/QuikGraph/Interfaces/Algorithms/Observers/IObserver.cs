using System;
using JetBrains.Annotations;

namespace QuikGraph.Algorithms.Observers
{
    /// <inheritdoc cref="Attach"/>
    /// <reference-ref id="gof02designpatterns" />
    public interface IObserver<in TAlgorithm>
    {
        /// <summary> Attaches to the appropriate events of the <paramref name="algorithm"/> </summary>
        /// <returns> a <see cref="T:System.IDisposable"/> to detach this <see cref="IObserver{TAlgorithm}"/> from registered events.</returns>
        /// <remarks>
        /// Usually constructs an object from the events.
        /// </remarks>
        [NotNull]
        IDisposable Attach([NotNull] TAlgorithm algorithm);
    }
}