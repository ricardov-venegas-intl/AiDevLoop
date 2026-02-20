# Task TASK-017: Implement ConsoleIO with output modes

## Description
Concrete implementation of `IConsoleIO`. Display output respecting three modes: Normal, Verbose, and Quiet. Show step progress, errors, warnings, verbose detail, and interactive user prompts. Use ANSI colors when the terminal supports them.

## Definition of Done
- [x] `WriteStep` displays in Normal mode, hidden in Quiet mode
- [x] `WriteError` displays in all modes
- [x] `WriteVerbose` displays only in Verbose mode
- [x] `Confirm` prompts user with default value and returns response
- [x] `PromptChoice<T>` displays numbered options and returns selection
- [x] Step progress format: `[N/8] Step Name → details`
- [x] Color output when terminal supports ANSI
- [x] No lint/type errors
- [x] Build succeeds in local environment
- [x] Ready for code review

## Steps
1. Define `OutputMode` enum in `AiDevLoop.Core/Domain/OutputMode.cs` (Normal, Verbose, Quiet)
2. Create `src/AiDevLoop.Shell/Adapters/ConsoleIO.cs` implementing `IConsoleIO`
   - Constructor accepts `OutputMode`, `TextWriter output`, `TextReader input` (inject for testability)
   - `WriteStep`: shown in Normal and Verbose, hidden in Quiet — format `[N/8] Step Name → details`
   - `WriteError`: always shown — red foreground when ANSI supported
   - `WriteWarning`: always shown — yellow foreground when ANSI supported  
   - `WriteVerbose`: only in Verbose mode — dimmed or `[VERBOSE]` prefix
   - `Confirm`: always shown — display question, accept `y`/`n`, return bool
   - `PromptChoice<T>`: always shown — display numbered options, return selected value
   - Use `Console.IsOutputRedirected` to gate ANSI color
3. Create `tests/AiDevLoop.Shell.Tests/ConsoleIOTests.cs`
   - Use `StringWriter`/`StringReader` injections to capture output
   - Cover all output modes (Normal, Verbose, Quiet)
   - Cover all write methods
   - Cover `Confirm` with y/n and default
   - Cover `PromptChoice<T>` with valid and invalid input

## Acceptance Criteria
- [x] `WriteStep` visible in Normal, visible in Verbose, hidden in Quiet
- [x] `WriteError` visible in all three modes
- [x] `WriteWarning` visible in all three modes
- [x] `WriteVerbose` only visible in Verbose mode
- [x] `Confirm` accepts `y`/`Y` → true, `n`/`N` → false
- [x] `PromptChoice<T>` loops on invalid input, returns value for valid 1-based index
- [x] Step format matches `[N/8] Step Name → details`
- [x] Tests pass with injected `TextWriter`/`TextReader`
- [x] `dotnet build -warnaserror` passes
- [x] `dotnet test` passes
