using System;
using NUnit.Framework;
using static QuikGraph.Tests.GraphTestHelpers;

namespace QuikGraph.Petri.Tests
{
    /// <summary>
    /// Tests related to <see cref="PetriNet{TToken}"/>.
    /// </summary>
    internal sealed class PetriNetTests
    {
        [Test]
        public void Constructor()
        {
            var net = new PetriNet<int>();
            CollectionAssert.IsEmpty(net.Places);
            CollectionAssert.IsEmpty(net.Transitions);
            CollectionAssert.IsEmpty(net.Arcs);
            Assert.IsNotNull(net.Graph);
            AssertEmptyGraph(net.Graph);
        }

        [Test]
        public void MutableNetContent()
        {
            var net = new PetriNet<int>();
            var place1 = net.AddPlace("P1");
            CollectionAssert.AreEquivalent(new[] { place1 }, net.Places);
            CollectionAssert.IsEmpty(net.Transitions);
            CollectionAssert.IsEmpty(net.Arcs);
            net.Graph.AssertHasVertices(place1);
            net.Graph.AssertNoEdge();

            var transition1 = net.AddTransition("T1");
            CollectionAssert.AreEquivalent(new[] { place1 }, net.Places);
            CollectionAssert.AreEquivalent(new[] { transition1 }, net.Transitions);
            CollectionAssert.IsEmpty(net.Arcs);
            net.Graph.AssertHasVertices(place1, transition1);
            net.Graph.AssertNoEdge();

            var place2 = net.AddPlace("P2");
            var transition2 = net.AddTransition("T2");
            var place3 = net.AddPlace("P3");
            CollectionAssert.AreEquivalent(new[] { place1, place2, place3 }, net.Places);
            CollectionAssert.AreEquivalent(new[] { transition1, transition2 }, net.Transitions);
            CollectionAssert.IsEmpty(net.Arcs);
            net.Graph.AssertHasVertices(place1, place2, place3, transition1, transition2);
            net.Graph.AssertNoEdge();

            var arc1 = net.AddArc(place1, transition1);
            CollectionAssert.AreEquivalent(new[] { place1, place2, place3 }, net.Places);
            CollectionAssert.AreEquivalent(new[] { transition1, transition2 }, net.Transitions);
            CollectionAssert.AreEquivalent(new[] { arc1 }, net.Arcs);
            net.Graph.AssertHasVertices(place1, place2, place3, transition1, transition2);
            net.Graph.AssertHasEdges(arc1);

            var arc2 = net.AddArc(transition2, place3);
            var arc3 = net.AddArc(place2, transition2);
            CollectionAssert.AreEquivalent(new[] { place1, place2, place3 }, net.Places);
            CollectionAssert.AreEquivalent(new[] { transition1, transition2 }, net.Transitions);
            CollectionAssert.AreEquivalent(new[] { arc1, arc2, arc3 }, net.Arcs);
            net.Graph.AssertHasVertices(place1, place2, place3, transition1, transition2);
            net.Graph.AssertHasEdges(arc1, arc2, arc3);
        }

        [Test]
        public void Clone()
        {
            var net = new PetriNet<int>();
            AssertEmpty(net);

            var clonedNet = net.Clone();
            Assert.IsNotNull(clonedNet);
            AssertEmpty(clonedNet);

            clonedNet = (PetriNet<int>)((ICloneable)net).Clone();
            Assert.IsNotNull(clonedNet);
            AssertEmpty(clonedNet);

            var place1 = net.AddPlace("p1");
            var place2 = net.AddPlace("p2");
            var transition1 = net.AddTransition("t1");
            net.Graph.AssertHasVertices(place1, place2, transition1);
            net.Graph.AssertNoEdge();

            clonedNet = net.Clone();
            Assert.IsNotNull(clonedNet);
            AssertAreEqual(net, clonedNet);

            clonedNet = (PetriNet<int>)((ICloneable)net).Clone();
            Assert.IsNotNull(clonedNet);
            AssertAreEqual(net, clonedNet);

            var place3 = net.AddPlace("p3");
            var transition2 = net.AddTransition("t2");
            var arc1 = net.AddArc(place1, transition1);
            var arc2 = net.AddArc(place2, transition1);
            var arc3 = net.AddArc(place3, transition2);
            var arc4 = net.AddArc(transition1, place3);
            var arc5 = net.AddArc(transition2, place1);
            net.Graph.AssertHasVertices(place1, place2, place3, transition1, transition2);
            net.Graph.AssertHasEdges(arc1, arc2, arc3, arc4, arc5);

            clonedNet = net.Clone();
            Assert.IsNotNull(clonedNet);
            AssertAreEqual(net, clonedNet);

            clonedNet = (PetriNet<int>)((ICloneable)net).Clone();
            Assert.IsNotNull(clonedNet);
            AssertAreEqual(net, clonedNet);

            var place4 = net.AddPlace("p4");
            var transition3 = net.AddTransition("t3");
            net.Graph.AssertHasVertices(place1, place2, place3, place4, transition1, transition2, transition3);
            net.Graph.AssertHasEdges(arc1, arc2, arc3, arc4, arc5);

            clonedNet = net.Clone();
            Assert.IsNotNull(clonedNet);
            AssertAreEqual(net, clonedNet);

            clonedNet = (PetriNet<int>)((ICloneable)net).Clone();
            Assert.IsNotNull(clonedNet);
            AssertAreEqual(net, clonedNet);

            #region Local function

            void AssertEmpty<TToken>(PetriNet<TToken> n)
            {
                CollectionAssert.IsEmpty(n.Places);
                CollectionAssert.IsEmpty(n.Transitions);
                CollectionAssert.IsEmpty(n.Arcs);
                AssertEmptyGraph(n.Graph);
            }

            void AssertAreEqual<TToken>(PetriNet<TToken> expected, PetriNet<TToken> actual)
            {
                CollectionAssert.AreEqual(expected.Places, actual.Places);
                CollectionAssert.AreEqual(expected.Transitions, actual.Transitions);
                CollectionAssert.AreEqual(expected.Arcs, actual.Arcs);
                AssertEquivalentGraphs(expected.Graph, actual.Graph);
            }

            #endregion
        }

