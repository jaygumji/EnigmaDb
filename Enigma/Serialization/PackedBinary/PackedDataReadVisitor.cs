using System;
using Enigma.Binary;
using Enigma.IO;

namespace Enigma.Serialization.PackedBinary
{
    public class PackedDataReadVisitor : IReadVisitor
    {
        private readonly BinaryDataReader _reader;
        private UInt32 _nextIndex;
        private bool _endOfLevel;

        public PackedDataReadVisitor(BinaryDataReader reader)
        {
            _reader = reader;
        }

        private static bool SkipDataIndex(uint dataIndex, uint index)
        {
            return 0 < dataIndex && dataIndex < index;
        }

        private bool MoveToIndex(UInt32 index)
        {
            if (_endOfLevel) return false;
            if (_nextIndex > index) return false;
            if (_nextIndex > 0) {
                var nextIndex = _nextIndex;
                _nextIndex = 0;
                if (nextIndex == index) return true;
            }
            UInt32 dataIndex;
            while (SkipDataIndex(dataIndex = _reader.ReadZ(), index)) {
                var byteLength = _reader.ReadByte();
                if (byteLength == BinaryPacker.Null) continue;

                if (byteLength != BinaryPacker.VariabelLength)
                    _reader.Skip(byteLength);
                else {
                    var length = _reader.ReadUInt32();
                    _reader.Skip(length);
                }
            }
            if (dataIndex == 0) {
                _endOfLevel = true;
                return false;
            }
            if (dataIndex > index) {
                _nextIndex = dataIndex;
                return false;
            }
            return true;
        }

        public ValueState TryVisit(VisitArgs args)
        {
            if (args.Type == LevelType.Root) return ValueState.Found;

            if (args.Metadata.Index > 0 && !MoveToIndex(args.Metadata.Index))
                return ValueState.NotFound;

            var byteLength = _reader.ReadByte();
            if (byteLength == BinaryPacker.Null) return ValueState.Null;
            if (byteLength != BinaryPacker.VariabelLength)
                throw new UnexpectedLengthException(args, byteLength);

            _reader.Skip(4);

            return ValueState.Found;
        }

        public void Leave(VisitArgs args)
        {
            if (_endOfLevel) {
                _endOfLevel = false;
                return;
            }

            if (args.Type == LevelType.Root) return;
            if (args.Type.IsCollection()) return;
            if (args.Type.IsDictionary()) return;

            MoveToIndex(UInt32.MaxValue);
            _endOfLevel = false;
        }

        public bool TryVisitValue(VisitArgs args, out byte? value)
        {
            if (args.Metadata.Index > 0 && !MoveToIndex(args.Metadata.Index)) {
                value = null;
                return false;
            }
            var length = _reader.ReadByte();
            if (length == BinaryPacker.Null) {
                value = null;
                return true;
            }
            if (length != BinaryInformation.Byte.FixedLength)
                throw new UnexpectedLengthException(args, length);

            value = _reader.ReadByte();
            return true;
        }

        public bool TryVisitValue(VisitArgs args, out short? value)
        {
            if (args.Metadata.Index > 0 && !MoveToIndex(args.Metadata.Index)) {
                value = null;
                return false;
            }
            var length = _reader.ReadByte();
            if (length == BinaryPacker.Null) {
                value = null;
                return true;
            }
            if (length != BinaryInformation.Int16.FixedLength)
                throw new UnexpectedLengthException(args, length);

            value = _reader.ReadInt16();
            return true;
        }

        public bool TryVisitValue(VisitArgs args, out int? value)
        {
            if (args.Metadata.Index > 0 && !MoveToIndex(args.Metadata.Index)) {
                value = null;
                return false;
            }
            var length = _reader.ReadByte();
            if (length == BinaryPacker.Null) {
                value = null;
                return true;
            }
            if (length != BinaryInformation.Int32.FixedLength)
                throw new UnexpectedLengthException(args, length);

            value = _reader.ReadInt32();
            return true;
        }

        public bool TryVisitValue(VisitArgs args, out long? value)
        {
            if (args.Metadata.Index > 0 && !MoveToIndex(args.Metadata.Index)) {
                value = null;
                return false;
            }
            var length = _reader.ReadByte();
            if (length == BinaryPacker.Null) {
                value = null;
                return true;
            }
            if (length != BinaryInformation.Int64.FixedLength)
                throw new UnexpectedLengthException(args, length);

            value = _reader.ReadInt64();
            return true;
        }

        public bool TryVisitValue(VisitArgs args, out ushort? value)
        {
            if (args.Metadata.Index > 0 && !MoveToIndex(args.Metadata.Index)) {
                value = null;
                return false;
            }
            var length = _reader.ReadByte();
            if (length == BinaryPacker.Null) {
                value = null;
                return true;
            }
            if (length != BinaryInformation.UInt16.FixedLength)
                throw new UnexpectedLengthException(args, length);

            value = _reader.ReadUInt16();
            return true;
        }

        public bool TryVisitValue(VisitArgs args, out uint? value)
        {
            if (args.Metadata.Index > 0 && !MoveToIndex(args.Metadata.Index)) {
                value = null;
                return false;
            }
            var length = _reader.ReadByte();
            if (length == BinaryPacker.Null) {
                value = null;
                return true;
            }
            if (length != BinaryInformation.UInt32.FixedLength)
                throw new UnexpectedLengthException(args, length);

            value = _reader.ReadUInt32();
            return true;
        }

