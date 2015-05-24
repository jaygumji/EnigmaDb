using Enigma.Serialization;

namespace Enigma.Test.Serialization.Fakes
{
    public interface IReadStatistics
    {
        int VisitCount { get; }
        int LeaveCount { get; }
        int VisitByteCount { get; }
        int VisitInt16Count { get; }
        int VisitInt32Count { get; }
        int VisitInt64Count { get; }
        int VisitUInt16Count { get; }
        int VisitUInt32Count { get; }
        int VisitUInt64Count { get; }
        int VisitBooleanCount { get; }
        int VisitSingleCount { get; }
        int VisitDoubleCount { get; }
        int VisitDecimalCount { get; }
        int VisitTimeSpanCount { get; }
        int VisitDateTimeCount { get; }
        int VisitStringCount { get; }
        int VisitGuidCount { get; }
        int VisitBlobCount { get; }

        int VisitValueCount { get; }

        void AssertVisitOrderExact(params LevelType[] expectedLevels);
        void AssertVisitOrderBeginsWith(params LevelType[] expectedLevels);
    }
}