using NUnit.Framework;
using QuikGraph.Graphviz.Dot;

namespace QuikGraph.Graphviz.Tests
{
    /// <summary>
    /// Tests for <see cref="GraphvizColor"/>.
    /// </summary>
    [TestFixture]
    internal sealed class GraphvizColorTests
    {
        #region Test classes

        private class TestClass
        {
        }

        #endregion
        
        [Test]
        public void Constructor()
        {
            var color = default(GraphvizColor);
            CheckColor(color, 0, 0, 0, 0);

            color = new GraphvizColor(250, 255, 125, 45);
            CheckColor(color, 250, 255, 125, 45);

            color = new GraphvizColor(125, 12, 25, 42);
            CheckColor(color, 125, 12, 25, 42);

            #region Local function

            void CheckColor(GraphvizColor c, byte a, byte r, byte g, byte b)
            {
                Assert.AreEqual(a, c.A);
                Assert.AreEqual(r, c.R);
                Assert.AreEqual(g, c.G);
                Assert.AreEqual(b, c.B);
            }

            #endregion
        }

        [Test]
        public void Equals()
        {
            var color1 = new GraphvizColor(255, 150, 160, 25);
            var color2 = new GraphvizColor(255, 150, 160, 25);
            var color3 = new GraphvizColor(125, 150, 160, 25);
            var color4 = new GraphvizColor(255, 155, 160, 25);
            var color5 = new GraphvizColor(255, 150, 120, 25);
            var color6 = new GraphvizColor(255, 150, 160, 30);

            Assert.AreEqual(color1, color1);

            Assert.IsTrue(color1 == color2);
            Assert.IsTrue(color1.Equals(color2));
            Assert.IsTrue(color1.Equals((object)color2));
            Assert.IsFalse(color1 != color2);
            Assert.AreEqual(color1, color2);

            Assert.IsFalse(color1 == color3);
            Assert.IsFalse(color1.Equals(color3));
            Assert.IsFalse(color1.Equals((object)color3));
            Assert.IsTrue(color1 != color3);
            Assert.AreNotEqual(color1, color3);

            Assert.IsFalse(color1 == color4);
            Assert.IsFalse(color1.Equals(color4));
            Assert.IsFalse(color1.Equals((object)color4));
            Assert.IsTrue(color1 != color4);
            Assert.AreNotEqual(color1, color4);

            Assert.IsFalse(color1 == color5);
            Assert.IsFalse(color1.Equals(color5));
            Assert.IsFalse(color1.Equals((object)color5));
            Assert.IsTrue(color1 != color5);
            Assert.AreNotEqual(color1, color5);

            Assert.IsFalse(color1 == color6);
            Assert.IsFalse(color1.Equals(color6));
            Assert.IsFalse(color1.Equals((object)color6));
            Assert.IsTrue(color1 != color6);
            Assert.AreNotEqual(color1, color6);

            Assert.AreNotEqual(null, color1);
            Assert.IsFalse(color1.Equals(null));
            Assert.AreNotEqual(new TestClass(), color1);
            Assert.AreNotEqual(color1, new TestClass());
        }

        [Test]
        public void HashCode()
        {
            var color1 = new GraphvizColor();
            var color2 = new GraphvizColor();
            var color3 = new GraphvizColor(125, 150, 160, 25);
            var color4 = new GraphvizColor(125, 150, 160, 25);

            Assert.AreEqual(color1.GetHashCode(), color2.GetHashCode());
            Assert.AreNotEqual(color1.GetHashCode(), color3.GetHashCode());
            Assert.AreEqual(color3.GetHashCode(), color4.GetHashCode());
        }
    }
}