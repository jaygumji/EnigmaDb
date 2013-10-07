using System;
using System.Collections.Generic;
using System.Linq;
using Remotion.Linq;
using System.Linq.Expressions;
using Enigma.Modelling;

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
            var objectExpression = new ObjectExpressionBuilder(_model);
            var visitor = new EnigmaQueryModelVisitor(objectExpression);
            visitor.VisitQueryModel(queryModel);
            var criteria = objectExpression.Criteria;
            Expression where = null;

            IEnumerable<T> result;
            if (objectExpression.HasKeys) {
                var entityEngine = _engine.GetEntityEngine<T>();
                var temp = new List<T>();
                foreach (var idValue in objectExpression.Keys) {
                    T entity;
                    if (entityEngine.TryGet(idValue, out entity))
                        temp.Add(entity);
                }
                result = temp;
            }
            else
            {
                result = _engine.GetEntityEngine<T>().Match(criteria);
                objectExpression.TryGetExpression(out where);
            }

            if (criteria.Take > 0 || criteria.Skip > 0 || where != null)
            {
                var queryable = result.AsQueryable();
                if (where != null)
                {
                    var lambda = Expression.Lambda<Func<T, bool>>(where, objectExpression.EntityParameter);
                    var methodCall = Expression.Call(typeof(Queryable), "Where", new Type[] { queryable.ElementType }, queryable.Expression, lambda);
                    queryable = queryable.Provider.CreateQuery<T>(methodCall);
                }

                if (criteria.Skip > 0) queryable = queryable.Skip(criteria.Skip);
                if (criteria.Take > 0) queryable = queryable.Take(criteria.Take);

                result = queryable;
            }

            return result;
        }

        public T ExecuteScalar<T>(QueryModel queryModel)
        {
            return ExecuteCollection<T>(queryModel).Single();
        }

        public T ExecuteSingle<T>(QueryModel queryModel, bool returnDefaultWhenEmpty)
        {
            return returnDefaultWhenEmpty ? ExecuteCollection<T>(queryModel).SingleOrDefault() : ExecuteCollection<T>(queryModel).Single();
        }
    }
}
