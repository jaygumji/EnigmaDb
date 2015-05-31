using System;
using System.IO;
using System.Runtime.CompilerServices;

namespace Enigma.Binary
{
    /// <summary>
    /// Variable length packing algorithm. Value will range from 1-5 bytes
    /// </summary>
    public static class BinaryV32Packer
    {
        /// <summary>
        /// Makes the value take variable length, it takes as much as it needs ranging from 1-5 bytes
        /// </summary>
        /// <param name="stream">The stream where we write the value to</param>
        /// <param name="nullableValue">The value to pack</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void PackU(Stream stream, UInt32? nullableValue)
        {
            byte length;
            if (!nullableValue.HasValue) {
                stream.WriteByte(7);
                return;
            }
            var value = nullableValue.Value;
            if (value <= 0x1FU) length = 0;
            else if (value <= 0x1FFFU) length = 1;
            else if (value <= 0x1FFFFFU) length = 2;
            else if (value <= 0x1FFFFFFFU) length = 3;
            else length = 4;

            var b = (byte)(value << 27 >> 24);
            b = (byte)(b | length);
            stream.WriteByte(b);

            if (length == 0) return;
            b = (byte)(value << 19 >> 24);
            stream.WriteByte(b);

            if (length == 1) return;
            b = (byte)(value << 11 >> 24);
            stream.WriteByte(b);

            if (length == 2) return;
            b = (byte)(value << 3 >> 24);
            stream.WriteByte(b);

            if (length == 3) return;
            b = (byte)(value >> 29);
            stream.WriteByte(b);
        }

        /// <summary>
        /// Unpacks a value packed with the variable pack algorithm
        /// </summary>
        /// <param name="stream">The stream where we read the value from</param>
        /// <returns>The unpacked value</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static uint? UnpackU(Stream stream)
        {
            var b = (UInt32)stream.ReadByte();
            if (b == 7) return null;

            var length = (byte)(b << 29 >> 29);

            var result = b >> 3;

            if (length == 0) return result;
            b = (UInt32)stream.ReadByte();
            var part = b << 5;
            result |= part;

            if (length == 1) return result;
            b = (UInt32)stream.ReadByte();
            part = b << 13;
            result |= part;

            if (length == 2) return result;
            b = (UInt32)stream.ReadByte();
            part = b << 21;
            result |= part;

            if (length == 3) return result;
            b = (UInt32)stream.ReadByte();
            part = b << 29;
            return result | part;
        }

        /// <summary>
        /// Makes the value take variable length, it takes as much as it needs ranging from 1-5 bytes
        /// </summary>
        /// <param name="stream">The stream where we write the value to</param>
        /// <param name="nullableValue">The value to pack</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void PackS(Stream stream, Int32? nullableValue)
        {
            if (!nullableValue.HasValue) {
                stream.WriteByte(7);
                return;
            }

            var value = nullableValue.Value;
            var isNegative = value < 0;
            var bits = isNegative ? ((UInt32)value) ^ UInt32.MaxValue : ((UInt32)value);
            var signedBit = isNegative ? (byte)8U : (byte)0U;

            byte length;
            if (bits <= 0xFU) length = 0;
            else if (bits <= 0xFFFU) length = 1;
            else if (bits <= 0xFFFFFU) length = 2;
            else if (bits <= 0xFFFFFFFU) length = 3;
            else length = 4;

            var b = (byte)(bits << 28 >> 24);
            b = (byte)(b | length | signedBit);
            stream.WriteByte(b);

            if (length == 0) return;
            b = (byte)(bits << 20 >> 24);
            stream.WriteByte(b);

            if (length == 1) return;
            b = (byte)(bits << 12 >> 24);
            stream.WriteByte(b);

            if (length == 2) return;
            b = (byte)(bits << 4 >> 24);
            stream.WriteByte(b);

            if (length == 3) return;
            b = (byte)(bits >> 28);
            stream.WriteByte(b);
        }

        /// <summary>
        /// Unpacks a value packed with the variable pack algorithm
        /// </summary>
        /// <param name="stream">The stream where we read the value from</param>
        /// <returns>The unpacked value</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Int32? UnpackS(Stream stream)
        {
            var b = (UInt32)stream.ReadByte();
            if (b == 7) return null;

            var length = (byte)(b << 29 >> 29);
            var isNegative = (b << 28 >> 31) == 1;

            var result = b >> 4;

            if (length == 0) return isNegative ? (Int32)(result ^ UInt32.MaxValue) : (Int32)result;
            b = (UInt32)stream.ReadByte();
            var part = b << 4;
            result |= part;

            if (length == 1) return isNegative ? (Int32)(result ^ UInt32.MaxValue) : (Int32)result;
            b = (UInt32)stream.ReadByte();
            part = b << 12;
            result |= part;

            if (length == 2) return isNegative ? (Int32)(result ^ UInt32.MaxValue) : (Int32)result;
            b = (UInt32)stream.ReadByte();
            part = b << 20;
            result |= part;

            if (length == 3) return isNegative ? (Int32)(result ^ UInt32.MaxValue) : (Int32)result;
            b = (UInt32)stream.ReadByte();
            part = b << 28;
            result |= part;

            return isNegative ? (Int32)(result ^ UInt32.MaxValue) : (Int32)result;
        }


    }
}