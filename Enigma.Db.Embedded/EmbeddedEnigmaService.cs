using Enigma.Db.Engine;
using Enigma.Db.Linq;
using Enigma.Modelling;
using Enigma.Store;
using Enigma.Store.FileSystem;
using Enigma.Store.Memory;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Enigma.Db.Embedded
{
    public class EmbeddedEnigmaService
    {

        private readonly ConcurrentDictionary<string, EntityTable> _tables;
        private readonly IStorageFactoryService _service;

        public EmbeddedEnigmaService(IStorageFactoryService service)
        {
            _tables = new ConcurrentDictionary<string, EntityTable>();
            _service = service;
        }

        public static EmbeddedEnigmaService CreateFileSystem(string baseDirectory)
        {
            return new EmbeddedEnigmaService(new FileSystemStorageFactoryService(baseDirectory, new CompositeStorageConfiguration()));
        }

        public static EmbeddedEnigmaService CreateFileSystem(string baseDirectory, CompositeStorageConfiguration configuration)
        {
            return new EmbeddedEnigmaService(new FileSystemStorageFactoryService(baseDirectory, configuration));
        }

        public static EmbeddedEnigmaService CreateMemory()
        {
            return new EmbeddedEnigmaService(new MemoryStorageFactoryService());
        }

        internal EntityTable Table(string name)
        {
            return _tables.GetOrAdd(name, n => {
                var storage = _service.CreateStorage(name);
                var indexes = _service.CreateIndexes(name);
                return new EntityTable(storage, indexes);
            });
        }

        public void Truncate()
        {
            var tables = _tables.Values.ToList();
            foreach (var table in tables)
                table.Storage.Maintenance.Truncate();
        }

        public bool TryAdd(string name, IKey key, byte[] content)
        {
            return Table(name).Storage.TryAdd(key, content);
        }

        public bool TryUpdate(string name, IKey key, byte[] content)
        {
            return Table(name).Storage.TryUpdate(key, content);
        }

        public bool TryRemove(string name, IKey key)
        {
            return Table(name).Storage.TryRemove(key);
        }

        public bool TryGet(string name, IKey key, out byte[] content)
        {
            return Table(name).Storage.TryGet(key, out content);
        }

        public IEnumerable<byte[]> All(string name)
        {
            return Table(name).Storage.All();
        }

        public IEnumerable<byte[]> Match(string name, EnigmaCriteria criteria)
        {
            var table = Table(name);
            if (criteria.IndexOperations.Count <= 0)
                return Table(name).Storage.All();

            var keys = table.Indexes.Match(criteria.IndexOperations);

            var result = new List<byte[]>();
            foreach (var key in keys) {
                byte[] content;
                if (table.Storage.TryGet(key, out content))
                    result.Add(content);
            }
            return result;
        }

        public void Synchronize(Model model)
        {
            _service.SynchronizeModel(model);
        }
    }
}
