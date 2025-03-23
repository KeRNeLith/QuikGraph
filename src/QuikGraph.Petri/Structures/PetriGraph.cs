#if SUPPORTS_SERIALIZATION
using System;
#endif

namespace QuikGraph.Petri
{
#if SUPPORTS_SERIALIZATION
    [Serializable]
#endif
    internal sealed class PetriGraph<TToken> : BidirectionalGraph<IPetriVertex, IArc<TToken>>, IPetriGraph<TToken>
    {
        /// <summary>
        /// Initializes a new <see cref="PetriGraph{TToken}"/> class.
        /// </summary>
        public PetriGraph()
            : base(true)
        {
        }
    }
}