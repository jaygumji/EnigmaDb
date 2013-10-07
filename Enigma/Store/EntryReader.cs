using Enigma.Store.Binary;
using Enigma.Binary;
using System;

namespace Enigma.Store
{
    public class EntryReader
    {
        private readonly byte[] _data;
        private readonly long _start;
        private int _index;
        private readonly IBinaryConverter<Entry> _converter;
        private bool _isFragmented;

        public EntryReader(IBinaryStore store)
        {
            _data = store.ReadAll(out _start);
            _converter = EntryBinaryConverter.Instance;
        }

        public bool IsFragmented { get { return _isFragmented; } }

        public bool TryGetNext(out Entry entry)
        {
            if (_data.Length <= _index) {
                entry = null;
                return false;
            }

            if (!EntryBinaryConverter.IsActive(_data, _index))
            {
                entry = null;
                _isFragmented = true;
                return false;
            }

            entry = _converter.Convert(_data, _index);
            entry.Offset = _start + _index;
            _index += EntryBinaryConverter.GetLength(entry.Key);
            return true;
        }
    }
}
