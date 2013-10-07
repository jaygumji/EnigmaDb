using System;

namespace Enigma.Store
{
    public class OutOfSpaceException : Exception
    {
        public OutOfSpaceException()
            : base("The binary store ran out of space")
        {
        }
    }
}
