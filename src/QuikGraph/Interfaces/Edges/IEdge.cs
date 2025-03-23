using JetBrains.Annotations;

namespace QuikGraph
{
    /// <summary> Represents a directed edge. </summary>
    public interface IEdge<out TVertex> //: IEquatable<IEdge<TVertex>>
    {
        /// <summary> Gets the source vertex. </summary>
        [NotNull]
        TVertex Source { get; }

        /// <summary> Gets the target vertex. </summary>
        [NotNull]
        TVertex Target { get; }
    }
}