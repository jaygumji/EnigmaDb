using System;
using System.IO;
using Enigma.Serialization.PackedBinary;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Enigma.Test.Serialization.Fakes
{
    [TestClass]
    public class NullableValuesTests
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

            NullableValuesEntity actual;
            var serializer = new PackedDataSerializer<NullableValuesEntity>();
            using (var stream = new MemoryStream()) {
                serializer.Serialize(stream, graph);

                stream.Seek(0, SeekOrigin.Begin);

                actual = serializer.Deserialize(stream);
            }

            Assert.IsNotNull(actual);
            Assert.AreEqual(1, actual.Id);
            Assert.IsNull(actual.MayBool);
            Assert.IsNull(actual.MayDateTime);
            Assert.AreEqual(44, actual.MayInt);
            Assert.AreEqual(new TimeSpan(22, 30, 10), actual.MayTimeSpan);
        }

    }
}
