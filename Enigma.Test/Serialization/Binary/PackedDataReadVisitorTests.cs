﻿using System.IO;
using System.Linq;
using Enigma.IO;
using Enigma.Serialization.PackedBinary;
using Enigma.Serialization.Reflection.Emit;
using Enigma.Test.Serialization.HardCoded;
using Enigma.Testing.Fakes.Entities;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Enigma.Test.Serialization.Binary
{

    [TestClass]
    public class PackedDataReadVisitorTests
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

            Assert.IsNotNull(actual.Relation);
            Assert.AreEqual(expected.Relation.Id, actual.Relation.Id);
            Assert.AreEqual(expected.Relation.Name, actual.Relation.Name);
            Assert.AreEqual(expected.Relation.Description, actual.Relation.Description);
            Assert.AreEqual(expected.Relation.Value, actual.Relation.Value);
            Assert.IsNull(actual.DummyRelation);

            Assert.IsTrue(expected.IndexedValues.Keys.SequenceEqual(actual.IndexedValues.Keys));
            Assert.IsTrue(expected.IndexedValues.Values.SequenceEqual(actual.IndexedValues.Values));

            Assert.IsNotNull(actual.Categories);
            Assert.AreEqual(3, actual.Categories.Count);
            Assert.IsTrue(expected.Categories.Keys.SequenceEqual(actual.Categories.Keys));
            Assert.IsTrue(expected.Categories.Values.SequenceEqual(actual.Categories.Values));
        }

        [TestMethod]
        public void ReadHardCodedTravelTest()
        {
            var bytes = SerializationTestContext.GetFilledDataBlockBlob();
            var stream = new MemoryStream(bytes);
            var visitor = new PackedDataReadVisitor(stream);

            var traveller = DataBlockHardCodedTraveller.Create();

            var graph = new DataBlock();
            traveller.Travel(visitor, graph);

            var expected = DataBlock.Filled();
            AssertAreEqual(expected, graph);
        }

        [TestMethod]
        public void ReadDynamicTravelTest()
        {
            var bytes = SerializationTestContext.GetFilledDataBlockBlob();
            var stream = new MemoryStream(bytes);
            var visitor = new PackedDataReadVisitor(stream);

            var context = new DynamicTravellerContext();
            var traveller = context.GetInstance<DataBlock>();

            var graph = new DataBlock();
            traveller.Travel(visitor, graph);

            var expected = DataBlock.Filled();
            AssertAreEqual(expected, graph);
        }

    }
}