        [Test]
        public void ObjectToString()
        {
            var net = new PetriNet<int>();
            string expectedString =
                "-----------------------------------------------" + Environment.NewLine +
                "Places (0)" + Environment.NewLine +
                "Transitions (0)" + Environment.NewLine +
                "Arcs" + Environment.NewLine;
            Assert.AreEqual(expectedString, net.ToString());

            var place1 = net.AddPlace("TestPlace");
            expectedString =
                "-----------------------------------------------" + Environment.NewLine +
                "Places (1)" + Environment.NewLine +
                "\tP(TestPlace|0)" + Environment.NewLine +
                Environment.NewLine +
                "Transitions (0)" + Environment.NewLine +
                "Arcs" + Environment.NewLine;
            Assert.AreEqual(expectedString, net.ToString());

            place1.Marking.Add(1);
            place1.Marking.Add(5);
            var place2 = net.AddPlace("TestPlace2");
            expectedString =
                "-----------------------------------------------" + Environment.NewLine +
                "Places (2)" + Environment.NewLine +
                "\tP(TestPlace|2)" + Environment.NewLine +
                "\tInt32" + Environment.NewLine +
                "\tInt32" + Environment.NewLine +
                Environment.NewLine +
                "\tP(TestPlace2|0)" + Environment.NewLine +
                Environment.NewLine +
                "Transitions (0)" + Environment.NewLine +
                "Arcs" + Environment.NewLine;
            Assert.AreEqual(expectedString, net.ToString());

            var transition = net.AddTransition("Transition");
            expectedString =
                "-----------------------------------------------" + Environment.NewLine +
                "Places (2)" + Environment.NewLine +
                "\tP(TestPlace|2)" + Environment.NewLine +
                "\tInt32" + Environment.NewLine +
                "\tInt32" + Environment.NewLine +
                Environment.NewLine +
                "\tP(TestPlace2|0)" + Environment.NewLine +
                Environment.NewLine +
                "Transitions (1)" + Environment.NewLine +
                "\tT(Transition)" + Environment.NewLine +
                Environment.NewLine +
                "Arcs" + Environment.NewLine;
            Assert.AreEqual(expectedString, net.ToString());

            net.AddArc(place1, transition);
            net.AddArc(transition, place2);
            expectedString =
                "-----------------------------------------------" + Environment.NewLine +
                "Places (2)" + Environment.NewLine +
                "\tP(TestPlace|2)" + Environment.NewLine +
                "\tInt32" + Environment.NewLine +
                "\tInt32" + Environment.NewLine +
                Environment.NewLine +
                "\tP(TestPlace2|0)" + Environment.NewLine +
                Environment.NewLine +
                "Transitions (1)" + Environment.NewLine +
                "\tT(Transition)" + Environment.NewLine +
                Environment.NewLine +
                "Arcs" + Environment.NewLine +
                "\tP(TestPlace|2) -> T(Transition)" + Environment.NewLine +
                "\tT(Transition) -> P(TestPlace2|0)" + Environment.NewLine;
            Assert.AreEqual(expectedString, net.ToString());
        }
    }
}