using System;
using JetBrains.Annotations;

namespace QuikGraph
{
    /// <summary>
    /// Delegate to perform a check on the given <paramref name="vertex"/>.
    /// </summary>
    /// <typeparam name="TVertex">Vertex type.</typeparam>
    /// <param name="vertex">Vertex to check condition.</param>
    /// <returns>True if the <paramref name="vertex"/> matches the predicate, false otherwise.</returns>
    [Obsolete("Rather use " + nameof(Func<TVertex, bool>))]
    public delegate bool VertexPredicate<in TVertex>([NotNull] TVertex vertex);
}