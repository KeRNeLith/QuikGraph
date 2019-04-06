﻿#if SUPPORTS_SERIALIZATION
using System;
#endif
using System.Collections.Generic;
#if SUPPORTS_CONTRACTS
using System.Diagnostics.Contracts;
#endif
using QuickGraph.Algorithms.Services;

namespace QuickGraph.Algorithms.MaximumFlow
{
    /// <summary>
    /// Routines to add and remove auxiliary edges when using <see cref="EdmondsKarpMaximumFlowAlgorithm{TVertex, TEdge}"/> 
    /// or <see cref="MaximumBipartiteMatchingAlgorithm{TVertex, TEdge}.InternalCompute()"/>. 
    /// Remember to call <see cref="RemoveReversedEdges()"/> to remove auxiliary edges.
    /// </summary>
    /// <typeparam name="TVertex">The type of vertex.</typeparam>
    /// <typeparam name="TEdge">The type of edge.</typeparam>
    /// <remarks>
    /// Will throw an exception in <see cref="ReversedEdgeAugmentorAlgorithm{TVertex, TEdge}.AddReversedEdges"/> if TEdge is a value type,
    /// e.g. <see cref="SEdge{TVertex}"/>.
    /// <seealso href="https://github.com/YaccConstructor/QuickGraph/issues/183#issue-377613647"/>.
    /// </remarks>
#if SUPPORTS_SERIALIZATION
    [Serializable]
#endif
    public sealed class ReversedEdgeAugmentorAlgorithm<TVertex, TEdge>
        : IDisposable
        where TEdge : IEdge<TVertex>
    {
        private readonly IMutableVertexAndEdgeListGraph<TVertex,TEdge> visitedGraph;
        private readonly EdgeFactory<TVertex, TEdge> edgeFactory;
        private IList<TEdge> augmentedEgdes = new List<TEdge>();
        private Dictionary<TEdge,TEdge> reversedEdges = new Dictionary<TEdge,TEdge>();
        private bool augmented = false;

        public ReversedEdgeAugmentorAlgorithm(
            IMutableVertexAndEdgeListGraph<TVertex, TEdge> visitedGraph,
            EdgeFactory<TVertex, TEdge> edgeFactory)
            : this(null, visitedGraph, edgeFactory)
        { }

        public ReversedEdgeAugmentorAlgorithm(
            IAlgorithmComponent host,
            IMutableVertexAndEdgeListGraph<TVertex,TEdge> visitedGraph,
            EdgeFactory<TVertex,TEdge> edgeFactory)
        {
#if SUPPORTS_CONTRACTS
            Contract.Requires(visitedGraph != null);
            Contract.Requires(edgeFactory != null);
#endif

            this.visitedGraph = visitedGraph;
            this.edgeFactory = edgeFactory;
        }

        public IMutableVertexAndEdgeListGraph<TVertex,TEdge> VisitedGraph
        {
            get
            {
                return this.visitedGraph;
            }
        }

        public EdgeFactory<TVertex, TEdge> EdgeFactory
        {
            get { return this.edgeFactory; }
        }

        public ICollection<TEdge> AugmentedEdges
        {
            get
            {
                return this.augmentedEgdes;
            }
        }

        public Dictionary<TEdge,TEdge> ReversedEdges
        {
            get
            {
                return this.reversedEdges;
            }
        }

        public bool Augmented
        {
            get
            {
                return this.augmented;
            }
        }

        public event EdgeAction<TVertex,TEdge> ReversedEdgeAdded;
        private void OnReservedEdgeAdded(TEdge e)
        {
            var eh = this.ReversedEdgeAdded;
            if (eh != null)
                eh(e);
        }

        /// <summary>
        /// Adds auxiliary edges to <see cref="VisitedGraph"/> to store residual flows.
        /// </summary>
        /// <remarks>
        /// Will throw an exception if TEdge is a value type, e.g. <see cref="SEdge{TVertex}"/>.
        /// <seealso href="https://github.com/YaccConstructor/QuickGraph/issues/183#issue-377613647"/>.
        /// </remarks>
        public void AddReversedEdges()
        {
            if (this.Augmented)
                throw new InvalidOperationException("Graph already augmented");
            // step 1, find edges that need reversing
            IList<TEdge> notReversedEdges = new List<TEdge>();
            foreach (var edge in this.VisitedGraph.Edges)
            {
                // if reversed already found, continue
                if (this.reversedEdges.ContainsKey(edge))
                    continue;

                TEdge reversedEdge = this.FindReversedEdge(edge);
                if (reversedEdge != null)
                {
                    // setup edge
                    this.reversedEdges[edge] = reversedEdge;
                    // setup reversed if needed
                    if (!this.reversedEdges.ContainsKey(reversedEdge))
                        this.reversedEdges[reversedEdge] = edge;
                    continue;
                }

                // this edge has no reverse
                notReversedEdges.Add(edge);
            }

            // step 2, go over each not reversed edge, add reverse
            foreach (var edge in notReversedEdges)
            {
                if (this.reversedEdges.ContainsKey(edge))
                    continue;

                // already been added
                TEdge reversedEdge = this.FindReversedEdge(edge);
                if (reversedEdge != null)
                {
                    this.reversedEdges[edge] = reversedEdge;
                    continue;
                }

                // need to create one
                reversedEdge = this.edgeFactory(edge.Target, edge.Source);
                if (!this.VisitedGraph.AddEdge(reversedEdge))
                    throw new InvalidOperationException("We should not be here");
                this.augmentedEgdes.Add(reversedEdge);
                this.reversedEdges[edge] = reversedEdge;
                this.reversedEdges[reversedEdge] = edge;
                this.OnReservedEdgeAdded(reversedEdge);
            }

            this.augmented = true;
        }

        public void RemoveReversedEdges()
        {
            if (!this.Augmented)
                throw new InvalidOperationException("Graph is not yet augmented");

            foreach (var edge in this.augmentedEgdes)
                this.VisitedGraph.RemoveEdge(edge);

            this.augmentedEgdes.Clear();
            this.reversedEdges.Clear();

            this.augmented = false;
        }

        private TEdge FindReversedEdge(TEdge edge)
        {
            foreach (var redge in this.VisitedGraph.OutEdges(edge.Target))
                if (redge.Target.Equals(edge.Source))
                    return redge;
            return default(TEdge);
        }

        void IDisposable.Dispose()
        {
            if(this.Augmented)
                this.RemoveReversedEdges();
        }
    }
}
