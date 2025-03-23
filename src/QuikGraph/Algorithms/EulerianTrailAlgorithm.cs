using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using JetBrains.Annotations;
using QuikGraph.Algorithms.Observers;
using QuikGraph.Algorithms.Search;
using QuikGraph.Algorithms.Services;

namespace QuikGraph.Algorithms
{
    /// <summary> Extension Methods for <see cref="IVertexAndEdgeListGraph{TVertex,TEdge}"/> </summary>
    public static class VertexAndEdgeListGraphX
    {
        /// <summary> Returns the number of Eulerian trails in the <paramref name="graph"/>>. </summary>
        /// <remarks>This is the core of Euler's Argument:
        /// The necessary and sufficient condition for a walk over all Edges is that
        /// 1. the graph is connected and
        /// 2. each node has an even degree, (then this is an Euler cycle and returns 1)
        /// 3. except for a pair of nodes with an odd degree, which form the Start and End of the Path.
        ///
        /// All Eulerian circuits are also Eulerian paths, but not all Eulerian paths are Eulerian circuits.
        /// When there is an odd number of nodes with an odd degree,
        /// there can neither be an Euler Path nor Cycle. 
        /// </remarks>
        /// <exception cref="T:System.ArgumentNullException"><paramref name="graph"/> is <see langword="null"/>.</exception>
        [Pure]
        public static int ComputeEulerianPathCount<TVertex, TEdge>(
            [NotNull] this IVertexAndEdgeListGraph<TVertex, TEdge> graph) where TEdge : IEdge<TVertex>
        {
            if (graph is null)
                throw new ArgumentNullException(nameof(graph));

            if (graph.EdgeCount < graph.VertexCount)
                return 0;

            int odd = graph.OddVertices().Count();
            if (odd == 0)
                return 1;
            if (odd % 2 != 0)
                return 0;
            return odd / 2; 
        }

        /// <summary> Gets odd vertices of the given <paramref name="graph"/>. </summary>
        /// <param name="graph">Graph to visit.</param>
        /// <returns>Enumerable of odd vertices.</returns>
        /// <exception cref="T:System.ArgumentNullException"><paramref name="graph"/> is <see langword="null"/>.</exception>
        [Pure]
        [NotNull, ItemNotNull]
        public static IEnumerable<TVertex> OddVertices<TVertex, TEdge>(
            [NotNull] this IVertexAndEdgeListGraph<TVertex, TEdge> graph)
            where TEdge : IEdge<TVertex>
            => VertexDegree(graph)
                .Where(pair => pair.Value % 2 != 0) // Odds
                .Select(pair => pair.Key);

        /// <summary> Returns the directed Degree for every Vertex </summary>
        /// <remarks>
        /// The Degree for directed Graphs is the Difference between the FanOut and the FanIn.
        /// 
        /// This is similar to the <see cref="UndirectedGraph{TVertex,TEdge}.AdjacentDegree"/> for undirected Graphs.
        /// </remarks>
        /// <exception cref="ArgumentNullException"></exception>
        public static Dictionary<TVertex, int> VertexDegree<TVertex, TEdge>(this IVertexAndEdgeListGraph<TVertex, TEdge> graph) where TEdge : IEdge<TVertex>
        {
            if (graph is null)
                throw new ArgumentNullException(nameof(graph));

            var fanOut = new Dictionary<TVertex, int>(graph.VertexCount);
            foreach (TVertex vertex in graph.Vertices)
            {
                fanOut.Add(vertex, 0);
            }

            foreach (TEdge edge in graph.Edges)
            {
                ++fanOut[edge.Source];
                --fanOut[edge.Target];
            }

            return fanOut;
        }

        /// <summary> Returns the <paramref name="fanIn"/> or the fanOut for each Vertex </summary>
        /// <remarks>The Difference between both is the <seealso cref="VertexDegree{TVertex,TEdge}"/></remarks>
        /// <exception cref="ArgumentNullException"></exception>
        public static IDictionary<TVertex, int> Fan<TVertex, TEdge>(this IVertexAndEdgeListGraph<TVertex, TEdge> graph
        , bool fanIn, [CanBeNull] IDictionary<TVertex, int> fan = null) where TEdge : IEdge<TVertex>
        {
            if (graph is null)
                throw new ArgumentNullException(nameof(graph));

            fan = fan ?? new Dictionary<TVertex, int>(graph.VertexCount);
            foreach (TEdge edge in graph.Edges)
            {
                TVertex key = fanIn ? edge.Target : edge.Source;
                fan.TryGetValue(key, out int count);
                fan[key] = 1 + count;
            }

            return fan;
        }

