# Task TASK-018: Implement Claude LLM Client

## Description
Implement `ILLMClient` for the Anthropic Claude CLI. Construct the `claude` command with the appropriate flags to pass a prompt, execute it via `IProcessRunner`, and return the LLM response from stdout. Handle failures per FR-6.3.

## Definition of Done
- [x] Constructs correct `claude` CLI command
- [x] Passes prompt content to the CLI tool
- [x] Captures LLM response from stdout
- [x] Returns error when CLI returns non-zero exit code
- [x] Returns error for empty LLM output
- [x] Uses `IProcessRunner` for process execution (testable with mock)
- [x] No lint/type errors
- [x] Build succeeds in local environment
- [x] Ready for code review

## Steps
1. Create `src/AiDevLoop.Shell/Adapters/ClaudeLLMClient.cs` implementing `ILLMClient`
   - Constructor injects `IProcessRunner`
   - `InvokeAsync` constructs `claude -p "<prompt>"` command (or writes to temp file for long prompts)
   - On non-zero exit code: throw `InvalidOperationException` with stdout+stderr context
   - On empty/whitespace stdout: throw `InvalidOperationException` with message
   - Return trimmed stdout on success
2. Create `tests/AiDevLoop.Shell.Tests/ClaudeLLMClientTests.cs`
   - Use `NSubstitute` to mock `IProcessRunner`
   - Test: successful invocation returns stdout
   - Test: non-zero exit code throws
   - Test: empty stdout throws

## Acceptance Criteria
- [x] `InvokeAsync` invokes `claude` via `IProcessRunner.RunAsync`
- [x] Prompt is passed to the process (via `-p` flag or stdin piping approach)
- [x] Returns trimmed stdout string on success
- [x] Throws (or propagates) on non-zero exit code with error context
- [x] Throws on empty/whitespace response
- [x] `dotnet build -warnaserror` passes
- [x] `dotnet test` passes
