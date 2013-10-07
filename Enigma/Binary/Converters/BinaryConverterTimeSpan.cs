using System;
namespace Enigma.Binary.Converters
{
    public class BinaryConverterTimeSpan : IBinaryConverter<TimeSpan>
    {
        public TimeSpan Convert(byte[] value)
        {
            if (value == null) throw new ArgumentNullException("value");
            return Convert(value, 0, value.Length);
        }

        public TimeSpan Convert(byte[] value, int startIndex)
        {
            if (value == null) throw new ArgumentNullException("value");
            return new TimeSpan(BitConverter.ToInt64(value, startIndex));
        }

        public TimeSpan Convert(byte[] value, int startIndex, int length)
        {
            return Convert(value, startIndex);
        }

        public byte[] Convert(TimeSpan value)
        {
            return BitConverter.GetBytes(value.Ticks);
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
            return Convert((TimeSpan)value);
        }

        public void Convert(TimeSpan value, byte[] buffer)
        {
            Convert(value, buffer, 0);
        }

        public void Convert(TimeSpan value, byte[] buffer, int offset)
        {
            if (buffer == null) throw new ArgumentNullException("buffer");
            var bytes = Convert(value);
            if (buffer.Length < offset + bytes.Length)
                throw new BufferOverflowException("The buffer can not contain the value");
            Array.Copy(bytes, 0, buffer, offset, bytes.Length);
        }

        public void Convert(object value, byte[] buffer)
        {
            Convert((TimeSpan)value, buffer, 0);
        }

        public void Convert(object value, byte[] buffer, int offset)
        {
            Convert((TimeSpan)value, buffer, offset);
        }

    }
}
