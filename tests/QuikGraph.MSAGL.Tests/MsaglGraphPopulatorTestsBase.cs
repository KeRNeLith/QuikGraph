using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using JetBrains.Annotations;
using NUnit.Framework;
using static QuikGraph.MSAGL.Tests.MsaglGraphTestHelpers;
using NotNullAttribute = JetBrains.Annotations.NotNullAttribute;

namespace QuikGraph.MSAGL.Tests
{
    /// <summary>
    /// Base class for tests relative to <see cref="MsaglGraphPopulator{TVertex,TEdge}"/>.
    /// </summary>
    internal class MsaglGraphPopulatorTestsBase
    {
        protected static void Compute_Test<TPopulator>(
            [NotNull, InstantHandle] Func<IEdgeListGraph<int, IEdge<int>>, TPopulator> createPopulator)
            where TPopulator : MsaglGraphPopulator<int, IEdge<int>>
        {
            // Empty graph
            var graph = new AdjacencyGraph<int, IEdge<int>>();
            MsaglGraphPopulator<int, IEdge<int>> populator = createPopulator(graph);
            populator.Compute();
            AssertAreEquivalent(graph, populator.MsaglGraph);

            // Only vertices
            graph = new AdjacencyGraph<int, IEdge<int>>();
            graph.AddVertexRange( 1, 2, 3 );
            populator = createPopulator(graph);
            populator.Compute();
            AssertAreEquivalent(graph, populator.MsaglGraph);

            // With vertices and edges
            graph = new AdjacencyGraph<int, IEdge<int>>();
            graph.AddVerticesAndEdgeRange(
                Edge.Create(1, 2),
                Edge.Create(1, 3),
                Edge.Create(2, 3)
            );
            graph.AddVertexRange( 5, 6 );
            populator = createPopulator(graph);
            populator.Compute();
            AssertAreEquivalent(graph, populator.MsaglGraph);

            // With cycles
            graph = new AdjacencyGraph<int, IEdge<int>>();
            graph.AddVerticesAndEdgeRange(
                Edge.Create(1, 2),
                Edge.Create(1, 3),
                Edge.Create(2, 4),
                Edge.Create(3, 1),
                Edge.Create(4, 1)
            );
            populator = createPopulator(graph);
            populator.Compute();
            AssertAreEquivalent(graph, populator.MsaglGraph);

            // With self edge
            graph = new AdjacencyGraph<int, IEdge<int>>();
            graph.AddVerticesAndEdgeRange(
                Edge.Create(1, 2),
                Edge.Create(1, 3),
                Edge.Create(2, 2),
                Edge.Create(3, 1)
            );
            populator = createPopulator(graph);
            populator.Compute();
            AssertAreEquivalent(graph, populator.MsaglGraph);

            // Undirected graph
            var undirectedGraph = new UndirectedGraph<int, IEdge<int>>();
            undirectedGraph.AddVerticesAndEdgeRange(
                Edge.Create(1, 2),
                Edge.Create(1, 3),
                Edge.Create(2, 4),
                Edge.Create(3, 1)
            );
            populator = createPopulator(undirectedGraph);
            populator.Compute();
            AssertAreEquivalent(undirectedGraph, populator.MsaglGraph);
        }

        [SuppressMessage("ReSharper", "AccessToModifiedClosure")]
        protected static void Handlers_Test<TPopulator>(
            [NotNull, InstantHandle] Func<IEdgeListGraph<int, IEdge<int>>, TPopulator> createPopulator)
            where TPopulator : MsaglGraphPopulator<int, IEdge<int>>
        {
            // Empty graph
            var graph = new AdjacencyGraph<int, IEdge<int>>();
            MsaglGraphPopulator<int, IEdge<int>> populator = createPopulator(graph);
            populator.NodeAdded += (_, _) => Assert.Fail($"{nameof(MsaglGraphPopulator<object, Edge<object>>.NodeAdded)} event called.");
            populator.EdgeAdded += (_, _) => Assert.Fail($"{nameof(MsaglGraphPopulator<object, Edge<object>>.EdgeAdded)} event called.");
            populator.Compute();

            // Only vertices
            graph = new AdjacencyGraph<int, IEdge<int>>();
            graph.AddVertexRange( 1, 2, 3 );
            populator = createPopulator(graph);
            var expectedVerticesAdded = new HashSet<int> { 1, 2, 3 };
            populator.NodeAdded += (_, args) =>
            {
                Assert.IsTrue(expectedVerticesAdded.Remove(args.Vertex));
                Assert.AreEqual(args.Vertex, args.Node.UserData);
            };
            populator.EdgeAdded += (_, _) => Assert.Fail($"{nameof(MsaglGraphPopulator<object, Edge<object>>.EdgeAdded)} event called.");
            populator.Compute();
            CollectionAssert.IsEmpty(expectedVerticesAdded);

            // With vertices and edges
            var edge12 = Edge.Create(1, 2);
            var edge13 = Edge.Create(1, 3);
            var edge23 = Edge.Create(2, 3);
            graph = new AdjacencyGraph<int, IEdge<int>>();
            graph.AddVerticesAndEdgeRange( edge12, edge13, edge23 );
            graph.AddVertexRange( 5, 6 );
            populator = createPopulator(graph);
            expectedVerticesAdded = new HashSet<int> { 1, 2, 3, 5, 6 };
            var expectedEdgesAdded = new HashSet<IEdge<int>> { edge12, edge13, edge23 };
            populator.NodeAdded += (_, args) =>
            {
                Assert.IsTrue(expectedVerticesAdded.Remove(args.Vertex));
                Assert.AreEqual(args.Vertex, args.Node.UserData);
            };
            populator.EdgeAdded += (_, args) =>
            {
                Assert.IsTrue(expectedEdgesAdded.Remove(args.Edge));
                Assert.AreSame(args.Edge, args.MsaglEdge.UserData);
            };
            populator.Compute();
            CollectionAssert.IsEmpty(expectedVerticesAdded);
            CollectionAssert.IsEmpty(expectedEdgesAdded);
        }
    }
}