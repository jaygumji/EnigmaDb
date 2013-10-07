using Enigma.Store.Binary;
using Enigma.Binary;
using System;

namespace Enigma.Store
{
    public class EntryWriter
    {
        private readonly IBinaryConverter<Entry> _converter;
        private IBinaryStore _store;

        public EntryWriter(IBinaryStore store)
        {
            _store = store;
            _converter = EntryBinaryConverter.Instance;
        }

        public void Write(Entry entry)
        {
            var value = _converter.Convert(entry);
            long offset;
            if (!_store.TryWrite(value, out offset))
                throw new OutOfSpaceException();

            entry.Offset = offset;
        }

        public void WriteRemove(Entry entry)
        {
            var offset = entry.Offset + EntryBinaryConverter.IsActiveValueOffset;
            var data = BitConverter.GetBytes(false);
            _store.Write(offset, data);
        }
    }
}
