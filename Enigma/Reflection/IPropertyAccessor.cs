using System.Collections.Generic;

namespace Enigma.Reflection
{
    public interface IPropertyAccessor
    {
        IEnumerable<object> GetValuesOf(IEnumerable<object> values);
    }
}