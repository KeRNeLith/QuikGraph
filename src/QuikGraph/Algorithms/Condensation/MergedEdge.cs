﻿#if SUPPORTS_SERIALIZATION
using System;
#endif
using System.Collections.Generic;

namespace QuickGraph.Algorithms.Condensation
{
#if SUPPORTS_SERIALIZATION
    [Serializable]
#endif
    public sealed class MergedEdge<TVertex, TEdge> : Edge<TVertex>
        where TEdge : IEdge<TVertex>
    {
        private List<TEdge> edges = new List<TEdge>();

        public MergedEdge(TVertex source, TVertex target)
            :base(source,target)
        { }

        public IList<TEdge> Edges
        {
            get { return this.edges; }
        }

        public static MergedEdge<TVertex, TEdge> Merge(
            MergedEdge<TVertex, TEdge> inEdge,
            MergedEdge<TVertex, TEdge> outEdge
            )
        {
            MergedEdge<TVertex, TEdge> newEdge = new MergedEdge<TVertex, TEdge>(
                inEdge.Source, outEdge.Target);
            newEdge.edges = new List<TEdge>(inEdge.Edges.Count + outEdge.Edges.Count);
            newEdge.edges.AddRange(inEdge.Edges);
            newEdge.edges.AddRange(outEdge.edges);

            return newEdge;
        }
    }
}
