using System;
using System.Collections.Generic;
using Enigma.Serialization;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Enigma.Test.Serialization.Fakes
{
    public class FakeReadVisitor : IReadVisitor
    {

        private readonly int _allowedVisitCount;
        private readonly Dictionary<VisitArgs, int> _propertyVisitCounts;

        private Int16 _nextInt16;
        private Int32 _nextInt32;
        private Int64 _nextInt64;
        private UInt16 _nextUInt16;
        private UInt32 _nextUInt32;
        private UInt64 _nextUInt64;

        public int VisitCount { get; private set; }
        public int LeaveCount { get; private set; }
        private int ExpectedLeaveCount { get; set; }

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

        public FakeReadVisitor() : this(-1)
        {
        }

        public FakeReadVisitor(int allowedVisitCount)
        {
            _allowedVisitCount = allowedVisitCount;
            _propertyVisitCounts = new Dictionary<VisitArgs, int>();
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

        public ValueState TryVisit(VisitArgs args)
        {
            VisitCount++;

            if (!ShouldRead(args)) return ValueState.NotFound;
            
            ExpectedLeaveCount++;
            return ValueState.Found;
        }

        private bool ShouldRead(VisitArgs args)
        {
            var visitCount = 0;
            if (_propertyVisitCounts.ContainsKey(args))
                visitCount = ++_propertyVisitCounts[args];
            else
                _propertyVisitCounts.Add(args, ++visitCount);

            if (args.Type == LevelType.CollectionItem || args.Type == LevelType.DictionaryKey ||
                args.Type == LevelType.DictionaryValue)
                return _allowedVisitCount < 0
                    ? (visitCount % 2) == 1
                    : visitCount <= _allowedVisitCount;

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
            value = ++_nextInt16;
            return ShouldRead(args);
        }

        public bool TryVisitValue(VisitArgs args, out int? value)
        {
            VisitInt32Count++;
            value = ++_nextInt32;
            return ShouldRead(args);
        }

        public bool TryVisitValue(VisitArgs args, out long? value)
        {
            VisitInt64Count++;
            value = ++_nextInt64;
            return ShouldRead(args);
        }

        public bool TryVisitValue(VisitArgs args, out ushort? value)
        {
            VisitUInt16Count++;
            value = ++_nextUInt16;
            return ShouldRead(args);
        }

        public bool TryVisitValue(VisitArgs args, out uint? value)
        {
            VisitUInt32Count++;
            value = ++_nextUInt32;
            return ShouldRead(args);
        }

        public bool TryVisitValue(VisitArgs args, out ulong? value)
        {
            VisitUInt64Count++;
            value = ++_nextUInt64;
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
            Assert.AreEqual(ExpectedLeaveCount, LeaveCount);
        }

    }
}
