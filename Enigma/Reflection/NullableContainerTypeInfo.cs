using System;
using System.Reflection;

namespace Enigma.Reflection
{
    public class NullableContainerTypeInfo : IContainerTypeInfo
    {
        private readonly Type _elementType;
        private readonly Lazy<ConstructorInfo> _constructor; 

        public NullableContainerTypeInfo(Type type, Type elementType)
        {
            _elementType = elementType;
            _constructor = new Lazy<ConstructorInfo>(() => type.GetConstructor(new[] {elementType}));
        }

        public Type ElementType { get { return _elementType; } }
        public ConstructorInfo Constructor { get { return _constructor.Value; } }
    }
}