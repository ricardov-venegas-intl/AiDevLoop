using AiDevLoop.Core.Domain;
using Xunit;
using TaskStatus = AiDevLoop.Core.Domain.TaskStatus;

namespace AiDevLoop.Core.Tests;

/// <summary>
/// Unit tests for <see cref="StateManager"/>.
/// </summary>
public class StateManagerTests
{
    private static readonly TaskId SampleId = new("TASK-014");

    // -----------------------------------------------------------------------
    // DetermineResumePoint
    // -----------------------------------------------------------------------

    [Fact]
    public void DetermineResumePoint_no_current_task_returns_error()
    {
        var result = StateManager.DetermineResumePoint(
            currentTaskExists: false,
            implementationNotesExists: false,
            reviewExists: false,
            statusInPlan: TaskStatus.Pending,
            taskId: SampleId);

        var err = Assert.IsType<Result<ResumeState, string>.Err>(result);
        Assert.Equal("No task in progress", err.Error);
    }

    [Fact]
    public void DetermineResumePoint_only_current_task_returns_step3()
    {
        var result = StateManager.DetermineResumePoint(
            currentTaskExists: true,
            implementationNotesExists: false,
            reviewExists: false,
            statusInPlan: TaskStatus.InProgress,
            taskId: SampleId);

        var ok = Assert.IsType<Result<ResumeState, string>.Ok>(result);
        Assert.Equal(3, ok.Value.NextStep);
        Assert.Equal(SampleId, ok.Value.TaskId);
    }

    [Fact]
    public void DetermineResumePoint_with_pending_status_and_current_task_returns_step3()
    {
        // Plan might not yet have been updated — still step 3.
        var result = StateManager.DetermineResumePoint(
            currentTaskExists: true,
            implementationNotesExists: false,
            reviewExists: false,
            statusInPlan: TaskStatus.Pending,
            taskId: SampleId);

        var ok = Assert.IsType<Result<ResumeState, string>.Ok>(result);
        Assert.Equal(3, ok.Value.NextStep);
    }

    [Fact]
    public void DetermineResumePoint_with_implementation_notes_no_review_returns_step4()
    {
        var result = StateManager.DetermineResumePoint(
            currentTaskExists: true,
            implementationNotesExists: true,
            reviewExists: false,
            statusInPlan: TaskStatus.InProgress,
            taskId: SampleId);

        var ok = Assert.IsType<Result<ResumeState, string>.Ok>(result);
        Assert.Equal(4, ok.Value.NextStep);
        Assert.Equal(SampleId, ok.Value.TaskId);
    }

    [Fact]
    public void DetermineResumePoint_with_review_present_returns_step6()
    {
        var result = StateManager.DetermineResumePoint(
            currentTaskExists: true,
            implementationNotesExists: true,
            reviewExists: true,
            statusInPlan: TaskStatus.InProgress,
            taskId: SampleId);

        var ok = Assert.IsType<Result<ResumeState, string>.Ok>(result);
        Assert.Equal(6, ok.Value.NextStep);
        Assert.Equal(SampleId, ok.Value.TaskId);
    }

    [Fact]
    public void DetermineResumePoint_review_without_notes_returns_step6()
    {
        // review.md alone is enough to signal step 6, regardless of notes.
        var result = StateManager.DetermineResumePoint(
            currentTaskExists: true,
            implementationNotesExists: false,
            reviewExists: true,
            statusInPlan: TaskStatus.InProgress,
            taskId: SampleId);

        var ok = Assert.IsType<Result<ResumeState, string>.Ok>(result);
        Assert.Equal(6, ok.Value.NextStep);
    }

    // -----------------------------------------------------------------------
    // ExtractTaskId
    // -----------------------------------------------------------------------

    [Fact]
    public void ExtractTaskId_valid_header_returns_task_id()
    {
        const string content = """
            # Current Task

            ## TASK-014: Implement StateManager

            Some description here.
            """;

        var result = StateManager.ExtractTaskId(content);

        var ok = Assert.IsType<Result<TaskId, string>.Ok>(result);
        Assert.Equal(new TaskId("TASK-014"), ok.Value);
    }

    [Fact]
    public void ExtractTaskId_header_at_start_of_file_returns_task_id()
    {
        const string content = "## TASK-001: Some task\nDetails follow.";

        var result = StateManager.ExtractTaskId(content);

        var ok = Assert.IsType<Result<TaskId, string>.Ok>(result);
        Assert.Equal(new TaskId("TASK-001"), ok.Value);
    }

    [Fact]
    public void ExtractTaskId_missing_header_returns_error()
    {
        const string content = """
            # Current Task

            No proper task header here.
            """;

        var result = StateManager.ExtractTaskId(content);

        Assert.IsType<Result<TaskId, string>.Err>(result);
    }

    [Fact]
    public void ExtractTaskId_malformed_header_without_colon_returns_error()
    {
        // Missing the colon after the task ID — should not match.
        const string content = "## TASK-007 Missing colon\nSome content.";

        var result = StateManager.ExtractTaskId(content);

        Assert.IsType<Result<TaskId, string>.Err>(result);
    }

    [Fact]
    public void ExtractTaskId_empty_content_returns_error()
    {
        var result = StateManager.ExtractTaskId(string.Empty);

        Assert.IsType<Result<TaskId, string>.Err>(result);
    }

    [Fact]
    public void ExtractTaskId_task_id_not_at_line_start_returns_error()
    {
        // "## TASK-" must be at the start of a line.
        const string content = "Some prefix ## TASK-005: Indented\nOther content.";

        var result = StateManager.ExtractTaskId(content);

        Assert.IsType<Result<TaskId, string>.Err>(result);
    }
}
