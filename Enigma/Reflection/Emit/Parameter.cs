using System;
using System.Reflection.Emit;

namespace Enigma.Reflection.Emit
{
    public class Parameter : IILCode
    {

        public static readonly Parameter This = new Parameter(il => il.LoadThis());

        private readonly Action<ILExpressed> _valueLoader;

        private Parameter(Action<ILExpressed> valueLoader)
        {
            _valueLoader = valueLoader;
        }

        void IILCode.Generate(ILExpressed il)
        {
            _valueLoader.Invoke(il);
        }

        public static Parameter Of(LocalBuilder local)
        {
            return new Parameter(il => il.LoadLocal(local));
        }

        public static Parameter Of(IVariable variable)
        {
            return new Parameter(il => il.LoadVar(variable));
        }

        public static Parameter Of(IILCode code)
        {
            return new Parameter(il => il.Generate(code));
        }

        public static implicit operator Parameter(LocalBuilder local)
        {
            return Of(local);
        }

        public static implicit operator Parameter(Variable variable)
        {
            return Of(variable);
        }

        public static implicit operator Parameter(CallMethodILCode callMethod)
        {
            return Of(callMethod);
        }

        public static implicit operator Parameter(string value)
        {
            return Of(Constant.Of(value));
        }

        public static implicit operator Parameter(int value)
        {
            return Of(Constant.Of(value));
        }

        public static implicit operator Parameter(uint value)
        {
            return Of(Constant.Of(value));
        }
    }
}