using System;
using System.Collections.Generic;

namespace Enigma.Serialization.Reflection
{
    public class SerializableType
    {
        private readonly Type _type;
        private readonly IReadOnlyDictionary<string, SerializableProperty> _properties;

        public SerializableType(Type type, IReadOnlyDictionary<string, SerializableProperty> properties)
        {
            _type = type;
            _properties = properties;
        }

        public Type Type { get { return _type; } }

        public IEnumerable<SerializableProperty> Properties
        {
            get { return _properties.Values; }
        }

        public SerializableProperty FindProperty(string propertyName)
        {
            SerializableProperty ser;
            if (!_properties.TryGetValue(propertyName, out ser))
                throw new ArgumentException("Property was not found, " + propertyName);

            return ser;
        }
    }
}
