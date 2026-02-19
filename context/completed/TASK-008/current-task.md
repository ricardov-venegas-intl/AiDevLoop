# Task TASK-008: Implement TaskSelector

## Description
A pure function that selects the next task to execute from a `Plan`. If no task ID is provided, auto-select the first `pending` task whose dependencies are all `done`. If a task ID is provided, validate it exists, is `pending`, and has all dependencies satisfied. Return the full `TaskDefinition` on success or a typed `SelectionError` on failure.

## Definition of Done
- [x] Auto-selects first pending task when all its dependencies are done
- [x] Skips tasks with unsatisfied dependencies during auto-select
- [x] Selects specific task by ID when provided
- [x] Returns `TaskNotFound` for non-existent task ID
- [x] Returns `TaskNotPending` when task is already `Done` or `InProgress`
- [x] Returns `DependenciesNotMet` with the list of unsatisfied dependency IDs
- [x] Returns `NoPendingTasks` when no eligible tasks remain
- [x] No lint/type errors
- [x] Build succeeds in local environment
- [x] Ready for code review

## Steps
1. Create `src/AiDevLoop.Core/TaskSelector.cs` implementing `SelectTask(Plan plan, TaskId? taskId)` as a static pure function.
2. Auto-select logic: iterate milestones in order, then tasks in order within each milestone; pick the first `Pending` task whose `DependsOn` list has all entries resolved to `Done` status in the plan.
3. Explicit-ID logic: validate the task exists, is `Pending`, and has all dependencies `Done`; return appropriate error variants otherwise.
4. Create `tests/AiDevLoop.Core.Tests/TaskSelectorTests.cs` with unit tests for all acceptance criteria.
5. Build with `dotnet build -warnaserror` and run `dotnet test`.

## Acceptance Criteria
- [x] `SelectTask(plan, null)` returns the first eligible `Pending` task by milestone+task order
- [x] `SelectTask(plan, null)` skips tasks whose dependencies are not `Done`
- [x] `SelectTask(plan, taskId)` returns the `TaskDefinition` for a valid pending task with satisfied deps
- [x] `SelectTask(plan, taskId)` returns `TaskNotFound` when the ID is not in the plan
- [x] `SelectTask(plan, taskId)` returns `TaskNotPending` when the task is `Done` or `InProgress`
- [x] `SelectTask(plan, taskId)` returns `DependenciesNotMet` with unsatisfied dep IDs listed
- [x] `SelectTask(plan, null)` returns `NoPendingTasks` when no eligible task exists
- [x] Pure function — no I/O, no mutation
- [x] XML docs on all public APIs
- [x] One type per file
