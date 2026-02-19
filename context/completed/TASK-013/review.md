# Code Review Report: TASK-013

## Summary

The `CommitMessageBuilder` implementation is correct and well-structured. It satisfies all spec requirements — conventional commit format, lowercase, trailing-period strip, 72-char truncation with `...` replacing the last three title characters. No blocking issues found.

## Issues Found

### Blocking

- (none)

### Non-Blocking

- **NB-1 — No guard when `prefix` alone exceeds or nearly exceeds 72 characters.**
  `GenerateCommitMessage` [CommitMessageBuilder.cs](../src/AiDevLoop.Core/CommitMessageBuilder.cs#L38):

  ```csharp
  var maxTitleLength = MaxSubjectLength - prefix.Length;
  title = string.Concat(title.AsSpan(0, maxTitleLength - 3), "...");
  ```

  If `maxTitleLength < 3` (i.e., `prefix.Length > 69`), then `maxTitleLength - 3` is negative and `AsSpan` throws `ArgumentOutOfRangeException`. A task ID of 62+ characters would trigger this. Not currently possible with real task IDs (e.g., `TASK-013` → prefix = 16 chars), but there is no defensive check or meaningful error message. Consider:

  ```csharp
  if (maxTitleLength <= 3)
      throw new ArgumentException(
          $"Task ID '{task.Id.Value}' produces a prefix that leaves no room for a title.", nameof(task));
  ```

- **NB-2 — Missing test: title reduced to empty string by `TrimEnd('.')`.**
  A `Name` of `"."` or `"..."` becomes an empty string after `TrimEnd('.')`, producing `feat(TASK-X): ` (trailing space, no title). The spec does not define this case and the code does not crash, but no test exercises it. Add a test to document and pin the current behaviour.

### Nitpicks

- **N-1 — `string.Concat` for the prefix; prefer interpolation.**
  `var prefix = string.Concat("feat(", task.Id.Value, "): ");` is unusual where a simple `$"feat({task.Id.Value}): "` would be more idiomatic and readable. The `string.Concat` + `AsSpan` pattern is appropriate for the hot truncation path but not for a simple three-part prefix.

- **N-2 — `var` on `prefix` and `title` is borderline per style rule 10.**
  The return type of `string.Concat` is `string`, but it is not immediately obvious from the expression form used. `string prefix = ...` and `string title = ...` are marginally clearer and strictly comply with rule 10 ("use `var` only when the type is explicit on the right-hand side").

## Compliance

- [x] Architecture adherence — pure static function in `AiDevLoop.Core`, zero I/O, no project-reference violations
- [x] Code style compliance — Allman braces, XML docs present, `ArgumentNullException.ThrowIfNull`, `PascalCase` constants; minor `var` and `string.Concat` style gaps (nitpick level only)
- [x] Test coverage adequate — 10 tests cover happy path, uppercase, lowercase, trailing period, internal period, exactly-72, over-72, ellipsis replacement, combined period+truncation, and null guard
- [ ] Risk areas addressed — the negative-`maxTitleLength` path is unguarded (NB-1); empty-title edge case is untested (NB-2)
