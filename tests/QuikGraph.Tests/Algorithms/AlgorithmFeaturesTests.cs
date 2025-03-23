using System;
#if SUPPORTS_TASKS
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using JetBrains.Annotations;
#endif
using NUnit.Framework;
using QuikGraph.Algorithms;

namespace QuikGraph.Tests.Algorithms
{
    /// <summary>
    /// Tests related to algorithm features (state, services).
    /// </summary>
    [TestFixture]
    internal sealed class AlgorithmFeaturesTests
    {
        #region Test classes

        private class TestAlgorithm : AlgorithmBase<AdjacencyGraph<int, IEdge<int>>>
        {
            public TestAlgorithm()
                : base(new AdjacencyGraph<int, IEdge<int>>())
            {
            }

            protected override void InternalCompute()
            {
            }

            protected override bool TryGetService(Type serviceType, out object service)
            {
                if (serviceType == typeof(TestService))
                {
                    service = new TestService();
                    return true;
                }

                if (serviceType == typeof(TestNullService))
                {
                    // ReSharper disable once AssignNullToNotNullAttribute
                    return base.TryGetService(null, out service);
                }

                return base.TryGetService(serviceType, out service);
            }
        }

#if SUPPORTS_TASKS
        private static readonly TimeSpan TimeoutDelay = TimeSpan.FromSeconds(5);

        private class ManageableTestAlgorithm : AlgorithmBase<AdjacencyGraph<int, IEdge<int>>>
        {
            [NotNull]
            public ManualResetEvent InitializeEvent { get; }
            [NotNull]
            public ManualResetEvent InitializedEvent { get; } = new ManualResetEvent(false);

            [NotNull]
            public ManualResetEvent ComputeEvent { get; }
            [NotNull]
            public ManualResetEvent ComputedEvent { get; } = new ManualResetEvent(false);

            [NotNull]
            public ManualResetEvent CleanEvent { get; }
            [NotNull]
            public ManualResetEvent CleanedEvent { get; } = new ManualResetEvent(false);

            public ManageableTestAlgorithm(
                [NotNull] ManualResetEvent initialize,
                [NotNull] ManualResetEvent compute,
                [NotNull] ManualResetEvent clean)
                : base(new AdjacencyGraph<int, IEdge<int>>())
            {
                InitializeEvent = initialize;
                ComputeEvent = compute;
                CleanEvent = clean;
            }

            protected override void Initialize()
            {
                InitializeEvent.Set();
                InitializedEvent.WaitOne(TimeoutDelay);
                ThrowIfCancellationRequested();
            }

            protected override void InternalCompute()
            {
                ComputeEvent.Set();
                ComputedEvent.WaitOne(TimeoutDelay);
                ThrowIfCancellationRequested();
            }

            protected override void Clean()
            {
                CleanEvent.Set();
                CleanedEvent.WaitOne(TimeoutDelay);
            }
        }
#endif

        private class TestService
        {
        }

        private class TestNotInService
        {
        }

        private class TestNullService
        {
        }

        #endregion

#if SUPPORTS_TASKS
        [Test]
        public void AlgorithmNormalStates()
        {
            var initialize = new ManualResetEvent(false);
            var compute = new ManualResetEvent(false);
            var clean = new ManualResetEvent(false);
            var finished = new ManualResetEvent(false);

            var algorithm = new ManageableTestAlgorithm(initialize, compute, clean);

            bool hasStarted = false;
            bool hasFinished = false;
            var expectedStates = new Queue<ComputationState>();
            expectedStates.Enqueue(ComputationState.Running);
            expectedStates.Enqueue(ComputationState.Finished);

            algorithm.Started += (_, _) =>
            {
                if (hasStarted)
                    Assert.Fail($"{nameof(AlgorithmBase<object>.Started)} event called twice.");
                hasStarted = true;
            };
            algorithm.Finished += (_, _) =>
            {
                if (hasFinished)
                    Assert.Fail($"{nameof(AlgorithmBase<object>.Finished)} event called twice.");
                hasFinished = true;
                finished.Set();
            };
            algorithm.StateChanged += (_, _) =>
            {
                Assert.AreEqual(expectedStates.Peek(), algorithm.State);
                expectedStates.Dequeue();
            };
            algorithm.Aborted += (_, _) =>
            {
                Assert.Fail($"{nameof(AlgorithmBase<object>.Aborted)} event called.");
            };

            Assert.AreEqual(ComputationState.NotRunning, algorithm.State);

            // Run the algorithm
            Task.Run(() =>
            {
                Assert.DoesNotThrow(algorithm.Compute);
            });

            initialize.WaitOne(TimeoutDelay);
            Assert.AreEqual(ComputationState.Running, algorithm.State);
            Assert.IsTrue(hasStarted);

            algorithm.InitializedEvent.Set();

            compute.WaitOne(TimeoutDelay);
            Assert.AreEqual(ComputationState.Running, algorithm.State);

            algorithm.ComputedEvent.Set();

            clean.WaitOne(TimeoutDelay);
            Assert.AreEqual(ComputationState.Running, algorithm.State);

            algorithm.CleanedEvent.Set();

            finished.WaitOne(TimeoutDelay);
            Assert.AreEqual(ComputationState.Finished, algorithm.State);
            Assert.IsTrue(hasFinished);
            Assert.IsTrue(expectedStates.Count == 0);
        }

