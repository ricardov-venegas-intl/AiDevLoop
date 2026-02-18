namespace AiDevLoop.Core.Domain;

/// <summary>
/// The result of executing a single validation command (lint, typecheck, test, build, etc.).
/// </summary>
/// <param name="Name">The name of the command that was run.</param>
/// <param name="ExitCode">The process exit code; <c>0</c> indicates success.</param>
/// <param name="Stdout">The standard output captured from the process.</param>
/// <param name="Stderr">The standard error captured from the process.</param>
public record CommandResult(
    string Name,
    int ExitCode,
    string Stdout,
    string Stderr)
{
    /// <summary>
    /// Gets a value indicating whether the command completed successfully (exit code 0).
    /// </summary>
    public bool Succeeded => ExitCode == 0;
}
