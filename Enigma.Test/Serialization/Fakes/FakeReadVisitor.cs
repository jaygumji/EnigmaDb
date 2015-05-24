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

        private readonly ReadStatistics _statistics;
        private readonly Stack<VisitArgs> _args;

        public int AllowedVisitCount { get; set; }
        public bool ReadOnlyNull { get; set; }

        public FakeReadVisitor()
        {
            AllowedVisitCount = -1;
            _propertyVisitCounts = new Dictionary<string, int>();
            _args = new Stack<VisitArgs>();
            _statistics = new ReadStatistics();
        }

        public ReadStatistics Statistics { get { return _statistics; } }

        public ValueState TryVisit(VisitArgs args)
        {
            _statistics.VisitCount++;
            _statistics.AckVisited(args);

            if (!ShouldRead(args)) return ValueState.NotFound;
            
            _statistics.ExpectedLeaveCount++;
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
            _statistics.LeaveCount++;
        }

        public bool TryVisitValue(VisitArgs args, out byte? value)
        {
            _statistics.AckVisited(args);
            _statistics.VisitByteCount++;
            value = ReadOnlyNull ? (byte?) null : 42;
            return ShouldRead(args);
        }

        public bool TryVisitValue(VisitArgs args, out short? value)
        {
            _statistics.AckVisited(args);
            _statistics.VisitInt16Count++;
            value = ReadOnlyNull ? (short?)null : ++_nextInt16;
            return ShouldRead(args);
        }

        public bool TryVisitValue(VisitArgs args, out int? value)
        {
            _statistics.AckVisited(args);
            _statistics.VisitInt32Count++;
            value = ReadOnlyNull ? (int?)null : ++_nextInt32;
            return ShouldRead(args);
        }

        public bool TryVisitValue(VisitArgs args, out long? value)
        {
            _statistics.AckVisited(args);
            _statistics.VisitInt64Count++;
            value = ReadOnlyNull ? (long?)null : ++_nextInt64;
            return ShouldRead(args);
        }

        public bool TryVisitValue(VisitArgs args, out ushort? value)
        {
            _statistics.AckVisited(args);
            _statistics.VisitUInt16Count++;
            value = ReadOnlyNull ? (ushort?)null : ++_nextUInt16;
            return ShouldRead(args);
        }

        public bool TryVisitValue(VisitArgs args, out uint? value)
        {
            _statistics.AckVisited(args);
            _statistics.VisitUInt32Count++;
            value = ReadOnlyNull ? (uint?)null : ++_nextUInt32;
            return ShouldRead(args);
        }

        public bool TryVisitValue(VisitArgs args, out ulong? value)
        {
            _statistics.AckVisited(args);
            _statistics.VisitUInt64Count++;
            value = ReadOnlyNull ? (ulong?)null : ++_nextUInt64;
            return ShouldRead(args);
        }

        public bool TryVisitValue(VisitArgs args, out bool? value)
        {
            _statistics.AckVisited(args);
            _statistics.VisitBooleanCount++;
            value = ReadOnlyNull ? (bool?)null : true;
            return ShouldRead(args);
        }

        public bool TryVisitValue(VisitArgs args, out float? value)
        {
            _statistics.AckVisited(args);
            _statistics.VisitSingleCount++;
            value = ReadOnlyNull ? (float?)null : 42.3f;
            return ShouldRead(args);
        }

        public bool TryVisitValue(VisitArgs args, out double? value)
        {
            _statistics.AckVisited(args);
            _statistics.VisitDoubleCount++;
            value = ReadOnlyNull ? (double?)null : 42.7d;
            return ShouldRead(args);
        }

        public bool TryVisitValue(VisitArgs args, out decimal? value)
        {
            _statistics.AckVisited(args);
            _statistics.VisitDecimalCount++;
            value = ReadOnlyNull ? (decimal?)null : 42.5563M;
            return ShouldRead(args);
        }

        public bool TryVisitValue(VisitArgs args, out TimeSpan? value)
        {
            _statistics.AckVisited(args);
            _statistics.VisitTimeSpanCount++;
            value = ReadOnlyNull ? (TimeSpan?)null : new TimeSpan(12, 30, 00);
            return ShouldRead(args);
        }

        public bool TryVisitValue(VisitArgs args, out DateTime? value)
        {
            _statistics.AckVisited(args);
            _statistics.VisitDateTimeCount++;
            value = ReadOnlyNull ? (DateTime?)null : new DateTime(2001, 01, 07, 13, 30, 42);
            return ShouldRead(args);
        }

        public bool TryVisitValue(VisitArgs args, out string value)
        {
            _statistics.AckVisited(args);
            _statistics.VisitStringCount++;
            value = ReadOnlyNull ? null : "Hello World - " + Guid.NewGuid();
            return ShouldRead(args);
        }

        public bool TryVisitValue(VisitArgs args, out Guid? value)
        {
            _statistics.AckVisited(args);
            _statistics.VisitGuidCount++;
            value = ReadOnlyNull ? (Guid?)null : Guid.Empty;
            return ShouldRead(args);
        }

        public bool TryVisitValue(VisitArgs args, out byte[] value)
        {
            _statistics.AckVisited(args);
            _statistics.VisitBlobCount++;
            value = ReadOnlyNull ? null : new byte[] { 1, 2, 3 };
            return ShouldRead(args);
        }

    }
}
