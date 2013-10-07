using System;
using System.Threading;
namespace Enigma.Threading
{
    public class ExclusiveLock : ILock, IDisposable
    {
        private readonly ReaderWriterLockSlim _lock = new ReaderWriterLockSlim();

        public ILockHandle Enter()
        {
            _lock.EnterReadLock();
            return new LockHandle(this);
        }

        public bool TryEnter()
        {
            return _lock.TryEnterReadLock(TimeSpan.Zero);
        }

        public bool TryEnter(System.TimeSpan timeLimit)
        {
            return _lock.TryEnterReadLock(timeLimit);
        }

        public void Exit()
        {
            _lock.ExitReadLock();
        }

        public ILockHandle EnterExclusive()
        {
            _lock.EnterWriteLock();
            return new ExclusiveLockHandle(this);
        }

        public bool TryEnterExclusive()
        {
            return _lock.TryEnterWriteLock(TimeSpan.Zero);
        }

        public bool TryEnterExclusive(TimeSpan timeLimit)
        {
            return _lock.TryEnterWriteLock(timeLimit);
        }

        public void ExitExclusive()
        {
            _lock.ExitWriteLock();
        }

        public void Dispose()
        {
            _lock.Dispose();
        }
    }
}
