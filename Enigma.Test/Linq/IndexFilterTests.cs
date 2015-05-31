using System.Linq;
using Enigma.Test.TestDb;
using Enigma.Testing.Fakes.Entities.Cars;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Enigma.Test.Linq
{
    [TestClass]
    public class IndexFilterTests
    {

        [TestMethod]
        public void OrderByOnIndexAndTakeTest()
        {
            using (var context = Scenario.ManyCars()) {
                var q = (from c in context.Cars
                    orderby c.EstimatedValue descending
                    select c).Take(3);

                var list = q.ToList();
                var first = list[0];
                Assert.AreEqual(770000, first.EstimatedValue);
            }
        }

        [TestMethod]
        public void SimpleFilterOnIndexAndOrderByOnIndexAndTakeTest()
        {
            using (var context = Scenario.ManyCars()) {
                var q = (from c in context.Cars
                         where c.EstimatedValue > 50000
                         orderby c.EstimatedValue descending
                         select c).Take(3);

                var list = q.ToList();
                var first = list[0];
                Assert.AreEqual(770000, first.EstimatedValue);
            }
        }

        [TestMethod]
        public void GetEntityByNestedIndexValue()
        {
            using (var context = Scenario.ManyCars()) {
                var q = (from c in context.Cars
                    where c.Engine.HorsePower == 127
                    select c);

                var car = q.First();

                Assert.AreEqual(RandomCars.AK9777.RegistrationNumber, car.RegistrationNumber);
            }
        }

    }
}
