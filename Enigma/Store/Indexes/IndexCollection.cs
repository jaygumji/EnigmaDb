using System;
using Enigma.Db.Linq;
using Enigma.Store.Binary;
using System.Collections.Generic;
using System.Linq;
using Enigma.Store.Keys;

namespace Enigma.Store.Indexes
{
    public class IndexCollection : IIndexCollection
    {

        private readonly IIndexConfigurator _configurator;
        private readonly Dictionary<string, IIndexStorage> _storages;

        public IndexCollection(IIndexConfigurator configurator)
        {
            _configurator = configurator;
            _storages = new Dictionary<string, IIndexStorage>();

            var indexStorageType = typeof(IndexStorage<>);
            foreach (var details in _configurator.Indexes) {
                var streamProvider = _configurator.GetStreamProvider(details.UniqueName);
                var store = new BinaryStore(streamProvider);

                var indexType = details.Type.IsEnum
                    ? Enum.GetUnderlyingType(details.Type)
                    : details.Type;

                var type = indexStorageType.MakeGenericType(indexType);
                var indexStorage = (IIndexStorage) Activator.CreateInstance(type, store, details);
                indexStorage.Initialize();
                _storages.Add(details.UniqueName, indexStorage);
            }
        }

        public bool IsModified { get { return _storages.Any(s => s.Value.IsModified); } }

        public void Add(IKey key, object entity)
        {
            foreach (var storage in _storages.Values)
                storage.Add(key, entity);
        }

        public void Update(IKey key, object entity)
        {
            foreach (var storage in _storages.Values)
                storage.Update(key, entity);
        }

        public void Remove(IKey entityId)
        {
            foreach (var storage in _storages.Values)
                storage.Remove(entityId);
        }

        public void CommitModifications()
        {
            foreach (var storage in _storages.Values)
                storage.CommitModifications();
        }

        public IEnumerable<IKey> Match(IEnumerable<EnigmaIndexOperation> indexOperations)
        {
            var keys = new List<IEnumerable<IKey>>();
            foreach (var indexOperation in indexOperations) {
                IIndexStorage storage;
                if (_storages.TryGetValue(indexOperation.UniqueName, out storage))
                    keys.Add(storage.Match(indexOperation.Operation, indexOperation.Value));
            }

            if (keys.Count == 0) return new IKey[] {};
            if (keys.Count == 1) return keys[0];

            // TODO: This might need to be rewritten to improve performance
            return keys.Aggregate((agg, nextKeys) => agg.Intersect(nextKeys)).ToList();
        }
    }
}
