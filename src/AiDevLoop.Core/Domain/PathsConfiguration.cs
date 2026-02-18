namespace AiDevLoop.Core.Domain;

/// <summary>
/// Configures the directory paths used by the tool for documentation, context, and prompt files.
/// </summary>
/// <param name="Docs">The relative path to the documentation directory (default: <c>"docs"</c>).</param>
/// <param name="Context">The relative path to the context directory (default: <c>"context"</c>).</param>
/// <param name="Prompts">The relative path to the prompts directory (default: <c>"prompts"</c>).</param>
public record PathsConfiguration(
    string Docs,
    string Context,
    string Prompts);
