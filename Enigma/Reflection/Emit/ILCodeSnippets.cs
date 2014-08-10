using System;
using System.Reflection;

namespace Enigma.Reflection.Emit
{
    public class ILCodeSnippets
    {
        private readonly ILExpressed _il;

        public ILCodeSnippets(ILExpressed il)
        {
            _il = il;
        }

        public void GetPropertyValue(IVariable instance, PropertyInfo property)
        {
            if (!property.CanRead) throw new InvalidOperationException("Can not get value from a property with no getter");
            var getMethod = property.GetGetMethod();
            InvokeMethod(instance, getMethod);
        }

        public void InvokeMethod(IVariable instance, MethodInfo method, params Parameter[] parameters)
        {
            _il.Generate(new CallMethodILCode(instance, method, parameters));
        }

        public void InvokeMethod(MethodInfo method, params Parameter[] parameters)
        {
            _il.Generate(new CallMethodILCode(method, parameters));
        }

        public void AsNullable(Type type)
        {
            var container = _il.TypeCache.Extend(type.AsNullable())
                .Container.AsNullable();

            _il.Construct(container.Constructor);
        }

    }
}