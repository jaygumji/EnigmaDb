namespace Enigma.Scheduling
{
    public interface IScheduledEntry
    {
        IDateTimeConfiguration When { get; }

        void Execute();
    }
}