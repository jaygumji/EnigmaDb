using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using Enigma.Serialization.PackedBinary;
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

    }

    public class ValueDictionaryComparer : IEqualityComparer<KeyValuePair<string, int>>
    {
        public bool Equals(KeyValuePair<string, int> x, KeyValuePair<string, int> y)
        {
            return x.Key == y.Key && x.Value == y.Value;
        }

        public int GetHashCode(KeyValuePair<string, int> obj)
        {
            return obj.Key.GetHashCode() ^ obj.Value.GetHashCode();
        }
    }
}
