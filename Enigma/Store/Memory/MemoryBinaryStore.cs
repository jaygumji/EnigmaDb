using Enigma.Store.Binary;
using System;
using System.IO;

namespace Enigma.Store.Memory
{
    /// <summary>
    /// Memory binary store that is used for system operations, optimized for performance
    /// </summary>
    public class MemoryBinaryStore : IBinaryStore, IDisposable
    {

        private readonly MemoryStream _stream;
        private readonly long? _maxSize;

        /// <summary>
        /// Creates a new instance of <see cref="MemoryBinaryStore"/>
        /// </summary>
        public MemoryBinaryStore() : this((long?) null)
        {
        }

        /// <summary>
        /// Creates a new instance of <see cref="MemoryBinaryStore"/>
        /// </summary>
        /// <param name="maxSize">The maximum size this store may contain</param>
        public MemoryBinaryStore(long? maxSize)
        {
            _stream = new MemoryStream();
            _maxSize = maxSize;
        }

        /// <summary>
        /// Creates a new instance of <see cref="MemoryBinaryStore"/>
        /// </summary>
        /// <param name="buffer">The buffer of this store</param>
        public MemoryBinaryStore(byte[] buffer)
        {
            _stream = new MemoryStream(buffer);
            _maxSize = buffer.LongLength;
        }

        public bool IsEmpty { get { return _stream.Length == 0; } }

        public bool IsSpaceAvailable(long length)
        {
            if (!_maxSize.HasValue) return true;

            return _stream.Length + length <= _maxSize.Value;
        }

        public void Write(long storeOffset, byte[] data)
        {
            _stream.Seek(storeOffset, SeekOrigin.Begin);
            _stream.Write(data, 0, data.Length);
        }

        public bool TryWrite(byte[] data, out long storeOffset)
        {
            if (!IsSpaceAvailable(data.Length))
            {
                storeOffset = 0;
                return false;
            }

            _stream.Seek(0, SeekOrigin.End);
            storeOffset = _stream.Length;
            _stream.Write(data, 0, data.Length);
            return true;
        }

        public byte[] ReadAll(out long offset)
        {
            offset = 0;
            return _stream.ToArray();
        }

        public byte[] Read(long storeOffset, long length)
        {
            var buffer = new byte[length];
            _stream.Seek(storeOffset, SeekOrigin.Begin);
            _stream.Read(buffer, 0, (int) length);
            return buffer;
        }

        public byte[] ToArray()
        {
            return _stream.ToArray();
        }

        public void Dispose()
        {
            _stream.Dispose();
        }

        public void TruncateTo(byte[] data)
        {
            _stream.Seek(0, SeekOrigin.Begin);
            _stream.Write(data, 0, data.Length);
        }
    }
}
