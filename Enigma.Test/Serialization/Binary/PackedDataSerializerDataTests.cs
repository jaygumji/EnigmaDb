using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Enigma.Serialization.PackedBinary;
using Enigma.Test.Fakes;
using Enigma.Test.Serialization.Fakes;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Enigma.Test.Serialization.Binary
{
    [TestClass]
    public class PackedDataSerializerDataTests
    {
        [TestMethod]
        public void WriteAndReadNullableValuesTest()
        {
            var graph = new NullableValuesEntity {
                Id = 1,
                MayBool = null,
                MayDateTime = null,
                MayInt = 44,
                MayTimeSpan = new TimeSpan(22, 30, 10)
            };
            var context = new SerializationTestContext();
            var actual = context.SerializeAndDeserialize(graph);

            Assert.IsNotNull(actual);
            Assert.AreEqual(1, actual.Id);
            Assert.IsNull(actual.MayBool);
            Assert.IsNull(actual.MayDateTime);
            Assert.AreEqual(44, actual.MayInt);
            Assert.AreEqual(new TimeSpan(22, 30, 10), actual.MayTimeSpan);
        }

        [TestMethod]
        public void ValueDictionaryTest()
        {
            var graph = new ValueDictionary {
                Test = new Dictionary<string, int> {
                    {"Test1", 1},
                    {"Test2", 2},
                    {"Test3", 3},
                }
            };

            var context = new SerializationTestContext();
            var actual = context.SerializeAndDeserialize(graph);

            Assert.IsNotNull(actual);
            Assert.IsNotNull(actual.Test);
            Assert.AreEqual(3, actual.Test.Count);

            Assert.IsTrue(graph.Test.SequenceEqual(actual.Test, new ValueDictionaryComparer()));
        }


        [TestMethod]
        public void ComplexDictionaryTest()
        {
            var graph = new ComplexDictionary {
                Test = new Dictionary<Identifier, Category> {
                    {new Identifier {Id = 1, Type = ApplicationType.Api}, new Category {Name = "Warning", Description = "Warning of something", Image = new byte[]{1, 2, 3, 4, 5}}},
                    {new Identifier {Id = 2, Type = ApplicationType.Api}, new Category {Name = "Error", Description = "Error of something", Image = new byte[]{1, 2, 3, 4, 5, 6, 7, 8, 9}}},
                    {new Identifier {Id = 3, Type = ApplicationType.Service}, new Category {Name = "Temporary"}}
                }
            };

            var context = new SerializationTestContext();
            var actual = context.SerializeAndDeserialize(graph);

            Assert.IsNotNull(actual);
            Assert.IsNotNull(actual.Test);
            Assert.AreEqual(3, actual.Test.Count);

            Assert.IsTrue(graph.Test.Keys.SequenceEqual(actual.Test.Keys));
            Assert.IsTrue(graph.Test.Values.SequenceEqual(actual.Test.Values));
        }

        [TestMethod]
        public void IdentifierTest()
        {
            var graph = new Identifier { Id = 1, Type = ApplicationType.Api };

            var context = new SerializationTestContext();
            var actual = context.SerializeAndDeserialize(graph);

            Assert.IsNotNull(actual);
            Assert.AreEqual(graph.Id, actual.Id);
            Assert.AreEqual(graph.Type, actual.Type);
        }

    }

}
