# Implementation Notes: TASK-018

## Task Name
Implement Claude LLM Client

## Decisions Made
- Used `-p` flag with inline prompt rather than a temp-file approach because `IProcessRunner` does not support stdin redirection, and the `-p` flag is the standard non-interactive mode for the Claude CLI.
- Inner double-quotes in the prompt are escaped (`"` → `\"`) before being embedded in the argument string, preventing argument parsing issues.
- Raised `InvalidOperationException` for both non-zero exit codes and empty/whitespace responses, following the same error convention as other Shell adapters.

## Known Limitations
- Very large prompts passed via `-p` may hit OS command-line length limits. If this becomes an issue, a temp-file approach using a future stdin-capable `IProcessRunner` overload would be preferred.
- The exact Claude CLI version and flag compatibility (`--print`, `-p`) is assumed based on published Claude CLI documentation. Verify against the installed CLI version at integration time.

## Risk Areas
- Command-line length limits for large prompts on Windows (~32 767 characters).
- Shell injection is mitigated by escaping double-quotes, but characters like backticks or `$()` could still be interpreted by some shells depending on how `IProcessRunner` invokes the process.

## Dependencies
- IProcessRunner (TASK-016)
- ILLMClient interface (TASK-003)

## Testing Notes
- Used in-memory `FakeProcessRunner` for deterministic tests; no external process or network calls.
- 6 tests cover: trimmed stdout return, non-zero exit code exception, empty stdout exception, whitespace stdout exception, correct command name, and double-quote escaping in arguments.
