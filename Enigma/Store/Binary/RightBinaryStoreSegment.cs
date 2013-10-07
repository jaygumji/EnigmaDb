using Enigma.Store.Binary;

namespace Enigma.Store.Binary
{
    internal class RightBinaryStoreSegment : IBinaryStore
    {

        private readonly IDualBinaryStore _store;

        internal RightBinaryStoreSegment(IDualBinaryStore store)
        {
            _store = store;
        }

        public bool IsEmpty
        {
            get { return _store.IsRightEmpty; }
        }

        public bool IsSpaceAvailable(long length)
        {
            return _store.IsSpaceAvailable(length);
        }

        public void Write(long storeOffset, byte[] data)
        {
            _store.WriteRight(storeOffset, data);
        }

        public bool TryWrite(byte[] data, out long storeOffset)
        {
            return _store.TryWriteRight(data, out storeOffset);
        }

        public byte[] ReadAll(out long offset)
        {
            return _store.ReadAllRight(out offset);
        }

        public byte[] Read(long storeOffset, long length)
        {
            return _store.ReadRight(storeOffset, length);
        }

        public void TruncateTo(byte[] data)
        {
            _store.TruncateRightTo(data);
        }
    }
}
