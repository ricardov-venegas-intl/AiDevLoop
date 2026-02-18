# Task TASK-002: Define core domain types

## Description
Create all domain types for the functional core: the `Result<TValue, TError>` discriminated union, value objects (`TaskId`, enums for `TaskStatus`, `Complexity`, `IssueClassification`, `Command`), data records (`TaskDefinition`, `Plan`, `Milestone`, `FileReference`, `CommandResult`, `ValidationResult`, `ReviewIssue`, `ReviewResult`, `ResumeState`, `CommandLineArgs`, `CommandLineOptions`, `Configuration`, `PathsConfiguration`, `ValidationConfiguration`, `GitStatus`), and error discriminated unions (`SelectionError` with `TaskNotFound`, `DependenciesNotMet`, `TaskNotPending`, `NoPendingTasks` variants). Each type gets its own file under `Domain/`.

## Definition of Done
- [ ] All domain types compile with zero nullable warnings
- [ ] `Result<T, TError>` supports `Ok` and `Err` cases with pattern matching via `switch` expressions
- [ ] Record equality works correctly for `TaskId` value objects
- [ ] `SelectionError` discriminated union has all four variants
- [ ] `Configuration.Default` factory returns config matching FR-3.2 defaults
- [ ] All collection properties use `IReadOnlyList<T>`
- [ ] Unit tests verify `Result` pattern matching and `TaskId` equality
- [ ] No lint/type errors
- [ ] Build succeeds in local environment
- [ ] Ready for code review

## Steps
1. Create `Result<TValue, TError>` discriminated union with `Ok` and `Err` cases
2. Create `TaskId` readonly record struct value object
3. Create `TaskStatus`, `Complexity`, `IssueClassification`, `Command` enums
4. Create `TaskDefinition`, `Plan`, `Milestone`, `FileReference` records
5. Create `CommandResult`, `ValidationResult` records
6. Create `ReviewIssue`, `ReviewResult` records
7. Create `ResumeState`, `GitStatus` records
8. Create `CommandLineArgs`, `CommandLineOptions` records
9. Create `Configuration`, `PathsConfiguration`, `ValidationConfiguration` records
10. Create `SelectionError` discriminated union with four variants
11. Write unit tests for `Result` and `TaskId` in `Domain/ResultTests.cs`
12. Build and verify zero errors/warnings

## Acceptance Criteria
- [ ] All domain types compile with zero nullable warnings
- [ ] `Result<T, TError>` supports `Ok` and `Err` cases with pattern matching via `switch` expressions
- [ ] Record equality works correctly for `TaskId` value objects
- [ ] `SelectionError` discriminated union has all four variants
- [ ] `Configuration.Default` factory returns config matching FR-3.2 defaults
- [ ] All collection properties use `IReadOnlyList<T>`
- [ ] Unit tests verify `Result` pattern matching and `TaskId` equality
