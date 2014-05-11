using System;
using System.Reflection;

namespace Enigma.Serialization.Reflection.Emit
{
    public sealed class NullableMembers
    {
        public static readonly Type NullableTypeDefinition = typeof(Nullable<>);

        public readonly Type NullableType;
        public readonly ConstructorInfo Constructor;
        public readonly MethodInfo GetHasValue;
        public readonly MethodInfo GetValue;

        public NullableMembers(Type elementType)
        {
            NullableType = NullableTypeDefinition.MakeGenericType(elementType);
            Constructor = NullableType.GetConstructor(new[] { elementType });
            GetHasValue = NullableType.GetProperty("HasValue").GetGetMethod();
            GetValue = NullableType.GetProperty("Value").GetGetMethod();
        }
    }
}