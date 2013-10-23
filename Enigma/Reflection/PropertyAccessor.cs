using System.Collections.Generic;
using System.Reflection;

namespace Enigma.Reflection
{
    class PropertyAccessor : IPropertyAccessor
    {
        private readonly PropertyInfo _propertyInfo;

        public PropertyAccessor(PropertyInfo propertyInfo)
        {
            _propertyInfo = propertyInfo;
        }

        public IEnumerable<object> GetValuesOf(IEnumerable<object> values)
        {
            var next = new List<object>();

            foreach (var value in values) {
                var nextValue = _propertyInfo.GetValue(value);
                next.Add(nextValue);
            }

            return next;
        }
    }
}