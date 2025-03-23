using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;

namespace QuikGraph.Tests.Extensions
{
    /// <summary>
    /// Tests related to <see cref="EdgeExtensions"/>.
    /// </summary>
    internal sealed class EdgeExtensionsTests
    {
        [Test]
        public void IsSelfEdge()
        {
            var edge1 = Edge.Create(1, 1);
            var edge2 = Edge.Create(1, 2);
            var edge3 = Edge.Create(2, 1);

            var v1 = new TestVertex("1");
            var v2 = new TestVertex("2");
            var edge4 = Edge.Create(v1, v1);
            var edge5 = Edge.Create(v1, v2);
            var edge6 = Edge.Create(v2, v1);


            Assert.IsTrue(edge1.IsSelfEdge());
            Assert.IsFalse(edge2.IsSelfEdge());
            Assert.IsFalse(edge3.IsSelfEdge());
            Assert.IsTrue(edge4.IsSelfEdge());
            Assert.IsFalse(edge5.IsSelfEdge());
            Assert.IsFalse(edge6.IsSelfEdge());

            // Edges cases
            var v1Bis = new TestVertex("1");
            var edge7 = Edge.Create(v1, v1Bis);
            Assert.IsFalse(edge7.IsSelfEdge());

            var equatableV1 = new EquatableTestVertex("1");
            var equatableV1Bis = new EquatableTestVertex("1");
            var edge8 = Edge.Create(equatableV1, equatableV1Bis);
            Assert.IsTrue(edge8.IsSelfEdge());
        }

        [Test]
        public void IsSelfEdge_Throws()
        {
            // ReSharper disable once AssignNullToNotNullAttribute
            // ReSharper disable once ReturnValueOfPureMethodIsNotUsed
            Assert.Throws<ArgumentNullException>(() => ((Edge<int>)null).IsSelfEdge());
        }

        [Test]
        public void GetOtherVertex()
        {
            var edge1 = Edge.Create(1, 1);
            var edge2 = Edge.Create(1, 2);
            var edge3 = Edge.Create(2, 1);

            var v1 = new TestVertex("1");
            var v2 = new TestVertex("2");
            var edge4 = Edge.Create(v1, v1);
            var edge5 = Edge.Create(v1, v2);
            var edge6 = Edge.Create(v2, v1);

            Assert.AreEqual(1, edge1.GetOtherVertex(1));
            Assert.AreEqual(2, edge2.GetOtherVertex(1));
            Assert.AreEqual(1, edge2.GetOtherVertex(2));
            Assert.AreEqual(2, edge3.GetOtherVertex(1));
            Assert.AreEqual(1, edge3.GetOtherVertex(2));

            Assert.AreSame(v1, edge4.GetOtherVertex(v1));
            Assert.AreSame(v2, edge5.GetOtherVertex(v1));
            Assert.AreSame(v1, edge5.GetOtherVertex(v2));
            Assert.AreSame(v2, edge6.GetOtherVertex(v1));
            Assert.AreSame(v1, edge6.GetOtherVertex(v2));

            // Edges cases
            var vNotInEdge = new TestVertex("1");
            Assert.AreSame(v1, edge5.GetOtherVertex(vNotInEdge));

            var equatableV1 = new EquatableTestVertex("1");
            var equatableV2 = new EquatableTestVertex("2");
            var equatableV1Bis = new EquatableTestVertex("1");
            var edge8 = Edge.Create(equatableV1, equatableV2);
            Assert.AreSame(equatableV2, edge8.GetOtherVertex(equatableV1Bis));
        }

        [Test]
        public void GetOtherVertex_Throws()
        {
            // ReSharper disable AssignNullToNotNullAttribute
            // ReSharper disable ReturnValueOfPureMethodIsNotUsed
            Assert.Throws<ArgumentNullException>(() => ((Edge<int>)null).GetOtherVertex(1));

            var testEdge = Edge.Create(new TestVertex("1"), new TestVertex("2"));
            Assert.Throws<ArgumentNullException>(() => testEdge.GetOtherVertex(null));

            Assert.Throws<ArgumentNullException>(() => ((Edge<TestVertex>)null).GetOtherVertex(null));
            // ReSharper restore ReturnValueOfPureMethodIsNotUsed
            // ReSharper restore AssignNullToNotNullAttribute
        }

