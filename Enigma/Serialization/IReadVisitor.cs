using System;

namespace Enigma.Serialization
{
    public interface IReadVisitor
    {
        ValueState TryVisit(VisitArgs args);
        void Leave(VisitArgs args);

        bool TryVisitValue(VisitArgs args, out Byte? value);
        bool TryVisitValue(VisitArgs args, out Int16? value);
        bool TryVisitValue(VisitArgs args, out Int32? value);
        bool TryVisitValue(VisitArgs args, out Int64? value);
        bool TryVisitValue(VisitArgs args, out UInt16? value);
        bool TryVisitValue(VisitArgs args, out UInt32? value);
        bool TryVisitValue(VisitArgs args, out UInt64? value);
        bool TryVisitValue(VisitArgs args, out Boolean? value);
        bool TryVisitValue(VisitArgs args, out Single? value);
        bool TryVisitValue(VisitArgs args, out Double? value);
        bool TryVisitValue(VisitArgs args, out Decimal? value);
        bool TryVisitValue(VisitArgs args, out TimeSpan? value);
        bool TryVisitValue(VisitArgs args, out DateTime? value);
        bool TryVisitValue(VisitArgs args, out String value);
        bool TryVisitValue(VisitArgs args, out Guid? value);
        bool TryVisitValue(VisitArgs args, out byte[] value);
    }
}