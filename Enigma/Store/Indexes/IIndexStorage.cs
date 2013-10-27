using System.Collections.Generic;

namespace Enigma.Store.Indexes
{
    public interface IIndexStorage
    {
        bool IsModified { get; }
        bool IsEmpty { get; }

        void Initialize();

        void Add(IKey key, object entity);
        void Update(IKey key, object entity);
        void Remove(IKey entityId);
    }
}
