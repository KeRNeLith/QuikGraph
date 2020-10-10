using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using QuikGraph.Algorithms;

namespace QuikGraph.Tests.Algorithms.Contracts
{
    /// <summary>
    /// Provider of algorithm <see cref="Type"/> implementing contracts.
    /// </summary>
    internal static class AlgorithmsProvider
    {
        /// <summary>
        /// Gets all implementations of the given <typeparamref name="T"/> type.
        /// </summary>
        [Pure]
        [NotNull, ItemNotNull]
        private static IEnumerable<Type> GetImplementationsOf<T>()
        {
            return GetImplementationsOf(typeof(T));
        }

        /// <summary>
        /// Gets all implementations of the given <paramref name="targetType"/>.
        /// </summary>
        [Pure]
        [NotNull, ItemNotNull]
        private static IEnumerable<Type> GetImplementationsOf([NotNull] Type targetType)
        {
            IEnumerable<Type> implementations = AppDomain.CurrentDomain.GetAssemblies()
                .SelectMany(assembly => assembly.GetTypes())
                .Where(type => !type.IsInterface && !type.IsAbstract)
                .SelectMany(
                    type => type.GetInterfaces(),
                    (type, inheritedOrImplementedInterface) => new
                    {
                        Type = type,
                        InheritedOrImplementedInterface = inheritedOrImplementedInterface
                    })
                .Where(pair =>
                    targetType.IsAssignableFrom(pair.InheritedOrImplementedInterface)
                    ||
                    (pair.InheritedOrImplementedInterface.IsGenericType
                     && targetType.IsAssignableFrom(pair.InheritedOrImplementedInterface.GetGenericTypeDefinition())))
                .Select(pair => pair.Type);

            return implementations;
        }
        
        /// <summary>
        /// Gets all implementations of the <see cref="IVertexColorizerAlgorithm{TVertex}"/> interface.
        /// </summary>
        [NotNull, ItemNotNull]
        public static IEnumerable<Type> VertexColorizers => GetImplementationsOf(typeof(IVertexColorizerAlgorithm<>));
    }
}