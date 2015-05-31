using System;
using System.IO;
using System.Runtime.CompilerServices;

namespace Enigma.Binary
{
    /// <summary>
    /// Predetermined variable length pack algorithm
    /// </summary>
    public static class BinaryPV64Packer
    {
        /// <summary>
        /// Gets the required length in a stream to pack the entire value
        /// </summary>
        /// <param name="value">The value to calculate length for</param>
        /// <returns>The required length in a stream</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static byte GetULength(UInt64 value)
        {
            if (value <= 0xFF) return 1;
            if (value <= 0xFFFFUL) return 2;
            if (value <= 0xFFFFFFUL) return 3;
            if (value <= 0xFFFFFFFFUL) return 4;
            if (value <= 0xFFFFFFFFFFUL) return 5;
            if (value <= 0xFFFFFFFFFFFFUL) return 6;
            if (value <= 0xFFFFFFFfFFFFFFUL) return 7;
            return 8;
        }

        /// <summary>
        /// Gets the required length in a stream to pack the entire value
        /// </summary>
        /// <param name="value">The value to calculate length for</param>
        /// <returns>The required length in a stream</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static byte GetSLength(Int64 value)
        {
            var bits = value < 0 ? ((UInt64)value) ^ UInt64.MaxValue : (UInt64)value;
            if (bits <= 0x7F) return 1;
            if (bits <= 0x7FFFUL) return 2;
            if (bits <= 0x7FFFFFUL) return 3;
            if (bits <= 0x7FFFFFFFUL) return 4;
            if (bits <= 0x7FFFFFFFFFUL) return 5;
            if (bits <= 0x7FFFFFFFFFFFUL) return 6;
            if (bits <= 0x7FFFFFFfFFFFFFUL) return 7;
            if (bits <= 0x7FFFFFFfFFFFFFFFUL) return 8;
            return 9;
        }

        /// <summary>
        /// Packs the value in the stream
        /// </summary>
        /// <param name="stream">The stream where we write the value to</param>
        /// <param name="value">The value to pack</param>
        /// <param name="length">The length to pack</param>
        public static void PackU(Stream stream, UInt64 value, byte length)
        {
            var b = (byte)(value << 56 >> 56);
            stream.WriteByte(b);

            if (length == 1) return;
            b = (byte)(value << 48 >> 56);
            stream.WriteByte(b);

            if (length == 2) return;
            b = (byte)(value << 40 >> 56);
            stream.WriteByte(b);

            if (length == 3) return;
            b = (byte)(value << 32 >> 56);
            stream.WriteByte(b);

            if (length == 4) return;
            b = (byte)(value << 24 >> 56);
            stream.WriteByte(b);

            if (length == 5) return;
            b = (byte)(value << 16 >> 56);
            stream.WriteByte(b);

            if (length == 6) return;
            b = (byte)(value << 8 >> 56);
            stream.WriteByte(b);

            if (length == 7) return;
            b = (byte)(value >> 56);
            stream.WriteByte(b);
        }

        /// <summary>
        /// Unpacks a value
        /// </summary>
        /// <param name="stream">The stream where we read the value from</param>
        /// <param name="length"></param>
        /// <returns>The unpacked value</returns>
        public static UInt64 UnpackU(Stream stream, byte length)
        {
            var b = (UInt64)stream.ReadByte();
            var result = b;

            if (length == 1) return result;
            b = (UInt64)stream.ReadByte();
            var part = b << 8;
            result |= part;

            if (length == 2) return result;
            b = (UInt64)stream.ReadByte();
            part = b << 16;
            result |= part;

            if (length == 3) return result;
            b = (UInt64)stream.ReadByte();
            part = b << 24;
            result |= part;

            if (length == 4) return result;
            b = (UInt64)stream.ReadByte();
            part = b << 32;
            result |= part;

            if (length == 5) return result;
            b = (UInt64)stream.ReadByte();
            part = b << 40;
            result |= part;

            if (length == 6) return result;
            b = (UInt64)stream.ReadByte();
            part = b << 48;
            result |= part;

            if (length == 7) return result;
            b = (UInt64)stream.ReadByte();
            part = b << 56;
            return result | part;
        }

