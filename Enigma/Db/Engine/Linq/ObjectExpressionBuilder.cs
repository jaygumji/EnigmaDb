using Enigma.Modelling;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using Enigma.Store;
using Enigma.Store.Indexes;

namespace Enigma.Db.Linq
{
    public class ObjectExpressionBuilder
    {

        private static readonly MethodInfo AnyMethodWithInnerDelegate = typeof(Enumerable).GetMethods().First(m => m.Name == "Any" && m.GetParameters().Count() == 2);

        private readonly Stack<ParameterExpression> _parameters;
        private readonly Stack<Expression> _expressions;
        private PropertyPath _propertyPath;
        private readonly Model _model;
        private IEntityMap _entityMap;
        private ParameterExpression _entityParameter;
        private readonly EnigmaCriteria _criteria;
        private readonly List<object> _keys; 

        public ObjectExpressionBuilder(Model model)
        {
            _parameters = new Stack<ParameterExpression>();
            _expressions = new Stack<Expression>();
            _model = model;
            _criteria = new EnigmaCriteria();
            _keys = new List<object>();
        }

        public Model Model { get { return _model; } }
        public IEnumerable<ParameterExpression> Parameters { get { return _parameters; } }
        public ParameterExpression EntityParameter { get { return _entityParameter; } }
        public ParameterExpression LastParameter { get { return _parameters.Peek(); } }
        public EnigmaCriteria Criteria { get { return _criteria; }}
        public IEnumerable<object> Keys { get { return _keys; } }
        public bool HasKeys { get { return _keys.Count > 0; } }

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
            if (_propertyPath != null)
            {
                var path = _propertyPath.GetPath();
                _propertyPath = null;
                if (_entityMap != null) {
                    if (expressionType == ExpressionType.Equal
                        && string.Equals(path, _entityMap.KeyName, StringComparison.InvariantCulture)) {

                        _keys.Add(((ConstantExpression)right).Value);
                    }
                    CompareOperation compareOperation;
                    if (CompareOperations.TryGet(expressionType, out compareOperation)) {
                        var index = _entityMap.Indexes.FirstOrDefault(i => string.Equals(i.PropertyName, path, StringComparison.InvariantCulture));
                        if (index != null) {
                            _criteria.IndexOperations.Add(new EnigmaIndexOperation {
                                Operation = compareOperation,
                                UniqueName = path,
                                Value = ((ConstantExpression)right).Value
                            });
                        }
                    }
                }
            }

            var expression = Expression.MakeBinary(expressionType, left, right);
            _expressions.Push(expression);
        }

        public void Convert(UnaryExpression expression)
        {
            Expression operand;
            if (_propertyPath != null)
            {
                operand = _propertyPath.GetExpression(_parameters);
                //_propertyPath = null;
            }
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
                return true;
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
    }
}
