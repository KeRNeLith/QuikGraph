using NUnit.Framework;
using QuikGraph.Algorithms.TSP;

namespace QuikGraph.Tests.Algorithms.TSP
{
    /// <summary>
    /// Tests for <see cref="TaskPriority"/>.
    /// </summary>
    [TestFixture]
    internal sealed class TaskPriorityTests
    {
        [Test]
        public void Constructor()
        {
            // ReSharper disable once ObjectCreationAsStatement
            Assert.DoesNotThrow(() => new TaskPriority(10.0, 5));
        }

        [Test]
        public void Equals()
        {
            var priority1 = new TaskPriority(1.0, 2);
            var priority2 = new TaskPriority(1.0, 2);
            var priority3 = new TaskPriority(2.0, 2);
            var priority4 = new TaskPriority(1.0, 1);
            var priority5 = new TaskPriority(2.0, 1);

            Assert.AreEqual(priority1, priority1);
            Assert.AreEqual(priority1, priority2);
            Assert.IsTrue(priority1 == priority2);
            Assert.IsTrue(priority2 == priority1);
            Assert.IsFalse(priority1 != priority2);
            Assert.IsFalse(priority2 != priority1);

            Assert.AreNotEqual(priority1, priority3);
            Assert.IsFalse(priority1 == priority3);
            Assert.IsFalse(priority3 == priority1);
            Assert.IsTrue(priority1 != priority3);
            Assert.IsTrue(priority3 != priority1);

            Assert.AreNotEqual(priority1, priority4);
            Assert.IsFalse(priority1 == priority4);
            Assert.IsFalse(priority4 == priority1);
            Assert.IsTrue(priority1 != priority4);
            Assert.IsTrue(priority4 != priority1);

            Assert.AreNotEqual(priority1, priority5);
            Assert.IsFalse(priority1 == priority5);
            Assert.IsFalse(priority5 == priority1);
            Assert.IsTrue(priority1 != priority5);
            Assert.IsTrue(priority5 != priority1);

            Assert.AreNotEqual(null, priority1);
            Assert.IsFalse(priority1.Equals(null));
            Assert.IsFalse(priority1 == null);
            Assert.IsFalse(null == priority1);
            Assert.IsTrue(priority1 != null);
            Assert.IsTrue(null != priority1);
        }

        [Test]
        public void Hashcode()
        {
            var priority1 = new TaskPriority(1.0, 2);
            var priority2 = new TaskPriority(1.0, 2);
            var priority3 = new TaskPriority(2.0, 2);

            Assert.AreEqual(priority1.GetHashCode(), priority2.GetHashCode());
            Assert.AreNotEqual(priority1.GetHashCode(), priority3.GetHashCode());
        }

        [Test]
        public void Comparison()
        {
            var priority12A = new TaskPriority(1.0, 2);

            Assert.IsTrue(priority12A >= null);
            Assert.IsTrue(priority12A > null);

            Assert.IsFalse(priority12A < null);
            Assert.IsFalse(priority12A <= null);

            var priority12B = new TaskPriority(1.0, 2);

            Assert.IsTrue(priority12A <= priority12B);
            Assert.IsTrue(priority12A >= priority12B);

            Assert.IsFalse(priority12A < priority12B);
            Assert.IsFalse(priority12A > priority12B);

            var priority20 = new TaskPriority(2.0, 2);

            Assert.IsTrue(priority12A < priority20);
            Assert.IsTrue(priority12A <= priority20);

            Assert.IsFalse(priority12A > priority20);
            Assert.IsFalse(priority12A >= priority20);

            var priority10 = new TaskPriority(1.0, 1);

            Assert.IsTrue(priority12A < priority10);
            Assert.IsTrue(priority12A <= priority10);

            Assert.IsFalse(priority12A > priority10);
            Assert.IsFalse(priority12A >= priority10);

        }
    }
}