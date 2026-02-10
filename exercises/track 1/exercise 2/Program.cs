using System;
using System.Threading;
public class Program
{
    public static void Main()
    {
        var queue = new TaskQueue(maxWorkers: 3);
        queue.Add(new WorkItem("fetch-users", Handlers.FetchData, priority: 10));
        queue.Add(new WorkItem("fetch-orders", Handlers.FetchData, priority: 10));
        queue.Add(new WorkItem("weekly-report", () => Handlers.GenerateReport(), priority: 5));
        queue.Add(new WorkItem("notify-admin", () => Handlers.SendNotification("All tasks queued"), priority: 1));
        queue.Start();
        Thread.Sleep(5000);
        queue.Stop();
        Console.WriteLine($"Completed {queue.Completed.Count} tasks:");
        foreach (var item in queue.Completed)
        {
            Console.WriteLine($"  {item.Name}: {item.Status} -> {item.Result ?? item.Error?.Message}");
        }
    }
}