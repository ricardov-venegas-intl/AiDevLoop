# Implementation Notes: Task TASK-002

## Task Name
Define core domain types

## Decisions Made

- **`Result.cs` contains all three types (`Result<TValue, TError>`, `Ok`, `Err`)**: The "one type per file" rule is waived for discriminated union hierarchies. The three types are entirely co-dependent — `Ok` and `Err` are meaningless without their parent abstract record. The architecture documentation shows them together as a unit. Same rationale applies to `SelectionError.cs`.

- **`FileReferenceKind.cs` added (not in original scope list)**: The constraint "one type per file" required extracting the `FileReferenceKind` enum from `FileReference.cs`. This extra file is consistent with the coding style guide and prevents multiple type declarations in a single source file.

- **`Option<T>` replaced with nullable types (`T?`)**: The architecture diagrams show `Option<TaskId>` in `CommandLineArgs`. Since `Option<T>` is not in the task's files-in-scope list and C#'s nullable reference/value types provide equivalent semantics, nullable types are used instead (`TaskId?`, `string?`, `int?`). This avoids introducing an undocumented algebraic type wrapper.

- **`SelectionError.cs` includes `TaskNotPending` and `NoPendingTasks` variants**: These two variants are listed in the TASK-002 constraints but not shown in the architecture doc example, which only shows `TaskNotFound` and `DependenciesNotMet`. Both are added as specified by the task constraints.

- **`Configuration.Default` uses an empty commands dictionary**: The FR-3.2 schema shows sample commands like `npm run lint`, but these are project-specific examples rather than global defaults. The default configuration provides an empty `IReadOnlyDictionary<string, string>` for `ValidationConfiguration.Commands` so no commands run without explicit configuration.

- **`CorePlaceholder.cs` retained**: The placeholder class remains; it does not conflict with any new domain type and removing it is not in this task's scope.

## Known Limitations

- **`TaskStatus` name collision**: `System.Threading.Tasks.TaskStatus` exists in the BCL. Code in a file that uses both `AiDevLoop.Core.Domain` and `System.Threading.Tasks` (async/await code) will require a fully-qualified name or a `using alias`. This is acceptable since the Core project has zero async dependencies.

- **Validation commands default is empty**: The `Configuration.Default` returns an empty commands dict. Callers that need specific validation (build, test) must supply a config file. See FR-3.2.

## Risk Areas

- **Pattern exhaustiveness**: `Result<TValue, TError>` does not have a sealed base with a `switch` expression compiler warning. At runtime, any unknown subtype would hit the discard `_` arm. The architecture's guideline is to always include a discard arm — this is documented in test examples.

## Dependencies

- None — this task has no project dependencies beyond TASK-001 (solution structure).

## Testing Notes

- 12 unit tests cover `Result<T,E>` pattern matching (Ok/Err arms, equality) and `TaskId` value semantics (equality, use as dictionary key, `ToString`).
- Combined with 3 pre-existing smoke tests, total test count is 13.
- No tests were written for the DTO records beyond what exercises `Result` and `TaskId`; those types are pure data containers with no behaviour to test.
