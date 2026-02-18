namespace AiDevLoop.Core.Domain;

/// <summary>
/// A summary of all validation command results for a single validation run.
/// </summary>
/// <param name="AllPassed">
/// <see langword="true"/> when every validation command exited with code <c>0</c>.
/// </param>
/// <param name="FailedCommands">
/// The subset of commands that exited with a non-zero exit code.
/// </param>
/// <param name="Warnings">
/// Non-fatal warnings collected during the validation run.
/// </param>
public record ValidationResult(
    bool AllPassed,
    IReadOnlyList<CommandResult> FailedCommands,
    IReadOnlyList<string> Warnings);