        [Test]
        public void IsAdjacent()
        {
            var edge1 = Edge.Create(1, 1);
            var edge2 = Edge.Create(1, 2);
            var edge3 = Edge.Create(2, 1);

            var v1 = new TestVertex("1");
            var v2 = new TestVertex("2");
            var vNotInEdge = new TestVertex("1");
            var edge4 = Edge.Create(v1, v1);
            var edge5 = Edge.Create(v1, v2);
            var edge6 = Edge.Create(v2, v1);

            Assert.IsTrue(edge1.IsAdjacent(1));
            Assert.IsFalse(edge1.IsAdjacent(2));
            Assert.IsTrue(edge2.IsAdjacent(1));
            Assert.IsTrue(edge2.IsAdjacent(2));
            Assert.IsFalse(edge2.IsAdjacent(3));
            Assert.IsTrue(edge3.IsAdjacent(1));
            Assert.IsTrue(edge3.IsAdjacent(2));
            Assert.IsFalse(edge3.IsAdjacent(3));

            Assert.IsTrue(edge4.IsAdjacent(v1));
            Assert.IsTrue(edge5.IsAdjacent(v1));
            Assert.IsTrue(edge5.IsAdjacent(v2));
            Assert.IsFalse(edge5.IsAdjacent(vNotInEdge));
            Assert.IsTrue(edge6.IsAdjacent(v1));
            Assert.IsTrue(edge6.IsAdjacent(v2));
            Assert.IsFalse(edge6.IsAdjacent(vNotInEdge));

            // Edges cases
            var equatableV1 = new EquatableTestVertex("1");
            var equatableV2 = new EquatableTestVertex("2");
            var equatableV3 = new EquatableTestVertex("3");
            var equatableV1Bis = new EquatableTestVertex("1");
            var edge8 = Edge.Create(equatableV1, equatableV2);
            Assert.IsTrue(edge8.IsAdjacent(equatableV1Bis));
            Assert.IsFalse(edge8.IsAdjacent(equatableV3));
        }

        [Test]
        public void IsAdjacent_Throws()
        {
            // ReSharper disable AssignNullToNotNullAttribute
            // ReSharper disable ReturnValueOfPureMethodIsNotUsed
            Assert.Throws<ArgumentNullException>(() => ((Edge<int>)null).IsAdjacent(1));

            var testEdge = Edge.Create(new TestVertex("1"), new TestVertex("2"));
            Assert.Throws<ArgumentNullException>(() => testEdge.IsAdjacent(null));

            Assert.Throws<ArgumentNullException>(() => ((Edge<TestVertex>)null).IsAdjacent(null));
            // ReSharper restore ReturnValueOfPureMethodIsNotUsed
            // ReSharper restore AssignNullToNotNullAttribute
        }

