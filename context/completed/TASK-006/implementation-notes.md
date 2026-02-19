# Implementation Notes: TASK-006

## Task Name
Implement CommandLineParser

## Decisions Made

- **Location in AiDevLoop.Cli**: The task specified `src/AiDevLoop.Cli/`. While the parser is pure logic with no I/O (making it a natural fit for `AiDevLoop.Core`), placing it in Cli avoids coupling Core to the concept of a CLI command syntax. The Cli project's public nature and its existing reference by `AiDevLoop.Shell.Tests` make tests straightforward from that project.

- **Manual string parsing**: No external CLI parsing libraries used (per requirements). Implementation uses `StartsWith` prefix matching for key=value flags and direct equality for boolean flags.

- **Validation ordering**: The `--verbose`/`--quiet` conflict and the `--from-step` + `run` constraint are validated *after* all flags are collected. This ensures consistent error reporting regardless of flag order, rather than stopping mid-parse.

- **`--help` and `--version` not implemented**: These were listed in the description but absent from the acceptance criteria. They currently return "Unknown flag" errors. A future task should add `ShowHelp` and `ShowVersion` to the `Command` enum and wire them up in `Program.cs`.

- **`TryGetFlagValue` helper**: Extracted to a reusable helper to avoid repetition of prefix-stripping logic. Uses `StringComparison.Ordinal` for flag matching (flags are ASCII, performance-sensitive path).

- **`TryParseCommand` uses switch statement**: The out-parameter assignment pattern suits the switch statement form over a switch expression here. A switch expression form would require an additional mapping step.

## Known Limitations

- No validation of the task ID format (e.g., does not enforce `TASK-\d+` pattern). This is intentional: the task ID is validated downstream when the plan is read.
- `--help` and `--version` flags return an unknown-flag error. This is a known gap for a future task.
- Duplicate flags (e.g., `--llm=claude --llm=copilot`) silently use the last value. This is acceptable for the current scope.

## Risk Areas

- **Assembly loading conflict in `Load_ReadsYamlFile` test**: Pre-existing failure in `AiDevLoop.Shell.Tests.ConfigurationLoaderTests`. The `AiDevLoop.Core` assembly appears to load twice due to the combined `Shell` + `Cli` references, causing the `Assert.IsType` comparison to fail. This is not introduced by this task.

## Dependencies

- `AiDevLoop.Core.Domain`: `Command`, `CommandLineArgs`, `CommandLineOptions`, `TaskId`, `Result<T, TError>`

## Testing Notes

- 25 tests cover all acceptance criteria plus edge cases (zero/negative/non-numeric `--from-step`, case-insensitive command, combined flags, defaults).
- Tests live in `AiDevLoop.Shell.Tests` (which already references `AiDevLoop.Cli`).
- All tests are pure unit tests — no I/O, no mocking needed.
