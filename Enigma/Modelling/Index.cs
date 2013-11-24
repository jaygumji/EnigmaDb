using System;
using System.Linq;
using System.Reflection;
using Enigma.Reflection;

namespace Enigma.Modelling
{
    public class Index<T> : Index, IIndex<T>
    {
        public Index(string uniqueName) : base(uniqueName, typeof(T))
        {
        }
    }

    public abstract class Index : IIndex
    {
        private readonly string _uniqueName;
        private readonly Type _valueType;

        protected Index(string uniqueName, Type valueType)
        {
            _uniqueName = uniqueName;

            var extendedValueType = new ExtendedType(valueType);
            switch (extendedValueType.Class) {
                case TypeClass.Complex:
                    throw new ArgumentException("Only values is accepted as an index");
                case TypeClass.Dictionary:
                    throw new NotSupportedException("Indexed dictionaries are currently not supported");
                case TypeClass.Nullable:
                    valueType = extendedValueType.Container.AsNullable().ElementType;
                    break;
                case TypeClass.Collection:
                    valueType = extendedValueType.Container.AsCollection().ElementType;
                    break;
            }

            _valueType = valueType;
        }

        public string UniqueName { get { return _uniqueName; } }
        public Type ValueType { get { return _valueType; } }

        public static Index Create(Type entityType, string uniqueName)
        {
            var propertyInfo = PropertyExtractor.GetProperties(entityType, uniqueName).Last();
            var type = typeof(Index<>).MakeGenericType(propertyInfo.PropertyType);
            return (Index) Activator.CreateInstance(type, uniqueName);
        }

        public void CopyFrom(Index index)
        {
        }
    }
}
