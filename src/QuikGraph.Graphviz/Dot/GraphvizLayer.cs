using System;
using JetBrains.Annotations;

namespace QuikGraph.Graphviz.Dot
{
    /// <summary>
    /// Graphviz layer.
    /// </summary>
    public class GraphvizLayer
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="GraphvizLayer"/> class.
        /// </summary>
        /// <param name="name">Layer name.</param>
        /// <exception cref="T:System.ArgumentException"><paramref name="name"/> is <see langword="null"/> or empty.</exception>
        public GraphvizLayer([NotNull] string name)
        {
            SetName(name);
        }

        private void SetName([NotNull] string name)
        {
            if (string.IsNullOrEmpty(name))
                throw new ArgumentException("Name cannot be null or empty.", nameof(name));
            _name = name;
        }

        private string _name;

        /// <summary>
        /// Layer name.
        /// </summary>
        /// <exception cref="T:System.ArgumentException">Set value is <see langword="null"/> or empty.</exception>
        public string Name
        {
            get => _name;
            set => SetName(value);
        }
    }
}