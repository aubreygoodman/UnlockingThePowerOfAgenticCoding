# Foundational Concepts: What Everyone Needs to Know

**Read this regardless of your track.** This doc covers the shared mental models that make everything else click. It should take about 15 minutes.

---

## 1. The Spectrum of AI-Assisted Coding

AI coding tools are not one thing. They exist on a spectrum, and understanding where a tool sits changes how you use it.

| Mode | What it does | Your role | Example |
|------|-------------|-----------|---------|
| **Chat** | You ask a question, it answers | Questioner | "How does React's useEffect cleanup work?" |
| **Autocomplete** | It predicts your next few characters/lines | Driver, accepting/rejecting suggestions | GitHub Copilot tab-completion |
| **Inline Edit** | You select code and ask for a specific change | Director of a single change | "Refactor this function to use async/await" |
| **Agentic** | You describe an outcome; it reads files, writes code, runs commands, iterates | Specifier and reviewer | "Add input validation to the signup form with tests" |

Most people's first AI experience is chat. Many engineers have used autocomplete. The jump to **agentic** is where the real leverage is - and where the real learning curve lives.

**The key shift:** In autocomplete, you're driving and the AI is suggesting. In agentic coding, you're *delegating* and the AI is driving. That changes everything about how you work.

---

## 2. What LLMs Are (Just Enough to Be Effective)

You don't need a deep understanding of transformer architectures. You need a working mental model.

**Think of an LLM as an extremely well-read junior engineer** who:

- Has read virtually all public code, documentation, and technical writing
- Can recognize patterns and apply them to new situations
- Has no persistent memory between conversations (unless you give it context)
- Does not "know" things the way you do - it predicts plausible next outputs based on patterns
- Can be confidently wrong, especially about specifics of *your* codebase

This mental model explains most of the behavior you'll see:
- **Why it's great at boilerplate and common patterns** - it's seen millions of examples
- **Why it struggles with novel architecture decisions** - it's pattern-matching, not reasoning from first principles
- **Why context matters so much** - without context about your codebase, it's guessing based on general patterns
- **Why it sometimes "hallucinates"** - it produces plausible-sounding output even when it doesn't have real information

---

## 3. Context Is Everything

The single biggest factor in whether you get good results from an AI coding tool is **how much relevant context it has**.

### What context means in practice

- **The files it can see.** An agentic tool that can read your codebase will outperform one that only sees the snippet you pasted.
- **The patterns in your project.** If your codebase uses a specific ORM, test framework, or architectural pattern, the AI needs to see examples of that to follow suit.
- **Your intent.** "Fix the bug" is low-context. "The `/users` endpoint returns 500 when the email field is null because we're not validating input before the database call" is high-context.

### The context hierarchy

From least to most effective:

1. **No context** - asking a generic question with no code ("how do I sort a list in Python")
2. **Snippet context** - pasting a function and asking about it
3. **File context** - the AI can see the full file
4. **Project context** - the AI can navigate your codebase, read related files, understand structure
5. **Project context + specification** - all of the above, plus a clear description of what you want and why

Agentic tools operate at levels 4 and 5. That's why they're more powerful - and why learning to provide good specification is the highest-leverage skill you can develop.

---

## 4. The Specification Skill

Specification is the art of telling the AI what you want clearly enough that it can do it. This is not "prompt engineering tricks." It's the same skill you use when writing a clear ticket, a good PR description, or a design doc.

### Good specification includes

- **What** you want to happen (the outcome, not the steps)
- **Where** in the codebase it should happen (or enough info for the agent to find the right place)
- **Constraints** that matter (must work with existing auth, must not break the API contract, should follow the patterns in the existing code)
- **How you'll know it's right** (expected behavior, edge cases, existing tests that should still pass)

### Examples

**Vague (likely to produce mediocre results):**
> Add error handling to the API

**Specific (likely to produce good results):**
> The POST /orders endpoint doesn't handle the case where a referenced product ID doesn't exist in the database. Add validation that checks product IDs before creating the order, returns a 400 with a clear error message listing the invalid IDs, and add a test case for this scenario. Follow the validation pattern used in the POST /users endpoint.

Notice: the specific version doesn't tell the AI *how* to implement it step by step. It describes the **outcome, the constraints, and a reference pattern.** The agent figures out the implementation.

---

## 5. Verification Is Your Job

This is the non-negotiable foundation: **you are responsible for the code that goes into the codebase, regardless of who or what wrote it.**

### What verification looks like

- **Read the diff.** Agentic tools produce multi-file changes. Read them the same way you'd review a teammate's PR.
- **Run the tests.** If tests pass, that's a good signal. If the agent didn't write tests, that's a red flag worth addressing.
- **Check the edges.** AI-generated code tends to handle the happy path well. Look at error cases, null inputs, boundary conditions.
- **Verify it matches your intent.** Sometimes the AI produces working code that solves a slightly different problem than what you asked for.

### Common traps

- **Rubber-stamping.** The code looks plausible, the tests pass, so you approve without really reading. This is the #1 risk with AI-generated code.
- **Over-trusting test coverage.** The AI wrote tests - great. But did the tests actually test meaningful behavior, or are they testing that the code does what the code does?
- **Anchoring on the first result.** If the agent's output isn't right, iterate. Say what's wrong and what you want instead. Don't manually fix AI code when you could give the agent better direction and have it fix itself - that's how you build the delegation skill.

---

## 6. When to Use It and When Not To

AI coding tools are not universally better or faster. Developing judgment about when to reach for them is itself a core skill.

### High-value uses

- **Boilerplate and repetitive code.** CRUD endpoints, test scaffolding, data transformations with clear input/output
- **Working in unfamiliar territory.** New framework, new language, unfamiliar part of the codebase
- **Refactoring with clear rules.** "Convert all callbacks to async/await," "rename X to Y across the codebase"
- **Exploring and understanding code.** "Explain how the auth middleware works" or "trace the flow of an order from API to database"
- **Writing tests for existing code.** Especially when the behavior is well-defined

### Lower-value or risky uses

- **Novel architecture decisions.** The AI can generate options, but the judgment call is yours
- **Highly specialized domain logic.** If the domain knowledge isn't well-represented in public code, the AI is more likely to get it wrong
- **When you don't understand the output.** If you can't verify it, you shouldn't ship it
- **Security-critical code.** Use it as a starting point, but review with extra scrutiny

---

## Key Takeaways

1. **Agentic coding is delegation, not autocomplete.** The skill is in specifying outcomes and reviewing results.
2. **Context determines quality.** The more relevant context the AI has, the better the output.
3. **Specification is the highest-leverage skill.** Clear intent beats clever prompting.
4. **Verification is non-negotiable.** You own the code, period.
5. **Judgment about when to use AI is itself a skill** that you'll develop with practice.

---

*Next step: Head to your track exercises to put these concepts into practice with real code.*
