using System;
using System.Collections.Generic;
using Enigma.Binary;
using Enigma.IO;

namespace Enigma.Serialization
{
    public class PackedDataWriteVisitor : IWriteVisitor
    {
        private readonly BinaryDataWriter _writer;
        private readonly Stack<WriteVisitArgs> _stack;

        public PackedDataWriteVisitor(BinaryDataWriter writer)
        {
            _writer = writer;
            _stack = new Stack<WriteVisitArgs>();
        }

        public void Visit(WriteVisitArgs args)
        {
            _stack.Push(args);

            switch (args.Type) {
                case LevelType.Single:
                case LevelType.Collection:
                case LevelType.Dictionary:
                    _writer.WriteZ(args.Index);
                    _writer.Write(Byte.MaxValue);
                    // Create a reservation to write the length of the entry
                    args.State = _writer.Reserve();
                    break;
                case LevelType.Item:
                case LevelType.KeyValue:
                    _writer.Write(true);
                    break;
            }
        }

        public void Leave()
        {
            var args = _stack.Pop();
            switch (args.Type) {
                case LevelType.Single:
                case LevelType.Collection:
                case LevelType.Dictionary:
                    _writer.WriteZ(0);
                    // Updates the reservation with the length of this entry
                    _writer.Write((WriteReservation) args.State);
                    break;
            }
        }

        public void VisitValue(byte? value, WriteVisitArgs args)
        {
            if (args.Index > 0)
                _writer.WriteZ(args.Index);

            if (value == null) {
                _writer.Write(BinaryPacker.Null);
                return;
            }

            _writer.Write((Byte)BinaryInformation.Byte.FixedLength);
            _writer.Write(value.Value);
        }

        public void VisitValue(short? value, WriteVisitArgs args)
        {
            if (args.Index > 0)
                _writer.WriteZ(args.Index);

            if (value == null) {
                _writer.Write(BinaryPacker.Null);
                return;
            }

            _writer.Write((Byte)BinaryInformation.Int16.FixedLength);
            _writer.Write(value.Value);
        }

        public void VisitValue(int? value, WriteVisitArgs args)
        {
            if (args.Index > 0)
                _writer.WriteZ(args.Index);

            if (value == null) {
                _writer.Write(BinaryPacker.Null);
                return;
            }

            _writer.Write((Byte)BinaryInformation.Int32.FixedLength);
            _writer.Write(value.Value);
        }

        public void VisitValue(long? value, WriteVisitArgs args)
        {
            if (args.Index > 0)
                _writer.WriteZ(args.Index);

            if (value == null) {
                _writer.Write(BinaryPacker.Null);
                return;
            }

            _writer.Write((Byte)BinaryInformation.Int64.FixedLength);
            _writer.Write(value.Value);
        }

        public void VisitValue(ushort? value, WriteVisitArgs args)
        {
            if (args.Index > 0)
                _writer.WriteZ(args.Index);

            if (value == null) {
                _writer.Write(BinaryPacker.Null);
                return;
            }

            _writer.Write((Byte)BinaryInformation.UInt16.FixedLength);
            _writer.Write(value.Value);
        }

        public void VisitValue(uint? value, WriteVisitArgs args)
        {
            if (args.Index > 0)
                _writer.WriteZ(args.Index);

            if (value == null) {
                _writer.Write(BinaryPacker.Null);
                return;
            }

            _writer.Write((Byte)BinaryInformation.UInt32.FixedLength);
            _writer.Write(value.Value);
        }

        public void VisitValue(ulong? value, WriteVisitArgs args)
        {
            if (args.Index > 0)
                _writer.WriteZ(args.Index);

            if (value == null) {
                _writer.Write(BinaryPacker.Null);
                return;
            }

            _writer.Write((Byte)BinaryInformation.UInt64.FixedLength);
            _writer.Write(value.Value);
        }

        public void VisitValue(bool? value, WriteVisitArgs args)
        {
            if (args.Index > 0)
                _writer.WriteZ(args.Index);

            if (value == null) {
                _writer.Write(BinaryPacker.Null);
                return;
            }

            _writer.Write((Byte)BinaryInformation.Boolean.FixedLength);
            _writer.Write(value.Value);
        }

        public void VisitValue(float? value, WriteVisitArgs args)
        {
            if (args.Index > 0)
                _writer.WriteZ(args.Index);

            if (value == null) {
                _writer.Write(BinaryPacker.Null);
                return;
            }

            _writer.Write((Byte)BinaryInformation.Single.FixedLength);
            _writer.Write(value.Value);
        }

        public void VisitValue(double? value, WriteVisitArgs args)
        {
            if (args.Index > 0)
                _writer.WriteZ(args.Index);

            if (value == null) {
                _writer.Write(BinaryPacker.Null);
                return;
            }

            _writer.Write((Byte)BinaryInformation.Double.FixedLength);
            _writer.Write(value.Value);
        }

        public void VisitValue(decimal? value, WriteVisitArgs args)
        {
            if (args.Index > 0)
                _writer.WriteZ(args.Index);

            if (value == null) {
                _writer.Write(BinaryPacker.Null);
                return;
            }

            _writer.Write((Byte)BinaryInformation.Decimal.FixedLength);
            _writer.Write(value.Value);
        }

        public void VisitValue(TimeSpan? value, WriteVisitArgs args)
        {
            if (args.Index > 0)
                _writer.WriteZ(args.Index);

            if (value == null) {
                _writer.Write(BinaryPacker.Null);
                return;
            }

            _writer.Write((Byte)BinaryInformation.TimeSpan.FixedLength);
            _writer.Write(value.Value);
        }

        public void VisitValue(DateTime? value, WriteVisitArgs args)
        {
            if (args.Index > 0)
                _writer.WriteZ(args.Index);
            
            if (value == null) {
                _writer.Write(BinaryPacker.Null);
                return;
            }

            _writer.Write((Byte)BinaryInformation.DateTime.FixedLength);
            _writer.Write(value.Value);
        }

        public void VisitValue(string value, WriteVisitArgs args)
        {
            if (args.Index > 0)
                _writer.WriteZ(args.Index);

            if (value == null) {
                _writer.Write(BinaryPacker.Null);
                return;
            }

            _writer.Write(BinaryPacker.VariabelLength);
            _writer.Write(value);
        }

        public void VisitValue(Guid? value, WriteVisitArgs args)
        {
            if (args.Index > 0)
                _writer.WriteZ(args.Index);

            if (value == null) {
                _writer.Write(BinaryPacker.Null);
                return;
            }

            _writer.Write((Byte)BinaryInformation.Guid.FixedLength);
            _writer.Write(value.Value);
        }

        public void VisitValue(byte[] value, WriteVisitArgs args)
        {
            if (args.Index > 0)
                _writer.WriteZ(args.Index);

            if (value == null) {
                _writer.Write(BinaryPacker.Null);
                return;
            }

            _writer.Write(BinaryPacker.VariabelLength);
            _writer.Write(value);
        }

    }
}