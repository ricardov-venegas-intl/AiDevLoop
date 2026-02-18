namespace AiDevLoop.Core.Domain;

/// <summary>
/// Configures the validation step of the development loop, including the maximum number of
/// review iterations and the commands to run for validation.
/// </summary>
/// <param name="MaxReviewIterations">
/// The maximum number of review-fix cycles before the tool aborts. Defaults to <c>3</c>.
/// </param>
/// <param name="Commands">
/// A mapping of validation command names (e.g., <c>"build"</c>, <c>"test"</c>) to their
/// shell commands (e.g., <c>"dotnet build"</c>). An empty dictionary disables all validation commands.
/// </param>
public record ValidationConfiguration(
    int MaxReviewIterations,
    IReadOnlyDictionary<string, string> Commands);
