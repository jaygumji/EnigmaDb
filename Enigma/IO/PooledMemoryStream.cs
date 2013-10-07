using System.IO;

namespace Enigma.IO
{
    internal class PooledMemoryStream : IWriteStream
    {

        private IStreamProvider _provider;
        private readonly MemoryStream _stream;

        public PooledMemoryStream(IStreamProvider provider, MemoryStream stream)
        {
            _provider = provider;
            _stream = stream;
        }

        public MemoryStream Stream { get { return _stream; } }

        public long Length { get { return _stream.Length; } }

        public long Seek(long offset, SeekOrigin origin)
        {
            return _stream.Seek(offset, origin);
        }

        public int Read(byte[] buffer, int offset, int count)
        {
            return _stream.Read(buffer, offset, count);
        }

        public void Dispose()
        {
            _provider.Return(this);
        }

        public void Write(byte[] buffer, int offset, int count)
        {
            _stream.Write(buffer, offset, count);
        }

        public void Flush(bool force)
        {
            _stream.Flush();
        }
    }
}
