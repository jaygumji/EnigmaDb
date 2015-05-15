using System;

namespace Enigma.Serialization.Reflection
{
    public class TypeValidArgs
    {
        private readonly Type _type;

        public bool IsValid { get; set; }

        public TypeValidArgs(Type type)
        {
            _type = type;
            IsValid = true;
        }

        public Type Type
        {
            get { return _type; }
        }

    }
}