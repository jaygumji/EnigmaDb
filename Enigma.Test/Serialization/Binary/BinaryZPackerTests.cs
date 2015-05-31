using Enigma.Binary;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Enigma.Test.Serialization.Binary
{
    [TestClass]
    public class BinaryZPackerTests
    {
        [TestMethod]
        public void PackZTest()
        {
            const uint value = (uint) 0x0FC0D096;
            var buffer = BinaryZPacker.Pack(value);

            Assert.IsTrue((3 & buffer[0]) == 3);
            Assert.IsTrue(((buffer[0] & 0xFC) & 0x58) == 0x58);

            var unpacked = BinaryZPacker.Unpack(buffer, 0);
            Assert.AreEqual(value, unpacked);
        }

    }
}