        [Test]
        public void IsPath()
        {
            Assert.IsFalse(Enumerable.Empty<IEdge<int>>().IsPath());

            var edge1 = Edge.Create(1, 1);
            // 1 -> 1
            Assert.IsTrue(new[] { edge1 }.IsPath());

            var edge2 = Edge.Create(1, 2);
            // 1 -> 2
            Assert.IsTrue(new[] { edge2 }.IsPath());

            var edge3 = Edge.Create(2, 1);
            // 1 -> 2 -> 1
            Assert.IsTrue(new[] { edge2, edge3 }.IsPath());
            // 1 -> 1 -> 2 -> 1 -> 1
            Assert.IsTrue(new[] { edge1, edge2, edge3, edge1 }.IsPath());

            var edge4 = Edge.Create(1, 4);
            // 1 -> 2 -> 1 -> 4
            Assert.IsTrue(new[] { edge2, edge3, edge4 }.IsPath());
            // 1 -> 2 -> 1 -> 4-1 -> 2
            Assert.IsFalse(new[] { edge2, edge3, edge4, edge2 }.IsPath());
            // 2 -> 1 -> 4-1 -> 2
            Assert.IsFalse(new[] { edge3, edge4, edge2 }.IsPath());


            var v1 = new TestVertex("1");
            var v2 = new TestVertex("2");
            var v4 = new TestVertex("4");
            var edge5 = Edge.Create(v1, v1);
            // 1 -> 1
            Assert.IsTrue(new[] { edge5 }.IsPath());

            var edge6 = Edge.Create(v1, v2);
            // 1 -> 2
            Assert.IsTrue(new[] { edge6 }.IsPath());

            var edge7 = Edge.Create(v2, v1);
            // 1 -> 2 -> 1
            Assert.IsTrue(new[] { edge6, edge7 }.IsPath());
            // 1 -> 1 -> 2 -> 1 -> 1
            Assert.IsTrue(new[] { edge5, edge6, edge7, edge5 }.IsPath());

            var edge8 = Edge.Create(v1, v4);
            // 1 -> 2 -> 1 -> 4
            Assert.IsTrue(new[] { edge6, edge7, edge8 }.IsPath());
            // 1 -> 2 -> 1 -> 4-1 -> 2
            Assert.IsFalse(new[] { edge6, edge7, edge8, edge6 }.IsPath());
            // 2 -> 1 -> 4-1 -> 2
            Assert.IsFalse(new[] { edge7, edge8, edge6 }.IsPath());


            // Edge cases
            var v2Bis = new TestVertex("2");
            var edge9 = Edge.Create(v2Bis, v1);
            // 1 -> 1 -> 2-2Bis -> 1 -> 1 -> 1
            Assert.IsFalse(new[] { edge5, edge6, edge9, edge5 }.IsPath());

            var equatableV1 = new EquatableTestVertex("1");
            var equatableV2 = new EquatableTestVertex("2");
            var equatableV2Bis = new EquatableTestVertex("2");
            var equatableV4 = new EquatableTestVertex("4");
            var edge10 = Edge.Create(equatableV1, equatableV1);
            var edge11 = Edge.Create(equatableV1, equatableV2);
            var edge12 = Edge.Create(equatableV2Bis, equatableV1);
            var edge13 = Edge.Create(equatableV1, equatableV4);
            // 1 -> 1 -> 2-2Bis -> 1 -> 4
            Assert.IsTrue(new[] { edge10, edge11, edge12, edge13 }.IsPath());
        }

        [Test]
        public void IsPath_Throws()
        {
            // ReSharper disable once AssignNullToNotNullAttribute
            // ReSharper disable once ReturnValueOfPureMethodIsNotUsed
            Assert.Throws<NullReferenceException>(() => ((IEnumerable<IEdge<int>>)null).IsPath());
        }

