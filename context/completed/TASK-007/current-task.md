# Task TASK-007: Implement MarkdownPlanParser

## Description
A pure parser that converts `docs/implementation-plan.md` into the in-memory `Plan` model used by the Core library. The parser extracts milestone lists and full task definition blocks and produces `TaskDefinition` objects with metadata, dependencies and `FilesInScope`.

## Definition of Done
- [x] Parser implemented in `src/AiDevLoop.Core/MarkdownPlanParser.cs`
- [x] Unit tests added and passing
- [x] Documentation & implementation notes recorded
- [x] Plan updated to mark the task complete

## Steps
1. Implement `MarkdownPlanParser.Parse(string)` returning `Result<Plan, string>`
2. Add unit tests covering normal and error cases
3. Run `dotnet build` and `dotnet test`
4. Add implementation notes and a code review summary
5. Archive task context files to `context/completed/TASK-007/`

## Acceptance Criteria
- [x] Correctly parses milestone checklists and task definition blocks
- [x] Produces `TaskDefinition` with Id, Name, Status, Complexity, DependsOn, Description, FilesInScope
- [x] Handles malformed input gracefully (returns `Err`)
