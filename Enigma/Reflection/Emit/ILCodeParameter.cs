using System;
using System.Reflection.Emit;

namespace Enigma.Reflection.Emit
{
    public class ILCodeParameter : IILCode
    {

        public static readonly ILCodeParameter This = new ILCodeParameter(il => il.LoadThis());
        public static readonly ILCodeParameter Null = new ILCodeParameter(il => il.LoadNull());

        private readonly Action<ILExpressed> _valueLoader;

        private ILCodeParameter(Action<ILExpressed> valueLoader)
        {
            _valueLoader = valueLoader;
        }

        void IILCode.Generate(ILExpressed il)
        {
            _valueLoader.Invoke(il);
        }

        public static ILCodeParameter Of(LocalBuilder local)
        {
            return new ILCodeParameter(il => il.Var.Load(local));
        }

        public static ILCodeParameter Of(ILCodeVariable variable)
        {
            return new ILCodeParameter(il => il.Var.Load(variable));
        }

        public static ILCodeParameter Of(IILCode code)
        {
            return new ILCodeParameter(il => il.Generate(code));
        }

        public static ILCodeParameter Of(string value)
        {
            return new ILCodeParameter(il => il.LoadValue(value));
        }

        public static ILCodeParameter Of(int value)
        {
            return new ILCodeParameter(il => il.LoadValue(value));
        }

        public static ILCodeParameter Of(uint value)
        {
            return new ILCodeParameter(il => il.LoadValue(value));
        }

        public static implicit operator ILCodeParameter(LocalBuilder local)
        {
            return Of(local);
        }

        public static implicit operator ILCodeParameter(ILCodeVariable variable)
        {
            return Of(variable);
        }

        public static implicit operator ILCodeParameter(CallMethodILCode callMethod)
        {
            return Of(callMethod);
        }

        public static implicit operator ILCodeParameter(string value)
        {
            return Of(value);
        }

        public static implicit operator ILCodeParameter(int value)
        {
            return Of(value);
        }

        public static implicit operator ILCodeParameter(uint value)
        {
            return Of(value);
        }
    }
}