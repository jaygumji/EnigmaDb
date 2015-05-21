using System;
using System.Collections.Generic;
using System.Reflection;

namespace Enigma.Reflection
{
    public class ExtendedType
    {
        private static readonly IList<Type> SystemValueClasses = new[] {
            typeof (DateTime), typeof (String), typeof (TimeSpan), typeof(Guid), typeof(Decimal), typeof(byte[])
        }; 

        private readonly Type _type;
        private readonly Lazy<IContainerTypeInfo> _containerTypeInfo;
        private readonly Lazy<TypeClass> _class;

        public ExtendedType(Type type)
        {
            _type = type;
            _containerTypeInfo = new Lazy<IContainerTypeInfo>(type.GetContainerTypeInfo);
            _class = new Lazy<TypeClass>(() => GetTypeClass(type, _containerTypeInfo.Value));
        }

        public Type Ref { get { return _type; } }
        public TypeClass Class { get { return _class.Value; } }
        public IContainerTypeInfo Container { get { return _containerTypeInfo.Value; } }
        public bool ImplementsCollection { get { return Class == TypeClass.Collection || Class == TypeClass.Dictionary; } }

        public bool IsValueOrNullableOfValue()
        {
            if (Class == TypeClass.Value) return true;
            if (Class != TypeClass.Nullable) return false;
            return Container.AsNullable().ElementType.Extend().Class == TypeClass.Value;
        }

        public bool IsEnum()
        {
            return _type.IsEnum || (Class == TypeClass.Nullable && Container.AsNullable().ElementType.IsEnum);
        }

        public Type GetUnderlyingEnumType()
        {
            if (_type.IsEnum) return Enum.GetUnderlyingType(_type);

            if (Class == TypeClass.Nullable) {
                var elementType = Container.AsNullable().ElementType;
                if (elementType.IsEnum) {
                    var underlyingType = Enum.GetUnderlyingType(elementType);
                    return typeof (Nullable<>).MakeGenericType(underlyingType);
                }
            }

            throw new InvalidOperationException("The type is not an enum");
        }

        public bool TryGetCollectionTypeInfo(out CollectionContainerTypeInfo collectionTypeInfo)
        {
            collectionTypeInfo = _containerTypeInfo.Value as CollectionContainerTypeInfo;
            return collectionTypeInfo != null;
        }

        public bool TryGetDictionaryTypeInfo(out DictionaryContainerTypeInfo dictionaryTypeInfo)
        {
            dictionaryTypeInfo = _containerTypeInfo.Value as DictionaryContainerTypeInfo;
            return dictionaryTypeInfo != null;
        }

        #region OnGet VariableType Class

        private static TypeClass GetTypeClass(Type type, IContainerTypeInfo containerInfo)
        {
            if (type.IsPrimitive) return TypeClass.Value;
            if (type.IsEnum) return TypeClass.Value;
            if (SystemValueClasses.Contains(type)) return TypeClass.Value;

            var dictionary = containerInfo as DictionaryContainerTypeInfo;
            if (dictionary != null) return TypeClass.Dictionary;

            var collection = containerInfo as CollectionContainerTypeInfo;
            if (collection != null) return TypeClass.Collection;

            var nullable = containerInfo as NullableContainerTypeInfo;
            if (nullable != null) return TypeClass.Nullable;

            return TypeClass.Complex;
        }

        #endregion

    }

}
