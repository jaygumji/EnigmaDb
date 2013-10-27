using System;
using Enigma.IO;
using Enigma.Store.Binary;

namespace Enigma.Store.Indexes
{
    public class TableIndexFactory
    {
        private static readonly Type FactoryType = typeof(TableIndexFactory);
        private const string AsComparableMethod = "AsComparableOf";

        public ITableIndex AsComparable(IStreamProvider streamProvider, IndexConfiguration details)
        {
            var indexType = details.Type.IsEnum
                ? Enum.GetUnderlyingType(details.Type)
                : details.Type;

            var method = FactoryType.GetMethod(AsComparableMethod).MakeGenericMethod(indexType);
            return (ITableIndex) method.Invoke(this, new object[] {streamProvider, details});
        }

        public ITableIndex AsComparableOf<T>(IStreamProvider streamProvider, IndexConfiguration details)
            where T : IComparable<T>
        {
            var store = new BinaryStore(streamProvider);
            var indexStorage = new IndexStorage<T>(store, details);
            indexStorage.Initialize();
            return new TableIndex<T>(indexStorage, new ComparableIndexAlgorithm<T>());
        }

    }
}