using AiDevLoop.Core.Domain;
using Xunit;
using TaskStatus = AiDevLoop.Core.Domain.TaskStatus;

namespace AiDevLoop.Core.Tests;

/// <summary>
/// Unit tests for <see cref="TaskSelector.SelectTask"/>.
/// </summary>
public class TaskSelectorTests
{
    // -----------------------------------------------------------------------
    // Helpers
    // -----------------------------------------------------------------------

    private static TaskId Id(string value) => new(value);

    private static TaskDefinition MakeTask(
        string id,
        TaskStatus status = TaskStatus.Pending,
        string[]? dependsOn = null) =>
        new(
            Id: new TaskId(id),
            Name: id,
            Status: status,
            Complexity: Complexity.Simple,
            DependsOn: (dependsOn ?? []).Select(d => new TaskId(d)).ToList(),
            Description: string.Empty,
            Steps: [],
            AcceptanceCriteria: [],
            FilesInScope: []);

    private static Plan SingleMilestonePlan(params TaskDefinition[] tasks) =>
        new("Test Plan", [new Milestone(1, "M1", tasks)]);

    // -----------------------------------------------------------------------
    // Auto-select (taskId == null)
    // -----------------------------------------------------------------------

    [Fact]
    public void AutoSelect_returns_first_pending_task_with_no_deps()
    {
        var t1 = MakeTask("T-001");
        var t2 = MakeTask("T-002");
        var plan = SingleMilestonePlan(t1, t2);

        var result = TaskSelector.SelectTask(plan, null);

        var ok = Assert.IsType<Result<TaskDefinition, SelectionError>.Ok>(result);
        Assert.Equal(Id("T-001"), ok.Value.Id);
    }

    [Fact]
    public void AutoSelect_skips_task_with_pending_dependency()
    {
        var t1 = MakeTask("T-001", TaskStatus.Pending);
        var t2 = MakeTask("T-002", TaskStatus.Pending, dependsOn: ["T-001"]);
        // t2 depends on t1 which is still Pending → t1 should be selected
        var plan = SingleMilestonePlan(t2, t1); // note: t2 first to test ordering

        var result = TaskSelector.SelectTask(plan, null);

        var ok = Assert.IsType<Result<TaskDefinition, SelectionError>.Ok>(result);
        Assert.Equal(Id("T-001"), ok.Value.Id);
    }

    [Fact]
    public void AutoSelect_selects_task_whose_deps_are_done()
    {
        var t1 = MakeTask("T-001", TaskStatus.Done);
        var t2 = MakeTask("T-002", TaskStatus.Pending, dependsOn: ["T-001"]);
        var plan = SingleMilestonePlan(t1, t2);

        var result = TaskSelector.SelectTask(plan, null);

        var ok = Assert.IsType<Result<TaskDefinition, SelectionError>.Ok>(result);
        Assert.Equal(Id("T-002"), ok.Value.Id);
    }

    [Fact]
    public void AutoSelect_skips_non_pending_tasks()
    {
        var t1 = MakeTask("T-001", TaskStatus.Done);
        var t2 = MakeTask("T-002", TaskStatus.InProgress);
        var t3 = MakeTask("T-003", TaskStatus.Pending);
        var plan = SingleMilestonePlan(t1, t2, t3);

        var result = TaskSelector.SelectTask(plan, null);

        var ok = Assert.IsType<Result<TaskDefinition, SelectionError>.Ok>(result);
        Assert.Equal(Id("T-003"), ok.Value.Id);
    }

    [Fact]
    public void AutoSelect_returns_NoPendingTasks_when_all_done()
    {
        var t1 = MakeTask("T-001", TaskStatus.Done);
        var t2 = MakeTask("T-002", TaskStatus.Done);
        var plan = SingleMilestonePlan(t1, t2);

        var result = TaskSelector.SelectTask(plan, null);

        var err = Assert.IsType<Result<TaskDefinition, SelectionError>.Err>(result);
        Assert.IsType<NoPendingTasks>(err.Error);
    }

    [Fact]
    public void AutoSelect_returns_NoPendingTasks_for_empty_plan()
    {
        var plan = new Plan("Empty", []);

        var result = TaskSelector.SelectTask(plan, null);

        var err = Assert.IsType<Result<TaskDefinition, SelectionError>.Err>(result);
        Assert.IsType<NoPendingTasks>(err.Error);
    }

    [Fact]
    public void AutoSelect_respects_milestone_then_task_order()
    {
        var t1 = MakeTask("T-001", TaskStatus.Pending);
        var t2 = MakeTask("T-002", TaskStatus.Pending);
        var plan = new Plan("Test", [
            new Milestone(1, "M1", [t1]),
            new Milestone(2, "M2", [t2])
        ]);

        var result = TaskSelector.SelectTask(plan, null);

        var ok = Assert.IsType<Result<TaskDefinition, SelectionError>.Ok>(result);
        Assert.Equal(Id("T-001"), ok.Value.Id);
    }

    [Fact]
    public void AutoSelect_within_milestone_selects_in_task_id_order_not_definition_order()
    {
        var t1 = MakeTask("T-001", TaskStatus.Pending);
        var t2 = MakeTask("T-002", TaskStatus.Pending);
        // t2 is defined first in the list, but T-001 should be selected (task ID order)
        var plan = SingleMilestonePlan(t2, t1);

        var result = TaskSelector.SelectTask(plan, null);

        var ok = Assert.IsType<Result<TaskDefinition, SelectionError>.Ok>(result);
        Assert.Equal(Id("T-001"), ok.Value.Id);
    }

