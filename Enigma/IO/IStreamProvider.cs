using System;
using System.IO;

namespace Enigma.IO
{
    public interface IStreamProvider : IDisposable
    {

        IWriteStream AcquireWriteStream();
        IReadStream AcquireReadStream();

        void Return(IStream stream);

        void ClearReadBuffers();
    }
}
