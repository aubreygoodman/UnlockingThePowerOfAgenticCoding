# Track 1 Exercises: Building the Mental Model

**Who this is for:** Engineers with no or minimal AI coding experience. These exercises help you build intuition for what AI coding tools do, how to interact with them, and when to trust the output.

**Prerequisites:** Read [foundational-concepts.md](foundational-concepts.md) first.

**Tools:** Each exercise notes whether to use **Claude Code** (terminal-based) or **Cursor** (IDE-based). If you have access to both, try both and notice the differences. If you only have one, that's fine - the core skills transfer.

---

## Exercise 1: Your First AI Conversation About Code (Warm-up)

**Goal:** See how an AI reasons about code, what it gets right, and what you need to verify.
**Time:** ~15 minutes
**Type:** Self-contained (everything you need is right here)

### The buggy code

Here's a C# method that's supposed to calculate the average rating from a list of product reviews. It has several bugs.

```csharp
using System;
using System.Collections.Generic;

public class Review
{
    public int Rating { get; set; }    // 1-5 stars
    public bool Verified { get; set; }
}

public class RatingCalculator
{
    /// <summary>
    /// Calculate the average star rating from a list of reviews.
    /// Returns the average rounded to one decimal place.
    /// Returns 0 if there are no reviews.
    /// </summary>
    public static double CalculateAverageRating(List<Review> reviews)
    {
        int total = 0;
        foreach (var review in reviews)
        {
            total += review.Rating;
        }

        double average = total / reviews.Count;
        return Math.Round(average);
    }
}

// These should all work correctly:
public class Program
{
    public static void Main()
    {
        // Expected: 0
        Console.WriteLine(RatingCalculator.CalculateAverageRating(new List<Review>()));

        // Expected: 5.0
        Console.WriteLine(RatingCalculator.CalculateAverageRating(new List<Review>
        {
            new Review { Rating = 5, Verified = true }
        }));

        // Expected: 4.0
        Console.WriteLine(RatingCalculator.CalculateAverageRating(new List<Review>
        {
            new Review { Rating = 4, Verified = true },
            new Review { Rating = 3, Verified = true },
            new Review { Rating = 5, Verified = false },
        }));
    }
}
```

### What to do

1. **Copy the code above** into a file called `RatingCalculator.cs` (or whatever you like).

2. **Open it with your AI tool:**
   - **Claude Code:** In your terminal, navigate to the directory and run `claude`. Then say: *"Look at RatingCalculator.cs. There are bugs - find them and fix them."*
   - **Cursor:** Open the file in Cursor. Select all the code, open the inline chat (Ctrl+K / Cmd+K), and say: *"This code has bugs. Find and fix them."*

3. **Watch what happens.** Don't just accept the fix - observe:
   - What bugs did the AI find?
   - How did it explain its reasoning?
   - Did it find all the bugs, or only some?
   - Did it change anything unnecessarily?

4. **Verify the fix yourself.** Run the corrected code. Do all three test cases produce the expected output?

### Reflection checklist

After you've completed the exercise, take a minute to reflect:

- [ ] What surprised you about how the AI approached the problem?
- [ ] Did the AI find bugs you hadn't noticed? Did it miss any?
- [ ] Did you understand the AI's explanation, or did you need to look anything up?
- [ ] How confident are you in the fix *without* running the code? How about *after* running it?
- [ ] Did the AI change anything beyond what was needed (unnecessary "improvements")?

**Key takeaway:** The AI can spot common patterns (like division-by-zero) quickly, but *you* are the one who decides whether the fix is correct and complete. This verify-then-trust loop is the foundation of everything that follows.

---

## Exercise 2: Explore Unfamiliar Code

**Goal:** Use an AI tool to understand code you didn't write, rather than using it to generate code.
**Time:** ~20 minutes
**Type:** Self-contained

### The code to explore

Below is a small project (3 files) that implements a basic task queue. You don't need to know how it works yet - that's the point.

**File: `WorkItem.cs`**
```csharp
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
```

**File: `TaskQueue.cs`**
```csharp
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
```

**File: `Handlers.cs`**
```csharp
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
```

**File: `Program.cs`**
```csharp
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
```

### What to do

