using Enigma.Binary;
using Enigma.Db.Engine;
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
        private readonly EntityTable _table;

        public EmbeddedEnigmaEntityEngine(EntityMap<T> entityMap, EmbeddedEnigmaService service, IBinaryConverter<T> converter)
        {
            _entityMap = entityMap;
            _service = service;
            _converter = converter;

            _keyProperty = _entityMap.GetKeyProperty();
            _table = service.Table(entityMap.Name);
            RepairIndexesIfNeeded();
        }

        private void RepairIndexesIfNeeded()
        {
            if (_table.Indexes.NeedsRepair())
                RepairIndexes();
        }

        private void RepairIndexes()
        {
            var entities = All();
            foreach (var entity in entities) {
                var key = CreateKey(entity);
                _table.Indexes.Repair(key, entity);
            }
            _table.Indexes.MarkAsRepaired();
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

            if (!_table.Storage.TryAdd(key, content))
                return false;

            if (_service.Configuration.Engine.UpdateIndexesInBackground)
                _service.BackgroundQueue.Enqueue(() => _table.Indexes.Add(key, entity));
            else
                _table.Indexes.Add(key, entity);

            return true;
        }

        public bool TryUpdate(T entity)
        {
            var key = CreateKey(entity);
            var data = _converter.Convert(entity);

            if (!_table.Storage.TryUpdate(key, data))
                return false;

            if (_service.Configuration.Engine.UpdateIndexesInBackground)
                _service.BackgroundQueue.Enqueue(() => _table.Indexes.Update(key, entity));
            else
                _table.Indexes.Update(key, entity);

            return true;
        }

        public bool TryRemove(T entity)
        {
            var key = CreateKey(entity);

            if (!_table.Storage.TryRemove(key))
                return false;

            if (_service.Configuration.Engine.UpdateIndexesInBackground)
                _service.BackgroundQueue.Enqueue(() => _table.Indexes.Remove(key));
            else
                _table.Indexes.Remove(key);

            return true;
        }

        public void CommitModifications()
        {
            if (_service.Configuration.Engine.UpdateIndexesInBackground)
                _service.BackgroundQueue.Enqueue(() => _table.Indexes.CommitModifications());
            else
                _table.Indexes.CommitModifications();
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
            return Range(0, 0);
        }

        public IEnumerable<T> Range(int skip, int take)
        {
            var storage = _service.Table(_entityMap.Name).Storage;
            var entries = storage.TableOfContent.Entries;
            if (skip > 0)
                entries = entries.Skip(skip);
            if (take > 0)
                entries = entries.Take(take);

            var result = new List<T>();
            foreach (var entry in entries) {
                byte[] content;
                if (storage.TryGet(entry.Key, out content))
                    result.Add(_converter.Convert(content));
            }
            return result;
        }

        public bool TryResolve(IEnigmaExpressionTreeExecutor executor, out IEnumerable<T> values)
        {
            IEnumerable<IKey> keys;
            if (!executor.TryResolve(out keys)) {
                values = null;
                return false;
            }

            var result = new List<T>();
            var storage = _service.Table(_entityMap.Name).Storage;
            foreach (var key in keys) {
                byte[] content;
                if (storage.TryGet(key, out content))
                    result.Add(_converter.Convert(content));
            }
            values = result;
            return true;
        }

    }
}
