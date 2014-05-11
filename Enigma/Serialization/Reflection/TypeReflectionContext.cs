using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Enigma.Reflection;

namespace Enigma.Serialization.Reflection
{
    public class TypeReflectionContext
    {

        private static readonly ConcurrentDictionary<Type, TypeReflectionContext> Cache = new ConcurrentDictionary<Type, TypeReflectionContext>();

        public static TypeReflectionContext Get(Type type)
        {
            return Cache.GetOrAdd(type, t => new TypeReflectionContext(t));
        }

        private readonly Type _type;
        private readonly Lazy<IList<PropertyReflectionContext>> _properties;
        private readonly ExtendedType _extended;

        private TypeReflectionContext(Type type)
        {
            _type = type;
            _extended = type.Extend();
            _properties = new Lazy<IList<PropertyReflectionContext>>(() => ParseProperties(type));
        }

        private static IList<PropertyReflectionContext> ParseProperties(Type type)
        {
            var properties = type.GetProperties(BindingFlags.Instance | BindingFlags.Public)
                .Where(p => p.CanRead && p.CanWrite)
                .Select(p => new PropertyReflectionContext(p))
                .ToList();

            uint index = 0;
            foreach (var property in properties)
                property.Index = ++index;

            return properties;
        }

        public Type Type { get { return _type; } }
        public ExtendedType Extended { get { return _extended; } }
        public IEnumerable<PropertyReflectionContext> SerializableProperties { get { return _properties.Value; } }

    }

    public class PropertyReflectionContext
    {
        private readonly PropertyInfo _property;
        private readonly TypeReflectionContext _propertyTypeContext;

        public PropertyReflectionContext(PropertyInfo property)
        {
            _property = property;
            _propertyTypeContext = TypeReflectionContext.Get(property.PropertyType);
        }

        public TypeReflectionContext PropertyTypeContext
        {
            get { return _propertyTypeContext; }
        }

        public PropertyInfo Property
        {
            get { return _property; }
        }

        public uint Index { get; internal set; }
    }
}
