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
