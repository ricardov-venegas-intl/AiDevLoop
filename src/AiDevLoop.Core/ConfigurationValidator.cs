using AiDevLoop.Core.Domain;

namespace AiDevLoop.Core;

/// <summary>
/// Pure functions for validating a <see cref="Configuration"/> object.
/// </summary>
public static class ConfigurationValidator
{
    private static readonly IReadOnlySet<string> ValidLlmProviders =
        new HashSet<string>(StringComparer.OrdinalIgnoreCase) { "claude", "copilot" };

    /// <summary>
    /// Validates a <see cref="Configuration"/> object against expected schema rules.
    /// Accumulates all validation errors rather than short-circuiting on the first failure.
    /// </summary>
    /// <param name="configuration">The configuration to validate.</param>
    /// <returns>
    /// <see cref="Result{TValue,TError}.Ok"/> containing the original configuration when all
    /// rules pass; otherwise <see cref="Result{TValue,TError}.Err"/> containing the full list
    /// of error messages.
    /// </returns>
    public static Result<Configuration, IReadOnlyList<string>> Validate(Configuration configuration)
    {
        var errors = new List<string>();

        ValidateLlmProvider(configuration.Llm, errors);
        ValidateMaxReviewIterations(configuration.Validation.MaxReviewIterations, errors);
        ValidatePaths(configuration.Paths, errors);
        ValidateCommands(configuration.Validation.Commands, errors);

        if (errors.Count > 0)
        {
            return new Result<Configuration, IReadOnlyList<string>>.Err(errors);
        }

        return new Result<Configuration, IReadOnlyList<string>>.Ok(configuration);
    }

    private static void ValidateLlmProvider(string llm, List<string> errors)
    {
        if (!ValidLlmProviders.Contains(llm))
        {
            errors.Add($"LLM provider must be 'claude' or 'copilot', but was '{llm}'.");
        }
    }

    private static void ValidateMaxReviewIterations(int maxReviewIterations, List<string> errors)
    {
        if (maxReviewIterations <= 0)
        {
            errors.Add($"MaxReviewIterations must be greater than 0, but was {maxReviewIterations}.");
        }
    }

    private static void ValidatePaths(PathsConfiguration paths, List<string> errors)
    {
        if (string.IsNullOrWhiteSpace(paths.Docs))
        {
            errors.Add("Paths.Docs must be a non-empty string.");
        }

        if (string.IsNullOrWhiteSpace(paths.Context))
        {
            errors.Add("Paths.Context must be a non-empty string.");
        }

        if (string.IsNullOrWhiteSpace(paths.Prompts))
        {
            errors.Add("Paths.Prompts must be a non-empty string.");
        }
    }

    private static void ValidateCommands(IReadOnlyDictionary<string, string>? commands, List<string> errors)
    {
        if (commands is null)
        {
            return;
        }

        foreach (var (key, value) in commands)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                errors.Add($"Validation command '{key}' must have a non-empty command string.");
            }
        }
    }
}
