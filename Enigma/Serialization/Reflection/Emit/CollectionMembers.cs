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

        public CollectionMembers(ExtendedType collectionType)
        {
            ElementType = collectionType.Container.AsCollection().ElementType;
            VariableType = collectionType.Inner.IsInterface
                ? typeof (List<>).MakeGenericType(ElementType)
                : collectionType.Inner;
            Add = VariableType.GetMethod("Add", new[] { ElementType });
            if (Add == null) throw InvalidGraphException.MissingCollectionAddMethod(collectionType.Inner);
            Constructor = VariableType.GetConstructor(Type.EmptyTypes);
            if (Constructor == null) throw InvalidGraphException.CollectionHasNoParameterLessConstructor(collectionType.Inner);
        }
    }
}