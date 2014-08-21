using System;
using System.Reflection.Emit;

namespace Enigma.Reflection.Emit
{
    public abstract class ILCodeVariable : IILCodeVariable
    {

        private readonly Type _variableType;

        protected ILCodeVariable(Type variableType)
        {
            _variableType = variableType;
        }

        public Type VariableType { get { return _variableType; } }
        
        public bool CanGet { get { return true; } }
        public bool CanGetAddress { get { return true; } }
        public virtual bool CanSet { get { return true; } }

        void IILCodeVariable.Get(ILExpressed il)
        {
            OnGet(il);
        }

        void IILCodeVariable.GetAddress(ILExpressed il)
        {
            OnGetAddress(il);
        }

        void IILCodeVariable.Set(ILExpressed il)
        {
            OnSet(il);
        }

        protected virtual void OnGet(ILExpressed il)
        {
            throw new InvalidOperationException("Not possible to use this variable in a get operation, " + GetType().Name);
        }

        protected virtual void OnGetAddress(ILExpressed il)
        {
            throw new InvalidOperationException("Not possible to use this variable in a get address operation, " + GetType().Name);
        }

        protected virtual void OnSet(ILExpressed il)
        {
            throw new InvalidOperationException("Not possible to use this variable in a set operation, " + GetType().Name);
        }

        public static implicit operator ILCodeVariable(LocalBuilder local)
        {
            return new LocalILCodeVariable(local);
        }

    }
}