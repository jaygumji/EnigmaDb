using System;
using System.Reflection;

namespace Enigma.Reflection.Emit
{
    public class ILInstanceMethod
    {
        private readonly ILExpressed _il;
        private readonly MethodInfo _method;

        public ILInstanceMethod(ILExpressed il, MethodInfo method)
        {
            if (method.IsStatic)
                throw new ArgumentException("Instance method is required");

            _il = il;
            _method = method;
        }

        public void Invoke(ILCodeParameter instance, params ILCodeParameter[] parameters)
        {
            _il.Snippets.InvokeMethod(instance, _method, parameters);
        }

        public ILCodeParameter AsParameter(ILCodeParameter instance, params ILCodeParameter[] parameters)
        {
            return new CallMethodILCode(instance, _method, parameters);
        }

    }
}
