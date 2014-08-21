using System;

namespace Enigma.Reflection.Emit
{
    public interface IILCodeVariable
    {
        Type VariableType { get; }

        bool CanGet { get; }
        bool CanGetAddress { get; }
        bool CanSet { get; }

        void Get(ILExpressed il);
        void GetAddress(ILExpressed il);
        void Set(ILExpressed il);
    }
}