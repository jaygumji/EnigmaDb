using System;
namespace Enigma.Binary.Converters
{
    public class BinaryConverterSingle : IBinaryConverter<Single>
    {
        public Single Convert(byte[] value)
        {
            if (value == null) throw new ArgumentNullException("value");
            return Convert(value, 0, value.Length);
        }

        public Single Convert(byte[] value, int startIndex)
        {
            if (value == null) throw new ArgumentNullException("value");
            return BitConverter.ToSingle(value, startIndex);
        }

        public Single Convert(byte[] value, int startIndex, int length)
        {
            return Convert(value, startIndex);
        }

        public byte[] Convert(Single value)
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
            return Convert((Single)value);
        }

        public void Convert(Single value, byte[] buffer)
        {
            Convert(value, buffer, 0);
        }

        public void Convert(Single value, byte[] buffer, int offset)
        {
            if (buffer == null) throw new ArgumentNullException("buffer");
            var bytes = Convert(value);
            if (buffer.Length < offset + bytes.Length)
                throw new BufferOverflowException("The buffer can not contain the value");
            Array.Copy(bytes, 0, buffer, offset, bytes.Length);
        }

        public void Convert(object value, byte[] buffer)
        {
            Convert((Single)value, buffer, 0);
        }

        public void Convert(object value, byte[] buffer, int offset)
        {
            Convert((Single)value, buffer, offset);
        }

    }
}
