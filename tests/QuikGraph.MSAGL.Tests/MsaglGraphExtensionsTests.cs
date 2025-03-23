using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using Microsoft.Msagl.Drawing;
using NUnit.Framework;
using static QuikGraph.MSAGL.Tests.MsaglGraphTestHelpers;

namespace QuikGraph.MSAGL.Tests
{
    /// <summary>
    /// Tests related to <see cref="MsaglGraphExtensions"/>.
    /// </summary>
    internal sealed class MsaglGraphExtensionsTests
    {
        #region Test classes

        private class VertexTestFormatProvider : IFormatProvider
        {
            public object GetFormat(Type formatType)
            {
                return null;
            }
        }

        #endregion

        [Test]
        public void CreatePopulators()
        {
            var graph = new AdjacencyGraph<int, IEdge<int>>();
            CreatePopulators_Test(graph);

            var undirectedGraph = new UndirectedGraph<int, IEdge<int>>();
            CreatePopulators_Test(undirectedGraph);

            #region Local function

            void CreatePopulators_Test(IEdgeListGraph<int, IEdge<int>> g)
            {
                var populator = g.CreateMsaglPopulator();
                Assert.IsNotNull(populator);

                populator = g.CreateMsaglPopulator("TestFormat {0}");
                Assert.IsNotNull(populator);

                populator = g.CreateMsaglPopulator("TestFormat {0}", new VertexTestFormatProvider());
                Assert.IsNotNull(populator);

                populator = g.CreateMsaglPopulator(v => v.ToString());
                Assert.IsNotNull(populator);
            }

            #endregion
        }

        [Test]
        public void CreatePopulators_Throws()
        {
            var graph = new AdjacencyGraph<int, IEdge<int>>();
            // ReSharper disable AssignNullToNotNullAttribute
            Assert.Throws<ArgumentNullException>(() => MsaglGraphExtensions.CreateMsaglPopulator<int, IEdge<int>>(null));

            Assert.Throws<ArgumentNullException>(() => MsaglGraphExtensions.CreateMsaglPopulator<int, IEdge<int>>(null, vertex => vertex.ToString()));
            Assert.Throws<ArgumentNullException>(() => graph.CreateMsaglPopulator(null));
            Assert.Throws<ArgumentNullException>(() => MsaglGraphExtensions.CreateMsaglPopulator<int, IEdge<int>>(null, null));

            Assert.Throws<ArgumentNullException>(() => MsaglGraphExtensions.CreateMsaglPopulator<int, IEdge<int>>(null, "Format {0}"));
            // ReSharper restore AssignNullToNotNullAttribute
        }

