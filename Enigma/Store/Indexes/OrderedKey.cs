using System;
using System.Collections.Generic;

namespace Enigma.Store.Indexes
{
    public class OrderedKey : IKey, IComparable<OrderedKey>
    {
        private readonly IKey _key;
        private readonly List<object> _values;
        private readonly OrderingDirection[] _directions;

        public OrderedKey(IKey key, OrderingDirection[] directions)
        {
            _key = key;
            _directions = directions;
            _values = new List<object>();
        }

        public void AddIndexedValue(object value)
        {
            _values.Add(value);
        }

        /// <summary>
        /// Compares the current object with another object of the same type.
        /// </summary>
        /// <param name="other">An object to compare with this object.</param>
        /// <returns>
        /// A value that indicates the relative order of the objects being compared. The return value has the following meanings: Value Meaning Less than zero This object is less than the <paramref name="other" /> parameter.Zero This object is equal to <paramref name="other" />. Greater than zero This object is greater than <paramref name="other" />.
        /// </returns>
        public int CompareTo(OrderedKey other)
        {
            for (var index = 0; index < _values.Count; index++) {
                var left = _values[index];
                var right = other._values[index];

                if (left == null && right == null) continue;

                if (DBNull.Value.Equals(left)) return -1;
                if (DBNull.Value.Equals(right)) return -1;

                if (left == null) return _directions[index] == OrderingDirection.Ascending ? - 1 : 1;
                if (right == null) return _directions[index] == OrderingDirection.Ascending ? 1 : -1;

                var comparableLeft = left as IComparable;
                if (comparableLeft == null) continue;

                var compareResult = comparableLeft.CompareTo(right);
                if (compareResult == 0) continue;

                if (_directions[index] == OrderingDirection.Descending)
                    compareResult *= -1;

                return compareResult;
            }
            return 0;
        }

        public byte[] Value { get { return _key.Value; } }

        public override int GetHashCode()
        {
            return _key.GetHashCode();
        }

        public override bool Equals(object obj)
        {
            return _key.Equals(obj);
        }
    }
}