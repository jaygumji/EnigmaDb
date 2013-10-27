using System;
using Enigma.Db.Linq;
using Enigma.IO;
using Enigma.Store.Binary;
using System.Collections.Generic;
using System.Linq;
using Enigma.Store.Keys;

namespace Enigma.Store.Indexes
{

    public class IndexCollection : IIndexCollection
    {

        private readonly IIndexConfigurator _configurator;
        private readonly Dictionary<string, ITableIndex> _indexes;
        private readonly List<ITableIndex> _indexesRequireRepairs; 

        public IndexCollection(IIndexConfigurator configurator)
        {
            _configurator = configurator;
            _indexes = new Dictionary<string, ITableIndex>();
            _indexesRequireRepairs = new List<ITableIndex>();

            var tableIndexFactory = new TableIndexFactory();
            foreach (var details in _configurator.Indexes) {
                var streamProvider = _configurator.GetStreamProvider(details.UniqueName);

                var tableIndex = tableIndexFactory.AsComparable(streamProvider, details);
                _indexes.Add(details.UniqueName, tableIndex);

                if (streamProvider.SourceState == StreamProviderSourceState.Created)
                    _indexesRequireRepairs.Add(tableIndex);
            }
        }

        public bool IsModified { get { return _indexes.Any(s => s.Value.Storage.IsModified); } }

        public bool NeedsRepair()
        {
            return _indexesRequireRepairs.Count > 0;
        }

        public void MarkAsRepaired()
        {
            _indexesRequireRepairs.Clear();
        }

        public void Repair(IKey key, object entity)
        {
            foreach (var index in _indexesRequireRepairs)
                index.Storage.Add(key, entity);
        }

        public void Add(IKey key, object entity)
        {
            foreach (var index in _indexes.Values)
                index.Storage.Add(key, entity);
        }

        public void Update(IKey key, object entity)
        {
            foreach (var index in _indexes.Values)
                index.Storage.Update(key, entity);
        }

        public void Remove(IKey entityId)
        {
            foreach (var index in _indexes.Values)
                index.Storage.Remove(entityId);
        }

        public void CommitModifications()
        {
            foreach (var index in _indexes.Values)
                index.CommitModifications();
        }

        public IEnumerable<IKey> Match(IEnumerable<EnigmaIndexOperation> indexOperations)
        {
            var keys = new List<IEnumerable<IKey>>();
            foreach (var indexOperation in indexOperations) {
                ITableIndex storage;
                if (_indexes.TryGetValue(indexOperation.UniqueName, out storage))
                    keys.Add(storage.Match(indexOperation.Operation, indexOperation.Value));
            }

            if (keys.Count == 0) return Key.EmptyKeys;
            if (keys.Count == 1) return keys[0];

            // TODO: This might need to be rewritten to improve performance
            return keys.Aggregate((agg, nextKeys) => agg.Intersect(nextKeys)).ToList();
        }
    }
}
