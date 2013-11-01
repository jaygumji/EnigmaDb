using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace Enigma.Threading
{
    public class BackgroundQueue : IDisposable
    {
        private readonly Thread _thread;
        private readonly SortedList<DateTime, CompositeBackgroundTask> _scheduledTasks;
        private readonly List<IBackgroundTask> _tasks; 
        private readonly AutoResetEvent _event;

        private DateTime _nextTaskAt;
        private bool _continue;

        /// <summary>
        /// How long the background queue remains idle before being forced to check the queue 
        /// </summary>
        /// <remarks>
        /// <para>This is to ensure that the queue never misses anything.
        /// The event should make sure that we actually take care of everything
        /// without forcing a control, but call us paranoid.</para>
        /// <para>Set this to Times.Infinite to have it wait indefinetly</para>
        /// </remarks>
        public TimeSpan MaxIdleTime { get; set; }

        /// <summary>
        /// Create a new instance of <see cref="BackgroundQueue"/>
        /// </summary>
        public BackgroundQueue()
        {
            _thread = new Thread(ThreadRun);
            _scheduledTasks = new SortedList<DateTime, CompositeBackgroundTask>();
            _tasks = new List<IBackgroundTask>();
            _event = new AutoResetEvent(false);

            MaxIdleTime = TimeSpan.FromMinutes(5);
        }

        public void Start()
        {
            _thread.Start();
        }

        public void Stop()
        {
            _continue = false;
            _event.Set();
        }

        public void Enqueue(InvokeHandler invoker)
        {
            Enqueue(new BackgroundTask {Invoker = invoker});
        }

        public void Enqueue(InvokeHandler invoker, DateTime dueAt)
        {
            Enqueue(new BackgroundTask { Invoker = invoker }, dueAt);
        }

        public void Enqueue(IBackgroundTask task)
        {
            lock (_tasks)
                _tasks.Add(task);

            UpdateBackgroundWorker();
        }

        public void Enqueue(IBackgroundTask task, DateTime dueAt)
        {
            if (dueAt <= DateTime.Now) {
                Enqueue(task);
                return;
            }

            CompositeBackgroundTask compositeTask;
            lock (_scheduledTasks) {
                if (!_scheduledTasks.TryGetValue(dueAt, out compositeTask)) {
                    compositeTask = new CompositeBackgroundTask();
                    _scheduledTasks.Add(dueAt, compositeTask);
                }
            }
            compositeTask.Add(task);
            UpdateBackgroundWorker(dueAt);
        }

        private void UpdateBackgroundWorker()
        {
            _event.Set();
        }

        private void UpdateBackgroundWorker(DateTime triggersAt)
        {
            if (triggersAt < _nextTaskAt) {
                _event.Set();
                _nextTaskAt = triggersAt;
            }
        }

        private void ThreadRun(object obj)
        {
            while (_continue){
                if (_scheduledTasks.Count == 0) {
                    _event.WaitOne(MaxIdleTime);
                    continue;
                }

                var now = DateTime.Now;
                var timeToNext = _nextTaskAt.Subtract(now);
                if (timeToNext > TimeSpan.Zero) {
                    _event.WaitOne(timeToNext > MaxIdleTime ? MaxIdleTime : timeToNext);
                    continue;
                }

                foreach (var task in DequeueTasks())
                    task.Invoke();

                var dueTasks = GetDueTasks(now);
                foreach (var task in dueTasks)
                    task.Invoke();
            }
        }

        private IEnumerable<IBackgroundTask> DequeueTasks()
        {
            lock (_tasks)
                return _tasks.ToList();
        }

        private IEnumerable<IBackgroundTask> GetDueTasks(DateTime dueAt)
        {
            var result = new List<IBackgroundTask>();
            lock (_scheduledTasks) {
                while (true) {
                    if (_scheduledTasks.Count == 0) return result;
                    var taskDueAt = _scheduledTasks.Keys[0];
                    if (taskDueAt > dueAt) return result;
                    result.Add(_scheduledTasks.Values[0]);
                    _scheduledTasks.RemoveAt(0);
                }
            }
        }


        public void Dispose()
        {
            Stop();
            _thread.Join(TimeSpan.FromSeconds(1));
            _thread.Abort();
            _thread.Join(TimeSpan.FromSeconds(1));
        }
    }
}
