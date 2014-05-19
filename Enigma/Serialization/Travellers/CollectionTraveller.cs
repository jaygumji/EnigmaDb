using System.Collections.Generic;

namespace Enigma.Serialization.Travellers
{
    public class CollectionTraveller<T> where T : class
    {

        private readonly IElementValueTraveller<T> _traveller;

        public CollectionTraveller(IElementValueTraveller<T> traveller)
        {
            _traveller = traveller;
        }

        public void VisitChildren(IWriteVisitor visitor, ICollection<T> collection)
        {
            foreach (var value in collection)
                _traveller.VisitValue(visitor, value, WriteVisitArgs.CollectionItem);
        }

        public void VisitChildren(IReadVisitor visitor, ICollection<T> collection)
        {
            T value;
            while (_traveller.TryVisitValue(visitor, ReadVisitArgs.CollectionItem, out value) && !_traveller.IsNull(value))
                collection.Add(value);
        }
        
    }
}