        [Test]
        public void HasCycles()
        {
            Assert.IsFalse(Enumerable.Empty<IEdge<int>>().HasCycles());

            var edge1 = Edge.Create(1, 1);
            // 1 -> 1
            Assert.IsTrue(new[] { edge1 }.HasCycles());

            var edge2 = Edge.Create(1, 2);
            // 1 -> 2
            Assert.IsFalse(new[] { edge2 }.HasCycles());

            var edge3 = Edge.Create(2, 1);
            // 1 -> 2 -> 1
            Assert.IsTrue(new[] { edge2, edge3 }.HasCycles());
            // 2 -> 1 -> 2
            Assert.IsTrue(new[] { edge3, edge2 }.HasCycles());

            var edge4 = Edge.Create(1, 4);
            var edge5 = Edge.Create(2, 3);
            var edge6 = Edge.Create(3, 4);
            var edge7 = Edge.Create(3, 1);
            var edge8 = Edge.Create(3, 3);
            var edge9 = Edge.Create(4, 3);
            // 1 -> 2 -> 1 -> 4
            Assert.IsTrue(new[] { edge2, edge3, edge4 }.HasCycles());
            // 2 -> 1 -> 4 -> 3
            Assert.IsFalse(new[] { edge3, edge4, edge9 }.HasCycles());
            // 2 -> 1 -> 4 -> 3 -> 1
            Assert.IsTrue(new[] { edge3, edge4, edge9, edge7 }.HasCycles());
            // 2 -> 3 -> 4 -> 3 -> 3
            Assert.IsTrue(new[] { edge5, edge6, edge9, edge8 }.HasCycles());

            var edge10 = Edge.Create(2, 4);
            var edge11 = Edge.Create(3, 2);
            var edge12 = Edge.Create(2, 5);
            // 1 -> 4 -> 3 -> 2 -> 5
            Assert.IsFalse(new[] { edge4, edge9, edge11, edge12 }.HasCycles());
            // 1 -> 2 -> 4 -> 3 -> 2 -> 5
            Assert.IsTrue(new[] { edge2, edge10, edge9, edge11, edge12 }.HasCycles());
            // 1 -> 4 -> 3 -> 3 -> 2 -> 5
            Assert.IsTrue(new[] { edge4, edge9, edge8, edge11, edge12 }.HasCycles());


            var v1 = new TestVertex("1");
            var v2 = new TestVertex("2");
            var v3 = new TestVertex("3");
            var v4 = new TestVertex("4");
            var v5 = new TestVertex("5");
            var edge13 = Edge.Create(v1, v1);
            // 1 -> 1
            Assert.IsTrue(new[] { edge13 }.HasCycles());

            var edge14 = Edge.Create(v1, v2);
            // 1 -> 2
            Assert.IsFalse(new[] { edge14 }.HasCycles());

            var edge15 = Edge.Create(v2, v1);
            // 1 -> 2 -> 1
            Assert.IsTrue(new[] { edge14, edge15 }.HasCycles());
            // 2 -> 1 -> 2
            Assert.IsTrue(new[] { edge15, edge14 }.HasCycles());

            var edge16 = Edge.Create(v1, v4);
            var edge17 = Edge.Create(v2, v3);
            var edge18 = Edge.Create(v3, v4);
            var edge19 = Edge.Create(v3, v1);
            var edge20 = Edge.Create(v3, v3);
            var edge21 = Edge.Create(v4, v3);
            // 1 -> 2 -> 1 -> 4
            Assert.IsTrue(new[] { edge14, edge15, edge16 }.HasCycles());
            // 2 -> 1 -> 4 -> 3
            Assert.IsFalse(new[] { edge15, edge16, edge21 }.HasCycles());
            // 2 -> 1 -> 4 -> 3 -> 1
            Assert.IsTrue(new[] { edge15, edge16, edge21, edge19 }.HasCycles());
            // 2 -> 3 -> 4 -> 3 -> 3
            Assert.IsTrue(new[] { edge17, edge18, edge21, edge20 }.HasCycles());

            var edge22 = Edge.Create(v2, v4);
            var edge23 = Edge.Create(v3, v2);
            var edge24 = Edge.Create(v2, v5);
            // 1 -> 4 -> 3 -> 2 -> 5
            Assert.IsFalse(new[] { edge16, edge21, edge23, edge24 }.HasCycles());
            // 1 -> 2 -> 4 -> 3 -> 2 -> 5
            Assert.IsTrue(new[] { edge14, edge22, edge21, edge23, edge24 }.HasCycles());
            // 1 -> 4 -> 3 -> 3 -> 2 -> 5
            Assert.IsTrue(new[] { edge16, edge21, edge20, edge23, edge24 }.HasCycles());

            // Edge cases
            var v2Bis = new TestVertex("2");
            var edge25 = Edge.Create(v4, v2Bis);
            // 1 -> 2 -> 4 -> 2Bis
            Assert.IsFalse(new[] { edge14, edge22, edge25 }.HasCycles());

            var equatableV1 = new EquatableTestVertex("1");
            var equatableV2 = new EquatableTestVertex("2");
            var equatableV2Bis = new EquatableTestVertex("2");
            var equatableV4 = new EquatableTestVertex("4");
            var edge26 = Edge.Create(equatableV1, equatableV2);
            var edge27 = Edge.Create(equatableV2, equatableV4);
            var edge28 = Edge.Create(equatableV4, equatableV2Bis);
            // 1 -> 2 -> 4 -> 2Bis
            Assert.IsTrue(new[] { edge26, edge27, edge28 }.HasCycles());
        }

        [Test]
        public void HasCycles_OnlyForPath()
        {
            // The method only work well if given a path
            // This test use edges that are not a path nor has cycle
            var edge14 = Edge.Create(1, 4); 
            var edge21 = Edge.Create(2, 1); 
            var edge43 = Edge.Create(4, 3); 
            Assert.IsTrue(new[] { edge14, edge21, edge43 }.HasCycles());
        }

        [Test]
        public void HasCycles_Throws()
        {
            // ReSharper disable once AssignNullToNotNullAttribute
            // ReSharper disable once ReturnValueOfPureMethodIsNotUsed
            Assert.Throws<ArgumentNullException>(() => ((IEnumerable<IEdge<int>>)null).HasCycles());
        }

