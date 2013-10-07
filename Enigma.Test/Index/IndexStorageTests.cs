using System.Linq;
using Enigma.Store.Indexes;
using Enigma.Store.Keys;
using Enigma.Store.Memory;
using Enigma.Test.TestDb;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Enigma.Test.Index
{
    [TestClass]
    public class IndexStorageTests
    {

        private class TestContext
        {
            public MemoryBinaryStore Store { get; set; }
            public IndexStorage<int> Storage { get; set; } 
        }

        private TestContext CreateTarget()
        {
            var store = new MemoryBinaryStore();
            return CreateTarget(store);
        }

        private TestContext CreateTarget(MemoryBinaryStore store)
        {
            var configuration = new IndexConfiguration {
                EntityType = typeof (Car),
                Type = typeof (int),
                UniqueName = "Nationality"
            };
            var storage = new IndexStorage<int>(store, configuration);
            storage.Initialize();
            return new TestContext {Store = store, Storage = storage};
        }

        [TestMethod]
        public void AddEntityTest()
        {
            var context = CreateTarget();
            
            var car = new Car {
                RegistrationNumber = "AAA000",
                Nationality = Nationality.Sweden
            };

            context.Storage.Add(new StringKey(car.RegistrationNumber), car);
            context.Storage.CommitModifications();

            var actual = context.Storage.Match(CompareOperation.Equal, (int)Nationality.Sweden).ToList();
            Assert.AreEqual(1, actual.Count);
            Assert.AreEqual(new StringKey(car.RegistrationNumber), actual.First());

            var store = new MemoryBinaryStore(context.Store.ToArray());
            context = CreateTarget(store);

            actual = context.Storage.Match(CompareOperation.Equal, (int)Nationality.Sweden).ToList();
            Assert.AreEqual(1, actual.Count);
            Assert.AreEqual(new StringKey(car.RegistrationNumber), actual.First());
        }

        [TestMethod]
        public void UpdateEntityTest()
        {
            var context = CreateTarget();

            var car = new Car {
                RegistrationNumber = "AAA000",
                Nationality = Nationality.Sweden
            };

            context.Storage.Add(new StringKey(car.RegistrationNumber), car);
            context.Storage.CommitModifications();

            var actual = context.Storage.Match(CompareOperation.Equal, (int) Nationality.Sweden).ToList();
            Assert.AreEqual(1, actual.Count);
            Assert.AreEqual(new StringKey(car.RegistrationNumber), actual.First());

            car.Nationality = Nationality.Denmark;
            context.Storage.Update(new StringKey(car.RegistrationNumber), car);
            context.Storage.CommitModifications();

            actual = context.Storage.Match(CompareOperation.Equal, (int)Nationality.Sweden).ToList();
            Assert.AreEqual(0, actual.Count);

            actual = context.Storage.Match(CompareOperation.Equal, (int)Nationality.Denmark).ToList();
            Assert.AreEqual(1, actual.Count);
            Assert.AreEqual(new StringKey(car.RegistrationNumber), actual.First());
        }

        [TestMethod]
        public void RemoveEntityTest()
        {
            var context = CreateTarget();

            var car = new Car {
                RegistrationNumber = "AAA000",
                Nationality = Nationality.Sweden
            };

            context.Storage.Add(new StringKey(car.RegistrationNumber), car);
            context.Storage.CommitModifications();

            var actual = context.Storage.Match(CompareOperation.Equal, (int)Nationality.Sweden).ToList();
            Assert.AreEqual(1, actual.Count);
            Assert.AreEqual(new StringKey(car.RegistrationNumber), actual.First());

            car.Nationality = Nationality.Denmark;
            context.Storage.Remove(new StringKey(car.RegistrationNumber));
            context.Storage.CommitModifications();

            actual = context.Storage.Match(CompareOperation.Equal, (int)Nationality.Sweden).ToList();
            Assert.AreEqual(0, actual.Count);
        }

    }
}
