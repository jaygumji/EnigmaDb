namespace Enigma.Serialization.Travellers
{
    public interface IElementValueTraveller<T>
    {
        void VisitValue(IWriteVisitor visitor, T value, WriteVisitArgs args);
        bool TryVisitValue(IReadVisitor visitor, ReadVisitArgs args, out T value);

        bool IsNull(T value);
    }
}