        [Test]
        public void IsPathWithoutCycles()
        {
            Assert.IsTrue(Enumerable.Empty<IEdge<int>>().IsPathWithoutCycles());

            var edge1 = Edge.Create(1, 1);
            // 1 -> 1
            Assert.IsFalse(new[] { edge1 }.IsPathWithoutCycles());

            var edge2 = Edge.Create(1, 2);
            // 1 -> 2
            Assert.IsTrue(new[] { edge2 }.IsPathWithoutCycles());

            var edge3 = Edge.Create(2, 1);
            // 1 -> 2 -> 1
            Assert.IsFalse(new[] { edge2, edge3 }.IsPathWithoutCycles());
            // 2 -> 1 -> 2
            Assert.IsFalse(new[] { edge3, edge2 }.IsPathWithoutCycles());

            var edge4 = Edge.Create(1, 4);
            var edge5 = Edge.Create(2, 3);
            var edge6 = Edge.Create(3, 4);
            var edge7 = Edge.Create(3, 1);
            var edge8 = Edge.Create(3, 3);
            var edge9 = Edge.Create(4, 3);
            // 1 -> 2 -> 1 -> 4
            Assert.IsFalse(new[] { edge2, edge3, edge4 }.IsPathWithoutCycles());
            // 2 -> 1 -> 4 -> 3
            Assert.IsTrue(new[] { edge3, edge4, edge9 }.IsPathWithoutCycles());
            // 2 -> 1 -> 4 -> 3 -> 1
            Assert.IsFalse(new[] { edge3, edge4, edge9, edge7 }.IsPathWithoutCycles());
            // 2 -> 3 -> 4 -> 3 -> 3
            Assert.IsFalse(new[] { edge5, edge6, edge9, edge8 }.IsPathWithoutCycles());

            var edge10 = Edge.Create(2, 4);
            var edge11 = Edge.Create(3, 2);
            var edge12 = Edge.Create(2, 5);
            // 1 -> 4 -> 3 -> 2 -> 5
            Assert.IsTrue(new[] { edge4, edge9, edge11, edge12 }.IsPathWithoutCycles());
            // 1 -> 2 -> 4 -> 3 -> 2 -> 5
            Assert.IsFalse(new[] { edge2, edge10, edge9, edge11, edge12 }.IsPathWithoutCycles());
            // 1 -> 4 -> 3 -> 3 -> 2 -> 5
            Assert.IsFalse(new[] { edge4, edge9, edge8, edge11, edge12 }.IsPathWithoutCycles());

            // Not a path: 1 -> 2-4 -> 3
            Assert.IsFalse(new[] { edge2, edge9 }.IsPathWithoutCycles());


            var v1 = new TestVertex("1");
            var v2 = new TestVertex("2");
            var v3 = new TestVertex("3");
            var v4 = new TestVertex("4");
            var v5 = new TestVertex("5");
            var edge13 = Edge.Create(v1, v1);
            // 1 -> 1
            Assert.IsFalse(new[] { edge13 }.IsPathWithoutCycles());

            var edge14 = Edge.Create(v1, v2);
            // 1 -> 2
            Assert.IsTrue(new[] { edge14 }.IsPathWithoutCycles());

            var edge15 = Edge.Create(v2, v1);
            // 1 -> 2 -> 1
            Assert.IsFalse(new[] { edge14, edge15 }.IsPathWithoutCycles());
            // 2 -> 1 -> 2
            Assert.IsFalse(new[] { edge15, edge14 }.IsPathWithoutCycles());

            var edge16 = Edge.Create(v1, v4);
            var edge17 = Edge.Create(v2, v3);
            var edge18 = Edge.Create(v3, v4);
            var edge19 = Edge.Create(v3, v1);
            var edge20 = Edge.Create(v3, v3);
            var edge21 = Edge.Create(v4, v3);
            // 1 -> 2 -> 1 -> 4
            Assert.IsFalse(new[] { edge14, edge15, edge16 }.IsPathWithoutCycles());
            // 2 -> 1 -> 4 -> 3
            Assert.IsTrue(new[] { edge15, edge16, edge21 }.IsPathWithoutCycles());
            // 2 -> 1 -> 4 -> 3 -> 1
            Assert.IsFalse(new[] { edge15, edge16, edge21, edge19 }.IsPathWithoutCycles());
            // 2 -> 3 -> 4 -> 3 -> 3
            Assert.IsFalse(new[] { edge17, edge18, edge21, edge20 }.IsPathWithoutCycles());

            var edge22 = Edge.Create(v2, v4);
            var edge23 = Edge.Create(v3, v2);
            var edge24 = Edge.Create(v2, v5);
            // 1 -> 4 -> 3 -> 2 -> 5
            Assert.IsTrue(new[] { edge16, edge21, edge23, edge24 }.IsPathWithoutCycles());
            // 1 -> 2 -> 4 -> 3 -> 2 -> 5
            Assert.IsFalse(new[] { edge14, edge22, edge21, edge23, edge24 }.IsPathWithoutCycles());
            // 1 -> 4 -> 3 -> 3 -> 2 -> 5
            Assert.IsFalse(new[] { edge16, edge21, edge20, edge23, edge24 }.IsPathWithoutCycles());

            // Edge cases
            var v2Bis = new TestVertex("2");
            var edge25 = Edge.Create(v4, v2Bis);
            // 1 -> 2 -> 4 -> 2Bis
            Assert.IsTrue(new[] { edge14, edge22, edge25 }.IsPathWithoutCycles());

            var equatableV1 = new EquatableTestVertex("1");
            var equatableV2 = new EquatableTestVertex("2");
            var equatableV2Bis = new EquatableTestVertex("2");
            var equatableV4 = new EquatableTestVertex("4");
            var edge26 = Edge.Create(equatableV1, equatableV2);
            var edge27 = Edge.Create(equatableV2, equatableV4);
            var edge28 = Edge.Create(equatableV4, equatableV2Bis);
            // 1 -> 2 -> 4 -> 2Bis
            Assert.IsFalse(new[] { edge26, edge27, edge28 }.IsPathWithoutCycles());
        }

