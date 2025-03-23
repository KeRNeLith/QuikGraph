using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using JetBrains.Annotations;
using NUnit.Framework;
using NotNullAttribute = JetBrains.Annotations.NotNullAttribute;

namespace QuikGraph.Tests
{
    /// <summary>
    /// Additional assertion helpers.
    /// </summary>
    internal static class QuikGraphAssert
    {
        /// <summary>
        /// Checks that the <paramref name="onValue"/> predicate is true for all <paramref name="values"/>.
        /// </summary>
        /// <typeparam name="T">Value type.</typeparam>
        /// <param name="values">Values to check.</param>
        /// <param name="onValue">Predicate.</param>
        public static void TrueForAll<T>(
            [NotNull, ItemCanBeNull] IEnumerable<T> values,
            [NotNull, InstantHandle] Predicate<T> onValue)
        {
            foreach (T value in values)
            {
                Assert.IsTrue(onValue(value));
            }
        }

        /// <summary>
        /// Asserts implication is true (if <paramref name="value" /> is true,
        /// <paramref name="impliedValue" /> should hold).
        /// </summary>
        public static void ImpliesIsTrue(bool value, [NotNull, InstantHandle] Func<bool> impliedValue)
        {
            if (!value)
                return;
            Assert.IsTrue(impliedValue());
        }

        /// <summary>
        /// Determines whether two exceptions are same type.
        /// </summary>
        /// <param name="left">An exception object.</param>
        /// <param name="right">An exception object.</param>
        /// <returns>Returns true if <paramref name="left" /> and <paramref name="right" /> have
        /// the same type, otherwise false.</returns>
        private static bool EqualExceptions(Exception left, Exception right)
        {
            Assert.IsNotNull(left);
            Assert.IsNotNull(right);
            return left.GetType() == right.GetType();
        }

        /// <summary>
        /// Contains the exception from executing code, if an exception was thrown.
        /// </summary>
        private struct CatchResult
        {
            /// <summary>
            /// Contains the exception object, if any.
            /// </summary>
            [CanBeNull]
            public Exception Exception { get; }

            /// <summary>
            /// Initializes a new structure.
            /// </summary>
            /// <param name="exception">The exception object.</param>
            public CatchResult([CanBeNull] Exception exception)
            {
                Exception = exception;
            }

            /// <summary>
            /// Indicates whether the structure contains an exception.
            /// </summary>
            public bool HasException => Exception != null;

            /// <summary>
            /// Contains the exception type.
            /// </summary>
            /// <remarks>If the structure does not contain an exception object, this property is null.</remarks>
            public Type ExceptionType => Exception?.GetType();
        }

        /// <summary>
        /// Executes a specified delegate and returns the result.
        /// </summary>
        /// <param name="action">An <see cref="Action"/> delegate that performs a user-defined action.</param>
        /// <returns>Returns a <see cref="CatchResult"/> structure that contains an exception object,
        /// if an exception was thrown.</returns>
        private static CatchResult Catch([NotNull, InstantHandle] Action action)
        {
            try
            {
                action();
                return new CatchResult();
            }
            catch (Exception ex)
            {
                return new CatchResult(ex);
            }
        }

        /// <summary>
        /// Asserts that the observable behavior of two delegates is the same.
        /// </summary>
        /// <overloads>Asserts that the observable behavior of two delegates is the same.</overloads>
        /// <param name="left">An <see cref="Action"/> delegate that performs a user-defined action.</param>
        /// <param name="right">An <see cref="Action"/> delegate that performs a user-defined action.</param>
        /// <remarks>"Same behavior" is defined as both delegates throwing the same exception or neither delegate throwing an exception.</remarks>
        public static void AreBehaviorsEqual(
            [NotNull, InstantHandle] Action left,
            [NotNull, InstantHandle] Action right)
        {
            CatchResult catchResult1 = Catch(left);
            CatchResult catchResult2 = Catch(right);
            if (!catchResult1.HasException)
            {
                Assert.IsTrue(
                    (!catchResult2.HasException ? 1 : 0) != 0,
                    $"returned <> raised '{catchResult2.ExceptionType}'");
            }
            else
            {
                Assert.IsTrue(
                    (catchResult2.HasException ? 1 : 0) != 0,
                    $"raised '{catchResult1.ExceptionType}' <> returned");

                Assert.IsTrue(
                    ((EqualExceptions(catchResult1.Exception, catchResult2.Exception)) ? 1 : 0) != 0,
                    $"raised '{catchResult1.ExceptionType}' <> raised '{catchResult2.ExceptionType}'");
            }
        }

        /// <summary>
        /// Contains either a value returned by executing code or the exception that was thrown.
        /// </summary>
        /// <typeparam name="T">The return type.</typeparam>
        public struct CatchResult<T>
        {
            /// <summary>
            /// Contains the return value, if any.
            /// </summary>
            [CanBeNull]
            public T Value { get; }

            /// <summary>
            /// Contains the exception, if any.
            /// </summary>
            [CanBeNull]
            public Exception Exception { get; }

            /// <summary>
            /// Initializes a new instance of <see cref="CatchResult{T}" />.
            /// </summary>
            /// <param name="value">A return value.</param>
            public CatchResult([CanBeNull] T value)
            {
                Value = value;
                Exception = null;
            }

            /// <summary>
            /// Initializes a new instance of <see cref="CatchResult{T}" />.
            /// </summary>
            /// <param name="exception">An exception object.</param>
            public CatchResult([NotNull] Exception exception)
            {
                Value = default;
                Exception = exception;
            }

