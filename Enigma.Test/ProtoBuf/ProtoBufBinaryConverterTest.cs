﻿using Enigma.ProtocolBuffer;
using Enigma.Test.TestDb;
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

            var car = Scenario.AJD289;

            var bytes = target.Convert(car);

            Assert.IsNotNull(bytes);
            Assert.IsTrue(bytes.Length > 0);
            Assert.AreEqual(10, bytes[0]);
        }
    }
}