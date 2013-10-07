using System;
namespace Enigma.Modelling
{
    public interface IIndex
    {
        string PropertyName { get; }
        Type PropertyType { get; }
    }

    public interface IIndex<T> : IIndex
    {
    }
}
