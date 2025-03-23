#if SUPPORTS_SERIALIZATION
using System;
using JetBrains.Annotations;
using NUnit.Framework;
using static QuikGraph.Tests.SerializationTestHelpers;

namespace QuikGraph.Tests.Exceptions
{
    /// <summary>
    /// Tests for exceptions.
    /// </summary>
    [TestFixture]
    internal sealed class ExceptionTests
    {
        private static void ExceptionConstructorTest<TException>(
            [NotNull, InstantHandle] Func<string, Exception, TException> createException)
            where TException : Exception
        {
            const string message = "Test exception message.";
            var innerException = new Exception("Inner");

            Exception exception = createException(message, innerException);
            Assert.AreEqual(message, exception.Message);
            Assert.AreSame(innerException, exception.InnerException);
            
            exception = createException(message, null);
            Assert.AreEqual(message, exception.Message);
            Assert.IsNull(exception.InnerException);
        }

        [Test]
        public void ExceptionsConstructor()
        {
            ExceptionConstructorTest((m, e) => new NegativeCapacityException(m, e));
            ExceptionConstructorTest((m, e) => new NegativeCycleGraphException(m, e));
            ExceptionConstructorTest((m, e) => new NegativeWeightException(m, e));
            ExceptionConstructorTest((m, e) => new CyclicGraphException(m, e));
            ExceptionConstructorTest((m, e) => new NonStronglyConnectedGraphException(m, e));
            ExceptionConstructorTest((m, e) => new NoPathFoundException(m, e));
            ExceptionConstructorTest((m, e) => new ParallelEdgeNotAllowedException(m, e));
            //ExceptionConstructorTest((m, e) => new VertexNotFoundException(m, e));
        }

        private static void ExceptionSerializationTest<TException>(
            [NotNull, InstantHandle] Func<TException> createException)
            where TException : Exception
        {
            Exception exception = createException();

            // Save the full ToString() value, including the exception message and stack trace.
            string exceptionToString = exception.ToString();

            Exception deserializedException = SerializeAndDeserialize(exception);

            // Double-check that the exception message and stack trace (owned by the base Exception) are preserved
            Assert.AreNotSame(exception, deserializedException);
            Assert.AreEqual(exceptionToString, deserializedException.ToString());
        }

        [Test]
        public void ExceptionsSerialization()
        {
            ExceptionSerializationTest(() => new NegativeCapacityException());
            ExceptionSerializationTest(() => new NegativeCycleGraphException());
            ExceptionSerializationTest(() => new NegativeWeightException());
            ExceptionSerializationTest(() => new CyclicGraphException());
            ExceptionSerializationTest(() => new NonStronglyConnectedGraphException());
            ExceptionSerializationTest(() => new NoPathFoundException());
            ExceptionSerializationTest(() => new ParallelEdgeNotAllowedException());
            //ExceptionSerializationTest(() => new VertexNotFoundException());
        }
    }
}
#endif