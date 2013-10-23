using System;
using System.Collections.Generic;
using System.Reflection;

namespace Enigma.Reflection
{
    public class ExtendedType
    {
        private static readonly IList<Type> SystemValueClasses = new[] {
            typeof (DateTime), typeof (String), typeof (TimeSpan), typeof(Guid)
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

        public Type Inner { get { return _type; } }
        public TypeClass Class { get { return _class.Value; } }
        public IContainerTypeInfo Container { get { return _containerTypeInfo.Value; } }

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

        #region Get Type Class

        private static TypeClass GetTypeClass(Type type, IContainerTypeInfo containerInfo)
        {
            var collection = containerInfo as CollectionContainerTypeInfo;
            if (collection != null) return TypeClass.Collection;

            var dictionary = containerInfo as DictionaryContainerTypeInfo;
            if (dictionary != null) return TypeClass.Dictionary;

            var nullable = containerInfo as NullableContainerTypeInfo;
            if (nullable != null) return TypeClass.Nullable;

            if (type.IsPrimitive) return TypeClass.Value;
            if (type.IsEnum) return TypeClass.Value;
            if (SystemValueClasses.Contains(type)) return TypeClass.Value;

            return TypeClass.Complex;
        }

        #endregion

    }
}
