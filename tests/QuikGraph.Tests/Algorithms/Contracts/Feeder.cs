using System;
using System.Collections.Generic;
using System.Linq;
using QuikGraph.Algorithms;

namespace QuikGraph.Tests.Algorithms.Contracts
{
    public static class Feeder
    {
        public static IEnumerable<Type> GetConcreteClassesImplementing(Type targetType)
        {
            var implementors = (
                from assembly in AppDomain.CurrentDomain.GetAssemblies()
                from type in assembly.GetTypes()
                where !type.IsInterface && !type.IsAbstract
                from inheritedOrImplementedInterface in type.GetInterfaces()
                where
                    targetType.IsAssignableFrom(inheritedOrImplementedInterface)
                    || (inheritedOrImplementedInterface.IsGenericType
                        && targetType.IsAssignableFrom(inheritedOrImplementedInterface.GetGenericTypeDefinition()))
                select type
            );

            return implementors;
        }
        public static IEnumerable<Type> VertexColorizers
        {
            get => GetConcreteClassesImplementing(typeof(IVertexColorizerAlgorithm<>));
        }
    }
}