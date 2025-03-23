#if SUPPORTS_SERIALIZATION || SUPPORTS_CLONEABLE
using System;
#endif
using System.Collections.Generic;
using JetBrains.Annotations;
#if SUPPORTS_SERIALIZATION
using System.Runtime.Serialization;
#endif

namespace QuikGraph.Collections
{
    /// <summary> A cloneable dictionary of vertices associated to their edge-List. </summary>
    public interface IVertexEdgeDictionary<TVertex, TEdge> : IDictionary<TVertex, IEdgeList<TEdge>>
#if SUPPORTS_CLONEABLE
        , ICloneable
#endif
#if SUPPORTS_SERIALIZATION
        , ISerializable
#endif
     where TEdge : IEdge<TVertex>
    {
        /// <summary> Returns a clone of this dictionary. The vertices and edges are not cloned. </summary>
        [Pure]
        [NotNull]
#if SUPPORTS_CLONEABLE
        new
#endif
        IVertexEdgeDictionary<TVertex, TEdge> Clone();
    }
}