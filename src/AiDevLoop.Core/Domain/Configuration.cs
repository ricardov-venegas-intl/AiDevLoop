using System.Collections.ObjectModel;

namespace AiDevLoop.Core.Domain;

/// <summary>
/// The root configuration record for the AiDevLoop tool, loaded from
/// <c>.aidevloop.json</c> or <c>.aidevloop.yaml</c> in the project directory.
/// </summary>
/// <param name="Llm">
/// The LLM provider to use: <c>"claude"</c> or <c>"copilot"</c>. Defaults to <c>"claude"</c>.
/// </param>
/// <param name="Paths">Directory path configuration.</param>
/// <param name="Validation">Validation command and iteration configuration.</param>
/// <param name="Verbose">
/// When <see langword="true"/>, enables verbose output. Defaults to <see langword="false"/>.
/// </param>
public record Configuration(
    string Llm,
    PathsConfiguration Paths,
    ValidationConfiguration Validation,
    bool Verbose)
{
    /// <summary>
    /// Gets the default configuration matching the FR-3.2 specification:
    /// LLM provider <c>"claude"</c>, standard directory paths, maximum 3 review iterations,
    /// and verbose output disabled.
    /// </summary>
    public static Configuration Default { get; } = new Configuration(
        Llm: "claude",
        Paths: new PathsConfiguration(
            Docs: "docs",
            Context: "context",
            Prompts: "prompts"),
        Validation: new ValidationConfiguration(
            MaxReviewIterations: 3,
            Commands: new ReadOnlyDictionary<string, string>(new Dictionary<string, string>())),
        Verbose: false);
}
