using Enigma.Binary;
using Enigma.Modelling;
using Enigma.Store;
using Enigma.Store.Keys;
using System.Reflection;
using System.Linq;
using Enigma.Db.Linq;
using System.Collections.Generic;

namespace Enigma.Db.Embedded
{
    internal class EmbeddedEnigmaEntityEngine<T> : IEnigmaEntityEngine<T>
    {
        private readonly EntityMap<T> _entityMap;
        private readonly EmbeddedEnigmaService _service;
        private readonly IBinaryConverter<T> _converter;
        private readonly PropertyInfo _keyProperty;

        public EmbeddedEnigmaEntityEngine(EntityMap<T> entityMap, EmbeddedEnigmaService service, IBinaryConverter<T> converter)
        {
            _entityMap = entityMap;
            _service = service;
            _converter = converter;

            _keyProperty = _entityMap.GetKeyProperty();
        }

        private IKey CreateKey(T entity)
        {
            var value = _keyProperty.GetValue(entity);
            return Key.Create(value);
        }

        public bool TryAdd(T entity)
        {
            var key = CreateKey(entity);
            var content = _converter.Convert(entity);

            var table = _service.Table(_entityMap.Name);
            if (!table.Storage.TryAdd(key, content))
                return false;

            table.Indexes.Add(key, entity);
            return true;
        }

        public bool TryUpdate(T entity)
        {
            var key = CreateKey(entity);
            var data = _converter.Convert(entity);

            var table = _service.Table(_entityMap.Name);
            if (!table.Storage.TryUpdate(key, data))
                return false;

            table.Indexes.Update(key, entity);
            return true;
        }

        public bool TryRemove(T entity)
        {
            var key = CreateKey(entity);

            var table = _service.Table(_entityMap.Name);
            if (!table.Storage.TryRemove(key))
                return false;

            table.Indexes.Remove(key);
            return true;
        }

        public void CommitModifications()
        {
            var table = _service.Table(_entityMap.Name);
            table.Indexes.CommitModifications();
        }

        public bool TryGet(object id, out T entity)
        {
            var key = Key.Create(id);
            byte[] content;
            if (!_service.TryGet(_entityMap.Name, key, out content))
            {
                entity = default(T);
                return false;
            }

            entity = _converter.Convert(content);
            return true;
        }

        public IEnumerable<T> All()
        {
            return _service.All(_entityMap.Name).Select(b => _converter.Convert(b)).ToList();
        }


        public IEnumerable<T> Match(EnigmaCriteria criteria)
        {
            return _service.Match(_entityMap.Name, criteria).Select(b => _converter.Convert(b)).ToList();
        }

    }
}
