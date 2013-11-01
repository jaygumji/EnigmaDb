using System.Threading.Tasks;

namespace Enigma.Threading
{
    public class AsyncBackgroundTask : IBackgroundTask
    {
        public InvokeHandler Invoker { get; set; }

        void IBackgroundTask.Invoke()
        {
            Task.Factory.StartNew(() => Invoker());
        }
    }
}