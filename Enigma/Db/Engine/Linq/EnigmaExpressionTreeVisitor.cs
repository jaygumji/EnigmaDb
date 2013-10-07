using System;
using System.Linq.Expressions;
using System.Text;
using Remotion.Linq.Clauses.Expressions;
using Remotion.Linq.Clauses.ExpressionTreeVisitors;
using Remotion.Linq.Parsing;
using System.Collections.Generic;
using System.Reflection;
using Enigma.Db.Linq;

namespace Enigma.Db.Linq
{
    public class EnigmaExpressionTreeVisitor : ThrowingExpressionTreeVisitor
    {
        private readonly ObjectExpressionBuilder _objectExpression;

        public EnigmaExpressionTreeVisitor(ObjectExpressionBuilder objectExpression)
        {
            _objectExpression = objectExpression;
        }

        protected override Expression VisitQuerySourceReferenceExpression(QuerySourceReferenceExpression expression)
        {
            var entityType = expression.ReferencedQuerySource.ItemType;
            var entityAlias = expression.ReferencedQuerySource.ItemName;

            _objectExpression.AddParameter(entityType, entityAlias);

            return expression;
        }

        protected override Expression VisitBinaryExpression(BinaryExpression expression)
        {
            VisitExpression(expression.Left);
            VisitExpression(expression.Right);

            _objectExpression.Binary(expression.NodeType);

            return expression;
        }

        protected override Expression VisitMemberExpression(MemberExpression expression)
        {
            VisitExpression(expression.Expression);

            var property = expression.Member as PropertyInfo;
            if (property != null)
                _objectExpression.Property(expression);

            return expression;
        }

        protected override Expression VisitConstantExpression(ConstantExpression expression)
        {
            _objectExpression.Constant(expression);

            return expression;
        }

        protected override Expression VisitUnaryExpression(UnaryExpression expression)
        {
            if (expression.NodeType == ExpressionType.Convert)
            {
                VisitExpression(expression.Operand);
                _objectExpression.Convert(expression);
                return expression;
            }
            return base.VisitUnaryExpression(expression);
        }

        protected override Expression VisitMethodCallExpression(MethodCallExpression expression)
        {
            return base.VisitMethodCallExpression(expression);
        }

        protected override Expression VisitSubQueryExpression(SubQueryExpression expression)
        {
            var queryModelVisitor = new EnigmaQueryModelVisitor(_objectExpression, isSubQuery: true);
            queryModelVisitor.VisitQueryModel(expression.QueryModel);
            return expression;
        }

        protected override Exception CreateUnhandledItemException<T>(T unhandledItem, string visitMethod)
        {
            string itemText = FormatUnhandledItem(unhandledItem);
            var message = string.Format("The expression '{0}' (type: {1}) is not supported by this LINQ provider.", itemText, typeof(T));
            return new NotSupportedException(message);
        }

        private string FormatUnhandledItem<T>(T unhandledItem)
        {
            var itemAsExpression = unhandledItem as Expression;
            return itemAsExpression != null ? FormattingExpressionTreeVisitor.Format(itemAsExpression) : unhandledItem.ToString();
        }

    }
}
