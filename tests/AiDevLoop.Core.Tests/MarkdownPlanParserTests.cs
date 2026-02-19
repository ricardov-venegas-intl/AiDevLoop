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

        Assert.Equal(2, task.FilesInScope.Count);
        Assert.Contains(task.FilesInScope, f => f.Path == "src/AiDevLoop.Core/MarkdownPlanParser.cs" && f.Kind == FileReferenceKind.Create);
        Assert.Contains(task.FilesInScope, f => f.Path == "tests/AiDevLoop.Core.Tests/MarkdownPlanParserTests.cs" && f.Kind == FileReferenceKind.Create);
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

    [Fact]
    public void Parse_HandlesMultipleDependencies()
    {
        var md = @"# Plan

## Milestone 1 — Test

- [ ] TASK-001 · Simple · Test task

## TASK-001: Test task

**Status:** pending
**Complexity:** Simple
**Depends on:** TASK-002, TASK-003, TASK-004

### What to build
Test description.
";
        var result = MarkdownPlanParser.Parse(md);

        var ok = Assert.IsType<Result<Plan, string>.Ok>(result);
        var task = ok.Value.Milestones[0].Tasks[0];
        Assert.Equal(3, task.DependsOn.Count);
        Assert.Contains(task.DependsOn, d => d.Value == "TASK-002");
        Assert.Contains(task.DependsOn, d => d.Value == "TASK-003");
        Assert.Contains(task.DependsOn, d => d.Value == "TASK-004");
    }

    [Fact]
    public void Parse_HandlesNoDependencies_WithDash()
    {
        var md = @"# Plan

## Milestone 1 — Test

- [ ] TASK-001 · Simple · Test task

## TASK-001: Test task

**Status:** pending
**Complexity:** Simple
**Depends on:** —

### What to build
Test description.
";
        var result = MarkdownPlanParser.Parse(md);

        var ok = Assert.IsType<Result<Plan, string>.Ok>(result);
        var task = ok.Value.Milestones[0].Tasks[0];
        Assert.Empty(task.DependsOn);
    }

    [Fact]
    public void Parse_HandlesNoDependencies_WithHyphen()
    {
        var md = @"# Plan

## Milestone 1 — Test

- [ ] TASK-001 · Simple · Test task

## TASK-001: Test task

**Status:** pending
**Complexity:** Simple
**Depends on:** -

### What to build
Test description.
";
        var result = MarkdownPlanParser.Parse(md);

        var ok = Assert.IsType<Result<Plan, string>.Ok>(result);
        var task = ok.Value.Milestones[0].Tasks[0];
        Assert.Empty(task.DependsOn);
    }

    [Fact]
    public void Parse_HandlesInProgressStatus()
    {
        var md = @"# Plan

## Milestone 1 — Test

- [ ] TASK-001 · Simple · Test task

## TASK-001: Test task

**Status:** in-progress
**Complexity:** Simple
**Depends on:** —

### What to build
Test description.
";
        var result = MarkdownPlanParser.Parse(md);

        var ok = Assert.IsType<Result<Plan, string>.Ok>(result);
        var task = ok.Value.Milestones[0].Tasks[0];
        Assert.Equal(AiDevLoop.Core.Domain.TaskStatus.InProgress, task.Status);
    }

    [Fact]
    public void Parse_HandlesBlockedStatus()
    {
        var md = @"# Plan

## Milestone 1 — Test

- [ ] TASK-001 · Simple · Test task

## TASK-001: Test task

**Status:** blocked
**Complexity:** Simple
**Depends on:** —

### What to build
Test description.
";
        var result = MarkdownPlanParser.Parse(md);

        var ok = Assert.IsType<Result<Plan, string>.Ok>(result);
        var task = ok.Value.Milestones[0].Tasks[0];
        Assert.Equal(AiDevLoop.Core.Domain.TaskStatus.Blocked, task.Status);
    }

    [Fact]
    public void Parse_HandlesModifyFileReference()
    {
        var md = @"# Plan

## Milestone 1 — Test

- [ ] TASK-001 · Simple · Test task

## TASK-001: Test task

**Status:** pending
**Complexity:** Simple
**Depends on:** —

### What to build
Test description.

### Files in scope
- `src/Test.cs` (modify)
";
        var result = MarkdownPlanParser.Parse(md);

        var ok = Assert.IsType<Result<Plan, string>.Ok>(result);
        var task = ok.Value.Milestones[0].Tasks[0];
        var fileRef = Assert.Single(task.FilesInScope);
        Assert.Equal("src/Test.cs", fileRef.Path);
        Assert.Equal(FileReferenceKind.Modify, fileRef.Kind);
    }

    [Fact]
    public void Parse_HandlesReadOnlyFileReference()
    {
        var md = @"# Plan

## Milestone 1 — Test

- [ ] TASK-001 · Simple · Test task

## TASK-001: Test task

**Status:** pending
**Complexity:** Simple
**Depends on:** —

### What to build
Test description.

### Files in scope
- `docs/README.md` (read-only reference)
";
        var result = MarkdownPlanParser.Parse(md);

        var ok = Assert.IsType<Result<Plan, string>.Ok>(result);
        var task = ok.Value.Milestones[0].Tasks[0];
        var fileRef = Assert.Single(task.FilesInScope);
        Assert.Equal("docs/README.md", fileRef.Path);
        Assert.Equal(FileReferenceKind.ReadOnlyReference, fileRef.Kind);
    }

    [Fact]
    public void Parse_HandlesValidationCriteria_WithCheckboxes()
    {
        var md = @"# Plan

## Milestone 1 — Test

- [ ] TASK-001 · Simple · Test task

## TASK-001: Test task

**Status:** pending
**Complexity:** Simple
**Depends on:** —

### What to build
Test description.

### Validation criteria
- [x] First criterion
- [ ] Second criterion
";
        var result = MarkdownPlanParser.Parse(md);

        var ok = Assert.IsType<Result<Plan, string>.Ok>(result);
        var task = ok.Value.Milestones[0].Tasks[0];
        Assert.Equal(2, task.AcceptanceCriteria.Count);
        Assert.Contains("First criterion", task.AcceptanceCriteria);
        Assert.Contains("Second criterion", task.AcceptanceCriteria);
    }

    [Fact]
    public void Parse_HandlesValidationCriteria_WithPlainBullets()
    {
        var md = @"# Plan

## Milestone 1 — Test

- [ ] TASK-001 · Simple · Test task

## TASK-001: Test task

**Status:** pending
**Complexity:** Simple
**Depends on:** —

### What to build
Test description.

### Validation criteria
- First criterion
- Second criterion
";
        var result = MarkdownPlanParser.Parse(md);

        var ok = Assert.IsType<Result<Plan, string>.Ok>(result);
        var task = ok.Value.Milestones[0].Tasks[0];
        Assert.Equal(2, task.AcceptanceCriteria.Count);
        Assert.Contains("First criterion", task.AcceptanceCriteria);
        Assert.Contains("Second criterion", task.AcceptanceCriteria);
    }

    [Fact]
    public void Parse_HandlesTaskInChecklistWithoutDetailedDefinition()
    {
        var md = @"# Plan

## Milestone 1 — Test

- [ ] TASK-001 · Simple · Defined task
- [ ] TASK-002 · Medium · Undefined task

## TASK-001: Defined task

**Status:** pending
**Complexity:** Simple
**Depends on:** —

### What to build
Test description.
";
        var result = MarkdownPlanParser.Parse(md);

        var ok = Assert.IsType<Result<Plan, string>.Ok>(result);
        var milestone = ok.Value.Milestones[0];
        Assert.Equal(2, milestone.Tasks.Count);

        var definedTask = milestone.Tasks[0];
        Assert.Equal("TASK-001", definedTask.Id.Value);
        Assert.Equal("Defined task", definedTask.Name);
        Assert.NotEmpty(definedTask.Description);

        var undefinedTask = milestone.Tasks[1];
        Assert.Equal("TASK-002", undefinedTask.Id.Value);
        Assert.Equal("Undefined task", undefinedTask.Name);
        Assert.Empty(undefinedTask.Description); // No detailed definition
    }
}