        [Test]
        public void AlgorithmStates_AbortBeforeStart()
        {
            var initialize = new ManualResetEvent(true);
            var compute = new ManualResetEvent(true);
            var clean = new ManualResetEvent(true);
            var end = new ManualResetEvent(false);

            var algorithm = new ManageableTestAlgorithm(initialize, compute, clean);

            algorithm.Started += (_, _) =>
            {
                Assert.Fail($"{nameof(AlgorithmBase<object>.Started)} event called.");
            };
            algorithm.Finished += (_, _) =>
            {
                Assert.Fail($"{nameof(AlgorithmBase<object>.Finished)} event called.");
            };
            algorithm.StateChanged += (_, _) =>
            {
                Assert.Fail($"{nameof(AlgorithmBase<object>.StateChanged)} event called.");
            };
            algorithm.Aborted += (_, _) =>
            {
                Assert.Fail($"{nameof(AlgorithmBase<object>.Aborted)} event called.");
            };

            Assert.AreEqual(ComputationState.NotRunning, algorithm.State);

            // Abort the algorithm
            Task.Run(() =>
            {
                Assert.DoesNotThrow(() =>
                {
                    algorithm.Abort();
                    Assert.AreEqual(ComputationState.NotRunning, algorithm.State);
                    end.Set();
                });
            });

            end.WaitOne(TimeoutDelay);
        }

        [Test]
        public void AlgorithmStates_AbortDuringRun()
        {
            var initialize = new ManualResetEvent(false);
            var compute = new ManualResetEvent(false);
            var clean = new ManualResetEvent(false);
            var aborted = new ManualResetEvent(false);

            var algorithm = new ManageableTestAlgorithm(initialize, compute, clean);

            bool hasStarted = false;
            bool hasAborted = false;
            var expectedStates = new Queue<ComputationState>();
            expectedStates.Enqueue(ComputationState.Running);
            expectedStates.Enqueue(ComputationState.PendingAbortion);
            expectedStates.Enqueue(ComputationState.Aborted);

            algorithm.Started += (_, _) =>
            {
                if (hasStarted)
                    Assert.Fail($"{nameof(AlgorithmBase<object>.Started)} event called twice.");
                hasStarted = true;
            };
            algorithm.Finished += (_, _) =>
            {
                Assert.Fail($"{nameof(AlgorithmBase<object>.Finished)} event called.");
            };
            algorithm.StateChanged += (_, _) =>
            {
                Assert.AreEqual(expectedStates.Peek(), algorithm.State);
                expectedStates.Dequeue();
            };
            algorithm.Aborted += (_, _) =>
            {
                if (hasAborted)
                    Assert.Fail($"{nameof(AlgorithmBase<object>.Aborted)} event called twice.");
                hasAborted = true;
                aborted.Set();
            };

            Assert.AreEqual(ComputationState.NotRunning, algorithm.State);

            // Run the algorithm
            Task.Run(() =>
            {
                Assert.DoesNotThrow(algorithm.Compute);
            });

            initialize.WaitOne(TimeoutDelay);
            Assert.AreEqual(ComputationState.Running, algorithm.State);
            Assert.IsTrue(hasStarted);

            algorithm.InitializedEvent.Set();

            compute.WaitOne(TimeoutDelay);
            Assert.AreEqual(ComputationState.Running, algorithm.State);

            algorithm.Abort();
            algorithm.ComputedEvent.Set();

            Assert.AreEqual(ComputationState.PendingAbortion, algorithm.State);

            algorithm.CleanedEvent.Set();

            aborted.WaitOne(TimeoutDelay);
            Assert.AreEqual(ComputationState.Aborted, algorithm.State);
            Assert.IsTrue(hasAborted);
            Assert.IsTrue(expectedStates.Count == 0);
        }

