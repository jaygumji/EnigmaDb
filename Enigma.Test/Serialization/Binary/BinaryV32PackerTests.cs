using System;
using System.IO;
using Enigma.Binary;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Enigma.Test.Serialization.Binary
{
    [TestClass]
    public class BinaryV32PackerTests
    {
        private static void AssertPackU(UInt32? value)
        {
            using (var stream = new MemoryStream()) {
                BinaryV32Packer.PackU(stream, value);
                stream.Seek(0, SeekOrigin.Begin);
                var actual = BinaryV32Packer.UnpackU(stream);
                Assert.AreEqual(value, actual);
            }
        }

        private static void AssertPackS(Int32? value)
        {
            using (var stream = new MemoryStream()) {
                BinaryV32Packer.PackS(stream, value);
                stream.Seek(0, SeekOrigin.Begin);
                var actual = BinaryV32Packer.UnpackS(stream);
                Assert.AreEqual(value, actual);
            }
        }

        [TestMethod]
        public void Pack32NullUTest()
        {
            AssertPackU(null);
        }

        [TestMethod]
        public void Pack32NullSTest()
        {
            AssertPackS(null);
        }

        [TestMethod]
        public void Pack32UTest()
        {
            AssertPackU(0x0FC0D096U);
        }

        [TestMethod]
        public void Pack32STest()
        {
            AssertPackS(0x0FC0D096);
        }

        [TestMethod]
        public void Pack32HighUTest()
        {
            AssertPackU(UInt32.MaxValue);
        }

        [TestMethod]
        public void Pack32HighSTest()
        {
            AssertPackS(Int32.MaxValue);
        }

        [TestMethod]
        public void Pack32LowUTest()
        {
            AssertPackU(UInt32.MinValue);
        }

        [TestMethod]
        public void Pack32LowSTest()
        {
            AssertPackS(Int32.MinValue);
        }

    }
}