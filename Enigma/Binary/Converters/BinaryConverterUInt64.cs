using System;
namespace Enigma.Binary.Converters
{
    public class BinaryConverterUInt64 : IBinaryConverter<UInt64>
    {
        public UInt64 Convert(byte[] value)
        {
            if (value == null) throw new ArgumentNullException("value");
            return Convert(value, 0, value.Length);
        }

        public UInt64 Convert(byte[] value, int startIndex)
        {
            if (value == null) throw new ArgumentNullException("value");
            return BitConverter.ToUInt64(value, startIndex);
        }

        public UInt64 Convert(byte[] value, int startIndex, int length)
        {
            return Convert(value, startIndex);
        }

        public byte[] Convert(UInt64 value)
        {
            return BitConverter.GetBytes(value);
        }

        object IBinaryConverter.Convert(byte[] value)
        {
            if (value == null) throw new ArgumentNullException("value");
            return Convert(value, 0, value.Length);
        }

        object IBinaryConverter.Convert(byte[] value, int startIndex)
        {
            if (value == null) throw new ArgumentNullException("value");
            return Convert(value, startIndex, value.Length - startIndex);
        }

        object IBinaryConverter.Convert(byte[] value, int startIndex, int length)
        {
            return Convert(value, startIndex, length);
        }

        byte[] IBinaryConverter.Convert(object value)
        {
            return Convert((UInt64)value);
        }

        public void Convert(UInt64 value, byte[] buffer)
        {
            Convert(value, buffer, 0);
        }

        public void Convert(UInt64 value, byte[] buffer, int offset)
        {
            if (buffer == null) throw new ArgumentNullException("buffer");
            var bytes = Convert(value);
            if (buffer.Length < offset + bytes.Length)
                throw new BufferOverflowException("The buffer can not contain the value");
            Array.Copy(bytes, 0, buffer, offset, bytes.Length);
        }

        public void Convert(object value, byte[] buffer)
        {
            Convert((UInt64)value, buffer, 0);
        }

        public void Convert(object value, byte[] buffer, int offset)
        {
            Convert((UInt64)value, buffer, offset);
        }

    }
}
