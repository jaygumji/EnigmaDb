using System;
namespace Enigma.Binary.Converters
{
    public class BinaryConverterByte : IBinaryConverter<Byte>
    {
        public Byte Convert(byte[] value)
        {
            if (value == null) throw new ArgumentNullException("value");
            return Convert(value, 0, value.Length);
        }

        public Byte Convert(byte[] value, int startIndex)
        {
            if (value == null) throw new ArgumentNullException("value");
            return value[startIndex];
        }

        public Byte Convert(byte[] value, int startIndex, int length)
        {
            return Convert(value, startIndex);
        }

        public byte[] Convert(Byte value)
        {
            return new byte[] { value };
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
            return Convert((byte)value);
        }

        public void Convert(Byte value, byte[] buffer)
        {
            Convert(value, buffer, 0);
        }

        public void Convert(Byte value, byte[] buffer, int offset)
        {
            if (buffer == null) throw new ArgumentNullException("buffer");
            if (buffer.Length < offset + 1)
                throw new BufferOverflowException("The buffer can not contain the value");
            buffer[offset] = value;
        }

        public void Convert(object value, byte[] buffer)
        {
            Convert((Byte)value, buffer, 0);
        }

        public void Convert(object value, byte[] buffer, int offset)
        {
            Convert((Byte)value, buffer, offset);
        }

    }
}
