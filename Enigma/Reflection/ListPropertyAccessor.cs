using System.Collections;
using System.Collections.Generic;
using System.Reflection;

namespace Enigma.Reflection
{
    class ListPropertyAccessor : IPropertyAccessor
    {
        private readonly PropertyInfo _propertyInfo;

        public ListPropertyAccessor(PropertyInfo propertyInfo)
        {
            _propertyInfo = propertyInfo;
        }

        public IEnumerable<object> GetValuesOf(IEnumerable<object> values)
        {
            var next = new List<object>();

            foreach (var value in values) {
                var list = (IList) _propertyInfo.GetValue(value);
                foreach (var element in list) next.Add(element);
            }

            return next;
        }
    }
}