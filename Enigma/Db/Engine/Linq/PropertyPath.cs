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

        public PropertyPath()
        {
            _properties = new List<PropertyInfo>();
        }

        public void Add(MemberExpression expression)
        {
            var property = expression.Member as PropertyInfo;
            if (property == null) throw new ArgumentException("Member access is not a property");
            _properties.Add(property);
        }

        public string GetPath()
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
