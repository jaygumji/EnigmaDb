using System;
using System.Reflection;

namespace Enigma.Reflection.Emit
{
    public class CallMethodILCode : IILCode
    {
        private readonly ILCodeVariable _instance;
        private readonly MethodInfo _method;
        private readonly ILCodeParameter[] _parameters;

        public CallMethodILCode(MethodInfo method, params ILCodeParameter[] parameters) : this(null, method, parameters)
        {
        }

        public CallMethodILCode(ILCodeVariable instance, MethodInfo method, params ILCodeParameter[] parameters)
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
            if (_instance != null) {
                if (_instance.VariableType.IsValueType)
                    il.Var.LoadAddress(_instance);
                else
                    il.Var.Load(_instance);
            }

            foreach (var parameter in _parameters)
                il.Generate(parameter);

            if (_method.IsStatic || _instance.VariableType.IsValueType)
                il.Call(_method);
            else
                il.CallVirt(_method);
        }
    }
}