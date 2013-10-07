using System;
namespace Enigma.Threading
{
    public interface ILock
    {
        ILockHandle Enter();
        bool TryEnter();
        bool TryEnter(TimeSpan timeLimit);

        void Exit();
    }

    public interface ILock<TKey>
    {
        ILockHandle Enter(TKey key);
        bool TryEnter(TKey key);
        bool TryEnter(TKey key, TimeSpan timeLimit);

        void Exit(TKey key);
    }
}
