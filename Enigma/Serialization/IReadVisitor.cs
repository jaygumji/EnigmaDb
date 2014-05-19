using System;

namespace Enigma.Serialization
{
    public interface IReadVisitor
    {
        ValueState TryVisit(ReadVisitArgs args);
        void Leave();

        bool TryVisitValue(ReadVisitArgs args, out Byte? value);
        bool TryVisitValue(ReadVisitArgs args, out Int16? value);
        bool TryVisitValue(ReadVisitArgs args, out Int32? value);
        bool TryVisitValue(ReadVisitArgs args, out Int64? value);
        bool TryVisitValue(ReadVisitArgs args, out UInt16? value);
        bool TryVisitValue(ReadVisitArgs args, out UInt32? value);
        bool TryVisitValue(ReadVisitArgs args, out UInt64? value);
        bool TryVisitValue(ReadVisitArgs args, out Boolean? value);
        bool TryVisitValue(ReadVisitArgs args, out Single? value);
        bool TryVisitValue(ReadVisitArgs args, out Double? value);
        bool TryVisitValue(ReadVisitArgs args, out Decimal? value);
        bool TryVisitValue(ReadVisitArgs args, out TimeSpan? value);
        bool TryVisitValue(ReadVisitArgs args, out DateTime? value);
        bool TryVisitValue(ReadVisitArgs args, out String value);
        bool TryVisitValue(ReadVisitArgs args, out Guid? value);
        bool TryVisitValue(ReadVisitArgs args, out byte[] value);
    }
}