using System;
#if SUPPORTS_SERIALIZATION
using System.Runtime.Serialization;
#endif
using JetBrains.Annotations;

namespace QuikGraph
{
    /// <summary>
    /// Exception raised when an algorithm detected a cyclic graph when required acyclic.
    /// </summary>
#if SUPPORTS_SERIALIZATION
    [Serializable]
#endif
    public class CyclicGraphException : QuikGraphException
    {
        /// <summary>
        /// Initializes a new instance of <see cref="CyclicGraphException"/> class.
        /// </summary>
        public CyclicGraphException()
            : base("The graph contains at least one cycle.")
        {
        }

        /// <summary>
        /// Initializes a new instance of <see cref="CyclicGraphException"/> class.
        /// </summary>
        public CyclicGraphException([NotNull] string message, [CanBeNull] Exception innerException = null)
            : base(message, innerException)
        {
        }

#if SUPPORTS_SERIALIZATION
        /// <summary>
        /// Constructor used during runtime serialization.
        /// </summary>
        protected CyclicGraphException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
#endif
    }
}