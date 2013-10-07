using System;
namespace Enigma.Binary.Converters
{
    public class BinaryConverterDecimal : IBinaryConverter<Decimal>
    {

        /// <summary>
        /// Equivalent to internal Decimal.GetBytes method
        /// </summary>
        /// <param name="value"></param>
        /// <param name="buffer"></param>
        private static void GetBytes(decimal value, byte[] buffer, int o)
        {
            var bits = Decimal.GetBits(value);
            var lo = bits[0];
            var mid = bits[1];
            var hi = bits[2];
            var flags = bits[3];

            buffer[o] = (byte)lo;
            buffer[o+1] = (byte)(lo >> 8);
            buffer[o+2] = (byte)(lo >> 16);
            buffer[o+3] = (byte)(lo >> 24);
            buffer[o+4] = (byte)mid;
            buffer[o+5] = (byte)(mid >> 8);
            buffer[o+6] = (byte)(mid >> 16);
            buffer[o+7] = (byte)(mid >> 24);
            buffer[o+8] = (byte)hi;
            buffer[o+9] = (byte)(hi >> 8);
            buffer[o+10] = (byte)(hi >> 16);
            buffer[o+11] = (byte)(hi >> 24);
            buffer[o+12] = (byte)flags;
            buffer[o+13] = (byte)(flags >> 8);
            buffer[o+14] = (byte)(flags >> 16);
            buffer[o+15] = (byte)(flags >> 24);
        }

        /// <summary>
        /// Equivalent to internal Decimal.ToDecimal method, except startIndex parameter
        /// </summary>
        /// <param name="buffer"></param>
        /// <returns></returns>
        private static decimal ToDecimal(byte[] buffer, int s)
        {
            int num = (int)buffer[s] | (int)buffer[s + 1] << 8 | (int)buffer[s + 2] << 16 | (int)buffer[s + 3] << 24;
            int num2 = (int)buffer[s + 4] | (int)buffer[s + 5] << 8 | (int)buffer[s + 6] << 16 | (int)buffer[s + 7] << 24;
            int num3 = (int)buffer[s + 8] | (int)buffer[s + 9] << 8 | (int)buffer[s + 10] << 16 | (int)buffer[s + 11] << 24;
            int num4 = (int)buffer[s + 12] | (int)buffer[s + 13] << 8 | (int)buffer[s + 14] << 16 | (int)buffer[s + 15] << 24;
            return new Decimal(new int[] { num, num2, num3, num4 });
        }

        public Decimal Convert(byte[] value)
        {
            if (value == null) throw new ArgumentNullException("value");
            return Convert(value, 0, value.Length);
        }

        public Decimal Convert(byte[] value, int startIndex)
        {
            if (value == null) throw new ArgumentNullException("value");
            return ToDecimal(value, startIndex);
        }

        public Decimal Convert(byte[] value, int startIndex, int length)
        {
            return Convert(value, startIndex);
        }

        public byte[] Convert(Decimal value)
        {
            var result = new byte[16];
            GetBytes(value, result, 0);
            return result;
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
            return Convert((Decimal)value);
        }

        public void Convert(Decimal value, byte[] buffer)
        {
            Convert(value, buffer, 0);
        }

        public void Convert(Decimal value, byte[] buffer, int offset)
        {
            if (buffer == null) throw new ArgumentNullException("buffer");
            if (buffer.Length < offset + 16)
                throw new BufferOverflowException("The buffer can not contain the value");

            GetBytes(value, buffer, offset);
        }

        public void Convert(object value, byte[] buffer)
        {
            Convert((Decimal)value, buffer, 0);
        }

        public void Convert(object value, byte[] buffer, int offset)
        {
            Convert((Decimal)value, buffer, offset);
        }

    }
}
