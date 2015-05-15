using System;

namespace Enigma.Serialization
{
    public interface IVisitArgsFactory
    {
        IVisitArgsFactory ConstructWith(Type type);

        VisitArgs Construct(string propertyName);
    }
}
