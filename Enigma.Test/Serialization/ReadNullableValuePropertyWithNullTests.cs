using Enigma.Test.Serialization.Graphs;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Enigma.Test.Serialization
{
    [TestClass]
    public class ReadNullableValuePropertyWithNullTests
    {

        [TestMethod]
        public void ReadInt16Test()
        {
            var context = new SerializationTestContext();
            context.AssertReadSinglePropertyWithNull<NullableInt16Graph>();
        }

        [TestMethod]
        public void ReadInt32Test()
        {
            var context = new SerializationTestContext();
            context.AssertReadSinglePropertyWithNull<NullableInt32Graph>();
        }

        [TestMethod]
        public void ReadInt64Test()
        {
            var context = new SerializationTestContext();
            context.AssertReadSinglePropertyWithNull<NullableInt64Graph>();
        }

        [TestMethod]
        public void ReadUInt16Test()
        {
            var context = new SerializationTestContext();
            context.AssertReadSinglePropertyWithNull<NullableUInt16Graph>();
        }

        [TestMethod]
        public void ReadUInt32Test()
        {
            var context = new SerializationTestContext();
            context.AssertReadSinglePropertyWithNull<NullableUInt32Graph>();
        }

        [TestMethod]
        public void ReadUInt64Test()
        {
            var context = new SerializationTestContext();
            context.AssertReadSinglePropertyWithNull<NullableUInt64Graph>();
        }

        [TestMethod]
        public void ReadBooleanTest()
        {
            var context = new SerializationTestContext();
            context.AssertReadSinglePropertyWithNull<NullableBooleanGraph>();
        }

        [TestMethod]
        public void ReadSingleTest()
        {
            var context = new SerializationTestContext();
            context.AssertReadSinglePropertyWithNull<NullableSingleGraph>();
        }

        [TestMethod]
        public void ReadDoubleTest()
        {
            var context = new SerializationTestContext();
            context.AssertReadSinglePropertyWithNull<NullableDoubleGraph>();
        }

        [TestMethod]
        public void ReadDecimalTest()
        {
            var context = new SerializationTestContext();
            context.AssertReadSinglePropertyWithNull<NullableDecimalGraph>();
        }

        [TestMethod]
        public void ReadTimeSpanTest()
        {
            var context = new SerializationTestContext();
            context.AssertReadSinglePropertyWithNull<NullableTimeSpanGraph>();
        }

        [TestMethod]
        public void ReadDateTimeTest()
        {
            var context = new SerializationTestContext();
            context.AssertReadSinglePropertyWithNull<NullableDateTimeGraph>();
        }

        [TestMethod]
        public void ReadGuidTest()
        {
            var context = new SerializationTestContext();
            context.AssertReadSinglePropertyWithNull<NullableGuidGraph>();
        }

        [TestMethod]
        public void ReadEnumTest()
        {
            var context = new SerializationTestContext();
            context.AssertReadSinglePropertyWithNull<NullableEnumGraph>();
        }

    }
}