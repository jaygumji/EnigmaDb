using System;

namespace Enigma.Serialization.Travellers
{
    internal class ElementTravellerInt16 : IElementTraveller<short>, IElementTraveller<short?>
    {
        public void VisitValue(IWriteVisitor visitor, Int16 value, WriteVisitArgs args)
        {
            visitor.VisitValue(value, args);
        }

        public bool TryVisitValue(IReadVisitor visitor, ReadVisitArgs args, out Int16 value)
        {
            Int16? temp;
            if (visitor.TryVisitValue(args, out temp) && temp.HasValue) {
                value = temp.Value;
                return true;
            }
            value = default(Int16);
            return false;
        }

        public bool IsNull(Int16 value)
        {
            return false;
        }

        public void VisitValue(IWriteVisitor visitor, Int16? value, WriteVisitArgs args)
        {
            visitor.VisitValue(value, args);
        }

        public bool TryVisitValue(IReadVisitor visitor, ReadVisitArgs args, out Int16? value)
        {
            return visitor.TryVisitValue(args, out value);
        }

        public bool IsNull(Int16? value)
        {
            return !value.HasValue;
        }
    }
}