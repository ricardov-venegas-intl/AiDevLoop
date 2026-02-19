using AiDevLoop.Core.Domain;

namespace AiDevLoop.Core;

/// <summary>
/// Builds conventional commit messages from a <see cref="TaskDefinition"/>.
/// </summary>
public static class CommitMessageBuilder
{
    private const int MaxSubjectLength = 72;

    /// <summary>
    /// Generates a conventional commit message subject line for the given task.
    /// </summary>
    /// <remarks>
    /// Format: <c>feat(&lt;task-id&gt;): &lt;lowercased-title&gt;</c>.
    /// If the result exceeds 72 characters the title portion is truncated so the
    /// total length is exactly 72 characters, with the last three characters of
    /// the title replaced by <c>...</c>.
    /// Any trailing period is stripped from the title before processing.
    /// </remarks>
    /// <param name="task">The task for which to generate a commit message.</param>
    /// <returns>A conventional commit subject line of at most 72 characters.</returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="task"/> is <c>null</c>.</exception>
    public static string GenerateCommitMessage(TaskDefinition task)
    {
        ArgumentNullException.ThrowIfNull(task);

        string prefix = $"feat({task.Id.Value}): ";
        string title = task.Name.TrimEnd('.').ToLowerInvariant();

        string subject = prefix + title;
        if (subject.Length <= MaxSubjectLength)
        {
            return subject;
        }

        int maxTitleLength = MaxSubjectLength - prefix.Length;
        if (maxTitleLength <= 3)
        {
            // Degenerate: task ID alone fills the line; return just the prefix truncated.
            return prefix.Length > MaxSubjectLength
                ? prefix[..MaxSubjectLength]
                : prefix.TrimEnd();
        }

        title = string.Concat(title.AsSpan(0, maxTitleLength - 3), "...");
        return prefix + title;
    }
}
