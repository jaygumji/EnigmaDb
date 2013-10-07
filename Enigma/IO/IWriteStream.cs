namespace Enigma.IO
{
    public interface IWriteStream : IReadStream
    {
        void Write(byte[] buffer, int offset, int count);
    }
}
