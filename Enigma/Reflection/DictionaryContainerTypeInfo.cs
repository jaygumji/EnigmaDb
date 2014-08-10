using System;
using System.Collections.Generic;

namespace Enigma.Reflection
{
    public class DictionaryContainerTypeInfo : CollectionContainerTypeInfo
    {

        private static readonly Type KeyValuePairType = typeof(KeyValuePair<,>);

        private readonly Type _keyType;
        private readonly Type _valueType;
        private readonly Lazy<Type> _dictionaryInterfaceType;

        public DictionaryContainerTypeInfo(Type keyType, Type valueType)
            : base(KeyValuePairType.MakeGenericType(keyType, valueType))
        {
            _keyType = keyType;
            _valueType = valueType;

            _dictionaryInterfaceType = new Lazy<Type>(() => TypeExtensions.DictionaryType.MakeGenericType(keyType, valueType));
        }

        public Type KeyType { get { return _keyType; } }
        public Type ValueType { get { return _valueType; } }

        public Type DictionaryInterfaceType { get { return _dictionaryInterfaceType.Value; } }
    }
}