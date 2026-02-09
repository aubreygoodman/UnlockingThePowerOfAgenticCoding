# Track 3 Exercises: Sharpening the Edge

**Who this is for:** Engineers who already use agentic coding tools for real work but want to move from "sometimes great, sometimes frustrating" to consistently effective. These exercises focus on workflow patterns, not basics.

**Prerequisites:** Read [foundational-concepts.md](foundational-concepts.md) if you haven't. You should already be comfortable delegating tasks and reviewing agentic output - if not, start with Track 2.

**Tools:** Exercises reference both **Claude Code** and **Cursor**. Tool-specific notes are called out where the workflow differs.

---

## Exercise 1: Test-Driven Agentic Workflow (Warm-up)

**Goal:** Use tests as both the specification *and* the verification mechanism for agentic work. This pattern produces the most consistently reliable results.
**Time:** ~25 minutes
**Type:** Self-contained

### The function to implement

Here's a function signature and its expected behavior. Your job: write the tests first (or have the agent write them from the spec), then have the agent implement the function to pass the tests.

**Function:** `parse_duration(s: str) -> int`

**Behavior:**
- Parses a human-readable duration string and returns total seconds
- Supported formats: `"30s"`, `"5m"`, `"2h"`, `"1d"`, `"1h30m"`, `"2d12h"`, `"1h30m45s"`
- Units: `s` = seconds, `m` = minutes, `h` = hours, `d` = days
- Compound durations must be in descending unit order (d, h, m, s)
- Invalid input raises `ValueError` with a descriptive message
- Leading/trailing whitespace is stripped
- The string `"0s"` returns `0`
- Negative values are not supported (raise `ValueError`)

### Step 1: Write the tests first

Give the AI the function signature and behavior above, and ask it to **write only the tests**:

> *"Here's a function signature and spec. Write a comprehensive pytest test suite for it. Do NOT implement the function yet - just write the tests. Put a placeholder `pass` in the function body so the tests can import it.*
>
> *[paste the function signature and behavior above]*
>
> *Make sure to test: each unit individually, compound durations, edge cases (whitespace, '0s'), and all the error cases (invalid format, negative values, wrong unit order, empty string)."*

**Review the tests before moving on.** Check:
- [ ] Do the tests cover all the behaviors listed in the spec?
- [ ] Are the expected values correct? (e.g., `"1h30m"` = 5400 seconds)
- [ ] Are edge cases included?
- [ ] Do the tests for `ValueError` check the error message, not just the exception type?

### Step 2: Implement to pass the tests

Now tell the agent:

> *"Implement the `parse_duration` function so all the tests pass. Run the tests and iterate until they're green."*

- **Claude Code:** The agent can run `pytest` directly. Let it iterate.
- **Cursor:** You may need to run tests manually and paste failures back, or use the terminal integration.

### Step 3: Review the implementation

Even though the tests pass, review the code:
- Is the implementation readable and maintainable?
- Does it handle the spec correctly, or did it "overfit" to the specific test cases?
- Would you feel confident adding new test cases and having them pass without changing the implementation?

### Why this pattern works

- **Tests are an executable specification.** Instead of describing what you want in English (which is ambiguous), you describe it in code (which is precise).
- **Verification is automatic.** You don't have to manually check if the output is right - the tests tell you.
- **Iteration is fast.** When a test fails, the agent has a specific, actionable signal about what's wrong.

This pattern - tests first, implement to green, review the implementation - is the single most reliable agentic workflow. Use it whenever the task has well-defined inputs and outputs.

---

## Exercise 2: Plan, Execute, Iterate

**Goal:** Practice the full plan-execute-iterate cycle on a task complex enough to require mid-course corrections.
**Time:** ~45 minutes
**Type:** Self-contained + codebase

### The scenario

You'll work through a moderately complex task that's designed to expose the moments where you need to stop the agent and redirect. This builds the judgment that separates consistent results from hit-or-miss ones.

