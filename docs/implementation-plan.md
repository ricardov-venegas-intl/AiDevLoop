# Implementation Plan

## Milestone 1 — Project Scaffolding & Domain Model

- [x] TASK-001 · Trivial · Create solution and project structure
- [x] TASK-002 · Simple · Define core domain types
- [x] TASK-003 · Simple · Define shell adapter interfaces

## Milestone 2 — Configuration & CLI Parsing

- [x] TASK-004 · Simple · Implement ConfigurationLoader
- [x] TASK-005 · Simple · Implement ConfigurationValidator
- [x] TASK-006 · Simple · Implement CommandLineParser

## Milestone 3 — Plan Parsing & Task Management

- [x] TASK-007 · Medium · Implement MarkdownPlanParser
- [x] TASK-008 · Simple · Implement TaskSelector
- [x] TASK-009 · Simple · Implement PlanUpdater

## Milestone 4 — Core Business Logic

- [ ] TASK-010 · Simple · Implement ValidationEngine
- [ ] TASK-011 · Simple · Implement ReviewAnalyzer
- [ ] TASK-012 · Simple · Implement PromptBuilder
- [ ] TASK-013 · Trivial · Implement CommitMessageBuilder
- [ ] TASK-014 · Simple · Implement StateManager

## Milestone 5 — I/O Adapters

- [ ] TASK-015 · Simple · Implement FileOperations with atomic writes
- [ ] TASK-016 · Simple · Implement ProcessRunner
- [ ] TASK-017 · Simple · Implement ConsoleIO with output modes
- [ ] TASK-018 · Simple · Implement Claude LLM Client
- [ ] TASK-019 · Simple · Implement Copilot LLM Client
- [ ] TASK-020 · Simple · Implement GitClient

## Milestone 6 — Orchestration & Commands

- [ ] TASK-021 · Simple · Implement project validation
- [ ] TASK-022 · Simple · Add default prompt templates as embedded resources
- [ ] TASK-023 · Simple · Implement TaskOrchestrator skeleton and steps 1-2
- [ ] TASK-023a · Simple · Implement TaskOrchestrator step 3 (LLM implementation)
- [ ] TASK-024 · Simple · Implement TaskOrchestrator step 4
- [ ] TASK-025 · Medium · Implement TaskOrchestrator step 5
- [ ] TASK-026 · Simple · Implement TaskOrchestrator steps 6-7
- [ ] TASK-026a · Simple · Implement TaskOrchestrator step 8 (commit with user approval)
- [ ] TASK-027 · Simple · Implement resume command

## Milestone 7 — Entry Point, Polish & Distribution

- [ ] TASK-028 · Simple · Wire Program.cs entry point with DI
- [ ] TASK-029 · Simple · Add help system and version display
- [ ] TASK-030 · Trivial · Configure single-file executable publishing
- [ ] TASK-031 · Simple · E2E test fixtures and helpers
- [ ] TASK-031a · Simple · E2E happy path and failure tests
- [ ] TASK-031b · Simple · E2E resume tests

---

## Task Definitions

## TASK-001: Create solution and project structure

**Milestone:** 1 — Project Scaffolding & Domain Model
**Status:** Completed
**Complexity:** Trivial
**Depends on:** —

### What to build

Create the .NET 10.0 solution with three source projects (AiDevLoop.Cli, AiDevLoop.Core, AiDevLoop.Shell) and three test projects (AiDevLoop.Core.Tests, AiDevLoop.Shell.Tests, AiDevLoop.E2E.Tests). Set up project references following the Functional Core / Imperative Shell boundary: Cli references Core and Shell, Shell references Core, Core has zero project references. Add a Directory.Build.props for shared build settings.

### Files in scope

- `AiDevLoop.sln` (create)
- `src/AiDevLoop.Cli/AiDevLoop.Cli.csproj` (create)
- `src/AiDevLoop.Core/AiDevLoop.Core.csproj` (create)
- `src/AiDevLoop.Shell/AiDevLoop.Shell.csproj` (create)
- `tests/AiDevLoop.Core.Tests/AiDevLoop.Core.Tests.csproj` (create)
- `tests/AiDevLoop.Shell.Tests/AiDevLoop.Shell.Tests.csproj` (create)
- `tests/AiDevLoop.E2E.Tests/AiDevLoop.E2E.Tests.csproj` (create)
- `Directory.Build.props` (create)

### Constraints

- Target framework: `net10.0`
- Nullable reference types: enabled project-wide via Directory.Build.props
- Implicit usings: enabled
- AiDevLoop.Core must have zero project references (pure functional core, no I/O)
- AiDevLoop.Shell references only AiDevLoop.Core
- AiDevLoop.Cli references both AiDevLoop.Core and AiDevLoop.Shell
- Cli project OutputType: `Exe`
- Use xUnit as test framework with `Microsoft.NET.Test.Sdk`
- Test projects reference their corresponding source projects

### Validation criteria (Definition of Done)

- [x] Solution file exists and references all 6 projects
- [x] `dotnet build` succeeds with zero errors and zero warnings
- [x] `dotnet test` executes (even with no test methods yet)
- [x] Directory.Build.props sets TargetFramework, Nullable, and ImplicitUsings
- [x] Project reference graph: Core → nothing, Shell → Core, Cli → Core + Shell
- [x] No lint/type errors

### Context references

- `docs/architecture.md#module-structure`
- `docs/requirements.md#nfr-4-compatibility`
- `docs/coding-style.md`

---

## TASK-002: Define core domain types

**Milestone:** 1 — Project Scaffolding & Domain Model
**Status:** done
**Complexity:** Simple
**Depends on:** TASK-001

### What to build

Create all domain types for the functional core: the `Result<TValue, TError>` discriminated union, value objects (`TaskId`, enums for `TaskStatus`, `Complexity`, `IssueClassification`, `Command`), data records (`TaskDefinition`, `Plan`, `Milestone`, `FileReference`, `CommandResult`, `ValidationResult`, `ReviewIssue`, `ReviewResult`, `ResumeState`, `CommandLineArgs`, `CommandLineOptions`, `Configuration`, `PathsConfiguration`, `ValidationConfiguration`, `GitStatus`), and error discriminated unions (`SelectionError` with `TaskNotFound`, `DependenciesNotMet`, `TaskNotPending`, `NoPendingTasks` variants). Each type gets its own file under `Domain/`.

### Files in scope

- `src/AiDevLoop.Core/Domain/Result.cs` (create)
- `src/AiDevLoop.Core/Domain/TaskId.cs` (create)
- `src/AiDevLoop.Core/Domain/TaskStatus.cs` (create)
- `src/AiDevLoop.Core/Domain/Complexity.cs` (create)
- `src/AiDevLoop.Core/Domain/Command.cs` (create)
- `src/AiDevLoop.Core/Domain/IssueClassification.cs` (create)
- `src/AiDevLoop.Core/Domain/TaskDefinition.cs` (create)
- `src/AiDevLoop.Core/Domain/Plan.cs` (create)
- `src/AiDevLoop.Core/Domain/Milestone.cs` (create)
- `src/AiDevLoop.Core/Domain/FileReference.cs` (create)
- `src/AiDevLoop.Core/Domain/CommandResult.cs` (create)
- `src/AiDevLoop.Core/Domain/ValidationResult.cs` (create)
- `src/AiDevLoop.Core/Domain/ReviewIssue.cs` (create)
- `src/AiDevLoop.Core/Domain/ReviewResult.cs` (create)
- `src/AiDevLoop.Core/Domain/ResumeState.cs` (create)
- `src/AiDevLoop.Core/Domain/CommandLineArgs.cs` (create)
- `src/AiDevLoop.Core/Domain/CommandLineOptions.cs` (create)
- `src/AiDevLoop.Core/Domain/Configuration.cs` (create)
- `src/AiDevLoop.Core/Domain/PathsConfiguration.cs` (create)
- `src/AiDevLoop.Core/Domain/ValidationConfiguration.cs` (create)
- `src/AiDevLoop.Core/Domain/GitStatus.cs` (create)
- `src/AiDevLoop.Core/Domain/SelectionError.cs` (create)
- `tests/AiDevLoop.Core.Tests/Domain/ResultTests.cs` (create)

### Constraints

- Use `record` for DTOs and domain entities
- Use `readonly record struct` for value objects (`TaskId`)
- Use abstract record + sealed record subtypes for discriminated unions (`Result`, `SelectionError`)
- Enable and respect nullable reference types — no suppression operators
- All collection properties use `IReadOnlyList<T>`
- All records are immutable — no mutable properties
- `TaskId` wraps a string value (e.g., `"TASK-001"`)
- `TaskStatus` enum: `Pending`, `InProgress`, `Done`, `Blocked`
- `Complexity` enum: `Trivial`, `Simple`, `Medium`, `Complex`
- `IssueClassification` enum: `Blocking`, `NonBlocking`, `Nitpick`
- `Command` enum: `Run`, `Resume`
- `Configuration` must model the schema from FR-3.2 with nested `PathsConfiguration` and `ValidationConfiguration` records, plus a static factory method returning sensible defaults
- `CommandResult(string Name, int ExitCode, string Stdout, string Stderr)` per architecture
- `ValidationResult(bool AllPassed, IReadOnlyList<CommandResult> FailedCommands, IReadOnlyList<string> Warnings)` per architecture
- `ReviewIssue(string Description, IssueClassification Classification)` per architecture
- `ReviewResult(IReadOnlyList<ReviewIssue> Issues, bool HasBlockingIssues, int IterationNumber)` per architecture
- `ResumeState(int NextStep, TaskId TaskId)` per architecture
- `GitStatus(bool HasUncommittedChanges, bool HasUnstagedChanges, IReadOnlyList<string> ModifiedFiles)` per architecture
- `FileReference(string Path, FileReferenceKind Kind)` where Kind is `Create`, `Modify`, `ReadOnlyReference`
- `SelectionError` variants: `TaskNotFound(TaskId Id)`, `DependenciesNotMet(TaskId Id, IReadOnlyList<TaskId> Unsatisfied)`, `TaskNotPending(TaskId Id, TaskStatus CurrentStatus)`, `NoPendingTasks`
- All types must have XML documentation

