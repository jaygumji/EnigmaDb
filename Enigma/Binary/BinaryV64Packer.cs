using System;
using System.IO;
using System.Runtime.CompilerServices;

namespace Enigma.Binary
{
    /// <summary>
    /// Variable length packing algorithm. Value will range from 1-5 bytes
    /// </summary>
    public static class BinaryV64Packer
    {

        /// <summary>
        /// Makes the value take variable length
        /// </summary>
        /// <param name="stream">The stream where we write the value to</param>
        /// <param name="nullableValue">The value to pack</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void PackU(Stream stream, UInt64? nullableValue)
        {
            byte length;
            if (!nullableValue.HasValue) {
                stream.WriteByte(15);
                return;
            }
            var value = nullableValue.Value;
            if (value <= 0xF) length = 0;
            else if (value <= 0xFFFUL) length = 1;
            else if (value <= 0xFFFFFUL) length = 2;
            else if (value <= 0xFFFFFFFUL) length = 3;
            else if (value <= 0xFFFFFFFFFUL) length = 4;
            else if (value <= 0xFFFFFFFFFFFUL) length = 5;
            else if (value <= 0xFFFFFFFFFFFFFUL) length = 6;
            else if (value <= 0xFFFFFFFFFFFFFFFUL) length = 7;
            else length = 8;

            var b = (byte)(value << 60 >> 56);
            b = (byte)(b | length);
            stream.WriteByte(b);

            if (length == 0) return;
            b = (byte)(value << 52 >> 56);
            stream.WriteByte(b);

            if (length == 1) return;
            b = (byte)(value << 44 >> 56);
            stream.WriteByte(b);

            if (length == 2) return;
            b = (byte)(value << 36 >> 56);
            stream.WriteByte(b);

            if (length == 3) return;
            b = (byte)(value << 28 >> 56);
            stream.WriteByte(b);

            if (length == 4) return;
            b = (byte)(value << 20 >> 56);
            stream.WriteByte(b);

            if (length == 5) return;
            b = (byte)(value << 12 >> 56);
            stream.WriteByte(b);

            if (length == 6) return;
            b = (byte)(value << 4 >> 56);
            stream.WriteByte(b);

            if (length == 7) return;
            b = (byte)(value >> 60);
            stream.WriteByte(b);
        }

        /// <summary>
        /// Unpacks a value packed with the variable pack algorithm
        /// </summary>
        /// <param name="stream">The stream where we read the value from</param>
        /// <returns>The unpacked value</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static UInt64? UnpackU(Stream stream)
        {
            var b = (UInt64)stream.ReadByte();
            if (b == 15) return null;

            var length = (byte)(b << 60 >> 60);

            var result = b >> 4;

            if (length == 0) return result;
            b = (UInt64)stream.ReadByte();
            var part = b << 4;
            result |= part;

            if (length == 1) return result;
            b = (UInt64)stream.ReadByte();
            part = b << 12;
            result |= part;

            if (length == 2) return result;
            b = (UInt64)stream.ReadByte();
            part = b << 20;
            result |= part;

            if (length == 3) return result;
            b = (UInt64)stream.ReadByte();
            part = b << 28;
            result |= part;

            if (length == 4) return result;
            b = (UInt64)stream.ReadByte();
            part = b << 36;
            result |= part;

            if (length == 5) return result;
            b = (UInt64)stream.ReadByte();
            part = b << 44;
            result |= part;

            if (length == 6) return result;
            b = (UInt64)stream.ReadByte();
            part = b << 52;
            result |= part;

            if (length == 7) return result;
            b = (UInt64)stream.ReadByte();
            part = b << 60;
            return result | part;
        }

