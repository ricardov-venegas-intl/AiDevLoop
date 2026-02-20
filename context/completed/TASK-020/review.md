# Code Review Report: TASK-020

## Summary

`GitClient` is well-structured and covers the happy path correctly. One platform correctness bug with CRLF line endings is blocking; four non-blocking issues (missing guard, naming mismatch, bad escape edge case, missing test) and three nitpicks round out the findings.

## Issues Found

### Blocking

- **CRLF in `GetStatusAsync` file paths.** `result.Stdout.Split('\n')` does not strip `\r`, so on Windows (or any git config with `core.autocrlf = true`) each file path in `ModifiedFiles` gains a trailing `\r` (e.g. `src/Foo.cs\r`). Fix: split on `['\r', '\n']`, or call `line.TrimEnd('\r')` before slicing.

### Non-Blocking

- **No null/whitespace guard on `CommitAsync(message)`.**  Passing `null` or an empty string produces an invalid `git commit -m ""` silently. Add `ArgumentException.ThrowIfNullOrWhiteSpace(message)` at the top of the method.

- **`ModifiedFiles` includes untracked files.** Lines with `??` status have both columns set to `?`, so neither flag is set, but `line[3..]` is still appended to `modifiedFiles`. The property name `ModifiedFiles` implies tracked changes only. Either filter out `??` lines when building the list, or rename the property to `ChangedFiles` / update the XML doc to clarify.

- **Message escaping breaks on a trailing backslash.** `message.Replace("\"", "\\\"")` converts a message ending in `\` (e.g. `foo\`) into `commit -m "foo\"`. The Windows CRT argument parser treats `\"` as an escaped quote, so the closing `"` is consumed and the argument is never terminated. Fix: also escape backslashes immediately preceding a double-quote, or prefer passing arguments as an array via `ProcessStartInfo.ArgumentList`.

- **Missing test: `GetStatusAsync_NonZeroExitCode_Throws`.** All other methods test for non-zero exit; `GetStatusAsync` does not. The code handles it, but coverage has a gap.

### Nitpicks

- `using System.Threading;` and `using System.Threading.Tasks;` in `GitClient.cs` are redundant — implicit usings already include them.

- `using System;`, `using System.Threading;`, and `using System.Threading.Tasks;` in `GitClientTests.cs` are similarly redundant.

- `GetStatusAsync_PopulatesModifiedFiles` only asserts `Assert.Contains("src/Foo.cs", ...)` — it doesn't verify the collection count or absence of unexpected entries. A stronger assertion (`Assert.Equal(["src/Foo.cs"], status.ModifiedFiles)`) would catch regressions more reliably.

## Compliance

- [x] Architecture adherence — `sealed class`, constructor injection, `ConfigureAwait(false)`, one type per file
- [x] Code style compliance — Allman braces, `_camelCase` fields, expression bodies where readable
- [ ] Test coverage adequate — 8 of 9 required cases covered; `GetStatusAsync` non-zero exit is missing
- [ ] Risk areas addressed — CRLF handling and message-escaping edge cases are not covered
