using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Enigma.Test
{
    [TestClass]
    public class BlobComparerTests
    {
        [TestMethod]
        public void CompareEqualTest()
        {
            var left = new Byte[] { 1, 2, 3 };
            var right = new Byte[] { 1, 2, 3 };

            const int expected = 0;
            Assert.AreEqual(expected, BlobComparer.CompareBlobs(left, right));
        }

        [TestMethod]
        public void CompareEqualBothNullTest()
        {
            var left = (byte[]) null;
            var right = (byte[])null;

            const int expected = 0;
            Assert.AreEqual(expected, BlobComparer.CompareBlobs(left, right));
        }

        [TestMethod]
        public void CompareLeftIsNullTest()
        {
            var left = (byte[])null;
            var right = new Byte[] { 1, 2, 3 };

            const int expected = -1;
            Assert.AreEqual(expected, BlobComparer.CompareBlobs(left, right));
        }

        [TestMethod]
        public void CompareLeftLessTest()
        {
            var left = new Byte[] { 1, 0, 3 };
            var right = new Byte[] { 1, 2, 3 };

            const int expected = -1;
            Assert.AreEqual(expected, BlobComparer.CompareBlobs(left, right));
        }

        [TestMethod]
        public void CompareLeftGreaterTest()
        {
            var left = new Byte[] { 2, 1, 3 };
            var right = new Byte[] { 1, 2, 3 };

            const int expected = 1;
            Assert.AreEqual(expected, BlobComparer.CompareBlobs(left, right));
        }

        [TestMethod]
        public void CompareRightIsNullTest()
        {
            var right = (byte[])null;
            var left = new Byte[] { 1, 2, 3 };

            const int expected = 1;
            Assert.AreEqual(expected, BlobComparer.CompareBlobs(left, right));
        }

        [TestMethod]
        public void CompareRightLessTest()
        {
            var right = new Byte[] { 1, 0, 3 };
            var left = new Byte[] { 1, 2, 3 };

            const int expected = 1;
            Assert.AreEqual(expected, BlobComparer.CompareBlobs(left, right));
        }

        [TestMethod]
        public void CompareRightGreaterTest()
        {
            var right = new Byte[] { 2, 1, 3 };
            var left = new Byte[] { 1, 2, 3 };

            const int expected = -1;
            Assert.AreEqual(expected, BlobComparer.CompareBlobs(left, right));
        }

        [TestMethod]
        public void GetHashCodeOfNullTest()
        {
            const int expected = 0;
            var actual = BlobComparer.GetBlobHashCode(null);
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void GetHashCodeOfWholeIntTest()
        {
            const int expected = 402338134;
            var actual = BlobComparer.GetBlobHashCode(BitConverter.GetBytes(expected));
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void GetHashCodeOfHalfIntTest()
        {
            const short expected = 31241;
            var actual = BlobComparer.GetBlobHashCode(BitConverter.GetBytes(expected));
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void GetHashCodeOfSeriesTest()
        {
            const int part1 = 402338134;
            const int part2 = 235436;
            const short part3 = 31241;

            var blob = new byte[10];
            Array.Copy(BitConverter.GetBytes(part1), 0, blob, 0, 4);
            Array.Copy(BitConverter.GetBytes(part2), 0, blob, 4, 4);
            Array.Copy(BitConverter.GetBytes(part3), 0, blob, 8, 2);

            var actual = BlobComparer.GetBlobHashCode(blob);
            const int expected = part1 ^ part2 ^ part3;
            Assert.AreEqual(expected, actual);
        }

    }
}
