using System;
using System.Reflection.Emit;

namespace Enigma.Reflection.Emit
{
    public class ILCodeParameter : IILCodeParameter
    {

        public static readonly ILCodeParameter This = new ILCodeParameter(il => il.LoadThis());
        public static readonly ILCodeParameter Null = new ILCodeParameter(il => il.LoadNull());

        private readonly ILGenerationHandler _valueLoader;
        private readonly ILGenerationHandler _valueAddressLoader;

        private ILCodeParameter(ILGenerationHandler valueLoader) : this(valueLoader, null)
        {
        }

        private ILCodeParameter(ILGenerationHandler valueLoader, ILGenerationHandler valueAddressLoader)
        {
            _valueLoader = valueLoader;
            _valueAddressLoader = valueAddressLoader;
        }

        void IILCodeParameter.Load(ILExpressed il)
        {
            _valueLoader.Invoke(il);
        }

        void IILCodeParameter.LoadAddress(ILExpressed il)
        {
            if (_valueAddressLoader == null)
                throw new NotSupportedException("This parameter does not support address loading");

            _valueAddressLoader.Invoke(il);
        }

        public static ILCodeParameter Of(LocalBuilder local)
        {
            return new ILCodeParameter(il => il.Var.Load(local), il => il.Var.LoadAddress(local));
        }

        public static ILCodeParameter Of(ILCodeVariable variable)
        {
            return new ILCodeParameter(il => il.Var.Load(variable), il => il.Var.LoadAddress(variable));
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

        public static ILCodeParameter Of(Type type)
        {
            return new ILCodeParameter(il => il.LoadRef(type));
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

        public static implicit operator ILCodeParameter(Type type)
        {
            return Of(type);
        }
    }
}