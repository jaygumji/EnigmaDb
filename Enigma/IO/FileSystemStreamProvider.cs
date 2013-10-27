using System;
using System.IO;

namespace Enigma.IO
{
    public class FileSystemStreamProvider : IStreamProvider
    {

        private readonly string _path;
        private readonly StreamProviderSourceState _sourceState;

        public FileSystemStreamProvider(string path)
        {
            _path = path;
            _sourceState = File.Exists(path)
                ? StreamProviderSourceState.Reconnected
                : StreamProviderSourceState.Created;
        }

        public StreamProviderSourceState SourceState { get { return _sourceState; } }

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
            // If we're going with a set amount of readers later on,
            // this should clear the read buffer of all the readers
        }

        public void Dispose()
        {
            // If we're going with a set amount of readers/writers,
            // this should dispose of all the readers/writers
        }

    }
}
