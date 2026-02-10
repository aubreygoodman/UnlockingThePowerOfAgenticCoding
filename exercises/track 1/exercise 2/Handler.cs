using System;
using System.Threading;
public static class Handlers
{
    private static readonly Random Rng = new();
    /// <summary>Simulate fetching data from an external API.</summary>
    public static string FetchData()
    {
        Thread.Sleep(Rng.Next(500, 1500));
        if (Rng.NextDouble() < 0.2)
            throw new HttpRequestException("API timeout");
        int users = Rng.Next(10, 100);
        return $"{{\"users\": {users}, \"timestamp\": \"{DateTime.UtcNow:O}\"}}";
    }
    /// <summary>Simulate generating a report.</summary>
    public static string GenerateReport(string? data = null)
    {
        Thread.Sleep(Rng.Next(300, 800));
        return $"Report generated with {data ?? "no"} data";
    }
    /// <summary>Simulate sending a notification.</summary>
    public static string SendNotification(string message = "Task complete")
    {
        Thread.Sleep(200);
        return $"Sent: {message}";
    }
}