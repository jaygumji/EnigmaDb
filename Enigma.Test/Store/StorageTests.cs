using System;
using Enigma.Store;
using Enigma.Store.Binary;
using Enigma.Store.Keys;
using Enigma.Store.Memory;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Enigma.Test.Store
{
    [TestClass]
    public class StorageTests
    {

        [TestMethod]
        public void UpdateTest()
        {
            var key = new Int32Key(42);
            var content = new byte[] {1, 2, 3, 4, 5, 6, 7, 8, 9, 10};
            var changedContent = new byte[] {1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15};

            var storage = new CompositeStorage(new MemoryCompositeStorageConfigurator());
            var actual = storage.TryAdd(key, content);
            Assert.AreEqual(true, actual, "Unexpectedly failed to add content");

            actual = storage.TryUpdate(key, changedContent);
            Assert.AreEqual(true, actual, "Unexpectedly failed to update content");
        }

    }
}
