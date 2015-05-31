using System;

namespace Enigma.IO
{
    public interface IDataReader
    {
        Byte ReadByte();
        Int16 ReadInt16();
        Int32 ReadInt32();
        Int64 ReadInt64();
        UInt16 ReadUInt16();
        UInt32 ReadUInt32();
        UInt64 ReadUInt64();
        Boolean ReadBoolean();
        Single ReadSingle();
        Double ReadDouble();
        Decimal ReadDecimal();
        TimeSpan ReadTimeSpan();
        DateTime ReadDateTime();
        String ReadString();
        String ReadString(uint length);
        Guid ReadGuid();
        byte[] ReadBlob();
        byte[] ReadBlob(uint length);
    }
}