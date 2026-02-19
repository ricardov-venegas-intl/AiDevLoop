using System.Collections.ObjectModel;
using AiDevLoop.Core.Domain;
using Xunit;

namespace AiDevLoop.Core.Tests;

/// <summary>
/// Unit tests for <see cref="ConfigurationValidator"/>.
/// </summary>
public class ConfigurationValidatorTests
{
    private static Configuration ValidConfig() => Configuration.Default;

    private static Configuration WithLlm(string llm) =>
        ValidConfig() with { Llm = llm };

    private static Configuration WithMaxReviewIterations(int value) =>
        ValidConfig() with
        {
            Validation = ValidConfig().Validation with { MaxReviewIterations = value }
        };

    private static Configuration WithPaths(string docs, string context, string prompts) =>
        ValidConfig() with
        {
            Paths = new PathsConfiguration(docs, context, prompts)
        };

    private static Configuration WithCommands(Dictionary<string, string> commands) =>
        ValidConfig() with
        {
            Validation = ValidConfig().Validation with
            {
                Commands = new ReadOnlyDictionary<string, string>(commands)
            }
        };

    /// <summary>
    /// The default <see cref="Configuration.Default"/> is valid and returns <c>Ok</c>.
    /// </summary>
    [Fact]
    public void Validate_ReturnsOk_ForValidConfiguration()
    {
        var result = ConfigurationValidator.Validate(ValidConfig());

        var ok = Assert.IsType<Result<Configuration, IReadOnlyList<string>>.Ok>(result);
        Assert.Equal(ValidConfig(), ok.Value);
    }

    /// <summary>
    /// LLM provider <c>"gpt"</c> is not supported and should return an error.
    /// </summary>
    [Theory]
    [InlineData("gpt")]
    [InlineData("openai")]
    [InlineData("")]
    [InlineData("   ")]
    public void Validate_ReturnsErr_ForUnsupportedLlmProvider(string provider)
    {
        var result = ConfigurationValidator.Validate(WithLlm(provider));

        var err = Assert.IsType<Result<Configuration, IReadOnlyList<string>>.Err>(result);
        Assert.Single(err.Error);
        Assert.Contains("claude", err.Error[0]);
        Assert.Contains("copilot", err.Error[0]);
    }

    /// <summary>
    /// LLM provider matching is case-insensitive.
    /// </summary>
    [Theory]
    [InlineData("Claude")]
    [InlineData("CLAUDE")]
    [InlineData("Copilot")]
    [InlineData("COPILOT")]
    public void Validate_ReturnsOk_ForValidLlmProviderCaseInsensitive(string provider)
    {
        var result = ConfigurationValidator.Validate(WithLlm(provider));

        Assert.IsType<Result<Configuration, IReadOnlyList<string>>.Ok>(result);
    }

    /// <summary>
    /// <see cref="ValidationConfiguration.MaxReviewIterations"/> of zero returns an error.
    /// </summary>
    [Fact]
    public void Validate_ReturnsErr_ForMaxReviewIterationsZero()
    {
        var result = ConfigurationValidator.Validate(WithMaxReviewIterations(0));

        var err = Assert.IsType<Result<Configuration, IReadOnlyList<string>>.Err>(result);
        Assert.Single(err.Error);
        Assert.Contains("MaxReviewIterations", err.Error[0]);
    }

    /// <summary>
    /// <see cref="ValidationConfiguration.MaxReviewIterations"/> of negative value returns an error.
    /// </summary>
    [Fact]
    public void Validate_ReturnsErr_ForMaxReviewIterationsNegative()
    {
        var result = ConfigurationValidator.Validate(WithMaxReviewIterations(-5));

        var err = Assert.IsType<Result<Configuration, IReadOnlyList<string>>.Err>(result);
        Assert.Single(err.Error);
        Assert.Contains("MaxReviewIterations", err.Error[0]);
    }