        /// <summary>
        /// Makes the value take variable length, it takes as much as it needs ranging from 1-5 bytes
        /// </summary>
        /// <param name="stream">The stream where we write the value to</param>
        /// <param name="nullableValue">The value to pack</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void PackS(Stream stream, Int64? nullableValue)
        {
            if (!nullableValue.HasValue) {
                stream.WriteByte(15);
                return;
            }

            var value = nullableValue.Value;
            var isNegative = value < 0;
            var bits = isNegative ? ((UInt64)value) ^ UInt64.MaxValue : ((UInt64)value);
            var signedBit = isNegative ? (byte)16 : (byte)0;

            byte length;
            if (bits <= 0x7U) length = 0;
            else if (bits <= 0x7FFU) length = 1;
            else if (bits <= 0x7FFFFU) length = 2;
            else if (bits <= 0x7FFFFFFU) length = 3;
            else if (bits <= 0x7FFFFFFFFU) length = 4;
            else if (bits <= 0x7FFFFFFFFFFU) length = 5;
            else if (bits <= 0x7FFFFFFFFFFFFU) length = 6;
            else if (bits <= 0x7FFFFFFFFFFFFFFU) length = 7;
            else length = 8;

            var b = (byte)(bits << 61 >> 56);
            b = (byte)(b | length | signedBit);
            stream.WriteByte(b);

            if (length == 0) return;
            b = (byte)(bits << 53 >> 56);
            stream.WriteByte(b);

            if (length == 1) return;
            b = (byte)(bits << 45 >> 56);
            stream.WriteByte(b);

            if (length == 2) return;
            b = (byte)(bits << 37 >> 56);
            stream.WriteByte(b);

            if (length == 3) return;
            b = (byte)(bits << 29 >> 56);
            stream.WriteByte(b);

            if (length == 4) return;
            b = (byte)(bits << 21 >> 56);
            stream.WriteByte(b);

            if (length == 5) return;
            b = (byte)(bits << 13 >> 56);
            stream.WriteByte(b);

            if (length == 6) return;
            b = (byte)(bits << 5 >> 56);
            stream.WriteByte(b);

            if (length == 7) return;
            b = (byte)(bits >> 59);
            stream.WriteByte(b);
        }

        /// <summary>
        /// Unpacks a value packed with the variable pack algorithm
        /// </summary>
        /// <param name="stream">The stream where we read the value from</param>
        /// <returns>The unpacked value</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Int64? UnpackS(Stream stream)
        {
            var b = (UInt64)stream.ReadByte();
            if (b == 15) return null;

            var length = (byte)(b << 60 >> 60);
            var isNegative = (b << 59 >> 63) == 1;

            var result = b >> 5;

            if (length == 0) return isNegative ? (Int64)(result ^ UInt64.MaxValue) : (Int64)result;
            b = (UInt64)stream.ReadByte();
            var part = b << 3;
            result |= part;

            if (length == 1) return isNegative ? (Int64)(result ^ UInt64.MaxValue) : (Int64)result;
            b = (UInt64)stream.ReadByte();
            part = b << 11;
            result |= part;

            if (length == 2) return isNegative ? (Int64)(result ^ UInt64.MaxValue) : (Int64)result;
            b = (UInt64)stream.ReadByte();
            part = b << 19;
            result |= part;

            if (length == 3) return isNegative ? (Int64)(result ^ UInt64.MaxValue) : (Int64)result;
            b = (UInt64)stream.ReadByte();
            part = b << 27;
            result |= part;

            if (length == 4) return isNegative ? (Int64)(result ^ UInt64.MaxValue) : (Int64)result;
            b = (UInt64)stream.ReadByte();
            part = b << 35;
            result |= part;

            if (length == 5) return isNegative ? (Int64)(result ^ UInt64.MaxValue) : (Int64)result;
            b = (UInt64)stream.ReadByte();
            part = b << 43;
            result |= part;

            if (length == 6) return isNegative ? (Int64)(result ^ UInt64.MaxValue) : (Int64)result;
            b = (UInt64)stream.ReadByte();
            part = b << 51;
            result |= part;

            if (length == 7) return isNegative ? (Int64)(result ^ UInt64.MaxValue) : (Int64)result;
            b = (UInt64)stream.ReadByte();
            part = b << 59;
            result |= part;

            return isNegative ? (Int64)(result ^ UInt64.MaxValue) : (Int64)result;
        }

    }
}