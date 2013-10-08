﻿using System;
namespace Enigma.Binary.Converters
{
    public class BinaryConverterInt16 : IBinaryConverter<Int16>
    {
        public Int16 Convert(byte[] value)
        {
            if (value == null) throw new ArgumentNullException("value");
            return Convert(value, 0, value.Length);
        }

        public Int16 Convert(byte[] value, int startIndex)
        {
            if (value == null) throw new ArgumentNullException("value");
            return BitConverter.ToInt16(value, startIndex);
        }

        public Int16 Convert(byte[] value, int startIndex, int length)
        {
            return Convert(value, startIndex);
        }

        public byte[] Convert(Int16 value)
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
            return Convert((Int16)value);
        }

        public void Convert(Int16 value, byte[] buffer)
        {
            Convert(value, buffer, 0);
        }

        public void Convert(Int16 value, byte[] buffer, int offset)
        {
            if (buffer == null) throw new ArgumentNullException("buffer");
            var bytes = Convert(value);
            if (buffer.Length < offset + bytes.Length)
                throw new BufferOverflowException("The buffer can not contain the value");
            Array.Copy(bytes, 0, buffer, offset, bytes.Length);
        }

        public void Convert(object value, byte[] buffer)
        {
            Convert((Int16) value, buffer, 0);
        }

        public void Convert(object value, byte[] buffer, int offset)
        {
            Convert((Int16) value, buffer, offset);
        }
    }
}