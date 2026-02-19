using System;
using System.Linq;
using AiDevLoop.Core.Domain;
using Xunit;

namespace AiDevLoop.Core.Tests;

/// <summary>
/// Unit tests for <see cref="MarkdownPlanParser"/>.
/// </summary>
public class MarkdownPlanParserTests
{
    private const string SamplePlan =
@"# Implementation Plan

## Milestone 3 — Plan Parsing & Task Management

- [ ] TASK-007 · Medium · Implement MarkdownPlanParser
- [ ] TASK-008 · Simple · Implement TaskSelector

## TASK-007: Implement MarkdownPlanParser

**Milestone:** 3 — Plan Parsing & Task Management
**Status:** pending
**Complexity:** Medium
**Depends on:** TASK-002

### What to build
A pure function that parses the content of an `implementation-plan.md` file into a `Plan` record. Extract milestone lists (headers + checkbox items) and full task definition blocks.

### Files in scope
- `src/AiDevLoop.Core/MarkdownPlanParser.cs` (create)
- `tests/AiDevLoop.Core.Tests/MarkdownPlanParserTests.cs` (create)

### Validation criteria
- [ ] Parses a well-formed plan with multiple milestones and tasks
- [ ] Extracts task ID, title, status, complexity, dependencies correctly
";

    [Fact]
    public void Parse_ReturnsOk_ForWellFormedSample()
    {
        var result = MarkdownPlanParser.Parse(SamplePlan);

        var ok = Assert.IsType<Result<Plan, string>.Ok>(result);
        var plan = ok.Value;

        Assert.Equal("Implementation Plan", plan.Title);
        var milestone = Assert.Single(plan.Milestones.Where(m => m.Number == 3));

        var task = Assert.Single(milestone.Tasks.Where(t => t.Id.Value == "TASK-007"));
        Assert.Equal("Implement MarkdownPlanParser", task.Name);
        Assert.Equal(AiDevLoop.Core.Domain.TaskStatus.Pending, task.Status);
        Assert.Equal(AiDevLoop.Core.Domain.Complexity.Medium, task.Complexity);
        Assert.Contains(task.DependsOn, d => d.Value == "TASK-002");

        var fileRef = Assert.Single(task.FilesInScope);
        Assert.Equal("src/AiDevLoop.Core/MarkdownPlanParser.cs", fileRef.Path);
        Assert.Equal(FileReferenceKind.Create, fileRef.Kind);
    }

    [Fact]
    public void Parse_ReturnsErr_ForEmptyContent()
    {
        var result = MarkdownPlanParser.Parse(string.Empty);

        var err = Assert.IsType<Result<Plan, string>.Err>(result);
        Assert.Contains("empty", err.Error, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public void Parse_ReturnsErr_WhenNoTaskDefinitions()
    {
        var md = "# Title\n\n## Milestone 1 — X\n\n- [ ] TASK-999 · Simple · Placeholder";
        var result = MarkdownPlanParser.Parse(md);

        var err = Assert.IsType<Result<Plan, string>.Err>(result);
        Assert.Contains("No task definitions", err.Error, StringComparison.OrdinalIgnoreCase);
    }
}
