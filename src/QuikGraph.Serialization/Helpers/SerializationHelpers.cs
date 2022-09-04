#if SUPPORTS_GRAPHML_GRAPH_SERIALIZATION
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Xml.Serialization;
using System.ComponentModel;
using JetBrains.Annotations;

namespace QuikGraph.Serialization
{
    internal static class SerializationHelpers
    {
        /// <summary>
        /// Checks if the given <paramref name="type"/> is treatable (not null, <see cref="object"/> or <see cref="ValueType"/>).
        /// </summary>
        /// <param name="type"><see cref="Type"/> to check.</param>
        /// <returns>True if the <paramref name="type"/> can be treated, false otherwise.</returns>
        [Pure]
        [ContractAnnotation("type:null => false")]
        private static bool IsTreatableType([CanBeNull] Type type)
        {
            return type != null
                   && type != typeof(object)
                   && type != typeof(ValueType);
        }

        /// <summary>
        /// Checks if given <paramref name="type"/> is assignable to given <paramref name="genericType"/>.
        /// </summary>
        /// <param name="type">Type to check if assignable.</param>
        /// <param name="genericType">Type to check compatibility with.</param>
        /// <returns>True if <paramref name="type"/> is assignable to <paramref name="genericType"/>, false otherwise.</returns>
        [Pure]
        public static bool IsAssignableToGenericType([NotNull] Type type, [NotNull] Type genericType)
        {
            while (true)
            {
                if (type is null || type == typeof(object))
                    return false;

                if (type == genericType)
                    return true;

                if ((type.IsGenericType ? type.GetGenericTypeDefinition() : type) == genericType)
                    return true;

                if (genericType.IsInterface
                    && type.GetInterfaces().Any(interfaceType => IsAssignableToGenericType(interfaceType, genericType)))
                    return true;

                type = type.BaseType;
            }
        }

        /// <summary>
        /// Checks if the given <paramref name="property"/> is an indexed one.
        /// </summary>
        /// <param name="property">A <see cref="PropertyInfo"/>.</param>
        /// <returns>True if the <paramref name="property"/> is an indexed property, false otherwise.</returns>
        [Pure]
        private static bool IsIndexed([NotNull] PropertyInfo property)
        {
            return property.GetIndexParameters().Length != 0;
        }

        /// <summary>
        /// Gets all properties that are marked with <see cref="XmlAttributeAttribute"/> on given type.
        /// </summary>
        /// <param name="type">Object type.</param>
        /// <returns>Enumerable of serializable properties information.</returns>
        [Pure]
        [NotNull]
        public static IEnumerable<PropertySerializationInfo> GetAttributeProperties([CanBeNull] Type type)
        {
            Type currentType = type;
            while (IsTreatableType(currentType))
            {
                // Iterate through properties that must have a get, and are not indexed property
                IEnumerable<PropertyInfo> properties = currentType
                    .GetProperties(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly)
                    .Where(ValidProperty);
                foreach (PropertyInfo property in properties)
                {
                    // Is it tagged with XmlAttributeAttribute
                    if (TryGetAttributeName(property, out string name))
                    {
                        if (TryGetDefaultValue(property, out object value))
                            yield return new PropertySerializationInfo(property, name, value);
                        else
                            yield return new PropertySerializationInfo(property, name);
                    }
                }

                currentType = currentType.BaseType;
            }

            #region Local function

            bool ValidProperty(PropertyInfo property)
            {
                return property.CanRead && !IsIndexed(property);
            }

            #endregion
        }

        [Pure]
        public static bool TryGetAttributeName([NotNull] PropertyInfo property, out string name)
        {
            var attribute = Attribute.GetCustomAttribute(property, typeof(XmlAttributeAttribute)) as XmlAttributeAttribute;
            if (attribute is null)
            {
                name = null;
                return false;
            }

            name = string.IsNullOrEmpty(attribute.AttributeName)
                ? property.Name
                : attribute.AttributeName;
            return true;
        }

        [Pure]
        public static bool TryGetDefaultValue([NotNull] PropertyInfo property, out object value)
        {
            var attribute = Attribute.GetCustomAttribute(property, typeof(DefaultValueAttribute)) as DefaultValueAttribute;
            if (attribute is null)
            {
                value = null;
                return false;
            }

            value = attribute.Value;
            return true;
        }
    }
}
#endif