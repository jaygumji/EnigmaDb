using Enigma.Db.Linq;
using Enigma.Store;
using System.Collections.Generic;

namespace Enigma.Db
{
    public interface IEnigmaEntityEngine
    {
    }

    public interface IEnigmaEntityEngine<T> : IEnigmaEntityEngine
    {
        bool TryAdd(T entity);
        bool TryUpdate(T entity);
        bool TryRemove(T entity);

        void CommitModifications();

        bool TryGet(object id, out T entity);

        IEnumerable<T> All();
        IEnumerable<T> Range(int skip, int take);

        bool TryResolve(IEnigmaExpressionTreeExecutor executor, out IEnumerable<T> values);
    }

}
