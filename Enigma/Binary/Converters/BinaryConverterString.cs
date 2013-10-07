using System;
namespace Enigma.Binary.Converters
{
    public class BinaryConverterString : IBinaryConverter<String>
    {
        public String Convert(byte[] value)
        {
            if (value == null) throw new ArgumentNullException("value");
            return Convert(value, 0, value.Length);
        }

        public String Convert(byte[] value, int startIndex)
        {
            if (value == null) throw new ArgumentNullException("value");
            return Convert(value, startIndex, value.Length - startIndex);
        }

        public String Convert(byte[] value, int startIndex, int length)
        {
            if (value == null) throw new ArgumentNullException("value");
            return System.Text.Encoding.UTF8.GetString(value, startIndex, length);
        }

        public byte[] Convert(String value)
        {
            return System.Text.Encoding.UTF8.GetBytes(value);
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
            return Convert((String)value);
        }

        public void Convert(String value, byte[] buffer)
        {
            Convert(value, buffer, 0);
        }

        public void Convert(String value, byte[] buffer, int offset)
        {
            if (buffer == null) throw new ArgumentNullException("buffer");
            if (value == null || value.Length == 0) return;
            System.Text.Encoding.UTF8.GetBytes(value, 0, value.Length, buffer, offset);
        }

        public void Convert(object value, byte[] buffer)
        {
            Convert((String)value, buffer, 0);
        }

        public void Convert(object value, byte[] buffer, int offset)
        {
            Convert((String)value, buffer, offset);
        }

    }
}
