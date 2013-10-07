using System.IO;

namespace Enigma.IO
{
    public interface IReadStream : IStream
    {
        long Seek(long offset, SeekOrigin origin);
        int Read(byte[] buffer, int offset, int count);
    }
}
