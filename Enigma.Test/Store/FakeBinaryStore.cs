using Enigma.Store.Binary;
using System.IO;

namespace Enigma.Test.Store
{
    public class FakeBinaryStore : IBinaryStore
    {

        private readonly MemoryStream _stream;
        private long _currentOffset;

        public FakeBinaryStore() : this(null)
        {
        }

        public FakeBinaryStore(byte[] data)
        {
            _stream = new MemoryStream();
            if (data != null && data.Length > 0)
                _stream.Write(data, 0, data.Length);
            _currentOffset = _stream.Length;
        }

        public bool IsEmpty { get { return _stream.Length == 0; } }

        public long CurrentOffset { get { return _currentOffset; } }
        public MemoryStream Stream { get { return _stream; } }
        public long Length { get { return _stream.Length; } }


        public bool IsSpaceAvailable(long length)
        {
            return true;
        }

        public bool TryWrite(byte[] data, out long storeOffset)
        {
            Write(data, out storeOffset);
            return true;
        }
        
        public void Write(byte[] data, out long storeOffset)
        {
            storeOffset = _currentOffset;
            _stream.Write(data, 0, data.Length);
            _currentOffset += data.Length;
        }

        public void Write(long storeOffset, byte[] data)
        {
            _stream.Seek(storeOffset, SeekOrigin.Begin);
            _stream.Write(data, 0, data.Length);
            _stream.Seek(0, SeekOrigin.End);
        }

        public byte[] ReadAll(out long offset)
        {
            offset = 0;
            var buffer = new byte[_stream.Length];
            _stream.Seek(0, SeekOrigin.Begin);
            _stream.Read(buffer, 0, buffer.Length);
            return buffer;
        }

        public byte[] Read(long storeOffset, long length)
        {
            var buffer = new byte[length];
            _stream.Seek(storeOffset, SeekOrigin.Begin);
            _stream.Read(buffer, 0, buffer.Length);
            return buffer;
        }

        public void TruncateTo(byte[] data)
        {
            _stream.Seek(0, SeekOrigin.Begin);
            _stream.Write(data, 0, data.Length);
        }
    }
}
