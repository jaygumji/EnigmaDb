using System;
using System.IO;
using System.Runtime.CompilerServices;
using Enigma.Binary;
using Enigma.Serialization;

namespace Enigma.IO
{
    public class BinaryDataWriter : IDataWriter
    {
        private readonly Stream _stream;
        private const UInt32 ZMaxValue = 0x3FFFFFFF;

        public BinaryDataWriter(Stream stream)
        {
            _stream = stream;
        }

        public WriteReservation Reserve()
        {
            var position = _stream.Position;
            Write((UInt32) 0);
            return new WriteReservation(position);
        }

        public void Write(WriteReservation reservation)
        {
            var value = (UInt32) (_stream.Position - reservation.Position);
            Write(reservation, value);
        }

        public void Write(WriteReservation reservation, UInt32 value)
        {
            var currentPosition = _stream.Position;
            _stream.Seek(reservation.Position, SeekOrigin.Begin);
            Write(value);
            _stream.Seek(currentPosition, SeekOrigin.Begin);
        }

        /// <summary>
        /// More tightly packed than the V version but the max size is 1/4 of an <see cref="UInt32"/>
        /// </summary>
        /// <param name="value">The value to pack</param>
        public void WriteZ(UInt32 value)
        {
            BinaryPacker.PackZ(_stream, value);
        }

        /// <summary>
        /// Makes the value take variable length, it takes as much as it needs ranging from 1-5 bytes
        /// </summary>
        /// <param name="value">The value to pack</param>
        public void WriteV(UInt32 value)
        {
            byte length;
            if (value <= 0x1F) length = 0;
            else if (value <= 0x1FFF) length = 1;
            else if (value <= 0x1FFFFF) length = 2;
            else if (value <= 0x1FFFFFFF) length = 3;
            else length = 4;

            var b = (byte)(value << 27 >> 24);
            b = (byte)(b | length);
            _stream.WriteByte(b);

            if (length == 0) return;
            b = (byte)(value << 19 >> 24);
            _stream.WriteByte(b);

            if (length == 1) return;
            b = (byte)(value << 11 >> 24);
            _stream.WriteByte(b);

            if (length == 2) return;
            b = (byte)(value << 3 >> 24);
            _stream.WriteByte(b);

            if (length == 3) return;
            b = (byte)(value >> 29);
            _stream.WriteByte(b);
        }

        /// <summary>
        /// Makes the value take variable length, it takes as much as it needs ranging from 1-5 bytes
        /// </summary>
        /// <param name="nullableValue">The value to pack</param>
        public void WriteNV(UInt32? nullableValue)
        {
            byte length;
            if (!nullableValue.HasValue) {
                _stream.WriteByte(7);
                return;
            }
            var value = nullableValue.Value;
            if (value <= 0x1F) length = 0;
            else if (value <= 0x1FFF) length = 1;
            else if (value <= 0x1FFFFF) length = 2;
            else if (value <= 0x1FFFFFFF) length = 3;
            else length = 4;

            var b = (byte)(value << 27 >> 24);
            b = (byte)(b | length);
            _stream.WriteByte(b);

            if (length == 0) return;
            b = (byte)(value << 19 >> 24);
            _stream.WriteByte(b);

            if (length == 1) return;
            b = (byte)(value << 11 >> 24);
            _stream.WriteByte(b);

            if (length == 2) return;
            b = (byte)(value << 3 >> 24);
            _stream.WriteByte(b);

            if (length == 3) return;
            b = (byte)(value >> 29);
            _stream.WriteByte(b);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void Write<T>(IBinaryInformation<T> info, T value)
        {
            var bytes = info.Converter.Convert(value);
            _stream.Write(bytes, 0, bytes.Length);
        }

        public void Write(byte value)
        {
            _stream.WriteByte(value);
        }

        public void Write(short value)
        {
            Write(BinaryInformation.Int16, value);
        }

        public void Write(int value)
        {
            Write(BinaryInformation.Int32, value);
        }

        public void Write(long value)
        {
            Write(BinaryInformation.Int64, value);
        }

        public void Write(ushort value)
        {
            Write(BinaryInformation.UInt16, value);
        }

        public void Write(uint value)
        {
            Write(BinaryInformation.UInt32, value);
        }

        public void Write(ulong value)
        {
            Write(BinaryInformation.UInt64, value);
        }

        public void Write(bool value)
        {
            Write(BinaryInformation.Boolean, value);
        }

        public void Write(float value)
        {
            Write(BinaryInformation.Single, value);
        }

        public void Write(double value)
        {
            Write(BinaryInformation.Double, value);
        }

        public void Write(decimal value)
        {
            Write(BinaryInformation.Decimal, value);
        }

        public void Write(TimeSpan value)
        {
            Write(BinaryInformation.TimeSpan, value);
        }

        public void Write(DateTime value)
        {
            Write(BinaryInformation.DateTime, value);
        }

        public void Write(string value)
        {
            if (value == null)
                throw new ArgumentNullException("value");

            Write((UInt32)value.Length);
            Write(BinaryInformation.String, value);
        }

        public void Write(Guid value)
        {
            Write(BinaryInformation.Guid, value);
        }
        
        public void Write(byte[] value)
        {
            if (value == null)
                throw new ArgumentNullException("value");

            Write((UInt32)value.Length);
            _stream.Write(value, 0, value.Length);
        }

    }
}