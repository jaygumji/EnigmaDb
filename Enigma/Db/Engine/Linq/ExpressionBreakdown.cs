using System.Linq.Expressions;
using System.Reflection;
namespace Enigma.Db.Linq
{
    public abstract class ExpressionBreakdown
    {

        protected ExpressionBreakdown(ExpressionBreakdownType type)
        {
            Type = type;
        }

        public ExpressionBreakdownType Type { get; private set; }

        public abstract Expression Reconstruct(ParameterExpression parameter);

    }
}
