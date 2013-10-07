using System;

namespace Enigma.Binary
{
    public class BufferOverflowException : Exception
    {

        public BufferOverflowException(string message, Exception innerException) : base(message, innerException) { }
        public BufferOverflowException(string message) : base(message) { }

    }
}
