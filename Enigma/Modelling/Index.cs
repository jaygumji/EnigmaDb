using System;
using System.Reflection;
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
        private readonly Type _propertyType;

        protected Index(PropertyInfo propertyInfo)
            : this(propertyInfo.Name, propertyInfo.PropertyType)
        {
        }

        protected Index(string propertyName, Type propertyType)
        {
            _propertyName = propertyName;
            _propertyType = propertyType;
        }

        public string PropertyName { get { return _propertyName; } }
        public Type PropertyType { get { return _propertyType; } }

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
