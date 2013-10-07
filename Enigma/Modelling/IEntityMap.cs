using System;
using System.Collections.Generic;
namespace Enigma.Modelling
{
    public interface IEntityMap
    {
        Type EntityType { get; }
        string Name { get; }
        string KeyName { get; }
        IEnumerable<IPropertyMap> Properties { get; }
        IEnumerable<IIndex> Indexes { get; }
    }

    public interface IEntityMap<T> : IEntityMap
    {
    }
}
