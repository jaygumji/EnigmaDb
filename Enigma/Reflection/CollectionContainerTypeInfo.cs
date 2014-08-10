using System;

namespace Enigma.Reflection
{
    public class CollectionContainerTypeInfo : IContainerTypeInfo
    {
        private readonly Type _elementType;
        private readonly Lazy<Type> _collectionInterfaceType;

        public CollectionContainerTypeInfo(Type elementType)
        {
            _elementType = elementType;
            _collectionInterfaceType = new Lazy<Type>(() => TypeExtensions.CollectionType.MakeGenericType(elementType));
        }

        public Type ElementType { get { return _elementType; } }
        public Type CollectionInterfaceType { get { return _collectionInterfaceType.Value; } }
    }
}