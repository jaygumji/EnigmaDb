using System.Collections.Generic;
using System.Linq;

namespace Enigma.Store.Indexes
{
    public class ImmutableSortedCollection<TKey, TValue>
    {

        private readonly List<TKey> _keys;
        private readonly Dictionary<TKey, TValue> _dictionary;

        public ImmutableSortedCollection(SortedList<TKey, TValue> sortedList)
        {
            _keys = sortedList.Keys.ToList();
            _dictionary = new Dictionary<TKey, TValue>(sortedList);
        }

        public ImmutableSortedCollection(SortedDictionary<TKey, TValue> sortedDictionary)
        {
            _keys = sortedDictionary.Keys.ToList();
            _dictionary = new Dictionary<TKey, TValue>(sortedDictionary);
        }

        public int Count { get { return _dictionary.Count; } }
        public IList<TKey> Keys { get { return _keys; } }

        public TValue this[TKey key] { get { return _dictionary[key]; } }

        public int BinarySearchKey(TKey key)
        {
            return Enigma.Binary.Algorithm.BinarySearch.Search(_keys, key);
        }

        public bool TryGetValue(TKey key, out TValue value)
        {
            return _dictionary.TryGetValue(key, out value);
        }
    }
}