    /// <summary>
    /// <see cref="ValidationConfiguration.MaxReviewIterations"/> of 1 is valid.
    /// </summary>
    [Fact]
    public void Validate_ReturnsOk_ForMaxReviewIterationsOne()
    {
        var result = ConfigurationValidator.Validate(WithMaxReviewIterations(1));

        Assert.IsType<Result<Configuration, IReadOnlyList<string>>.Ok>(result);
    }

    /// <summary>
    /// Empty <see cref="PathsConfiguration.Docs"/> returns an error.
    /// </summary>
    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public void Validate_ReturnsErr_ForEmptyDocsPath(string docs)
    {
        var result = ConfigurationValidator.Validate(WithPaths(docs, "context", "prompts"));

        var err = Assert.IsType<Result<Configuration, IReadOnlyList<string>>.Err>(result);
        Assert.Contains(err.Error, e => e.Contains("Paths.Docs"));
    }

    /// <summary>
    /// Empty <see cref="PathsConfiguration.Context"/> returns an error.
    /// </summary>
    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public void Validate_ReturnsErr_ForEmptyContextPath(string context)
    {
        var result = ConfigurationValidator.Validate(WithPaths("docs", context, "prompts"));

        var err = Assert.IsType<Result<Configuration, IReadOnlyList<string>>.Err>(result);
        Assert.Contains(err.Error, e => e.Contains("Paths.Context"));
    }

    /// <summary>
    /// Empty <see cref="PathsConfiguration.Prompts"/> returns an error.
    /// </summary>
    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public void Validate_ReturnsErr_ForEmptyPromptsPath(string prompts)
    {
        var result = ConfigurationValidator.Validate(WithPaths("docs", "context", prompts));

        var err = Assert.IsType<Result<Configuration, IReadOnlyList<string>>.Err>(result);
        Assert.Contains(err.Error, e => e.Contains("Paths.Prompts"));
    }

    /// <summary>
    /// An empty <see cref="ValidationConfiguration.Commands"/> dictionary is valid.
    /// </summary>
    [Fact]
    public void Validate_ReturnsOk_ForEmptyCommandsDictionary()
    {
        var result = ConfigurationValidator.Validate(WithCommands(new Dictionary<string, string>()));

        Assert.IsType<Result<Configuration, IReadOnlyList<string>>.Ok>(result);
    }

    /// <summary>
    /// A command entry with a non-empty value is valid.
    /// </summary>
    [Fact]
    public void Validate_ReturnsOk_ForNonEmptyCommandValue()
    {
        var result = ConfigurationValidator.Validate(
            WithCommands(new Dictionary<string, string> { { "build", "dotnet build" } }));

        Assert.IsType<Result<Configuration, IReadOnlyList<string>>.Ok>(result);
    }

    /// <summary>
    /// A command entry with an empty string value produces an error.
    /// </summary>
    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public void Validate_ReturnsErr_ForEmptyCommandValue(string commandValue)
    {
        var result = ConfigurationValidator.Validate(
            WithCommands(new Dictionary<string, string> { { "build", commandValue } }));

        var err = Assert.IsType<Result<Configuration, IReadOnlyList<string>>.Err>(result);
        Assert.Contains(err.Error, e => e.Contains("build"));
    }

    /// <summary>
    /// Multiple validation failures are accumulated into a single <c>Err</c> result.
    /// </summary>
    [Fact]
    public void Validate_AccumulatesMultipleErrors()
    {
        var config = new Configuration(
            Llm: "gpt",
            Paths: new PathsConfiguration(Docs: "", Context: "", Prompts: ""),
            Validation: new ValidationConfiguration(
                MaxReviewIterations: 0,
                Commands: new ReadOnlyDictionary<string, string>(new Dictionary<string, string>())),
            Verbose: false);

        var result = ConfigurationValidator.Validate(config);

        var err = Assert.IsType<Result<Configuration, IReadOnlyList<string>>.Err>(result);
        // Expecting 5 errors: LLM + MaxReviewIterations + 3 paths
        Assert.Equal(5, err.Error.Count);
    }
}
