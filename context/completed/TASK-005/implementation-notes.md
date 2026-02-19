# Implementation Notes: Task 005

## Task Name
Implement ConfigurationValidator

## Decisions Made

- **Case-insensitive LLM provider matching**: The spec says `"claude"` or `"copilot"`, but `StringComparer.OrdinalIgnoreCase` is used so `"Claude"` and `"COPILOT"` are also accepted. This is more user-friendly for CLI tools where config files may use mixed casing.

- **Accumulate all errors with a `List<string>`**: Per the constraint, all errors are gathered before returning. A private `List<string>` is passed through private helper methods; the public method wraps it in `IReadOnlyList<string>` for the error case.

- **Null `Commands` dictionary is valid**: `IReadOnlyDictionary<string, string>?` is nullable in the helper so a null dictionary doesn't raise errors. The domain type currently declares it non-nullable, but the validator accounts for defensive null handling.

- **Whitespace-only values are rejected**: `string.IsNullOrWhiteSpace` is used for path and command value checks, not `string.IsNullOrEmpty`. A path of `"   "` is semantically empty.

- **No structural changes to `Configuration` or related domain types**: The validator is a pure function operating on the existing domain model — no new fields, no modifications.

## Known Limitations

- **Command key validation is not performed**: Only command values are validated for being non-empty strings. Empty/whitespace command keys are not checked (the spec does not require this).

- **No path format validation**: Paths are only checked for non-empty. Cross-platform path format validity (illegal characters, reserved names, etc.) is not validated here — that is a concern for the I/O layer at runtime.

## Risk Areas

- **No risk areas identified** — this is a simple pure function with straightforward, accumulation-based validation logic.

## Dependencies

- `AiDevLoop.Core.Domain.Configuration`, `PathsConfiguration`, `ValidationConfiguration`, `Result<T,E>` from TASK-002.
- TASK-004 (`ConfigurationLoader`) produces `Configuration` objects that flow into this validator.

## Testing Notes

- 25 unit tests covering all 6 validation rules.
- Theory tests (`[InlineData]`) exercise boundary cases (empty string, whitespace string) for path and command value checks.
- `Validate_AccumulatesMultipleErrors` asserts exactly 5 errors when LLM, MaxReviewIterations, and all three paths are simultaneously invalid.
- All 34 `AiDevLoop.Core.Tests` pass.
- Pre-existing failure `Load_ReadsYamlFile` in `AiDevLoop.Shell.Tests` is unrelated to this task.

## Build & Test Status

✅ All projects build successfully with `-warnaserror`
✅ 34/34 Core.Tests pass (25 new)
✅ No compiler warnings

## Files Created

- `src/AiDevLoop.Core/ConfigurationValidator.cs`
- `tests/AiDevLoop.Core.Tests/ConfigurationValidatorTests.cs`