### Validation criteria (Definition of Done)

- [x] All domain types compile with zero nullable warnings
- [x] `Result<T, TError>` supports `Ok` and `Err` cases with pattern matching via `switch` expressions
- [x] Record equality works correctly for `TaskId` value objects
- [x] `SelectionError` discriminated union has all four variants
- [x] `Configuration.Default` factory returns config matching FR-3.2 defaults
- [x] All collection properties use `IReadOnlyList<T>`
- [x] Unit tests verify `Result` pattern matching and `TaskId` equality
- [x] No lint/type errors

### Context references

- `docs/architecture.md#component-breakdown`
- `docs/architecture.md#error-handling-strategy`
- `docs/architecture.md#command-line-interface-structure`
- `docs/architecture.md#git-workflow`
- `docs/requirements.md#fr-3-configuration`
- `docs/coding-style.md`

---

## TASK-003: Define shell adapter interfaces

**Milestone:** 1 — Project Scaffolding & Domain Model
**Status:** done
**Complexity:** Simple
**Depends on:** TASK-002

### What to build

Define the five I/O adapter interfaces that form the boundary between the orchestrator and external systems: `IFileOperations`, `IProcessRunner`, `ILLMClient`, `IGitClient`, and `IConsoleIO`. These interfaces enable testability via in-memory mock implementations in tests.

### Files in scope

- `src/AiDevLoop.Shell/Adapters/IFileOperations.cs` (create)
- `src/AiDevLoop.Shell/Adapters/IProcessRunner.cs` (create)
- `src/AiDevLoop.Shell/Adapters/ILLMClient.cs` (create)
- `src/AiDevLoop.Shell/Adapters/IGitClient.cs` (create)
- `src/AiDevLoop.Shell/Adapters/IConsoleIO.cs` (create)

### Constraints

- `IFileOperations`: synchronous methods — `ReadFile`, `WriteFile` (atomic), `CopyFile`, `MoveFile`, `CreateDirectory`, `FileExists`, `DirectoryExists`, `ListFiles`, `ArchiveContextFiles`. No async — file I/O is fast enough per architecture simplicity decisions.
- `IProcessRunner`: async methods with `CancellationToken` — `Task<CommandResult> RunAsync(string command, string arguments, CancellationToken cancellationToken)` plus an overload with working directory and verbose flag
- `ILLMClient`: async method — `Task<string> InvokeAsync(string prompt, CancellationToken cancellationToken)`. Returns LLM response text.
- `IGitClient`: async methods with `CancellationToken` — `Task StageAllAsync(CancellationToken cancellationToken)`, `Task CommitAsync(string message, CancellationToken cancellationToken)`, `Task<GitStatus> GetStatusAsync(CancellationToken cancellationToken)`. These signatures are the canonical source of truth and supersede any older synchronous `IGitClient` definitions (e.g., in `docs/architecture.md`).
- `IConsoleIO`: synchronous methods — `WriteStep`, `WriteError`, `WriteWarning`, `WriteVerbose`, `Confirm`, `PromptChoice<T>` per architecture specification
- All methods must have XML documentation
- Interface segregation: each interface has exactly one responsibility
- Use domain types from TASK-002 (`CommandResult`, `GitStatus`) in return types
- Prefix all interface names with `I`

### Validation criteria (Definition of Done)

- [ ] All five interfaces compile with zero errors
- [ ] `IProcessRunner`, `ILLMClient`, and `IGitClient` methods are async and accept `CancellationToken`
- [ ] `IConsoleIO` methods match the architecture specification signatures
- [ ] `IFileOperations` methods cover read, write, copy, move, directory, existence, archive operations
- [ ] All methods have XML documentation
- [ ] No lint/type errors

### Context references

- `docs/architecture.md#component-breakdown`
- `docs/architecture.md#console-io-interface`
- `docs/architecture.md#git-workflow`
- `docs/architecture.md#solid-principles-application`
- `docs/coding-style.md`

---

## TASK-004: Implement ConfigurationLoader

**Milestone:** 2 — Configuration & CLI Parsing
**Status:** done
**Complexity:** Simple
**Depends on:** TASK-002

### What to build

A `ConfigurationLoader` that reads configuration from `.aidevloop.json` or `.aidevloop.yaml` in the project root directory. If neither exists, return a `Configuration` record with sensible defaults. Support merging command-line overrides (`--llm`, `--verbose`, `--config`) into the loaded configuration. When `--config` is specified, load from that path instead.

### Files in scope

- `src/AiDevLoop.Cli/ConfigurationLoader.cs` (create)
- `tests/AiDevLoop.Cli.Tests/ConfigurationLoaderTests.cs` (create)

### Constraints

- Use `System.Text.Json` for JSON deserialization
- Add `YamlDotNet` NuGet package to AiDevLoop.Cli for YAML support
- Look for `.aidevloop.json` first, then `.aidevloop.yaml` — first found wins
- Default config values per FR-3.2: llm=`"claude"`, paths.docs=`"docs"`, paths.context=`"context"`, paths.prompts=`"prompts"`, validation.maxReviewIterations=`3`, verbose=`false`
- CLI overrides take precedence over file settings (merge, not replace)
- Handle missing/malformed files gracefully — return error, don't throw
- `ConfigurationLoader` is in the CLI project per architecture

### Validation criteria (Definition of Done)

- [x] Loads valid `.aidevloop.json` correctly
- [x] Loads valid `.aidevloop.yaml` correctly
- [x] Returns defaults when no config file exists
- [x] CLI `--llm` override takes precedence over file value
- [x] CLI `--verbose` override takes precedence over file value
- [x] `--config=path` loads from the specified path
- [x] Returns error for malformed JSON/YAML
- [x] No lint/type errors

### Context references

- `docs/requirements.md#fr-3-configuration`
- `docs/architecture.md#component-breakdown`
- `docs/coding-style.md`

---

## TASK-005: Implement ConfigurationValidator

**Milestone:** 2 — Configuration & CLI Parsing
**Status:** done
**Complexity:** Simple
**Depends on:** TASK-004

### What to build

A pure function that validates a `Configuration` object against the expected schema rules. Returns `Result<Configuration, IReadOnlyList<string>>` where the error case contains all validation error messages (not just the first).

### Files in scope

- `src/AiDevLoop.Core/ConfigurationValidator.cs` (create)
- `tests/AiDevLoop.Core.Tests/ConfigurationValidatorTests.cs` (create)

### Constraints