        /// <summary> Tries to compute all Euler-Paths </summary>
        /// <returns>false when the <paramref name="graph"/> does not have any Euler-Paths</returns>
        public static bool TryComputeTrails<TVertex, TEdge>(this IMutableVertexAndEdgeListGraph<TVertex, TEdge> graph
            , Func<TVertex, TVertex, TEdge> edgeFactory
            , out ICollection<TEdge>[] trails, out TEdge[] circuit) where TEdge : IEdge<TVertex>
        {
            trails = new ICollection<TEdge>[0];
            circuit = new TEdge[0];

            int pathCount = graph.ComputeEulerianPathCount();
            if (pathCount == 0)
                return false;

            var algorithm = graph.CreateEulerianTrailAlgorithm();
            algorithm.AddTemporaryEdges((s, t) => edgeFactory(s, t));
            algorithm.Compute();
            trails = algorithm.Trails().ToArray();

            algorithm.RemoveTemporaryEdges();
            //Assert.IsNotNull(algorithm.Circuit);
            circuit = algorithm.Circuit;
            return true;
        }

        /// <summary> Creates a new <see cref="EulerianTrailAlgorithm{TVertex,TEdge}"/> class. </summary>
        public static EulerianTrailAlgorithm<TVertex
            , TEdge> CreateEulerianTrailAlgorithm<TVertex, TEdge>(this IMutableVertexAndEdgeListGraph<TVertex, TEdge> graph,
            [CanBeNull] IAlgorithmComponent host = null) where TEdge : IEdge<TVertex>
            => new EulerianTrailAlgorithm<TVertex, TEdge>(graph, host);
    }

