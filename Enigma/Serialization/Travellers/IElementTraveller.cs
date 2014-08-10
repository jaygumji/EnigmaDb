namespace Enigma.Serialization.Travellers
{
    public interface IElementTraveller<T>
    {
        void VisitValue(IWriteVisitor visitor, T value, WriteVisitArgs args);
        bool TryVisitValue(IReadVisitor visitor, ReadVisitArgs args, out T value);

        bool IsNull(T value);
    }

    internal class ElementTravellerComplex<T> : IElementTraveller<T> where T : class
    {
        private readonly IGraphTraveller<T> _traveller;

        public ElementTravellerComplex(IGraphTraveller<T> traveller)
        {
            _traveller = traveller;
        }

        public void VisitValue(IWriteVisitor visitor, T value, WriteVisitArgs args)
        {
            visitor.Visit(args);
            _traveller.Travel(visitor, value);
            visitor.Leave();
        }

        public bool TryVisitValue(IReadVisitor visitor, ReadVisitArgs args, out T value)
        {
            if (visitor.TryVisit(args) == ValueState.Found) {
                //var value 
                //_traveller.Travel();
            }

            value = default(T);
            return false;
        }

        public bool IsNull(T value)
        {
            throw new System.NotImplementedException();
        }
    }
}