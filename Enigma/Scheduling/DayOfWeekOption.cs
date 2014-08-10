using System;

namespace Enigma.Scheduling
{
    [Flags]
    public enum DayOfWeekOption
    {
        None = 0,
        Monday = 1,
        Tuesday = 2,
        Wednesday = 4,
        Thursday = 8,
        Friday = 16,
        Saturday = 32,
        Sunday = 64,

        // Enum flags for easy usage
        Weekdays = 31,
        Weekend = 96,
        AllDays = 127
    }

    public static class DayOfWeekOptionExtensions
    {
        public static bool Contains(this DayOfWeekOption option, DayOfWeek dayOfWeek)
        {
            switch (dayOfWeek) {
                case DayOfWeek.Monday:
                    return (option & DayOfWeekOption.Monday) != DayOfWeekOption.None;
                case DayOfWeek.Tuesday:
                    return (option & DayOfWeekOption.Tuesday) != DayOfWeekOption.None;
                case DayOfWeek.Wednesday:
                    return (option & DayOfWeekOption.Wednesday) != DayOfWeekOption.None;
                case DayOfWeek.Thursday:
                    return (option & DayOfWeekOption.Thursday) != DayOfWeekOption.None;
                case DayOfWeek.Friday:
                    return (option & DayOfWeekOption.Friday) != DayOfWeekOption.None;
                case DayOfWeek.Saturday:
                    return (option & DayOfWeekOption.Saturday) != DayOfWeekOption.None;
                case DayOfWeek.Sunday:
                    return (option & DayOfWeekOption.Sunday) != DayOfWeekOption.None;
            }
            throw new InvalidOperationException("Invalid day of week");
        }
    }
}