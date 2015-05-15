using System;
using System.Reflection;

namespace Enigma.Serialization.Reflection
{
    public class SerializationReflectionInspector
    {

        public bool CanBeSerialized(Type type, PropertyInfo property)
        {
            if (!property.CanWrite) return false;

            var args = new PropertyValidArgs(type, property);
            IsPropertyValid(args);
            return args.IsValid;
        }

        public bool CanBeDeserialized(Type type, PropertyInfo property)
        {
            if (!property.CanRead) return false;

            var args = new PropertyValidArgs(type, property);
            IsPropertyValid(args);
            return args.IsValid;
        }

        public bool CanBeSerialized(Type type)
        {
            var args = new TypeValidArgs(type);
            IsTypeValid(args);
            return args.IsValid;
        }

        public bool CanBeDeserialized(Type type)
        {
            var args = new TypeValidArgs(type);
            IsTypeValid(args);
            return args.IsValid;
        }

        public SerializationMetadata AcquirePropertyMetadata(Type type, PropertyInfo property, ref uint nextIndex)
        {
            var args = new AcquirePropertyMetadataArgs(type, property);
            OnAcquirePropertyMetadata(args);

            var index = args.Index.HasValue ? args.Index.Value : nextIndex++;
            var metadata = new SerializationMetadata(index, args.Args);
            return metadata;
        }

        protected virtual void IsPropertyValid(PropertyValidArgs args) { }
        protected virtual void IsTypeValid(TypeValidArgs args) { }
        protected virtual void OnAcquirePropertyMetadata(AcquirePropertyMetadataArgs args) { }

    }
}