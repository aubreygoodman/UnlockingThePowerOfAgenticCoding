# Track 2 Exercises: From Autocomplete to Delegation

**Who this is for:** Engineers who use Copilot or similar autocomplete tools regularly but haven't done agentic coding - delegating multi-step tasks to an AI and reviewing the output.

**Prerequisites:** Read [foundational-concepts.md](foundational-concepts.md) first. If you haven't done any of the Track 1 exercises and this is your first time using Claude Code or Cursor's agent mode, consider doing Track 1 Exercise 3 as a warm-up.

**Tools:** Each exercise notes tool-specific details. Both **Claude Code** and **Cursor** work for all exercises.

---

## Exercise 1: Specification Side-by-Side (Warm-up)

**Goal:** See firsthand how the quality of your specification drives the quality of the result.
**Time:** ~25 minutes
**Type:** Self-contained

### The task

You'll build the same thing twice: **a small CLI tool that reads a CSV file and outputs JSON.** (You don't need to keep the result - this is about observing the difference.)

### Round 1: The vague prompt

Open your AI tool and give it this prompt exactly as written:

> *"Build a CLI tool that converts CSV to JSON."*

That's it. Don't add anything. Let the agent work and produce its result.

**Save the output** (or take a screenshot). Note:
- How many files did it create?
- What decisions did it make on your behalf (language, library, error handling, output format)?
- Would you be comfortable shipping this?

### Round 2: The detailed specification

Now give it this prompt:

> *"Build a C# console application called `Csv2Json` that converts a CSV file to JSON. Requirements:*
>
> *- Uses only the .NET standard libraries (no NuGet packages) - use System.Text.Json for JSON serialization*
> *- Takes the input CSV path as the first command-line argument*
> *- Outputs JSON to stdout by default, or to a file if `--output <path>` is provided*
> *- The JSON should be an array of objects where each object's keys are the CSV column headers*
> *- Handle these error cases: file not found (exit code 1 with message to stderr), empty CSV file (output empty JSON array `[]`), malformed CSV rows (skip the row and print a warning to stderr)*
> *- Include a `--pretty` flag that formats the JSON with indentation*
> *- Should be runnable as `dotnet run -- input.csv` or as a compiled executable `Csv2Json.exe input.csv`*
> *- Include XML doc comments and a brief usage message when run with no arguments"*

**Save this output too.**

### Compare the results

| Question | Vague prompt | Detailed specification |
|----------|-------------|----------------------|
| Did the tool handle errors? | | |
| Did it match your expectations? | | |
| How much manual fixing would it need? | | |
| Did the AI make assumptions you disagree with? | | |
| Would you trust this in production? | | |

### Reflection

- The difference you just saw is the **specification skill** in action. You didn't tell the AI *how* to implement the CSV parsing or argument handling - you told it **what the outcome should look like.**
- In autocomplete mode, you control quality line-by-line. In agentic mode, you control quality *upfront through specification* and *afterward through review.* This is the fundamental shift.
- Ask yourself: was writing the detailed spec significantly harder than writing the code yourself? Usually the answer is no - and the spec is reusable, reviewable, and produces more consistent results.

---

## Exercise 2: Multi-File Feature

**Goal:** Practice the agentic workflow on a feature that spans multiple files - the kind of task where agentic tools really shine over autocomplete.
**Time:** ~40 minutes
**Type:** Self-contained

### The task

Build a minimal REST API endpoint with validation, a handler, and tests. We'll use C# with ASP.NET Core minimal APIs.

**Give the AI this specification:**

