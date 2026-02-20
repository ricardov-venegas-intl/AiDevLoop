# Task 016: Implement ProcessRunner

## Description
Concrete implementation of `IProcessRunner`. Execute external commands asynchronously using `System.Diagnostics.Process`, capture stdout and stderr separately, and return a `CommandResult` with the exit code. Limit captured output to prevent memory issues with large command output.

## Definition of Done
- [ ] Executes a command and captures stdout
- [ ] Captures stderr separately from stdout
- [ ] Returns correct exit code
- [ ] Respects `CancellationToken` for cancellation (kills process)
- [ ] Limits output to 500 lines in non-verbose mode
- [ ] Handles command-not-found gracefully
- [ ] No lint/type errors
- [ ] Build succeeds in local environment
- [ ] Ready for code review

## Steps
1. Create `src/AiDevLoop.Shell/Adapters/ProcessRunner.cs` implementing `IProcessRunner`
2. Use `System.Diagnostics.Process` with `RedirectStandardOutput` and `RedirectStandardError`, `UseShellExecute = false`
3. Async: await process exit via `WaitForExitAsync(cancellationToken)`, kill on cancellation
4. Capture stdout and stderr into separate `StringBuilder`s
5. In non-verbose mode, keep only the last 500 lines (use a queue/deque approach)
6. Handle `Win32Exception` / `FileNotFoundException` for command-not-found -> return descriptive `CommandResult` with exit code -1
7. Create `tests/AiDevLoop.Shell.Tests/ProcessRunnerTests.cs`
8. Run `dotnet build -warnaserror` and `dotnet test` to validate

## Acceptance Criteria
- [ ] `RunAsync` executes a real command and returns stdout
- [ ] `RunAsync` returns correct non-zero exit code for failing commands
- [ ] `RunAsync` captures stderr separately
- [ ] `RunAsync` kills process and throws `OperationCanceledException` on cancellation
- [ ] Non-verbose mode: stdout/stderr truncated to last 500 lines
- [ ] Command-not-found returns `CommandResult` with exit code -1 and descriptive message (does not throw)
- [ ] All public APIs have XML documentation
- [ ] Zero build warnings with `-warnaserror`
- [ ] All unit tests pass

## Context References
- `docs/architecture.md#component-breakdown`
- `docs/architecture.md#performance-considerations`
- `docs/requirements.md#fr-4-task-execution`
- `docs/coding-style.md`
- `src/AiDevLoop.Shell/Adapters/IProcessRunner.cs`
- `src/AiDevLoop.Core/Domain/CommandResult.cs`
