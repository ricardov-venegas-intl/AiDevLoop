# Task TASK-011: Implement ReviewAnalyzer

## Description
A pure function that parses a `review.md` document produced by the review agent. Extract issues with their classifications (`BLOCKING`, `NON-BLOCKING`, `NITPICK`), detect the `APPROVED` marker at the top of the document, and determine whether the review passes (no blocking issues exist).

## Definition of Done
- [ ] Detects `APPROVED` at document top as pass (no blocking issues)
- [ ] Extracts `BLOCKING` issues correctly
- [ ] Extracts `NON-BLOCKING` issues correctly
- [ ] Extracts `NITPICK` issues correctly
- [ ] `HasBlockingIssues` is true only when `BLOCKING` issues exist
- [ ] Returns correct `IterationNumber` in result
- [ ] Handles empty/malformed review document gracefully
- [ ] Handles varied casing of classification markers
- [ ] No lint/type errors
- [ ] Build succeeds in local environment
- [ ] Ready for code review

## Steps
1. Create `src/AiDevLoop.Core/ReviewAnalyzer.cs` with a static class containing `AnalyzeReview(string reviewDocument, int iterationNumber)` pure function
2. Create `tests/AiDevLoop.Core.Tests/ReviewAnalyzerTests.cs` with unit tests covering all acceptance criteria
3. Run `dotnet build -warnaserror` and `dotnet test` to verify

## Acceptance Criteria
- [ ] `APPROVED` (case-insensitive) anywhere in the first non-empty line → `HasBlockingIssues=false`, no issues extracted
- [ ] Lines/sections with `BLOCKING` marker → issues classified as `IssueClassification.Blocking`
- [ ] Lines/sections with `NON-BLOCKING` marker → classified as `IssueClassification.NonBlocking`
- [ ] Lines/sections with `NITPICK` marker → classified as `IssueClassification.Nitpick`
- [ ] `HasBlockingIssues` equals `Issues.Any(i => i.Classification == Blocking)`
- [ ] `IterationNumber` is stored in `ReviewResult.IterationNumber`
- [ ] null/empty/whitespace document → `ReviewResult` with empty issues, `HasBlockingIssues=false`
- [ ] Classification markers case-insensitive (`blocking`, `BLOCKING`, `Blocking` all recognized)
- [ ] Pure function — no I/O, no dependencies
- [ ] XML documentation on all public APIs