    [Fact]
    public void AutoSelect_returns_NoPendingTasks_when_all_pending_have_unsatisfied_deps()
    {
        var t1 = MakeTask("T-001", TaskStatus.Pending, dependsOn: ["T-002"]);
        var t2 = MakeTask("T-002", TaskStatus.Pending, dependsOn: ["T-001"]);
        var plan = SingleMilestonePlan(t1, t2);

        var result = TaskSelector.SelectTask(plan, null);

        var err = Assert.IsType<Result<TaskDefinition, SelectionError>.Err>(result);
        Assert.IsType<NoPendingTasks>(err.Error);
    }

    // -----------------------------------------------------------------------
    // Explicit select (taskId provided)
    // -----------------------------------------------------------------------

    [Fact]
    public void ExplicitSelect_returns_task_when_pending_and_deps_done()
    {
        var t1 = MakeTask("T-001", TaskStatus.Done);
        var t2 = MakeTask("T-002", TaskStatus.Pending, dependsOn: ["T-001"]);
        var plan = SingleMilestonePlan(t1, t2);

        var result = TaskSelector.SelectTask(plan, Id("T-002"));

        var ok = Assert.IsType<Result<TaskDefinition, SelectionError>.Ok>(result);
        Assert.Equal(Id("T-002"), ok.Value.Id);
    }

    [Fact]
    public void ExplicitSelect_returns_task_when_pending_and_no_deps()
    {
        var t1 = MakeTask("T-001", TaskStatus.Pending);
        var plan = SingleMilestonePlan(t1);

        var result = TaskSelector.SelectTask(plan, Id("T-001"));

        var ok = Assert.IsType<Result<TaskDefinition, SelectionError>.Ok>(result);
        Assert.Equal(Id("T-001"), ok.Value.Id);
    }

    [Fact]
    public void ExplicitSelect_returns_TaskNotFound_for_unknown_id()
    {
        var t1 = MakeTask("T-001");
        var plan = SingleMilestonePlan(t1);

        var result = TaskSelector.SelectTask(plan, Id("T-999"));

        var err = Assert.IsType<Result<TaskDefinition, SelectionError>.Err>(result);
        var notFound = Assert.IsType<TaskNotFound>(err.Error);
        Assert.Equal(Id("T-999"), notFound.Id);
    }

    [Fact]
    public void ExplicitSelect_returns_TaskNotPending_when_task_is_done()
    {
        var t1 = MakeTask("T-001", TaskStatus.Done);
        var plan = SingleMilestonePlan(t1);

        var result = TaskSelector.SelectTask(plan, Id("T-001"));

        var err = Assert.IsType<Result<TaskDefinition, SelectionError>.Err>(result);
        var notPending = Assert.IsType<TaskNotPending>(err.Error);
        Assert.Equal(Id("T-001"), notPending.Id);
        Assert.Equal(TaskStatus.Done, notPending.CurrentStatus);
    }

    [Fact]
    public void ExplicitSelect_returns_TaskNotPending_when_task_is_in_progress()
    {
        var t1 = MakeTask("T-001", TaskStatus.InProgress);
        var plan = SingleMilestonePlan(t1);

        var result = TaskSelector.SelectTask(plan, Id("T-001"));

        var err = Assert.IsType<Result<TaskDefinition, SelectionError>.Err>(result);
        var notPending = Assert.IsType<TaskNotPending>(err.Error);
        Assert.Equal(TaskStatus.InProgress, notPending.CurrentStatus);
    }

    [Fact]
    public void ExplicitSelect_returns_TaskNotPending_when_task_is_blocked()
    {
        var t1 = MakeTask("T-001", TaskStatus.Blocked);
        var plan = SingleMilestonePlan(t1);

        var result = TaskSelector.SelectTask(plan, Id("T-001"));

        var err = Assert.IsType<Result<TaskDefinition, SelectionError>.Err>(result);
        var notPending = Assert.IsType<TaskNotPending>(err.Error);
        Assert.Equal(Id("T-001"), notPending.Id);
        Assert.Equal(TaskStatus.Blocked, notPending.CurrentStatus);
    }

    [Fact]
    public void ExplicitSelect_returns_DependenciesNotMet_with_unsatisfied_dep_ids()
    {
        var t1 = MakeTask("T-001", TaskStatus.Pending);
        var t2 = MakeTask("T-002", TaskStatus.Pending, dependsOn: ["T-001"]);
        var plan = SingleMilestonePlan(t1, t2);

        var result = TaskSelector.SelectTask(plan, Id("T-002"));

        var err = Assert.IsType<Result<TaskDefinition, SelectionError>.Err>(result);
        var depsNotMet = Assert.IsType<DependenciesNotMet>(err.Error);
        Assert.Equal(Id("T-002"), depsNotMet.Id);
        Assert.Single(depsNotMet.Unsatisfied);
        Assert.Equal(Id("T-001"), depsNotMet.Unsatisfied[0]);
    }

    [Fact]
    public void ExplicitSelect_lists_all_unsatisfied_deps()
    {
        var t1 = MakeTask("T-001", TaskStatus.Pending);
        var t2 = MakeTask("T-002", TaskStatus.Pending);
        var t3 = MakeTask("T-003", TaskStatus.Pending, dependsOn: ["T-001", "T-002"]);
        var plan = SingleMilestonePlan(t1, t2, t3);

        var result = TaskSelector.SelectTask(plan, Id("T-003"));

        var err = Assert.IsType<Result<TaskDefinition, SelectionError>.Err>(result);
        var depsNotMet = Assert.IsType<DependenciesNotMet>(err.Error);
        Assert.Equal(2, depsNotMet.Unsatisfied.Count);
        Assert.Contains(Id("T-001"), depsNotMet.Unsatisfied);
        Assert.Contains(Id("T-002"), depsNotMet.Unsatisfied);
    }
}
