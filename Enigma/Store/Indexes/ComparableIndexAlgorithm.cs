using Enigma.Binary;
using Enigma.Binary.Algorithm;
using System;
using System.Linq;
using System.Collections.Generic;
using Enigma.Store.Keys;

namespace Enigma.Store.Indexes
{
    public class ComparableIndexAlgorithm<T> : IComparableIndexAlgorithm<T>
        where T : IComparable<T>
    {
        private readonly IBinaryConverter<T> _converter;
        private ImmutableSortedCollection<T, IList<IKey>> _indexedValues;

        public ComparableIndexAlgorithm()
        {
            var information = BinaryInformation.Of<T>();
            _converter = information.Converter;
        }

        public void Update(ImmutableSortedCollection<T, IList<IKey>> indexedValues)
        {
            _indexedValues = indexedValues;
        }

        public IEnumerable<IKey> Equal(T value)
        {
            var indexedValues = _indexedValues;
            IList<IKey> entries;
            return indexedValues.TryGetValue(value, out entries) ? entries : Key.EmptyKeys;
        }

        public IEnumerable<IKey> NotEqual(T value)
        {
            var indexedValues = _indexedValues;
            return (from v in indexedValues.Keys
                    where v.CompareTo(value) != 0
                    select v).SelectMany(v => indexedValues[v]).Distinct().ToList();
        }

        public IEnumerable<IKey> GreaterThan(T value)
        {
            var indexedValues = _indexedValues;
            var length = indexedValues.Keys.Count;
            var startIndex = BinarySearch.Search(indexedValues.Keys, value);

            if (startIndex >= 0)
                startIndex++;
            else
                startIndex = ~startIndex;

            var result = new List<IKey>();
            var firstKey = indexedValues.Keys[startIndex];
            if (firstKey.CompareTo(value) > 0) result.AddRange(indexedValues[firstKey]);
            for (var index = startIndex; index < length; index++) {
                var key = indexedValues.Keys[index];
                var entries = indexedValues[key];
                result.AddRange(entries);
            }
            return result.Distinct();
        }

        public IEnumerable<IKey> GreaterThanOrEqual(T value)
        {
            var indexedValues = _indexedValues;
            var length = indexedValues.Keys.Count;
            var startIndex = BinarySearch.Search(indexedValues.Keys, value);

            if (startIndex < 0)
                startIndex = ~startIndex;

            var result = new List<IKey>();
            var firstKey = indexedValues.Keys[startIndex];
            if (firstKey.CompareTo(value) >= 0) result.AddRange(indexedValues[firstKey]);
            for (var index = startIndex + 1; index < length; index++) {
                var key = indexedValues.Keys[index];
                var entries = indexedValues[key];
                result.AddRange(entries);
            }
            return result.Distinct();
        }

        public IEnumerable<IKey> LessThan(T value)
        {
            var indexedValues = _indexedValues;
            var startIndex = BinarySearch.Search(indexedValues.Keys, value);

            if (startIndex >= 0)
                startIndex--;
            else
                startIndex = ~startIndex;

            var result = new List<IKey>();
            for (var index = 0; index < startIndex; index++) {
                var key = indexedValues.Keys[index];
                var entries = indexedValues[key];
                result.AddRange(entries);
            }
            var lastKey = indexedValues.Keys[startIndex];
            if (lastKey.CompareTo(value) < 0) result.AddRange(indexedValues[lastKey]);
            return result.Distinct();
        }

        public IEnumerable<IKey> LessThanOrEqual(T value)
        {
            var indexedValues = _indexedValues;
            var startIndex = BinarySearch.Search(indexedValues.Keys, value);

            if (startIndex < 0) startIndex = ~startIndex - 1;

            var result = new List<IKey>();
            for (var index = 0; index < startIndex; index++) {
                var key = indexedValues.Keys[index];
                var entries = indexedValues[key];
                result.AddRange(entries);
            }
            var lastKey = indexedValues.Keys[startIndex];
            if (lastKey.CompareTo(value) <= 0) result.AddRange(indexedValues[lastKey]);
            return result.Distinct();
        }

        public IEnumerable<IKey> Contains(IEnumerable<T> values)
        {
            var indexedValues = _indexedValues;
            return values.SelectMany(value => {
                IList<IKey> entries;
                if (!indexedValues.TryGetValue(value, out entries))
                    return Key.EmptyKeys;

                return entries;
            }).Distinct().ToList();
        }

        IEnumerable<IKey> IComparableIndexAlgorithm.Equal(byte[] value)
        {
            return Equal(_converter.Convert(value));
        }

        IEnumerable<IKey> IComparableIndexAlgorithm.NotEqual(byte[] value)
        {
            return NotEqual(_converter.Convert(value));
        }

        IEnumerable<IKey> IComparableIndexAlgorithm.GreaterThan(byte[] value)
        {
            return GreaterThan(_converter.Convert(value));
        }

        IEnumerable<IKey> IComparableIndexAlgorithm.GreaterThanOrEqual(byte[] value)
        {
            return GreaterThanOrEqual(_converter.Convert(value));
        }

        IEnumerable<IKey> IComparableIndexAlgorithm.LessThan(byte[] value)
        {
            return LessThan(_converter.Convert(value));
        }

        IEnumerable<IKey> IComparableIndexAlgorithm.LessThanOrEqual(byte[] value)
        {
            return LessThanOrEqual(_converter.Convert(value));
        }

        IEnumerable<IKey> IComparableIndexAlgorithm.Contains(IEnumerable<byte[]> values)
        {
            return Contains(values.Select(value => _converter.Convert(value)));
        }

    }
}
