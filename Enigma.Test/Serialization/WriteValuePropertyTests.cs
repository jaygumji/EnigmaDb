using System;
using System.Collections.Generic;
using Enigma.Test.Fakes;
using Enigma.Test.Serialization.Graphs;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Enigma.Test.Serialization
{
    [TestClass]
    public class WriteValuePropertyTests
    {

        [TestMethod]
        public void WriteInt16Test()
        {
            var context = new SerializationTestContext();
            context.AssertWriteSingleProperty(new Int16Graph { Value = 42 });
        }

        [TestMethod]
        public void WriteInt32Test()
        {
            var context = new SerializationTestContext();
            context.AssertWriteSingleProperty(new Int32Graph { Value = 42 });
        }

        [TestMethod]
        public void WriteInt64Test()
        {
            var context = new SerializationTestContext();
            context.AssertWriteSingleProperty(new Int64Graph { Value = 42 });
        }

        [TestMethod]
        public void WriteUInt16Test()
        {
            var context = new SerializationTestContext();
            context.AssertWriteSingleProperty(new UInt16Graph { Value = 42 });
        }

        [TestMethod]
        public void WriteUInt32Test()
        {
            var context = new SerializationTestContext();
            context.AssertWriteSingleProperty(new UInt32Graph { Value = 42 });
        }

        [TestMethod]
        public void WriteUInt64Test()
        {
            var context = new SerializationTestContext();
            context.AssertWriteSingleProperty(new UInt64Graph { Value = 42 });
        }

        [TestMethod]
        public void WriteBooleanTest()
        {
            var context = new SerializationTestContext();
            context.AssertWriteSingleProperty(new BooleanGraph { Value = true });
        }

        [TestMethod]
        public void WriteSingleTest()
        {
            var context = new SerializationTestContext();
            context.AssertWriteSingleProperty(new SingleGraph { Value = 42.3f });
        }

        [TestMethod]
        public void WriteDoubleTest()
        {
            var context = new SerializationTestContext();
            context.AssertWriteSingleProperty(new DoubleGraph { Value = 42.7d });
        }

        [TestMethod]
        public void WriteDecimalTest()
        {
            var context = new SerializationTestContext();
            context.AssertWriteSingleProperty(new DecimalGraph { Value = 42.74343M });
        }

        [TestMethod]
        public void WriteTimeSpanTest()
        {
            var context = new SerializationTestContext();
            context.AssertWriteSingleProperty(new TimeSpanGraph { Value = new TimeSpan(12, 30, 00) });
        }

        [TestMethod]
        public void WriteDateTimeTest()
        {
            var context = new SerializationTestContext();
            context.AssertWriteSingleProperty(new DateTimeGraph { Value = new DateTime(2001, 01, 07, 15, 30, 24) });
        }

        [TestMethod]
        public void WriteStringTest()
        {
            var context = new SerializationTestContext();
            context.AssertWriteSingleProperty(new StringGraph { Value = "Hello World" });
        }

        [TestMethod]
        public void WriteGuidTest()
        {
            var context = new SerializationTestContext();
            context.AssertWriteSingleProperty(new GuidGraph { Value = Guid.Empty });
        }

        [TestMethod]
        public void WriteBlobTest()
        {
            var context = new SerializationTestContext();
            context.AssertWriteSingleProperty(new BlobGraph { Value = new byte[]{1,2,3} });
        }

        [TestMethod]
        public void WriteEnumTest()
        {
            var context = new SerializationTestContext();
            context.AssertWriteSingleProperty(new EnumGraph { Value = ApplicationType.Api });
        }

        [TestMethod]
        public void WriteComplexTest()
        {
            var context = new SerializationTestContext();
            context.AssertWrite(4, new ComplexGraph { Value = new Relation {Id = Guid.Empty, Name = "Test", Value = 1} });
        }

        [TestMethod]
        public void WriteDictionaryTest()
        {
            var context = new SerializationTestContext();
            context.AssertWrite(2, new DictionaryGraph { Value = new Dictionary<int, string> { {2, "Test"}}});
        }

        [TestMethod]
        public void WriteDictionaryWithComplexValueTest()
        {
            var context = new SerializationTestContext();
            context.AssertWrite(12, new DictionaryWithComplexValueGraph { Value = new Dictionary<string, Category> {
                    {"A", new Category {Name = "Warning", Description = "Warning of something", Image = new byte[]{1, 2, 3, 4, 5}}},
                    {"B", new Category {Name = "Error", Description = "Error of something", Image = new byte[]{1, 2, 3, 4, 5, 6, 7, 8, 9}}},
                    {"C", new Category {Name = "Temporary"}}
            }});
        }

        [TestMethod]
        public void WriteDictionaryWithComplexKeyAndValueTest()
        {
            var context = new SerializationTestContext();
            context.AssertWrite(15, new DictionaryWithComplexKeyAndValueGraph {
                Value = new Dictionary<Identifier, Category> {
                    {new Identifier {Id = 1, Type = ApplicationType.Api}, new Category {Name = "Warning", Description = "Warning of something", Image = new byte[]{1, 2, 3, 4, 5}}},
                    {new Identifier {Id = 2, Type = ApplicationType.Api}, new Category {Name = "Error", Description = "Error of something", Image = new byte[]{1, 2, 3, 4, 5, 6, 7, 8, 9}}},
                    {new Identifier {Id = 3, Type = ApplicationType.Service}, new Category {Name = "Temporary"}}
                }
            });
        }

        [TestMethod]
        public void WriteDictionaryWithComplexKeyTest()
        {
            var context = new SerializationTestContext();
            context.AssertWrite(9, new DictionaryWithComplexKeyGraph {
                Value = new Dictionary<Identifier, string> {
                    {new Identifier {Id = 1, Type = ApplicationType.Api}, "A"},
                    {new Identifier {Id = 2, Type = ApplicationType.Api}, "B"},
                    {new Identifier {Id = 3, Type = ApplicationType.Service}, "C"}
                }
            });
        }

        [TestMethod]
        public void WriteCollectionTest()
        {
            var context = new SerializationTestContext();
            context.AssertWrite(1, new CollectionGraph { Value = new List<string> { "Test" } });
        }

        [TestMethod]
        public void WriteCollectionOfComplexTest()
        {
            var context = new SerializationTestContext();
            context.AssertWrite(4, new CollectionOfComplexGraph { Value = new List<Relation> { new Relation { Id = Guid.Empty, Name = "Test", Value = 1 } } });
        }

    }
}
