using System;
using System.Linq.Expressions;
using System.Reflection;
namespace Enigma.Expressions
{
    public static class PropertySelector
    {
        public static PropertyInfo GetProperty<T, TProperty>(Expression<Func<T, TProperty>> propertyExpression)
        {
            var memberExpression = propertyExpression.Body as MemberExpression;
            if (memberExpression == null)
                throw new ArgumentException("Expression does not specify a property");

            var propertyInfo = memberExpression.Member as PropertyInfo;
            if (propertyInfo == null)
                throw new ArgumentException("Expression does not specify a property");

            return propertyInfo;
        }
    }
}
