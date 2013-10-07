using System.Linq.Expressions;

namespace Enigma.Store.Indexes
{
    public enum CompareOperation
    {
        Equal,
        NotEqual,
        GreaterThan,
        GreaterThanOrEqual,
        LessThan,
        LessThanOrEqual,
        Contains
    }

    public static class CompareOperations
    {
        public static bool TryGet(ExpressionType expressionType, out CompareOperation compareOperation)
        {
            switch (expressionType) {
                case ExpressionType.Equal:
                    compareOperation = CompareOperation.Equal;
                    return true;
                case ExpressionType.NotEqual:
                    compareOperation = CompareOperation.NotEqual;
                    return true;
                case ExpressionType.GreaterThan:
                    compareOperation = CompareOperation.GreaterThan;
                    return true;
                case ExpressionType.GreaterThanOrEqual:
                    compareOperation = CompareOperation.GreaterThanOrEqual;
                    return true;
                case ExpressionType.LessThan:
                    compareOperation = CompareOperation.LessThan;
                    return true;
                case ExpressionType.LessThanOrEqual:
                    compareOperation = CompareOperation.LessThanOrEqual;
                    return true;
            }
            compareOperation = CompareOperation.Equal;
            return false;
        }
    }
}
