using System;
using System.Collections.Generic;
using Enigma.Binary;
using Enigma.IO;

namespace Enigma.Serialization.PackedBinary
{
    public class PackedDataWriteVisitor : IWriteVisitor
    {
        private readonly BinaryDataWriter _writer;
        private readonly Stack<WriteReservation> _reservations; 

        public PackedDataWriteVisitor(BinaryDataWriter writer)
        {
            _writer = writer;
            _reservations = new Stack<WriteReservation>();
        }

        public void Visit(object level, VisitArgs args)
        {
            if (args.Type == LevelType.Root) return;

            if (args.Metadata.Index > 0)
                _writer.WriteZ(args.Metadata.Index);

            if (level != null) {
                _writer.Write(BinaryPacker.VariabelLength);

                // Create a reservation to write the length of the entry
                _reservations.Push(_writer.Reserve());
            }
            else
                _writer.Write(BinaryPacker.Null);
        }

        public void Leave(object level, VisitArgs args)
        {
            if (args.Type == LevelType.Root) return;

            if (level != null) {
                _writer.WriteZ(0);
                // Updates the reservation with the length of this entry
                _writer.Write(_reservations.Pop());
            }
        }

        public void VisitValue(byte? value, VisitArgs args)
        {
            if (args.Metadata.Index > 0)
                _writer.WriteZ(args.Metadata.Index);

            if (value == null) {
                _writer.Write(BinaryPacker.Null);
                return;
            }

            _writer.Write((Byte)BinaryInformation.Byte.FixedLength);
            _writer.Write(value.Value);
        }

        public void VisitValue(short? value, VisitArgs args)
        {
            if (args.Metadata.Index > 0)
                _writer.WriteZ(args.Metadata.Index);

            if (value == null) {
                _writer.Write(BinaryPacker.Null);
                return;
            }

            _writer.Write((Byte)BinaryInformation.Int16.FixedLength);
            _writer.Write(value.Value);
        }

        public void VisitValue(int? value, VisitArgs args)
        {
            if (args.Metadata.Index > 0)
                _writer.WriteZ(args.Metadata.Index);

            if (value == null) {
                _writer.Write(BinaryPacker.Null);
                return;
            }

            _writer.Write((Byte)BinaryInformation.Int32.FixedLength);
            _writer.Write(value.Value);
        }

        public void VisitValue(long? value, VisitArgs args)
        {
            if (args.Metadata.Index > 0)
                _writer.WriteZ(args.Metadata.Index);

            if (value == null) {
                _writer.Write(BinaryPacker.Null);
                return;
            }

            _writer.Write((Byte)BinaryInformation.Int64.FixedLength);
            _writer.Write(value.Value);
        }

        public void VisitValue(ushort? value, VisitArgs args)
        {
            if (args.Metadata.Index > 0)
                _writer.WriteZ(args.Metadata.Index);

            if (value == null) {
                _writer.Write(BinaryPacker.Null);
                return;
            }

            _writer.Write((Byte)BinaryInformation.UInt16.FixedLength);
            _writer.Write(value.Value);
        }

        public void VisitValue(uint? value, VisitArgs args)
        {
            if (args.Metadata.Index > 0)
                _writer.WriteZ(args.Metadata.Index);

            if (value == null) {
                _writer.Write(BinaryPacker.Null);
                return;
            }

            _writer.Write((Byte)BinaryInformation.UInt32.FixedLength);
            _writer.Write(value.Value);
        }

        public void VisitValue(ulong? value, VisitArgs args)
        {
            if (args.Metadata.Index > 0)
                _writer.WriteZ(args.Metadata.Index);

            if (value == null) {
                _writer.Write(BinaryPacker.Null);
                return;
            }

            _writer.Write((Byte)BinaryInformation.UInt64.FixedLength);
            _writer.Write(value.Value);
        }

        public void VisitValue(bool? value, VisitArgs args)
        {
            if (args.Metadata.Index > 0)
                _writer.WriteZ(args.Metadata.Index);

            if (value == null) {
                _writer.Write(BinaryPacker.Null);
                return;
            }

            _writer.Write((Byte)BinaryInformation.Boolean.FixedLength);
            _writer.Write(value.Value);
        }

        public void VisitValue(float? value, VisitArgs args)
        {
            if (args.Metadata.Index > 0)
                _writer.WriteZ(args.Metadata.Index);

            if (value == null) {
                _writer.Write(BinaryPacker.Null);
                return;
            }

            _writer.Write((Byte)BinaryInformation.Single.FixedLength);
            _writer.Write(value.Value);
        }

        public void VisitValue(double? value, VisitArgs args)
        {
            if (args.Metadata.Index > 0)
                _writer.WriteZ(args.Metadata.Index);

            if (value == null) {
                _writer.Write(BinaryPacker.Null);
                return;
            }

            _writer.Write((Byte)BinaryInformation.Double.FixedLength);
            _writer.Write(value.Value);
        }

        public void VisitValue(decimal? value, VisitArgs args)
        {
            if (args.Metadata.Index > 0)
                _writer.WriteZ(args.Metadata.Index);

            if (value == null) {
                _writer.Write(BinaryPacker.Null);
                return;
            }

            _writer.Write((Byte)BinaryInformation.Decimal.FixedLength);
            _writer.Write(value.Value);
        }

        public void VisitValue(TimeSpan? value, VisitArgs args)
        {
            if (args.Metadata.Index > 0)
                _writer.WriteZ(args.Metadata.Index);

            if (value == null) {
                _writer.Write(BinaryPacker.Null);
                return;
            }

            _writer.Write((Byte)BinaryInformation.TimeSpan.FixedLength);
            _writer.Write(value.Value);
        }

        public void VisitValue(DateTime? value, VisitArgs args)
        {
            if (args.Metadata.Index > 0)
                _writer.WriteZ(args.Metadata.Index);
            
            if (value == null) {
                _writer.Write(BinaryPacker.Null);
                return;
            }

            _writer.Write((Byte)BinaryInformation.DateTime.FixedLength);
            _writer.Write(value.Value);
        }

        public void VisitValue(string value, VisitArgs args)
        {
            if (args.Metadata.Index > 0)
                _writer.WriteZ(args.Metadata.Index);

            if (value == null) {
                _writer.Write(BinaryPacker.Null);
                return;
            }

            _writer.Write(BinaryPacker.VariabelLength);
            _writer.Write(value);
        }

        public void VisitValue(Guid? value, VisitArgs args)
        {
            if (args.Metadata.Index > 0)
                _writer.WriteZ(args.Metadata.Index);

            if (value == null) {
                _writer.Write(BinaryPacker.Null);
                return;
            }

            _writer.Write((Byte)BinaryInformation.Guid.FixedLength);
            _writer.Write(value.Value);
        }

        public void VisitValue(byte[] value, VisitArgs args)
        {
            if (args.Metadata.Index > 0)
                _writer.WriteZ(args.Metadata.Index);

            if (value == null) {
                _writer.Write(BinaryPacker.Null);
                return;
            }

            _writer.Write(BinaryPacker.VariabelLength);
            _writer.Write(value);
        }

    }
}