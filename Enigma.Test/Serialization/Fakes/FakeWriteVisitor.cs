using System;
using Enigma.Serialization;

namespace Enigma.Test.Serialization.Fakes
{

    
    public class FakeWriteVisitor : IWriteVisitor
    {
        private readonly WriteStatistics _statistics = new WriteStatistics();

        public WriteStatistics Statistics
        {
            get { return _statistics; }
        }

        public void Visit(object level, VisitArgs args)
        {
            Statistics.VisitCount++;
            _statistics.AckVisited(args);
        }

        public void Leave(object level, VisitArgs args)
        {
            Statistics.LeaveCount++;
        }

        public void VisitValue(byte? value, VisitArgs args)
        {
            Statistics.VisitByteCount++;
            _statistics.AckVisited(args);
        }

        public void VisitValue(short? value, VisitArgs args)
        {
            Statistics.VisitInt16Count++;
            _statistics.AckVisited(args);
        }

        public void VisitValue(int? value, VisitArgs args)
        {
            Statistics.VisitInt32Count++;
            _statistics.AckVisited(args);
        }

        public void VisitValue(long? value, VisitArgs args)
        {
            Statistics.VisitInt64Count++;
            _statistics.AckVisited(args);
        }

        public void VisitValue(ushort? value, VisitArgs args)
        {
            Statistics.VisitUInt16Count++;
            _statistics.AckVisited(args);
        }

        public void VisitValue(uint? value, VisitArgs args)
        {
            Statistics.VisitUInt32Count++;
            _statistics.AckVisited(args);
        }

        public void VisitValue(ulong? value, VisitArgs args)
        {
            Statistics.VisitUInt64Count++;
            _statistics.AckVisited(args);
        }

        public void VisitValue(bool? value, VisitArgs args)
        {
            Statistics.VisitBooleanCount++;
            _statistics.AckVisited(args);
        }

        public void VisitValue(float? value, VisitArgs args)
        {
            Statistics.VisitSingleCount++;
            _statistics.AckVisited(args);
        }

        public void VisitValue(double? value, VisitArgs args)
        {
            Statistics.VisitDoubleCount++;
            _statistics.AckVisited(args);
        }

        public void VisitValue(decimal? value, VisitArgs args)
        {
            Statistics.VisitDecimalCount++;
            _statistics.AckVisited(args);
        }

        public void VisitValue(TimeSpan? value, VisitArgs args)
        {
            Statistics.VisitTimeSpanCount++;
            _statistics.AckVisited(args);
        }

        public void VisitValue(DateTime? value, VisitArgs args)
        {
            Statistics.VisitDateTimeCount++;
            _statistics.AckVisited(args);
        }

        public void VisitValue(string value, VisitArgs args)
        {
            Statistics.VisitStringCount++;
            _statistics.AckVisited(args);
        }

        public void VisitValue(Guid? value, VisitArgs args)
        {
            Statistics.VisitGuidCount++;
            _statistics.AckVisited(args);
        }

        public void VisitValue(byte[] value, VisitArgs args)
        {
            Statistics.VisitBlobCount++;
            _statistics.AckVisited(args);
        }

    }
}
