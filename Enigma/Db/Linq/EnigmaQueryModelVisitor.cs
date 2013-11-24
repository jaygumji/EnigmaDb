using System;
using System.Linq.Expressions;
using Remotion.Linq;
using Remotion.Linq.Clauses;
using Remotion.Linq.Clauses.ResultOperators;
using System.Linq;
using System.Collections.Generic;
using Enigma.Modelling;

namespace Enigma.Db.Linq
{

    public class EnigmaQueryModelVisitor : QueryModelVisitorBase
    {

        private readonly ObjectExpressionBuilder _objectExpression;
        private readonly bool _isSubQuery;

        public EnigmaQueryModelVisitor(ObjectExpressionBuilder objectExpression)
        {
            _objectExpression = objectExpression;
        }

        public EnigmaQueryModelVisitor(ObjectExpressionBuilder objectExpression, bool isSubQuery) : this(objectExpression)
        {
            _isSubQuery = isSubQuery;
        }

        public override void VisitQueryModel(QueryModel queryModel)
        {
            base.VisitQueryModel(queryModel);
        }

        public override void VisitResultOperator(ResultOperatorBase resultOperator, QueryModel queryModel, int index)
        {
            base.VisitResultOperator(resultOperator, queryModel, index);

            if (resultOperator is FirstResultOperator) {
                _objectExpression.Executor.Take(1);
                return;
            }

            var takeResultOperator = resultOperator as TakeResultOperator;
            if (takeResultOperator != null) {
                var countConstantExpression = takeResultOperator.Count as ConstantExpression;
                if (countConstantExpression != null) {
                    _objectExpression.Take = (int)countConstantExpression.Value;
                }

                return;
            }

            var skipResultOperator = resultOperator as SkipResultOperator;
            if (skipResultOperator != null) {
                var countConstantExpression = skipResultOperator.Count as ConstantExpression;
                if (countConstantExpression != null)
                    _objectExpression.Skip = (int)countConstantExpression.Value;

                return;
            }

            var contains = resultOperator as ContainsResultOperator;
            if (contains != null)
            {
                var visitor = new EnigmaExpressionTreeVisitor(_objectExpression);
                visitor.VisitExpression(contains.Item);
                _objectExpression.Contains();
                return;
            }

            var any = resultOperator as AnyResultOperator;
            if (any != null)
            {
                _objectExpression.Any();
                return;
            }
        }

        public override void VisitMainFromClause(MainFromClause fromClause, QueryModel queryModel)
        {
            var visitor = new EnigmaExpressionTreeVisitor(_objectExpression);
            visitor.VisitExpression(fromClause.FromExpression);
            //_objectExpression.AddParameter(fromClause.ItemType, fromClause.ItemName);
        }

        public override void VisitSelectClause(SelectClause selectClause, QueryModel queryModel)
        {
            base.VisitSelectClause(selectClause, queryModel);
        }

        public override void VisitWhereClause(WhereClause whereClause, QueryModel queryModel, int index)
        {
            var predicate = whereClause.Predicate;

            var visitor = new EnigmaExpressionTreeVisitor(_objectExpression);
            visitor.VisitExpression(predicate);

            base.VisitWhereClause(whereClause, queryModel, index);
        }

        public override void VisitOrderByClause(OrderByClause orderByClause, QueryModel queryModel, int index)
        {
            var visitor = new EnigmaExpressionTreeVisitor(_objectExpression);
            foreach (var ordering in orderByClause.Orderings) {
                visitor.VisitExpression(ordering.Expression);
                _objectExpression.OrderBy(ordering.OrderingDirection);
            }

            base.VisitOrderByClause(orderByClause, queryModel, index);
        }

        public override void VisitJoinClause(JoinClause joinClause, QueryModel queryModel, GroupJoinClause groupJoinClause)
        {
            base.VisitJoinClause(joinClause, queryModel, groupJoinClause);
        }

        public override void VisitAdditionalFromClause(AdditionalFromClause fromClause, QueryModel queryModel, int index)
        {
            base.VisitAdditionalFromClause(fromClause, queryModel, index);
        }

        public override void VisitGroupJoinClause(GroupJoinClause groupJoinClause, QueryModel queryModel, int index)
        {
            base.VisitGroupJoinClause(groupJoinClause, queryModel, index);
        }

    }
}
