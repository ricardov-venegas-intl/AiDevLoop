# Implementation Task Prompt

<context>
## Domain Knowledge
- **Project:** AiDevLoop is a command-line tool that automates Phase 2 (Development Loop) of the LLM-Assisted Development Methodology.
- **Pattern:** Functional Core + Imperative Shell (pipeline-based)
- **Tech Stack:** C# 13 / .NET 10, System.Text.Json, xUnit, NSubstitute (CLI app)
- **Projects:** AiDevLoop.Cli (entry), AiDevLoop.Shell (I/O & orchestrator), AiDevLoop.Core (pure logic)

## Reference Documentation
- @docs/requirements.md — Product requirements, CLI behavior, validation commands
- @docs/architecture.md — Functional Core / Imperative Shell, I/O adapter interfaces, pipeline design
- @docs/coding-style.md — C# conventions (XML docs, one type per file, TimeProvider, record types, async/await)
- @docs/implementation-plan.md — Phased development plan with acceptance criteria

## Key Patterns
- Functional Core: pure functions with no I/O; deterministic and easily unit-testable
- Imperative Shell: orchestrates I/O (file operations, process runner, LLM client, git, console) and invokes core functions
- I/O Adapters: `IFileOperations`, `IProcessRunner`, `ILLMClient`, `IGitClient`, `IConsoleIO` — small, focused interfaces with in-memory/test implementations
- Domain types: use `record` for DTOs and `readonly record struct` for value objects; prefer immutability
- Dependency injection: constructor injection in the Shell; Core has zero dependencies (no service locators)
- Coordination: Orchestrator composes prompts and drives the 8-step pipeline; core functions return decision objects and never perform I/O
</context>

<task>
**Objective:** Implement the next pending task from @docs/implementation-plan.md following quality gates and code review.

**Deliverable:**
1. Task documentation in @./context/current-task.md (task number, name, definition of done, steps, acceptance criteria)
2. Git branch named `task/{task-name}` with implementation
3. Implementation notes in @./context/implementation-notes.md
4. Code review report in @./context/review.md
5. Updated @docs/implementation-plan.md with task marked as complete
6. Confirmation request before final commit
</task>

<requirements>
## Must-Have Criteria
- [ ] Task selection matches implementation-plan.md status (unchecked items only)
- [ ] All code passes `dotnet build -warnaserror`
- [ ] All tests pass (`dotnet test`)
- [ ] Code follows @docs/coding-style.md conventions (XML docs, one type per file, nullable refs enabled)
- [ ] Implementation notes document decisions, known limitations, and risk areas
- [ ] Code review via dotnet-code-reviewer agent completed
- [ ] All blocking issues from review resolved (max 3 review iterations)
- [ ] User verification requested before commit

## Quality Standards
- XML documentation on all public APIs
- Proper async/await with CancellationToken parameters
- Record types for models, `readonly record struct` for value objects
- Services injected via constructor, never instantiated directly
- No deep inheritance hierarchies; composition preferred
</requirements>

<constraints>
## Avoid
- Asking about file locations, architecture decisions, or code patterns that can be inferred from docs
- Creating multiple types in a single file
- Mutable classes when records work
- Business logic in the imperative shell instead of the functional core
- Blocking async calls (`.Result`, `.Wait()`)
- Commits without user verification

## Boundaries
- CLI-only tool — no GUI or desktop UI components
- Cross-platform targets: Windows, macOS, and Linux
- Local, single-user usage only — no multi-user coordination or shared server mode
- No implicit session state between runs beyond explicit configuration and cache files

## Before Asking Any Question
1. Can I find this in @docs/requirements.md?
2. Can I find this in @docs/architecture.md?
3. Can I infer this from existing code patterns?
4. Is this decision unimportant to the outcome?

→ If yes to any, proceed without asking.
</constraints>

<output-format>
## File Locations & Structure

**Task Documentation** (@./context/current-task.md):
```
# Task [Number]: [Name]

## Description
[From implementation-plan.md]

## Definition of Done
[From implementation-plan.md - typically includes: Functional completeness; Quality gates; Documentation; Security & Performance; Handoff ready]
- [ ] Build succeeds in local environment
- [ ] Ready for code review

## Steps
1. [Step 1]
2. [Step 2]
...

## Acceptance Criteria
- [ ] Criterion 1
- [ ] Criterion 2
...
```

