# Task TASK-019: Implement Copilot LLM Client

## Description
Implement `ILLMClient` for the GitHub Copilot CLI. Same pattern as `ClaudeLLMClient` but targeting the `copilot` CLI tool. If the two implementations share significant logic, extract a common base helper method or shared utility.

## Definition of Done
- [x] Constructs correct `copilot` CLI command with prompt
- [x] Passes prompt content to the CLI tool
- [x] Captures LLM response from stdout
- [x] Returns error when CLI returns non-zero exit code
- [x] Returns error for empty output
- [x] No lint/type errors
- [x] Build succeeds in local environment
- [x] Ready for code review

## Steps
1. Create `src/AiDevLoop.Shell/Adapters/CopilotLLMClient.cs` implementing `ILLMClient`
   - Constructor injects `IProcessRunner`
   - `InvokeAsync` constructs `gh copilot suggest -t shell "<prompt>"` or equivalent non-interactive invocation
   - On non-zero exit code: throw `InvalidOperationException` with stdout+stderr context
   - On empty/whitespace stdout: throw `InvalidOperationException`
   - Return trimmed stdout on success
2. Create `tests/AiDevLoop.Shell.Tests/CopilotLLMClientTests.cs`
   - Use in-memory `FakeProcessRunner` (same pattern as `ClaudeLLMClientTests`)
   - Cover: successful response, non-zero exit throws, empty stdout throws, whitespace stdout throws, command is "copilot" (or "gh"), args contain prompt

## Acceptance Criteria
- [x] `InvokeAsync` invokes the Copilot CLI via `IProcessRunner.RunAsync`
- [x] Prompt is passed to the process
- [x] Returns trimmed stdout string on success
- [x] Throws on non-zero exit code with error context
- [x] Throws on empty/whitespace response
- [x] `dotnet build -warnaserror` passes
- [x] `dotnet test` passes
