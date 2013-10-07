using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Enigma.Store.Keys;
using Enigma.Store;
using Enigma.Store.Binary;

namespace Enigma.Test.Store
{
    [TestClass]
    public class EntryWriterTests
    {

        private static readonly Entry TestEntry = new Entry { Key = new Int32Key(1), ValueOffset = 77, ValueLength = 200 };

        [TestMethod]
        public void WriteTest()
        {
            var store = new FakeBinaryStore();
            var entryWriter = new EntryWriter(store);
            entryWriter.Write(TestEntry);
            Assert.AreEqual(22, store.Length);
        }

        [TestMethod]
        public void WriteRemoveTest()
        {
            var entryData = EntryBinaryConverter.Instance.Convert(TestEntry);
            var store = new FakeBinaryStore(entryData);
            var entryWriter = new EntryWriter(store);

            var data = store.Stream.ToArray();
            Assert.AreEqual(1, data[EntryBinaryConverter.IsActiveValueOffset]);

            entryWriter.WriteRemove(TestEntry);
            Assert.AreEqual(22, store.Length);

            data = store.Stream.ToArray();
            Assert.AreEqual(0, data[EntryBinaryConverter.IsActiveValueOffset]);
        }
    }
}
