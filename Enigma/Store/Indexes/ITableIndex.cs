using System.Collections.Generic;
using Enigma.Modelling;

namespace Enigma.Store.Indexes
{
    public interface ITableIndex
    {
        IIndexAlgorithm IndexAlgorithm { get; }
        IIndexStorage Storage { get; }

        void CommitModifications();

        IEnumerable<IKey> Match(CompareOperation operation, object value);
    }
}
