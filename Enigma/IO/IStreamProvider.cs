using System;
using System.IO;

namespace Enigma.IO
{
    public interface IStreamProvider : IDisposable
    {
        StreamProviderSourceState SourceState { get; }

        IWriteStream AcquireWriteStream();
        IReadStream AcquireReadStream();

        void Return(IStream stream);

        void ClearReadBuffers();
    }
}
