using System;
using System.Collections.Generic;
using System.Threading;

namespace Enigma.Threading
{
    /// <summary>
    /// Manages a queue of work in the background with FIFO
    /// </summary>
    public class BackgroundQueue : IDisposable
    {
        private readonly Thread _thread;
        private readonly SortedList<DateTime, CompositeBackgroundTask> _scheduledTasks;
        private readonly AutoResetEvent _event;
        private readonly ManualResetEventSlim _idleEvent;
        private readonly object _tasksLock = new object();
        private List<IBackgroundTask> _tasks; 
        
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
        /// Initializes a new instance of the <see cref="BackgroundQueue"/> class.
        /// </summary>
        public BackgroundQueue()
        {
            _thread = new Thread(ThreadRun);
            _scheduledTasks = new SortedList<DateTime, CompositeBackgroundTask>();
            _tasks = new List<IBackgroundTask>();
            _event = new AutoResetEvent(false);
            _idleEvent = new ManualResetEventSlim(true);
            
            MaxIdleTime = TimeSpan.FromMinutes(5);
            _continue = true;
        }

        private void Stop()
        {
            _continue = false;
            _event.Set();
        }

        /// <summary>
        /// Enqueues the specified invoker.
        /// </summary>
        /// <param name="invoker">The invoker.</param>
        public void Enqueue(InvokeHandler invoker)
        {
            Enqueue(new BackgroundTask {Invoker = invoker});
        }

        /// <summary>
        /// Enqueues the specified invoker.
        /// </summary>
        /// <param name="invoker">The invoker.</param>
        /// <param name="dueAt">The due at.</param>
        public void Enqueue(InvokeHandler invoker, DateTime dueAt)
        {
            Enqueue(new BackgroundTask { Invoker = invoker }, dueAt);
        }

        /// <summary>
        /// Enqueues the specified task.
        /// </summary>
        /// <param name="task">The task.</param>
        public void Enqueue(IBackgroundTask task)
        {
            lock (_tasksLock)
                _tasks.Add(task);

            UpdateBackgroundWorker();
        }

        /// <summary>
        /// Enqueues the specified task.
        /// </summary>
        /// <param name="task">The task.</param>
        /// <param name="dueAt">The due at.</param>
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

        /// <summary>
        /// Waits until idle.
        /// </summary>
        public void WaitUntilIdle()
        {
            _idleEvent.Wait();
        }

        /// <summary>
        /// Waits until idle.
        /// </summary>
        /// <param name="timeout">The timeout.</param>
        public void WaitUntilIdle(TimeSpan timeout)
        {
            _idleEvent.Wait(timeout);
        }

        private void UpdateBackgroundWorker()
        {
            if (_thread.ThreadState == ThreadState.Unstarted)
                _thread.Start();

            _event.Set();
        }

        private void UpdateBackgroundWorker(DateTime triggersAt)
        {
            if (_thread.ThreadState == ThreadState.Unstarted)
                _thread.Start();

            if (triggersAt < _nextTaskAt) {
                _event.Set();
                _nextTaskAt = triggersAt;
            }
        }

        /// <summary>
        /// The main working method of the thread.
        /// </summary>
        /// <param name="obj">An unused object argument.</param>
        private void ThreadRun(object obj)
        {
            _idleEvent.Reset();

            while (_continue){
                if (_tasks.Count > 0) {
                    foreach (var task in DequeueTasks())
                        task.Invoke();
                }

                if (_scheduledTasks.Count == 0) {
                    _idleEvent.Set();
                    _event.WaitOne(MaxIdleTime);
                    _idleEvent.Reset();
                    continue;
                }

                var now = DateTime.Now;
                var timeToNext = _nextTaskAt.Subtract(now);
                if (timeToNext > TimeSpan.Zero) {
                    _idleEvent.Set();
                    _event.WaitOne(timeToNext > MaxIdleTime ? MaxIdleTime : timeToNext);
                    _idleEvent.Reset();
                    continue;
                }

                var dueTasks = GetDueTasks(now);
                foreach (var task in dueTasks)
                    task.Invoke();
            }
        }

        private IEnumerable<IBackgroundTask> DequeueTasks()
        {
            var tasks = _tasks;

            lock (_tasksLock)
                _tasks = new List<IBackgroundTask>();

            return tasks;
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

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        /// <remarks>
        /// <para>This will attempt to do a soft stop of the background queue lasting max 1 second.
        /// If that fails, a hard stop will ensue.</para>
        /// </remarks>
        public void Dispose()
        {
            Stop();
            if (_thread.ThreadState != ThreadState.Running) return;
            _thread.Join(TimeSpan.FromSeconds(1));
            if (_thread.ThreadState != ThreadState.Running) return;
            _thread.Abort();
            _thread.Join(TimeSpan.FromSeconds(1));
        }
    }
}
