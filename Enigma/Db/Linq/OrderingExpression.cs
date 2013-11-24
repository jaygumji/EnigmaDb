using System.Linq.Expressions;
using Remotion.Linq.Clauses;

namespace Enigma.Db.Linq
{
    public class OrderingExpression
    {
        private readonly PropertyPath _path;
        private readonly Expression _expression;
        private readonly OrderingDirection _direction;

        public OrderingExpression(PropertyPath path, Expression expression, OrderingDirection direction)
        {
            _path = path;
            _expression = expression;
            _direction = direction;
        }

        public PropertyPath Path
        {
            get { return _path; }
        }

        public Expression Expression
        {
            get { return _expression; }
        }

        public OrderingDirection Direction
        {
            get { return _direction; }
        }
    }
}