        [Test]
        public void IsPathWithoutCycles_Throws()
        {
            // ReSharper disable once AssignNullToNotNullAttribute
            // ReSharper disable once ReturnValueOfPureMethodIsNotUsed
            Assert.Throws<ArgumentNullException>(() => ((IEnumerable<IEdge<int>>)null).IsPathWithoutCycles());
        }

        [Test]
        public void ToVertexPair()
        {
            var edge1 = Edge.Create(1, 1);
            Assert.AreEqual(
                new SEquatableEdge<int>(1, 1),
                edge1.ToVertexPair());

            var edge2 = Edge.Create(1, 2);
            Assert.AreEqual(
                new SEquatableEdge<int>(1, 2),
                edge2.ToVertexPair());
        }

        [Test]
        public void ToVertexPair_Throws()
        {
            // ReSharper disable once ReturnValueOfPureMethodIsNotUsed
            // ReSharper disable once AssignNullToNotNullAttribute
            Assert.Throws<ArgumentNullException>(
                () => ((Edge<int>)null).ToVertexPair());
        }

        [Test]
        public void IsPredecessor()
        {
            IDictionary<int, IEdge<int>> predecessors = new Dictionary<int, IEdge<int>>();
            Assert.IsFalse(predecessors.IsPredecessor(1, 2));

            predecessors.Add(1, Edge.Create(0, 1));
            Assert.IsFalse(predecessors.IsPredecessor(1, 2));

            predecessors.Add(2, Edge.Create(0, 2));
            Assert.IsFalse(predecessors.IsPredecessor(1, 2));

            predecessors.Add(3, Edge.Create(1, 3));
            predecessors.Add(4, Edge.Create(3, 4));
            predecessors.Add(5, Edge.Create(2, 5));
            Assert.IsFalse(predecessors.IsPredecessor(1, 2));

            predecessors[2] = Edge.Create(1, 2);
            Assert.IsTrue(predecessors.IsPredecessor(1, 2));

            predecessors[2] = Edge.Create(4, 2);
            Assert.IsTrue(predecessors.IsPredecessor(1, 2));

            predecessors[4] = Edge.Create(4, 4);
            Assert.IsFalse(predecessors.IsPredecessor(1, 2));

            Assert.IsTrue(predecessors.IsPredecessor(1, 1));
        }

