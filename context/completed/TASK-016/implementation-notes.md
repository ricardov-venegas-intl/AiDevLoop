# Implementation Notes: TASK-016

## Task Name
Implement ProcessRunner

## Decisions Made
- **Lock-based thread safety for output handlers**: `OutputDataReceived` and `ErrorDataReceived` fire on thread-pool threads, so each collection (`List<string>` for verbose, `Queue<string>` for non-verbose) is guarded by its own `lock` to avoid races.
- **Separate collections per mode**: Verbose mode uses `List<string>` (unbounded, preserves all output); non-verbose mode uses `Queue<string>` capped at 500 items per stream (oldest lines dropped). This avoids allocating an extra collection and simplifies the join at the end.
- **Simple overload delegates to full overload**: `RunAsync(command, arguments, ct)` calls `RunAsync(command, arguments, "", false, ct)`, keeping a single implementation path.
- **`Assert.ThrowsAnyAsync` for cancellation test**: `WaitForExitAsync` throws `TaskCanceledException` (a subtype of `OperationCanceledException`) when cancelled. xUnit's `Assert.ThrowsAsync<T>` requires an exact match, so `ThrowsAnyAsync<OperationCanceledException>` is used instead.
- **Process.Kill(entireProcessTree: true) on cancellation**: Ensures child processes spawned by the cancelled command do not linger.

## Known Limitations
- **Output ordering is not strictly guaranteed**: Because async events from stdout and stderr may interleave with lock acquisition order, the final joined strings reflect arrival order but do not guarantee interleaved ordering between the two streams.
- **Non-verbose mode drops oldest lines**: When >500 lines are emitted, the oldest are dropped silently. The last 500 lines per stream are retained.

## Risk Areas
- **Cross-platform command availability**: OS-specific tests (`cmd /c exit 1`, `ping -n 30 127.0.0.1`) are guarded with `RuntimeInformation.IsOSPlatform(OSPlatform.Windows)` and return early on non-Windows rather than being skipped via `Skip`. They always appear as "passed" on Linux/macOS without executing.
- **Race between `BeginOutputReadLine` and process exit**: On very fast-exiting processes, output may not be fully flushed before `WaitForExitAsync` returns. `WaitForExitAsync` waits for both the process and its redirected streams to flush, so this is handled correctly by the framework.

## Dependencies
- TASK-003: IProcessRunner interface
- TASK-002: CommandResult domain type

