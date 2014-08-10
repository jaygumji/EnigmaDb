using System;
using System.Collections.Generic;
using System.Linq;

namespace Enigma.Scheduling
{
    public class DateTimeQueue<T>
    {

        private readonly SortedDictionary<DateTime, List<T>> _entries; 

        public DateTimeQueue()
        {
            _entries = new SortedDictionary<DateTime, List<T>>();
        }

        public bool IsEmpty { get { return _entries.Count == 0; } }

        public void Enqueue(DateTime when, T value)
        {
            List<T> list;
            lock (_entries) {
                if (!_entries.TryGetValue(when, out list)) {
                    list = new List<T>();
                    _entries.Add(when, list);
                }
            }
            lock (list)
                list.Add(value);
        }

        public bool TryDequeue(out IEnumerable<T> values)
        {
            if (_entries.Count == 0) {
                values = null;
                return false;
            }

            var now = DateTime.Now;
            KeyValuePair<DateTime, List<T>> kv;
            lock (_entries) {
                if (_entries.Count == 0) {
                    values = null;
                    return false;
                }

                kv = _entries.First();
                if (kv.Key > now) {
                    values = null;
                    return false;
                }
                _entries.Remove(kv.Key);
            }

            values = kv.Value;
            return true;
        }

        public bool TryPeekNextEntryAt(out DateTime nextAt)
        {
            if (_entries.Count == 0) {
                nextAt = default(DateTime);
                return false;
            }

            KeyValuePair<DateTime, List<T>> kv;
            lock (_entries) {
                if (_entries.Count == 0) {
                    nextAt = default(DateTime);
                    return false;
                }

                kv = _entries.First();
            }

            nextAt = kv.Key;
            return true;
        }

    }
}
