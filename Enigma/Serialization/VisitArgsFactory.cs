using System;
using Enigma.Serialization.Reflection;

namespace Enigma.Serialization
{
    public class VisitArgsFactory : IVisitArgsFactory
    {
        private readonly SerializableTypeProvider _provider;
        private readonly SerializableType _ser;

        public VisitArgsFactory(SerializableTypeProvider provider, Type type)
        {
            _provider = provider;
            _ser = provider.GetOrCreate(type);
        }

        public IVisitArgsFactory ConstructWith(Type type)
        {
            return new VisitArgsFactory(_provider, type);
        }

        public VisitArgs Construct(string propertyName)
        {
            var property = _ser.FindProperty(propertyName);
            return property.CreateVisitArgs();
        }
    }
}