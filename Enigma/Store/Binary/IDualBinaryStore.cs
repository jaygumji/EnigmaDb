namespace Enigma.Store.Binary
{
    public interface IDualBinaryStore
    {
        bool IsLeftEmpty { get; }
        bool IsRightEmpty { get; }

        IBinaryStore Left { get; }
        IBinaryStore Right { get; }

        bool IsSpaceAvailable(long length);

        void WriteLeft(long storeOffset, byte[] data);
        void WriteRight(long storeOffset, byte[] data);
        bool TryWriteLeft(byte[] data, out long storeOffset);
        bool TryWriteRight(byte[] data, out long storeOffset);

        byte[] ReadAllLeft(out long offset);
        byte[] ReadAllRight(out long offset);

        byte[] ReadLeft(long storeOffset, long length);
        byte[] ReadRight(long storeOffset, long length);

        void TruncateRightTo(byte[] data);
        void TruncateLeftTo(byte[] data);
    }
}
