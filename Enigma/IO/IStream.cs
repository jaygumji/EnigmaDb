using System;

namespace Enigma.IO
{
    public interface IStream : IDisposable
    {
        long Length { get; }

        void Flush();
        void FlushForced();
    }
}
