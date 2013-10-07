using System;
namespace Enigma.Binary.Converters
{
    public class BinaryConverterInt64 : IBinaryConverter<Int64>
    {
        public Int64 Convert(byte[] value)
        {
            if (value == null) throw new ArgumentNullException("value");
            return Convert(value, 0, value.Length);
        }

        public Int64 Convert(byte[] value, int startIndex)
        {
            if (value == null) throw new ArgumentNullException("value");
            return BitConverter.ToInt64(value, startIndex);
        }

        public Int64 Convert(byte[] value, int startIndex, int length)
        {
            return Convert(value, startIndex);
        }

        public byte[] Convert(Int64 value)
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
            return Convert((Int64)value);
        }

        public void Convert(Int64 value, byte[] buffer)
        {
            Convert(value, buffer, 0);
        }

        public void Convert(Int64 value, byte[] buffer, int offset)
        {
            if (buffer == null) throw new ArgumentNullException("buffer");
            var bytes = Convert(value);
            if (buffer.Length < offset + bytes.Length)
                throw new BufferOverflowException("The buffer can not contain the value");
            Array.Copy(bytes, 0, buffer, offset, bytes.Length);
        }

        public void Convert(object value, byte[] buffer)
        {
            Convert((Int64)value, buffer, 0);
        }

        public void Convert(object value, byte[] buffer, int offset)
        {
            Convert((Int64)value, buffer, offset);
        }

    }
}
