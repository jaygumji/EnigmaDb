using Enigma.Modelling;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using Enigma.Store;
using Enigma.Store.Indexes;
using Remotion.Linq.Clauses;
using OrderingDirection = Remotion.Linq.Clauses.OrderingDirection;

namespace Enigma.Db.Linq
{
    public class ObjectExpressionBuilder
    {

        private static readonly MethodInfo AnyMethodWithInnerDelegate = typeof(Enumerable).GetMethods().First(m => m.Name == "Any" && m.GetParameters().Count() == 2);

        private readonly Stack<ParameterExpression> _parameters;
        private readonly Stack<Expression> _expressions;
        private readonly List<OrderingExpression> _orderings;
        private PropertyPath _propertyPath;
        private readonly Model _model;
        private readonly IEnigmaExpressionTreeExecutor _executor;
        private IEntityMap _entityMap;
        private ParameterExpression _entityParameter;
        private int _skip;
        private int _take;

        public ObjectExpressionBuilder(Model model, IEnigmaExpressionTreeExecutor executor)
        {
            _parameters = new Stack<ParameterExpression>();
            _expressions = new Stack<Expression>();
            _orderings = new List<OrderingExpression>();
            _model = model;
            _executor = executor;
        }

        public Model Model { get { return _model; } }
        public IEnumerable<ParameterExpression> Parameters { get { return _parameters; } }
        public ParameterExpression EntityParameter { get { return _entityParameter; } }
        public ParameterExpression LastParameter { get { return _parameters.Peek(); } }
        public IEnigmaExpressionTreeExecutor Executor { get { return _executor; } }
        public IEnumerable<OrderingExpression> Orderings { get { return _orderings; } } 
        public int Skip { get { return _skip; } set { _skip = value; _executor.Skip(value); } }
        public int Take { get { return _take; } set { _take = value; _executor.Take(value); } }

        public void AddParameter(Type entityType, string entityAlias)
        {
            var parameterExpression = _parameters.FirstOrDefault(p => p.Name == entityAlias);
            if (parameterExpression != null)
            {
                _expressions.Push(parameterExpression);
                return;
            }

            parameterExpression = Expression.Parameter(entityType, entityAlias);
            _expressions.Push(parameterExpression);
            _parameters.Push(parameterExpression);

            if (_entityParameter == null && !IsValue(entityType))
            {
                _entityParameter = parameterExpression;
                _entityMap = _model.GetEntity(entityType);
            }
        }

        private static bool IsValue(Type type)
        {
            return type.IsValueType || type == typeof(string);
        }

        public void Binary(ExpressionType expressionType)
        {
            var right = _expressions.Pop();
            var left = _expressions.Pop();

            CompareOperation operation;
            if (CompareOperations.TryGet(expressionType, out operation)) {
                if (!TryBinaryAnalyzeCompare(operation, right))
                    _executor.Unknown();
            }
            else if (expressionType == ExpressionType.And || expressionType == ExpressionType.AndAlso) {
                _executor.And();
            }
            else if (expressionType == ExpressionType.Or || expressionType == ExpressionType.OrElse) {
                _executor.Or();
            }

            var expression = Expression.MakeBinary(expressionType, left, right);
            _expressions.Push(expression);
        }

        private bool TryBinaryAnalyzeCompare(CompareOperation operation, Expression right)
        {
            if (_propertyPath == null) return false;

            var path = _propertyPath;
            var pathString = _propertyPath.GetUniqueName();
            _propertyPath = null;

            if (_entityMap == null) return false;

            if (operation == CompareOperation.Equal
                && string.Equals(pathString, _entityMap.KeyName, StringComparison.InvariantCulture)) {
                _executor.EqualKey(((ConstantExpression) right).Value);
                return true;
            }

            var index = _entityMap.Indexes.FirstOrDefault(i => string.Equals(i.UniqueName, pathString, StringComparison.InvariantCulture));
                
            if (index == null) return false;

            _executor.Compare(path, operation, ((ConstantExpression)right).Value);
            return true;
        }

        public void Convert(UnaryExpression expression)
        {
            Expression operand;
            if (_propertyPath != null)
                operand = _propertyPath.GetExpression(_parameters);
            else
                operand = _expressions.Pop();

            _expressions.Push(Expression.Convert(operand, expression.Type));
        }

        public void Property(MemberExpression expression)
        {
            if (_propertyPath == null)
                _propertyPath = new PropertyPath();

            _propertyPath.Add(expression);

            var previous = _expressions.Pop();
            var newExpression = Expression.Property(previous, (PropertyInfo) expression.Member);
            _expressions.Push(newExpression);
        }

        public void Constant(ConstantExpression expression)
        {
            _expressions.Push(expression);
        }

        public bool TryGetExpression(out Expression expression)
        {
            if (_entityParameter != null && _expressions.Count > 0)
            {
                expression = _expressions.Peek();
                // Edge case when no where clause is specified.
                return expression.NodeType != ExpressionType.Constant;
            }

            expression = null;
            return false;
        }

        public void Contains()
        {
            var property = (MemberExpression) _expressions.Pop();
            var constant = (ConstantExpression) _expressions.Pop();
            // property = _propertyPath.GetExpression(_parameters);
            _propertyPath = null;

            var containsMethod = constant.Type.GetMethod("Contains");
            var expression = Expression.Call(constant, containsMethod, property);
            _expressions.Push(expression);
        }

        public void Any()
        {
            var inner = _expressions.Pop();
            var source = _expressions.Pop();

            var enumerableType = SearchForInterface(source, typeof(IEnumerable<>));
            if (enumerableType == null) throw new Exception("Failed to find enumerable type of source");
            var elementType = enumerableType.GetGenericArguments()[0];
            var parameter = Parameters.First(p => p.Type == elementType);

            var anyMethod = AnyMethodWithInnerDelegate.MakeGenericMethod(elementType);
            var lambda = Expression.Lambda(inner, parameter);
            //var methodCall = Expression.Call(typeof(Queryable), "Where", new Type[] { queryable.ElementType }, queryable.Expression, lambda);
            var callExpression = Expression.Call(anyMethod, source, lambda);
            _expressions.Push(callExpression);
        }

        private Type SearchForInterface(Expression expression, Type interfaceType)
        {
            var constant = expression as ConstantExpression;
            if (constant != null)
                return constant.Type.GetInterfaces().FirstOrDefault(i => i.GetGenericTypeDefinition() == interfaceType);

            var member = expression as MemberExpression;
            if (member != null)
            {
                //if (member.Expression != null) return SearchForInterface(member.Expression, interfaceType);

                return ((PropertyInfo)member.Member).PropertyType.GetInterfaces().FirstOrDefault(i => i.GetGenericTypeDefinition() == interfaceType);
            }
            return null;
        }

        public void OrderBy(OrderingDirection orderingDirection)
        {
            var orderByExpression = _expressions.Pop();

            if (_propertyPath == null)
                throw new ExpressionTreeParseException("Order by requires an entity property");

            var path = _propertyPath;

            _orderings.Add(new OrderingExpression(path, orderByExpression, orderingDirection));

            var pathString = _propertyPath.GetUniqueName();
            _propertyPath = null;

            if (_entityMap == null) return;

            var index = _entityMap.Indexes.FirstOrDefault(i => string.Equals(i.UniqueName, pathString, StringComparison.InvariantCulture));

            if (index == null) return;

            _executor.OrderBy(path, OrderingDirections.Get(orderingDirection));
        }
    }
}
