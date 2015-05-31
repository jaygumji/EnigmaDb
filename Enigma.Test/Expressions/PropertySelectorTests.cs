using System.Linq;
using Enigma.Expressions;
using Enigma.Test.TestDb;
using Enigma.Testing.Fakes.Entities.Cars;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Enigma.Test.Expressions
{
    [TestClass]
    public class PropertySelectorTests
    {
        [TestMethod]
        public void GetPropertyPathTest()
        {
            var path = PropertySelector.GetPropertyPath<Car, int>(c => c.Engine.HorsePower).ToList();
            Assert.AreEqual(2, path.Count);
            Assert.AreEqual("Engine", path[0].Name);
            Assert.AreEqual("HorsePower", path[1].Name);
        }

    }
}
