namespace Enigma.Threading
{
    public class BackgroundTask : IBackgroundTask
    {
        public InvokeHandler Invoker { get; set; }

        void IBackgroundTask.Invoke()
        {
            Invoker();
        }
    }
}
