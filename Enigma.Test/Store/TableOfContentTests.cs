using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Enigma.Store;
using Enigma.Store.Keys;
using Enigma.Store.Binary;

namespace Enigma.Test.Store
{

    internal static class TableOfContentManagerExtensions
    {
        public static void Add(this ITableOfContentManager tableOfContent, IKey key, long offset, long length)
        {
            Entry entry;
            tableOfContent.TryReserve(key, out entry);
            entry.ValueOffset = offset;
            entry.ValueLength = length;
            tableOfContent.Enable(entry);
        }
    }

    [TestClass]
    public class TableOfContentTests
    {

        [TestMethod]
        public void AddTest()
        {
            bool isFragmented;
            var store = new FakeBinaryStore();
            var tableOfContent = TableOfContent.From(store, out isFragmented);

            tableOfContent.Add(new Int32Key(1), 0, 200);

            Assert.AreEqual(22, store.Length);
            Assert.AreEqual(22, store.CurrentOffset);
        }

        [TestMethod]
        public void RemoveTest()
        {
            var entry = new Entry { Key = new Int32Key(1), ValueOffset = 0, ValueLength = 200 };
            var store = new FakeBinaryStore(EntryBinaryConverter.Instance.Convert(entry));

            bool isFragmented;
            var key = new Int32Key(1);
            var tableOfContent = TableOfContent.From(store, out isFragmented);
            Assert.IsFalse(isFragmented);
            Assert.IsTrue(tableOfContent.Contains(key));
            Assert.IsTrue(tableOfContent.TryRemove(key));
            Assert.IsFalse(tableOfContent.Contains(key));

            tableOfContent = TableOfContent.From(store, out isFragmented);
            Assert.IsTrue(isFragmented);
            Assert.IsFalse(tableOfContent.Contains(key));
        }

        [TestMethod]
        public void AddMultileAndRemoveTest()
        {
            var entry = new Entry { Key = new Int32Key(1), ValueOffset = 0, ValueLength = 200 };
            var store = new FakeBinaryStore(EntryBinaryConverter.Instance.Convert(entry));

            bool isFragmented;
            var tableOfContent = TableOfContent.From(store, out isFragmented);
            Assert.IsFalse(isFragmented);
            tableOfContent.Add(new Int32Key(2), 200, 215);
            tableOfContent.Add(new Int32Key(3), 415, 215);
            tableOfContent.Add(new Int32Key(4), 630, 230);
            tableOfContent.Add(new Int32Key(5), 860, 640);
            tableOfContent.Add(new Int32Key(6), 1500, 500);

            Assert.AreEqual(22 * 6, store.Length);
            Assert.AreEqual(22 * 6, store.CurrentOffset);

            var key1 = new Int32Key(1);
            var key2 = new Int32Key(2);
            var key3 = new Int32Key(3);

            Assert.IsTrue(tableOfContent.Contains(key1));
            Assert.IsTrue(tableOfContent.Contains(key2));
            Assert.IsTrue(tableOfContent.Contains(key3));
            Assert.IsTrue(tableOfContent.TryRemove(key2));
            Assert.IsTrue(tableOfContent.Contains(key1));
            Assert.IsFalse(tableOfContent.Contains(key2));
            Assert.IsTrue(tableOfContent.Contains(key3));

            Assert.IsTrue(tableOfContent.TryRemove(key3));
            Assert.IsTrue(tableOfContent.Contains(key1));
            Assert.IsFalse(tableOfContent.Contains(key2));
            Assert.IsFalse(tableOfContent.Contains(key3));

            tableOfContent = TableOfContent.From(store, out isFragmented);
            Assert.IsTrue(isFragmented);
            Assert.IsTrue(tableOfContent.Contains(key1));
            Assert.IsFalse(tableOfContent.Contains(key2));
            Assert.IsFalse(tableOfContent.Contains(key3));
        }
    }
}
