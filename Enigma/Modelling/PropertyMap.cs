using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;
using System.Text;

namespace Enigma.Modelling
{
    public class PropertyMap<T> : PropertyMap, IPropertyMap<T>
    {
        public PropertyMap(PropertyInfo propertyInfo)
            : base(propertyInfo)
        {
        }

        public PropertyMap(PropertyInfo propertyInfo, int index)
            : base(propertyInfo, index)
        {
        }
    }

    public abstract class PropertyMap : IPropertyMap
    {
        protected PropertyMap(PropertyInfo propertyInfo)
        {
            PropertyName = propertyInfo.Name;
            Name = propertyInfo.Name;
            Index = -1;
        }

        protected PropertyMap(PropertyInfo propertyInfo, int index)
        {
            PropertyName = propertyInfo.Name;
            Name = propertyInfo.Name;
            Index = index;
        }

        public string PropertyName { get; set; }
        public string Name { get; set; }
        public int Index { get; set; }

        public static PropertyMap Create(Type entityType, string propertyName)
        {
            var property = entityType.GetProperty(propertyName, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
            return Create(property);
        }

        public static PropertyMap Create(PropertyInfo propertyInfo)
        {
            var type = typeof(PropertyMap<>).MakeGenericType(propertyInfo.PropertyType);
            return (PropertyMap)Activator.CreateInstance(type, propertyInfo);
        }

        public override int GetHashCode()
        {
            return PropertyName.GetHashCode() ^ (Name.GetHashCode() + Index.GetHashCode());
        }

        public void CopyFrom(PropertyMap propertyMap)
        {
            Index = propertyMap.Index;
            Name = propertyMap.Name;
        }
    }

}
