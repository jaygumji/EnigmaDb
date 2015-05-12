using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using Enigma.Serialization.PackedBinary;
using Enigma.Test.Fakes;
using Enigma.Test.Serialization.Fakes;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Enigma.Test.Serialization
{
    [TestClass]
    public class SpecificTests
    {

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

            ValueDictionary actual;

            var serializer = new PackedDataSerializer<ValueDictionary>();
            using (var stream = new MemoryStream()) {
                serializer.Serialize(stream, graph);

                stream.Seek(0, SeekOrigin.Begin);

                actual = serializer.Deserialize(stream);
            }

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

            ComplexDictionary actual;
            var serializer = new PackedDataSerializer<ComplexDictionary>();
            using (var stream = new MemoryStream()) {
                serializer.Serialize(stream, graph);

                stream.Seek(0, SeekOrigin.Begin);

                actual = serializer.Deserialize(stream);
            }

            Assert.IsNotNull(actual);
            Assert.IsNotNull(actual.Test);
            Assert.AreEqual(3, actual.Test.Count);

            Assert.IsTrue(graph.Test.Keys.SequenceEqual(actual.Test.Keys));
            Assert.IsTrue(graph.Test.Values.SequenceEqual(actual.Test.Values));
        }

        [TestMethod]
        public void IdentifierTest()
        {
            var graph = new Identifier {Id = 1, Type = ApplicationType.Api};

            Identifier actual;
            var serializer = new PackedDataSerializer<Identifier>();
            using (var stream = new MemoryStream()) {
                serializer.Serialize(stream, graph);

                stream.Seek(0, SeekOrigin.Begin);

                actual = serializer.Deserialize(stream);
            }

            Assert.IsNotNull(actual);
            Assert.AreEqual(graph.Id, actual.Id);
            Assert.AreEqual(graph.Type, actual.Type);
        }

    }

}
