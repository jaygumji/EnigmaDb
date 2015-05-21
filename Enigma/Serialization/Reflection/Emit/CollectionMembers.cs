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
        public readonly ExtendedType ElementTypeExt;
        public readonly MethodInfo ToArray;

        public CollectionMembers(ExtendedType collectionType)
        {
            ElementType = collectionType.Container.AsCollection().ElementType;
            ElementTypeExt = ElementType.Extend();
            VariableType = collectionType.Ref.IsInterface || collectionType.Ref.IsArray
                ? typeof (List<>).MakeGenericType(ElementType)
                : typeof (ICollection<>).MakeGenericType(ElementType);

            Add = VariableType.GetMethod("Add", new[] { ElementType });
            var instanceType = collectionType.Ref.IsInterface || collectionType.Ref.IsArray
                ? VariableType
                : collectionType.Ref;

            Constructor = instanceType.GetConstructor(Type.EmptyTypes);
            if (Constructor == null) throw InvalidGraphException.NoParameterLessConstructor(collectionType.Ref);

            if (collectionType.Ref.IsArray) {
                ToArray = VariableType.GetMethod("ToArray");
            }
        }
    }
}