**The task:** Build a simple Markdown link checker - a tool that reads a Markdown file, finds all `[text](url)` links, checks if they're valid (return HTTP 200), and reports broken ones.

### Phase 1: Plan before executing

Before letting the agent write code, use planning mode to think through the approach:

- **Claude Code:** Start with plan mode: *"/plan Build a Markdown link checker that reads a .md file, extracts all links, checks each one with an HTTP request, and reports which are broken. Think through the design before writing code."*
- **Cursor:** In Agent mode, prefix your prompt: *"Before writing any code, outline your approach for: a Markdown link checker that..."*

**Review the plan.** Look for:
- Does it handle relative links vs. absolute URLs?
- Does it plan for timeouts on HTTP requests?
- Does it address concurrent checking (links can be slow one at a time)?
- Does it plan for redirects (a 301 isn't "broken")?

If the plan misses something, **redirect now** - before any code is written:
> *"Good plan, but add: handling for redirects (follow up to 3 redirects, treat the final status as the result), a timeout of 10 seconds per URL, and run checks concurrently with a max of 5 simultaneous requests."*

### Phase 2: Execute with checkpoints

Now let the agent implement. But set explicit checkpoints:

> *"Implement the link checker in this order, and stop after each step so I can review:*
> *1. The Markdown parser that extracts links (with a test)*
> *2. The HTTP checker (with a test using a mock)*
> *3. The main CLI that ties it together*
> *4. Integration test with a sample Markdown file"*

At each checkpoint, review and redirect if needed. Common places the agent goes off track:

| Symptom | What to say |
|---------|------------|
| Over-engineers the Markdown parser (full AST) | *"We don't need a full parser. A regex for `[text](url)` patterns is fine for this use case."* |
| Doesn't mock HTTP calls in tests | *"Don't make real HTTP requests in tests. Use `unittest.mock.patch` or `responses` library."* |
| Ignores concurrent execution | *"The plan included concurrent checking. Implement that - use `asyncio` or `concurrent.futures`."* |
| Adds features you didn't ask for | *"Remove the HTML output option. Stick to the spec - plain text report to stdout."* |

### Phase 3: Reflect on when you intervened

After completing the task:

- [ ] At which checkpoints did you need to redirect? Why?
- [ ] Were there moments where the agent was going down a path you let it continue even though you were unsure? What happened?
- [ ] Were there moments where you intervened too early and could have let it finish?
- [ ] How did the checkpoint approach compare to giving it the whole task at once?

### When to let the agent go vs. when to stop

Build this mental model:

| Let it continue when... | Stop and redirect when... |
|------------------------|--------------------------|
| It's making progress toward the right outcome | It's building the wrong abstraction or architecture |
| Minor style differences from what you'd write | It's adding scope you didn't ask for |
| It's exploring the codebase to find context | It's modifying files it shouldn't touch |
| Test failures it's iterating on | The same test has failed 3+ times the same way |
| Small deviations from the spec you can fix in review | Fundamental misunderstanding of the requirement |

---

## Exercise 3: Full Workflow Integration

**Goal:** Take a real task from your sprint through to a PR, using your full agentic toolkit, and document the workflow pattern that works for you.
**Time:** ~60 minutes
**Type:** Real-codebase template

### Set up project context (one-time)

If you haven't already, create a project context file in your repo:

**For Claude Code** - create `CLAUDE.md` in your repo root:

```markdown
# Project Context

## Tech stack
[Your stack - e.g., Python 3.11, FastAPI, PostgreSQL, pytest]

## Architecture
[Brief description - e.g., "Monorepo with services in /src/services,
shared models in /src/models, tests mirror src structure in /tests"]

## Conventions
- [Your conventions - e.g., "Use structured logging via structlog"]
- [Test pattern - e.g., "Integration tests use the test database fixture in conftest.py"]
- [Code style - e.g., "Follow existing patterns in the codebase over general best practices"]

## Common commands
- Run tests: [your command]
- Run linter: [your command]
- Start dev server: [your command]
```

**For Cursor** - create `.cursorrules` in your repo root with similar content.

These files give the agent persistent context about your project. You'll update them as you learn what the agent needs to know.

### Pick your task

Choose a real task from your sprint or backlog. Ideal characteristics:
- Meaningful enough to take 30+ minutes manually
- Touches 2-5 files
- Has clear acceptance criteria
- Has existing tests you can validate against (or should have tests written)

Examples: implement a new API endpoint, fix a bug with a known repro, add a feature to an existing module, refactor a component to a new pattern.

### The workflow

**1. Gather context and write specification** (10 minutes)

Before opening any AI tool, write a specification in your own words. Use this structure:

```
## Task
[One sentence: what and why]

## Context
- Related files: [list the key files]
- Reference pattern: [point to similar existing code]
- Dependencies: [what this interacts with]

## Requirements
- [Acceptance criteria 1]
- [Acceptance criteria 2]
- [Constraints]

## Verification
- [ ] Tests pass: [which tests]
- [ ] Manual check: [what to verify]
- [ ] No regressions: [what shouldn't break]
```

**2. Plan** (5 minutes)

- **Claude Code:** Use `/plan` or start with *"Let's plan the approach for this before writing code."*
- **Cursor:** Ask the agent to outline its approach first.

Review the plan. Does it match your mental model of how this should be implemented? Redirect if not.

**3. Execute** (15-20 minutes)

Let the agent implement. Use checkpoints for multi-step work. Intervene when:
- It's going off-plan without good reason
- It's modifying files outside the scope
- The same error keeps recurring

**4. Review** (10 minutes)

Review the complete diff, not just the files you expected to change:

```bash
git diff
```

- **Claude Code:** You can ask *"Show me a summary of all changes you made"* or *"Explain why you changed [file]"*
- **Cursor:** Use the source control panel to review the full diff

Check for:
- [ ] All acceptance criteria met
- [ ] No unintended changes (scope creep, "helpful" refactors)
- [ ] Tests added/updated
- [ ] Consistent with project patterns
- [ ] No secrets, debug code, or TODOs left behind

**5. Test and finalize** (5-10 minutes)

Run your full test suite (not just the new tests). Check for regressions. If anything fails, tell the agent - don't fix it manually.

**6. Create the PR** (5 minutes)

Have the agent help draft the PR description:

> *"Based on the changes in this branch, write a PR description covering: what changed, why, how to test, and any follow-up items."*

### Document your workflow pattern

After completing the task, capture what worked. This is personal - there's no "right" workflow. Write down:

1. **My specification approach:** How detailed do you need to be? Where do you under-specify and get bad results?
2. **My planning habits:** Do you need to plan every time, or only for complex tasks?
3. **My intervention triggers:** What signals tell you the agent is going off track?
4. **My review process:** What do you check first? What catches most issues?
5. **My tool preferences:** Which tool (Claude Code vs. Cursor) works better for which types of tasks?

Keep this somewhere you can reference and update. Over the next few weeks, you'll refine it as you develop your personal pattern.

### Reflection

- [ ] How did using project context files (CLAUDE.md / .cursorrules) affect the quality of results?
- [ ] Where in the workflow did you add the most value? (This tells you where to focus your energy.)
- [ ] What would you add to the project context file based on this experience?
- [ ] If you did this task again tomorrow, what would you do differently?

---

## What's Next

You've now built a complete workflow. From here:

1. **Refine your project context file** after every few tasks. The better it gets, the better your results get.
2. **Develop your specification templates.** Different task types (bug fix, feature, refactor) benefit from different specification shapes.
3. **Help your teammates.** The best way to solidify your own workflow is to help someone in Track 1 or 2 work through their exercises. You'll discover gaps in your own understanding.
4. **Stay skeptical, stay curious.** The tools are improving fast. Patterns that don't work today might work next month. Keep experimenting.
