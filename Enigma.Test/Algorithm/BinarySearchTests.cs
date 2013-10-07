using Enigma.Binary.Algorithm;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;

namespace Enigma.Test.Algorithm
{
    [TestClass]
    public class BinarySearchTests
    {

        [TestMethod]
        public void SearchTest()
        {

            var list = new List<int> {
                3, 5, 7, 11, 16, 19,
                22, 27, 33, 39, 39, 44
            };

            var index = BinarySearch.Search(list, 3);
            Assert.AreEqual(0, index);

            index = BinarySearch.Search(list, 0);
            Assert.AreEqual(-1, index);
            Assert.AreEqual(0, ~index);

            index = BinarySearch.Search(list, 18);
            Assert.AreEqual(-6, index);

            var closestMatch = ~index;
            Assert.AreEqual(5, closestMatch);
        }

    }
}
