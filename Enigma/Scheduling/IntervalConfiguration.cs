using System;

namespace Enigma.Scheduling
{
    public class IntervalConfiguration : IDateTimeConfiguration
    {
        public IDateConfiguration Date { get; set; }
        public TimeSpan Interval { get; set; }

        public DateTime GetNext(DateTime @from)
        {
            var nextAt = from.Add(Interval);
            var nextDate = Date.NextAt(nextAt);
            return nextDate.Date == nextAt.Date
                ? nextAt
                : nextDate.Date;
        }
    }
}