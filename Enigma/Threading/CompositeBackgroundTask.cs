using System.Collections;
using System.Collections.Generic;

namespace Enigma.Threading
{
    public class CompositeBackgroundTask : IBackgroundTask, ICollection<IBackgroundTask>
    {

        private readonly List<IBackgroundTask> _tasks; 

        public CompositeBackgroundTask()
        {
            _tasks = new List<IBackgroundTask>();
        }

        public void Add(IBackgroundTask task)
        {
            _tasks.Add(task);
        }

        public void Clear()
        {
            _tasks.Clear();
        }

        public bool Contains(IBackgroundTask task)
        {
            return _tasks.Contains(task);
        }

        public void CopyTo(IBackgroundTask[] array, int arrayIndex)
        {
            _tasks.CopyTo(array, arrayIndex);
        }

        public bool Remove(IBackgroundTask task)
        {
            return _tasks.Remove(task);
        }

        public int Count { get { return _tasks.Count; } }
        public bool IsReadOnly { get { return false; } }

        public void Invoke()
        {
            foreach (var task in _tasks)
                task.Invoke();
        }

        public IEnumerator<IBackgroundTask> GetEnumerator()
        {
            return _tasks.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}