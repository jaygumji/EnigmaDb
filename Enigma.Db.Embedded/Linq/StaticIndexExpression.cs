using System.Collections.Generic;
using Enigma.Store;
using Enigma.Store.Keys;

namespace Enigma.Db.Embedded.Linq
{
    internal class StaticIndexExpression : IIndexExpression
    {

        private readonly IEnumerable<IKey> _keys;

        public StaticIndexExpression(object id)
        {
            _keys = new[] {Key.Create(id)};
        }

        public bool IsNull { get { return false; } }

        public IEnumerable<IKey> Resolve(EmbeddedEnigmaService service)
        {
            return _keys;
        }
    }
}