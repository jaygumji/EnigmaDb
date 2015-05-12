using System;
using System.Collections.Generic;
using System.Reflection;
using Enigma.Reflection;

namespace Enigma.Serialization.Reflection.Emit
{
    public class CollectionMembers
    {
        public readonly Type VariableType;
        public readonly Type ElementType;
        public readonly MethodInfo Add;
        public readonly ConstructorInfo Constructor;
        public readonly ExtendedType ElementTypeRef;

        public CollectionMembers(ExtendedType collectionType)
        {
            ElementType = collectionType.Container.AsCollection().ElementType;
            ElementTypeRef = ElementType.Extend();
            VariableType = typeof(ICollection<>).MakeGenericType(ElementType);

            Add = VariableType.GetMethod("Add", new[] { ElementType });
            var instanceType = collectionType.Inner.IsInterface
                ? typeof(List<>).MakeGenericType(ElementType)
                : collectionType.Inner;

            Constructor = instanceType.GetConstructor(Type.EmptyTypes);
            if (Constructor == null) throw InvalidGraphException.NoParameterLessConstructor(collectionType.Inner);
        }
    }
}