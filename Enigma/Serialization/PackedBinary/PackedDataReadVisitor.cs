using System;
using System.IO;
using Enigma.Binary;
using Enigma.IO;

namespace Enigma.Serialization.PackedBinary
{
    public class PackedDataReadVisitor : IReadVisitor
    {
        private readonly Stream _stream;
        private readonly BinaryDataReader _reader;
        private UInt32 _nextIndex;
        private bool _endOfLevel;

        public PackedDataReadVisitor(Stream stream)
        {
            _stream = stream;
            _reader = new BinaryDataReader(stream);
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
                if (byteLength == BinaryZPacker.Null) continue;

                if (byteLength != BinaryZPacker.VariabelLength)
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
            if (byteLength == BinaryZPacker.Null) return ValueState.Null;
            if (byteLength != BinaryZPacker.VariabelLength)
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
            if (length == BinaryZPacker.Null) {
                value = null;
                return true;
            }
            if (length != BinaryInformation.Byte.FixedLength)
                throw new UnexpectedLengthException(args, length);

            value = (Byte)_stream.ReadByte();
            return true;
        }

        public bool TryVisitValue(VisitArgs args, out short? value)
        {
            if (args.Metadata.Index > 0 && !MoveToIndex(args.Metadata.Index)) {
                value = null;
                return false;
            }
            var length = _reader.ReadByte();
            if (length == BinaryZPacker.Null) {
                value = null;
                return true;
            }
            value = (Int16)BinaryPV64Packer.UnpackS(_stream, length);
            return true;
        }

        public bool TryVisitValue(VisitArgs args, out int? value)
        {
            if (args.Metadata.Index > 0 && !MoveToIndex(args.Metadata.Index)) {
                value = null;
                return false;
            }
            var length = _reader.ReadByte();
            if (length == BinaryZPacker.Null) {
                value = null;
                return true;
            }
            value = (Int32)BinaryPV64Packer.UnpackS(_stream, length);
            return true;
        }

        public bool TryVisitValue(VisitArgs args, out long? value)
        {
            if (args.Metadata.Index > 0 && !MoveToIndex(args.Metadata.Index)) {
                value = null;
                return false;
            }
            var length = _reader.ReadByte();
            if (length == BinaryZPacker.Null) {
                value = null;
                return true;
            }
            value = BinaryPV64Packer.UnpackS(_stream, length);
            return true;
        }

        public bool TryVisitValue(VisitArgs args, out ushort? value)
        {
            if (args.Metadata.Index > 0 && !MoveToIndex(args.Metadata.Index)) {
                value = null;
                return false;
            }
            var length = _reader.ReadByte();
            if (length == BinaryZPacker.Null) {
                value = null;
                return true;
            }
            value = (UInt16)BinaryPV64Packer.UnpackU(_stream, length);
            return true;
        }

        public bool TryVisitValue(VisitArgs args, out uint? value)
        {
            if (args.Metadata.Index > 0 && !MoveToIndex(args.Metadata.Index)) {
                value = null;
                return false;
            }
            var length = _reader.ReadByte();
            if (length == BinaryZPacker.Null) {
                value = null;
                return true;
            }
            value = (UInt32)BinaryPV64Packer.UnpackU(_stream, length);
            return true;
        }

        public bool TryVisitValue(VisitArgs args, out ulong? value)
        {
            if (args.Metadata.Index > 0 && !MoveToIndex(args.Metadata.Index)) {
                value = null;
                return false;
            }
            var length = _reader.ReadByte();
            if (length == BinaryZPacker.Null) {
                value = null;
                return true;
            }
            value = BinaryPV64Packer.UnpackU(_stream, length);
            return true;
        }

        public bool TryVisitValue(VisitArgs args, out bool? value)
        {
            if (args.Metadata.Index > 0 && !MoveToIndex(args.Metadata.Index)) {
                value = null;
                return false;
            }
            var length = _reader.ReadByte();
            if (length == BinaryZPacker.Null) {
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
            if (length == BinaryZPacker.Null) {
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
            if (length == BinaryZPacker.Null) {
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
            if (length == BinaryZPacker.Null) {
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
            if (length == BinaryZPacker.Null) {
                value = null;
                return true;
            }
            value = new TimeSpan(BinaryPV64Packer.UnpackS(_stream, length));
            return true;
        }

        public bool TryVisitValue(VisitArgs args, out DateTime? value)
        {
            if (args.Metadata.Index > 0 && !MoveToIndex(args.Metadata.Index)) {
                value = null;
                return false;
            }
            var length = _reader.ReadByte();
            if (length == BinaryZPacker.Null) {
                value = null;
                return true;
            }
            value = new DateTime(BinaryPV64Packer.UnpackS(_stream, length), DateTimeKind.Utc).ToLocalTime();
            return true;
        }

        public bool TryVisitValue(VisitArgs args, out string value)
        {
            if (args.Metadata.Index > 0 && !MoveToIndex(args.Metadata.Index)) {
                value = null;
                return false;
            }
            var length = _reader.ReadByte();
            if (length == BinaryZPacker.Null) {
                value = null;
                return true;
            }

            var lengthToRead = length == BinaryZPacker.VariabelLength ? _reader.ReadV() : length;

            value = _reader.ReadString(lengthToRead);
            return true;
        }

        public bool TryVisitValue(VisitArgs args, out Guid? value)
        {
            if (args.Metadata.Index > 0 && !MoveToIndex(args.Metadata.Index)) {
                value = null;
                return false;
            }
            var length = _reader.ReadByte();
            if (length == BinaryZPacker.Null) {
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
            if (length == BinaryZPacker.Null) {
                value = null;
                return true;
            }

            var lengthToRead = length == BinaryZPacker.VariabelLength ? _reader.ReadV() : length;

            value = _reader.ReadBlob(lengthToRead);
            return true;
        }
    }
}