        public bool TryVisitValue(VisitArgs args, out ulong? value)
        {
            if (args.Metadata.Index > 0 && !MoveToIndex(args.Metadata.Index)) {
                value = null;
                return false;
            }
            var length = _reader.ReadByte();
            if (length == BinaryPacker.Null) {
                value = null;
                return true;
            }
            if (length != BinaryInformation.UInt64.FixedLength)
                throw new UnexpectedLengthException(args, length);

            value = _reader.ReadUInt64();
            return true;
        }

        public bool TryVisitValue(VisitArgs args, out bool? value)
        {
            if (args.Metadata.Index > 0 && !MoveToIndex(args.Metadata.Index)) {
                value = null;
                return false;
            }
            var length = _reader.ReadByte();
            if (length == BinaryPacker.Null) {
                value = null;
                return true;
            }
            if (length != BinaryInformation.Boolean.FixedLength)
                throw new UnexpectedLengthException(args, length);

            value = _reader.ReadBoolean();
            return true;
        }

        public bool TryVisitValue(VisitArgs args, out float? value)
        {
            if (args.Metadata.Index > 0 && !MoveToIndex(args.Metadata.Index)) {
                value = null;
                return false;
            }
            var length = _reader.ReadByte();
            if (length == BinaryPacker.Null) {
                value = null;
                return true;
            }
            if (length != BinaryInformation.Single.FixedLength)
                throw new UnexpectedLengthException(args, length);

            value = _reader.ReadSingle();
            return true;
        }

        public bool TryVisitValue(VisitArgs args, out double? value)
        {
            if (args.Metadata.Index > 0 && !MoveToIndex(args.Metadata.Index)) {
                value = null;
                return false;
            }
            var length = _reader.ReadByte();
            if (length == BinaryPacker.Null) {
                value = null;
                return true;
            }
            if (length != BinaryInformation.Double.FixedLength)
                throw new UnexpectedLengthException(args, length);

            value = _reader.ReadDouble();
            return true;
        }

        public bool TryVisitValue(VisitArgs args, out decimal? value)
        {
            if (args.Metadata.Index > 0 && !MoveToIndex(args.Metadata.Index)) {
                value = null;
                return false;
            }
            var length = _reader.ReadByte();
            if (length == BinaryPacker.Null) {
                value = null;
                return true;
            }
            if (length != BinaryInformation.Decimal.FixedLength)
                throw new UnexpectedLengthException(args, length);

            value = _reader.ReadDecimal();
            return true;
        }

        public bool TryVisitValue(VisitArgs args, out TimeSpan? value)
        {
            if (args.Metadata.Index > 0 && !MoveToIndex(args.Metadata.Index)) {
                value = null;
                return false;
            }
            var length = _reader.ReadByte();
            if (length == BinaryPacker.Null) {
                value = null;
                return true;
            }
            if (length != BinaryInformation.TimeSpan.FixedLength)
                throw new UnexpectedLengthException(args, length);

            value = _reader.ReadTimeSpan();
            return true;
        }

        public bool TryVisitValue(VisitArgs args, out DateTime? value)
        {
            if (args.Metadata.Index > 0 && !MoveToIndex(args.Metadata.Index)) {
                value = null;
                return false;
            }
            var length = _reader.ReadByte();
            if (length == BinaryPacker.Null) {
                value = null;
                return true;
            }
            if (length != BinaryInformation.DateTime.FixedLength)
                throw new UnexpectedLengthException(args, length);

            value = _reader.ReadDateTime();
            return true;
        }

        public bool TryVisitValue(VisitArgs args, out string value)
        {
            if (args.Metadata.Index > 0 && !MoveToIndex(args.Metadata.Index)) {
                value = null;
                return false;
            }
            var length = _reader.ReadByte();
            if (length == BinaryPacker.Null) {
                value = null;
                return true;
            }
            if (length != BinaryPacker.VariabelLength)
                throw new UnexpectedLengthException(args, length);

            value = _reader.ReadString();
            return true;
        }

        public bool TryVisitValue(VisitArgs args, out Guid? value)
        {
            if (args.Metadata.Index > 0 && !MoveToIndex(args.Metadata.Index)) {
                value = null;
                return false;
            }
            var length = _reader.ReadByte();
            if (length == BinaryPacker.Null) {
                value = null;
                return true;
            }
            if (length != BinaryInformation.Guid.FixedLength)
                throw new UnexpectedLengthException(args, length);

            value = _reader.ReadGuid();
            return true;
        }

        public bool TryVisitValue(VisitArgs args, out byte[] value)
        {
            if (args.Metadata.Index > 0 && !MoveToIndex(args.Metadata.Index)) {
                value = null;
                return false;
            }
            var length = _reader.ReadByte();
            if (length == BinaryPacker.Null) {
                value = null;
                return true;
            }
            if (length != BinaryPacker.VariabelLength)
                throw new UnexpectedLengthException(args, length);

            value = _reader.ReadBlob();
            return true;
        }
    }
}