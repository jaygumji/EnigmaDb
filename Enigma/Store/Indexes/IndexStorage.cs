using Enigma.Binary;
using Enigma.Reflection;
using Enigma.Store.Binary;
using Enigma.Store.Keys;
using System;
using System.Collections.Generic;

namespace Enigma.Store.Indexes
{
    public class IndexStorage<T> : IIndexStorage
        where T : IComparable<T>
    {
        private readonly IBinaryStore _store;
        private readonly ComparableIndex<T> _index;
        private readonly Dictionary<IKey, IDictionary<T, IndexEntry<T>>> _entries; 
        private SortedDictionary<T, IList<IKey>> _values;
        private readonly IBinaryInformation<T> _information;
        private readonly IBinaryInformation<Int32> _lengthInformation;
        private readonly IBinaryInformation<Boolean> _isActiveInformation;
        private readonly PropertyExtractor<T> _extractor;

        private bool _isModified = false;

        public IndexStorage(IBinaryStore store, IndexConfiguration configuration)
        {
            _store = store;
            _index = new ComparableIndex<T>(configuration.UniqueName);
            _information = BinaryInformation.Of<T>();
            _lengthInformation = BinaryInformation.Of<Int32>();
            _isActiveInformation = BinaryInformation.Of<Boolean>();
            _extractor = new PropertyExtractor<T>(configuration.EntityType, configuration.UniqueName);

            _entries = new Dictionary<IKey, IDictionary<T, IndexEntry<T>>>();
        }

        public void Initialize()
        {
            var values = new Dictionary<T, IList<IKey>>();
            LoadFromStore(_entries, values);
            _values = new SortedDictionary<T, IList<IKey>>(values);
            _index.Update(new ImmutableSortedCollection<T, IList<IKey>>(_values));
        }

        private void LoadFromStore(IDictionary<IKey, IDictionary<T, IndexEntry<T>>> entriesLookup, IDictionary<T, IList<IKey>> values)
        {
            long storeOffset;
            var bytes = _store.ReadAll(out storeOffset);
            var offset = 0;
            var valueLength = _information.FixedLength;

            while (offset < bytes.Length) {
                var entryOffset = offset;
                var isActive = _isActiveInformation.Converter.Convert(bytes, offset);
                offset += _isActiveInformation.FixedLength;

                if (!_information.IsFixedLength) {
                    valueLength = _lengthInformation.Converter.Convert(bytes, offset);
                    offset += _lengthInformation.FixedLength;
                }

                var keyLength = _lengthInformation.Converter.Convert(bytes, offset);
                offset += _lengthInformation.FixedLength;

                if (isActive) {
                    var value = _information.Converter.Convert(bytes, offset, valueLength);
                    offset += valueLength;
                    var key = Key.Create(bytes, offset, keyLength);
                    offset += keyLength;

                    IList<IKey> keys;
                    if (!values.TryGetValue(value, out keys)) {
                        keys = new List<IKey>();
                        values.Add(value, keys);
                    }
                    keys.Add(key);

                    IDictionary<T, IndexEntry<T>> entries;
                    if (!entriesLookup.TryGetValue(key, out entries)) {
                        entries = new Dictionary<T, IndexEntry<T>>();
                        entriesLookup.Add(key, entries);
                    }
                    entries.Add(value, new IndexEntry<T> {
                        Offset = entryOffset,
                        Value = value
                    });
                }
                else {
                    offset += valueLength + keyLength;
                }
            }
        }

        public IComparableIndex Index { get { return _index; } }
        public bool IsModified { get { return _isModified; } }

        public void Add(IKey key, object entity)
        {
            var newValues = _extractor.Extract(entity);

            IDictionary<T, IndexEntry<T>> entries;
            if (!_entries.TryGetValue(key, out entries)) {
                entries = new Dictionary<T, IndexEntry<T>>();
                _entries.Add(key, entries);
            }

            foreach (var value in newValues)
                AddValue(entries, key, value);

            _isModified = true;
        }

        public void Update(IKey key, object entity)
        {
            var newEntries = new Dictionary<T, IndexEntry<T>>();
            var newValues = _extractor.Extract(entity);
            IDictionary<T, IndexEntry<T>> oldEntries;
            if (_entries.TryGetValue(key, out oldEntries)) {
                foreach (var value in newValues) {
                    IndexEntry<T> entry;
                    if (oldEntries.TryGetValue(value, out entry)) {
                        newEntries.Add(value, entry);
                        oldEntries.Remove(value);
                    }
                    else {
                        AddValue(newEntries, key, value);
                        _isModified = true;
                    }
                }
                _entries[key] = newEntries;

                if (oldEntries.Count > 0) {
                    RemoveValues(key, oldEntries.Values);
                    _isModified = true;
                }
            }
            else
                Add(key, entity);

        }

        public void Remove(IKey entityId)
        {
            IDictionary<T, IndexEntry<T>> entries;
            if (_entries.TryGetValue(entityId, out entries)) {
                RemoveValues(entityId, entries.Values);
                _entries.Remove(entityId);
                _isModified = true;
            }
        }

        public void CommitModifications()
        {
            if (!_isModified) return;

            _isModified = false;
            _index.Update(new ImmutableSortedCollection<T, IList<IKey>>(_values));
        }

        public IEnumerable<IKey> Match(CompareOperation operation, object value)
        {
            switch (operation) {
                case CompareOperation.Equal:
                    return _index.Equal((T)value);
                case CompareOperation.NotEqual:
                    return _index.NotEqual((T)value);
                case CompareOperation.GreaterThan:
                    return _index.GreaterThan((T)value);
                case CompareOperation.GreaterThanOrEqual:
                    return _index.GreaterThanOrEqual((T)value);
                case CompareOperation.LessThan:
                    return _index.LessThan((T)value);
                case CompareOperation.LessThanOrEqual:
                    return _index.LessThanOrEqual((T)value);
                case CompareOperation.Contains:
                    return _index.Contains((IEnumerable<T>)value);
                default:
                    throw new System.InvalidOperationException("Comparable index does not have the operation " + operation.ToString());
            }
        }

        private void AddValue(IDictionary<T, IndexEntry<T>> entries, IKey key, T value)
        {
            var valueBytes = _information.Converter.Convert(value);

            var entryLength = (_information.IsFixedLength ? 0 : _lengthInformation.FixedLength) // Value length
                + _lengthInformation.FixedLength // Key length
                + _isActiveInformation.FixedLength // IsActive
                + key.Value.Length // Key
                + valueBytes.Length; // Value

            var entryData = new byte[entryLength];
            var offset = 0;

            _isActiveInformation.Converter.Convert(true, entryData, offset++);

            if (!_information.IsFixedLength) {
                _lengthInformation.Converter.Convert(valueBytes.Length, entryData, offset);
                offset += _lengthInformation.FixedLength;
            }

            _lengthInformation.Converter.Convert(key.Value.Length, entryData, offset);
            offset += _lengthInformation.FixedLength;

            Array.Copy(valueBytes, 0, entryData, offset, valueBytes.Length);
            offset += valueBytes.Length;

            Array.Copy(key.Value, 0, entryData, offset, key.Value.Length);

            long dataOffset;
            _store.TryWrite(entryData, out dataOffset);

            var entry = new IndexEntry<T> {
                Offset = (int)dataOffset,
                Value = value
            };

            entries.Add(value, entry);

            IList<IKey> keys;
            if (!_values.TryGetValue(value, out keys)) {
                keys = new List<IKey>();
                _values.Add(value, keys);
            }
            keys.Add(key);
        }

        private void RemoveValues(IKey key, IEnumerable<IndexEntry<T>> entries)
        {
            var removed = _isActiveInformation.Converter.Convert(false);
            foreach (var entry in entries) {
                _store.Write(entry.Offset, removed);
                IList<IKey> keys;
                if (_values.TryGetValue(entry.Value, out keys))
                    keys.Remove(key);
            }
        }

    }
}
