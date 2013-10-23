using System;

namespace Enigma.Reflection
{
    public class NullableContainerTypeInfo : IContainerTypeInfo
    {
        private readonly Type _elementType;

        public NullableContainerTypeInfo(Type elementType)
        {
            _elementType = elementType;
        }

        public Type ElementType { get { return _elementType; } }

    }
}