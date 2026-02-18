# Task 003: Define shell adapter interfaces

## Description

Define the five I/O adapter interfaces that form the boundary between the orchestrator and external systems: `IFileOperations`, `IProcessRunner`, `ILLMClient`, `IGitClient`, and `IConsoleIO`. These interfaces enable testability via in-memory mock implementations in tests.

## Definition of Done

- [ ] All five interfaces compile with zero errors
- [ ] `IProcessRunner`, `ILLMClient`, and `IGitClient` methods are async and accept `CancellationToken`
- [ ] `IConsoleIO` methods match the architecture specification signatures
- [ ] `IFileOperations` methods cover read, write, copy, move, directory, existence, archive operations
- [ ] All methods have XML documentation
- [ ] No lint/type errors
- [ ] Build succeeds in local environment
- [ ] All tests pass
- [ ] Ready for code review

## Steps

1. Create `IFileOperations` interface with synchronous methods
2. Create `IProcessRunner` interface with async methods
3. Create `ILLMClient` interface with async method
4. Create `IGitClient` interface with async methods
5. Create `IConsoleIO` interface with synchronous methods
6. Verify all interfaces compile without warnings
7. Run full build and test suite

## Acceptance Criteria

- [ ] `IFileOperations`: synchronous methods — `ReadFile`, `WriteFile` (atomic), `CopyFile`, `MoveFile`, `CreateDirectory`, `FileExists`, `DirectoryExists`, `ListFiles`, `ArchiveContextFiles`
- [ ] `IProcessRunner`: async methods with `CancellationToken` — `Task<CommandResult> RunAsync(string command, string arguments, CancellationToken cancellationToken)` plus an overload with working directory and verbose flag
- [ ] `ILLMClient`: async method — `Task<string> InvokeAsync(string prompt, CancellationToken cancellationToken)`. Returns LLM response text.
- [ ] `IGitClient`: async methods with `CancellationToken` — `Task StageAllAsync(CancellationToken cancellationToken)`, `Task CommitAsync(string message, CancellationToken cancellationToken)`, `Task<GitStatus> GetStatusAsync(CancellationToken cancellationToken)`
- [ ] `IConsoleIO`: synchronous methods — `WriteStep`, `WriteError`, `WriteWarning`, `WriteVerbose`, `Confirm`, `PromptChoice<T>` per architecture specification
- [ ] All methods must have XML documentation
- [ ] Interface segregation: each interface has exactly one responsibility
- [ ] Use domain types from TASK-002 (`CommandResult`, `GitStatus`) in return types
- [ ] Prefix all interface names with `I`
