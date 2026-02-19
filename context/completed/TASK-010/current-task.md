# Task TASK-010: Implement ValidationEngine

## Description
A pure function that analyzes a list of `CommandResult` objects from validation commands (lint, typecheck, test, build) and produces a `ValidationResult`. Any command with a non-zero exit code is a failure. Collect all failures and any warnings (commands with stderr output despite success).

## Definition of Done
- [ ] Returns `AllPassed=true` when all commands succeed (exit code 0)
- [ ] Returns `AllPassed=false` when any command fails
- [ ] `FailedCommands` contains all failed `CommandResult` objects
- [ ] `Warnings` includes stderr content from passing commands with stderr output
- [ ] Empty command list returns `AllPassed=true` with warning about no validation
- [ ] No lint/type errors
- [ ] Build succeeds in local environment
- [ ] Ready for code review

## Steps
1. Create `src/AiDevLoop.Core/ValidationEngine.cs` with a static class containing `Validate(IReadOnlyList<CommandResult> commandResults)` pure function
2. Create `tests/AiDevLoop.Core.Tests/ValidationEngineTests.cs` with unit tests covering all acceptance criteria
3. Run `dotnet build -warnaserror` and `dotnet test` to verify

## Acceptance Criteria
- [ ] Returns `AllPassed=true` when all commands succeed (exit code 0)
- [ ] Returns `AllPassed=false` when any command fails
- [ ] `FailedCommands` contains all failed `CommandResult` objects
- [ ] `Warnings` includes stderr content from passing commands with stderr output
- [ ] Empty command list returns `AllPassed=true` with a warning about no validation commands
- [ ] Pure function — no I/O, no dependencies
- [ ] XML documentation on all public APIs