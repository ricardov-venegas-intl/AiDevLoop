using System.IO;
using AiDevLoop.Cli;
using AiDevLoop.Core.Domain;
using Xunit;

namespace AiDevLoop.Cli.Tests;

/// <summary>
/// Unit tests for <see cref="AiDevLoop.Cli.ConfigurationLoader"/>.
/// </summary>
public class ConfigurationLoaderTests
{
    private static string NewTempDir()
    {
        var dir = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName());
        Directory.CreateDirectory(dir);
        return dir;
    }

    /// <summary>
    /// When no configuration file exists the loader returns <see cref="Configuration.Default"/>.
    /// </summary>
    [Fact]
    public void Load_ReturnsDefault_WhenNoConfigPresent()
    {
        var dir = NewTempDir();
        var result = ConfigurationLoader.Load(dir, new CommandLineOptions(null, null, false, false, null));

        Assert.IsType<Result<Configuration, string>.Ok>(result);
        var ok = (Result<Configuration, string>.Ok)result;
        Assert.Equal(Configuration.Default.Llm, ok.Value.Llm);
        Assert.Equal(Configuration.Default.Paths.Docs, ok.Value.Paths.Docs);
        Assert.Equal(Configuration.Default.Validation.MaxReviewIterations, ok.Value.Validation.MaxReviewIterations);
        Assert.False(ok.Value.Verbose);
    }

    /// <summary>
    /// Loads a JSON config file and verifies CLI overrides take precedence.
    /// </summary>
    [Fact]
    public void Load_ReadsJsonFile_AndAppliesOverrides()
    {
        var dir = NewTempDir();
        var json = @"{
  ""Llm"": ""claude"",
  ""Paths"": { ""Docs"": ""docs"", ""Context"": ""context"", ""Prompts"": ""prompts"" },
  ""Validation"": { ""MaxReviewIterations"": 3, ""Commands"": {} },
  ""Verbose"": false
}";
        File.WriteAllText(Path.Combine(dir, ".aidevloop.json"), json);

        var result1 = ConfigurationLoader.Load(dir, new CommandLineOptions(null, null, false, false, null));
        Assert.IsType<Result<Configuration, string>.Ok>(result1);
        var cfg1 = ((Result<Configuration, string>.Ok)result1).Value;
        Assert.Equal("claude", cfg1.Llm);

        // CLI override should take precedence
        var result2 = ConfigurationLoader.Load(dir, new CommandLineOptions("copilot", null, false, false, null));
        var cfg2 = ((Result<Configuration, string>.Ok)result2).Value;
        Assert.Equal("copilot", cfg2.Llm);
    }

    /// <summary>
    /// Malformed JSON configuration returns an <c>Err</c> result with a helpful message.
    /// </summary>
    [Fact]
    public void Load_ReturnsError_ForMalformedJson()
    {
        var dir = NewTempDir();
        File.WriteAllText(Path.Combine(dir, ".aidevloop.json"), "{ not-json }");

        var result = ConfigurationLoader.Load(dir, new CommandLineOptions(null, null, false, false, null));
        Assert.IsType<Result<Configuration, string>.Err>(result);
        var err = (Result<Configuration, string>.Err)result;
        Assert.Contains("Malformed JSON", err.Error);
    }

    /// <summary>
    /// Loads a YAML configuration file successfully.
    /// </summary>
    [Fact]
    public void Load_ReadsYamlFile()
    {
        var dir = NewTempDir();
        var yaml = @"llm: copilot
paths:
  docs: docs
  context: context
  prompts: prompts
validation:
  maxReviewIterations: 2
  commands: {}
verbose: true
";
        File.WriteAllText(Path.Combine(dir, ".aidevloop.yaml"), yaml);

        var result = ConfigurationLoader.Load(dir, new CommandLineOptions(null, null, false, false, null));
        if (result is Result<Configuration, string>.Err e)
            throw new Xunit.Sdk.XunitException($"ConfigurationLoader.Load returned Err: {e.Error}");

        Assert.IsType<Result<Configuration, string>.Ok>(result);
        var cfg = ((Result<Configuration, string>.Ok)result).Value;
        Assert.Equal("copilot", cfg.Llm);
        Assert.True(cfg.Verbose);
        Assert.Equal(2, cfg.Validation.MaxReviewIterations);
    }

    /// <summary>
    /// Honors an explicit <c>--config</c> path (relative to project root).
    /// </summary>
    [Fact]
    public void Load_UsesExplicitConfigPath_WhenProvided()
    {
        var dir = NewTempDir();
        var alt = Path.Combine(dir, "myconfig.json");
        File.WriteAllText(alt, "{ \"Llm\": \"copilot\", \"Paths\": { \"Docs\": \"docs\", \"Context\": \"context\", \"Prompts\": \"prompts\" }, \"Validation\": { \"MaxReviewIterations\": 1, \"Commands\": {} }, \"Verbose\": false }");

        var result = ConfigurationLoader.Load(dir, new CommandLineOptions(null, "myconfig.json", false, false, null));
        Assert.IsType<Result<Configuration, string>.Ok>(result);
        var cfg = ((Result<Configuration, string>.Ok)result).Value;
        Assert.Equal("copilot", cfg.Llm);
        Assert.Equal(1, cfg.Validation.MaxReviewIterations);
    }
}
