namespace AiDevLoop.Shell.Adapters;

using AiDevLoop.Core.Domain;

/// <summary>
/// Provides asynchronous process execution capabilities.
/// </summary>
public interface IProcessRunner
{
    /// <summary>
    /// Runs a command with the specified arguments and returns the result.
    /// </summary>
    /// <param name="command">The command to run (e.g., <c>"dotnet"</c>, <c>"git"</c>).</param>
    /// <param name="arguments">The arguments to pass to the command.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>A <see cref="CommandResult"/> containing the exit code, stdout, and stderr.</returns>
    Task<CommandResult> RunAsync(
        string command,
        string arguments,
        CancellationToken cancellationToken);

    /// <summary>
    /// Runs a command with the specified arguments in an optional working directory.
    /// </summary>
    /// <param name="command">The command to run (e.g., <c>"dotnet"</c>, <c>"git"</c>).</param>
    /// <param name="arguments">The arguments to pass to the command.</param>
    /// <param name="workingDirectory">The working directory for the process. If <see langword="null"/>, the current directory is used.</param>
    /// <param name="verbose">If <see langword="true"/>, additional diagnostic output is produced.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>A <see cref="CommandResult"/> containing the exit code, stdout, and stderr.</returns>
    Task<CommandResult> RunAsync(
        string command,
        string arguments,
        string workingDirectory,
        bool verbose,
        CancellationToken cancellationToken);
}
