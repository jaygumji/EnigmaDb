using System;

namespace Enigma.Serialization
{
    public class UnexpectedLengthException : Exception
    {
        public UnexpectedLengthException(ReadVisitArgs args, uint length) : this(args.Name, args.Index, length) { }

        public UnexpectedLengthException(string name, uint index, uint length) : base(string.Format("Unexpected length of {0}, index {1}, value was {2}", name, index, length))
        {
        }
    }
}