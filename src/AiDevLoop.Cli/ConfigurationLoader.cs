using System.IO;
using System.Text.Json;
using System.Collections.ObjectModel;
using AiDevLoop.Core.Domain;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

namespace AiDevLoop.Cli;

/// <summary>
/// Loads <see cref="Configuration"/> from disk and merges command-line overrides.
/// </summary>
public static class ConfigurationLoader
{
    /// <summary>
    /// Load the configuration for the workspace rooted at <paramref name="projectRoot"/>.
    /// Behavior:
    /// - If <paramref name="overrides"/>.ConfigPath is provided, that path is used (relative to
    ///   <paramref name="projectRoot"/> when not rooted).
    /// - Otherwise the loader prefers <c>.aidevloop.json</c> over <c>.aidevloop.yaml</c>.
    /// - If no file is present the <see cref="Configuration.Default"/> is returned.
    /// - Command-line overrides are applied after file loading (only <c>--llm</c> and <c>--verbose</c>). 
    /// </summary>
    /// <param name="projectRoot">Directory to search for config files.</param>
    /// <param name="overrides">Optional CLI overrides (may be <c>null</c>).</param>
    /// <returns>
    /// <see cref="Result{TValue,TError}.Ok"/> with the resolved <see cref="Configuration"/>,
    /// or <see cref="Result{TValue,TError}.Err"/> with an error message when a specified
    /// config file cannot be read/deserialized.
    /// </returns>
    public static Result<Configuration, string> Load(string projectRoot, CommandLineOptions? overrides)
    {
        if (string.IsNullOrWhiteSpace(projectRoot))
            projectRoot = Directory.GetCurrentDirectory();

        // Resolve explicit --config path if provided
        string? explicitPath = overrides?.ConfigPath;
        string? chosenPath = null;

        if (!string.IsNullOrEmpty(explicitPath))
        {
            var candidate = Path.IsPathRooted(explicitPath) ? explicitPath : Path.Combine(projectRoot, explicitPath);
            if (!File.Exists(candidate))
                return new Result<Configuration, string>.Err($"Config file not found: {candidate}");

            chosenPath = candidate;
        }
        else
        {
            var jsonPath = Path.Combine(projectRoot, ".aidevloop.json");
            var yamlPath = Path.Combine(projectRoot, ".aidevloop.yaml");

            if (File.Exists(jsonPath)) chosenPath = jsonPath;
            else if (File.Exists(yamlPath)) chosenPath = yamlPath;
        }

        Configuration config;

        if (chosenPath is null)
        {
            config = Configuration.Default;
        }
        else
        {
            try
            {
                var ext = Path.GetExtension(chosenPath).ToLowerInvariant();
                var raw = File.ReadAllText(chosenPath);

                if (ext == ".json")
                {
                    var opts = new JsonSerializerOptions(JsonSerializerDefaults.Web)
                    {
                        PropertyNameCaseInsensitive = true
                    };

                    var deserialized = JsonSerializer.Deserialize<Configuration>(raw, opts);
                    if (deserialized is null)
                        return new Result<Configuration, string>.Err("Configuration file deserialized to null.");

                    config = deserialized;
                }
                else // yaml / yml
                {
                    // Use an intermediate DTO with concrete collection types so YamlDotNet can
                    // deserialize mappings like `commands: {}` reliably (IReadOnlyDictionary
                    // can be problematic for the deserializer).
                    var deserializer = new DeserializerBuilder()
                        .WithNamingConvention(CamelCaseNamingConvention.Instance)
                        .IgnoreUnmatchedProperties()
                        .Build();

                    // DTOs mirror the public Configuration shape but use concrete types
                    // for nested dictionaries so deserialization succeeds consistently.
                    var dto = deserializer.Deserialize<YamlConfigurationDto>(raw);
                    if (dto is null)
                        return new Result<Configuration, string>.Err("Configuration file deserialized to null (YAML).");

                    var commands = dto.Validation?.Commands ?? new Dictionary<string, string>();

                    config = new Configuration(
                        Llm: dto.Llm ?? Configuration.Default.Llm,
                        Paths: new PathsConfiguration(
                            Docs: dto.Paths?.Docs ?? Configuration.Default.Paths.Docs,
                            Context: dto.Paths?.Context ?? Configuration.Default.Paths.Context,
                            Prompts: dto.Paths?.Prompts ?? Configuration.Default.Paths.Prompts),
                        Validation: new ValidationConfiguration(
                            MaxReviewIterations: dto.Validation?.MaxReviewIterations ?? Configuration.Default.Validation.MaxReviewIterations,
                            Commands: new ReadOnlyDictionary<string, string>(commands)),
                        Verbose: dto.Verbose);
                }
            }
            catch (JsonException jex)
            {
                return new Result<Configuration, string>.Err($"Malformed JSON configuration: {jex.Message}");
            }
            catch (YamlDotNet.Core.YamlException yex)
            {
                return new Result<Configuration, string>.Err($"Malformed YAML configuration: {yex.Message}");
            }
            catch (IOException ioex)
            {
                return new Result<Configuration, string>.Err($"I/O error reading configuration: {ioex.Message}");
            }
        }

        // Apply CLI overrides (only when explicitly provided)
        if (!string.IsNullOrEmpty(overrides?.Llm))
            config = config with { Llm = overrides!.Llm! };

        if (overrides is not null && overrides.Verbose)
            config = config with { Verbose = true };

        return new Result<Configuration, string>.Ok(config);
    }

    // DTOs used only for YAML deserialization to avoid issues mapping to
    // readonly/immutable collection types directly.
    private class YamlConfigurationDto
    {
        public string? Llm { get; set; }
        public YamlPathsDto? Paths { get; set; }
        public YamlValidationDto? Validation { get; set; }
        public bool Verbose { get; set; }
    }

    private class YamlPathsDto
    {
        public string? Docs { get; set; }
        public string? Context { get; set; }
        public string? Prompts { get; set; }
    }

    private class YamlValidationDto
    {
        public int? MaxReviewIterations { get; set; }
        public Dictionary<string, string>? Commands { get; set; }
    }
}
