namespace Enigma.Store.Binary
{
    public interface IBinaryStore
    {
        bool IsEmpty { get; }

        bool IsSpaceAvailable(long length);

        void Write(long storeOffset, byte[] data);
        bool TryWrite(byte[] data, out long storeOffset);

        byte[] ReadAll(out long offset);
        byte[] Read(long storeOffset, long length);

        void TruncateTo(byte[] data);
    }
}
