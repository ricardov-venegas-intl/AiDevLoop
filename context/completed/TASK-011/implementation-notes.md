# Implementation Notes — TASK-011: ReviewAnalyzer

## What was implemented

- `src/AiDevLoop.Core/ReviewAnalyzer.cs` — static class with a single public method `AnalyzeReview(string? reviewDocument, int iterationNumber)`
- `tests/AiDevLoop.Core.Tests/ReviewAnalyzerTests.cs` — 20 xUnit tests covering all acceptance criteria

## Key decisions

### NON-BLOCKING before BLOCKING
`NON-BLOCKING` contains the substring `BLOCKING`, so the `IndexOf` check for `NON-BLOCKING` must come first. If `BLOCKING` were checked first, every non-blocking line would be misclassified.

### No regex
Used `string.IndexOf` with `StringComparison.OrdinalIgnoreCase` throughout, as required. Bold-marker removal loops with `string.Remove` instead of `Regex.Replace`.

### Description extraction
1. `TrimStart('#', '-', '*', '>')` removes leading Markdown structure characters.
2. A `while` loop removes all `**` occurrences (handles nested or repeated bold markers).
3. After locating the keyword, the remainder is `TrimStart`'d of `:`, `-`, `*`, and whitespace before a final `Trim()`.

### APPROVED check scope
Only the first non-empty line is tested for the approved marker. Subsequent lines containing "APPROVED" are ignored so they don't shadow real issues.

### HasBlockingIssues derivation
Computed as `issuesArray.Any(i => i.Classification == IssueClassification.Blocking)` — never set independently, so it is always consistent with the Issues list.

### Return type
`issues.ToArray()` produces an immutable snapshot returned as `IReadOnlyList<ReviewIssue>` (via the `ReviewResult` record), consistent with the project's no-mutable-collections-from-APIs rule.

## Build & test results

- Build: succeeded, 0 warnings
- Tests: 144 total, 0 failed (20 new tests added for TASK-011)
