using System;
using System.Collections.Generic;
using System.Linq;
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
            ArrayContainerTypeInfo arrayTypeInfo;
            if (collectionType.TryGetArrayTypeInfo(out arrayTypeInfo)) {
                if (arrayTypeInfo.Ranks > 3)
                    throw new NotSupportedException("The serialization engine is limited to 3 ranks in arrays");
                if (arrayTypeInfo.Ranks == 3) {
                    var baseType = typeof (ICollection<>);
                    ElementType = baseType.MakeGenericType(baseType.MakeGenericType(arrayTypeInfo.ElementType));
                    ToArray = typeof (ArrayProvider).GetMethod("To3DArray").MakeGenericMethod(arrayTypeInfo.ElementType);
                }
                else if (arrayTypeInfo.Ranks == 2) {
                    ElementType = typeof (ICollection<>).MakeGenericType(arrayTypeInfo.ElementType);
                    ToArray = typeof(ArrayProvider).GetMethod("To2DArray").MakeGenericMethod(arrayTypeInfo.ElementType);
                }
                else {
                    ElementType = arrayTypeInfo.ElementType;
                    ToArray = typeof(ArrayProvider).GetMethod("ToArray").MakeGenericMethod(arrayTypeInfo.ElementType);
                }
            }
            else {
                ElementType = collectionType.Container.AsCollection().ElementType;
            }

            ElementTypeExt = ElementType.Extend();
            VariableType = typeof (ICollection<>).MakeGenericType(ElementType);

            Add = VariableType.GetMethod("Add", new[] { ElementType });
            var instanceType = collectionType.Ref.IsInterface || collectionType.Ref.IsArray
                ? typeof(List<>).MakeGenericType(ElementType)
                : collectionType.Ref;

            Constructor = instanceType.GetConstructor(Type.EmptyTypes);
            if (Constructor == null) throw InvalidGraphException.NoParameterLessConstructor(collectionType.Ref);
        }
    }
}