- Pure function: no I/O, no side effects
- Validate: LLM provider is `"claude"` or `"copilot"`
- Validate: `maxReviewIterations` > 0
- Validate: all path values are non-empty strings
- Validate: validation command strings are non-empty if provided (null/missing commands are acceptable — they'll be skipped)
- Accumulate all errors — return the complete list, not short-circuit on first failure
- Return `Ok(config)` when valid, `Err(errors)` when invalid

### Validation criteria (Definition of Done)

- [x] Valid configuration returns `Ok`
- [x] Invalid LLM provider (e.g., `"gpt"`) returns error
- [x] `maxReviewIterations` of 0 or negative returns error
- [x] Empty path values return errors
- [x] Multiple validation errors accumulated in single result
- [x] Null/missing validation commands do not trigger errors
- [x] No lint/type errors

### Context references

- `docs/architecture.md#component-breakdown`
- `docs/requirements.md#fr-3-configuration`
- `docs/coding-style.md`

---

## TASK-006: Implement CommandLineParser

**Milestone:** 2 — Configuration & CLI Parsing
**Status:** done
**Complexity:** Simple
**Depends on:** TASK-002

### What to build

A `CommandLineParser` that parses `string[] args` into a `CommandLineArgs` record. Support `run` and `resume` commands, an optional positional task ID argument for `run`, and flags: `--llm=`, `--config=`, `--verbose`, `--quiet`, `--from-step=`, `--help`, `--version`.

### Files in scope

- `src/AiDevLoop.Cli/CommandLineParser.cs` (create)
- `tests/AiDevLoop.Shell.Tests/CommandLineParserTests.cs` (create)

> Note: Per TASK-001, we only maintain three test projects (`Core.Tests`, `Shell.Tests`, `E2E.Tests`).  
> As an explicit, approved exception to TASK-001, `tests/AiDevLoop.Shell.Tests` will reference both `AiDevLoop.Shell` and `AiDevLoop.Cli` so that CLI-facing behavior (including `CommandLineParserTests`) is covered without introducing a separate `AiDevLoop.Cli.Tests` project and while keeping all shell/CLI surface tests together.
### Constraints

- No external CLI parsing library — implement manually with simple string parsing
- Return `Result<CommandLineArgs, string>` — error message for invalid input
- If no command specified: treat as `--help` request
- `--from-step` is only valid with the `resume` command
- `--verbose` and `--quiet` are mutually exclusive — error if both specified
- Task ID is a positional arg after `run` (e.g., `run TASK-005`)
- Flag formats: `--flag` (boolean), `--flag=value` (key-value)
- Unrecognized flags produce an error

### Validation criteria (Definition of Done)

- [x] `run` parses to `Command.Run` with no task ID
- [x] `run TASK-005` parses to `Command.Run` with `TaskId("TASK-005")`
- [x] `resume` parses to `Command.Resume`
- [x] `resume --from-step=4` parses with step number 4
- [x] `--llm=copilot` sets LLM provider override
- [x] `--verbose` sets verbose flag
- [x] `--quiet` sets quiet flag
- [x] `--verbose --quiet` together returns error
- [x] `--from-step` with `run` command returns error
- [x] Unknown flags return error
- [x] No lint/type errors

### Context references

- `docs/architecture.md#command-line-interface-structure`
- `docs/requirements.md#fr-1-command-line-interface`
- `docs/coding-style.md`

---

## TASK-007: Implement MarkdownPlanParser

**Milestone:** 3 — Plan Parsing & Task Management
**Status:** done
**Complexity:** Medium
**Depends on:** TASK-002

### What to build

A pure function that parses the content of an `implementation-plan.md` file into a `Plan` record. Extract milestone lists (headers + checkbox items) and full task definition blocks. Parse each task block into a `TaskDefinition` record with all fields: ID, title, milestone, status, complexity, dependencies, what to build, files in scope, constraints, validation criteria, and context references.

### Files in scope

- `src/AiDevLoop.Core/MarkdownPlanParser.cs` (create)
- `tests/AiDevLoop.Core.Tests/MarkdownPlanParserTests.cs` (create)

### Constraints

- Pure function: `Result<Plan, string> Parse(string markdownContent)`
- Parse milestone headers: `## Milestone N — Name`
- Parse task list items: `- [ ] TASK-XXX · Complexity · Title` (pending) and `- [x] TASK-XXX · Complexity · Title` (done)
- Parse task definition blocks starting with `## TASK-XXX: Title`
- Extract `**Status:**` field — map to `TaskStatus` enum
- Extract `**Complexity:**` field — map to `Complexity` enum
- Extract `**Depends on:**` field — parse comma-separated task IDs, handle `—` as no dependencies
- Extract section content under `### What to build`, `### Files in scope`, `### Constraints`, `### Validation criteria`, `### Context references`
- Parse `Files in scope` bullet items into `FileReference` records (path + kind from parenthetical)
- Must contain at least one task definition per FR-2.3
- Handle malformed/partial content gracefully — return error with description, don't throw

### Validation criteria (Definition of Done)

- [ ] Parses a well-formed plan with multiple milestones and tasks
- [ ] Extracts task ID, title, status, complexity, dependencies correctly
- [ ] Parses `What to build` section as prose text
- [ ] Parses `Files in scope` into `FileReference` records with path and kind
- [ ] Parses `Validation criteria` checkbox items as list of strings
- [ ] Parses `Context references` as list of strings
- [ ] Handles `Depends on: —` (no dependencies) as empty list
- [ ] Returns error for empty plan content
- [ ] Returns error for plan with no task definitions
- [ ] No lint/type errors

### Context references

- `docs/LLM-Assisted-Development-Methodology.md#implementation-planmd--format-spec`
- `docs/requirements.md#fr-2-project-validation`
- `docs/coding-style.md`

---

## TASK-008: Implement TaskSelector

**Milestone:** 3 — Plan Parsing & Task Management
**Status:** Completed
**Complexity:** Simple
**Depends on:** TASK-007

### What to build

A pure function that selects the next task to execute from a `Plan`. If no task ID is provided, auto-select the first `pending` task whose dependencies are all `done`. If a task ID is provided, validate it exists, is `pending`, and has all dependencies satisfied. Return the full `TaskDefinition` on success or a typed `SelectionError` on failure.

### Files in scope

- `src/AiDevLoop.Core/TaskSelector.cs` (create)
- `tests/AiDevLoop.Core.Tests/TaskSelectorTests.cs` (create)

### Constraints

- Pure function: `Result<TaskDefinition, SelectionError> SelectTask(Plan plan, TaskId? taskId)`
- Auto-select order: milestone order first, then task ID order within a milestone
- A dependency is "satisfied" when the dependency task's status is `Done`
- Error cases use the discriminated union variants:
  - `TaskNotFound` when specified ID doesn't exist in plan
  - `TaskNotPending` when specified task isn't in `Pending` status
  - `DependenciesNotMet` with the list of unsatisfied task IDs
  - `NoPendingTasks` when auto-select finds no eligible task
- Do not mutate the Plan — return the selected TaskDefinition only

### Validation criteria (Definition of Done)

- [x] Auto-selects first pending task when all its dependencies are done
- [x] Skips tasks with unsatisfied dependencies during auto-select
- [x] Selects specific task by ID when provided
- [x] Returns `TaskNotFound` for non-existent task ID
- [x] Returns `TaskNotPending` when task is already `Done` or `InProgress`
- [x] Returns `DependenciesNotMet` with the list of unsatisfied dependency IDs
- [x] Returns `NoPendingTasks` when no eligible tasks remain
- [x] No lint/type errors

### Context references

- `docs/architecture.md#component-breakdown`
- `docs/requirements.md#fr-4-task-execution`
- `docs/coding-style.md`

---

## TASK-009: Implement PlanUpdater

**Milestone:** 3 — Plan Parsing & Task Management
**Status:** Completed
**Complexity:** Simple
**Depends on:** TASK-007

### What to build

A pure function that updates task status in the raw markdown content of `implementation-plan.md`. Changes both the milestone checkbox line (`- [ ]` ↔ `- [x]`) and the `**Status:**` field inside the task definition block. Returns the updated markdown string.

### Files in scope

- `src/AiDevLoop.Core/PlanUpdater.cs` (create)
- `tests/AiDevLoop.Core.Tests/PlanUpdaterTests.cs` (create)

### Constraints

- Pure function: `string UpdateTaskStatus(string planContent, TaskId taskId, TaskStatus newStatus)`
- Checkbox: `- [ ]` for pending/in-progress/blocked, `- [x]` for done
- Status field: replace the value after `**Status:**` with the new status lowercase (`pending`, `in-progress`, `done`, `blocked`)
- Deterministic text manipulation — match on task ID to locate the correct lines
- Preserve all other content unchanged (no accidental whitespace changes)
- Per ADR-003: simple text replacement, no LLM involvement

### Validation criteria (Definition of Done)

- [ ] Changes checkbox from `[ ]` to `[x]` when marking as `Done`
- [ ] Changes checkbox from `[x]` to `[ ]` when marking as `InProgress` or `Pending`
- [ ] Updates `**Status:**` field text in the task definition block
- [ ] Preserves all other document content unchanged
- [ ] Handles task at any position in the document (first, middle, last)
- [ ] No lint/type errors

### Context references

- `docs/architecture.md#adr-003-text-based-plan-updates`
- `docs/requirements.md#fr-4-task-execution`
- `docs/coding-style.md`

---

## TASK-010: Implement ValidationEngine

**Milestone:** 4 — Core Business Logic
**Status:** pending
**Complexity:** Simple
**Depends on:** TASK-002

### What to build

A pure function that analyzes a list of `CommandResult` objects from validation commands (lint, typecheck, test, build) and produces a `ValidationResult`. Any command with a non-zero exit code is a failure. Collect all failures and any warnings (commands with stderr output despite success).

### Files in scope

- `src/AiDevLoop.Core/ValidationEngine.cs` (create)
- `tests/AiDevLoop.Core.Tests/ValidationEngineTests.cs` (create)

### Constraints

- Pure function: `ValidationResult Validate(IReadOnlyList<CommandResult> commandResults)`. This signature supersedes the older `ValidationResult Validate(TaskDefinition task, List<CommandResult> commandResults)` signature shown in `docs/architecture.md` — the implementation plan is the canonical source of truth.
- `ExitCode != 0` means the command failed
- `AllPassed` is true only when every command has `ExitCode == 0`
- `FailedCommands` contains all `CommandResult` objects with non-zero exit codes
- `Warnings` includes stderr content from commands that passed but had non-empty stderr
- Handle empty command list gracefully (no validation commands = all passed, with a warning)

### Validation criteria (Definition of Done)

- [ ] Returns `AllPassed=true` when all commands succeed (exit code 0)
- [ ] Returns `AllPassed=false` when any command fails
- [ ] `FailedCommands` contains all failed `CommandResult` objects
- [ ] `Warnings` includes stderr content from passing commands with stderr output
- [ ] Empty command list returns `AllPassed=true` with warning about no validation
- [ ] No lint/type errors

### Context references

- `docs/architecture.md#component-breakdown`
- `docs/requirements.md#fr-4-task-execution`
- `docs/coding-style.md`

---

## TASK-011: Implement ReviewAnalyzer

**Milestone:** 4 — Core Business Logic
**Status:** pending
**Complexity:** Simple
**Depends on:** TASK-002

### What to build

A pure function that parses a `review.md` document produced by the review agent. Extract issues with their classifications (`BLOCKING`, `NON-BLOCKING`, `NITPICK`), detect the `APPROVED` marker at the top of the document, and determine whether the review passes (no blocking issues exist).

### Files in scope

- `src/AiDevLoop.Core/ReviewAnalyzer.cs` (create)
- `tests/AiDevLoop.Core.Tests/ReviewAnalyzerTests.cs` (create)

### Constraints

- Pure function: `ReviewResult AnalyzeReview(string reviewDocument, int iterationNumber)`
- Detect `APPROVED` (case-insensitive) at the top of the document as an immediate pass
- Parse lines/sections containing classification markers: `BLOCKING`, `NON-BLOCKING`, `NITPICK`
- Each issue consists of a description and its classification
- `HasBlockingIssues` is `true` if any issue is classified as `Blocking`
- Track `IterationNumber` in the result for max-iteration logic
- Handle empty, null, or malformed review documents gracefully — treat as no issues (pass)
- Be tolerant of varied formatting from LLM agents (markers may appear in different casing or formats)

### Validation criteria (Definition of Done)

- [ ] Detects `APPROVED` at document top as pass (no blocking issues)
- [ ] Extracts `BLOCKING` issues correctly
- [ ] Extracts `NON-BLOCKING` issues correctly
- [ ] Extracts `NITPICK` issues correctly
- [ ] `HasBlockingIssues` is true only when `BLOCKING` issues exist
- [ ] Returns correct `IterationNumber` in result
- [ ] Handles empty/malformed review document gracefully
- [ ] Handles varied casing of classification markers
- [ ] No lint/type errors

### Context references

- `docs/architecture.md#component-breakdown`
- `docs/requirements.md#fr-4-task-execution`
- `docs/LLM-Assisted-Development-Methodology.md#code-reviewpromptmd`
- `docs/coding-style.md`

---

## TASK-012: Implement PromptBuilder

**Milestone:** 4 — Core Business Logic
**Status:** pending
**Complexity:** Simple
**Depends on:** TASK-002

### What to build

A pure function that constructs LLM agent prompts by combining a prompt template string with the task definition content and pre-loaded reference file contents. The shell pre-loads all files into a dictionary before calling this function. Output a single concatenated string suitable for passing to the LLM CLI.

### Files in scope

- `src/AiDevLoop.Core/PromptBuilder.cs` (create)
- `tests/AiDevLoop.Core.Tests/PromptBuilderTests.cs` (create)

### Constraints

- Pure function: `string BuildPrompt(string promptTemplate, string taskContent, IReadOnlyDictionary<string, string> loadedFiles, IReadOnlyList<string> contextReferences)`
- Concatenation order: prompt template → task content → referenced doc contents
- Separate each section with a markdown horizontal rule (`---`) and a header identifying the source file
- Include only files listed in `contextReferences` from the `loadedFiles` dictionary
- Skip missing references gracefully — insert a comment noting the file was not found
- For review/fix prompts: code diff is passed as an entry in `loadedFiles` with a known key (e.g., `"code-diff"`)

### Validation criteria (Definition of Done)

- [ ] Builds prompt with template + task + referenced files in correct order
- [ ] Sections separated by clear delimiters with source identification
- [ ] Includes only files matching context references
- [ ] Skips missing reference files with a placeholder comment
- [ ] Handles empty context references list (template + task only)
- [ ] Output is a valid string
- [ ] No lint/type errors

### Context references

- `docs/architecture.md#component-breakdown`
- `docs/LLM-Assisted-Development-Methodology.md#reusable-prompts`
- `docs/coding-style.md`

---

## TASK-013: Implement CommitMessageBuilder

**Milestone:** 4 — Core Business Logic
**Status:** pending
**Complexity:** Trivial
**Depends on:** TASK-002

### What to build

A pure function that generates a conventional commit message from a `TaskDefinition`. Format: `feat(TASK-XXX): <task title>` where the task title is cleaned and lowercased for the commit message subject line.

### Files in scope

- `src/AiDevLoop.Core/CommitMessageBuilder.cs` (create)
- `tests/AiDevLoop.Core.Tests/CommitMessageBuilderTests.cs` (create)

### Constraints

- Pure function: `string GenerateCommitMessage(TaskDefinition task)`
- Conventional commit format: `feat(<task-id>): <title>`
- Use `feat` as the default type (sufficient for MVP)
- Title should be lowercased for the commit subject
- Maximum 72 characters for the subject line — truncate with `...` if exceeded
- No trailing period on the subject line

### Validation criteria (Definition of Done)

- [ ] Generates `feat(TASK-001): add post /auth/login endpoint` format
- [ ] Includes task ID in scope
- [ ] Title is lowercased
- [ ] Truncates to 72 characters with `...` for long titles
- [ ] No trailing period
- [ ] No lint/type errors

### Context references

- `docs/architecture.md#component-breakdown`
- `docs/requirements.md#fr-4-task-execution`
- `docs/coding-style.md`

---

## TASK-014: Implement StateManager

**Milestone:** 4 — Core Business Logic
**Status:** pending
**Complexity:** Simple
**Depends on:** TASK-002

### What to build

A pure function that determines the resume point based on which context files exist and the task's current status in the plan. Used by the `resume` command to decide which development loop step to continue from. Also includes a helper to extract the task ID from `current-task.md` content.

### Files in scope

- `src/AiDevLoop.Core/StateManager.cs` (create)
- `tests/AiDevLoop.Core.Tests/StateManagerTests.cs` (create)

### Constraints

- Pure function: `Result<ResumeState, string> DetermineResumePoint(bool currentTaskExists, bool implementationNotesExists, bool reviewExists, TaskStatus statusInPlan)`
- Also: `Result<TaskId, string> ExtractTaskId(string currentTaskContent)` — parse task ID from the `## TASK-XXX:` header in current-task.md
- Resume logic per architecture:
  - No `current-task.md` → error `"No task in progress"`
  - `current-task.md` only → resume from step 3 (implement)
  - `implementation-notes.md` exists, no `review.md` → resume from step 4 (validate)
  - `review.md` exists → resume from step 6 (integration check)
- Status `Pending` with existing `current-task.md` → resume from step 3 (status wasn't updated yet)

### Validation criteria (Definition of Done)

- [ ] Returns error when no `current-task.md` exists
- [ ] Returns step 3 when only `current-task.md` exists
- [ ] Returns step 4 when `implementation-notes.md` exists but `review.md` missing
- [ ] Returns step 6 when `review.md` exists
- [ ] `ExtractTaskId` parses task ID from markdown header
- [ ] `ExtractTaskId` returns error for malformed content
- [ ] No lint/type errors

### Context references

- `docs/architecture.md#component-breakdown`
- `docs/requirements.md#fr-5-resume-execution`
- `docs/coding-style.md`

---

## TASK-015: Implement FileOperations with atomic writes

**Milestone:** 5 — I/O Adapters
**Status:** pending
**Complexity:** Simple
**Depends on:** TASK-003

### What to build

Concrete implementation of `IFileOperations`. Read files as strings, write files atomically (write to temp file then move), create directories recursively, copy and move files, check existence, list directory contents, and archive context files to `context/completed/{TASK-ID}/`.

### Files in scope

- `src/AiDevLoop.Shell/Adapters/FileOperations.cs` (create)
- `tests/AiDevLoop.Shell.Tests/FileOperationsTests.cs` (create)

### Constraints

- Atomic writes per ADR-006: write content to a temp file in the same directory, then `File.Move` with overwrite to the target path
- Create parent directories automatically (`Directory.CreateDirectory`) before writing
- Cross-platform path handling: use `Path.Combine`, never hardcode separators
- UTF-8 encoding for all text file operations, no BOM
- Archive operation copies `current-task.md`, `implementation-notes.md`, and `review.md` to `context/completed/{TASK-ID}/`
- `ListFiles` returns relative paths within the specified directory
- Handle file-not-found by returning a clear error message, not an unhandled exception

### Validation criteria (Definition of Done)

- [ ] `ReadFile` returns file content as string
- [ ] `WriteFile` uses atomic write (creates temp file, then moves)
- [ ] `WriteFile` creates parent directories if they don't exist
- [ ] `FileExists` and `DirectoryExists` return correct results
- [ ] `CreateDirectory` creates nested directory structure
- [ ] `ArchiveContextFiles` copies all three context files to `completed/{TASK-ID}/`
- [ ] Cross-platform paths work correctly
- [ ] No lint/type errors

### Context references

- `docs/architecture.md#adr-006-atomic-file-writes`
- `docs/architecture.md#component-breakdown`
- `docs/requirements.md#nfr-2-reliability`
- `docs/coding-style.md`

---

## TASK-016: Implement ProcessRunner

**Milestone:** 5 — I/O Adapters
**Status:** pending
**Complexity:** Simple
**Depends on:** TASK-003

### What to build

Concrete implementation of `IProcessRunner`. Execute external commands asynchronously using `System.Diagnostics.Process`, capture stdout and stderr separately, and return a `CommandResult` with the exit code. Limit captured output to prevent memory issues with large command output.

### Files in scope

- `src/AiDevLoop.Shell/Adapters/ProcessRunner.cs` (create)
- `tests/AiDevLoop.Shell.Tests/ProcessRunnerTests.cs` (create)

### Constraints

- Use `System.Diagnostics.Process` with `RedirectStandardOutput` and `RedirectStandardError`
- Async execution with `CancellationToken` support — kill process on cancellation
- Capture stdout and stderr into separate strings
- Limit captured output to last 500 lines in non-verbose mode per architecture performance section
- Set `WorkingDirectory` to the project root (passed as parameter or injected)
- Handle command-not-found by catching `Win32Exception` / `FileNotFoundException` and returning a descriptive `CommandResult` with non-zero exit code
- Do not use shell execution (`UseShellExecute = false`)

### Validation criteria (Definition of Done)

- [ ] Executes a command and captures stdout
- [ ] Captures stderr separately from stdout
- [ ] Returns correct exit code
- [ ] Respects `CancellationToken` for cancellation (kills process)
- [ ] Limits output to 500 lines in non-verbose mode
- [ ] Handles command-not-found gracefully
- [ ] No lint/type errors

### Context references

- `docs/architecture.md#component-breakdown`
- `docs/architecture.md#performance-considerations`
- `docs/requirements.md#fr-4-task-execution`
- `docs/coding-style.md`

---

## TASK-017: Implement ConsoleIO with output modes

**Milestone:** 5 — I/O Adapters
**Status:** pending
**Complexity:** Simple
**Depends on:** TASK-003

### What to build

Concrete implementation of `IConsoleIO`. Display output respecting three modes: Normal, Verbose, and Quiet. Show step progress, errors, warnings, verbose detail, and interactive user prompts. Use ANSI colors when the terminal supports them.

### Files in scope

- `src/AiDevLoop.Shell/Adapters/ConsoleIO.cs` (create)
- `tests/AiDevLoop.Shell.Tests/ConsoleIOTests.cs` (create)

### Constraints

- Constructor takes `OutputMode` enum or two bools (`verbose`, `quiet`) to configure behavior
- `WriteStep`: shown in Normal and Verbose, hidden in Quiet — format as `[N/8] Step Name → details`
- `WriteError`: always shown in all modes — red text if color supported
- `WriteWarning`: always shown in all modes — yellow text if color supported
- `WriteVerbose`: only shown in Verbose mode — dimmed or prefixed with `[VERBOSE]`
- `Confirm`: always shown, accepts `y`/`n` input, has a default value, returns `bool`
- `PromptChoice<T>`: always shown, displays numbered options, returns selected value
- Use `Console.IsOutputRedirected` to detect ANSI color support
- Tests should use a `TextWriter` injection to verify output without actual console

### Validation criteria (Definition of Done)

- [ ] `WriteStep` displays in Normal mode, hidden in Quiet mode
- [ ] `WriteError` displays in all modes
- [ ] `WriteVerbose` displays only in Verbose mode
- [ ] `Confirm` prompts user with default value and returns response
- [ ] `PromptChoice<T>` displays numbered options and returns selection
- [ ] Step progress format: `[N/8] Step Name → details`
- [ ] Color output when terminal supports ANSI
- [ ] No lint/type errors

### Context references

- `docs/architecture.md#console-io-interface`
- `docs/requirements.md#fr-7-output-and-logging`
- `docs/coding-style.md`

---

## TASK-018: Implement Claude LLM Client

**Milestone:** 5 — I/O Adapters
**Status:** pending
**Complexity:** Simple
**Depends on:** TASK-003, TASK-016

### What to build

Implement `ILLMClient` for the Anthropic Claude CLI. Construct the `claude` command with the appropriate flags to pass a prompt, execute it via `IProcessRunner`, and return the LLM response from stdout. Handle failures according to FR-6.3.

### Files in scope

- `src/AiDevLoop.Shell/Adapters/ClaudeLLMClient.cs` (create)
- `tests/AiDevLoop.Shell.Tests/ClaudeLLMClientTests.cs` (create)

### Constraints

- Depend on `IProcessRunner` (injected via constructor) — do not create processes directly
- Construct the command to pass the prompt via stdin or a temp file (depending on Claude CLI capabilities)
- Use `--print` or equivalent flag to get raw text output (not interactive mode)
- Handle non-zero exit code: throw or return error per FR-6.3
- Handle empty/malformed output as an error condition
- Log the full prompt and response when verbose mode is enabled (pass-through to the process runner, the orchestrator handles verbose logging)

### Validation criteria (Definition of Done)

- [ ] Constructs correct `claude` CLI command
- [ ] Passes prompt content to the CLI tool
- [ ] Captures LLM response from stdout
- [ ] Returns error when CLI returns non-zero exit code
- [ ] Returns error for empty LLM output
- [ ] Uses `IProcessRunner` for process execution (testable with mock)
- [ ] No lint/type errors

### Context references

- `docs/architecture.md#adr-005-llm-client-abstraction`
- `docs/architecture.md#llm-automation-model`
- `docs/requirements.md#fr-6-llm-agent-integration`
- `docs/coding-style.md`

---

## TASK-019: Implement Copilot LLM Client

**Milestone:** 5 — I/O Adapters
**Status:** pending
**Complexity:** Simple
**Depends on:** TASK-016

### What to build

Implement `ILLMClient` for the GitHub Copilot CLI. Same pattern as `ClaudeLLMClient` but targeting the `copilot` CLI tool. If the two implementations share significant logic, extract a common base helper method or shared utility.

### Files in scope

- `src/AiDevLoop.Shell/Adapters/CopilotLLMClient.cs` (create)
- `tests/AiDevLoop.Shell.Tests/CopilotLLMClientTests.cs` (create)

### Constraints

- Follow the same `IProcessRunner`-based pattern as `ClaudeLLMClient`
- Construct command for `copilot` CLI with appropriate flags
- Same error handling: non-zero exit code and empty output are error conditions
- If substantial code overlaps with `ClaudeLLMClient`, extract shared logic into a private helper (prefer composition over inheritance)

### Validation criteria (Definition of Done)

- [ ] Constructs correct `copilot` CLI command with prompt
- [ ] Passes prompt content to the CLI tool
- [ ] Captures LLM response from stdout
- [ ] Returns error when CLI returns non-zero exit code
- [ ] Returns error for empty output
- [ ] No lint/type errors

### Context references

- `docs/architecture.md#adr-005-llm-client-abstraction`
- `docs/requirements.md#fr-6-llm-agent-integration`
- `docs/coding-style.md`

---

## TASK-020: Implement GitClient

**Milestone:** 5 — I/O Adapters
**Status:** pending
**Complexity:** Simple
**Depends on:** TASK-003, TASK-016

### What to build

Implement `IGitClient` using `IProcessRunner` to execute git commands. Stage all changes (`git add .`), commit with a provided message (`git commit -m`), and get repository status by parsing `git status --porcelain` output into a `GitStatus` record.

### Files in scope

- `src/AiDevLoop.Shell/Adapters/GitClient.cs` (create)
- `tests/AiDevLoop.Shell.Tests/GitClientTests.cs` (create)

### Constraints

- Use `IProcessRunner` for all git commands (dependency injection)
- `StageAllAsync`: execute `git add .`
- `CommitAsync`: execute `git commit -m "<message>"` — use git's configured user.name/email
- `GetStatusAsync`: execute `git status --porcelain`, parse output into `GitStatus` record
- Detect detached HEAD via `git symbolic-ref --short HEAD` (non-zero exit = detached)
- Warn (not error) on pre-existing uncommitted changes before staging
- Async with `CancellationToken` on all methods

### Validation criteria (Definition of Done)

- [ ] `StageAllAsync` executes `git add .`
- [ ] `CommitAsync` executes `git commit -m` with the provided message
- [ ] `GetStatusAsync` parses porcelain output into `GitStatus` fields correctly
- [ ] Detects detached HEAD state
- [ ] Handles clean repo (no changes) correctly
- [ ] No lint/type errors

### Context references

- `docs/architecture.md#git-workflow`
- `docs/requirements.md#fr-4-task-execution`
- `docs/coding-style.md`

---

## TASK-021: Implement project validation

**Milestone:** 6 — Orchestration & Commands
**Status:** pending
**Complexity:** Simple
**Depends on:** TASK-015, TASK-017, TASK-004

### What to build

A `ProjectValidator` that verifies Phase 1 document presence (mandatory = error, optional = warning), creates required directories (`context/`, `context/completed/`), and delegates prompt template initialization to `IFileOperations`. Called by the orchestrator before any task execution begins.

### Files in scope

- `src/AiDevLoop.Shell/ProjectValidator.cs` (create)
- `tests/AiDevLoop.Shell.Tests/ProjectValidatorTests.cs` (create)

### Constraints

- Mandatory docs (error if missing): `requirements.md`, `architecture.md`, `implementation-plan.md` — located under the configured `docs` path
- Optional docs (warning via `IConsoleIO.WriteWarning`): `data-models.md`, `ui-design.md`, `test-strategy.md`, `code-style.md`
- Create `context/` and `context/completed/` directories if they don't exist
- Create `prompts/` directory and initialize default templates if missing (delegate to TASK-022's `InitializePrompts`)
- Respect configured paths from `Configuration.Paths`
- Return a result indicating success or listing all missing mandatory documents

### Validation criteria (Definition of Done)

- [ ] Errors when any mandatory doc is missing, with clear error listing the expected path
- [ ] Warns (does not error) when optional docs are missing
- [ ] Creates `context/` directory if missing
- [ ] Creates `context/completed/` directory if missing
- [ ] Initializes prompt templates if `prompts/` is missing
- [ ] Respects configured paths (not hardcoded `"docs"`)
- [ ] No lint/type errors

### Context references

- `docs/requirements.md#fr-2-project-validation`
- `docs/architecture.md#component-breakdown`
- `docs/coding-style.md`

---

## TASK-022: Add default prompt templates as embedded resources

**Milestone:** 6 — Orchestration & Commands
**Status:** pending
**Complexity:** Simple
**Depends on:** TASK-015

### What to build

Create the three default prompt template files (`implement-task.prompt.md`, `code-review.prompt.md`, `fix-issues.prompt.md`) as embedded resources in the Shell project. Add an `InitializePrompts` method to `FileOperations` that extracts these templates to the configured `prompts/` directory if the files don't already exist on disk.

### Files in scope

- `src/AiDevLoop.Shell/Resources/implement-task.prompt.md` (create)
- `src/AiDevLoop.Shell/Resources/code-review.prompt.md` (create)
- `src/AiDevLoop.Shell/Resources/fix-issues.prompt.md` (create)
- `src/AiDevLoop.Shell/AiDevLoop.Shell.csproj` (modify — add `<EmbeddedResource>` items)
- `src/AiDevLoop.Shell/Adapters/FileOperations.cs` (modify — add `InitializePrompts` method)
- `src/AiDevLoop.Shell/Adapters/IFileOperations.cs` (modify — add `InitializePrompts` to interface)
- `tests/AiDevLoop.Shell.Tests/PromptTemplateTests.cs` (create)

### Constraints

- Template content must match the specifications from the Reusable Prompts section of the methodology doc
- Mark resource files as `EmbeddedResource` in `.csproj`
- `InitializePrompts` checks each file individually — do not overwrite user-customized prompts that already exist
- Use `Assembly.GetManifestResourceStream` to read embedded resources
- The method accepts the target prompts directory path as a parameter

### Validation criteria (Definition of Done)

- [ ] All three prompt template files are embedded in the assembly
- [ ] Template content matches the methodology specification
- [ ] `InitializePrompts` creates files in the prompts directory
- [ ] Existing prompt files are not overwritten
- [ ] Resources load correctly from the assembly
- [ ] No lint/type errors

### Context references

- `docs/LLM-Assisted-Development-Methodology.md#reusable-prompts`
- `docs/architecture.md#component-breakdown`
- `docs/coding-style.md`

---

## TASK-023: Implement TaskOrchestrator skeleton and steps 1-2

**Milestone:** 6 — Orchestration & Commands
**Status:** pending
**Complexity:** Simple
**Depends on:** TASK-003, TASK-008, TASK-022

### What to build

Create the `TaskOrchestrator` class that coordinates the development loop. Implement steps 1 (Select Task) and 2 (Load Task). Step 1 loads the plan, parses it, and selects a task. Step 2 copies the task block to `context/current-task.md` and creates an `implementation-notes.md` template. This task establishes the orchestrator skeleton with its constructor, DI dependencies, and `RunAsync` entry point.

### Files in scope

- `src/AiDevLoop.Shell/TaskOrchestrator.cs` (create)
- `tests/AiDevLoop.Shell.Tests/TaskOrchestratorTests.cs` (create)

### Constraints

- Constructor receives all adapter interfaces (`IFileOperations`, `IProcessRunner`, `ILLMClient`, `IGitClient`, `IConsoleIO`) plus `Configuration` via DI
- Provide a `RunAsync(TaskId? taskId, CancellationToken)` entry point
- Step 1: read plan via `IFileOperations`, parse via `MarkdownPlanParser`, select via `TaskSelector`
- Step 2: write task block to `context/current-task.md`, create `context/implementation-notes.md` with the Context Handoff template headings (Decisions made, Risk areas, Known limitations)
- Display progress via `IConsoleIO.WriteStep` (e.g., `[1/8] Select Task → TASK-005`)
- Map core `SelectionError` variants to user-friendly error messages

### Validation criteria (Definition of Done)

- [ ] Step 1 selects the correct task (auto or by ID)
- [ ] Step 2 writes `current-task.md` with the full task block content
- [ ] Step 2 creates `implementation-notes.md` with Context Handoff template
- [ ] Progress displayed via `ConsoleIO` for each step
- [ ] Core `SelectionError` variants produce user-friendly messages
- [ ] No lint/type errors

### Context references

- `docs/architecture.md#component-breakdown`
- `docs/architecture.md#data-flow-pipeline`
- `docs/requirements.md#fr-4-task-execution`
- `docs/LLM-Assisted-Development-Methodology.md#phase-2--development-loop-repeat-per-task`
- `docs/coding-style.md`

---

## TASK-023a: Implement TaskOrchestrator step 3 (LLM implementation)

**Milestone:** 6 — Orchestration & Commands
**Status:** pending
**Complexity:** Simple
**Depends on:** TASK-023, TASK-012

### What to build

Add step 3 (Implement) to the `TaskOrchestrator`. Load all files referenced in the task's context references, build the implementation prompt via `PromptBuilder`, invoke the LLM agent via `ILLMClient`, and update task status to `in-progress`.

### Files in scope

- `src/AiDevLoop.Shell/TaskOrchestrator.cs` (modify)
- `tests/AiDevLoop.Shell.Tests/TaskOrchestratorTests.cs` (modify)

### Constraints

- Load all files from task's context references via `IFileOperations`
- Build prompt via `PromptBuilder` with template + task content + loaded files
- Invoke `ILLMClient.InvokeAsync` with the built prompt
- Update task status to `InProgress` via `PlanUpdater` + `IFileOperations`
- Display progress: `[3/8] Implement → invoking LLM agent`

### Validation criteria (Definition of Done)

- [ ] Loads all referenced docs and builds the correct prompt
- [ ] Invokes the LLM agent with the built prompt
- [ ] Task status updated to `in-progress` in `implementation-plan.md`
- [ ] Progress displayed via `ConsoleIO`
- [ ] No lint/type errors

### Context references

- `docs/architecture.md#data-flow-pipeline`
- `docs/architecture.md#llm-automation-model`
- `docs/requirements.md#fr-4-task-execution`
- `docs/coding-style.md`

---

## TASK-024: Implement TaskOrchestrator step 4

**Milestone:** 6 — Orchestration & Commands
**Status:** pending
**Complexity:** Simple
**Depends on:** TASK-023a, TASK-010

### What to build

Add step 4 (Automated Validation) to the `TaskOrchestrator`. Execute all validation commands from the configuration (`lint`, `typecheck`, `test`, `build`) via `IProcessRunner`. Analyze results via `ValidationEngine`. On failure, prompt the user with interactive failure handling: continue review loop, skip to commit, or abort.

### Files in scope

- `src/AiDevLoop.Shell/TaskOrchestrator.cs` (modify)
- `tests/AiDevLoop.Shell.Tests/TaskOrchestratorTests.cs` (modify)

### Constraints

- Run each validation command defined in `Configuration.Validation.Commands` sequentially
- Skip commands that are null/empty in config — log a warning via `IConsoleIO.WriteWarning`
- Pass each `CommandResult` to `ValidationEngine.Validate`
- On failure: display error summary (failing commands + output excerpt), prompt user per FR-4.3:
  - `y` — continue to the review loop (step 5)
  - `n` — skip to step 8 (commit) with warnings about skipped validation
  - `abort` — exit without committing, preserve state for `resume`
- Display progress: `[4/8] Automated Validation → lint, typecheck, test, build`
- Validation step is reusable (called again inside the review loop after fixes)

### Validation criteria (Definition of Done)

- [ ] Runs all configured validation commands sequentially
- [ ] Skips undefined/empty commands with warning
- [ ] Passes when all commands succeed
- [ ] Displays error summary on failure
- [ ] Prompts user with `y`/`n`/`abort` on failure
- [ ] `y` continues to review, `n` skips to commit, `abort` exits with state preserved
- [ ] Validation method is reusable by the review loop
- [ ] No lint/type errors

### Context references

- `docs/requirements.md#fr-4-task-execution`
- `docs/architecture.md#data-flow-pipeline`
- `docs/coding-style.md`

---

## TASK-025: Implement TaskOrchestrator step 5

**Milestone:** 6 — Orchestration & Commands
**Status:** pending
**Complexity:** Medium
**Depends on:** TASK-024, TASK-011

### What to build

Add step 5 (Review Loop) to the `TaskOrchestrator`. Loop up to `maxReviewIterations`: invoke the review agent with the review prompt, parse `context/review.md` via `ReviewAnalyzer`, invoke the fix agent if blocking issues exist, then re-run validation (step 4). Exit when no blocking issues remain. Prompt the user if the iteration limit is exhausted with blocking issues still present.

### Files in scope

- `src/AiDevLoop.Shell/TaskOrchestrator.cs` (modify)
- `tests/AiDevLoop.Shell.Tests/TaskOrchestratorTests.cs` (modify)

### Constraints

- Build review prompt via `PromptBuilder` with: task content + `implementation-notes.md` + code diff (from `git diff` via `IProcessRunner`)
- Invoke review agent via `ILLMClient`, review agent writes `context/review.md`
- Parse `context/review.md` via `ReviewAnalyzer`
- If no `BLOCKING` issues: exit loop, proceed to step 6
- If `BLOCKING` issues: build fix prompt, invoke fix agent via `ILLMClient`, fix agent updates code and `implementation-notes.md`, re-run step 4 (validation)
- After `maxReviewIterations` exhausted with blocking issues: prompt user per FR-4.3:
  - `y` — run one more review iteration
  - `n` — skip to step 8 with warnings
  - `abort` — exit, preserve state
- Display iteration count: `[5/8] Review Loop — iteration 1/3`

### Validation criteria (Definition of Done)

- [ ] Invokes review agent with correct prompt (task + notes + diff)
- [ ] Reads and parses `review.md` for issue classifications
- [ ] Exits loop when no blocking issues found
- [ ] Invokes fix agent when blocking issues exist
- [ ] Re-runs validation after fix
- [ ] Loops up to `maxReviewIterations`
- [ ] Prompts user when iteration limit exhausted
- [ ] Non-blocking issues logged but don't block progress
- [ ] No lint/type errors

### Context references

- `docs/requirements.md#fr-4-task-execution`
- `docs/architecture.md#data-flow-pipeline`
- `docs/architecture.md#llm-automation-model`
- `docs/LLM-Assisted-Development-Methodology.md#code-reviewpromptmd`
- `docs/LLM-Assisted-Development-Methodology.md#fix-issuespromptmd`
- `docs/coding-style.md`

---

## TASK-026: Implement TaskOrchestrator steps 6-7

**Milestone:** 6 — Orchestration & Commands
**Status:** pending
**Complexity:** Simple
**Depends on:** TASK-025, TASK-009

### What to build

Add steps 6 (Integration Check) and 7 (Update Documentation) to the `TaskOrchestrator`. Step 6 runs the full test suite and build by reusing validation logic from step 4. Step 7 archives context files to `context/completed/{TASK-ID}/` and updates the plan status.

### Files in scope

- `src/AiDevLoop.Shell/TaskOrchestrator.cs` (modify)
- `tests/AiDevLoop.Shell.Tests/TaskOrchestratorTests.cs` (modify)

### Constraints

- Step 6: run `test` and `build` commands from config via `IProcessRunner` — reuse validation logic from step 4
- Step 7: archive `current-task.md`, `implementation-notes.md`, `review.md` to `context/completed/{TASK-ID}/` via `IFileOperations.ArchiveContextFiles`. Clean up context files after archiving.
- Display progress: `[6/8] Integration Check`, `[7/8] Update Documentation`

### Validation criteria (Definition of Done)

- [ ] Step 6 runs full test suite and build commands
- [ ] Step 7 archives all three context files to `completed/{TASK-ID}/`
- [ ] Step 7 cleans up context files after archiving
- [ ] Progress displayed via `ConsoleIO` for each step
- [ ] No lint/type errors

### Context references

- `docs/requirements.md#fr-4-task-execution`
- `docs/architecture.md#data-flow-pipeline`
- `docs/coding-style.md`

---

## TASK-026a: Implement TaskOrchestrator step 8 (commit with user approval)

**Milestone:** 6 — Orchestration & Commands
**Status:** pending
**Complexity:** Simple
**Depends on:** TASK-026, TASK-013, TASK-020

### What to build

Add step 8 (Commit) to the `TaskOrchestrator`. Generate the commit message via `CommitMessageBuilder`, display it to the user, pause for mandatory approval, then stage and commit on approval or abort on rejection. This is the single mandatory human checkpoint — it must never be skipped.

### Files in scope

- `src/AiDevLoop.Shell/TaskOrchestrator.cs` (modify)
- `tests/AiDevLoop.Shell.Tests/TaskOrchestratorTests.cs` (modify)

### Constraints

- Generate commit message via `CommitMessageBuilder`
- Display proposed commit message via `IConsoleIO`
- Call `IConsoleIO.Confirm` for **mandatory** user approval
- On approval: `IGitClient.StageAllAsync` + `IGitClient.CommitAsync`, then update task status to `Done` via `PlanUpdater` + `IFileOperations`
- On rejection: exit with code 1, task stays `InProgress`, do not revert archived files (useful for `resume`)
- Step 8 is the single mandatory human checkpoint — never skip it

### Validation criteria (Definition of Done)

- [ ] Displays the proposed commit message
- [ ] Pauses for mandatory user approval
- [ ] Commits on user approval, task marked `Done` after commit
- [ ] Aborts on user rejection, task stays `InProgress`
- [ ] Exit code 1 on rejection, exit code 0 on success
- [ ] No lint/type errors

### Context references

- `docs/requirements.md#fr-4-task-execution`
- `docs/architecture.md#data-flow-pipeline`
- `docs/architecture.md#llm-automation-model`
- `docs/coding-style.md`

---

## TASK-027: Implement resume command

**Milestone:** 6 — Orchestration & Commands
**Status:** pending
**Complexity:** Simple
**Depends on:** TASK-014, TASK-023a

### What to build

Add resume functionality to the `TaskOrchestrator`. Check for `context/current-task.md`, determine the last completed step via `StateManager`, and resume execution from the next incomplete step. Support `--from-step=N` override. Handle ambiguous state with a user confirmation prompt.

### Files in scope

- `src/AiDevLoop.Shell/TaskOrchestrator.cs` (modify)
- `tests/AiDevLoop.Shell.Tests/TaskOrchestratorResumeTests.cs` (create)

### Constraints

- Provide a `ResumeAsync(int? fromStep, CancellationToken)` entry point
- Check context file existence via `IFileOperations` (current-task.md, implementation-notes.md, review.md)
- Extract task ID from `current-task.md` via `StateManager.ExtractTaskId`
- Determine resume point via `StateManager.DetermineResumePoint`
- If `--from-step=N` is provided, use that step instead of auto-detection
- If auto-detected state is ambiguous (e.g., partial files), prompt user via `IConsoleIO.Confirm`: `Resume from step N? (y/n)`
- Display: `Resuming TASK-XXX from step N`
- Continue through remaining steps reusing existing step implementations (steps 3-8)
- Error exit code 3 when no task is in progress

### Validation criteria (Definition of Done)

- [ ] Detects current task from `context/current-task.md`
- [ ] Resumes from correct step based on `StateManager` analysis
- [ ] `--from-step` override works correctly
- [ ] Prompts user when state is ambiguous
- [ ] Displays `Resuming TASK-XXX from step N` message
- [ ] Executes remaining steps normally after resume point
- [ ] Returns error when no task in progress
- [ ] No lint/type errors

### Context references

- `docs/requirements.md#fr-5-resume-execution`
- `docs/architecture.md#component-breakdown`
- `docs/coding-style.md`

---

## TASK-028: Wire Program.cs entry point with DI

**Milestone:** 7 — Entry Point, Polish & Distribution
**Status:** pending
**Complexity:** Simple
**Depends on:** TASK-005, TASK-006, TASK-021, TASK-026a, TASK-027

### What to build

The main `Program.cs` that ties everything together. Parse CLI arguments, load and validate configuration, register all services in a DI container, run project validation, route to the correct command (`run` → `TaskOrchestrator.RunAsync`, `resume` → `TaskOrchestrator.ResumeAsync`), handle top-level exceptions, and return the correct exit code.

### Files in scope

- `src/AiDevLoop.Cli/Program.cs` (create)
- `tests/AiDevLoop.E2E.Tests/ProgramTests.cs` (create)

### Constraints

- Use `Microsoft.Extensions.DependencyInjection` for the DI container
- Register the correct `ILLMClient` implementation based on config (`ClaudeLLMClient` or `CopilotLLMClient`)
- Register all other adapters: `FileOperations`, `ProcessRunner`, `ConsoleIO`, `GitClient`
- Register `TaskOrchestrator` and `ProjectValidator`
- Exit codes per FR-8.3: `0` = success, `1` = user abort, `2` = config error, `3` = execution error
- Run `ProjectValidator` before executing any command
- Handle `--help` and `--version` before loading config (they don't require config)
- Wire `Console.CancelKeyPress` to a `CancellationTokenSource` and pass the token through
- Catch unhandled exceptions at the top level, display user-friendly error, return exit code 3

### Validation criteria (Definition of Done)

- [ ] DI container registers all adapter implementations correctly
- [ ] Correct `ILLMClient` selected based on config
- [ ] `run` command routes to `TaskOrchestrator.RunAsync`
- [ ] `resume` command routes to `TaskOrchestrator.ResumeAsync`
- [ ] Returns exit code 0 on success
- [ ] Returns exit code 1 on user abort
- [ ] Returns exit code 2 on configuration error
- [ ] Returns exit code 3 on execution error
- [ ] Ctrl+C triggers cancellation via `CancellationToken`
- [ ] No lint/type errors

### Context references

- `docs/architecture.md#component-breakdown`
- `docs/requirements.md#fr-8-error-handling`
- `docs/requirements.md#fr-1-command-line-interface`
- `docs/coding-style.md`

---

## TASK-029: Add help system and version display

**Milestone:** 7 — Entry Point, Polish & Distribution
**Status:** pending
**Complexity:** Simple
**Depends on:** TASK-028

### What to build

Add `--help` and `--version` flag handling. The help system displays usage, available commands, all flags, and usage examples. Each sub-command has its own help text (`run --help`, `resume --help`). Version reads from `AssemblyInformationalVersion` metadata.

### Files in scope

- `src/AiDevLoop.Cli/HelpDisplay.cs` (create)
- `src/AiDevLoop.Cli/CommandLineParser.cs` (modify — detect help/version flags)
- `src/AiDevLoop.Cli/Program.cs` (modify — handle help/version before command routing)

### Constraints

- Help text is concise and includes usage examples from the architecture CLI section
- Global help: `aidevloop --help` shows tool description, commands (`run`, `resume`), and global flags
- Command help: `aidevloop run --help` shows run-specific options (task ID, `--llm`, `--verbose`)
- Command help: `aidevloop resume --help` shows resume-specific options (`--from-step`)
- `--version` displays version from `AssemblyInformationalVersion` attribute
- Error messages for invalid args include a hint: `Run 'aidevloop --help' for usage`
- Help and version are handled before config loading (no config needed)

### Validation criteria (Definition of Done)

- [ ] `--help` displays global usage, commands, and flags
- [ ] `run --help` shows run-specific options and examples
- [ ] `resume --help` shows resume-specific options and examples
- [ ] Help includes usage examples
- [ ] `--version` displays the tool version
- [ ] Invalid args hint at `--help`
- [ ] No lint/type errors

### Context references

- `docs/requirements.md#fr-1-command-line-interface`
- `docs/architecture.md#command-line-interface-structure`
- `docs/coding-style.md`

---

## TASK-030: Configure single-file executable publishing

**Milestone:** 7 — Entry Point, Polish & Distribution
**Status:** pending
**Complexity:** Trivial
**Depends on:** TASK-028

### What to build

Configure the Cli project for single-file self-contained publishing. Set the assembly name to `aidevloop`. Create publish profiles for Windows (x64), macOS (x64, arm64), and Linux (x64, arm64). Enable compression for smaller binaries.

### Files in scope

- `src/AiDevLoop.Cli/AiDevLoop.Cli.csproj` (modify — publishing properties)
- `src/AiDevLoop.Cli/Properties/PublishProfiles/win-x64.pubxml` (create)
- `src/AiDevLoop.Cli/Properties/PublishProfiles/osx-x64.pubxml` (create)
- `src/AiDevLoop.Cli/Properties/PublishProfiles/osx-arm64.pubxml` (create)
- `src/AiDevLoop.Cli/Properties/PublishProfiles/linux-x64.pubxml` (create)
- `src/AiDevLoop.Cli/Properties/PublishProfiles/linux-arm64.pubxml` (create)

### Constraints

- `AssemblyName`: `aidevloop`
- `PublishSingleFile`: `true`
- `SelfContained`: `true`
- `EnableCompressionInSingleFile`: `true`
- `PublishTrimmed`: `false` (avoid trimming issues in MVP)
- Per ADR-007: users should not need .NET runtime installed
- Alias `adl` is an OS-level concern (documented, not implemented in tool)

### Validation criteria (Definition of Done)

- [ ] `dotnet publish -c Release -r win-x64` produces a single executable
- [ ] Executable named `aidevloop.exe` on Windows, `aidevloop` on macOS/Linux
- [ ] Binary is self-contained (no .NET runtime dependency)
- [ ] Publish profiles exist for all five target platforms
- [ ] No lint/type errors

### Context references

- `docs/architecture.md#adr-007-single-file-executable-deployment`
- `docs/requirements.md#nfr-4-compatibility`

---

## TASK-031: E2E test fixtures and helpers

**Milestone:** 7 — Entry Point, Polish & Distribution
**Status:** pending
**Complexity:** Simple
**Depends on:** TASK-028

### What to build

Create the shared infrastructure for end-to-end tests: fixture project files (minimal `implementation-plan.md` with at least two tasks and a dependency relationship, stub Phase 1 docs, config), mock implementations (`MockLLMClient`, `MockProcessRunner`, `MockConsoleIO`), and a test base class that sets up/tears down temporary directories with git repos.

### Files in scope

- `tests/AiDevLoop.E2E.Tests/Fixtures/docs/requirements.md` (create)
- `tests/AiDevLoop.E2E.Tests/Fixtures/docs/architecture.md` (create)
- `tests/AiDevLoop.E2E.Tests/Fixtures/docs/implementation-plan.md` (create)
- `tests/AiDevLoop.E2E.Tests/Fixtures/.aidevloop.json` (create)
- `tests/AiDevLoop.E2E.Tests/Helpers/MockLLMClient.cs` (create)
- `tests/AiDevLoop.E2E.Tests/Helpers/MockProcessRunner.cs` (create)
- `tests/AiDevLoop.E2E.Tests/Helpers/MockConsoleIO.cs` (create)
- `tests/AiDevLoop.E2E.Tests/Helpers/E2ETestBase.cs` (create)

### Constraints

- Each test creates a temporary directory with the fixture structure
- Initialize a real git repo (`git init`) in the temp directory for commit tests
- `MockLLMClient` returns predetermined responses (no actual LLM calls)
- `MockProcessRunner` supports configurable pass/fail per command
- `MockConsoleIO` captures output and provides scripted user responses
- Clean up all temp directories after tests (use `IDisposable` pattern or test cleanup)
- Fixture `implementation-plan.md` has at least two tasks with a dependency relationship
- All helpers must be deterministic and runnable in CI without external tools

### Validation criteria (Definition of Done)

- [ ] Fixture files create a valid project structure
- [ ] `MockLLMClient` returns configured responses
- [ ] `MockProcessRunner` returns configured results per command
- [ ] `MockConsoleIO` captures output and provides scripted input
- [ ] `E2ETestBase` sets up temp directory with git repo and cleans up after
- [ ] No lint/type errors

### Context references

- `docs/architecture.md#testing-strategy`
- `docs/requirements.md#acceptance-criteria`
- `docs/coding-style.md`

---

## TASK-031a: E2E happy path and failure tests

**Milestone:** 7 — Entry Point, Polish & Distribution
**Status:** pending
**Complexity:** Simple
**Depends on:** TASK-031

### What to build

End-to-end tests covering the happy path (full run: select → implement → validate → review → commit) and failure scenarios (validation failure prompts, missing mandatory docs). Use the fixtures and helpers from TASK-031.

### Files in scope

- `tests/AiDevLoop.E2E.Tests/RunCommandTests.cs` (create)
- `tests/AiDevLoop.E2E.Tests/ErrorHandlingTests.cs` (create)

### Constraints

- Happy path test: task selected, implemented, validated, reviewed, committed — verify exit code 0
- Failure path test: validation failure triggers user prompt, abort returns exit code 1
- Missing mandatory docs test: returns exit code 2
- Context files archived correctly after task completion
- All tests use mock implementations from TASK-031 helpers

### Validation criteria (Definition of Done)

- [ ] Happy path test: task completed end-to-end — exit code 0
- [ ] Failure path test: validation failure triggers prompt, abort returns exit code 1
- [ ] Missing mandatory docs test: returns exit code 2
- [ ] Context files archived correctly after task completion
- [ ] All tests pass deterministically without external dependencies
- [ ] No lint/type errors

### Context references

- `docs/architecture.md#testing-strategy`
- `docs/requirements.md#acceptance-criteria`
- `docs/coding-style.md`

---

## TASK-031b: E2E resume tests

**Milestone:** 7 — Entry Point, Polish & Distribution
**Status:** pending
**Complexity:** Simple
**Depends on:** TASK-031

### What to build

End-to-end tests covering the resume flow: simulate an interrupted task (pre-populate context files at various stages), run resume, and verify execution continues from the correct step. Use the fixtures and helpers from TASK-031.

### Files in scope

- `tests/AiDevLoop.E2E.Tests/ResumeCommandTests.cs` (create)

### Constraints

- Test resume from step 3 (only `current-task.md` exists)
- Test resume from step 4 (`implementation-notes.md` exists)
- Test resume from step 6 (`review.md` exists)
- Test `--from-step=N` override
- Test error when no task in progress (no `current-task.md`)
- All tests use mock implementations from TASK-031 helpers

### Validation criteria (Definition of Done)

- [ ] Resume from step 3 works correctly
- [ ] Resume from step 4 works correctly
- [ ] Resume from step 6 works correctly
- [ ] `--from-step` override works correctly
- [ ] Error returned when no task in progress
- [ ] All tests pass deterministically without external dependencies
- [ ] No lint/type errors

### Context references

- `docs/architecture.md#testing-strategy`
- `docs/requirements.md#acceptance-criteria`
- `docs/coding-style.md`
