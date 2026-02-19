# Implementation Notes: TASK-009

## Task Name
Implement PlanUpdater

## Decisions Made
- **Line-by-line state machine over regex**: Simple string operations are sufficient and easier to reason about. A state machine (flag `inTaskBlock`) locates the correct `**Status:**` line without risking false positives from similarly named status fields elsewhere in the document.
- **`continue`-free heading detection**: After detecting a `## ` heading, the code falls through to the status-field check via `else if`, which is safe since a heading line never starts with `**Status:**`. This avoids control-flow that could accidentally skip lines.
- **Partial-ID match guard**: `MatchesTaskPrefix` checks that the character after the task ID is a space or end-of-line, preventing `TASK-001` from matching `TASK-0010`.
- **Separator detection**: The method detects `\r\n` vs `\n` from the input and rejoins with the same separator so CRLF files are round-tripped correctly.
- **`statusUpdated` flag**: Marks the first `**Status:**` line inside the block as updated and ignores any subsequent ones, preventing double-updates if the block contains multiple status-like lines.

## Known Limitations
- **No validation that the task exists**: If `taskId` is not present in the plan, the method returns the original content unchanged without signalling an error. Callers are responsible for knowing the task is present.
- **Heading match is prefix-based**: `## TASK-001:` is matched by `StartsWith`, so a task ID that is a prefix of another (e.g., `TASK-1` vs `TASK-10`) could cause incorrect block detection. In practice, all task IDs follow the zero-padded `TASK-NNN` format, making this a non-issue.

## Risk Areas
- **Plan format drift**: The implementation depends on the exact markdown conventions (`- [ ]`/`- [x]` checkbox format, `**Status:** value` on a single line). If the plan format changes, the text patterns must be updated here too. Mitigation: the format is defined by ADR-003 and kept stable.

## Dependencies
- `TaskId` (Domain) — value object identifying the task
- `TaskStatus` (Domain) — enum with `Pending`, `InProgress`, `Done`, `Blocked` values

## Testing Notes
- A `MakePlan` helper builds a compact three-task plan string so each test is self-contained and readable.
- Tests cover all four `TaskStatus` values for both checkbox and status field.
- Position coverage tests (first, middle, last task) verify the state machine handles document boundaries correctly.
- An isolation test confirms only the targeted task's lines change and sibling task content is preserved.
- One test explicitly exercises the "already-checked → non-Done" unchecking path.
- 14 new tests added; total suite now at 107 passing.
