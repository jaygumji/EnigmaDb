using System.Linq;
using Enigma.Test.Fakes;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Enigma.Test.Serialization.Binary
{
    [TestClass]
    public class PackedDataWriteVisitorTests
    {

        private static string GetHardCodedHexString()
        {
            var bytes = DataBlock.SerializedFilled();
            Assert.IsNotNull(bytes);
            Assert.IsTrue(bytes.Length > 0);
            var hex = "0x" + string.Join("", bytes.Select(b => b.ToString("X")));
            Assert.IsNotNull(hex);
            return hex;
        }

        [TestMethod]
        public void WriteHardCodedTravelTest()
        {
            var hex = GetHardCodedHexString();
            StringAssert.StartsWith(hex, "0x410429115F5A3B9FA4585AEE0C9E6990A98FFB00048656C6C6F20576F726C64C2A4F1045E9798214871B2C09873ED5D3C18242FB1C4F6D7E9CD2083E1C7D332AD6D3882446666042288B81E85EB515B1402C80448E258000301094E5AA20000000000303480504347A81BD183812A3C1140FF300012344FF37000FF50005465737431FF50005465737432FF50005465737433FF50005465737434FF50005465737435048FFE000803025ACA187CC804CFF57000410D4F78EF6626F6B47BC5E71AD86549A638FFA000436F6E6E656374696F6ECFF2400047656E6572696320636F6E6E656374696F6E206265747765656E2072656C6174696F6E731044D0000");
        }

        [TestMethod]
        public void WriteDynamicTravelTest()
        {
            var context = new SerializationTestContext();

            var bytes = context.Pack(DataBlock.Filled());
            Assert.IsNotNull(bytes);
            Assert.IsTrue(bytes.Length > 0);
            var hex = "0x" + string.Join("", bytes.Select(b => b.ToString("X")));
            Assert.IsNotNull(hex);
            var expected = GetHardCodedHexString();
            Assert.AreEqual(expected, hex);
        }

        [TestMethod]
        public void ProtoBufTest()
        {
            var converter = new ProtocolBuffer.ProtocolBufferBinaryConverter<DataBlock>();
            var bytes = converter.Convert(DataBlock.Filled());
            Assert.IsNotNull(bytes);
            Assert.IsTrue(bytes.Length > 0);
            var hex = "0x" + string.Join("", bytes.Select(b => b.ToString("X")));
            Assert.IsNotNull(hex);
        }

    }
}
