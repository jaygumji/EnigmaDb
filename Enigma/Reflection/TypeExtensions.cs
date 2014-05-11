﻿using System;
using System.Collections.Generic;
using System.Linq;

namespace Enigma.Reflection
{
    public static class TypeExtensions
    {
        private static readonly Type CollectionType = typeof (ICollection<>);
        private static readonly Type DictionaryType = typeof (IDictionary<,>);
        private static readonly Type NullableType = typeof (Nullable<>);

        public static IContainerTypeInfo GetContainerTypeInfo(this Type type)
        {
            //Nullable.GetUnderlyingType(type);
            if (type.IsGenericType) {
                var genericTypeDefinition = type.GetGenericTypeDefinition();
                if (genericTypeDefinition == DictionaryType) {
                    var arguments = type.GetGenericArguments();
                    return new DictionaryContainerTypeInfo(arguments[0], arguments[1]);
                }

                if (genericTypeDefinition == CollectionType)
                    return new CollectionContainerTypeInfo(type.GetGenericArguments()[0]);

                if (genericTypeDefinition == NullableType)
                    return new NullableContainerTypeInfo(type.GetGenericArguments()[0]);
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
    }
}