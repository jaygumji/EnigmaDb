using System.Collections.Generic;
using System.Linq;
using Enigma.Db.Linq;
using Enigma.Store;
using Enigma.Store.Indexes;

namespace Enigma.Db.Embedded.Linq
{
    internal class EmbeddedEnigmaExpressionTreeExecutor : IEnigmaExpressionTreeExecutor
    {
        private readonly EmbeddedEnigmaService _service;
        private readonly Stack<IIndexExpression> _expressions;
        private readonly List<EmbeddedIndexOrdering> _orderings; 
        private int _take;
        private int _skip;

        public EmbeddedEnigmaExpressionTreeExecutor(EmbeddedEnigmaService service)
        {
            _service = service;
            _expressions = new Stack<IIndexExpression>();
            _orderings = new List<EmbeddedIndexOrdering>();
        }

        public void Compare(PropertyPath path, CompareOperation compareOperation, object value)
        {
            _expressions.Push(new CompareIndexExpression(path, compareOperation, value));
        }

        public void And()
        {
            if (_expressions.Count <= 1)
                throw ExpressionTreeParseException.InvalidExpressionCount("And");

            var right = _expressions.Pop();
            var left = _expressions.Pop();

            if (right.IsNull) {
                _expressions.Push(left);
                return;
            }
            if (left.IsNull) {
                _expressions.Push(right);
                return;
            }

            var and = new AndIndexExpression(left, right);
            _expressions.Push(and);
        }

        public void Or()
        {
            var right = _expressions.Pop();
            var left = _expressions.Pop();

            if (right.IsNull || left.IsNull) {
                _expressions.Push(NullIndexExpression.Instance);
                return;
            }

            var and = new AndIndexExpression(left, right);
            _expressions.Push(and);
        }

        public void EqualKey(object value)
        {
            _expressions.Push(new StaticIndexExpression(value));
        }

        public void Unknown()
        {
            _expressions.Push(NullIndexExpression.Instance);
        }

        public void OrderBy(PropertyPath path, OrderingDirection direction)
        {
            _orderings.Add(new EmbeddedIndexOrdering(path, direction));
        }

        public void Take(int count)
        {
            _take = count;
        }

        public void Skip(int count)
        {
            _skip = count;
        }

        private IEnumerable<IKey> ResolveKeys()
        {
            if (_expressions.Count > 1)
                throw new ExpressionTreeParseException("Final expression must be exactly one");

            if (_expressions.Count == 0)
                return null;

            var expression = _expressions.Peek();
            if (expression.IsNull)
                return null;

            return expression.Resolve(_service);
        }

        public bool TryResolve(out IEnumerable<IKey> keys)
        {
            keys = ResolveKeys();

            if (_orderings.Count > 0) {
                var directions = _orderings.Select(o => o.Direction).ToArray();
                OrderedKey[] orderedKeys;
                if (keys == null) {
                    var firstOrdering = _orderings[0];
                    var table = _service.Table(firstOrdering.Path.Type.Name);
                    var entries = table.Storage.TableOfContent.Entries;
                    orderedKeys = entries.Select(entry => new OrderedKey(entry.Key, directions)).ToArray();
                }
                else
                    orderedKeys = keys.Select(key => new OrderedKey(key, directions)).ToArray();

                foreach (var ordering in _orderings) {
                    var table = _service.Table(ordering.Path.Type.Name);
                    table.Indexes.ApplyOrderingValues(ordering.Path.GetUniqueName(), orderedKeys);
                }
                keys = orderedKeys.OrderBy(k => k);
            }

            if (keys == null) return false;

            if (_skip > 0)
                keys = keys.Skip(_skip);
            if (_take > 0)
                keys = keys.Take(_take);

            return true;
        }
    }
}
