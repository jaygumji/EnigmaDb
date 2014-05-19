using System;
using System.Collections.Generic;
using System.Reflection;
using Enigma.Reflection;

namespace Enigma.Serialization.Reflection.Emit
{
    public class DictionaryMembers
    {
        public readonly Type VariableType;
        public readonly Type KeyType;
        public readonly Type ValueType;
        public readonly Type ElementType;
        public readonly MethodInfo Add;
        public readonly ConstructorInfo Constructor;

        public DictionaryMembers(ExtendedType dictionaryType)
        {
            var container = dictionaryType.Container.AsDictionary();
            KeyType = container.KeyType;
            ValueType = container.ValueType;
            ElementType = container.ElementType;
            VariableType = typeof (IDictionary<,>).MakeGenericType(KeyType, ValueType);

            Add = VariableType.GetMethod("Add", new[] {KeyType, ValueType});
            var instanceType = dictionaryType.Inner.IsInterface
                ? typeof (Dictionary<,>).MakeGenericType(KeyType, ValueType)
                : dictionaryType.Inner;
            Constructor = instanceType.GetConstructor(Type.EmptyTypes);
            if (Constructor == null) throw InvalidGraphException.NoParameterLessConstructor(dictionaryType.Inner);
        }

    }
}