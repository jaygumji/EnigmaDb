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
        public void WriteDictionaryWithDictionaryKeyTest()
        {
            var context = new SerializationTestContext();
            context.AssertWrite(3, new DictionaryWithDictionaryKeyGraph {
                Value = new Dictionary<Dictionary<int, string>, string> {
                    {new Dictionary<int, string> {{42, "No 42"}}, "Hello World"}
                }
            });
        }

        [TestMethod]
        public void WriteDictionaryWithDictionaryValueTest()
        {
            var context = new SerializationTestContext();
            context.AssertWrite(3, new DictionaryWithDictionaryValueGraph {
                Value = new Dictionary<string, Dictionary<int, string>> {
                    {"X", new Dictionary<int, string> {{42, "No 42"}}}
                }
            });
        }

        [TestMethod]
        public void WriteDictionaryWithDictionaryKeyAndValueTest()
        {
            var context = new SerializationTestContext();
            context.AssertWrite(4, new DictionaryWithDictionaryKeyAndValueGraph {
                Value = new Dictionary<Dictionary<string, int>, Dictionary<int, string>> {
                    {new Dictionary<string, int> {{"No 42", 42}}, new Dictionary<int, string> {{42, "No 42"}}}
                }
            });
        }

        [TestMethod]
        public void WriteDictionaryWithCollectionKeyTest()
        {
            var context = new SerializationTestContext();
            context.AssertWrite(2, new DictionaryWithCollectionKeyGraph {
                Value = new Dictionary<List<int>, string> {
                    {new List<int> {42}, "Hello World"}
                }
            });
        }

        [TestMethod]
        public void WriteDictionaryWithCollectionValueTest()
        {
            var context = new SerializationTestContext();
            context.AssertWrite(2, new DictionaryWithCollectionValueGraph {
                Value = new Dictionary<string, List<int>> {
                    {"X", new List<int> {42}}
                }
            });
        }

        [TestMethod]
        public void WriteDictionaryWithCollectionKeyAndValueTest()
        {
            var context = new SerializationTestContext();
            context.AssertWrite(2, new DictionaryWithCollectionKeyAndValueGraph {
                Value = new Dictionary<List<int>, List<string>> {
                    {new List<int> {42}, new List<string> {"No 42"}}
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

        [TestMethod]
        public void WriteCollectionOfDictionaryTest()
        {
            var context = new SerializationTestContext();
            context.AssertWrite(2, new CollectionOfDictionaryGraph {
                Value = new List<Dictionary<string, int>> {
                    new Dictionary<string, int> {{"Test", 42}}
                }
            });
        }

        [TestMethod]
        public void WriteCollectionOfCollectionTest()
        {
            var context = new SerializationTestContext();
            context.AssertWrite(1, new CollectionOfCollectionGraph {
                Value = new List<List<string>> {
                    new List<string> {"Test"}
                }
            });
        }

        [TestMethod]
        public void WriteJaggedArrayTest()
        {
            var context = new SerializationTestContext();
            context.AssertWrite(6, new JaggedArrayGraph {
                Value = new[] { new []{5, 2, 3}, new []{1, 2, 3} }
            });
        }

    }
}
