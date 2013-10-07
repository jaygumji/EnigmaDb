using Enigma.IO;
using Enigma.Store.Binary;
using System;
using System.IO;

namespace Enigma.Store.Binary
{
    public class BinaryStore : IBinaryStore, IDisposable
    {
        private readonly IWriteStream _writeStream;
        private readonly IWriteStream _offsetWriteStream;
        private readonly IStreamProvider _provider;
        private long _currentOffset;
        private long _lastFlushOffset;
        private readonly long _start;
        private readonly long _length;
        private readonly bool _canGrow;
        private readonly object _writeLock = new object();

        public BinaryStore(IStreamProvider provider)
            : this(provider, 0, 0, true)
        {
        }

        public BinaryStore(IStreamProvider provider, long start, long maxLength)
            : this(provider, start, maxLength, false)
        {
        }

        private BinaryStore(IStreamProvider provider, long start, long length, bool canGrow)
        {
            _writeStream = provider.AcquireWriteStream();

            _provider = provider;
            _start = start;
            _length = length;
            _canGrow = canGrow;

            if (_writeStream.Length > start)
            {
                using (var readStream = _provider.AcquireReadStream())
                {
                    readStream.Seek(start, SeekOrigin.Begin);
                    var offsetBuffer = new byte[8];
                    readStream.Read(offsetBuffer, 0, offsetBuffer.Length);
                    _currentOffset = BitConverter.ToInt64(offsetBuffer, 0);
                }
            }
            else
            {
                _currentOffset = 8;
                var offsetBuffer = BitConverter.GetBytes(_currentOffset);
                _writeStream.Seek(start, SeekOrigin.Begin);
                _writeStream.Write(offsetBuffer, 0, offsetBuffer.Length);

                if (length > 0)
                {
                    var requiredFileSize = _start + _length;
                    var buffer = new byte[requiredFileSize - _writeStream.Length - 8];
                    _writeStream.Write(buffer, 0, buffer.Length);
                }
            }
            
            _lastFlushOffset = _currentOffset;
            _writeStream.Seek(start + _currentOffset, SeekOrigin.Begin);

            _offsetWriteStream = provider.AcquireWriteStream();
            _offsetWriteStream.Seek(start, SeekOrigin.Begin);
        }

        public bool IsEmpty { get { return _writeStream.Length == 0; } }

        public bool IsSpaceAvailable(long length)
        {
            if (_length <= 0) return true;

            return _currentOffset + length <= _length;
        }

        private void UpdateOffset()
        {
            var offsetBuffer = BitConverter.GetBytes(_currentOffset);
            _offsetWriteStream.Write(offsetBuffer, 0, offsetBuffer.Length);
            _offsetWriteStream.Seek(-8, SeekOrigin.Current);
        }

        public void Write(long storeOffset, byte[] data)
        {
            lock (_writeLock)
            {
                _writeStream.Seek(storeOffset, SeekOrigin.Begin);
                _writeStream.Write(data, 0, data.Length);
                _writeStream.Seek(0, SeekOrigin.End);
            }
        }

        public bool TryWrite(byte[] data, out long storeOffset)
        {
            lock (_writeLock)
            {
                if (!IsSpaceAvailable(data.Length))
                {
                    storeOffset = 0;
                    return false;
                }

                storeOffset = _currentOffset;
                _writeStream.Write(data, 0, data.Length);
                _currentOffset += data.Length;
                UpdateOffset();
                return true;
            }
        }

        private void EnsureFlushed(long offset)
        {
            if (offset < _lastFlushOffset) return;

            lock (_writeLock)
            {
                if (offset < _lastFlushOffset) return;

                _writeStream.Flush(true);
                _lastFlushOffset = _currentOffset;
            }
        }

        public byte[] ReadAll(out long offset)
        {
            offset = 0;
            EnsureFlushed(_currentOffset);

            if (_currentOffset <= _start + 8) return new byte[] {};

            var buffer = new byte[_currentOffset - _start - 8];
            using (var readStream = _provider.AcquireReadStream())
            {
                readStream.Seek(_start + 8, SeekOrigin.Begin);
                readStream.Read(buffer, 0, buffer.Length);
            }
            return buffer;
        }

        public byte[] Read(long storeOffset, long length)
        {
            EnsureFlushed(storeOffset);

            var buffer = new byte[length];
            using (var readStream = _provider.AcquireReadStream())
            {
                readStream.Seek(storeOffset, SeekOrigin.Begin);
                readStream.Read(buffer, 0, buffer.Length);
            }
            return buffer;
        }

        public void TruncateTo(byte[] data)
        {
            lock (_writeLock)
            {
                _currentOffset = 8 + data.Length;
                var offsetBuffer = BitConverter.GetBytes(_currentOffset);
                _writeStream.Seek(_start, SeekOrigin.Begin);
                _writeStream.Write(offsetBuffer, 0, offsetBuffer.Length);
                _writeStream.Write(data, 0, data.Length);
                _lastFlushOffset = 8;
            }
        }

        public void Dispose()
        {
            _writeStream.Dispose();
            _provider.Dispose();
            _offsetWriteStream.Dispose();
        }

    }
}
