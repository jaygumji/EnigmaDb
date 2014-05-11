using System;
using System.Linq;
using Enigma.Reflection;

namespace Enigma.Serialization.Reflection.Emit
{
    public static class ReflectionAnalyzer
    {

        public static bool TryGetComplexTypes(ExtendedType type, out Type[] types)
        {
            if (type.Class == TypeClass.Complex) {
                types = new[] { type.Inner };
                return true;
            }
            if (type.Class == TypeClass.Nullable) {
                var elementType = type.Container.AsNullable().ElementType.Extend();
                return TryGetComplexTypes(elementType, out types);
            }
            if (type.Class == TypeClass.Dictionary) {
                var container = type.Container.AsDictionary();
                Type[] keyTypes;
                var hasKeyTypes = TryGetComplexTypes(container.KeyType.Extend(), out keyTypes);

                Type[] valueTypes;
                var hasValueTypes = TryGetComplexTypes(container.ValueType.Extend(), out valueTypes);

                if (!hasKeyTypes && !hasValueTypes) {
                    types = null;
                    return false;
                }

                if (hasKeyTypes && hasValueTypes) types = keyTypes.Concat(valueTypes).ToArray();
                else if (hasKeyTypes) types = keyTypes;
                else types = valueTypes;

                return true;
            }
            if (type.Class == TypeClass.Collection) {
                var elementType = type.Container.AsCollection().ElementType.Extend();
                return TryGetComplexTypes(elementType, out types);
            }

            types = null;
            return false;
        }

    }
}
