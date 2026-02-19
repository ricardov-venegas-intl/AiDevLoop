namespace AiDevLoop.Shell.Adapters;

using AiDevLoop.Core.Domain;

/// <summary>
/// Provides asynchronous git operations for repository management.
/// </summary>
public interface IGitClient
{
    /// <summary>
    /// Stages all changes in the working directory (equivalent to <c>git add .</c>).
    /// </summary>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    Task StageAllAsync(CancellationToken cancellationToken);

    /// <summary>
    /// Commits all staged changes with the specified commit message.
    /// </summary>
    /// <param name="message">The commit message.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    Task CommitAsync(string message, CancellationToken cancellationToken);

    /// <summary>
    /// Retrieves the current status of the git repository.
    /// </summary>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>A <see cref="GitStatus"/> snapshot of the repository state.</returns>
    Task<GitStatus> GetStatusAsync(CancellationToken cancellationToken);
}
