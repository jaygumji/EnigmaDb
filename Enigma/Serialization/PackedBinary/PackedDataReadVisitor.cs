using System;
using System.Collections.Generic;
using Enigma.Binary;
using Enigma.IO;

namespace Enigma.Serialization.PackedBinary
{
    public class PackedDataReadVisitor : IReadVisitor
    {
        private readonly BinaryDataReader _reader;
        private readonly Stack<ReadVisitArgs> _stack;
        private UInt32 _nextIndex;
        private bool _endOfLevel;

        public PackedDataReadVisitor(BinaryDataReader reader)
        {
            _reader = reader;
            _stack = new Stack<ReadVisitArgs>();
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

        public ValueState TryVisit(ReadVisitArgs args)
        {
            if (args.Type != LevelType.Root)
                if (args.Index > 0 && !MoveToIndex(args.Index))
                    return ValueState.NotFound;

            switch (args.Type) {
                case LevelType.Single:
                case LevelType.Collection:
                case LevelType.Dictionary:
                case LevelType.DictionaryKey:
                case LevelType.DictionaryValue:
                case LevelType.CollectionItem:
                    var byteLength = _reader.ReadByte();
                    if (byteLength == BinaryPacker.Null) return ValueState.Null;
                    if (byteLength != BinaryPacker.VariabelLength)
                        throw new UnexpectedLengthException(args, byteLength);

                    _reader.Skip(4);

                    _stack.Push(args);
                    return ValueState.Found;
            }
            _stack.Push(args);
            return ValueState.Found;
        }

        public void Leave()
        {
            var args = _stack.Pop();

            if (_endOfLevel) {
                _endOfLevel = false;
                return;
            }

            switch (args.Type) {
                case LevelType.Single:
                case LevelType.DictionaryKey:
                case LevelType.DictionaryValue:
                case LevelType.CollectionItem:
                    MoveToIndex(UInt32.MaxValue);
                    _endOfLevel = false;
                    break;
            }
        }

        public bool TryVisitValue(ReadVisitArgs args, out byte? value)
        {
            if (args.Index > 0 && !MoveToIndex(args.Index)) {
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

        public bool TryVisitValue(ReadVisitArgs args, out short? value)
        {
            if (args.Index > 0 && !MoveToIndex(args.Index)) {
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

        public bool TryVisitValue(ReadVisitArgs args, out int? value)
        {
            if (args.Index > 0 && !MoveToIndex(args.Index)) {
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

        public bool TryVisitValue(ReadVisitArgs args, out long? value)
        {
            if (args.Index > 0 && !MoveToIndex(args.Index)) {
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

        public bool TryVisitValue(ReadVisitArgs args, out ushort? value)
        {
            if (args.Index > 0 && !MoveToIndex(args.Index)) {
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

        public bool TryVisitValue(ReadVisitArgs args, out uint? value)
        {
            if (args.Index > 0 && !MoveToIndex(args.Index)) {
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

        public bool TryVisitValue(ReadVisitArgs args, out ulong? value)
        {
            if (args.Index > 0 && !MoveToIndex(args.Index)) {
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

        public bool TryVisitValue(ReadVisitArgs args, out bool? value)
        {
            if (args.Index > 0 && !MoveToIndex(args.Index)) {
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

        public bool TryVisitValue(ReadVisitArgs args, out float? value)
        {
            if (args.Index > 0 && !MoveToIndex(args.Index)) {
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

        public bool TryVisitValue(ReadVisitArgs args, out double? value)
        {
            if (args.Index > 0 && !MoveToIndex(args.Index)) {
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

        public bool TryVisitValue(ReadVisitArgs args, out decimal? value)
        {
            if (args.Index > 0 && !MoveToIndex(args.Index)) {
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

        public bool TryVisitValue(ReadVisitArgs args, out TimeSpan? value)
        {
            if (args.Index > 0 && !MoveToIndex(args.Index)) {
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

        public bool TryVisitValue(ReadVisitArgs args, out DateTime? value)
        {
            if (args.Index > 0 && !MoveToIndex(args.Index)) {
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

        public bool TryVisitValue(ReadVisitArgs args, out string value)
        {
            if (args.Index > 0 && !MoveToIndex(args.Index)) {
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

        public bool TryVisitValue(ReadVisitArgs args, out Guid? value)
        {
            if (args.Index > 0 && !MoveToIndex(args.Index)) {
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

        public bool TryVisitValue(ReadVisitArgs args, out byte[] value)
        {
            if (args.Index > 0 && !MoveToIndex(args.Index)) {
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