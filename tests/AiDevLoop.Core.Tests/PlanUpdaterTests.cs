using AiDevLoop.Core.Domain;
using Xunit;
using TaskStatus = AiDevLoop.Core.Domain.TaskStatus;

namespace AiDevLoop.Core.Tests;

/// <summary>
/// Unit tests for <see cref="PlanUpdater.UpdateTaskStatus"/>.
/// </summary>
public class PlanUpdaterTests
{
    // -----------------------------------------------------------------------
    // Helpers
    // -----------------------------------------------------------------------

    private static TaskId Id(string value) => new(value);

    /// <summary>
    /// Builds a compact three-task plan string for testing.
    /// Each task entry can specify whether its checkbox is checked and what status value appears.
    /// </summary>
    private static string MakePlan(
        string checkbox1 = "[ ]", string status1 = "pending",
        string checkbox2 = "[ ]", string status2 = "pending",
        string checkbox3 = "[ ]", string status3 = "pending") =>
        $"""
        # Implementation Plan

        ## Milestone 1
        - {checkbox1} TASK-001 · Task One
        - {checkbox2} TASK-002 · Task Two
        - {checkbox3} TASK-003 · Task Three

        ## TASK-001: Task One

        **Status:** {status1}
        **Dependencies:** none

        ## TASK-002: Task Two

        **Status:** {status2}
        **Dependencies:** none

        ## TASK-003: Task Three

        **Status:** {status3}
        **Dependencies:** none
        """;

    // -----------------------------------------------------------------------
    // Checkbox toggling
    // -----------------------------------------------------------------------

    [Fact]
    public void UpdateTaskStatus_Done_SetsCheckedCheckbox()
    {
        var plan = MakePlan();

        var result = PlanUpdater.UpdateTaskStatus(plan, Id("TASK-002"), TaskStatus.Done);

        Assert.Contains("- [x] TASK-002", result);
    }

    [Fact]
    public void UpdateTaskStatus_InProgress_SetsUncheckedCheckbox()
    {
        var plan = MakePlan(checkbox2: "[x]", status2: "done");

        var result = PlanUpdater.UpdateTaskStatus(plan, Id("TASK-002"), TaskStatus.InProgress);

        Assert.Contains("- [ ] TASK-002", result);
    }

    [Fact]
    public void UpdateTaskStatus_Pending_SetsUncheckedCheckbox()
    {
        var plan = MakePlan(checkbox2: "[x]", status2: "done");

        var result = PlanUpdater.UpdateTaskStatus(plan, Id("TASK-002"), TaskStatus.Pending);

        Assert.Contains("- [ ] TASK-002", result);
    }

    [Fact]
    public void UpdateTaskStatus_Blocked_SetsUncheckedCheckbox()
    {
        var plan = MakePlan(checkbox2: "[x]", status2: "done");

        var result = PlanUpdater.UpdateTaskStatus(plan, Id("TASK-002"), TaskStatus.Blocked);

        Assert.Contains("- [ ] TASK-002", result);
    }

    // -----------------------------------------------------------------------
    // Status field update
    // -----------------------------------------------------------------------

    [Fact]
    public void UpdateTaskStatus_Done_SetsStatusFieldToDone()
    {
        var plan = MakePlan();

        var result = PlanUpdater.UpdateTaskStatus(plan, Id("TASK-002"), TaskStatus.Done);

        // Locate the TASK-002 block and confirm its status field
        var task2Index = result.IndexOf("## TASK-002:", StringComparison.Ordinal);
        var statusIndex = result.IndexOf("**Status:** done", task2Index, StringComparison.Ordinal);
        Assert.True(statusIndex > task2Index, "Expected '**Status:** done' inside TASK-002 block");
    }

    [Fact]
    public void UpdateTaskStatus_InProgress_SetsStatusFieldToInProgress()
    {
        var plan = MakePlan();

        var result = PlanUpdater.UpdateTaskStatus(plan, Id("TASK-001"), TaskStatus.InProgress);

        var task1Index = result.IndexOf("## TASK-001:", StringComparison.Ordinal);
        var statusIndex = result.IndexOf("**Status:** in-progress", task1Index, StringComparison.Ordinal);
        Assert.True(statusIndex > task1Index, "Expected '**Status:** in-progress' inside TASK-001 block");
    }

    [Fact]
    public void UpdateTaskStatus_Pending_SetsStatusFieldToPending()
    {
        var plan = MakePlan(status1: "done");

        var result = PlanUpdater.UpdateTaskStatus(plan, Id("TASK-001"), TaskStatus.Pending);

        var task1Index = result.IndexOf("## TASK-001:", StringComparison.Ordinal);
        var statusIndex = result.IndexOf("**Status:** pending", task1Index, StringComparison.Ordinal);
        Assert.True(statusIndex > task1Index, "Expected '**Status:** pending' inside TASK-001 block");
    }

