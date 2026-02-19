# Implementation Notes: TASK-010

## Task Name
Implement ValidationEngine

## Decisions Made
- **Warning source is raw `Stderr` string**: The spec says `Warnings` contains "stderr content (as strings)"—the raw `Stderr` value is used directly rather than wrapping it in a message object.
- **`string.IsNullOrEmpty` guard on `Stderr`**: Avoids adding empty-string entries to `Warnings` for passing commands that emitted nothing on stderr.
- **Collection expressions (`[]`)**: Used `[]` syntax (C# 12) for empty collections, consistent with the rest of the codebase.
- **`using Xunit;` required explicitly**: The test project does not enable implicit usings, so xUnit attributes must be imported with an explicit `using` directive (matching the pattern in all other test files).

## Known Limitations
- No deduplication of warnings — if multiple commands emit identical stderr content, each appears as a separate entry in `Warnings`.

## Risk Areas
- Relies on `CommandResult.Succeeded` (`ExitCode == 0`), so any change to that definition automatically propagates to `ValidationEngine`.

## Dependencies
- `CommandResult` (Domain) — per-command result record
- `ValidationResult` (Domain) — aggregate result record

## Testing Notes
- 13 new tests across 5 logical groups: empty input, all-passing, failures, warnings, mixed.
- Tests cover: `AllPassed` flag, `FailedCommands` contents, `Warnings` contents, no-commands warning, stderr-from-failed-commands not added to warnings.
- Total suite: 120 passing, 0 failed.
