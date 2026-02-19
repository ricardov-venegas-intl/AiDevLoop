# Implementation Notes: TASK-008

## Task Name
Implement TaskSelector

## Decisions Made
- **`using TaskStatus = AiDevLoop.Core.Domain.TaskStatus` alias in both files**: `System.Threading.Tasks.TaskStatus` causes an ambiguity with the domain enum when `using AiDevLoop.Core.Domain` is in scope. A using alias resolves it cleanly without qualifying every reference.
- **Private helper `AllDepsDone`**: Extracted to avoid duplicating the LINQ predicate between auto-select and the explicit-select dep check.
- **`Dictionary<TaskId, TaskDefinition>` built once per call**: Gives O(1) lookups when resolving dependency chains regardless of plan size.
- **`FirstOrDefault` for auto-select**: Simple and readable; the plan is processed in a single enumeration pass (milestones → tasks in order), so the ordering contract is satisfied without extra sorting.
- **XML crefs use fully qualified names (`AiDevLoop.Core.Domain.TaskStatus.Pending`)**: Required to disambiguate the cref inside the XML doc from `System.Threading.Tasks.TaskStatus`.

## Known Limitations
- None at this time

## Risk Areas
- None

## Dependencies
- TASK-007 domain types (Plan, Milestone, TaskDefinition, TaskId, TaskStatus, SelectionError, Result)

## Testing Notes
- All error cases covered: `NoPendingTasks`, `TaskNotFound`, `TaskNotPending`, `DependenciesNotMet`
- Edge cases: empty plan, all tasks done, circular deps expressed as mutually unsatisfied deps, multi-milestone ordering, partial deps satisfied
- All 64 tests pass (includes prior test suites)
