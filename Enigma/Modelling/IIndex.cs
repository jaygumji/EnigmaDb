using System;
namespace Enigma.Modelling
{
    public interface IIndex
    {
        string PropertyName { get; }
        Type ValueType { get; }
    }

    public interface IIndex<T> : IIndex
    {
    }
}
