using JetBrains.Annotations;

namespace QuikGraph.Algorithms
{
    /// <summary> An algorithm that puts colors on vertices and allow to get that color. </summary>
    public interface IVertexColorizerAlgorithm<in TVertex>
    {
        /// <summary>Gets the <see cref="GraphColor"/> associated to the given <paramref name="vertex"/>. </summary>
        /// <param name="vertex">The vertex.</param>
        /// <returns>The vertex <see cref="GraphColor"/>.</returns>
        /// <exception cref="T:System.ArgumentNullException"><paramref name="vertex"/> is <see langword="null"/>.</exception>
        /// <exception cref="VertexNotFoundException"><paramref name="vertex"/> has no associated color.</exception>
        [Pure]
        GraphColor? GetVertexColor([NotNull] TVertex vertex);
    }
}