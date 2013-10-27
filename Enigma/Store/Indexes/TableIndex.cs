using System.Collections.Generic;
using Enigma.IO;
using Enigma.Modelling;
using Enigma.Store.Keys;

namespace Enigma.Store.Indexes
{
    public class TableIndex<T> : ITableIndex
    {
        private readonly IndexStorage<T> _storage;
        private readonly IIndexAlgorithm<T> _indexAlgorithm;

        public TableIndex(IndexStorage<T> storage, IIndexAlgorithm<T> indexAlgorithm)
        {
            _storage = storage;
            _indexAlgorithm = indexAlgorithm;
            _storage.CommitTo(indexAlgorithm);
        }

        public IIndexStorage Storage { get { return _storage; } }
        public IIndexAlgorithm IndexAlgorithm { get { return _indexAlgorithm; } }

        public void CommitModifications()
        {
            if (!_storage.IsModified) return;
            _storage.CommitTo(_indexAlgorithm);
        }

        public IEnumerable<IKey> Match(CompareOperation operation, object value)
        {
            var indexAlgorithm = _indexAlgorithm as IComparableIndexAlgorithm<T>;
            if (indexAlgorithm == null) return Key.EmptyKeys;
            switch (operation)
            {
                case CompareOperation.Equal:
                    return indexAlgorithm.Equal((T)value);
                case CompareOperation.NotEqual:
                    return indexAlgorithm.NotEqual((T)value);
                case CompareOperation.GreaterThan:
                    return indexAlgorithm.GreaterThan((T)value);
                case CompareOperation.GreaterThanOrEqual:
                    return indexAlgorithm.GreaterThanOrEqual((T)value);
                case CompareOperation.LessThan:
                    return indexAlgorithm.LessThan((T)value);
                case CompareOperation.LessThanOrEqual:
                    return indexAlgorithm.LessThanOrEqual((T)value);
                case CompareOperation.Contains:
                    return indexAlgorithm.Contains((IEnumerable<T>)value);
                default:
                    throw new System.InvalidOperationException("Comparable index does not have the operation " + operation.ToString());
            }
        }

    }
}