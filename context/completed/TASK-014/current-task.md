# Task 014: Implement StateManager

## Description
A pure function that determines the resume point based on which context files exist and the task's current status in the plan. Used by the `resume` command to decide which development loop step to continue from. Also includes a helper to extract the task ID from `current-task.md` content.

## Definition of Done
- [x] Returns error when no `current-task.md` exists
- [x] Returns step 3 when only `current-task.md` exists
- [x] Returns step 4 when `implementation-notes.md` exists but `review.md` missing
- [x] Returns step 6 when `review.md` exists
- [x] `ExtractTaskId` parses task ID from markdown header
- [x] `ExtractTaskId` returns error for malformed content
- [x] No lint/type errors
- [x] Build succeeds in local environment
- [x] Ready for code review

## Steps
1. Create `src/AiDevLoop.Core/StateManager.cs` with static class containing:
   - `DetermineResumePoint(bool currentTaskExists, bool implementationNotesExists, bool reviewExists, TaskStatus statusInPlan) -> Result<ResumeState, string>`
   - `ExtractTaskId(string currentTaskContent) -> Result<TaskId, string>`
2. Implement resume logic:
   - No `current-task.md` -> error "No task in progress"
   - `current-task.md` only -> resume from step 3 (implement)
   - `implementation-notes.md` exists, no `review.md` -> resume from step 4 (validate)
   - `review.md` exists -> resume from step 6 (integration check)
3. Implement `ExtractTaskId` to parse `## TASK-XXX:` header from current-task.md content
4. Create `tests/AiDevLoop.Core.Tests/StateManagerTests.cs` with unit tests covering all cases
5. Run `dotnet build -warnaserror` and `dotnet test` to validate

## Acceptance Criteria
- [x] `DetermineResumePoint` returns Err("No task in progress") when `currentTaskExists` is false
- [x] `DetermineResumePoint` returns Ok(ResumeState.Step3) when only `currentTaskExists` is true
- [x] `DetermineResumePoint` returns Ok(ResumeState.Step4) when `currentTaskExists` and `implementationNotesExists` are true but `reviewExists` is false
- [x] `DetermineResumePoint` returns Ok(ResumeState.Step6) when `reviewExists` is true
- [x] `ExtractTaskId` successfully parses `## TASK-014: Implement StateManager` -> `TaskId("TASK-014")`
- [x] `ExtractTaskId` returns error for content without the expected header pattern
- [x] All public APIs have XML documentation
- [x] One type per file
- [x] Zero build warnings with `-warnaserror`
- [x] All unit tests pass

## Context References
- `docs/architecture.md#component-breakdown`
- `docs/requirements.md#fr-5-resume-execution`
- `docs/coding-style.md`
