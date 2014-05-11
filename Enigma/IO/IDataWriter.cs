using System;
using Enigma.Binary.Information;

namespace Enigma.IO
{
    public interface IDataWriter
    {
        void Write(Byte value);
        void Write(Int16 value);
        void Write(Int32 value);
        void Write(Int64 value);
        void Write(UInt16 value);
        void Write(UInt32 value);
        void Write(UInt64 value);
        void Write(Boolean value);
        void Write(Single value);
        void Write(Double value);
        void Write(Decimal value);
        void Write(TimeSpan value);
        void Write(DateTime value);
        void Write(String value);
        void Write(Guid value);
        void Write(byte[] value);
    }
}
