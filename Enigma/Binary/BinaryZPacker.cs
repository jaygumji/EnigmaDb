using System;
using System.IO;
using System.Runtime.CompilerServices;

namespace Enigma.Binary
{

    public static class BinarySVNPacker
    {
        public static void Pack(Stream stream, Int32? value)
        {

        }
    }

    public static class BinarySVPacker
    {
        public static void Pack(Stream stream, Int32 value)
        {
            
        }
    }

    public static class BinaryZPacker
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
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static byte[] Pack(UInt32 value)
        {
            using (var stream = new MemoryStream(4)) {
                Pack(stream, value);
                return stream.ToArray();
            }
        }

        /// <summary>
        /// The value is packed with a loss of 2 bits
        /// </summary>
        /// <param name="stream">The stream where we write the value to</param>
        /// <param name="value">Value</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Pack(Stream stream, UInt32 value)
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
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static UInt32 Unpack(byte[] buffer, int offset)
        {
            using (var stream = new MemoryStream(buffer)) {
                if (offset > 0)
                    stream.Seek(offset, SeekOrigin.Begin);

                return Unpack(stream);
            }
        }

        /// <summary>
        /// Unpacks the value which was packed with <see cref="Pack"/>
        /// </summary>
        /// <param name="stream">The stream containing the value</param>
        /// <returns>The unpacked value</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static UInt32 Unpack(Stream stream)
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
