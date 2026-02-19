using AiDevLoop.Core.Domain;
using TaskStatus = AiDevLoop.Core.Domain.TaskStatus;

namespace AiDevLoop.Core;

/// <summary>
/// Updates task status fields in the raw markdown content of an implementation plan.
/// </summary>
public static class PlanUpdater
{
    /// <summary>
    /// Returns a copy of <paramref name="planContent"/> with the milestone checkbox and
    /// <c>**Status:**</c> field updated for <paramref name="taskId"/>.
    /// </summary>
    /// <remarks>
    /// Two replacements are performed:
    /// <list type="bullet">
    ///   <item>The milestone list checkbox: <c>- [x]</c> when <paramref name="newStatus"/> is
    ///   <see cref="TaskStatus.Done"/>; <c>- [ ]</c> otherwise.</item>
    ///   <item>The <c>**Status:**</c> field inside the task definition block: lowercase status
    ///   string (<c>pending</c>, <c>in-progress</c>, <c>done</c>, <c>blocked</c>).</item>
    /// </list>
    /// All other content is preserved unchanged.
    /// </remarks>
    /// <param name="planContent">The raw markdown text of the implementation plan.</param>
    /// <param name="taskId">The identifier of the task to update.</param>
    /// <param name="newStatus">The new status to apply.</param>
    /// <returns>The updated markdown text.</returns>
    public static string UpdateTaskStatus(string planContent, TaskId taskId, TaskStatus newStatus)
    {
        var separator = planContent.Contains("\r\n", StringComparison.Ordinal) ? "\r\n" : "\n";
        var lines = planContent.Split(["\r\n", "\n"], StringSplitOptions.None);

        var headingPrefix = $"## {taskId.Value}:";
        var newCheckbox = newStatus == TaskStatus.Done ? "- [x]" : "- [ ]";
        var newStatusValue = ToStatusString(newStatus);

        bool inTaskBlock = false;
        bool statusUpdated = false;

        for (int i = 0; i < lines.Length; i++)
        {
            var line = lines[i];

            if (IsCheckboxLine(line, taskId))
            {
                // Replace only the leading "- [ ]" or "- [x]" (5 chars), preserve the rest
                lines[i] = newCheckbox + line[5..];
            }
            else if (line.StartsWith("## ", StringComparison.Ordinal))
            {
                inTaskBlock = line.StartsWith(headingPrefix, StringComparison.Ordinal);
                statusUpdated = false;
            }
            else if (inTaskBlock && !statusUpdated && line.StartsWith("**Status:**", StringComparison.Ordinal))
            {
                lines[i] = $"**Status:** {newStatusValue}";
                statusUpdated = true;
            }
        }

        return string.Join(separator, lines);
    }

    private static bool IsCheckboxLine(string line, TaskId taskId)
    {
        var unchecked_ = $"- [ ] {taskId.Value}";
        var checked_ = $"- [x] {taskId.Value}";
        return MatchesTaskPrefix(line, unchecked_) || MatchesTaskPrefix(line, checked_);
    }

    // Ensures the task ID is followed by a space or end-of-line to avoid partial ID matches.
    private static bool MatchesTaskPrefix(string line, string prefix) =>
        line.StartsWith(prefix, StringComparison.Ordinal) &&
        (line.Length == prefix.Length || line[prefix.Length] == ' ');

    private static string ToStatusString(TaskStatus status) => status switch
    {
        TaskStatus.Pending => "pending",
        TaskStatus.InProgress => "in-progress",
        TaskStatus.Done => "done",
        TaskStatus.Blocked => "blocked",
        _ => throw new ArgumentOutOfRangeException(nameof(status), status, null),
    };
}
