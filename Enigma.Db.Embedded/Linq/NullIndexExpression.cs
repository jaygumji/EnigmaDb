using System.Collections.Generic;
using Enigma.Store;

namespace Enigma.Db.Embedded.Linq
{
    internal class NullIndexExpression : IIndexExpression
    {

        private readonly static NullIndexExpression Singleton = new NullIndexExpression();
        public static NullIndexExpression Instance { get { return Singleton; } }

        private NullIndexExpression() { }

        public bool IsNull { get { return true; } }

        public IEnumerable<IKey> Resolve(EmbeddedEnigmaService service)
        {
            return null;
        }
    }
}