namespace AiDevLoop.Core.Domain;

/// <summary>
/// The parsed arguments supplied to the CLI entry point.
/// </summary>
/// <param name="Command">The top-level command to execute (<c>run</c> or <c>resume</c>).</param>
/// <param name="TaskId">
/// An optional explicit task identifier. When <see langword="null"/>, the tool auto-selects
/// the next pending task.
/// </param>
/// <param name="Options">Additional command-line flags and overrides.</param>
public record CommandLineArgs(
    Command Command,
    TaskId? TaskId,
    CommandLineOptions Options);
