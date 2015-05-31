using Enigma.ProtocolBuffer;
using Enigma.Test.TestDb;
using Enigma.Testing.Fakes.Entities.Cars;
using Microsoft.VisualStudio.TestTools.UnitTesting;
namespace Enigma.Test.ProtoBuf
{
    [TestClass]
    public class ProtoBufBinaryConverterTest
    {

        [TestMethod]
        public void ConvertTest()
        {
            var target = new ProtocolBufferBinaryConverter<Car>();

            var car = RandomCars.AK9777;

            var bytes = target.Convert(car);

            Assert.IsNotNull(bytes);
            Assert.IsTrue(bytes.Length > 0);


        }
    }
}
