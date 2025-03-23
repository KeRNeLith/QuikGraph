namespace QuikGraph.Tests.Algorithms
{
    /// <summary> AKA KeyValuePair;
    /// Represents a pair source and target vertex Indices.
    /// </summary>
    internal readonly struct VerticesPair
    {
        public VerticesPair(int source, int target)
        {
            Source = source;
            Target = target;
        }

        /// <summary>
        /// Source vertex.
        /// </summary>
        public int Source { get; }

        /// <summary>
        /// Target vertex.
        /// </summary>
        public int Target { get; }
    }
}