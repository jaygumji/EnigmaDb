using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Enigma.Modelling;
using Remotion.Linq;
using Remotion.Linq.Clauses;

namespace Enigma.Db.Linq
{
    public class EnigmaQueryExecutor : IQueryExecutor
    {
        private readonly IEnigmaEngine _engine;
        private readonly Model _model;

        public EnigmaQueryExecutor(IEnigmaEngine engine, Model model)
        {
            _engine = engine;
            _model = model;
        }

        public IEnumerable<T> ExecuteCollection<T>(QueryModel queryModel)
        {
            var executor = _engine.CreateExecutor();
            var objectExpression = new ObjectExpressionBuilder(_model, executor);
            var visitor = new EnigmaQueryModelVisitor(objectExpression);
            visitor.VisitQueryModel(queryModel);
            var entityEngine = _engine.GetEntityEngine<T>();
            IEnumerable<T> result;
            var isResolved = entityEngine.TryResolve(executor, out result);
            Expression where;
            var hasFilter = objectExpression.TryGetExpression(out where);
            if (!isResolved) {
                if (!hasFilter && !objectExpression.Orderings.Any()) {
                    result = entityEngine.Range(objectExpression.Skip, objectExpression.Take);
                }
                else {
                    result = entityEngine.All();
                }
            }
            var queryable = result.AsQueryable();
            if (hasFilter) {
                var whereCall = Expression.Call(typeof (Queryable), "Where", new[] {
                    queryable.ElementType
                }, new[] {
                    queryable.Expression,
                    Expression.Lambda<Func<T, bool>>(where, new[] { objectExpression.EntityParameter })
                });
                queryable = queryable.Provider.CreateQuery<T>(whereCall);
            }
            foreach (var ordering in objectExpression.Orderings) {
                var methodName = (ordering.Direction == OrderingDirection.Asc) ? "OrderBy" : "OrderByDescending";
                var orderByCall = Expression.Call(typeof (Queryable), methodName, new[] {
                    queryable.ElementType,
                    ordering.Path.End.PropertyType
                }, new[] {
                    queryable.Expression,
                    Expression.Lambda(ordering.Expression, new[] { objectExpression.EntityParameter })
                });
                queryable = queryable.Provider.CreateQuery<T>(orderByCall);
            }
            if (!isResolved && objectExpression.Skip > 0) {
                queryable = queryable.Skip(objectExpression.Skip);
            }
            if (objectExpression.Take > 0) {
                queryable = queryable.Take(objectExpression.Take);
            }
            result = queryable;
            return result;
        }

        public T ExecuteScalar<T>(QueryModel queryModel)
        {
            return ExecuteCollection<T>(queryModel).Single<T>();
        }

        public T ExecuteSingle<T>(QueryModel queryModel, bool returnDefaultWhenEmpty)
        {
            return returnDefaultWhenEmpty
                ? ExecuteCollection<T>(queryModel).SingleOrDefault<T>()
                : ExecuteCollection<T>(queryModel).Single<T>();
        }
    }
}