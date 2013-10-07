namespace Enigma.Threading
{
    class LockHandle : ILockHandle
    {
        private readonly ILock _lock;

        public LockHandle(ILock @lock)
        {
            _lock = @lock;
        }

        public void Dispose()
        {
            _lock.Exit();
        }
    }

    class LockHandle<TKey> : ILockHandle
    {
        private readonly ILock<TKey> _lock;
        private readonly TKey _key;

        public LockHandle(ILock<TKey> @lock, TKey key)
        {
            _lock = @lock;
            _key = key;
        }

        public void Dispose()
        {
            _lock.Exit(_key);
        }
    }

}
