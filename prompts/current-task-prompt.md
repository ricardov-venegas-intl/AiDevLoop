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
- @CLAUDE.md — Project conventions and coordination patterns

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
4. Code review report in @./context/code-review.md
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
- Abstract base classes in application code
- Blocking async calls (`.Result`, `.Wait()`)
- Commits without user verification

## Boundaries
- Desktop-only targets (Windows/macOS) — no mobile platforms
- Single-user app — no multi-user features
- Session state not persisted (except last root folder and settings)
- Token counting: length/4 heuristic only for v1

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

**Code Review Report** (@./context/code-review.md):
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
Format: `task/1.1-Create-Solution` (task number and kebab-case task name)
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
   - Before coding: Re-read relevant sections of @docs/architecture.md and @docs/requirements.md
   - Follow @docs/coding-style.md throughout
   - Build frequently: `dotnet build -warnaserror`
   - Run tests: `dotnet test`
   - Document implementation in @./context/implementation-notes.md as you work
   - Include: Decisions, known limitations, risk areas, dependencies, testing notes

4. **Code Review (Iterative)**
   - Invoke dotnet-code-reviewer agent
   - Agent should read: implementation-notes.md, requirements.md, architecture.md
   - Agent saves review report to @./context/code-review.md
   - Categorize issues: Blocking | Non-Blocking | Nitpicks
   - **If blocking issues:** Fix and repeat review (max 3 iterations)
   - **If no blocking issues:** Proceed to step 5

5. **User Verification**
   - Present summary of changes
   - Ask user approval before committing

6. **Update Plan**
   - Mark task as complete in @docs/implementation-plan.md
   - Check all checkboxes for the completed task section
   - Update task header checkbox: `### [x] [Task Number] [Task Name]`

7. **Archive Context Files**
   - Create folder under @./context/History named `{task-number}-{kebab-case-task-name}` (e.g., `1.1-Create-Solution`)
   - Move these files to the history folder:
     - @./context/current-task.md
     - @./context/implementation-notes.md
     - @./context/code-review.md

8. **No Commit Yet**
   - User must explicitly approve before running `git commit`

9. **Create Pull Request**
   - Once user approves, commit all changes with appropriate commit message
   - Push branch to remote
   - Create PR targeting main branch with summary of changes and test plan

10. **Stop**
   - Task complete — do not proceed to next task without explicit user request
</approach>

<validation-checklist>
After implementation and review approval, verify:
- [ ] Code builds without warnings (`dotnet build -warnaserror`)
- [ ] All tests pass (`dotnet test`)
- [ ] XML docs present on all public APIs
- [ ] One type per file
- [ ] No abstract base classes in application code
- [ ] Constructor injection used (no service locators)
- [ ] CancellationToken in all async methods
- [ ] Record types used for models
- [ ] Immutable collections returned from service methods
- [ ] All blocking review issues resolved
- [ ] Implementation notes and code review report saved
</validation-checklist>
