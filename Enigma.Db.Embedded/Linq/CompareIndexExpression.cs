using System.Collections.Generic;
using Enigma.Db.Linq;
using Enigma.Store;
using Enigma.Store.Indexes;

namespace Enigma.Db.Embedded.Linq
{
    internal class CompareIndexExpression : IIndexExpression
    {
        private readonly PropertyPath _path;
        private readonly CompareOperation _compareOperation;
        private readonly object _value;

        public CompareIndexExpression(PropertyPath path, CompareOperation compareOperation, object value)
        {
            _path = path;
            _compareOperation = compareOperation;
            _value = value;
        }

        public bool IsNull { get { return false; } }

        public IEnumerable<IKey> Resolve(EmbeddedEnigmaService service)
        {
            var pathString = _path.GetUniqueName();
            var table = service.Table(_path.Type.Name);
            return table.Indexes.Match(pathString, _compareOperation, _value);
        }
    }
}