using System;
using System.Collections.Generic;
using System.IO;
using Enigma.Binary;
using Enigma.IO;

namespace Enigma.Serialization.PackedBinary
{
    public sealed class PackedDataWriteVisitor : IWriteVisitor
    {
        private readonly Stream _stream;
        private readonly BinaryDataWriter _writer;
        private readonly Stack<WriteReservation> _reservations;

        public PackedDataWriteVisitor(Stream stream)
        {
            _stream = stream;
            _writer = new BinaryDataWriter(stream);
            _reservations = new Stack<WriteReservation>();
        }

        public void Visit(object level, VisitArgs args)
        {
            if (args.Type == LevelType.Root) return;

            if (args.Metadata.Index > 0)
                BinaryZPacker.Pack(_stream, args.Metadata.Index);

            if (level == null) {
                _stream.WriteByte(BinaryZPacker.Null);
                return;
            }

            _stream.WriteByte(BinaryZPacker.VariabelLength);
            _reservations.Push(_writer.Reserve());
        }

        public void Leave(object level, VisitArgs args)
        {
            if (args.Type == LevelType.Root) return;

            if (level != null) {
                var reservation = _reservations.Pop();
                _writer.Write(reservation);
                BinaryZPacker.Pack(_stream, 0);
            }
        }

        public void VisitValue(byte? value, VisitArgs args)
        {
            if (args.Metadata.Index > 0)
                BinaryZPacker.Pack(_stream, args.Metadata.Index);

            if (value == null) {
                _stream.WriteByte(BinaryZPacker.Null);
                return;
            }

            _stream.WriteByte((Byte)BinaryInformation.Byte.FixedLength);
            _stream.WriteByte(value.Value);
        }

        public void VisitValue(short? value, VisitArgs args)
        {
            if (args.Metadata.Index > 0)
                BinaryZPacker.Pack(_stream, args.Metadata.Index);

            if (value == null) {
                _stream.WriteByte(BinaryZPacker.Null);
                return;
            }

            var length = BinaryPV64Packer.GetSLength(value.Value);
            _stream.WriteByte(length);
            BinaryPV64Packer.PackS(_stream, value.Value, length);
        }

        public void VisitValue(int? value, VisitArgs args)
        {
            if (args.Metadata.Index > 0)
                BinaryZPacker.Pack(_stream, args.Metadata.Index);

            if (value == null) {
                _stream.WriteByte(BinaryZPacker.Null);
                return;
            }

            var length = BinaryPV64Packer.GetSLength(value.Value);
            _stream.WriteByte(length);
            BinaryPV64Packer.PackS(_stream, value.Value, length);
        }

        public void VisitValue(long? value, VisitArgs args)
        {
            if (args.Metadata.Index > 0)
                BinaryZPacker.Pack(_stream, args.Metadata.Index);

            if (value == null) {
                _stream.WriteByte(BinaryZPacker.Null);
                return;
            }

            var length = BinaryPV64Packer.GetSLength(value.Value);
            _stream.WriteByte(length);
            BinaryPV64Packer.PackS(_stream, value.Value, length);
        }

        public void VisitValue(ushort? value, VisitArgs args)
        {
            if (args.Metadata.Index > 0)
                BinaryZPacker.Pack(_stream, args.Metadata.Index);

            if (value == null) {
                _stream.WriteByte(BinaryZPacker.Null);
                return;
            }

            var length = BinaryPV64Packer.GetULength(value.Value);
            _stream.WriteByte(length);
            BinaryPV64Packer.PackU(_stream, value.Value, length);
        }

        public void VisitValue(uint? value, VisitArgs args)
        {
            if (args.Metadata.Index > 0)
                BinaryZPacker.Pack(_stream, args.Metadata.Index);

            if (value == null) {
                _stream.WriteByte(BinaryZPacker.Null);
                return;
            }

            var length = BinaryPV64Packer.GetULength(value.Value);
            _stream.WriteByte(length);
            BinaryPV64Packer.PackU(_stream, value.Value, length);
        }

        public void VisitValue(ulong? value, VisitArgs args)
        {
            if (args.Metadata.Index > 0)
                BinaryZPacker.Pack(_stream, args.Metadata.Index);

            if (value == null) {
                _stream.WriteByte(BinaryZPacker.Null);
                return;
            }

            var length = BinaryPV64Packer.GetULength(value.Value);
            _stream.WriteByte(length);
            BinaryPV64Packer.PackU(_stream, value.Value, length);
        }

