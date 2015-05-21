using System;

namespace Enigma.Reflection.Emit
{
    public interface IILCode
    {

        void Generate(ILExpressed il);

    }

    public interface IILCodeParameter
    {

        Type ParameterType { get; }

        void Load(ILExpressed il);
        void LoadAddress(ILExpressed il);

    }

}