    /// <summary> Algorithm that finds Eulerian <seealso cref="Trails()"/> and <see cref="Circuit"/> in a graph, starting from the <see cref="RootedAlgorithmBase{TVertex,TGraph}.TryGetRootVertex"/>. </summary>
    /// <remarks>
    /// AKA Eulerian Path that __traverses each Edge exactly once__.
    ///
    /// This requires that each <typeparamref name="TVertex"/>
    /// has an even <see cref="IImplicitUndirectedGraph{TVertex,TEdge}.AdjacentDegree"/>,
    /// except for at most 2, which are the End Nodes. 
    /// </remarks>
    /// <inheritdoc cref="Trails(TVertex)"/>
    public sealed class EulerianTrailAlgorithm<TVertex, TEdge>
        : RootedAlgorithmBase<TVertex, IMutableVertexAndEdgeListGraph<TVertex, TEdge>>
        , ITreeBuilderAlgorithm<TVertex, TEdge>
        where TEdge : IEdge<TVertex>
    {
        /// <summary> The processed Graph </summary>
        public IGraph<TVertex, TEdge> VisitededGraph => base.VisitedGraph;

        /// <summary> Built in <see cref="SearchRecursively(TVertex)"/> </summary>
        [NotNull, ItemNotNull]
        private readonly List<TEdge> _temporaryCircuit = new List<TEdge>();

        [CanBeNull]
        private TVertex _currentVertex;

        [NotNull, ItemNotNull]
        private List<TEdge> _temporaryEdges = new List<TEdge>();

        /// <summary> Creates a new <see cref="EulerianTrailAlgorithm{TVertex,TEdge}"/> class. </summary>
        /// <param name="visitedGraph">Graph to visit.</param>
        /// <param name="host">Host to use if set, otherwise use this reference.</param>
        /// <exception cref="T:System.ArgumentNullException"><paramref name="visitedGraph"/> is <see langword="null"/>.</exception>
        internal EulerianTrailAlgorithm([NotNull] IMutableVertexAndEdgeListGraph<TVertex, TEdge> visitedGraph,
            [CanBeNull] IAlgorithmComponent host = null)
            : base(visitedGraph, host)
        {
            _currentVertex = default(TVertex);
        }

        [NotNull, ItemNotNull]
        private List<TEdge> _circuit = new List<TEdge>();

        /// <summary> Circuit. </summary>
        [NotNull, ItemNotNull]
        public TEdge[] Circuit => _circuit.ToArray();

        [Pure]
        private bool NotInCircuit([NotNull] TEdge edge) => !_circuit.Contains(edge) && !_temporaryCircuit.Contains(edge);

        [Pure]
        [NotNull, ItemNotNull]
        private IEnumerable<TEdge> SelectOutEdgesNotInCircuit([NotNull] TVertex vertex)
            => VisitedGraph.OutEdges(vertex).Where(NotInCircuit);

        [Pure]
        private bool TrySelectSingleOutEdgeNotInCircuit([NotNull] TVertex vertex, out TEdge edge)
        {
            IEnumerable<TEdge> edgesNotInCircuit = SelectOutEdgesNotInCircuit(vertex);
            using (IEnumerator<TEdge> enumerator = edgesNotInCircuit.GetEnumerator())
            {
                if (!enumerator.MoveNext())
                {
                    edge = default(TEdge);
                    return false;
                }

                edge = enumerator.Current;
                return true;
            }
        }

        /// <inheritdoc />
        public event EdgeAction<TVertex, TEdge> TreeEdge;

        private void OnTreeEdge([NotNull] TEdge edge)
        {
            Debug.Assert(edge != null);

            TreeEdge?.Invoke(edge);
        }

        /// <summary>
        /// Fired when an edge is added to the circuit.
        /// </summary>
        public event EdgeAction<TVertex, TEdge> CircuitEdge;

        private void OnCircuitEdge([NotNull] TEdge edge)
        {
            Debug.Assert(edge != null);

            CircuitEdge?.Invoke(edge);
        }

        /// <summary> Fired when an edge is visited. </summary>
        public event EdgeAction<TVertex, TEdge> VisitEdge;

        private void OnVisitEdge([NotNull] TEdge edge)
        {
            Debug.Assert(edge != null);

            VisitEdge?.Invoke(edge);
        }

        private bool SearchRecursively([NotNull] TVertex vertex)
        {
            Debug.Assert(vertex != null);

            foreach (TEdge edge in SelectOutEdgesNotInCircuit(vertex))
            {
                OnTreeEdge(edge);

                TVertex target = edge.Target;
                // Add edge to temporary path
                _temporaryCircuit.Add(edge);

                // edge.Target should be equal to CurrentVertex.
                if (VisitedGraph.AreVerticesEqual(edge.Target, _currentVertex))
                    return true;

                // Continue search
                if (SearchRecursively(target))
                    return true;

                // Remove edge
                _temporaryCircuit.Remove(edge);
            }

            // It's a dead end
            return false;
        }

        /// <summary>
        /// Looks for a new path to add to the current vertex.
        /// </summary>
        /// <returns>True a new path was found, false otherwise.</returns>
        [Pure]
        private bool Visit()
        {
            // Find a vertex that needs to be visited
            foreach (TVertex source in _circuit.Select(edge => edge.Source))
            {
                bool edgeFound = TrySelectSingleOutEdgeNotInCircuit(source, out TEdge foundEdge);
                if (!edgeFound)
                    continue;

                OnVisitEdge(foundEdge);
                _currentVertex = source;
                if (SearchRecursively(_currentVertex))
                    return true;
            }

            // Could not augment circuit
            return false;
        }

        /// <summary>
        /// Merges the temporary circuit with the current circuit.
        /// </summary>
        /// <returns>True if all the graph edges are in the circuit.</returns>
        [Pure]
        private bool CircuitAugmentation()
        {
            var newCircuit = new List<TEdge>(_circuit.Count + _temporaryCircuit.Count);
            int i, j;

            // Follow C until w is found
            for (i = 0; i < _circuit.Count; ++i)
            {
                TEdge edge = _circuit[i];
                if (VisitedGraph.AreVerticesEqual(edge.Source, _currentVertex))
                    break;
                newCircuit.Add(edge);
            }

            // Follow D until W is found again
            for (j = 0; j < _temporaryCircuit.Count; ++j)
            {
                TEdge edge = _temporaryCircuit[j];
                newCircuit.Add(edge);
                OnCircuitEdge(edge);
                if (VisitedGraph.AreVerticesEqual(edge.Target, _currentVertex))
                    break;
            }
            _temporaryCircuit.Clear();

            // Continue C
            for (; i < _circuit.Count; ++i)
            {
                TEdge edge = _circuit[i];
                newCircuit.Add(edge);
            }

            // Set as new circuit
            _circuit = newCircuit;

            // Check if contains all edges
            if (_circuit.Count == VisitedGraph.EdgeCount)
                return true;

            return false;
        }

        #region AlgorithmBase<TGraph>

        /// <inheritdoc />
        protected override void InternalCompute()
        {
            if (VisitedGraph.VertexCount == 0)
                return;

            if (!TryGetRootVertex(out TVertex root))
            {
                root = VisitedGraph.Vertices.First();
            }

            _currentVertex = root;

            // Start search
            SearchRecursively(_currentVertex);
            if (CircuitAugmentation())
                return; // Circuit is found

            do
            {
                if (!Visit())
                    break; // Visit edges and build path
                if (CircuitAugmentation())
                    break; // Circuit is found
            } while (true);
        }

        #endregion

        [Pure]
        private bool HasEdgeToward([NotNull] TVertex u, [NotNull] TVertex v)
        {
            Debug.Assert(u != null);
            Debug.Assert(v != null);

            // ReSharper disable once AssignNullToNotNullAttribute
            return VisitedGraph
                .OutEdges(v)
                .Any(outEdge => VisitedGraph.AreVerticesEqual(outEdge.Target, u));
        }

        [Pure]
        private bool FindAdjacentOddVertex(
            [NotNull] TVertex u,
            [NotNull, ItemNotNull] ICollection<TVertex> oddVertices,
            [NotNull, InstantHandle] EdgeFactory<TVertex, TEdge> edgeFactory,
            out bool foundAdjacent)
        {
            bool found = false;
            foundAdjacent = false;
            // ReSharper disable once AssignNullToNotNullAttribute
            foreach (TVertex v in VisitedGraph.OutEdges(u).Select(outEdge => outEdge.Target))
            {
                if (!VisitedGraph.AreVerticesEqual(v, u) && oddVertices.Contains(v))
                {
                    foundAdjacent = true;
                    // Check that v does not have an out-edge towards u
                    if (HasEdgeToward(u, v))
                        continue;

                    // Add temporary edge
                    AddTemporaryEdge(u, v, oddVertices, edgeFactory);

                    // Set u to null
                    found = true;
                    break;
                }
            }

            return found;
        }

        /// <summary> Adds temporary edges to the graph to make all vertices even. </summary>
        /// <returns>Temporary edges list.</returns>
        /// <exception cref="T:System.ArgumentNullException"><paramref name="edgeFactory"/> is <see langword="null"/>.</exception>
        /// <exception cref="T:System.InvalidOperationException">
        /// Number of odd vertices is not even, failed to add temporary edge to <see cref="AlgorithmBase{TGraph}.VisitedGraph"/>,
        /// or failed to compute eulerian trail.
        /// </exception>
        [NotNull, ItemNotNull]
        public TEdge[] AddTemporaryEdges([NotNull, InstantHandle] EdgeFactory<TVertex, TEdge> edgeFactory)
        {
            if (edgeFactory is null)
                throw new ArgumentNullException(nameof(edgeFactory));

            // First gather odd edges
            var oddVertices = VisitedGraph.OddVertices().ToList();

            // Check that there are an even number of them
            if (oddVertices.Count % 2 != 0)
                throw new InvalidOperationException("Number of odd vertices in not even.");

            // Add temporary edges to create even edges
            _temporaryEdges = new List<TEdge>();

            while (oddVertices.Count > 0)
            {
                TVertex u = oddVertices[0];
                // Find adjacent odd vertex
                bool found = FindAdjacentOddVertex(u, oddVertices, edgeFactory, out bool foundAdjacent);
                if (!foundAdjacent)
                {
                    // Pick another vertex
                    if (oddVertices.Count < 2)
                        throw new InvalidOperationException("Eulerian trail failure.");
                    TVertex v = oddVertices[1];

                    // Add to temporary edges
                    AddTemporaryEdge(u, v, oddVertices, edgeFactory);

                    // Set u to null
                    found = true;
                }

                if (!found)
                {
                    oddVertices.Remove(u);
                    oddVertices.Add(u);
                }
            }

            return _temporaryEdges.ToArray();
        }

        private void AddTemporaryEdge(
            [NotNull] TVertex u,
            [NotNull] TVertex v,
            [NotNull, ItemNotNull] ICollection<TVertex> oddVertices,
            [NotNull, InstantHandle] EdgeFactory<TVertex, TEdge> edgeFactory)
        {
            TEdge tempEdge = edgeFactory(u, v);
            if (!VisitedGraph.AddEdge(tempEdge))
                throw new InvalidOperationException("Cannot add temporary edge.");

            // Add to temporary edges
            _temporaryEdges.Add(tempEdge);

            // Remove u,v from oddVertices
            oddVertices.Remove(u);
            oddVertices.Remove(v);
        }

        /// <summary> Removes temporary edges. </summary>
        public void RemoveTemporaryEdges()
        {
            // Remove from graph
            foreach (TEdge edge in _temporaryEdges)
            {
                VisitedGraph.RemoveEdge(edge);
            }
            _temporaryEdges.Clear();
        }

        /// <summary> Computes the set of Eulerian trails that traverse each edge exactly once. </summary>
        /// <remarks>
        /// This method returns a set of disjoint Eulerian trails.
        /// This set of trails spans the entire set of edges.
        /// </remarks>
        /// <returns>Eulerian trail set.</returns>
        [NotNull, ItemNotNull]
        public IEnumerable<ICollection<TEdge>> Trails()
        {
            var trail = new List<TEdge>();
            foreach (TEdge edge in _circuit)
            {
                if (_temporaryEdges.Contains(edge))
                {
                    // Store previous trail and start new one
                    if (trail.Count != 0)
                        yield return trail;

                    // Start new trail
                    trail = new List<TEdge>();
                }
                else
                {
                    trail.Add(edge);
                }
            }

            if (trail.Count != 0)
                yield return trail;
        }

        /// <summary>
        /// Computes a set of Eulerian trails, starting at <paramref name="startingVertex"/>
        /// that spans the entire graph.
        /// </summary>
        /// <remarks>
        /// <para>
        /// The algorithms iterates through the Eulerian circuit of the augmented graph
        /// (the augmented graph is the graph with additional edges to make the number of odd vertices even).
        /// </para>
        /// <para>
        /// If the current edge is not temporary, it is added to the current trail.
        /// </para>
        /// <para>
        /// If the current edge is temporary, the current trail is finished
        /// and added to the trail collection.
        ///
        /// The shortest path between the <paramref name="startingVertex"/>
        /// and the target vertex of the temporary edge is then used to start the new trail.
        ///
        /// This shortest path is computed using the <see cref="BreadthFirstSearchAlgorithm{TVertex,TEdge}"/>.
        /// </para>
        /// </remarks>
        /// <param name="startingVertex">Starting vertex.</param>
        /// <returns>Eulerian trail set, all starting at <paramref name="startingVertex"/>.</returns>
        /// <exception cref="T:System.ArgumentNullException"><paramref name="startingVertex"/> is <see langword="null"/>.</exception>
        /// <exception cref="T:System.InvalidOperationException">Eulerian trail not computed yet.</exception>
        [NotNull, ItemNotNull]
        public IEnumerable<ICollection<TEdge>> Trails([NotNull] TVertex startingVertex)
        {
            if (startingVertex == null)
                throw new ArgumentNullException(nameof(startingVertex));

            return TrailsInternal(startingVertex);
        }

        [Pure]
        private int FindFirstEdgeInCircuit([NotNull] TVertex startingVertex)
        {
            int i;
            for (i = 0; i < _circuit.Count; ++i)
            {
                TEdge edge = _circuit[i];
                if (_temporaryEdges.Contains(edge))
                    continue;
                if (VisitedGraph.AreVerticesEqual(edge.Source, startingVertex))
                    break;
            }

            if (i == _circuit.Count)
                throw new InvalidOperationException("Did not find vertex in Eulerian trail?");

            return i;
        }

        [NotNull, ItemNotNull]
        private IEnumerable<ICollection<TEdge>> TrailsInternal([NotNull] TVertex startingVertex)
        {
            int index = FindFirstEdgeInCircuit(startingVertex);

            // Create trail
            var trail = new List<TEdge>();
            var bfs = VisitedGraph.CreateBreadthFirstSearchAlgorithm();
            var vis = new VertexPredecessorRecorderObserver<TVertex, TEdge>(VisitedGraph.AreVerticesEqual);
            using (vis.Attach(bfs))
            {
                bfs.Compute(startingVertex);

                // Go through the edges and build the predecessor table
                int start = index;
                for (; index < _circuit.Count; ++index)
                {
                    TEdge edge = _circuit[index];
                    if (_temporaryEdges.Contains(edge))
                    {
                        // Store previous trail and start new one
                        if (trail.Count != 0)
                            yield return trail;

                        // Start new trail
                        // Take the shortest path from the start vertex to the target vertex
                        var path = vis.GetPath(edge.Target);
                        if (path == null)
                            throw new InvalidOperationException();
                        trail = new List<TEdge>(path);
                    }
                    else
                    {
                        trail.Add(edge);
                    }
                }

                // Starting again on the circuit
                for (index = 0; index < start; ++index)
                {
                    TEdge edge = _circuit[index];
                    if (_temporaryEdges.Contains(edge))
                    {
                        // Store previous trail and start new one
                        if (trail.Count != 0)
                            yield return trail;

                        // Start new trail
                        // Take the shortest path from the start vertex to the target vertex
                        var path = vis.GetPath(edge.Target);
                        if (path == null)
                            throw new InvalidOperationException();
                        trail = new List<TEdge>(path);
                    }
                    else
                    {
                        trail.Add(edge);
                    }
                }
            }

            // Adding the last element
            if (trail.Count != 0)
                yield return trail;
        }
    }
}