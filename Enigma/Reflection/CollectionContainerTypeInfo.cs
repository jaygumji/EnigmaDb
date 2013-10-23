using System;

namespace Enigma.Reflection
{
    public class CollectionContainerTypeInfo : IContainerTypeInfo
    {
        private readonly Type _elementType;

        public CollectionContainerTypeInfo(Type elementType)
        {
            _elementType = elementType;
        }

        public Type ElementType { get { return _elementType; } }
    }
}