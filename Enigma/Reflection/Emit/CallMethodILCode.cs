using System;
using System.Reflection;

namespace Enigma.Reflection.Emit
{
    public class CallMethodILCode : IILCode
    {
        private readonly IILCodeParameter _instance;
        private readonly MethodInfo _method;
        private readonly ILCodeParameter[] _parameters;
        private readonly ParameterInfo[] _methodParameters;

        public CallMethodILCode(MethodInfo method, params ILCodeParameter[] parameters) : this(null, method, parameters)
        {
        }

        public CallMethodILCode(ILCodeParameter instance, MethodInfo method, params ILCodeParameter[] parameters)
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
            var instanceIsValueType = _instance != null && _instance.ParameterType != null && _instance.ParameterType.IsValueType;
            if (_instance != null) {
                if (instanceIsValueType)
                    _instance.LoadAddress(il);
                else
                    _instance.Load(il);
            }

            for (var i = 0; i < _parameters.Length; i++) {
                var parameter = (IILCodeParameter) _parameters[i] ?? ILCodeParameter.Null;

                var methodParameter = _methodParameters[i];

                if (methodParameter.IsOut)
                    parameter.LoadAddress(il);
                else
                    parameter.Load(il);
            }

            if (_method.IsStatic || instanceIsValueType)
                il.Call(_method);
            else
                il.CallVirt(_method);
        }
    }
}