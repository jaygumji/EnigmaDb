using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Enigma.Reflection
{

    public static class PropertyExtractor
    {
        public static IEnumerable<PropertyInfo> GetProperties(Type type, string path)
        {
            var result = new List<PropertyInfo>();
            Resolve(type, path, (p, pt) => result.Add(p));
            return result;
        } 

        public static void Resolve(Type type, string path, Action<PropertyInfo, ExtendedType> action)
        {
            var propertyNames = path.Split('.');
            var containerType = type;
            foreach (var propertyName in propertyNames) {
                var property = containerType.GetProperty(propertyName);
                var extendedPropertyType = new ExtendedType(property.PropertyType);

                containerType = extendedPropertyType.Class == TypeClass.Collection
                    ? extendedPropertyType.Container.AsCollection().ElementType
                    : property.PropertyType;

                action(property, extendedPropertyType);
            }
        }  
    }

    public class PropertyExtractor<T>
    {

        private readonly List<IPropertyAccessor> _propertyAccessors;

        public PropertyExtractor(Type type, string path)
        {
            _propertyAccessors = Resolve(type, path);
        }

        private List<IPropertyAccessor> Resolve(Type type, string path)
        {
            var result = new List<IPropertyAccessor>();
            PropertyExtractor.Resolve(type, path, (property, extendedPropertyType) => {
                if (extendedPropertyType.Class == TypeClass.Collection)
                    result.Add(new ListPropertyAccessor(property));
                else
                    result.Add(new PropertyAccessor(property));
            });

            return result;
        }

        public IEnumerable<T> Extract(object entity)
        {
            var entities = (IEnumerable<object>) new[] {entity};
            foreach (var accessor in _propertyAccessors)
                entities = accessor.GetValuesOf(entities);

            return entities.Cast<T>();
        }
    }
}
