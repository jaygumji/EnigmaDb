using System;
using System.Collections.Generic;
using System.Linq;

namespace Enigma.Reflection
{
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
            var propertyNames = path.Split('.');
            var containerType = type;
            foreach (var propertyName in propertyNames) {
                var property = containerType.GetProperty(propertyName);
                var extendedPropertyType = new ExtendedType(property.PropertyType);

                if (extendedPropertyType.Class == TypeClass.Collection) {
                    containerType = extendedPropertyType.Container.AsCollection().ElementType;
                    result.Add(new ListPropertyAccessor(property));
                }
                else {
                    containerType = property.PropertyType;
                    result.Add(new PropertyAccessor(property));
                }
            }

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
