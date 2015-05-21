using System;
using System.Collections.Generic;
using Enigma.Serialization;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Enigma.Test.Serialization.Fakes
{
    public class FakeReadVisitor : IReadVisitor
    {

        private readonly Dictionary<string, int> _propertyVisitCounts;

        private Int16 _nextInt16;
        private Int32 _nextInt32;
        private Int64 _nextInt64;
        private UInt16 _nextUInt16;
        private UInt32 _nextUInt32;
        private UInt64 _nextUInt64;

        private readonly Stack<VisitArgs> _args; 

        public int AllowedVisitCount { get; set; }
        public bool ReadOnlyNull { get; set; }

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

        public FakeReadVisitor()
        {
            AllowedVisitCount = -1;
            _propertyVisitCounts = new Dictionary<string, int>();
            _args = new Stack<VisitArgs>();
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
            _args.Push(args);
            return ValueState.Found;
        }

        private static readonly HashSet<LevelType> EnumerationLevelTypes = new HashSet<LevelType> {
            LevelType.CollectionItem,
            LevelType.CollectionInCollection,
            LevelType.CollectionInDictionaryKey,
            LevelType.CollectionInDictionaryValue,
            LevelType.DictionaryKey,
            LevelType.DictionaryValue,
            LevelType.DictionaryInCollection,
            LevelType.DictionaryInDictionaryKey,
            LevelType.DictionaryInDictionaryValue
        }; 
        private bool ShouldRead(VisitArgs args)
        {
            var key = _args.Count == 0 ? args.Name : string.Concat(_args.Peek().Name, "---", args.Name);
            var visitCount = 0;
            if (_propertyVisitCounts.ContainsKey(key))
                visitCount = ++_propertyVisitCounts[key];
            else
                _propertyVisitCounts.Add(key, ++visitCount);

            if (EnumerationLevelTypes.Contains(args.Type)) {

                if (AllowedVisitCount < 0) {
                    var isValid = (visitCount % 2) == 1;
                    //if (ParentIsNestedCollection())
                    //    isValid = !isValid;
                    return isValid;
                }

                return visitCount <= AllowedVisitCount;
            }

            return true;
        }

        private bool ParentIsNestedCollection()
        {
            var parent = _args.Peek();
            switch (parent.Type) {
                case LevelType.CollectionInCollection:
                case LevelType.CollectionInDictionaryKey:
                case LevelType.CollectionInDictionaryValue:
                case LevelType.DictionaryInCollection:
                case LevelType.DictionaryInDictionaryKey:
                case LevelType.DictionaryInDictionaryValue:
                    return true;
            }
            return false;
        }

        public void Leave(VisitArgs args)
        {
            var expectedArgs = _args.Pop();
            Assert.AreEqual(expectedArgs, args);
            LeaveCount++;
        }

        public bool TryVisitValue(VisitArgs args, out byte? value)
        {
            VisitByteCount++;
            value = ReadOnlyNull ? (byte?) null : 42;
            return ShouldRead(args);
        }

        public bool TryVisitValue(VisitArgs args, out short? value)
        {
            VisitInt16Count++;
            value = ReadOnlyNull ? (short?)null : ++_nextInt16;
            return ShouldRead(args);
        }

        public bool TryVisitValue(VisitArgs args, out int? value)
        {
            VisitInt32Count++;
            value = ReadOnlyNull ? (int?)null : ++_nextInt32;
            return ShouldRead(args);
        }

        public bool TryVisitValue(VisitArgs args, out long? value)
        {
            VisitInt64Count++;
            value = ReadOnlyNull ? (long?)null : ++_nextInt64;
            return ShouldRead(args);
        }

        public bool TryVisitValue(VisitArgs args, out ushort? value)
        {
            VisitUInt16Count++;
            value = ReadOnlyNull ? (ushort?)null : ++_nextUInt16;
            return ShouldRead(args);
        }

        public bool TryVisitValue(VisitArgs args, out uint? value)
        {
            VisitUInt32Count++;
            value = ReadOnlyNull ? (uint?)null : ++_nextUInt32;
            return ShouldRead(args);
        }

        public bool TryVisitValue(VisitArgs args, out ulong? value)
        {
            VisitUInt64Count++;
            value = ReadOnlyNull ? (ulong?)null : ++_nextUInt64;
            return ShouldRead(args);
        }

        public bool TryVisitValue(VisitArgs args, out bool? value)
        {
            VisitBooleanCount++;
            value = ReadOnlyNull ? (bool?)null : true;
            return ShouldRead(args);
        }

        public bool TryVisitValue(VisitArgs args, out float? value)
        {
            VisitSingleCount++;
            value = ReadOnlyNull ? (float?)null : 42.3f;
            return ShouldRead(args);
        }

        public bool TryVisitValue(VisitArgs args, out double? value)
        {
            VisitDoubleCount++;
            value = ReadOnlyNull ? (double?)null : 42.7d;
            return ShouldRead(args);
        }

        public bool TryVisitValue(VisitArgs args, out decimal? value)
        {
            VisitDecimalCount++;
            value = ReadOnlyNull ? (decimal?)null : 42.5563M;
            return ShouldRead(args);
        }

        public bool TryVisitValue(VisitArgs args, out TimeSpan? value)
        {
            VisitTimeSpanCount++;
            value = ReadOnlyNull ? (TimeSpan?)null : new TimeSpan(12, 30, 00);
            return ShouldRead(args);
        }

        public bool TryVisitValue(VisitArgs args, out DateTime? value)
        {
            VisitDateTimeCount++;
            value = ReadOnlyNull ? (DateTime?)null : new DateTime(2001, 01, 07, 13, 30, 42);
            return ShouldRead(args);
        }

        public bool TryVisitValue(VisitArgs args, out string value)
        {
            VisitStringCount++;
            value = ReadOnlyNull ? null : "Hello World - " + Guid.NewGuid();
            return ShouldRead(args);
        }

        public bool TryVisitValue(VisitArgs args, out Guid? value)
        {
            VisitGuidCount++;
            value = ReadOnlyNull ? (Guid?)null : Guid.Empty;
            return ShouldRead(args);
        }

        public bool TryVisitValue(VisitArgs args, out byte[] value)
        {
            VisitBlobCount++;
            value = ReadOnlyNull ? null : new byte[] { 1, 2, 3 };
            return ShouldRead(args);
        }

        public void AssertHiearchy()
        {
            Assert.AreEqual(ExpectedLeaveCount, LeaveCount);
        }

    }
}
