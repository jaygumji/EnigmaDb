using System;
using System.Reflection;

namespace Enigma.Reflection.Emit
{
    public class ILStaticMethod
    {
        private readonly ILExpressed _il;
        private readonly MethodInfo _method;

        public ILStaticMethod(ILExpressed il, MethodInfo method)
        {
            if (!method.IsStatic)
                throw new ArgumentException("Static method is required");

            _il = il;
            _method = method;
        }

        public void Invoke(params ILCodeParameter[] parameters)
        {
            _il.Snippets.InvokeMethod(_method, parameters);
        }

        public ILCodeParameter AsParameter(params ILCodeParameter[] parameters)
        {
            return new CallMethodILCode(_method, parameters);
        }

    }
}