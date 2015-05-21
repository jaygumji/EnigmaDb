using Enigma.Test.Serialization.Graphs;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Enigma.Test.Serialization
{
    [TestClass]
    public class ReadNullableValuePropertyTests
    {

        [TestMethod]
        public void ReadInt16Test()
        {
            var context = new SerializationTestContext();
            context.AssertReadSingleProperty<NullableInt16Graph>();
        }

        [TestMethod]
        public void ReadInt32Test()
        {
            var context = new SerializationTestContext();
            context.AssertReadSingleProperty<NullableInt32Graph>();
        }

        [TestMethod]
        public void ReadInt64Test()
        {
            var context = new SerializationTestContext();
            context.AssertReadSingleProperty<NullableInt64Graph>();
        }

        [TestMethod]
        public void ReadUInt16Test()
        {
            var context = new SerializationTestContext();
            context.AssertReadSingleProperty<NullableUInt16Graph>();
        }

        [TestMethod]
        public void ReadUInt32Test()
        {
            var context = new SerializationTestContext();
            context.AssertReadSingleProperty<NullableUInt32Graph>();
        }

        [TestMethod]
        public void ReadUInt64Test()
        {
            var context = new SerializationTestContext();
            context.AssertReadSingleProperty<NullableUInt64Graph>();
        }

        [TestMethod]
        public void ReadBooleanTest()
        {
            var context = new SerializationTestContext();
            context.AssertReadSingleProperty<NullableBooleanGraph>();
        }

        [TestMethod]
        public void ReadSingleTest()
        {
            var context = new SerializationTestContext();
            context.AssertReadSingleProperty<NullableSingleGraph>();
        }

        [TestMethod]
        public void ReadDoubleTest()
        {
            var context = new SerializationTestContext();
            context.AssertReadSingleProperty<NullableDoubleGraph>();
        }

        [TestMethod]
        public void ReadDecimalTest()
        {
            var context = new SerializationTestContext();
            context.AssertReadSingleProperty<NullableDecimalGraph>();
        }

        [TestMethod]
        public void ReadTimeSpanTest()
        {
            var context = new SerializationTestContext();
            context.AssertReadSingleProperty<NullableTimeSpanGraph>();
        }

        [TestMethod]
        public void ReadDateTimeTest()
        {
            var context = new SerializationTestContext();
            context.AssertReadSingleProperty<NullableDateTimeGraph>();
        }

        [TestMethod]
        public void ReadGuidTest()
        {
            var context = new SerializationTestContext();
            context.AssertReadSingleProperty<NullableGuidGraph>();
        }

        [TestMethod]
        public void ReadEnumTest()
        {
            var context = new SerializationTestContext();
            context.AssertReadSingleProperty<NullableEnumGraph>();
        }

    }
}