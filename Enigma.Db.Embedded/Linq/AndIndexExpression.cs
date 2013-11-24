using System.Collections.Generic;
using System.Linq;
using Enigma.Store;

namespace Enigma.Db.Embedded.Linq
{
    internal class AndIndexExpression : IIndexExpression
    {
        private readonly IIndexExpression _left;
        private readonly IIndexExpression _right;

        public AndIndexExpression(IIndexExpression left, IIndexExpression right)
        {
            _left = left;
            _right = right;
        }

        public bool IsNull { get { return false; } }

        public IEnumerable<IKey> Resolve(EmbeddedEnigmaService service)
        {
            return _left.Resolve(service).Intersect(_right.Resolve(service));
        }
    }
}