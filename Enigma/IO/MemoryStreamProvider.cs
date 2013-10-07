using System;
using System.IO;
namespace Enigma.IO
{
    public class MemoryStreamProvider : IStreamProvider
    {

        private readonly byte[] _buffer;

        public MemoryStreamProvider(int capacity)
            : this(new byte[capacity])
        {
        }

        public MemoryStreamProvider(byte[] buffer)
        {
            _buffer = buffer;
        }

        public IWriteStream AcquireWriteStream()
        {
            return new PooledMemoryStream(this, new MemoryStream(_buffer));
        }

        public IReadStream AcquireReadStream()
        {
            return new PooledMemoryStream(this, new MemoryStream(_buffer));
        }

        public void Return(IStream stream)
        {
            var pooledMemoryStream = stream as PooledMemoryStream;
            if (pooledMemoryStream == null)
                throw new ArgumentException("The stream was not acquired by this stream provider");

            pooledMemoryStream.Stream.Dispose();
        }

        public void ClearReadBuffers()
        {
        }

        public byte[] ToArray()
        {
            return _buffer;
        }

        public void Dispose()
        {
        }
    }
}
