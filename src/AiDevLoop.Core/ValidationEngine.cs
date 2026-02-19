using AiDevLoop.Core.Domain;

namespace AiDevLoop.Core;

/// <summary>
/// Analyzes a list of <see cref="CommandResult"/> objects and produces a <see cref="ValidationResult"/>.
/// </summary>
public static class ValidationEngine
{
    private const string NoCommandsWarning = "No validation commands were provided.";

    /// <summary>
    /// Validates a set of command results, collecting failures and warnings.
    /// </summary>
    /// <param name="commandResults">
    /// The list of command results to analyze. May be empty but must not be <see langword="null"/>.
    /// </param>
    /// <returns>
    /// A <see cref="ValidationResult"/> where <see cref="ValidationResult.AllPassed"/> is
    /// <see langword="true"/> only when every command exited with code <c>0</c>.
    /// An empty input list returns <see cref="ValidationResult.AllPassed"/> as <see langword="true"/>
    /// with a warning indicating no commands were provided.
    /// </returns>
    public static ValidationResult Validate(IReadOnlyList<CommandResult> commandResults)
    {
        ArgumentNullException.ThrowIfNull(commandResults);

        if (commandResults.Count == 0)
        {
            return new ValidationResult(
                AllPassed: true,
                FailedCommands: [],
                Warnings: [NoCommandsWarning]);
        }

        var failed = commandResults
            .Where(r => !r.Succeeded)
            .ToArray();

        var warnings = commandResults
            .Where(r => r.Succeeded && !string.IsNullOrEmpty(r.Stderr))
            .Select(r => r.Stderr)
            .ToArray();

        return new ValidationResult(
            AllPassed: failed.Length == 0,
            FailedCommands: failed,
            Warnings: warnings);
    }
}
