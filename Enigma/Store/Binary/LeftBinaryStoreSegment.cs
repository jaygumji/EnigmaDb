using Enigma.Store.Binary;

namespace Enigma.Store.Binary
{
    internal class LeftBinaryStoreSegment : IBinaryStore
    {

        private readonly IDualBinaryStore _store;

        internal LeftBinaryStoreSegment(IDualBinaryStore store)
        {
            _store = store;
        }

        public bool IsEmpty
        {
            get { return _store.IsLeftEmpty; }
        }

        public bool IsSpaceAvailable(long length)
        {
            return _store.IsSpaceAvailable(length);
        }

        public void Write(long storeOffset, byte[] data)
        {
            _store.WriteLeft(storeOffset, data);
        }

        public bool TryWrite(byte[] data, out long storeOffset)
        {
            return _store.TryWriteLeft(data, out storeOffset);
        }

        public byte[] ReadAll(out long offset)
        {
            return _store.ReadAllLeft(out offset);
        }

        public byte[] Read(long storeOffset, long length)
        {
            return _store.ReadLeft(storeOffset, length);
        }

        public void TruncateTo(byte[] data)
        {
            _store.TruncateLeftTo(data);
        }
    }
}
