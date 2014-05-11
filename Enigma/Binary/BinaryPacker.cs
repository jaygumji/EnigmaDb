using System;
using System.IO;

namespace Enigma.Binary
{
    public static class BinaryPacker
    {

        internal const Byte Null = byte.MinValue;
        internal const Byte VariabelLength = byte.MaxValue;

        // 0011 1111 1111 1111 1111 1111 1111 1111
        private const UInt32 ZMaxValue = 0x3FFFFFFF;

        /// <summary>
        /// The value is packed with a loss of 2 bits
        /// </summary>
        /// <param name="value">Value</param>
        /// <returns>Packed value in binary format</returns>
        public static byte[] PackZ(UInt32 value)
        {
            using (var stream = new MemoryStream(4)) {
                PackZ(stream, value);
                return stream.ToArray();
            }
        }

        /// <summary>
        /// The value is packed with a loss of 2 bits
        /// </summary>
        /// <param name="stream">The stream where we write the value to</param>
        /// <param name="value">Value</param>
        public static void PackZ(Stream stream, UInt32 value)
        {
            if (value > ZMaxValue)
                throw new ArgumentOutOfRangeException("value", value, "Must be between 0 and " + ZMaxValue);

            byte length;
            if (value <= 0x3F) length = 0;
            else if (value <= 0x3FFF) length = 1;
            else if (value <= 0x3FFFFF) length = 2;
            else length = 3;

            var b = (byte)(value << 26 >> 24);
            b = (byte)(b | length);
            stream.WriteByte(b);

            if (length == 0) return;
            b = (byte)(value << 18 >> 24);
            stream.WriteByte(b);

            if (length == 1) return;
            b = (byte)(value << 10 >> 24);
            stream.WriteByte(b);

            if (length == 2) return;
            b = (byte)(value << 2 >> 24);
            stream.WriteByte(b);
        }

        /// <summary>
        /// Unpacks the value which was packed with <see cref="PackZ"/>
        /// </summary>
        /// <param name="buffer">The buffer containing the value</param>
        /// <param name="offset">Offset in the buffer where the value begins</param>
        /// <returns>The unpacked value</returns>
        public static UInt32 UnpackZ(byte[] buffer, int offset)
        {
            using (var stream = new MemoryStream(buffer)) {
                if (offset > 0)
                    stream.Seek(offset, SeekOrigin.Begin);

                return UnpackZ(stream);
            }
        }

        /// <summary>
        /// Unpacks the value which was packed with <see cref="PackZ(uint)"/>
        /// </summary>
        /// <param name="stream">The stream containing the value</param>
        /// <returns>The unpacked value</returns>
        public static UInt32 UnpackZ(Stream stream)
        {
            var b = (UInt32)stream.ReadByte();
            var length = (byte)(b << 30 >> 30);

            var result = b >> 2;

            if (length == 0) return result;
            b = (UInt32)stream.ReadByte();
            var part = b << 6;
            result |= part;

            if (length == 1) return result;
            b = (UInt32)stream.ReadByte();
            part = b << 14;
            result |= part;

            if (length == 2) return result;
            b = (UInt32)stream.ReadByte();
            part = b << 22;
            return result | part;
        }
    }
}
