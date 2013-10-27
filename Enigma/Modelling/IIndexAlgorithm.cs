using System.Collections.Generic;
using Enigma.Store;
using Enigma.Store.Indexes;

namespace Enigma.Modelling
{
    /// <summary>
    /// An index algorith to use in the index
    /// </summary>
    public interface IIndexAlgorithm
    {
    }

    /// <summary>
    /// An index algorith to use in the index
    /// </summary>
    public interface IIndexAlgorithm<T> : IIndexAlgorithm
    {
        void Update(ImmutableSortedCollection<T, IList<IKey>> indexedValues);
    }
}