using System.Collections.Generic;

namespace Enigma.Store.Indexes
{
    public interface IComparableIndex : IIndex
    {
        IEnumerable<IKey> Equal(byte[] value);
        IEnumerable<IKey> NotEqual(byte[] value);
        IEnumerable<IKey> GreaterThan(byte[] value);
        IEnumerable<IKey> GreaterThanOrEqual(byte[] value);
        IEnumerable<IKey> LessThan(byte[] value);
        IEnumerable<IKey> LessThanOrEqual(byte[] value);
        IEnumerable<IKey> Contains(IEnumerable<byte[]> values);
    }

    public interface IComparableIndex<T> : IComparableIndex
    {
        IEnumerable<IKey> Equal(T value);
        IEnumerable<IKey> NotEqual(T value);
        IEnumerable<IKey> GreaterThan(T value);
        IEnumerable<IKey> GreaterThanOrEqual(T value);
        IEnumerable<IKey> LessThan(T value);
        IEnumerable<IKey> LessThanOrEqual(T value);
        IEnumerable<IKey> Contains(IEnumerable<T> values);
    }
}
