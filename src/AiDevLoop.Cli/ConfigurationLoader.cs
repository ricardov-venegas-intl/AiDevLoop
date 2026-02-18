using System.IO;
using System.Text.Json;
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
                    var deserializer = new DeserializerBuilder()
                        .WithNamingConvention(CamelCaseNamingConvention.Instance)
                        .IgnoreUnmatchedProperties()
                        .Build();

                    var deserialized = deserializer.Deserialize<Configuration>(raw);
                    if (deserialized is null)
                        return new Result<Configuration, string>.Err("Configuration file deserialized to null (YAML).");

                    config = deserialized;
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
}
