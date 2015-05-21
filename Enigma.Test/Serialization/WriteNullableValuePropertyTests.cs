using System;
using Enigma.Test.Fakes;
using Enigma.Test.Serialization.Graphs;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Enigma.Test.Serialization
{
    [TestClass]
    public class WriteNullableValuePropertyTests
    {
        [TestMethod]
        public void WriteNullableInt16Test()
        {
            var context = new SerializationTestContext();
            context.AssertWriteSingleProperty(new NullableInt16Graph { Value = 42 });
        }

        [TestMethod]
        public void WriteNullableInt32Test()
        {
            var context = new SerializationTestContext();
            context.AssertWriteSingleProperty(new NullableInt32Graph { Value = 42 });
        }

        [TestMethod]
        public void WriteNullableInt64Test()
        {
            var context = new SerializationTestContext();
            context.AssertWriteSingleProperty(new NullableInt64Graph { Value = 42 });
        }

        [TestMethod]
        public void WriteNullableUInt16Test()
        {
            var context = new SerializationTestContext();
            context.AssertWriteSingleProperty(new NullableUInt16Graph { Value = 42 });
        }

        [TestMethod]
        public void WriteNullableUInt32Test()
        {
            var context = new SerializationTestContext();
            context.AssertWriteSingleProperty(new NullableUInt32Graph { Value = 42 });
        }

        [TestMethod]
        public void WriteNullableUInt64Test()
        {
            var context = new SerializationTestContext();
            context.AssertWriteSingleProperty(new NullableUInt64Graph { Value = 42 });
        }

        [TestMethod]
        public void WriteNullableBooleanTest()
        {
            var context = new SerializationTestContext();
            context.AssertWriteSingleProperty(new NullableBooleanGraph { Value = true });
        }

        [TestMethod]
        public void WriteNullableSingleTest()
        {
            var context = new SerializationTestContext();
            context.AssertWriteSingleProperty(new NullableSingleGraph { Value = 42.3f });
        }

        [TestMethod]
        public void WriteNullableDoubleTest()
        {
            var context = new SerializationTestContext();
            context.AssertWriteSingleProperty(new NullableDoubleGraph { Value = 42.7d });
        }

        [TestMethod]
        public void WriteNullableDecimalTest()
        {
            var context = new SerializationTestContext();
            context.AssertWriteSingleProperty(new NullableDecimalGraph { Value = 42.5434M });
        }

        [TestMethod]
        public void WriteNullableTimeSpanTest()
        {
            var context = new SerializationTestContext();
            context.AssertWriteSingleProperty(new NullableTimeSpanGraph { Value = new TimeSpan(12,30,00) });
        }

        [TestMethod]
        public void WriteNullableDateTimeTest()
        {
            var context = new SerializationTestContext();
            context.AssertWriteSingleProperty(new NullableDateTimeGraph { Value = new DateTime(2001, 01, 07, 15, 30, 24) });
        }

        [TestMethod]
        public void WriteNullableGuidTest()
        {
            var context = new SerializationTestContext();
            context.AssertWriteSingleProperty(new NullableGuidGraph { Value = Guid.Empty });
        }

        [TestMethod]
        public void WriteNullableEnumTest()
        {
            var context = new SerializationTestContext();
            context.AssertWriteSingleProperty(new NullableEnumGraph { Value = ApplicationType.Api });
        }

    }

}