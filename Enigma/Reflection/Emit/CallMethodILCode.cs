using System;
using System.Reflection;

namespace Enigma.Reflection.Emit
{
    public class CallMethodILCode : IILCode
    {
        private readonly IVariable _instance;
        private readonly MethodInfo _method;
        private readonly Parameter[] _parameters;

        public CallMethodILCode(MethodInfo method, params Parameter[] parameters) : this(null, method, parameters)
        {
        }

        public CallMethodILCode(IVariable instance, MethodInfo method, params Parameter[] parameters)
        {
            if (instance == null && !method.IsStatic)
                throw new ArgumentException("Instance must be provided for instance methods");
            if (instance != null && method.IsStatic)
                throw new ArgumentException("Static method may not be invoked with an instance");

            _instance = instance;
            _method = method;
            _parameters = parameters;
        }

        void IILCode.Generate(ILExpressed il)
        {
            if (_instance != null)
                il.LoadVar(_instance);

            foreach (var parameter in _parameters)
                il.Generate(parameter);

            if (_method.IsStatic)
                il.Call(_method);
            else
                il.CallVirt(_method);
        }
    }
}