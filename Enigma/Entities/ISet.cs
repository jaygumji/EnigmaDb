using System.Collections.Generic;
using System.Linq;

namespace Enigma.Entities
{
    public interface ISet : IQueryable
    {
    }

    public interface ISet<T> : IQueryable<T>, ISet
    {
        void Add(T entity);
        void Remove(T entity);

        T Get(object id);
        bool TryGet(object id, out T entity);
    }
}
