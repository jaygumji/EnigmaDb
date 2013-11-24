using System;
namespace Enigma.Modelling
{
    public interface IIndex
    {
        string UniqueName { get; }
        Type ValueType { get; }
    }

    public interface IIndex<T> : IIndex
    {
    }
}
