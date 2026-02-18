namespace AiDevLoop.Core.Domain;

/// <summary>
/// Optional flags and overrides provided on the command line, supplementing file-based configuration.
/// </summary>
/// <param name="Llm">
/// The LLM provider override (<c>"claude"</c> or <c>"copilot"</c>).
/// When <see langword="null"/>, the value from the config file is used.
/// </param>
/// <param name="ConfigPath">
/// The path to an alternate configuration file.
/// When <see langword="null"/>, the default <c>.aidevloop.json</c> / <c>.aidevloop.yaml</c> is used.
/// </param>
/// <param name="Verbose">
/// When <see langword="true"/>, enables verbose output including full LLM prompts and responses.
/// </param>
/// <param name="Quiet">
/// When <see langword="true"/>, suppresses step-progress output (errors and confirmations are still shown).
/// </param>
/// <param name="FromStep">
/// For the <c>resume</c> command: the one-based step number to resume from.
/// When <see langword="null"/>, the tool resumes from the persisted step.
/// </param>
public record CommandLineOptions(
    string? Llm,
    string? ConfigPath,
    bool Verbose,
    bool Quiet,
    int? FromStep);
