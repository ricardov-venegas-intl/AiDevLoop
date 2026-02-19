# TASK-012 Implementation Notes: PromptBuilder

## Files Created

- `src/AiDevLoop.Core/PromptBuilder.cs` — pure static `PromptBuilder` class
- `tests/AiDevLoop.Core.Tests/PromptBuilderTests.cs` — 13 xUnit tests

## Design Decisions

**Task section header**: The spec defines the separator format as `\n\n---\n\n## {sourceKey}\n\n{content}` for loaded files. The `taskContent` block uses the fixed header `## Task` to distinguish it from context reference sections, keeping the format consistent without inventing a sourceKey for the task parameter.

**No LINQ**: Simple `for` loop over `contextReferences` as required. `StringBuilder` used throughout for O(n) concatenation.

**Null/empty template and task**: Both treated as empty string (no output for that section). This means a null template + null task + empty refs returns `string.Empty`, which is the correct identity value.

**`promptTemplate` appears verbatim**: No separator is prepended before the template — it is the first thing in the output. The `---` separator only appears before `taskContent` and each context reference.

## Build & Test Results

- Build: succeeded (0 warnings, 0 errors, `-warnaserror`)
- Tests: 156 total, 156 passed, 0 failed (13 new tests for TASK-012)