        [Test]
        public void IsPredecessor_Throws()
        {
            var v1 = new TestVertex("1");
            var v2 = new TestVertex("2");
            // ReSharper disable ReturnValueOfPureMethodIsNotUsed
            // ReSharper disable AssignNullToNotNullAttribute
            Assert.Throws<ArgumentNullException>(
                () => ((IDictionary<TestVertex, IEdge<TestVertex>>)null).IsPredecessor(v1, v2));
            Assert.Throws<ArgumentNullException>(
                () => ((IDictionary<TestVertex, IEdge<TestVertex>>)null).IsPredecessor(null, v2));
            Assert.Throws<ArgumentNullException>(
                () => ((IDictionary<TestVertex, IEdge<TestVertex>>)null).IsPredecessor(v1, null));
            Assert.Throws<ArgumentNullException>(
                () => ((IDictionary<TestVertex, IEdge<TestVertex>>)null).IsPredecessor(null, null));

            IDictionary<TestVertex, IEdge<TestVertex>> predecessors = new Dictionary<TestVertex, IEdge<TestVertex>>();
            Assert.Throws<ArgumentNullException>(
                () => predecessors.IsPredecessor(null, v2));
            Assert.Throws<ArgumentNullException>(
                () => predecessors.IsPredecessor(v1, null));
            Assert.Throws<ArgumentNullException>(
                () => predecessors.IsPredecessor(null, null));
            // ReSharper restore AssignNullToNotNullAttribute
            // ReSharper restore ReturnValueOfPureMethodIsNotUsed
        }

        [Test]
        public void TryGetPath()
        {
            var predecessors = new Dictionary<int, IEdge<int>>();
            Assert.IsNull(predecessors.GetPath(2));

            var edge1 = Edge.Create(0, 1);
            predecessors.Add(1, edge1);
            Assert.IsNull(predecessors.GetPath(2));

            var edge2 = Edge.Create(0, 2);
            predecessors.Add(2, edge2);
            var path = predecessors.GetPath(2);
            CollectionAssert.AreEqual(new[] { edge2 }, path);

            var edge3 = Edge.Create(1, 3);
            var edge4 = Edge.Create(3, 4);
            var edge5 = Edge.Create(2, 5);
            predecessors.Add(3, edge3);
            predecessors.Add(4, edge4);
            predecessors.Add(5, edge5);
            path = predecessors.GetPath(2);
            CollectionAssert.AreEqual(new[] { edge2 }, path);

            var edge6 = Edge.Create(1, 2);
            predecessors[2] = edge6;
            path = predecessors.GetPath(2);
            CollectionAssert.AreEqual(new[] { edge1, edge6 }, path);

            var edge7 = Edge.Create(4, 2);
            predecessors[2] = edge7;
            path = predecessors.GetPath(2);
            CollectionAssert.AreEqual(new[] { edge1, edge3, edge4, edge7 }, path);

            var edge8 = Edge.Create(3, 3);
            predecessors[3] = edge8;
            path = predecessors.GetPath(2);
            CollectionAssert.AreEqual(new[] { edge4, edge7 }, path);
        }

        [Test]
        public void TryGetPath_Throws()
        {
            var v1 = new TestVertex("1");
            // ReSharper disable ReturnValueOfPureMethodIsNotUsed
            // ReSharper disable AssignNullToNotNullAttribute
            Assert.Throws<ArgumentNullException>(() => ((Dictionary<TestVertex, IEdge<TestVertex>>)null).GetPath(v1));
            Assert.Throws<ArgumentNullException>(() => ((Dictionary<TestVertex, IEdge<TestVertex>>)null).GetPath(null));

            var predecessors = new Dictionary<TestVertex, IEdge<TestVertex>>();
            Assert.Throws<ArgumentNullException>(() => predecessors.GetPath(null));
            // ReSharper restore AssignNullToNotNullAttribute
            // ReSharper restore ReturnValueOfPureMethodIsNotUsed
        }

        [Test]
        public void UndirectedVertexEquality()
        {
            var edge11 = Edge.Create(1, 1);
            Assert.IsTrue(edge11.UndirectedVertexEquality(1, 1));
            Assert.IsFalse(edge11.UndirectedVertexEquality(1, 2));
            Assert.IsFalse(edge11.UndirectedVertexEquality(2, 1));
            Assert.IsFalse(edge11.UndirectedVertexEquality(2, 2));

            var edge12 = Edge.Create(1, 2);
            Assert.IsFalse(edge12.UndirectedVertexEquality(1, 1));
            Assert.IsTrue(edge12.UndirectedVertexEquality(1, 2));
            Assert.IsTrue(edge12.UndirectedVertexEquality(2, 1));
            Assert.IsFalse(edge12.UndirectedVertexEquality(2, 2));
        }

