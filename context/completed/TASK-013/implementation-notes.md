# Implementation Notes

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
