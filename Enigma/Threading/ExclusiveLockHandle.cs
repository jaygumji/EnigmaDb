using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Enigma.Threading
{
    class ExclusiveLockHandle : ILockHandle
    {
        private readonly ExclusiveLock _lock;

        public ExclusiveLockHandle(ExclusiveLock @lock)
        {
            _lock = @lock;
        }

        public void Dispose()
        {
            _lock.ExitExclusive();
        }
    }
}
