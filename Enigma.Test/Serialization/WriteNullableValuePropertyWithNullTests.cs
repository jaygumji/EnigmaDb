using Enigma.Testing.Fakes.Graphs;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Enigma.Test.Serialization
{
    [TestClass]
    public class WriteNullableValuePropertyWithNullTests
    {
        [TestMethod]
        public void WriteNullableInt16Test()
        {
            var context = new SerializationTestContext();
            context.AssertWriteSingleProperty(new NullableInt16Graph { Value = null });
        }

        [TestMethod]
        public void WriteNullableInt32Test()
        {
            var context = new SerializationTestContext();
            context.AssertWriteSingleProperty(new NullableInt32Graph { Value = null });
        }

        [TestMethod]
        public void WriteNullableInt64Test()
        {
            var context = new SerializationTestContext();
            context.AssertWriteSingleProperty(new NullableInt64Graph { Value = null });
        }

        [TestMethod]
        public void WriteNullableUInt16Test()
        {
            var context = new SerializationTestContext();
            context.AssertWriteSingleProperty(new NullableUInt16Graph { Value = null });
        }

        [TestMethod]
        public void WriteNullableUInt32Test()
        {
            var context = new SerializationTestContext();
            context.AssertWriteSingleProperty(new NullableUInt32Graph { Value = null });
        }

        [TestMethod]
        public void WriteNullableUInt64Test()
        {
            var context = new SerializationTestContext();
            context.AssertWriteSingleProperty(new NullableUInt64Graph { Value = null });
        }

        [TestMethod]
        public void WriteNullableBooleanTest()
        {
            var context = new SerializationTestContext();
            context.AssertWriteSingleProperty(new NullableBooleanGraph { Value = null });
        }

        [TestMethod]
        public void WriteNullableSingleTest()
        {
            var context = new SerializationTestContext();
            context.AssertWriteSingleProperty(new NullableSingleGraph { Value = null });
        }

        [TestMethod]
        public void WriteNullableDoubleTest()
        {
            var context = new SerializationTestContext();
            context.AssertWriteSingleProperty(new NullableDoubleGraph { Value = null });
        }

        [TestMethod]
        public void WriteNullableDecimalTest()
        {
            var context = new SerializationTestContext();
            context.AssertWriteSingleProperty(new NullableDecimalGraph { Value = null });
        }

        [TestMethod]
        public void WriteNullableTimeSpanTest()
        {
            var context = new SerializationTestContext();
            context.AssertWriteSingleProperty(new NullableTimeSpanGraph { Value = null });
        }

        [TestMethod]
        public void WriteNullableDateTimeTest()
        {
            var context = new SerializationTestContext();
            context.AssertWriteSingleProperty(new NullableDateTimeGraph { Value = null });
        }

        [TestMethod]
        public void WriteNullableGuidTest()
        {
            var context = new SerializationTestContext();
            context.AssertWriteSingleProperty(new NullableGuidGraph { Value = null });
        }

        [TestMethod]
        public void WriteNullableEnumTest()
        {
            var context = new SerializationTestContext();
            context.AssertWriteSingleProperty(new NullableEnumGraph { Value = null });
        }

    }
}