> *"Create a C# ASP.NET Core minimal API with a single endpoint: `POST /bookmarks`.*
>
> *The project should have this structure:*
> ```
> Bookmarks/
>   Program.cs              # App setup, endpoint registration
>   BookmarkHandler.cs      # Request handler for the bookmark endpoint
>   BookmarkValidator.cs    # Input validation logic
>   BookmarkModels.cs       # Request/response DTOs
>   BookmarkTests.cs        # Tests using xUnit and WebApplicationFactory
> ```
>
> *The bookmark endpoint accepts JSON with these fields:*
> *- `Url` (required, must be a valid URL starting with http:// or https://)*
> *- `Title` (required, string, max 200 characters)*
> *- `Tags` (optional, list of strings, each max 50 characters)*
>
> *Behavior:*
> *- Valid request: return 201 with the bookmark data plus a generated `Id` (GUID) and `CreatedAt` (ISO timestamp)*
> *- Invalid request: return 400 with `{"error": "<description of what's wrong>"}` - be specific about which field failed and why*
> *- Store bookmarks in memory (a `List<Bookmark>` is fine - no database needed)*
>
> *Tests should cover:*
> *- Successful bookmark creation*
> *- Missing required fields (Url, Title separately)*
> *- Invalid URL format*
> *- Title exceeding max length*
> *- Valid request with and without tags*
>
> *Use `WebApplicationFactory<Program>` for integration testing. No external dependencies beyond the standard ASP.NET Core and xUnit packages."*

### The workflow

**1. Let the agent work.**
- **Claude Code:** Paste the spec and let it create the files. Watch which files it creates and in what order.
- **Cursor:** Use Agent mode (Ctrl+I / Cmd+I) with the spec. You may need to accept file creations as they come.

**2. Review the diff before doing anything else.**

This is where the autocomplete-to-delegation shift matters most. With autocomplete, you review line by line as you type. With agentic output, you review a **complete set of changes across multiple files** - just like a PR.

Check each file:

| File | What to look for |
|------|-----------------|
| `BookmarkValidator.cs` | Does the URL validation actually work? Is it using `Uri.TryCreate`, regex, or just string checks? Are edge cases handled? |
| `BookmarkHandler.cs` | Does it call the validator correctly? Does the 400 response include useful error messages? |
| `Program.cs` | Is the endpoint registered properly? Any unnecessary boilerplate or middleware? |
| `BookmarkModels.cs` | Are the DTOs clean? Are nullable types used correctly for optional fields? |
| `BookmarkTests.cs` | Do tests cover all the cases you specified? Are they testing *behavior* or just testing that the code runs? |

**3. Run the tests.**

```bash
cd Bookmarks
dotnet test --verbosity normal
```

**4. Iterate if needed.**

If something isn't right, don't fix it manually. Tell the agent what's wrong:
- *"The URL validator accepts `http://` with no domain - it should reject URLs without a valid domain after the protocol."*
- *"The test for missing title sends an empty string, but it should test with the `title` key completely absent."*

This is the review-and-iterate loop that replaces line-by-line editing.

### What good review looks like for multi-file changes

When reviewing agentic output that spans files, focus on these in order:

1. **Structure:** Did it create the files you asked for? Are responsibilities split correctly?
2. **Interfaces:** Do the files connect properly? Does `BookmarkHandler` use `BookmarkValidator` correctly?
3. **Logic:** Is the core logic correct? (Validation rules, response codes, error messages)
4. **Edge cases:** What happens with empty input, weird URLs, very long strings?
5. **Tests:** Do the tests actually verify the behavior, or do they just assert that the code doesn't crash?

### Reflection

- [ ] How did reviewing multi-file output compare to your normal code review process?
- [ ] Did you find yourself wanting to "just fix it" manually? (That's the autocomplete habit - practice delegating the fix instead.)
- [ ] Was the iteration cycle (tell the agent what's wrong, let it fix it) faster or slower than fixing it yourself?

---

## Exercise 3: Real Bug Fix or Feature

**Goal:** Apply the agentic workflow to your actual codebase and real work.
**Time:** ~45 minutes
**Type:** Real-codebase template

### Pick your task

Choose a real item from your backlog. Good candidates:

| Good for this exercise | Not ideal (save for Track 3) |
|----------------------|----------------------------|
| A bug with a known reproduction path | A vague "sometimes this breaks" bug |
| A small feature that adds to existing code | A feature requiring new architecture |
| A refactor with clear before/after | A refactor requiring design decisions |
| Adding tests to existing untested code | Writing tests for complex domain logic |

### The structured workflow

**1. Gather context** (5 minutes)

Before talking to the AI, figure out what it needs to know:

- What's the relevant area of the codebase? (Specific files, modules, or directories)
- Are there existing patterns it should follow? (Find a similar bug fix or feature to reference)
- What are the constraints? (Don't break the API, must work with existing auth, etc.)
- How will you verify the result? (Existing tests, manual testing, specific behavior to check)

**2. Write your specification** (5-10 minutes)

Use this template:

```
Task: [What needs to happen]

Context:
- The relevant code is in [paths/modules]
- This is similar to [reference example] - follow the same pattern

Requirements:
- [Specific requirement 1]
- [Specific requirement 2]
- [Constraint or thing to avoid]

Verification:
- [How to know it's right]
- [Tests that should pass]
- [Behavior to check]
```

This feels like overhead the first time. It gets faster, and the results are dramatically better.

**3. Delegate** (let the agent work)

- **Claude Code:** Pass the specification. If the codebase is large, you may want to point the agent at specific files: *"Start by reading `Services/PaymentService.cs` and `Tests/PaymentServiceTests.cs`."*
- **Cursor:** Open the relevant files, use Agent mode with the specification. Cursor's context is scoped to your open files and workspace, so make sure the relevant files are accessible.

**4. Review** (10-15 minutes)

Use the same multi-file review approach from Exercise 2:
- Structure and file organization
- Interfaces between components
- Core logic correctness
- Edge cases
- Test coverage

**5. Iterate** (as needed)

If the first pass isn't right:
- Be specific about what's wrong: *"The error handling in `ProcessPayment` catches all exceptions, but it should only catch `PaymentGatewayException` and let other exceptions propagate."*
- Reference patterns: *"Look at how `ProcessRefund` handles errors in the same file - match that pattern."*
- Don't restart from scratch unless the approach is fundamentally wrong. Iterate on what's there.

### Reflection: Manual vs. Agentic

After you finish, compare honestly:

| Dimension | Doing it manually | Using the agent |
|-----------|------------------|-----------------|
| Time to get a first working version | | |
| Time to get to a version you'd ship | | |
| Quality of the result | | |
| Your confidence in the result | | |
| Things you learned about the codebase | | |

There's no "right" answer. Sometimes manual is faster, especially for tasks you've done many times. The goal is to build an honest sense of where agentic tools add value in *your* workflow.

---

## What's Next

Once you've completed these three exercises:

1. **Try Exercise 3 again** with a different type of task (if you fixed a bug, try a feature, or vice versa).
2. **Revisit the self-assessment.** If you're consistently getting good results and knowing how to iterate, Track 3 will help you sharpen that into a repeatable workflow.
3. **Compare notes with a teammate.** Different people develop different specification styles and review habits. Sharing what works accelerates everyone's learning.
