using System;
namespace Enigma.Binary.Converters
{
    public class BinaryConverterSByte : IBinaryConverter<SByte>
    {
        public SByte Convert(byte[] value)
        {
            if (value == null) throw new ArgumentNullException("value");
            return Convert(value, 0, value.Length);
        }

        public SByte Convert(byte[] value, int startIndex)
        {
            if (value == null) throw new ArgumentNullException("value");
            return (SByte) value[startIndex];
        }

        public SByte Convert(byte[] value, int startIndex, int length)
        {
            return Convert(value, startIndex);
        }

        public byte[] Convert(SByte value)
        {
            return new byte[] { (byte)value };
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
            return Convert((SByte)value);
        }

        public void Convert(SByte value, byte[] buffer)
        {
            Convert(value, buffer, 0);
        }

        public void Convert(SByte value, byte[] buffer, int offset)
        {
            if (buffer == null) throw new ArgumentNullException("buffer");
            var bytes = Convert(value);
            if (buffer.Length < offset + bytes.Length)
                throw new BufferOverflowException("The buffer can not contain the value");
            Array.Copy(bytes, 0, buffer, offset, bytes.Length);
        }

        public void Convert(object value, byte[] buffer)
        {
            Convert((SByte)value, buffer, 0);
        }

        public void Convert(object value, byte[] buffer, int offset)
        {
            Convert((SByte)value, buffer, offset);
        }

    }
}
