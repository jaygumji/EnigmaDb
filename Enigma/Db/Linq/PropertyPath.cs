using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace Enigma.Db.Linq
{
    public class PropertyPath
    {
        private readonly List<PropertyInfo> _properties;
        private Type _type;
        private PropertyInfo _end;

        public PropertyPath()
        {
            _properties = new List<PropertyInfo>();
        }

        public bool IsEmpty { get { return _properties.Count == 0; } }
        public Type Type { get { return _type; } }
        public PropertyInfo End { get { return _end; } }

        public void Add(MemberExpression expression)
        {
            var property = expression.Member as PropertyInfo;
            if (property == null) throw new ArgumentException("Member access is not a property");
            _properties.Add(property);

            if (_type == null) _type = property.DeclaringType;
            _end = property;
        }

        public string GetUniqueName()
        {
            return string.Join(".", _properties.Select(p => p.Name));
        }

        public Expression GetExpression(IEnumerable<ParameterExpression> parameters)
        {
            var firstProperty = _properties.First();
            var parameter = parameters.First(p => p.Type == firstProperty.DeclaringType);
            var expression = Expression.Property(parameter, _properties[0]);
            for (var index = 1; index < _properties.Count; index++)
                expression = Expression.Property(expression, _properties[index]);

            return expression;
        }

    }
}