## Testing Notes
- Tests use `dotnet --version` as the universal cross-platform success case (always available in the project's SDK).
- `dotnet nonexistent-command-xyz` produces a non-zero exit code cross-platform without needing OS-specific commands.
- A random GUID as the executable name triggers the `Win32Exception`/`FileNotFoundException` path for the not-found test.
- Cancellation and Windows-specific stderr/exit-code tests guard with `RuntimeInformation.IsOSPlatform` and skip execution (not skip reporting) on non-Windows.

---

# Implementation Notes: TASK-015

## Task Name
Implement FileOperations with atomic writes

## Decisions Made
- Atomic write via temp-file-then-move: a `Path.GetRandomFileName()` temp file is written in the same directory as the target so the `File.Move` rename is guaranteed to be on the same volume, making it atomic on most OS/file-system combinations.
- No BOM: `new UTF8Encoding(encoderShouldEmitUTF8Identifier: false)` ensures plain UTF-8 output, consistent with how the rest of the tool reads and writes Markdown.
- Best-effort temp-file cleanup on write failure: if `File.WriteAllText` or `File.Move` throws, the catch block attempts `File.Delete(tempPath)` before re-throwing so stale temp files are not left behind.
- `ListFiles` returns paths relative to `dirPath` via `Path.GetRelativePath`, matching the interface contract.
- `ArchiveContextFiles` silently skips the three context files when they are absent; this is the expected behaviour when a task cycle did not produce all three files.

## Known Limitations
- `WriteFile` requires the target to have a parent directory that can be determined; a bare filename with no directory component (e.g. `"file.txt"` when the working directory is a root) will throw `IOException`. In practice all callers pass absolute paths.
- `ListFiles` uses `SearchOption.TopDirectoryOnly` — subdirectory contents are not returned, by design.

## Risk Areas
- Atomic rename across volumes: `File.Move` on Windows will throw if source and destination are on different drives. Mitigated by always writing the temp file in the same directory as the target.
- Concurrent writers to the same file: the temp-rename approach is safe for single-writer scenarios; concurrent multi-process writes are not in scope for this tool.

## Dependencies
- TASK-003: IFileOperations interface

## Testing Notes
- Tests use real temp directories created in `Path.GetTempPath()` with a random subfolder per test class instance, cleaned up in `Dispose()`.
- 14 test methods covering all public methods and edge cases (missing file, missing dirs, pattern filtering, partial archive).

---

# Implementation Notes: TASK-014

## Task Name
Implement StateManager

## Decisions Made
- Added `TaskId taskId` parameter to `DetermineResumePoint`: The existing `ResumeState` domain type is `record ResumeState(int NextStep, TaskId TaskId)`, not an enum as the task description assumed. To construct a `ResumeState` on success, the method needs the task ID. The natural call site is: `ExtractTaskId` first, then `DetermineResumePoint` with the resulting `TaskId`.
- `statusInPlan` parameter retained but not used in control flow: The task spec notes this is covered by file-existence checks. The `Pending` case with an existing `current-task.md` resolves to Step 3 via the normal "current-task.md only" branch.
- Static `Regex` field with `RegexOptions.Compiled`: Reused across calls without allocation cost.
- `reviewExists` check precedes `implementationNotesExists` check: Ensures `review.md` always routes to Step 6 regardless of whether `implementation-notes.md` is present.

## Known Limitations
- `statusInPlan` parameter has no effect on branching: The parameter is accepted to match the intended API surface but currently unused. If future logic must distinguish plan status, the method body will need updating.
- `ExtractTaskId` returns the first match only: If a document contains multiple `## TASK-NNN:` headers, only the first is used.

## Risk Areas
- `ResumeState` type mismatch with spec: The spec expected an enum; the existing type is a record. If the record's shape changes (e.g., fields removed), callers of `DetermineResumePoint` break. Mitigation: tests validate the exact fields returned.

## Dependencies
- TASK-002: Domain types (`Result<TValue,TError>`, `TaskId`, `TaskStatus`, `ResumeState`)

## Testing Notes
- 12 xUnit tests total: 6 for `DetermineResumePoint` (no files, only current-task, pending status, with notes, with review, review without notes) and 6 for `ExtractTaskId` (valid header, header at file start, missing header, malformed without colon, empty content, header not at line start).
- Pre-existing test failure `PlanUpdaterTests.UpdateTaskStatus_CrlfInput_PreservesCrlf` is unrelated to this task (CRLF normalisation bug in `PlanUpdater`).

---

## TASK-013 — CommitMessageBuilder

**Files created:**
- `src/AiDevLoop.Core/CommitMessageBuilder.cs`
- `tests/AiDevLoop.Core.Tests/CommitMessageBuilderTests.cs`

### Decisions

**Truncation replaces, not appends.**  
The `...` occupies the last 3 characters of the allowed title window, not appended after. This keeps the subject line exactly 72 chars. E.g. with a 16-char prefix, the title is capped at 56 chars; if the raw title is longer, it becomes `title[0..53] + "..."`.

**Trailing period stripped before truncation.**  
`TrimEnd('.')` runs before `ToLowerInvariant()` and before the length check, so a period never silently shifts the truncation boundary.

**`Span<T>` slicing for substring.**  
Used `string.Concat(title.AsSpan(0, n), "...")` instead of `title[..n] + "..."` to avoid an intermediate allocation for the slice.

**No LINQ, no regex.**  
All logic is simple string operations: `TrimEnd`, `ToLowerInvariant`, `string.Concat`, `AsSpan`.

### Test coverage (10 tests)

| # | Scenario |
|---|----------|
| 1 | Basic title → correct conventional format |
| 2 | Uppercase title → lowercased |
| 3 | Already-lowercase title → unchanged |
| 4 | Trailing period → stripped |
| 5 | Internal period → kept |
| 6 | Result exactly 72 chars → no truncation |
| 7 | Result exceeds 72 chars → truncated to exactly 72 ending in `...` |
| 8 | Long title → ellipsis replaces last 3 chars of allowed window |
| 9 | Trailing period + long title → period stripped first, then truncated |
| 10 | Null task → `ArgumentNullException` |