        [Test]
        [SuppressMessage("ReSharper", "AccessToModifiedClosure")]
        public void ToMsaglGraph()
        {
            var graph = new AdjacencyGraph<int, IEdge<int>>();
            ToMsaglGraph_Test(graph);

            graph = new AdjacencyGraph<int, IEdge<int>>();
            graph.AddVertexRange( 1, 2, 4 );
            ToMsaglGraph_Test(graph);

            graph = new AdjacencyGraph<int, IEdge<int>>();
            graph.AddVerticesAndEdgeRange(
                Edge.Create(1, 2),
                Edge.Create(2, 3),
                Edge.Create(2, 5),
                Edge.Create(3, 4),
                Edge.Create(4, 3)
            );
            graph.AddVertex(6);
            ToMsaglGraph_Test(graph);

            var undirectedGraph = new UndirectedGraph<int, IEdge<int>>();
            undirectedGraph.AddVerticesAndEdgeRange(
                Edge.Create(1, 2),
                Edge.Create(2, 3),
                Edge.Create(2, 5),
                Edge.Create(3, 4),
                Edge.Create(4, 3)
            );
            undirectedGraph.AddVertex(6);
            ToMsaglGraph_Test(undirectedGraph);

            #region Local function

            // ReSharper disable once InconsistentNaming
            void ToMsaglGraph_Test(IEdgeListGraph<int, IEdge<int>> g)
            {
                Graph msaglGraph = g.ToMsaglGraph();
                AssertAreEquivalent(g, msaglGraph);

                var expectedVerticesAdded = new HashSet<int>(g.Vertices);
                msaglGraph = g.IsVerticesEmpty
                    ? g.ToMsaglGraph(NoNodeAdded)
                    : g.ToMsaglGraph(NodeAdded);
                AssertAreEquivalent(g, msaglGraph);
                CollectionAssert.IsEmpty(expectedVerticesAdded);

                expectedVerticesAdded = new HashSet<int>(g.Vertices);
                msaglGraph = g.IsVerticesEmpty
                    ? g.ToMsaglGraph(VertexIdentity, NoNodeAdded)
                    : g.ToMsaglGraph(VertexIdentity, NodeAdded);
                AssertAreEquivalent(g, msaglGraph);
                CollectionAssert.IsEmpty(expectedVerticesAdded);


                var expectedEdgesAdded = new HashSet<IEdge<int>>(g.Edges);
                msaglGraph = g.IsEdgesEmpty
                    ? g.ToMsaglGraph(edgeAdded: NoEdgeAdded)
                    : g.ToMsaglGraph(edgeAdded: EdgeAdded);
                AssertAreEquivalent(g, msaglGraph);
                CollectionAssert.IsEmpty(expectedEdgesAdded);

                expectedEdgesAdded = new HashSet<IEdge<int>>(g.Edges);
                msaglGraph = g.IsEdgesEmpty
                    ? g.ToMsaglGraph(VertexIdentity, edgeAdded: NoEdgeAdded)
                    : g.ToMsaglGraph(VertexIdentity, edgeAdded: EdgeAdded);
                AssertAreEquivalent(g, msaglGraph);
                CollectionAssert.IsEmpty(expectedEdgesAdded);


                expectedVerticesAdded = new HashSet<int>(g.Vertices);
                expectedEdgesAdded = new HashSet<IEdge<int>>(g.Edges);
                if (g.IsVerticesEmpty && g.IsEdgesEmpty)
                {
                    msaglGraph = g.ToMsaglGraph(NoNodeAdded, NoEdgeAdded);
                }
                else if (g.IsVerticesEmpty)
                {
                    msaglGraph = g.ToMsaglGraph(NoNodeAdded, EdgeAdded);
                }
                else if (g.IsEdgesEmpty)
                {
                    msaglGraph = g.ToMsaglGraph(NodeAdded, NoEdgeAdded);
                }
                else
                {
                    msaglGraph = g.ToMsaglGraph(NodeAdded, EdgeAdded);
                }
                AssertAreEquivalent(g, msaglGraph);
                CollectionAssert.IsEmpty(expectedVerticesAdded);
                CollectionAssert.IsEmpty(expectedEdgesAdded);

                expectedVerticesAdded = new HashSet<int>(g.Vertices);
                expectedEdgesAdded = new HashSet<IEdge<int>>(g.Edges);
                if (g.IsVerticesEmpty && g.IsEdgesEmpty)
                {
                    msaglGraph = g.ToMsaglGraph(VertexIdentity, NoNodeAdded, NoEdgeAdded);
                }
                else if (g.IsVerticesEmpty)
                {
                    msaglGraph = g.ToMsaglGraph(VertexIdentity, NoNodeAdded, EdgeAdded);
                }
                else if (g.IsEdgesEmpty)
                {
                    msaglGraph = g.ToMsaglGraph(VertexIdentity, NodeAdded, NoEdgeAdded);
                }
                else
                {
                    msaglGraph = g.ToMsaglGraph(VertexIdentity, NodeAdded, EdgeAdded);
                }
                AssertAreEquivalent(g, msaglGraph);
                CollectionAssert.IsEmpty(expectedVerticesAdded);
                CollectionAssert.IsEmpty(expectedEdgesAdded);

                #region Local functions

                string VertexIdentity(int vertex)
                {
                    return $"id{vertex}";
                }

                void NoNodeAdded(object sender, MsaglVertexEventArgs<int> args)
                {
                    Assert.Fail($"{nameof(MsaglGraphPopulator<object, Edge<object>>.NodeAdded)} event called.");
                }

                void NodeAdded(object sender, MsaglVertexEventArgs<int> args)
                {
                    Assert.IsTrue(expectedVerticesAdded.Remove(args.Vertex));
                }

                void NoEdgeAdded(object sender, MsaglEdgeEventArgs<int, IEdge<int>> args)
                {
                    Assert.Fail($"{nameof(MsaglGraphPopulator<object, Edge<object>>.EdgeAdded)} event called.");
                }

                void EdgeAdded(object sender, MsaglEdgeEventArgs<int, IEdge<int>> args)
                {
                    Assert.IsTrue(expectedEdgesAdded.Remove(args.Edge));
                }

                #endregion
            }

            #endregion
        }
    }
}