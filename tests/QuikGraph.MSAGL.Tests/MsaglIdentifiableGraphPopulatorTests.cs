using System;
using NUnit.Framework;
using QuikGraph.Tests.Algorithms;


namespace QuikGraph.MSAGL.Tests
{
    /// <summary>
    /// Tests related to <see cref="MsaglIdentifiableGraphPopulator{TVertex,TEdge}"/>.
    /// </summary>
    internal sealed class MsaglIdentifiableGraphPopulatorTests : MsaglGraphPopulatorTestsBase
    {
        [Test]
        public void Constructor()
        {
            VertexIdentity<int> vertexIdentity = vertex => vertex.ToString();
            var graph = new AdjacencyGraph<int, IEdge<int>>();
            var populator = new MsaglIdentifiableGraphPopulator<int, IEdge<int>>(graph, vertexIdentity);
            AssertPopulatorProperties(populator, graph);

            var undirectedGraph = new UndirectedGraph<int, IEdge<int>>();
            populator = new MsaglIdentifiableGraphPopulator<int, IEdge<int>>(undirectedGraph, vertexIdentity);
            AssertPopulatorProperties(populator, undirectedGraph);

            #region Local function

            void AssertPopulatorProperties<TVertex, TEdge>(
                MsaglIdentifiableGraphPopulator<TVertex, TEdge> p,
                IEdgeListGraph<TVertex, TEdge> g)
                where TEdge : IEdge<TVertex>
            {
                p.AssertAlgorithmState(g);
                Assert.IsNull(p.MsaglGraph);
            }

            #endregion
        }

        [Test]
        public void Constructor_Throws()
        {
            VertexIdentity<int> vertexIdentity = vertex => vertex.ToString();
            var graph = new AdjacencyGraph<int, IEdge<int>>();

            // ReSharper disable ObjectCreationAsStatement
            // ReSharper disable AssignNullToNotNullAttribute
            Assert.Throws<ArgumentNullException>(
                () => new MsaglIdentifiableGraphPopulator<int, IEdge<int>>(graph, null));
            Assert.Throws<ArgumentNullException>(
                () => new MsaglIdentifiableGraphPopulator<int, IEdge<int>>(null, vertexIdentity));
            Assert.Throws<ArgumentNullException>(
                () => new MsaglIdentifiableGraphPopulator<int, IEdge<int>>(null, null));
            // ReSharper restore AssignNullToNotNullAttribute
            // ReSharper restore ObjectCreationAsStatement
        }

        [Test]
        public void Compute()
        {
            Compute_Test(graph => new MsaglIdentifiableGraphPopulator<int, IEdge<int>>(graph, vertex => vertex.ToString()));
        }

        [Test]
        public void Handlers()
        {
            Handlers_Test(graph => new MsaglIdentifiableGraphPopulator<int, IEdge<int>>(graph, vertex => vertex.ToString()));
        }

        [Test]
        public void VertexId()
        {
            var graph = new AdjacencyGraph<int, IEdge<int>>();
            graph.AddVerticesAndEdgeRange(
                Edge.Create(1, 2),
                Edge.Create(2, 3)
            );
            graph.AddVertexRange( 5, 6 );

            var populator = new MsaglIdentifiableGraphPopulator<int, IEdge<int>>(graph, vertex => $"MyTestId{vertex}");
            populator.Compute();

            // Check vertices has been well formatted
            Assert.IsNull(populator.MsaglGraph.FindNode("MyTestId0"));
            Assert.IsNotNull(populator.MsaglGraph.FindNode("MyTestId1"));
        }
    }
}