        public void VisitValue(bool? value, VisitArgs args)
        {
            if (args.Metadata.Index > 0)
                BinaryZPacker.Pack(_stream, args.Metadata.Index);

            if (value == null) {
                _stream.WriteByte(BinaryZPacker.Null);
                return;
            }

            _stream.WriteByte((Byte)BinaryInformation.Boolean.FixedLength);
            var bytes = BinaryInformation.Boolean.Converter.Convert(value.Value);
            _stream.Write(bytes, 0, bytes.Length);
        }

        public void VisitValue(float? value, VisitArgs args)
        {
            if (args.Metadata.Index > 0)
                BinaryZPacker.Pack(_stream, args.Metadata.Index);

            if (value == null) {
                _stream.WriteByte(BinaryZPacker.Null);
                return;
            }

            _stream.WriteByte((Byte)BinaryInformation.Single.FixedLength);
            var bytes = BinaryInformation.Single.Converter.Convert(value.Value);
            _stream.Write(bytes, 0, bytes.Length);
        }

        public void VisitValue(double? value, VisitArgs args)
        {
            if (args.Metadata.Index > 0)
                BinaryZPacker.Pack(_stream, args.Metadata.Index);

            if (value == null) {
                _stream.WriteByte(BinaryZPacker.Null);
                return;
            }

            _stream.WriteByte((Byte)BinaryInformation.Double.FixedLength);
            var bytes = BinaryInformation.Double.Converter.Convert(value.Value);
            _stream.Write(bytes, 0, bytes.Length);
        }

        public void VisitValue(decimal? value, VisitArgs args)
        {
            if (args.Metadata.Index > 0)
                BinaryZPacker.Pack(_stream, args.Metadata.Index);

            if (value == null) {
                _stream.WriteByte(BinaryZPacker.Null);
                return;
            }

            _stream.WriteByte((Byte)BinaryInformation.Decimal.FixedLength);
            var bytes = BinaryInformation.Decimal.Converter.Convert(value.Value);
            _stream.Write(bytes, 0, bytes.Length);
        }

        public void VisitValue(TimeSpan? value, VisitArgs args)
        {
            if (args.Metadata.Index > 0)
                BinaryZPacker.Pack(_stream, args.Metadata.Index);

            if (value == null) {
                _stream.WriteByte(BinaryZPacker.Null);
                return;
            }

            var length = BinaryPV64Packer.GetSLength(value.Value.Ticks);
            _stream.WriteByte(length);
            BinaryPV64Packer.PackS(_stream, value.Value.Ticks, length);
        }

        public void VisitValue(DateTime? value, VisitArgs args)
        {
            if (args.Metadata.Index > 0)
                BinaryZPacker.Pack(_stream, args.Metadata.Index);

            if (value == null) {
                _stream.WriteByte(BinaryZPacker.Null);
                return;
            }

            if (value.Value.Kind != DateTimeKind.Utc)
                value = value.Value.ToUniversalTime();

            var length = BinaryPV64Packer.GetSLength(value.Value.Ticks);
            _stream.WriteByte(length);
            BinaryPV64Packer.PackS(_stream, value.Value.Ticks, length);
        }

        public void VisitValue(string value, VisitArgs args)
        {
            if (args.Metadata.Index > 0)
                BinaryZPacker.Pack(_stream, args.Metadata.Index);

            if (value == null) {
                _stream.WriteByte(BinaryZPacker.Null);
                return;
            }

            if (value.Length < BinaryZPacker.VariabelLength)
                _stream.WriteByte((Byte)value.Length);
            else {
                _stream.WriteByte(BinaryZPacker.VariabelLength);
                BinaryV32Packer.PackU(_stream, (uint)value.Length);
            }
            var bytes = BinaryInformation.String.Converter.Convert(value);
            _stream.Write(bytes, 0, bytes.Length);
        }

        public void VisitValue(Guid? value, VisitArgs args)
        {
            if (args.Metadata.Index > 0)
                BinaryZPacker.Pack(_stream, args.Metadata.Index);

            if (value == null) {
                _stream.WriteByte(BinaryZPacker.Null);
                return;
            }

            _stream.WriteByte((Byte)BinaryInformation.Guid.FixedLength);
            var bytes = BinaryInformation.Guid.Converter.Convert(value.Value);
            _stream.Write(bytes, 0, bytes.Length);
        }

        public void VisitValue(byte[] value, VisitArgs args)
        {
            if (args.Metadata.Index > 0)
                BinaryZPacker.Pack(_stream, args.Metadata.Index);

            if (value == null) {
                _stream.WriteByte(BinaryZPacker.Null);
                return;
            }

            if (value.Length < BinaryZPacker.VariabelLength)
                _stream.WriteByte((Byte) value.Length);
            else {
                _stream.WriteByte(BinaryZPacker.VariabelLength);
                BinaryV32Packer.PackU(_stream, (uint)value.Length);
            }
            _stream.Write(value, 0, value.Length);
        }

    }
}