using System;
using JetBrains.Annotations;
using NUnit.Framework;

namespace QuikGraph.Serialization.Tests
{
    [TestFixture]
    internal sealed class SerializationHelpersTests
    {
        [TestCase(typeof(AdjacencyGraph<int, Edge<int>>), typeof(AdjacencyGraph<int, Edge<int>>), ExpectedResult = true)]
        [TestCase(typeof(AdjacencyGraph<int, Edge<int>>), typeof(IGraph<int, Edge<int>>), ExpectedResult = true)]
        [TestCase(typeof(AdjacencyGraph<int, Edge<int>>), typeof(IEdgeListGraph<int, Edge<int>>), ExpectedResult = true)]
        [TestCase(typeof(AdjacencyGraph<int, Edge<int>>), typeof(IVertexAndEdgeListGraph<int, Edge<int>>), ExpectedResult = true)]

        [TestCase(typeof(AdjacencyGraph<int, Edge<int>>), typeof(AdjacencyGraph<int, EquatableEdge<int>>), ExpectedResult = false)]
        [TestCase(typeof(AdjacencyGraph<int, Edge<int>>), typeof(AdjacencyGraph<float, Edge<float>>), ExpectedResult = false)]
        [TestCase(typeof(AdjacencyGraph<int, Edge<int>>), typeof(IGraph<float, Edge<float>>), ExpectedResult = false)]
        [TestCase(typeof(AdjacencyGraph<int, Edge<int>>), typeof(IUndirectedGraph<int, Edge<int>>), ExpectedResult = false)]
        [TestCase(typeof(AdjacencyGraph<int, Edge<int>>), typeof(IEdgeListGraph<float, Edge<float>>), ExpectedResult = false)]
        [TestCase(typeof(AdjacencyGraph<int, Edge<int>>), typeof(IVertexAndEdgeListGraph<float, Edge<float>>), ExpectedResult = false)]

        [TestCase(typeof(IVertexAndEdgeListGraph<int, Edge<int>>), typeof(IEdgeListGraph<int, Edge<int>>), ExpectedResult = true)]
        [TestCase(typeof(IVertexAndEdgeListGraph<int, Edge<int>>), typeof(IVertexAndEdgeListGraph<int, Edge<int>>), ExpectedResult = true)]

        [TestCase(typeof(IVertexAndEdgeListGraph<int, Edge<int>>), typeof(IEdgeListGraph<float, Edge<float>>), ExpectedResult = false)]
        [TestCase(typeof(IVertexAndEdgeListGraph<int, Edge<int>>), typeof(IVertexAndEdgeListGraph<float, Edge<float>>), ExpectedResult = false)]

        [TestCase(typeof(AdjacencyGraph<,>), typeof(AdjacencyGraph<,>), ExpectedResult = true)]
        [TestCase(typeof(AdjacencyGraph<int, Edge<int>>), typeof(AdjacencyGraph<,>), ExpectedResult = true)]
        [TestCase(typeof(AdjacencyGraph<,>), typeof(IGraph<,>), ExpectedResult = true)]
        public bool IsAssignableToGenericType([NotNull] Type typeToCheck, [NotNull] Type genericType)
        {
            return SerializationHelpers.IsAssignableToGenericType(typeToCheck, genericType);
        }
    }
}