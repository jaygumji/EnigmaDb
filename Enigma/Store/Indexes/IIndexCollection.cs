using Enigma.Db.Linq;
using System.Collections.Generic;
namespace Enigma.Store.Indexes
{
    public interface IIndexCollection
    {
        bool IsModified { get; }

        bool NeedsRepair();
        void MarkAsRepaired();

        void Repair(IKey key, object entity);
        void Add(IKey key, object entity);
        void Update(IKey key, object entity);
        void Remove(IKey entityId);

        void CommitModifications();

        IEnumerable<IKey> Match(IEnumerable<EnigmaIndexOperation> list);
    }
}
