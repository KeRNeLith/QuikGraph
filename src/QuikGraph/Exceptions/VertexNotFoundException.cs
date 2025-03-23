using System;
#if SUPPORTS_SERIALIZATION
using System.Runtime.Serialization;
#endif
using JetBrains.Annotations;

namespace QuikGraph
{
    /// <summary> Exception raised when trying to use a vertex that is not inside the manipulated graph. </summary>
    /// <remarks>
    /// Exceptions are expensive.
    /// Provide for a regular way to detect this like TryGet or returning a special Value,
    /// but only if the compiler supports detecting this. 
    /// </remarks>
#if SUPPORTS_SERIALIZATION
    [Serializable]
#endif
    //[Obsolete("Rather return null")]
    public class VertexNotFoundException : QuikGraphException
    {
        /// <summary>
        /// Initializes a new instance of <see cref="VertexNotFoundException"/> class.
        /// </summary>
        public VertexNotFoundException()
            : base("Vertex is not present in the graph.")
        {
        }

        /// <summary>
        /// Initializes a new instance of <see cref="VertexNotFoundException"/> class.
        /// </summary>
        public VertexNotFoundException([NotNull] string message, [CanBeNull] Exception innerException = null)
            : base(message, innerException)
        {
        }

#if SUPPORTS_SERIALIZATION
        /// <summary>
        /// Constructor used during runtime serialization.
        /// </summary>
        protected VertexNotFoundException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
#endif
    }
}