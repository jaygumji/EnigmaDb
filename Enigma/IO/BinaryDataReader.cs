using System;
using System.IO;
using System.Runtime.CompilerServices;
using Enigma.Binary;

namespace Enigma.IO
{
    public class BinaryDataReader : IDataReader
    {
        private readonly Stream _stream;

        public BinaryDataReader(Stream stream)
        {
            _stream = stream;
        }

        public UInt32 ReadZ()
        {
            return BinaryPacker.UnpackZ(_stream);
        }

        public UInt32 ReadV()
        {
            var b = (UInt32)_stream.ReadByte();
            var length = (byte)(b << 29 >> 29);

            var result = b >> 3;

            if (length == 0) return result;
            b = (UInt32)_stream.ReadByte();
            var part = b << 5;
            result |= part;

            if (length == 1) return result;
            b = (UInt32)_stream.ReadByte();
            part = b << 13;
            result |= part;

            if (length == 2) return result;
            b = (UInt32)_stream.ReadByte();
            part = b << 21;
            result |= part;

            if (length == 3) return result;
            b = (UInt32)_stream.ReadByte();
            part = b << 29;
            return result | part;
        }

        public UInt32? ReadNV()
        {
            var b = (UInt32)_stream.ReadByte();
            if (b == 7) return null;

            var length = (byte)(b << 29 >> 29);

            var result = b >> 3;

            if (length == 0) return result;
            b = (UInt32)_stream.ReadByte();
            var part = b << 5;
            result |= part;

            if (length == 1) return result;
            b = (UInt32)_stream.ReadByte();
            part = b << 13;
            result |= part;

            if (length == 2) return result;
            b = (UInt32)_stream.ReadByte();
            part = b << 21;
            result |= part;

            if (length == 3) return result;
            b = (UInt32)_stream.ReadByte();
            part = b << 29;
            return result | part;
        }

        public void Skip(uint length)
        {
            _stream.Seek(length, SeekOrigin.Current);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private T Read<T>(IBinaryInformation<T> info)
        {
            var data = new byte[info.FixedLength];
            _stream.Read(data, 0, data.Length);
            return info.Converter.Convert(data);
        }

        public byte ReadByte()
        {
            return (Byte) _stream.ReadByte();
        }

        public short ReadInt16()
        {
            return Read(BinaryInformation.Int16);
        }

        public int ReadInt32()
        {
            return Read(BinaryInformation.Int32);
        }

        public long ReadInt64()
        {
            return Read(BinaryInformation.Int64);
        }

        public ushort ReadUInt16()
        {
            return Read(BinaryInformation.UInt16);
        }

        public uint ReadUInt32()
        {
            return Read(BinaryInformation.UInt32);
        }

        public ulong ReadUInt64()
        {
            return Read(BinaryInformation.UInt64);
        }

        public bool ReadBoolean()
        {
            return Read(BinaryInformation.Boolean);
        }

        public float ReadSingle()
        {
            return Read(BinaryInformation.Single);
        }

        public double ReadDouble()
        {
            return Read(BinaryInformation.Double);
        }

        public decimal ReadDecimal()
        {
            return Read(BinaryInformation.Decimal);
        }

        public TimeSpan ReadTimeSpan()
        {
            return Read(BinaryInformation.TimeSpan);
        }

        public DateTime ReadDateTime()
        {
            return Read(BinaryInformation.DateTime);
        }

        public string ReadString()
        {
            var length = Read(BinaryInformation.UInt32);
            var data = new byte[length];
            _stream.Read(data, 0, data.Length);
            return BinaryInformation.String.Converter.Convert(data);
        }

        public Guid ReadGuid()
        {
            return Read(BinaryInformation.Guid);
        }

        public byte[] ReadBlob()
        {
            var length = Read(BinaryInformation.UInt32);
            var data = new byte[length];
            _stream.Read(data, 0, data.Length);
            return data;
        }

    }
}