using System;
using System.Collections.Generic;

namespace Enigma.Serialization.Reflection.Graph
{
    public interface IGraphTravellerCollection : IEnumerable<IGraphTraveller>
    {
        IGraphTraveller GetOrAdd(Type type);
    }
}