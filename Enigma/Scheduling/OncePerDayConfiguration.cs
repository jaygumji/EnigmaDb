using System;

namespace Enigma.Scheduling
{
    public class OncePerDayConfiguration : IDateTimeConfiguration
    {
        private TimeSpan _timeOfDay;

        public IDateConfiguration Date { get; set; }

        public TimeSpan TimeOfDay
        {
            get { return _timeOfDay; }
            set
            {
                if (value.Days > 0)
                    throw new InvalidSchedulerConfigurationException("Invalid time of day, day part may not be set");

                if (value < TimeSpan.Zero)
                    throw new InvalidSchedulerConfigurationException("Invalid time of day, must be positive");

                _timeOfDay = value;
            }
        }

        public DateTime GetNext(DateTime @from)
        {
            var nextAt = new DateTime(from.Year, from.Month, from.Day).Add(TimeOfDay);
            if (nextAt > from) return nextAt;

            var nextDate = Date.NextAt(nextAt.AddDays(1));
            nextAt = new DateTime(nextDate.Year, nextDate.Month, nextDate.Day).Add(TimeOfDay);
            return nextAt;
        }
    }
}