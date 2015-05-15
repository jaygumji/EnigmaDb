using System;
using System.Reflection;

namespace Enigma.Serialization.Reflection
{
    public class PropertyValidArgs
    {
        private readonly Type _type;
        private readonly PropertyInfo _property;

        public bool IsValid { get; set; }

        public PropertyValidArgs(Type type, PropertyInfo property)
        {
            _type = type;
            _property = property;
            IsValid = true;
        }

        public Type Type
        {
            get { return _type; }
        }

        public PropertyInfo Property
        {
            get { return _property; }
        }
    }
}