        /// <summary>
        /// Packs the value in the stream
        /// </summary>
        /// <param name="stream">The stream where we write the value to</param>
        /// <param name="value">The value to pack</param>
        /// <param name="length">The length to pack</param>
        public static void PackS(Stream stream, Int64 value, byte length)
        {
            var isNegative = value < 0;
            var signBit = (byte)(isNegative ? 1 : 0);
            var bits = isNegative ? ((UInt64)value) ^ UInt64.MaxValue : ((UInt64)value);

            var b = (byte)(bits << 57 >> 56);
            b |= signBit;
            stream.WriteByte(b);

            if (length == 1) return;
            b = (byte)(bits << 49 >> 56);
            stream.WriteByte(b);

            if (length == 2) return;
            b = (byte)(bits << 41 >> 56);
            stream.WriteByte(b);

            if (length == 3) return;
            b = (byte)(bits << 33 >> 56);
            stream.WriteByte(b);

            if (length == 4) return;
            b = (byte)(bits << 25 >> 56);
            stream.WriteByte(b);

            if (length == 5) return;
            b = (byte)(bits << 17 >> 56);
            stream.WriteByte(b);

            if (length == 6) return;
            b = (byte)(bits << 9 >> 56);
            stream.WriteByte(b);

            if (length == 7) return;
            b = (byte)(bits << 1 >> 56);
            stream.WriteByte(b);

            if (length == 8) return;
            b = (byte)(bits >> 63);
            stream.WriteByte(b);
        }

        /// <summary>
        /// Unpacks a value
        /// </summary>
        /// <param name="stream">The stream where we read the value from</param>
        /// <param name="length"></param>
        /// <returns>The unpacked value</returns>
        public static Int64 UnpackS(Stream stream, byte length)
        {
            var b = (UInt64)stream.ReadByte();
            var isNegative = (b << 63 >> 63) == 1;

            var result = b >> 1;

            if (length == 1) return isNegative ? (Int64)(result ^ UInt64.MaxValue) : (Int64)result; ;
            b = (UInt64)stream.ReadByte();
            var part = b << 7;
            result |= part;

            if (length == 2) return isNegative ? (Int64)(result ^ UInt64.MaxValue) : (Int64)result; ;
            b = (UInt64)stream.ReadByte();
            part = b << 15;
            result |= part;

            if (length == 3) return isNegative ? (Int64)(result ^ UInt64.MaxValue) : (Int64)result; ;
            b = (UInt64)stream.ReadByte();
            part = b << 23;
            result |= part;

            if (length == 4) return isNegative ? (Int64)(result ^ UInt64.MaxValue) : (Int64)result; ;
            b = (UInt64)stream.ReadByte();
            part = b << 31;
            result |= part;

            if (length == 5) return isNegative ? (Int64)(result ^ UInt64.MaxValue) : (Int64)result; ;
            b = (UInt64)stream.ReadByte();
            part = b << 39;
            result |= part;

            if (length == 6) return isNegative ? (Int64)(result ^ UInt64.MaxValue) : (Int64)result; ;
            b = (UInt64)stream.ReadByte();
            part = b << 47;
            result |= part;

            if (length == 7) return isNegative ? (Int64)(result ^ UInt64.MaxValue) : (Int64)result; ;
            b = (UInt64)stream.ReadByte();
            part = b << 55;
            result |= part;

            if (length == 8) return isNegative ? (Int64)(result ^ UInt64.MaxValue) : (Int64)result; ;
            b = (UInt64)stream.ReadByte();
            part = b << 63;
            result |= part;

            return isNegative ? (Int64)(result ^ UInt64.MaxValue) : (Int64)result;
        }
    
    }
}