using System.Collections.Generic;
using System.Linq;
using Enigma.Store;
using Enigma.Store.Indexes;
using Enigma.Store.Keys;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Enigma.Test.Index
{
    [TestClass]
    public class ComparableIndexTests
    {

        private static IComparableIndex<int> CreateIndex()
        {
            var index = new ComparableIndex<int>("MyTest");
            index.Update(new ImmutableSortedCollection<int, IList<IKey>>(new SortedList<int, IList<IKey>> {
                {0, new List<IKey> {
                    new StringKey("Value1")
                }},
                {1, new List<IKey> {
                    new StringKey("Value1"),
                    new StringKey("Value2")
                }},
                {2, new List<IKey> {
                    new StringKey("Value3")
                }},
                {3, new List<IKey> {
                    new StringKey("Value2"),
                    new StringKey("Value4")
                }},
                {6, new List<IKey> {
                    new StringKey("Value2")
                }},
                {7, new List<IKey> {
                    new StringKey("Value5")
                }},
                {8, new List<IKey> {
                    new StringKey("Value3")
                }},
                {10, new List<IKey> {
                    new StringKey("Value1"),
                    new StringKey("Value5"),
                    new StringKey("Value6")
                }},
                {14, new List<IKey> {
                    new StringKey("Value7")
                }},
            }));
            return index;
        }

        [TestMethod]
        public void EqualTest()
        {
            var index = CreateIndex();

            var values = index.Equal(10).ToList();
            Assert.AreEqual(3, values.Count);
            Assert.AreEqual(new StringKey("Value1"), values.First());
        }

        [TestMethod]
        public void NotEqualTest()
        {
            var index = CreateIndex();

            var values = index.NotEqual(14).ToList();
            Assert.AreEqual(6, values.Count);
            Assert.AreEqual(new StringKey("Value1"), values.First());
            Assert.AreEqual(new StringKey("Value6"), values.Last());
        }

        [TestMethod]
        public void GreaterThanTest()
        {
            var index = CreateIndex();

            var values = index.GreaterThan(7).ToList();
            Assert.AreEqual(5, values.Count);
            Assert.AreEqual(new StringKey("Value3"), values.First());
            Assert.AreEqual(new StringKey("Value7"), values.Last());
        }

        [TestMethod]
        public void GreaterThanOrEqualTest()
        {
            var index = CreateIndex();

            var values = index.GreaterThanOrEqual(7).ToList();
            Assert.AreEqual(5, values.Count);
            Assert.AreEqual(new StringKey("Value5"), values.First());
            Assert.AreEqual(new StringKey("Value7"), values.Last());
        }

        [TestMethod]
        public void LessThanTest()
        {
            var index = CreateIndex();

            var values = index.LessThan(2).ToList();
            Assert.AreEqual(2, values.Count);
            Assert.AreEqual(new StringKey("Value1"), values.First());
            Assert.AreEqual(new StringKey("Value2"), values.Last());
        }

        [TestMethod]
        public void LessThanOrEqualTest()
        {
            var index = CreateIndex();

            var values = index.LessThanOrEqual(2).ToList();
            Assert.AreEqual(3, values.Count);
            Assert.AreEqual(new StringKey("Value1"), values.First());
            Assert.AreEqual(new StringKey("Value3"), values.Last());
        }

        [TestMethod]
        public void ContainsTest()
        {
            var index = CreateIndex();

            var values = index.Contains(new [] {1,2,3}).ToList();
            Assert.AreEqual(4, values.Count);
            Assert.AreEqual(new StringKey("Value1"), values.First());
            Assert.AreEqual(new StringKey("Value4"), values.Last());
        }

    }
}
