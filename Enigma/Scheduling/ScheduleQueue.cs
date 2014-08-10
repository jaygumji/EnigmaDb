using System;
using System.Collections.Generic;

namespace Enigma.Scheduling
{
    public class ScheduleQueue
    {

        private readonly DateTimeQueue<IScheduledEntry> _entries;

        public ScheduleQueue()
        {
            _entries = new DateTimeQueue<IScheduledEntry>();
        }

        public void Enqueue(IScheduledEntry entry)
        {
            _entries.Enqueue(entry.When.GetNext(DateTime.Now), entry);
        }

        public bool TryDequeue(out IEnumerable<IScheduledEntry> entries)
        {
            return _entries.TryDequeue(out entries);
        }

        public bool TryPeekNextEntryAt(out DateTime nextAt)
        {
            return _entries.TryPeekNextEntryAt(out nextAt);
        }

    }
}
