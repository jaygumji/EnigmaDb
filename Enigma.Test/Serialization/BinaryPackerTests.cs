using Enigma.Binary;
using Enigma.Serialization;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Enigma.Test.Serialization
{
    [TestClass]
    public class BinaryPackerTests
    {
        [TestMethod]
        public void PackTest()
        {
            const uint value = (uint) 0x0FC0D096;
            var buffer = BinaryPacker.PackZ(value);

            Assert.IsTrue((3 & buffer[0]) == 3);
            Assert.IsTrue(((buffer[0] & 0xFC) & 0x58) == 0x58);

            var unpacked = BinaryPacker.UnpackZ(buffer, 0);
            Assert.AreEqual(value, unpacked);
        }

    }
}
