# Task TASK-009: Implement PlanUpdater

## Description
A pure function that updates task status in the raw markdown content of `implementation-plan.md`. Changes both the milestone checkbox line (`- [ ]` ↔ `- [x]`) and the `**Status:**` field inside the task definition block. Returns the updated markdown string.

## Definition of Done
- [ ] Changes checkbox from `[ ]` to `[x]` when marking as `Done`
- [ ] Changes checkbox from `[x]` to `[ ]` when marking as `InProgress` or `Pending`
- [ ] Updates `**Status:**` field text in the task definition block
- [ ] Preserves all other document content unchanged
- [ ] Handles task at any position in the document (first, middle, last)
- [ ] No lint/type errors
- [ ] Build succeeds in local environment
- [ ] Ready for code review

## Steps
1. Create `src/AiDevLoop.Core/PlanUpdater.cs` with a static class `PlanUpdater` containing a pure method `UpdateTaskStatus(string planContent, TaskId taskId, TaskStatus newStatus) → string`
2. Implement checkbox update: find the milestone line matching the task ID and toggle `- [ ]` ↔ `- [x]` based on whether the new status is `Done`
3. Implement `**Status:**` field update: find the task definition block section heading and update the status value (lowercase: `pending`, `in-progress`, `done`, `blocked`)
4. Ensure all other content is preserved unchanged
5. Create `tests/AiDevLoop.Core.Tests/PlanUpdaterTests.cs` with tests covering all validation criteria
6. Run `dotnet build -warnaserror` and `dotnet test` to confirm everything passes

## Acceptance Criteria
- [ ] `UpdateTaskStatus` is a pure static method — no I/O, no side effects
- [ ] Checkbox toggling works correctly for all `TaskStatus` values
- [ ] `**Status:**` field updated correctly for all `TaskStatus` values
- [ ] No other content is modified
- [ ] Edge cases covered: first task, last task, middle task
- [ ] All tests pass

## Files in Scope
- `src/AiDevLoop.Core/PlanUpdater.cs` (create)
- `tests/AiDevLoop.Core.Tests/PlanUpdaterTests.cs` (create)

## Constraints
- Pure function signature: `string UpdateTaskStatus(string planContent, TaskId taskId, TaskStatus newStatus)`
- Checkbox: `- [ ]` for `Pending`, `InProgress`, `Blocked`; `- [x]` for `Done`
- Status field value lowercase: `pending`, `in-progress`, `done`, `blocked`
- Deterministic text manipulation — match on task ID to locate the correct lines
- Preserve all other content unchanged (no accidental whitespace changes)
- Per ADR-003: simple text replacement, no LLM involvement

## References
- `docs/architecture.md#adr-003-text-based-plan-updates`
- `docs/requirements.md#fr-4-task-execution`
- `docs/coding-style.md`

