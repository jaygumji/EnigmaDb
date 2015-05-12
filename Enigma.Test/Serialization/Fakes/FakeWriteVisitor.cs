using System;
using Enigma.Serialization;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Enigma.Test.Serialization.Fakes
{
    public class FakeWriteVisitor : IWriteVisitor
    {

        public int VisitCount { get; private set; }
        public int LeaveCount { get; private set; }

        public int VisitByteCount { get; private set; }
        public int VisitInt16Count { get; private set; }
        public int VisitInt32Count { get; private set; }
        public int VisitInt64Count { get; private set; }
        public int VisitUInt16Count { get; private set; }
        public int VisitUInt32Count { get; private set; }
        public int VisitUInt64Count { get; private set; }
        public int VisitBooleanCount { get; private set; }
        public int VisitSingleCount { get; private set; }
        public int VisitDoubleCount { get; private set; }
        public int VisitDecimalCount { get; private set; }
        public int VisitTimeSpanCount { get; private set; }
        public int VisitDateTimeCount { get; private set; }
        public int VisitStringCount { get; private set; }
        public int VisitGuidCount { get; private set; }
        public int VisitBlobCount { get; private set; }
        
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

        public void Visit(object level, VisitArgs args)
        {
            VisitCount++;
        }

        public void Leave(object level, VisitArgs args)
        {
            LeaveCount++;
        }

        public void VisitValue(byte? value, VisitArgs args)
        {
            VisitByteCount++;
        }

        public void VisitValue(short? value, VisitArgs args)
        {
            VisitInt16Count++;
        }

        public void VisitValue(int? value, VisitArgs args)
        {
            VisitInt32Count++;
        }

        public void VisitValue(long? value, VisitArgs args)
        {
            VisitInt64Count++;
        }

        public void VisitValue(ushort? value, VisitArgs args)
        {
            VisitUInt16Count++;
        }

        public void VisitValue(uint? value, VisitArgs args)
        {
            VisitUInt32Count++;
        }

        public void VisitValue(ulong? value, VisitArgs args)
        {
            VisitUInt64Count++;
        }

        public void VisitValue(bool? value, VisitArgs args)
        {
            VisitBooleanCount++;
        }

        public void VisitValue(float? value, VisitArgs args)
        {
            VisitSingleCount++;
        }

        public void VisitValue(double? value, VisitArgs args)
        {
            VisitDoubleCount++;
        }

        public void VisitValue(decimal? value, VisitArgs args)
        {
            VisitDecimalCount++;
        }

        public void VisitValue(TimeSpan? value, VisitArgs args)
        {
            VisitTimeSpanCount++;
        }

        public void VisitValue(DateTime? value, VisitArgs args)
        {
            VisitDateTimeCount++;
        }

        public void VisitValue(string value, VisitArgs args)
        {
            VisitStringCount++;
        }

        public void VisitValue(Guid? value, VisitArgs args)
        {
            VisitGuidCount++;
        }

        public void VisitValue(byte[] value, VisitArgs args)
        {
            VisitBlobCount++;
        }

        public void AssertHiearchy()
        {
            Assert.AreEqual(VisitCount, LeaveCount);
        }

    }
}
