using System.Collections.Generic;

namespace Enigma.Store.Indexes
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