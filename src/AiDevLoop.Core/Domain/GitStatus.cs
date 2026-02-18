namespace AiDevLoop.Core.Domain;

/// <summary>
/// A snapshot of the current git repository status.
/// </summary>
/// <param name="HasStagedChanges">
/// <see langword="true"/> when there are staged changes that have not yet been committed.
/// </param>
/// <param name="HasUnstagedChanges">
/// <see langword="true"/> when there are modified files in the working directory that have not been staged.
/// </param>
/// <param name="ModifiedFiles">The relative paths of all modified files in the working directory.</param>
public record GitStatus(
    bool HasStagedChanges,
    bool HasUnstagedChanges,
    IReadOnlyList<string> ModifiedFiles);
