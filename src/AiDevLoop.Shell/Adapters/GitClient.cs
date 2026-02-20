namespace AiDevLoop.Shell.Adapters;

using System.Threading;
using System.Threading.Tasks;

using AiDevLoop.Core.Domain;

/// <summary>
/// Implements <see cref="IGitClient"/> by delegating to <see cref="IProcessRunner"/>
/// to execute git commands in the current working directory.
/// </summary>
public sealed class GitClient : IGitClient
{
    private readonly IProcessRunner _processRunner;

    /// <summary>
    /// Initializes a new instance of <see cref="GitClient"/>.
    /// </summary>
    /// <param name="processRunner">The process runner used to invoke git.</param>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="processRunner"/> is <see langword="null"/>.</exception>
    public GitClient(IProcessRunner processRunner)
    {
        ArgumentNullException.ThrowIfNull(processRunner);
        _processRunner = processRunner;
    }

    /// <summary>
    /// Stages all changes in the working tree by running <c>git add .</c>.
    /// </summary>
    /// <param name="cancellationToken">A token to observe for cancellation requests.</param>
    /// <exception cref="InvalidOperationException">Thrown when git exits with a non-zero code.</exception>
    public async Task StageAllAsync(CancellationToken cancellationToken)
    {
        CommandResult result = await _processRunner.RunAsync("git", "add .", cancellationToken).ConfigureAwait(false);
        if (!result.Succeeded)
        {
            throw new InvalidOperationException(
                $"git add . failed with exit code {result.ExitCode}. Stderr: {result.Stderr}");
        }
    }

    /// <summary>
    /// Creates a commit with the specified message by running <c>git commit -m "…"</c>.
    /// </summary>
    /// <param name="message">The commit message.</param>
    /// <param name="cancellationToken">A token to observe for cancellation requests.</param>
    /// <exception cref="ArgumentException">Thrown when <paramref name="message"/> is null or whitespace.</exception>
    /// <exception cref="InvalidOperationException">Thrown when git exits with a non-zero code.</exception>
    public async Task CommitAsync(string message, CancellationToken cancellationToken)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(message);
        string escapedMessage = message.Replace("\\", "\\\\").Replace("\"", "\\\"");
        string arguments = $"commit -m \"{escapedMessage}\"";

        CommandResult result = await _processRunner.RunAsync("git", arguments, cancellationToken).ConfigureAwait(false);
        if (!result.Succeeded)
        {
            throw new InvalidOperationException(
                $"git commit failed with exit code {result.ExitCode}. Stderr: {result.Stderr}");
        }
    }

    /// <summary>
    /// Retrieves the current git status by running <c>git status --porcelain</c> and parsing the output.
    /// </summary>
    /// <param name="cancellationToken">A token to observe for cancellation requests.</param>
    /// <returns>
    /// A <see cref="GitStatus"/> describing staged changes, unstaged changes, and the list of modified file paths.
    /// </returns>
    /// <exception cref="InvalidOperationException">Thrown when git exits with a non-zero code.</exception>
    public async Task<GitStatus> GetStatusAsync(CancellationToken cancellationToken)
    {
        CommandResult result = await _processRunner.RunAsync("git", "status --porcelain", cancellationToken).ConfigureAwait(false);
        if (!result.Succeeded)
        {
            throw new InvalidOperationException(
                $"git status --porcelain failed with exit code {result.ExitCode}. Stderr: {result.Stderr}");
        }

        if (string.IsNullOrWhiteSpace(result.Stdout))
        {
            return new GitStatus(false, false, []);
        }

        bool hasStagedChanges = false;
        bool hasUnstagedChanges = false;
        List<string> modifiedFiles = [];

        foreach (string rawLine in result.Stdout.Split(['\r', '\n'], StringSplitOptions.RemoveEmptyEntries))
        {
            string line = rawLine.TrimEnd();
            if (line.Length < 3)
            {
                continue;
            }

            char x = line[0];
            char y = line[1];

            if (x != ' ' && x != '?')
            {
                hasStagedChanges = true;
            }

            if (y != ' ' && y != '?')
            {
                hasUnstagedChanges = true;
            }

            // Skip untracked files (??) from the ModifiedFiles list
            if (x == '?' && y == '?')
            {
                continue;
            }

            string filePath = line[3..];
            if (!string.IsNullOrEmpty(filePath))
            {
                modifiedFiles.Add(filePath);
            }
        }

        return new GitStatus(hasStagedChanges, hasUnstagedChanges, modifiedFiles);
    }
}
