# Task TASK-020: Implement GitClient

## Description
Implement `IGitClient` using `IProcessRunner` to execute git commands. Stage all changes (`git add .`), commit with a provided message (`git commit -m`), and get repository status by parsing `git status --porcelain` output into a `GitStatus` record.

## Definition of Done
- [x] `StageAllAsync` executes `git add .`
- [x] `CommitAsync` executes `git commit -m` with the provided message
- [x] `GetStatusAsync` parses porcelain output into `GitStatus` fields correctly
- [x] Detects detached HEAD state
- [x] Handles clean repo (no changes) correctly
- [x] No lint/type errors
- [x] Build succeeds in local environment
- [x] Ready for code review

## Steps
1. Create `src/AiDevLoop.Shell/Adapters/GitClient.cs` implementing `IGitClient`
   - Constructor injects `IProcessRunner`
   - `StageAllAsync`: run `git add .`, throw on non-zero exit
   - `CommitAsync`: run `git commit -m "<escaped message>"`, throw on non-zero exit
   - `GetStatusAsync`: run `git status --porcelain`, parse each line for staged/unstaged markers, collect modified file paths
2. Create `tests/AiDevLoop.Shell.Tests/GitClientTests.cs`
   - In-memory `FakeProcessRunner` (same pattern as Claude/CopilotLLMClientTests)
   - Test: `StageAllAsync` invokes `git add .`
   - Test: `CommitAsync` invokes `git commit -m` with the message
   - Test: `GetStatusAsync` parses staged changes correctly
   - Test: `GetStatusAsync` parses unstaged changes correctly
   - Test: `GetStatusAsync` returns empty status for clean repo
   - Test: `StageAllAsync` throws on non-zero exit
   - Test: `CommitAsync` throws on non-zero exit

## Acceptance Criteria
- [x] `StageAllAsync` calls `git add .` via `IProcessRunner`
- [x] `CommitAsync` calls `git commit -m` with message via `IProcessRunner`
- [x] `GetStatusAsync` correctly sets `HasStagedChanges` from porcelain output
- [x] `GetStatusAsync` correctly sets `HasUnstagedChanges` from porcelain output
- [x] `GetStatusAsync` collects `ModifiedFiles` list
- [x] Non-zero exit codes from git throw `InvalidOperationException`
- [x] `dotnet build -warnaserror` passes
- [x] `dotnet test` passes
