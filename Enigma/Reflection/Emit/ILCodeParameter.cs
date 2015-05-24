using System;
using System.Reflection.Emit;

namespace Enigma.Reflection.Emit
{
    public abstract class ILCodeParameter : IILCodeParameter
    {

        public static readonly ILCodeParameter This = new ILCodeParameterDelegatable(null, il => il.LoadThis());
        public static readonly ILCodeParameter Null = new ILCodeParameterDelegatable(null, il => il.LoadNull());

        public abstract Type ParameterType { get; }

        void IILCodeParameter.Load(ILExpressed il)
        {
            Load(il);
        }

        void IILCodeParameter.LoadAddress(ILExpressed il)
        {
            LoadAddress(il);
        }

        protected abstract void Load(ILExpressed il);

        protected virtual void LoadAddress(ILExpressed il)
        {
            throw new NotSupportedException("This parameter does not support address loading, " + GetType().Name);
        }

        public static ILCodeParameter Of(LocalBuilder local)
        {
            return new ILCodeParameterDelegatable(local.LocalType, il => il.Var.Load(local), il => il.Var.LoadAddress(local));
        }

        public static ILCodeParameter Of(ILCodeVariable variable)
        {
            return new ILCodeParameterDelegatable(variable.VariableType, il => il.Var.Load(variable), il => il.Var.LoadAddress(variable));
        }

        public static ILCodeParameter Of(CallMethodILCode code)
        {
            return new ILCodeParameterDelegatable(code.ReturnType, il => il.Generate(code));
        }

        public static ILCodeParameter Of(IILCode code)
        {
            return new ILCodeParameterDelegatable(null, il => il.Generate(code));
        }

        public static ILCodeParameter Of(string value)
        {
            return new ILCodeParameterDelegatable(typeof(string), il => il.LoadValue(value));
        }

        public static ILCodeParameter Of(int value)
        {
            return new ILCodeParameterDelegatable(typeof(int), il => il.LoadValue(value));
        }

        public static ILCodeParameter Of(uint value)
        {
            return new ILCodeParameterDelegatable(typeof(uint), il => il.LoadValue(value));
        }

        public static ILCodeParameter Of(Type type)
        {
            return new ILCodeParameterDelegatable(typeof(Type), il => il.LoadRef(type));
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