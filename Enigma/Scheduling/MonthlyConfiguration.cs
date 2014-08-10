using System;
using System.Collections.Generic;

namespace Enigma.Scheduling
{
    public class MonthlyConfiguration : IDateConfiguration
    {
        private readonly List<byte> _daysInMonth;

        public MonthlyConfiguration()
        {
            _daysInMonth = new List<byte>();
        }

        public List<byte> DaysInMonth
        {
            get { return _daysInMonth; }
        }

        public DateTime NextAt(DateTime @from)
        {
            var day = (byte) from.Day;
            if (_daysInMonth.Contains(day))
                return from;

            var dt = from;
            for (var i = 0; i < 61; i++) {
                dt = dt.AddDays(1);
                day = (byte)dt.Day;
                if (_daysInMonth.Contains(day))
                    return dt.Date;
            }

            throw new InvalidSchedulerConfigurationException("Could not find the next valid date of this daily configuration");
        }
    }
}