using System.IO;
using System.Linq;
using Enigma.IO;
using Enigma.Serialization;
using Enigma.Serialization.Reflection.Emit;
using Enigma.Test.Fakes;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Enigma.Test.Serialization
{

    [TestClass]
    public class ReadTravellerTests
    {

        private static void AssertAreEqual(DataBlock expected, DataBlock actual)
        {
            Assert.AreEqual(expected.Id, actual.Id);
            Assert.AreEqual(expected.Int16, actual.Int16);
            Assert.AreEqual(expected.Int32, actual.Int32);
            Assert.AreEqual(expected.Int64, actual.Int64);
            Assert.AreEqual(expected.UInt16, actual.UInt16);
            Assert.AreEqual(expected.UInt32, actual.UInt32);
            Assert.AreEqual(expected.UInt64, actual.UInt64);
            Assert.AreEqual(expected.Single, actual.Single);
            Assert.AreEqual(expected.Double, actual.Double);
            Assert.AreEqual(expected.Decimal, actual.Decimal);
            Assert.AreEqual(expected.TimeSpan, actual.TimeSpan);
            Assert.AreEqual(expected.DateTime, actual.DateTime);
            Assert.AreEqual(expected.String, actual.String);
            Assert.AreEqual(expected.Boolean, actual.Boolean);
            Assert.AreEqual(expected.Byte, actual.Byte);
            Assert.IsTrue(expected.Blob.SequenceEqual(actual.Blob));

            Assert.IsTrue(expected.Messages.SequenceEqual(actual.Messages));
            Assert.IsTrue(expected.Stamps.SequenceEqual(actual.Stamps));

            Assert.AreEqual(expected.Relation.Id, actual.Relation.Id);
            Assert.AreEqual(expected.Relation.Name, actual.Relation.Name);
            Assert.AreEqual(expected.Relation.Description, actual.Relation.Description);
            Assert.AreEqual(expected.Relation.Value, actual.Relation.Value);
        }

        [TestMethod]
        public void ReadHardCodedTravelTest()
        {
            var bytes = DataBlock.SerializedFilled();
            var stream = new MemoryStream(bytes);
            var reader = new BinaryDataReader(stream);
            var visitor = new PackedDataReadVisitor(reader);

            var traveller = new DataBlockHardCodedTraveller();

            var graph = new DataBlock();
            traveller.Travel(visitor, graph);

            var expected = DataBlock.Filled();
            AssertAreEqual(expected, graph);
        }

        [TestMethod]
        public void ReadDynamicTravelTest()
        {
            var bytes = DataBlock.SerializedFilled();
            var stream = new MemoryStream(bytes);
            var reader = new BinaryDataReader(stream);
            var visitor = new PackedDataReadVisitor(reader);

            var context = new DynamicTravellerContext(true);
            var traveller = context.GetInstance<DataBlock>();

            var graph = new DataBlock();
            traveller.Travel(visitor, graph);

            var expected = DataBlock.Filled();
            AssertAreEqual(expected, graph);
        }

    }
}
