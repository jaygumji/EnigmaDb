using System;

namespace Enigma.Serialization
{
    public interface IWriteVisitor
    {
        void Visit(WriteVisitArgs args);
        void Leave();

        void VisitValue(Byte? value, WriteVisitArgs args);
        void VisitValue(Int16? value, WriteVisitArgs args);
        void VisitValue(Int32? value, WriteVisitArgs args);
        void VisitValue(Int64? value, WriteVisitArgs args);
        void VisitValue(UInt16? value, WriteVisitArgs args);
        void VisitValue(UInt32? value, WriteVisitArgs args);
        void VisitValue(UInt64? value, WriteVisitArgs args);
        void VisitValue(Boolean? value, WriteVisitArgs args);
        void VisitValue(Single? value, WriteVisitArgs args);
        void VisitValue(Double? value, WriteVisitArgs args);
        void VisitValue(Decimal? value, WriteVisitArgs args);
        void VisitValue(TimeSpan? value, WriteVisitArgs args);
        void VisitValue(DateTime? value, WriteVisitArgs args);
        void VisitValue(String value, WriteVisitArgs args);
        void VisitValue(Guid? value, WriteVisitArgs args);
        void VisitValue(byte[] value, WriteVisitArgs args);
    }
}