using System;
using System.Reflection;
using Enigma.Reflection;

namespace Enigma.Modelling
{
    public class Index<T> : Index, IIndex<T>
    {
        public Index(PropertyInfo propertyInfo) : base(propertyInfo)
        {
        }

        public Index(string propertyName) : base(propertyName, typeof(T))
        {
        }
    }

    public abstract class Index : IIndex
    {
        private readonly string _propertyName;
        private readonly Type _valueType;

        protected Index(PropertyInfo propertyInfo)
            : this(propertyInfo.Name, propertyInfo.PropertyType)
        {
        }

        protected Index(string propertyName, Type valueType)
        {
            _propertyName = propertyName;

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

        public string PropertyName { get { return _propertyName; } }
        public Type ValueType { get { return _valueType; } }

        public static Index Create(Type entityType, string propertyName)
        {
            var propertyInfo = entityType.GetProperty(propertyName, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
            return Create(entityType, propertyInfo);
        }

        public static Index Create(Type entityType, PropertyInfo propertyInfo)
        {
            var type = typeof(Index<>).MakeGenericType(propertyInfo.PropertyType);
            return (Index)Activator.CreateInstance(type, propertyInfo);
        }

        public void CopyFrom(Index index)
        {
        }
    }
}
