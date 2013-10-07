using Enigma.Store.Binary;
using System.Collections.Concurrent;
using System.Collections.Generic;

namespace Enigma.Store
{
    public class TableOfContent : ITableOfContentManager
    {
        private readonly ConcurrentDictionary<IKey, Entry> _entries;
        private readonly IBinaryStore _store;
        private readonly EntryWriter _entryWriter;

        public TableOfContent(IBinaryStore store, IDictionary<IKey, Entry> entries)
        {
            _entries = new ConcurrentDictionary<IKey, Entry>(entries);
            _store = store;
            _entryWriter = new EntryWriter(store);
        }

        public static TableOfContent From(IBinaryStore store)
        {
            bool isFragmented;
            return From(store, out isFragmented);
        }

        public static TableOfContent From(IBinaryStore store, out bool isFragmented)
        {
            var entries = new Dictionary<IKey, Entry>();
            if (store.IsEmpty)
            {
                isFragmented = false;
                return new TableOfContent(store, entries);
            }
            var entryReader = new EntryReader(store);
            Entry entry;
            while (entryReader.TryGetNext(out entry))
                entries.Add(entry.Key, entry);

            isFragmented = entryReader.IsFragmented;

            return new TableOfContent(store, entries);
        }

        public bool TryReserve(IKey key, out Entry entry)
        {
            entry = new Entry { Key = key, IsReserved = true };
            if (!_entries.TryAdd(key, entry))
            {
                entry = null;
                return false;
            }
            return true;
        }

        public void Enable(Entry entry)
        {
            entry.IsReserved = false;
            _entryWriter.Write(entry);
        }

        public bool TryRemove(IKey key)
        {
            Entry entry;
            if (!_entries.TryRemove(key, out entry))
                return false;

            _entryWriter.WriteRemove(entry);
            return true;
        }

        public int Count { get { return _entries.Count; } }

        public IEnumerable<Entry> Entries { get { return _entries.Values; } }

        public bool TryGet(IKey key, out IEntry entry)
        {
            Entry tempEntry;
            var result = _entries.TryGetValue(key, out tempEntry) && !tempEntry.IsReserved;
            entry = result ? tempEntry : null;
            return result;
        }

        public bool Contains(IKey key)
        {
            Entry entry;
            return _entries.TryGetValue(key, out entry) && !entry.IsReserved;
        }
    }
}
