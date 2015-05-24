using System.Collections.Generic;
using Enigma.Serialization;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Enigma.Test.Serialization.Fakes
{
    public class ReadStatistics : IReadStatistics
    {

        private readonly List<VisitArgs> _visitedArgs;
        
        public int VisitCount { get; set; }
        public int LeaveCount { get; set; }
        public int ExpectedLeaveCount { get; set; }

        public int VisitByteCount { get; set; }
        public int VisitInt16Count { get; set; }
        public int VisitInt32Count { get; set; }
        public int VisitInt64Count { get; set; }
        public int VisitUInt16Count { get; set; }
        public int VisitUInt32Count { get; set; }
        public int VisitUInt64Count { get; set; }
        public int VisitBooleanCount { get; set; }
        public int VisitSingleCount { get; set; }
        public int VisitDoubleCount { get; set; }
        public int VisitDecimalCount { get; set; }
        public int VisitTimeSpanCount { get; set; }
        public int VisitDateTimeCount { get; set; }
        public int VisitStringCount { get; set; }
        public int VisitGuidCount { get; set; }
        public int VisitBlobCount { get; set; }

        public ReadStatistics()
        {
            _visitedArgs = new List<VisitArgs>();
        }

        public int VisitValueCount
        {
            get
            {
                return VisitByteCount
                       + VisitInt16Count + VisitInt32Count + VisitInt64Count
                       + VisitUInt16Count + VisitUInt32Count + VisitUInt64Count
                       + VisitBooleanCount + VisitSingleCount + VisitDoubleCount
                       + VisitDecimalCount + VisitTimeSpanCount + VisitDateTimeCount
                       + VisitStringCount + VisitGuidCount + VisitBlobCount;
            }
        }

        public void AckVisited(VisitArgs args)
        {
            _visitedArgs.Add(args);
        }

        public void AssertHiearchy()
        {
            Assert.AreEqual(ExpectedLeaveCount, LeaveCount);
        }

        public void AssertVisitOrderExact(params LevelType[] expectedLevels)
        {
            Assert.AreEqual(expectedLevels.Length, _visitedArgs.Count, "Visited args count is not equal the expected count");
            AssertVisitOrderBeginsWith(expectedLevels);
        }

        public void AssertVisitOrderBeginsWith(params LevelType[] expectedLevels)
        {
            Assert.IsTrue(expectedLevels.Length <= _visitedArgs.Count, "Visited args count is lesser than the expected count");
            for (var i = 0; i < expectedLevels.Length; i++) {
                var args = _visitedArgs[i];
                var expectedLevel = expectedLevels[i];
                Assert.AreEqual(expectedLevel, args.Type);
            }
        }


    }
}