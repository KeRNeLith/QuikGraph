using System;
using System.Diagnostics;
using JetBrains.Annotations;

namespace QuikGraph.Utils
{
    /// <summary> Helpers to work with <see cref="IDisposable"/>. </summary>
    public static class DisposableHelpers
    {
        /// <summary> Calls the <paramref name="action"/> when going out of scope. </summary>
        /// <returns>An <see cref="IDisposable"/> object calling <paramref name="action"/> to give to a using clause.</returns>
        [Pure]
        [NotNull]
        public static IDisposable Finally([NotNull] Action action) => new FinallyScope(action);

        private struct FinallyScope : IDisposable
        {
            private Action _action;

            public FinallyScope([NotNull] Action action)
            {
                Debug.Assert(action != null);

                _action = action;
            }

            /// <inheritdoc />
            public void Dispose()
            {
                _action();
                _action = null;
            }
        }
    }
}