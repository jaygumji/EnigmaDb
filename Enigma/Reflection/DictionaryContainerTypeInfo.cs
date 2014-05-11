using System;
using System.Collections.Generic;

namespace Enigma.Reflection
{
    public class DictionaryContainerTypeInfo : CollectionContainerTypeInfo
    {
        private readonly Type _keyType;
        private readonly Type _valueType;

        public DictionaryContainerTypeInfo(Type keyType, Type valueType)
            : base(typeof(KeyValuePair<,>).MakeGenericType(keyType, valueType))
        {
            _keyType = keyType;
            _valueType = valueType;
        }

        public Type KeyType { get { return _keyType; } }
        public Type ValueType { get { return _valueType; } }
    }
}