using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

public class TaskQueue
{
    private readonly LinkedList<WorkItem> _queue = new();
    private readonly object _lock = new();
    private readonly List<Thread> _workers = new();
    private volatile bool _running;
    private readonly int _maxWorkers;

    public List<WorkItem> Completed { get; } = new();

    public TaskQueue(int maxWorkers = 2)
    {
        _maxWorkers = maxWorkers;
    }

    public void Add(WorkItem item)
    {
        lock (_lock)
        {
            _queue.AddLast(item);
            var sorted = _queue.OrderByDescending(t => t.Priority).ToList();
            _queue.Clear();
            foreach (var t in sorted) _queue.AddLast(t);
        }
    }

    public void Start()
    {
        _running = true;
        for (int i = 0; i < _maxWorkers; i++)
        {
            var worker = new Thread(WorkerLoop)
            {
                Name = $"worker-{i}",
                IsBackground = true
            };
            _workers.Add(worker);
            worker.Start();
        }
    }

    public void Stop()
    {
        _running = false;
        foreach (var w in _workers)
            w.Join(TimeSpan.FromSeconds(5));
    }

    private void WorkerLoop()
    {
        while (_running)
        {
            WorkItem? item = null;
            lock (_lock)
            {
                if (_queue.Count > 0)
                {
                    item = _queue.First!.Value;
                    _queue.RemoveFirst();
                }
            }
            if (item == null)
            {
                Thread.Sleep(100);
                continue;
            }
            item.Status = "running";
            try
            {
                item.Result = item.Action();
                item.Status = "done";
            }
            catch (Exception ex)
            {
                item.Error = ex;
                item.Status = "failed";
            }
            lock (_lock)
            {
                Completed.Add(item);
            }
        }
    }
}