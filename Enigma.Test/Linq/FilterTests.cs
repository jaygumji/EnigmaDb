using System;
using Enigma.Test.TestDb;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.Linq;

namespace Enigma.Test.Linq
{
    [TestClass]
    public class FilterTests
    {

        [TestMethod]
        public void EqualIdTest()
        {
            using (var context = Scenario.SomeCars())
            {
                var q = (from c in context.Cars
                         where c.RegistrationNumber == "AJD289"
                         select c);

                var car = q.First();
                Assert.AreEqual("AJD289", car.RegistrationNumber);
                Assert.AreEqual(CarBrand.Toyota, car.Model.Brand);
            }
        }
        
        [TestMethod]
        public void EqualPropertyTest()
        {
            using (var context = Scenario.SomeCars())
            {
                var q = (from c in context.Cars
                         where c.Nationality == Nationality.Sweden
                         select c);

                var cars = q.ToList();
                Assert.AreEqual(3, cars.Count);

                var car = cars.First(c => c.RegistrationNumber == "AJD289");
                Assert.AreEqual("AJD289", car.RegistrationNumber);
                Assert.AreEqual(CarBrand.Toyota, car.Model.Brand);
            }
        }

        [TestMethod]
        public void EstimateRangeTest()
        {
            using (var context = Scenario.SomeCars()) {
                var q = (from c in context.Cars
                         where c.EstimatedValue < 50000 && c.EstimatedAt > new DateTime(2008, 1, 1)
                         select c);

                var cars = q.ToList();
                Assert.AreEqual(1, cars.Count);

                var car = cars.First();
                Assert.AreEqual("MDS800", car.RegistrationNumber);
                Assert.AreEqual(CarBrand.Audi, car.Model.Brand);
            }
        }

        [TestMethod]
        public void GreaterThanNestedPropertyTest()
        {
            using (var context = Scenario.SomeCars())
            {
                var q = (from c in context.Cars
                         where c.Model.Year < 1990 && c.Model.Name == "Fiesta"
                         select c);

                var car = q.First();
                Assert.AreEqual("NDN022", car.RegistrationNumber);
                Assert.AreEqual(CarBrand.Ford, car.Model.Brand);
            }
        }

        [TestMethod]
        public void ContainsTest()
        {

            using (var context = Scenario.SomeCars())
            {
                var years = new List<int> {1988, 2000, 2001};
                var q = (from c in context.Cars
                         where years.Contains(c.Model.Year)
                         select c);

                var cars = q.ToArray();
                Assert.AreEqual(2, cars.Length);
                var first = cars.First();
                var second = cars.Skip(1).First();

                Assert.AreEqual(2001, first.Model.Year);
                Assert.AreEqual(1988, second.Model.Year);
            }

        }

        [TestMethod]
        public void AnyTest()
        {

            using (var context = Scenario.SomeCars())
            {
                var years = new List<int> { 1988, 2000, 2001 };
                var q = (from c in context.Cars
                         where years.Any(y => y == c.Model.Year)
                         select c);

                var cars = q.ToArray();
                Assert.AreEqual(2, cars.Length);
                var first = cars.First();
                var second = cars.Skip(1).First();

                Assert.AreEqual(2001, first.Model.Year);
                Assert.AreEqual(1988, second.Model.Year);
            }

        }

        [TestMethod]
        public void AnyAnyTest()
        {

            using (var context = Scenario.SomeCars())
            {
                var sizes = new List<double> { 1.8 };
                var q = (from c in context.Cars
                         where c.Compartments.Any(cp => sizes.Any(s => s == cp.SquareMeters))
                         select c);

                var cars = q.ToArray();
                Assert.AreEqual(1, cars.Length);
                var first = cars.First();

                Assert.AreEqual("NDN022", first.RegistrationNumber);
            }

        }

    }
}
