#if SUPPORTS_BINARY_GRAPH_SERIALIZATION
using System;
using System.Collections.Concurrent;
using System.Diagnostics;
using JetBrains.Annotations;
using QuikGraph.Collections;
using QuikGraph.Serialization.Collections;
using QuikGraph.Serialization.Formatters;

namespace QuikGraph.Serialization
{
    /// <summary>
    /// QuikGraph MessagePack formatter resolver cache.
    /// </summary>
    internal static class FormatterResolverCache
    {
        private struct Mapping
        {
            public Mapping([NotNull] Type objectType, [NotNull] Type formatterType)
            {
                ObjectType = objectType;
                FormatterType = formatterType;
            }

            [NotNull]
            public Type ObjectType { get; }

            [NotNull]
            public Type FormatterType { get; }
        }

        // Order is important for formatter resolution
        [NotNull]
        private static readonly Mapping[] FormatterMappings =
        {
            // Edges
            new Mapping(typeof(EquatableTaggedEdge<,>), typeof(EquatableTaggedEdgeFormatter<,>)),
            new Mapping(typeof(EquatableEdge<>), typeof(EquatableEdgeFormatter<>)),
            new Mapping(typeof(TaggedEdge<,>), typeof(TaggedEdgeFormatter<,>)),
            new Mapping(typeof(EquatableTermEdge<>), typeof(EquatableTermEdgeFormatter<>)),
            new Mapping(typeof(TermEdge<>), typeof(TermEdgeFormatter<>)),
            new Mapping(typeof(Edge<>), typeof(EdgeFormatter<>)),
            new Mapping(typeof(SEquatableTaggedEdge<,>), typeof(SEquatableTaggedEdgeFormatter<,>)),
            new Mapping(typeof(SEquatableEdge<>), typeof(SEquatableEdgeFormatter<>)),
            new Mapping(typeof(STaggedEdge<,>), typeof(STaggedEdgeFormatter<,>)),
            new Mapping(typeof(SEdge<>), typeof(SEdgeFormatter<>)),
            new Mapping(typeof(SReversedEdge<,>), typeof(SReversedEdgeFormatter<,>)),
            new Mapping(typeof(TaggedUndirectedEdge<,>), typeof(TaggedUndirectedEdgeFormatter<,>)),
            new Mapping(typeof(EquatableUndirectedEdge<>), typeof(EquatableUndirectedEdgeFormatter<>)),
            new Mapping(typeof(UndirectedEdge<>), typeof(UndirectedEdgeFormatter<>)),
            new Mapping(typeof(STaggedUndirectedEdge<,>), typeof(STaggedUndirectedEdgeFormatter<,>)),
            new Mapping(typeof(SUndirectedEdge<>), typeof(SUndirectedEdgeFormatter<>)),

            // Collections
            new Mapping(typeof(SerializableArray<>), typeof(SerializableArrayFormatter<>)),
            new Mapping(typeof(VertexList<>), typeof(VertexListFormatter<>)),
            new Mapping(typeof(EdgeList<,>), typeof(EdgeListFormatter<,>)),
            new Mapping(typeof(SerializableDictionary<,>), typeof(SerializableDictionaryFormatter<,>)),
            new Mapping(typeof(VertexEdgeDictionary<,>), typeof(VertexEdgeDictionaryFormatter<,>)),
            new Mapping(typeof(EdgeEdgeDictionary<,>), typeof(EdgeEdgeDictionaryFormatter<,>)),
        };

        [NotNull]
        private static readonly ConcurrentDictionary<Type, object> FormattersCache = new ConcurrentDictionary<Type, object>();

        [CanBeNull]
        internal static object GetFormatter([NotNull] Type objectType)
        {
            if (FormattersCache.TryGetValue(objectType, out object formatter))
                return formatter;

            foreach (Mapping mapping in FormatterMappings)
            {
                if (objectType.IsAssignableToGenericType(mapping.ObjectType))
                {
                    Type formatterType = mapping.FormatterType.MakeGenericType(objectType.GenericTypeArguments);
                    Debug.Assert(!formatterType.IsGenericTypeDefinition);
                    formatter = Activator.CreateInstance(formatterType);
                    FormattersCache[objectType] = formatter;
                    return formatter;
                }
            }

            // If type can not get, must return null for fallback mechanism
            return null;
        }

        #region Helpers

        [Pure]
        private static bool IsAssignableToGenericType([NotNull] this Type type, [NotNull] Type genericType)
        {
            while (true)
            {
                if (type is null || type == typeof(object))
                    return false;

                if (type == genericType)
                    return true;

                if ((type.IsGenericType ? type.GetGenericTypeDefinition() : type) == genericType)
                    return true;

                if (genericType.IsInterface)
                {
                    foreach (Type interfaceType in type.GetInterfaces())
                    {
                        if (interfaceType.IsAssignableToGenericType(genericType))
                            return true;
                    }
                }

                type = type.BaseType;
            }
        }

        #endregion
    }
}
#endif