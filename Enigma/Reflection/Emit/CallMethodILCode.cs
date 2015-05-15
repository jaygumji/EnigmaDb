using System;
using System.Reflection;

namespace Enigma.Reflection.Emit
{
    public class CallMethodILCode : IILCode
    {
        private readonly ILCodeVariable _instance;
        private readonly MethodInfo _method;
        private readonly ILCodeParameter[] _parameters;
        private readonly ParameterInfo[] _methodParameters;

        public CallMethodILCode(MethodInfo method, params ILCodeParameter[] parameters) : this(null, method, parameters)
        {
        }

        public CallMethodILCode(ILCodeVariable instance, MethodInfo method, params ILCodeParameter[] parameters)
        {
            if (instance == null && !method.IsStatic)
                throw new ArgumentException("Instance must be provided for instance methods");
            if (instance != null && method.IsStatic)
                throw new ArgumentException("Static method may not be invoked with an instance");

            _methodParameters = method.GetParameters();

            if (_methodParameters.Length < parameters.Length)
                throw new ArgumentException("The parameter length supplied is greater than the method supports");

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

            for (var i = 0; i < _parameters.Length; i++) {
                var parameter = (IILCodeParameter) _parameters[i];
                var methodParameter = _methodParameters[i];

                if (methodParameter.IsOut)
                    parameter.LoadAddress(il);
                else
                    parameter.Load(il);
            }

            if (_method.IsStatic || _instance.VariableType.IsValueType)
                il.Call(_method);
            else
                il.CallVirt(_method);
        }
    }
}