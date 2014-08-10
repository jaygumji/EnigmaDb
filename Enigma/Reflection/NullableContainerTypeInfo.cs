using System;
using System.Reflection;

namespace Enigma.Reflection
{
    public class NullableContainerTypeInfo : IContainerTypeInfo
    {
        private readonly Type _elementType;
        private readonly Lazy<ConstructorInfo> _constructor;
        private readonly Lazy<MethodInfo> _getHasValueMethod;

        public NullableContainerTypeInfo(Type type, Type elementType)
        {
            _elementType = elementType;
            _constructor = new Lazy<ConstructorInfo>(() => type.GetConstructor(new[] {elementType}));
            _getHasValueMethod = new Lazy<MethodInfo>(() => type.GetProperty("HasValue").GetGetMethod());
        }

        public Type ElementType { get { return _elementType; } }
        public ConstructorInfo Constructor { get { return _constructor.Value; } }
        public MethodInfo GetHasValueMethod { get { return _getHasValueMethod.Value; } }
    }
}