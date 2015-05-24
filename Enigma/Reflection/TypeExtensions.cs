using System;
using System.Collections.Generic;
using System.Linq;

namespace Enigma.Reflection
{
    public static class TypeExtensions
    {
        public static readonly Type CollectionType = typeof (ICollection<>);
        public static readonly Type DictionaryType = typeof (IDictionary<,>);
        public static readonly Type NullableType = typeof (Nullable<>);

        public static IContainerTypeInfo GetContainerTypeInfo(this Type type)
        {
            if (type.IsArray) {
                var ranks = type.GetArrayRank();
                var elementType = type.GetElementType();
                return new ArrayContainerTypeInfo(elementType, ranks);
            }
            if (type.IsGenericType) {
                var genericTypeDefinition = type.GetGenericTypeDefinition();
                if (genericTypeDefinition == DictionaryType) {
                    var arguments = type.GetGenericArguments();
                    return new DictionaryContainerTypeInfo(arguments[0], arguments[1]);
                }

                if (genericTypeDefinition == CollectionType)
                    return new CollectionContainerTypeInfo(type.GetGenericArguments()[0]);

                if (genericTypeDefinition == NullableType)
                    return new NullableContainerTypeInfo(type, type.GetGenericArguments()[0]);
            }

            var interfaceTypes = type.GetInterfaces();
            foreach (var interfaceType in interfaceTypes.Where(interfaceType => interfaceType.IsGenericType)) {
                var genericTypeDefinition = interfaceType.GetGenericTypeDefinition();
                if (genericTypeDefinition == CollectionType)
                    return new CollectionContainerTypeInfo(interfaceType.GetGenericArguments()[0]);
                if (genericTypeDefinition == DictionaryType) {
                    var arguments = interfaceType.GetGenericArguments();
                    return new DictionaryContainerTypeInfo(arguments[0], arguments[1]);
                }
            }

            return null;
        }

        public static Type AsNullable(this Type type)
        {
            return NullableType.MakeGenericType(type);
        }

    }
}