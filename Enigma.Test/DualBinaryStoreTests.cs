using Enigma.IO;
using Enigma.Store.Binary;
using Enigma.Store.FileSystem;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.IO;
using System.Linq;

namespace Enigma.Test
{
    [TestClass]
    public class DualBinaryStoreTests
    {

        [TestMethod]
        public void ReadWriteTest()
        {
            var leftTestData = new byte[] {1, 2, 3, 4, 5, 6, 7, 8, 9, 0};
            var rightTestData = new byte[] {1, 3, 3, 7};

            var path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "data.dat");
            if (File.Exists(path)) File.Delete(path);

            long leftOffset;
            long rightOffset;
            using (var store = new DualBinaryStore(new FileSystemStreamProvider(path), 0, 1024 * 1024))
            {
                Assert.IsTrue(store.Left.TryWrite(leftTestData, out leftOffset));
                Assert.IsTrue(store.Right.TryWrite(rightTestData, out rightOffset));

                var leftData = store.ReadLeft(leftOffset, leftTestData.Length);
                Assert.IsTrue(leftTestData.SequenceEqual(leftData));

                var rightData = store.ReadRight(rightOffset, rightTestData.Length);
                Assert.IsTrue(rightTestData.SequenceEqual(rightData));
            }

            using (var store = new DualBinaryStore(new FileSystemStreamProvider(path), 0, 1024 * 1024))
            {
                var leftData = store.ReadLeft(leftOffset, leftTestData.Length);
                Assert.IsTrue(leftTestData.SequenceEqual(leftData));

                var rightData = store.ReadRight(rightOffset, rightTestData.Length);
                Assert.IsTrue(rightTestData.SequenceEqual(rightData));
            }
        }

    }
}
