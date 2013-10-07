using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
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

                Type elementType;
                if (TryGetElementType(property.PropertyType, out elementType)) {
                    containerType = elementType;
                    result.Add(new ListPropertyAccessor(property));
                }
                else {
                    containerType = property.PropertyType;
                    result.Add(new PropertyAccessor(property));
                }
            }

            return result;
        }

        private static bool TryGetElementType(Type type, out Type elementType)
        {
            elementType = null;
            if (!type.IsGenericType) return false;

            var genericTypeDefinition = type.GetGenericTypeDefinition();
            if (genericTypeDefinition == typeof(IList<>)) {
                elementType = type.GetGenericArguments()[0];
                return true;
            }
            return false;
        }

        public IEnumerable<T> Extract(object entity)
        {
            var entities = (IEnumerable<object>) new[] {entity};
            foreach (var accessor in _propertyAccessors)
                entities = accessor.GetValuesOf(entities);

            return entities.Cast<T>();
        }
    }

    public interface IPropertyAccessor
    {
        IEnumerable<object> GetValuesOf(IEnumerable<object> values);
    }

    class PropertyAccessor : IPropertyAccessor
    {
        private readonly PropertyInfo _propertyInfo;

        public PropertyAccessor(PropertyInfo propertyInfo)
        {
            _propertyInfo = propertyInfo;
        }

        public IEnumerable<object> GetValuesOf(IEnumerable<object> values)
        {
            var next = new List<object>();

            foreach (var value in values) {
                var nextValue = _propertyInfo.GetValue(value);
                next.Add(nextValue);
            }

            return next;
        }
    }

    class ListPropertyAccessor : IPropertyAccessor
    {
        private readonly PropertyInfo _propertyInfo;

        public ListPropertyAccessor(PropertyInfo propertyInfo)
        {
            _propertyInfo = propertyInfo;
        }

        public IEnumerable<object> GetValuesOf(IEnumerable<object> values)
        {
            var next = new List<object>();

            foreach (var value in values) {
                var list = (IList) _propertyInfo.GetValue(value);
                foreach (var element in list) next.Add(value);
            }

            return next;
        }
    }
}
