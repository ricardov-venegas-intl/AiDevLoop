# Code Review Report: TASK-016

## Summary

`ProcessRunner` is well-structured and handles the happy path, cancellation, and command-not-found correctly. One blocking defect exists: the `verbose`/non-verbose initialization block allocates all four collections in both branches, making half of them dead weight in every execution.

## Issues Found

### Blocking

- **Dead-code initialization inflates allocations** (`ProcessRunner.cs` lines 52–66)
  Both the `verbose` and `else` branches initialize all four collections (`stdoutLines`, `stderrLines`, `stdoutQueue`, `stderrQueue`) identically. In verbose mode the two `Queue<string>` objects are allocated then never written to; in non-verbose mode the two `List<string>` objects are allocated then never written to. The `if (verbose)` guard on the initializers is meaningless and gives a false impression of optimization. Fix: initialize only the two collections actually used per mode, or (simpler) initialize all four unconditionally without the dead branch and add a comment.

### Non-Blocking

- **500-line cap is untested** — There is no test that drives >500 lines of output and asserts that only the last 500 are retained. This is the most unique behavior of the implementation and should have at least one test.
- **Platform-skip tests use `return` instead of xUnit `Skip`** — The cancellation, `cmd /c exit 1`, and stderr tests silently *pass* on Linux/macOS by returning early. Using `Skip.IfNot(RuntimeInformation.IsOSPlatform(OSPlatform.Windows))` (or xUnit `[PlatformSpecific]`) would mark them as skipped rather than green, giving accurate CI signal.
- **`using System.IO;` absent from explicit imports** — `Directory.GetCurrentDirectory()` is resolved via implicit global usings rather than an explicit `using System.IO;`. This is inconsistent with the other explicit `System.*` imports in the file.

### Nitpicks

- After `process.Kill(entireProcessTree: true)` in the cancellation handler, `Dispose()` is called immediately via the `using` block without a subsequent `WaitForExit()`. In practice .NET handles this gracefully, but a brief comment explaining the intentional omission of `WaitForExit` post-kill would improve readability.
- The separate `lock` objects for lists vs. queues are the respective collection instances (`lock (stdoutLines)` / `lock (stdoutQueue)`). This is correct but could be confusing since each mode uses a different object as its monitor. A dedicated `_stdoutLock` / `_stderrLock` pair (or using one collection per stream in both modes) would be clearer.

## Compliance

- [x] Architecture adherence — `ConfigureAwait(false)` present, no `.Result`/`.Wait()`, `using` for process disposal, exceptions for I/O errors, interface implemented correctly
- [x] Code style compliance — XML docs via `<inheritdoc/>`, `PascalCase` constant, one type per file, Allman braces, `System.*` imports sorted before internal namespaces
- [ ] Test coverage adequate — success, non-zero exit, command-not-found, verbose, cancellation, and Windows-specific cases are covered; **the 500-line output cap is not tested**
- [ ] Risk areas addressed — platform-specific tests always report "passed" on non-Windows (documented as a known limitation but still a CI hygiene risk)