        [Test]
        public void UndirectedVertexEquality_Throws()
        {
            var v1 = new TestVertex("1");
            var v2 = new TestVertex("2");
            // ReSharper disable ReturnValueOfPureMethodIsNotUsed
            // ReSharper disable AssignNullToNotNullAttribute
            Assert.Throws<ArgumentNullException>(
                () => ((Edge<TestVertex>)null).UndirectedVertexEquality(v1, v2));
            Assert.Throws<ArgumentNullException>(
                () => ((Edge<TestVertex>)null).UndirectedVertexEquality(null, v2));
            Assert.Throws<ArgumentNullException>(
                () => ((Edge<TestVertex>)null).UndirectedVertexEquality(v1, null));
            Assert.Throws<ArgumentNullException>(
                () => ((Edge<TestVertex>)null).UndirectedVertexEquality(null, null));

            var edge = Edge.Create(v1, v2);
            Assert.Throws<ArgumentNullException>(
                () => edge.UndirectedVertexEquality(null, v2));
            Assert.Throws<ArgumentNullException>(
                () => edge.UndirectedVertexEquality(v1, null));
            Assert.Throws<ArgumentNullException>(
                () => edge.UndirectedVertexEquality(null, null));
            // ReSharper restore AssignNullToNotNullAttribute
            // ReSharper restore ReturnValueOfPureMethodIsNotUsed
        }

        [Test]
        public void SortedVertexEquality()
        {
            var edge11 = Edge.Create(1, 1);
            Assert.IsTrue(edge11.SortedVertexEquality(1, 1));
            Assert.IsFalse(edge11.SortedVertexEquality(1, 2));
            Assert.IsFalse(edge11.SortedVertexEquality(2, 1));
            Assert.IsFalse(edge11.SortedVertexEquality(2, 2));

            var edge12 = Edge.Create(1, 2);
            Assert.IsFalse(edge12.SortedVertexEquality(1, 1));
            Assert.IsTrue(edge12.SortedVertexEquality(1, 2));
            Assert.IsFalse(edge12.SortedVertexEquality(2, 1));
            Assert.IsFalse(edge12.SortedVertexEquality(2, 2));
        }

        [Test]
        public void SortedVertexEquality_Throws()
        {
            var v1 = new TestVertex("1");
            var v2 = new TestVertex("2");
            // ReSharper disable ReturnValueOfPureMethodIsNotUsed
            // ReSharper disable AssignNullToNotNullAttribute
            Assert.Throws<ArgumentNullException>(
                () => ((Edge<TestVertex>)null).SortedVertexEquality(v1, v2));
            Assert.Throws<ArgumentNullException>(
                () => ((Edge<TestVertex>)null).SortedVertexEquality(null, v2));
            Assert.Throws<ArgumentNullException>(
                () => ((Edge<TestVertex>)null).SortedVertexEquality(v1, null));
            Assert.Throws<ArgumentNullException>(
                () => ((Edge<TestVertex>)null).SortedVertexEquality(null, null));

            var edge = Edge.Create(v1, v2);
            Assert.Throws<ArgumentNullException>(
                () => edge.SortedVertexEquality(null, v2));
            Assert.Throws<ArgumentNullException>(
                () => edge.SortedVertexEquality(v1, null));
            Assert.Throws<ArgumentNullException>(
                () => edge.SortedVertexEquality(null, null));
            // ReSharper restore AssignNullToNotNullAttribute
            // ReSharper restore ReturnValueOfPureMethodIsNotUsed
        }

        [Test]
        public void ReverseEdges()
        {
            CollectionAssert.IsEmpty(Enumerable.Empty<IEdge<int>>().ReverseEdges<int, IEdge<int>>());

            var edge1 = Edge.Create(1, 2);
            CollectionAssert.AreEqual(
                new[] { new SReversedEdge<int, IEdge<int>>(edge1) },
                new[] { edge1 }.ReverseEdges<int, IEdge<int>>());

            var edge2 = Edge.Create(2, 2);
            var edge3 = Edge.Create(3, 1);
            CollectionAssert.AreEqual(
                new SReversedEdge<int, IEdge<int>>[]
                {
                    new (edge1),
                    new (edge2),
                    new (edge3)
                },
                new[] { edge1, edge2, edge3 }.ReverseEdges<int, IEdge<int>>());
        }

        [Test]
        public void ReverseEdges_Throws()
        {
            // ReSharper disable once ReturnValueOfPureMethodIsNotUsed
            // ReSharper disable once AssignNullToNotNullAttribute
            Assert.Throws<ArgumentNullException>(
                () => EdgeExtensions.ReverseEdges<int, IEdge<int>>(null));
        }
    }
}