1. **Create these four files** in a folder (or just read them here - you don't need to run them).

2. **Ask the AI to explain the project.** Start broad, then get specific:
   - *"What does this project do? Give me a high-level overview."*
   - *"How does the priority system work? Walk me through what happens when I add a high-priority task after a low-priority one."*
   - *"What happens when `FetchData` throws an HttpRequestException? Trace the flow from the exception to where it ends up."*
   - *"Are there any concurrency issues in this code?"*

3. **Push deeper on one answer.** Pick whichever response you found most interesting and ask a follow-up. For example:
   - *"You mentioned a potential race condition - can you show me a specific sequence of events where that would cause a problem?"*
   - *"If I wanted to add retry logic for failed tasks, where would that go?"*

### What to notice

- How did the AI's explanation compare to reading the code yourself? Faster? More context?
- Did it catch anything you didn't see on your first read?
- Were any of its explanations wrong or misleading? (It's okay if you're not sure - flag anything that felt off.)
- How useful was the AI as an *exploration partner* vs. a *code writer*?

**Key takeaway:** AI tools are powerful for understanding code, not just writing it. When you join a new project or dig into an unfamiliar module, the AI can dramatically speed up your ramp-up time. But you still need to spot-check its explanations against the actual code.

---

## Exercise 3: Your First Delegated Task

**Goal:** Let an AI tool make a real (small, safe) change in your actual codebase and practice the full review loop.
**Time:** ~30 minutes
**Type:** Real-codebase template

### Pick your task

Choose one of these low-risk tasks from your own codebase. Pick whichever feels most comfortable:

| Task type | What to do | Why it's good for a first try |
|-----------|-----------|-------------------------------|
| **Add a log message** | Find a function that doesn't log on entry/exit and add structured logging | Low risk, easy to verify, easy to revert |
| **Rename a variable** | Find a poorly named variable and rename it across all files that reference it | Tests multi-file awareness, easy to verify with search |
| **Add a missing test** | Find a function with no test coverage and add a basic test | Safe (additive only), immediately verifiable |
| **Improve an error message** | Find a generic error message and make it more descriptive | Small scope, forces the AI to understand context |

### Walkthrough

**1. Provide context** (2-3 minutes)

Don't just say "add a log." Give the AI enough to do it well:

> **Claude Code example:**
> *"In `Services/OrderService.cs`, the `CreateOrder` method doesn't have any logging. Add a structured log message at the start of the method that logs the user ID and number of items. Use the same ILogger pattern as `UpdateOrder` in the same file. Use our existing logger - don't create a new one."*

> **Cursor example:**
> Open the file, select the function, and use inline chat or agent mode with a similar prompt.

Notice: you're specifying **what** (add logging), **where** (which function), **how** (match existing pattern), and **constraints** (use existing logger).

**2. Let the AI work** (1-2 minutes)

- In **Claude Code**, the agent will read the file, find the pattern, and make the change. Watch the tool calls scroll by.
- In **Cursor**, use Agent mode (Ctrl+I / Cmd+I) for the best experience with multi-step tasks, or Ctrl+K / Cmd+K for simpler inline changes.

**3. Review the output** (5-10 minutes)

This is the most important step. Ask yourself:

- [ ] Does the change match what I asked for?
- [ ] Does it follow the existing patterns in the codebase?
- [ ] Did the AI change anything I didn't ask it to change?
- [ ] Could this break anything? (Think about imports, side effects, test failures)
- [ ] Would I approve this if a teammate submitted it as a PR?

**4. Verify** (5 minutes)

- Run the relevant tests. Do they pass?
- If you added a log message, can you trigger the code path and see the log?
- If you renamed a variable, search the codebase - did it catch every reference?

### What to watch for

- **Scope creep.** You asked for one log line and the AI also "helpfully" refactored the function, added type hints, or updated the docstring. This is common. Learn to say: *"Undo everything except the log line I asked for."*
- **Pattern mismatch.** The AI added logging, but used a different format than the rest of the codebase. This is why pointing to a reference pattern matters.
- **Plausible but wrong.** The code looks correct but uses a logger name that doesn't exist, or imports from the wrong module. Always verify.

### Reflection

- [ ] How much time did the delegation + review take vs. doing it by hand?
- [ ] What would you do differently in your prompt next time?
- [ ] Did the "specify, delegate, review" loop feel natural or awkward? (Both are fine at this stage.)

**Key takeaway:** Delegating effectively is a skill. Your first attempt might feel slower than doing it yourself - that's normal. The value comes as you get better at specification and learn what the tools handle well. Stick with it.

---

## What's Next

Once you've completed these three exercises:

1. **Go back to the self-assessment.** Does Track 2 sound like your next step, or do you want more practice here?
2. **Try Exercise 3 again** with a different task type. Repetition builds the muscle memory.
3. **Share one insight** with the team - what surprised you, what worked, or what didn't.
