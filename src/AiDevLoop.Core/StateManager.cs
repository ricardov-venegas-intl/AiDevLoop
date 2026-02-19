using System.Text.RegularExpressions;
using AiDevLoop.Core.Domain;
using TaskStatus = AiDevLoop.Core.Domain.TaskStatus;

namespace AiDevLoop.Core;

/// <summary>
/// Determines how a previously-started development loop should be resumed based on
/// the presence of context files and plan status.
/// </summary>
public static class StateManager
{
    private static readonly Regex s_taskIdPattern =
        new(@"^## (TASK-\d+):", RegexOptions.Multiline | RegexOptions.Compiled);

    /// <summary>
    /// Determines the step at which an interrupted development loop should resume.
    /// </summary>
    /// <remarks>
    /// The decision tree is:
    /// <list type="bullet">
    ///   <item><description>No <c>current-task.md</c> → error "No task in progress".</description></item>
    ///   <item><description><c>current-task.md</c> only → resume at Step 3 (implementation).</description></item>
    ///   <item><description><c>implementation-notes.md</c> present, no <c>review.md</c> → resume at Step 4 (review).</description></item>
    ///   <item><description><c>review.md</c> present → resume at Step 6 (commit).</description></item>
    /// </list>
    /// The <paramref name="statusInPlan"/> parameter is provided for context; a <see cref="TaskStatus.Pending"/>
    /// status alongside an existing <c>current-task.md</c> is treated as Step 3 (plan was not yet updated).
    /// </remarks>
    /// <param name="currentTaskExists">Whether <c>current-task.md</c> exists in the context directory.</param>
    /// <param name="implementationNotesExists">Whether <c>implementation-notes.md</c> exists in the context directory.</param>
    /// <param name="reviewExists">Whether <c>review.md</c> exists in the context directory.</param>
    /// <param name="statusInPlan">The task's current status as recorded in the implementation plan.</param>
    /// <param name="taskId">The identifier of the task being resumed.</param>
    /// <returns>
    /// A <see cref="Result{TValue,TError}"/> containing a <see cref="ResumeState"/> on success,
    /// or a descriptive error string on failure.
    /// </returns>
    public static Result<ResumeState, string> DetermineResumePoint(
        bool currentTaskExists,
        bool implementationNotesExists,
        bool reviewExists,
        TaskStatus statusInPlan,
        TaskId taskId)
    {
        // statusInPlan is intentionally unused in control flow: file presence is the
        // authoritative signal. A Pending status with an existing current-task.md
        // simply means the plan was not yet updated — handled by the Step 3 branch.
        _ = statusInPlan;

        if (!currentTaskExists)
            return new Result<ResumeState, string>.Err("No task in progress");

        if (reviewExists)
            return new Result<ResumeState, string>.Ok(new ResumeState(6, taskId));

        if (implementationNotesExists)
            return new Result<ResumeState, string>.Ok(new ResumeState(4, taskId));

        return new Result<ResumeState, string>.Ok(new ResumeState(3, taskId));
    }

    /// <summary>
    /// Extracts the <see cref="TaskId"/> from the contents of a <c>current-task.md</c> file.
    /// </summary>
    /// <remarks>
    /// The method looks for a header of the form <c>## TASK-NNN:</c> at the start of a line,
    /// where <c>NNN</c> is one or more digits.
    /// </remarks>
    /// <param name="currentTaskContent">The full text content of <c>current-task.md</c>.</param>
    /// <returns>
    /// A <see cref="Result{TValue,TError}"/> containing the parsed <see cref="TaskId"/> on success,
    /// or a descriptive error string when no matching header is found.
    /// </returns>
    public static Result<TaskId, string> ExtractTaskId(string currentTaskContent)
    {
        var match = s_taskIdPattern.Match(currentTaskContent);

        return match.Success
            ? new Result<TaskId, string>.Ok(new TaskId(match.Groups[1].Value))
            : new Result<TaskId, string>.Err("Could not find a TASK-NNN header in current-task.md");
    }
}
