﻿namespace Enigma.Reflection
{
    public static class ContainerTypeInfoExtensions
    {

        public static ArrayContainerTypeInfo AsArray(this IContainerTypeInfo container)
        {
            return container as ArrayContainerTypeInfo;
        }

        public static CollectionContainerTypeInfo AsCollection(this IContainerTypeInfo container)
        {
            return container as CollectionContainerTypeInfo;
        }

        public static DictionaryContainerTypeInfo AsDictionary(this IContainerTypeInfo container)
        {
            return container as DictionaryContainerTypeInfo;
        }

        public static NullableContainerTypeInfo AsNullable(this IContainerTypeInfo container)
        {
            return container as NullableContainerTypeInfo;
        }

    }
}