            /// <summary>Tries to get the return value.</summary>
            /// <param name="value">Receives the return value. If the structure does not contain a return value,
            /// this parameter is set to <c>default(T)</c>.</param>
            /// <returns>Returns <c>true</c> if the structure contains a return value and<c>false</c> otherwise.</returns>
            [Pure]
            public bool TryGetValue(out T value)
            {
                if (HasValue)
                {
                    value = Value;
                    return true;
                }
                value = default;
                return false;
            }

            /// <summary>Tries to get the exception object.</summary>
            /// <param name="exception">Receives the exception object.If the structure does not contain an exception object,
            /// this parameter is set to <c>null</c>.</param>
            /// <returns>Returns <c>true</c> if the structure contains an exception object and<c>false</c> otherwise.</returns>
            [Pure]
            public bool TryGetException(out Exception exception)
            {
                exception = Exception;
                return exception != null;
            }

            /// <summary>
            /// Indicates whether the structure contains a return value.
            /// </summary>
            public bool HasValue => Exception == null;

            /// <summary>
            /// Indicates whether the structure contains an exception object.
            /// </summary>
            public bool HasException => Exception != null;

            /// <summary>
            /// Contains the exception type, if the structure contains an exception object.
            /// </summary>
            /// <remarks>If the structure does not contain an exception object, this property is null.</remarks>
            public Type ExceptionType => Exception?.GetType();
        }

        /// <summary>
        /// Executes a specified delegate and returns the result.
        /// </summary>
        /// <typeparam name="T">The return type.</typeparam>
        /// <param name="function">An <see cref="Func{T}"/> delegate that performs a user-defined function
        /// and returns a value of type <typeparamref name="T"/>.</param>
        /// <returns>Returns a <see cref="CatchResult"/> structure that contains a return value or,
        /// if an exception was thrown, an exception object.</returns>
        private static CatchResult<T> Catch<T>([NotNull, InstantHandle] Func<T> function)
        {
            try
            {
                return new CatchResult<T>(function());
            }
            catch (Exception ex)
            {
                return new CatchResult<T>(ex);
            }
        }

        /// <summary>
        /// Asserts that the observable behavior of two delegates is the same.
        /// </summary>
        /// <typeparam name="T">The delegates return type.</typeparam>
        /// <param name="left">A <see cref="Func{TResult}"/> delegate that performs a user-defined action and returns a value of type <typeparamref name="T"/>.</param>
        /// <param name="right">A <see cref="Func{TResult}"/> delegate that performs a user-defined action and returns a value of type <typeparamref name="T"/>.</param>
        /// <remarks>"Same behavior" is defined as both delegates returning the same value or both delegates throwing the same exception.</remarks>
        public static void AreBehaviorsEqual<T>(
            [NotNull, InstantHandle] Func<T> left,
            [NotNull, InstantHandle] Func<T> right)
        {
            CatchResult<T> catchResult1 = Catch(left);
            CatchResult<T> catchResult2 = Catch(right);
            if (catchResult1.HasValue)
            {
                Assert.IsTrue(
                    (catchResult2.HasValue ? 1 : 0) != 0,
                    $"result '{catchResult1.Value}' <> raised '{catchResult2.ExceptionType}'");

                Assert.IsTrue(
                    (EqualityComparer<T>.Default.Equals(catchResult1.Value, catchResult2.Value) ? 1 : 0) != 0,
                    $"result '{catchResult1.Value}' <> result '{catchResult2.Value}'");
            }
            else
            {
                Assert.IsTrue(
                    (catchResult2.HasException ? 1 : 0) != 0,
                    $"raised '{catchResult1.ExceptionType}' <> return '{catchResult2.Value}'");

                Assert.IsTrue(
                    (EqualExceptions(catchResult1.Exception, catchResult2.Exception) ? 1 : 0) != 0,
                    $"raised '{catchResult1.ExceptionType}' <> raised '{catchResult2.ExceptionType}'");
            }
        }

        /// <summary>
        /// Enumerates the specified <paramref name="enumerable"/> in a double "for each".
        /// </summary>
        /// <param name="enumerable">Enumerable.</param>
        [SuppressMessage("ReSharper", "PossibleMultipleEnumeration")]
        public static void DoubleForEach<T>(IEnumerable<T> enumerable)
        {
            Assert.IsNotNull(enumerable);
            using (IEnumerator<T> enumerator1 = enumerable.GetEnumerator())
            {
                Assert.IsNotNull(enumerator1);
                using (IEnumerator<T> enumerator2 = enumerable.GetEnumerator())
                {
                    Assert.IsNotNull(enumerator2);
                    Assert.AreEqual(enumerator1.MoveNext(), enumerator2.MoveNext());
                }
            }
        }

        /// <summary>
        /// Enumerates the specified <paramref name="enumerable"/> and reset it at each element.
        /// </summary>
        /// <param name="enumerable">Enumerable.</param>
        public static void MoveNextAndReset<T>(IEnumerable<T> enumerable)
        {
            Assert.IsNotNull(enumerable);
            using (IEnumerator<T> enumerator = enumerable.GetEnumerator())
            {
                Assert.IsNotNull(enumerator);
                bool expected = enumerator.MoveNext();
                try
                {
                    enumerator.Reset();
                }
                catch (NotSupportedException)
                {
                    return;
                }
                bool actual = enumerator.MoveNext();
                Assert.AreEqual(expected, actual);
            }
        }
    }
}