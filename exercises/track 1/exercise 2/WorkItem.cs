using System;

public class WorkItem
{
    public string Name { get; }
    public Func<string> Action { get; }
    public int Priority { get; }
    public string Status { get; set; } = "pending";
    public string? Result { get; set; }
    public Exception? Error { get; set; }
    public DateTime CreatedAt { get; } = DateTime.UtcNow;

    public WorkItem(string name, Func<string> action, int priority = 0)
    {
        Name = name;
        Action = action;
        Priority = priority;
    }

    public override string ToString() => $"WorkItem({Name}, status={Status})";
}