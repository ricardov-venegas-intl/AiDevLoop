# Task TASK-013: Implement CommitMessageBuilder

## Description
A pure function that generates a conventional commit message from a `TaskDefinition`. Format: `feat(TASK-XXX): <task title>` where the task title is cleaned and lowercased for the commit message subject line.

## Definition of Done
- [ ] Generates `feat(TASK-001): add post /auth/login endpoint` format
- [ ] Includes task ID in scope
- [ ] Title is lowercased
- [ ] Truncates to 72 characters with `...` for long titles
- [ ] No trailing period
- [ ] No lint/type errors
- [ ] Build succeeds in local environment
- [ ] Ready for code review

## Steps
1. Create `src/AiDevLoop.Core/CommitMessageBuilder.cs` with a static `GenerateCommitMessage(TaskDefinition task)` pure function
2. Create `tests/AiDevLoop.Core.Tests/CommitMessageBuilderTests.cs` with unit tests covering all acceptance criteria
3. Run `dotnet build -warnaserror` and `dotnet test` to verify

## Acceptance Criteria
- [ ] Output format: `feat(<task-id>): <lowercased title>`
- [ ] Task ID taken from `task.Id.Value` (e.g. `TASK-001`)
- [ ] Title lowercased
- [ ] Subject line max 72 characters; truncate with `...` if exceeded (the `...` counts toward the 72)
- [ ] No trailing period on the subject line
- [ ] Pure function — no I/O, no dependencies
- [ ] XML documentation on all public APIs

## Definition of Done
- [ ] Builds prompt with template + task + referenced files in correct order
- [ ] Sections separated by clear delimiters with source identification
- [ ] Includes only files matching context references
- [ ] Skips missing reference files with a placeholder comment
- [ ] Handles empty context references list (template + task only)
- [ ] Output is a valid string
- [ ] No lint/type errors
- [ ] Build succeeds in local environment
- [ ] Ready for code review

## Steps
1. Create `src/AiDevLoop.Core/PromptBuilder.cs` with a static `BuildPrompt` pure function
2. Create `tests/AiDevLoop.Core.Tests/PromptBuilderTests.cs` with unit tests covering all acceptance criteria
3. Run `dotnet build -warnaserror` and `dotnet test` to verify

## Acceptance Criteria
- [ ] Concatenation order: prompt template → task content → referenced doc contents
- [ ] Sections separated by `---` horizontal rule and a header identifying the source file (e.g. `## docs/architecture.md`)
- [ ] Only files listed in `contextReferences` are included; files not in `loadedFiles` get a placeholder comment
- [ ] Empty `contextReferences` → output is template + task only
- [ ] Pure function — no I/O, no dependencies
- [ ] XML documentation on all public APIs

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