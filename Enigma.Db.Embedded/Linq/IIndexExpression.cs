using System.Collections.Generic;
using Enigma.Store;

namespace Enigma.Db.Embedded.Linq
{
    internal interface IIndexExpression
    {
        bool IsNull { get; }

        IEnumerable<IKey> Resolve(EmbeddedEnigmaService service);
    }
}