    [Fact]
    public void UpdateTaskStatus_Blocked_SetsStatusFieldToBlocked()
    {
        var plan = MakePlan();

        var result = PlanUpdater.UpdateTaskStatus(plan, Id("TASK-003"), TaskStatus.Blocked);

        var task3Index = result.IndexOf("## TASK-003:", StringComparison.Ordinal);
        var statusIndex = result.IndexOf("**Status:** blocked", task3Index, StringComparison.Ordinal);
        Assert.True(statusIndex > task3Index, "Expected '**Status:** blocked' inside TASK-003 block");
    }

    // -----------------------------------------------------------------------
    // Position coverage (first, middle, last)
    // -----------------------------------------------------------------------

    [Fact]
    public void UpdateTaskStatus_FirstTask_UpdatesCorrectly()
    {
        var plan = MakePlan();

        var result = PlanUpdater.UpdateTaskStatus(plan, Id("TASK-001"), TaskStatus.Done);

        Assert.Contains("- [x] TASK-001", result);
        var idx = result.IndexOf("## TASK-001:", StringComparison.Ordinal);
        Assert.True(result.IndexOf("**Status:** done", idx, StringComparison.Ordinal) > idx);
    }

    [Fact]
    public void UpdateTaskStatus_MiddleTask_UpdatesCorrectly()
    {
        var plan = MakePlan();

        var result = PlanUpdater.UpdateTaskStatus(plan, Id("TASK-002"), TaskStatus.Done);

        Assert.Contains("- [x] TASK-002", result);
        var idx = result.IndexOf("## TASK-002:", StringComparison.Ordinal);
        Assert.True(result.IndexOf("**Status:** done", idx, StringComparison.Ordinal) > idx);
    }

    [Fact]
    public void UpdateTaskStatus_LastTask_UpdatesCorrectly()
    {
        var plan = MakePlan();

        var result = PlanUpdater.UpdateTaskStatus(plan, Id("TASK-003"), TaskStatus.Done);

        Assert.Contains("- [x] TASK-003", result);
        var idx = result.IndexOf("## TASK-003:", StringComparison.Ordinal);
        Assert.True(result.IndexOf("**Status:** done", idx, StringComparison.Ordinal) > idx);
    }

    // -----------------------------------------------------------------------
    // Isolation — other content unchanged
    // -----------------------------------------------------------------------

    [Fact]
    public void UpdateTaskStatus_OnlyTargetTaskIsModified()
    {
        var plan = MakePlan();

        var result = PlanUpdater.UpdateTaskStatus(plan, Id("TASK-002"), TaskStatus.Done);

        // TASK-001 and TASK-003 checkbox and status must remain unchanged
        Assert.Contains("- [ ] TASK-001", result);
        Assert.Contains("- [ ] TASK-003", result);

        var idx1 = result.IndexOf("## TASK-001:", StringComparison.Ordinal);
        Assert.True(result.IndexOf("**Status:** pending", idx1, StringComparison.Ordinal) > idx1);

        var idx3 = result.IndexOf("## TASK-003:", StringComparison.Ordinal);
        Assert.True(result.IndexOf("**Status:** pending", idx3, StringComparison.Ordinal) > idx3);
    }

    // -----------------------------------------------------------------------
    // CRLF round-tripping
    // -----------------------------------------------------------------------

    [Fact]
    public void UpdateTaskStatus_CrlfInput_PreservesCrlf()
    {
        // Normalize to LF first — the raw string literal may already use CRLF on Windows.
        var lfPlan = MakePlan().Replace("\r\n", "\n");
        var crlfPlan = lfPlan.Replace("\n", "\r\n");

        var result = PlanUpdater.UpdateTaskStatus(crlfPlan, Id("TASK-002"), TaskStatus.Done);

        Assert.Contains("\r\n", result);
        Assert.DoesNotContain("\r\r\n", result);
        Assert.Contains("- [x] TASK-002", result);
    }

    // -----------------------------------------------------------------------
    // Unknown task ID — no-op contract
    // -----------------------------------------------------------------------

    [Fact]
    public void UpdateTaskStatus_UnknownTaskId_ReturnsOriginalContentUnchanged()
    {
        var plan = MakePlan();

        var result = PlanUpdater.UpdateTaskStatus(plan, Id("TASK-999"), TaskStatus.Done);

        Assert.Equal(plan, result);
    }

    // -----------------------------------------------------------------------
    // Checkbox already in opposite state
    // -----------------------------------------------------------------------

    [Fact]
    public void UpdateTaskStatus_AlreadyChecked_MovedToNonDone_Unchecks()
    {
        var plan = MakePlan(checkbox2: "[x]", status2: "done");

        var result = PlanUpdater.UpdateTaskStatus(plan, Id("TASK-002"), TaskStatus.Pending);

        Assert.Contains("- [ ] TASK-002", result);
        Assert.DoesNotContain("- [x] TASK-002", result);
    }
}
