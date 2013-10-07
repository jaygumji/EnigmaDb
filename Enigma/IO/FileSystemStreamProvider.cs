using Enigma.IO;
using Enigma.Store.Binary;
using System;
using System.IO;

namespace Enigma.IO
{
    public class FileSystemStreamProvider : IStreamProvider
    {

        private readonly string _path;

        public FileSystemStreamProvider(string path)
        {
            _path = path;
        }

        public IWriteStream AcquireWriteStream()
        {
            var stream = new FileStream(_path, FileMode.OpenOrCreate, FileAccess.Write, FileShare.ReadWrite);
            return new PooledFileSystemStream(this, stream);
        }

        public IReadStream AcquireReadStream()
        {
            var stream = new FileStream(_path, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
            return new PooledFileSystemStream(this, stream);
        }

        public void Return(IStream stream)
        {
            var fileSystemStream = stream as PooledFileSystemStream;
            if (fileSystemStream == null)
                throw new ArgumentException("The stream parameter does not contain a stream handled by this provider");

            fileSystemStream.Stream.Dispose();
        }

        public void ClearReadBuffers()
        {
        }

        public void Dispose()
        {
        }

    }

    internal class PooledFileSystemStream : IWriteStream
    {

        private IStreamProvider _provider;
        private FileStream _stream;

        public PooledFileSystemStream(IStreamProvider provider, FileStream stream)
        {
            _provider = provider;
            _stream = stream;
        }

        public Stream Stream { get { return _stream; } }
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
            _stream.Flush(force);
        }
    }

}