        [Test]
        public void AlgorithmStates_CancelDuringRun()
        {
            var initialize = new ManualResetEvent(false);
            var compute = new ManualResetEvent(false);
            var clean = new ManualResetEvent(false);
            var finished = new ManualResetEvent(false);

            var algorithm = new ManageableTestAlgorithm(initialize, compute, clean);

            bool hasStarted = false;
            bool hasFinished = false;
            var expectedStates = new Queue<ComputationState>();
            expectedStates.Enqueue(ComputationState.Running);
            expectedStates.Enqueue(ComputationState.Finished);

            algorithm.Started += (_, _) =>
            {
                if (hasStarted)
                    Assert.Fail($"{nameof(AlgorithmBase<object>.Started)} event called twice.");
                hasStarted = true;
            };
            algorithm.Finished += (_, _) =>
            {
                if (hasFinished)
                    Assert.Fail($"{nameof(AlgorithmBase<object>.Finished)} event called twice.");
                hasFinished = true;
                finished.Set();
            };
            algorithm.StateChanged += (_, _) =>
            {
                Assert.AreEqual(expectedStates.Peek(), algorithm.State);
                expectedStates.Dequeue();
            };
            algorithm.Aborted += (_, _) =>
            {
                Assert.Fail($"{nameof(AlgorithmBase<object>.Aborted)} event called.");
            };

            Assert.AreEqual(ComputationState.NotRunning, algorithm.State);

            // Run the algorithm
            Task.Run(() =>
            {
                Assert.DoesNotThrow(algorithm.Compute);
            });

            initialize.WaitOne(TimeoutDelay);
            Assert.AreEqual(ComputationState.Running, algorithm.State);
            Assert.IsTrue(hasStarted);

            algorithm.InitializedEvent.Set();

            compute.WaitOne(TimeoutDelay);
            Assert.AreEqual(ComputationState.Running, algorithm.State);

            algorithm.Services.CancelManager.Cancel();
            algorithm.Services.CancelManager.ResetCancel(); // These calls don't change algorithm state
            algorithm.Services.CancelManager.Cancel();
            algorithm.ComputedEvent.Set();

            Assert.AreEqual(ComputationState.Running, algorithm.State);

            algorithm.CleanedEvent.Set();

            finished.WaitOne(TimeoutDelay);
            Assert.AreEqual(ComputationState.Finished, algorithm.State);
            Assert.IsTrue(hasFinished);
            Assert.IsTrue(expectedStates.Count == 0);
        }
#endif

        [Test]
        public void GetService()
        {
            var algorithm = new TestAlgorithm();
            var service = algorithm.GetService<TestService>();
            Assert.IsInstanceOf<TestService>(service);
        }

        [Test]
        public void GetService_Throws()
        {
            var algorithm = new TestAlgorithm();
            // ReSharper disable ReturnValueOfPureMethodIsNotUsed
            Assert.Throws<ArgumentNullException>(() => algorithm.GetService<TestNullService>());
            Assert.Throws<InvalidOperationException>(() => algorithm.GetService<TestNotInService>());
            // ReSharper restore ReturnValueOfPureMethodIsNotUsed
        }

        [Test]
        public void TryGetService()
        {
            var algorithm = new TestAlgorithm();
            Assert.IsTrue(algorithm.TryGetService(out TestService service));
            Assert.IsInstanceOf<TestService>(service);

            Assert.IsFalse(algorithm.TryGetService<TestNotInService>(out _));
        }
    }
}