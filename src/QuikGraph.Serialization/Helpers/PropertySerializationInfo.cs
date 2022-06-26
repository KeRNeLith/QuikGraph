#if SUPPORTS_GRAPHML_GRAPH_SERIALIZATION
using System.Reflection;
using JetBrains.Annotations;

namespace QuikGraph.Serialization
{
    internal struct PropertySerializationInfo
    {
        /// <summary>
        /// Gets the embedded <see cref="PropertyInfo"/>.
        /// </summary>
        [NotNull]
        public PropertyInfo Property { get; }

        /// <summary>
        /// Gets the property name.
        /// </summary>
        [NotNull]
        public string Name { get; }

        private readonly bool _hasValue;

        [CanBeNull]
        private readonly object _value;

        public PropertySerializationInfo([NotNull] PropertyInfo property, [NotNull] string name)
            : this(property, name, null)
        {
        }

        public PropertySerializationInfo(
            [NotNull] PropertyInfo property,
            [NotNull] string name,
            [CanBeNull] object value)
        {
            Property = property;
            Name = name;
            _value = value;
            _hasValue = _value != null;
        }

        [Pure]
        public bool TryGetDefaultValue(out object value)
        {
            value = _value;
            return _hasValue;
        }
    }
}
#endif