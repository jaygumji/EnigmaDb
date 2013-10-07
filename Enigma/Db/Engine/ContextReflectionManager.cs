using Enigma.Modelling;
using System;
using System.Linq;
using System.Collections.Concurrent;
using System.Reflection;
namespace Enigma.Db.Engine
{
    class ContextReflectionManager
    {

        private readonly ConcurrentDictionary<Type, ContextReflectionDetails> _details;

        public ContextReflectionManager()
        {
            _details = new ConcurrentDictionary<Type, ContextReflectionDetails>();
        }

        public ContextReflectionDetails GetDetails(Type type, Action<Model> onCreated)
        {
            return _details.GetOrAdd(type, t => GenerateDetails(t, onCreated));
        }

        private ContextReflectionDetails GenerateDetails(Type type, Action<Model> onCreated)
        {
            var setType = typeof(Enigma.Entities.ISet<>);
            var enigmaSetType = typeof(EnigmaSet<>);

            var setProperties = (from p in type.GetProperties(BindingFlags.Instance | BindingFlags.Public)
                                 where p.CanWrite && p.PropertyType.IsGenericType
                                 let genericTypeDef = p.PropertyType.GetGenericTypeDefinition()
                                 where genericTypeDef == setType || genericTypeDef == enigmaSetType
                                 select p).ToList();

            var model = new Model();
            foreach (var setProperty in setProperties)
            {
                var entityType = setProperty.PropertyType.GetGenericArguments()[0];
                model.Register(entityType);
            }

            onCreated(model);

            return new ContextReflectionDetails(model, setProperties);
        }

    }
}
