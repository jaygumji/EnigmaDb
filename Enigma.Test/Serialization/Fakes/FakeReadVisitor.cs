using System;
using Enigma.Serialization;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Enigma.Test.Serialization.Fakes
{
    public class FakeReadVisitor : IReadVisitor
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

        public ValueState TryVisit(VisitArgs args)
        {
            VisitCount++;

            return ShouldRead(args)
                ? ValueState.Found
                : ValueState.NotFound;
        }

        private bool ShouldRead(VisitArgs args)
        {
            if (args.Type == LevelType.CollectionItem || args.Type == LevelType.DictionaryKey ||
                args.Type == LevelType.DictionaryValue)
                return false;

            return true;
        }

        public void Leave(VisitArgs args)
        {
            LeaveCount++;
        }

        public bool TryVisitValue(VisitArgs args, out byte? value)
        {
            VisitByteCount++;
            value = 42;
            return ShouldRead(args);
        }

        public bool TryVisitValue(VisitArgs args, out short? value)
        {
            VisitInt16Count++;
            value = 42;
            return ShouldRead(args);
        }

        public bool TryVisitValue(VisitArgs args, out int? value)
        {
            VisitInt32Count++;
            value = 42;
            return ShouldRead(args);
        }

        public bool TryVisitValue(VisitArgs args, out long? value)
        {
            VisitInt64Count++;
            value = 42;
            return ShouldRead(args);
        }

        public bool TryVisitValue(VisitArgs args, out ushort? value)
        {
            VisitUInt16Count++;
            value = 42;
            return ShouldRead(args);
        }

        public bool TryVisitValue(VisitArgs args, out uint? value)
        {
            VisitUInt32Count++;
            value = 42;
            return ShouldRead(args);
        }

        public bool TryVisitValue(VisitArgs args, out ulong? value)
        {
            VisitUInt64Count++;
            value = 42;
            return ShouldRead(args);
        }

        public bool TryVisitValue(VisitArgs args, out bool? value)
        {
            VisitBooleanCount++;
            value = true;
            return ShouldRead(args);
        }

        public bool TryVisitValue(VisitArgs args, out float? value)
        {
            VisitSingleCount++;
            value = 42.3f;
            return ShouldRead(args);
        }

        public bool TryVisitValue(VisitArgs args, out double? value)
        {
            VisitDoubleCount++;
            value = 42.7d;
            return ShouldRead(args);
        }

        public bool TryVisitValue(VisitArgs args, out decimal? value)
        {
            VisitDecimalCount++;
            value = 42.5563M;
            return ShouldRead(args);
        }

        public bool TryVisitValue(VisitArgs args, out TimeSpan? value)
        {
            VisitTimeSpanCount++;
            value = new TimeSpan(12, 30, 00);
            return ShouldRead(args);
        }

        public bool TryVisitValue(VisitArgs args, out DateTime? value)
        {
            VisitDateTimeCount++;
            value = new DateTime(2001, 01, 07, 13, 30, 42);
            return ShouldRead(args);
        }

        public bool TryVisitValue(VisitArgs args, out string value)
        {
            VisitStringCount++;
            value = "Hello World";
            return ShouldRead(args);
        }

        public bool TryVisitValue(VisitArgs args, out Guid? value)
        {
            VisitGuidCount++;
            value = Guid.Empty;
            return ShouldRead(args);
        }

        public bool TryVisitValue(VisitArgs args, out byte[] value)
        {
            VisitBlobCount++;
            value = new byte[] {1, 2, 3};
            return ShouldRead(args);
        }

        public void AssertHiearchy()
        {
            Assert.AreEqual(VisitCount, LeaveCount);
        }

    }
}