**Implementation Notes** (@./context/implementation-notes.md):
```
# Implementation Notes: Task [Number]

## Task Name
[Name]

## Decisions Made
- [Decision 1]: [Rationale]
- [Decision 2]: [Rationale]

## Known Limitations
- [Limitation 1]: [Impact]

## Risk Areas
- [Risk 1]: [Mitigation]

## Dependencies
- [Dependency 1]

## Testing Notes
- [Note 1]
```

**Code Review Report** (@./context/review.md):
```
# Code Review Report: Task [Number]

## Summary
[1-2 sentence overview]

## Issues Found
### Blocking
- [Issue 1]

### Non-Blocking
- [Issue 1]

### Nitpicks
- [Issue 1]

## Compliance
- [ ] Architecture adherence
- [ ] Code style compliance
- [ ] Test coverage adequate
- [ ] Risk areas addressed
```

## Branch Naming
Format: `task/TASK-001-Create-solution-and-project-structure` (task number and kebab-case task name)
</output-format>

<approach>
## Execution Steps

1. **Select Next Task**
   - Read @docs/implementation-plan.md
   - Find first unchecked task
   - Extract task number, name, steps, acceptance criteria, definition of done
   - Save to @./context/current-task.md

2. **Create Branch**
   - get the latest changes from main
   - Create local branch: `git checkout -b task/{task-name}` from main

3. **Implement Task**
   - Invoke `dotnet-task-coder` agent with `@./context/current-task.md` as input.
   - The agent will:
     - Read the task definition from `@./context/current-task.md`.
     - Implement the required C# code, following all project conventions from `@docs/architecture.md`, `@docs/requirements.md`, and `@docs/coding-style.md`.
     - Build and test the code (`dotnet build`, `dotnet test`).
     - Document its work in `@./context/implementation-notes.md`.

4. **Code Review & Refinement (Iterative Loop)**
   - **Review**: Invoke the `dotnet-code-reviewer` agent.
     - **Input**: The agent will read the changed files, plus `@./context/implementation-notes.md`, `@docs/requirements.md`, and `@docs/architecture.md`.
     - **Output**: The agent saves its findings to `@./context/review.md`, categorizing issues as `Blocking`, `Non-Blocking`, or `Nitpicks`.
   - **Analyze & Refine**:
     - Read `@./context/review.md`.
     - **If `Blocking` issues exist**:
       - Invoke the `dotnet-task-coder` agent again to fix the issues.
       - **Input**: Provide the original `@./context/current-task.md` and the new `@./context/review.md` as context.
       - Repeat the review/refine loop (max 3 iterations).
     - **If no `Blocking` issues exist**: Proceed to the next step.

5. **Update Plan**
   - In @docs/implementation-plan.md, mark this task's milestone list item as complete (change `- [ ] TASK-XXX · ...` to `- [x] TASK-XXX · ...`).
   - In the task definition section for this task, update the `**Status:**` field to reflect completion (for example, `**Status:** Completed`).
   - Ensure all relevant checkboxes within the completed task's details (steps, acceptance criteria, etc.) are checked as appropriate.
   - In @./context/current-task.md, check all checkboxes under `Definition of Done` and `Acceptance Criteria`.

6. **Archive Context Files**
   - Create folder under @./context/completed named `{TASK-ID}` (e.g., `TASK-001`)
   - Move (do not copy) these files to the completed task folder:
     - @./context/current-task.md
     - @./context/implementation-notes.md
     - @./context/review.md
   - Verify that none of the following files exist in @./context/ after the move (delete if still present):
     - @./context/current-task.md
     - @./context/implementation-notes.md
     - @./context/code-review.md

7. **Request User Verification to Commit**
   - Present a summary of the changes made.
   - Ask for user approval before committing.
   - User must explicitly approve before running `git commit`.

8. **Create Pull Request**
   - Once user approves, commit all changes with appropriate commit message
   - Push branch to remote
   - Create PR targeting main branch with summary of changes and test plan

9. **Stop**
   - Task complete — do not proceed to next task without explicit user request
</approach>

<validation-checklist>
After implementation and review approval, verify:
- [ ] Code builds without warnings (`dotnet build -warnaserror`)
- [ ] All tests pass (`dotnet test`)
- [ ] XML docs present on all public APIs
- [ ] One type per file
- [ ] Functional Core remains pure (no I/O, no mutable shared state, no framework dependencies)
- [ ] Constructor injection used (no service locators)
- [ ] CancellationToken in all async methods
- [ ] Record types used for models
- [ ] Immutable collections returned from service methods
- [ ] All blocking review issues resolved
- [ ] Implementation notes and code review report saved
</validation-checklist>
