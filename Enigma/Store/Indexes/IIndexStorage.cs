using Enigma.Db.Linq;
using System.Collections.Generic;
namespace Enigma.Store.Indexes
{
    interface IIndexStorage
    {
        IComparableIndex Index { get; }
        bool IsModified { get; }

        void Initialize();

        void Add(IKey key, object entity);
        void Update(IKey key, object entity);
        void Remove(IKey entityId);

        void CommitModifications();
        
        IEnumerable<IKey> Match(CompareOperation operation, object value);
    }
}
