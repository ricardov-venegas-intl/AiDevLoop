# Task 005: Implement ConfigurationValidator

## Description
A pure function that validates a `Configuration` object against expected schema rules. Returns `Result<Configuration, IReadOnlyList<string>>` where the error case contains all validation error messages (not just the first).

## Definition of Done
- [ ] Valid configuration returns `Ok`
- [ ] Invalid LLM provider (e.g., `"gpt"`) returns error
- [ ] `maxReviewIterations` of 0 or negative returns error
- [ ] Empty path values return errors
- [ ] Multiple validation errors accumulated in single result
- [ ] Null/missing validation commands do not trigger errors
- [ ] No lint/type errors
- [ ] Build succeeds in local environment
- [ ] Ready for code review

## Steps
1. Create `src/AiDevLoop.Core/ConfigurationValidator.cs` with a static class containing a `Validate` pure function
2. Create `tests/AiDevLoop.Core.Tests/ConfigurationValidatorTests.cs` with unit tests covering all validation rules
3. Run `dotnet build` — verify zero warnings with `-warnaserror`
4. Run `dotnet test` — verify all tests pass

## Acceptance Criteria
- `ConfigurationValidator.Validate(config)` returns `Result<Configuration, IReadOnlyList<string>>.Ok` for a valid configuration
- Returns `Err` containing an error message for provider `"gpt"` (not `"claude"` or `"copilot"`)
- Returns `Err` for `MaxReviewIterations` <= 0
- Returns `Err` for empty `Docs`, `Context`, or `Prompts` paths
- Multiple errors are collected and returned together in a single `Err`
- `null` or empty `Commands` dictionary does not produce errors
- Command entries with empty string values produce errors
