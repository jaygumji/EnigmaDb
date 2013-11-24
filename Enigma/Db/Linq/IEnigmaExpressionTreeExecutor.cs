using System.Collections.Generic;
using Enigma.Store;
using Enigma.Store.Indexes;

namespace Enigma.Db.Linq
{
    public interface IEnigmaExpressionTreeExecutor
    {
        void Compare(PropertyPath path, CompareOperation compareOperation, object value);
        void And();
        void Or();
        void EqualKey(object value);
        void Unknown();

        void OrderBy(PropertyPath path, OrderingDirection direction);
        void Take(int count);
        void Skip(int count);

        bool TryResolve(out IEnumerable<IKey> keys);
    }
}