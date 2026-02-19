using AiDevLoop.Cli;
using AiDevLoop.Core.Domain;
using Xunit;

namespace AiDevLoop.Shell.Tests;

/// <summary>
/// Unit tests for <see cref="CommandLineParser"/>.
/// </summary>
public sealed class CommandLineParserTests
{
    // -------------------------------------------------------------------------
    // Command parsing
    // -------------------------------------------------------------------------

    /// <summary>Verifies that <c>run</c> alone produces <see cref="Command.Run"/> with no task ID.</summary>
    [Fact]
    public void Parse_RunWithNoExtraArgs_ReturnsRunCommandWithNoTaskId()
    {
        var result = CommandLineParser.Parse(["run"]);

        var ok = Assert.IsType<Result<CommandLineArgs, string>.Ok>(result);
        Assert.Equal(Command.Run, ok.Value.Command);
        Assert.Null(ok.Value.TaskId);
    }

    /// <summary>Verifies that <c>run TASK-005</c> produces <see cref="Command.Run"/> with the specified task ID.</summary>
    [Fact]
    public void Parse_RunWithTaskId_ReturnsRunCommandWithTaskId()
    {
        var result = CommandLineParser.Parse(["run", "TASK-005"]);

        var ok = Assert.IsType<Result<CommandLineArgs, string>.Ok>(result);
        Assert.Equal(Command.Run, ok.Value.Command);
        Assert.Equal(new TaskId("TASK-005"), ok.Value.TaskId);
    }

    /// <summary>Verifies that <c>resume</c> produces <see cref="Command.Resume"/> with no task ID.</summary>
    [Fact]
    public void Parse_Resume_ReturnsResumeCommandWithNoTaskId()
    {
        var result = CommandLineParser.Parse(["resume"]);

        var ok = Assert.IsType<Result<CommandLineArgs, string>.Ok>(result);
        Assert.Equal(Command.Resume, ok.Value.Command);
        Assert.Null(ok.Value.TaskId);
    }

    /// <summary>Verifies that command names are matched case-insensitively.</summary>
    [Fact]
    public void Parse_CommandIsCaseInsensitive()
    {
        var result = CommandLineParser.Parse(["RUN"]);

        var ok = Assert.IsType<Result<CommandLineArgs, string>.Ok>(result);
        Assert.Equal(Command.Run, ok.Value.Command);
    }

    // -------------------------------------------------------------------------
    // --from-step flag
    // -------------------------------------------------------------------------

    /// <summary>Verifies that <c>resume --from-step=4</c> sets <see cref="CommandLineOptions.FromStep"/> to 4.</summary>
    [Fact]
    public void Parse_ResumeWithFromStep_SetsFromStep()
    {
        var result = CommandLineParser.Parse(["resume", "--from-step=4"]);

        var ok = Assert.IsType<Result<CommandLineArgs, string>.Ok>(result);
        Assert.Equal(Command.Resume, ok.Value.Command);
        Assert.Equal(4, ok.Value.Options.FromStep);
    }

    /// <summary>Verifies that <c>--from-step=1</c> (minimum valid value) is accepted.</summary>
    [Fact]
    public void Parse_ResumeWithFromStepOne_Succeeds()
    {
        var result = CommandLineParser.Parse(["resume", "--from-step=1"]);

        var ok = Assert.IsType<Result<CommandLineArgs, string>.Ok>(result);
        Assert.Equal(1, ok.Value.Options.FromStep);
    }

    /// <summary>Verifies that using <c>--from-step</c> with the <c>run</c> command returns an error.</summary>
    [Fact]
    public void Parse_RunWithFromStep_ReturnsError()
    {
        var result = CommandLineParser.Parse(["run", "--from-step=2"]);

        var err = Assert.IsType<Result<CommandLineArgs, string>.Err>(result);
        Assert.Contains("--from-step", err.Error, StringComparison.OrdinalIgnoreCase);
        Assert.Contains("resume", err.Error, StringComparison.OrdinalIgnoreCase);
    }

    /// <summary>Verifies that <c>--from-step=0</c> (zero) returns an error.</summary>
    [Fact]
    public void Parse_FromStepWithZeroValue_ReturnsError()
    {
        var result = CommandLineParser.Parse(["resume", "--from-step=0"]);

        var err = Assert.IsType<Result<CommandLineArgs, string>.Err>(result);
        Assert.Contains("--from-step", err.Error, StringComparison.OrdinalIgnoreCase);
    }

    /// <summary>Verifies that a negative value for <c>--from-step</c> returns an error.</summary>
    [Fact]
    public void Parse_FromStepWithNegativeValue_ReturnsError()
    {
        var result = CommandLineParser.Parse(["resume", "--from-step=-1"]);

        var err = Assert.IsType<Result<CommandLineArgs, string>.Err>(result);
        Assert.Contains("--from-step", err.Error, StringComparison.OrdinalIgnoreCase);
    }

    /// <summary>Verifies that a non-numeric value for <c>--from-step</c> returns an error.</summary>
    [Fact]
    public void Parse_FromStepWithNonNumericValue_ReturnsError()
    {
        var result = CommandLineParser.Parse(["resume", "--from-step=abc"]);

        var err = Assert.IsType<Result<CommandLineArgs, string>.Err>(result);
        Assert.Contains("--from-step", err.Error, StringComparison.OrdinalIgnoreCase);
    }

    // -------------------------------------------------------------------------
    // --llm flag
    // -------------------------------------------------------------------------

    /// <summary>Verifies that <c>--llm=copilot</c> sets the LLM provider override to <c>"copilot"</c>.</summary>
    [Fact]
    public void Parse_LlmFlag_SetsLlmProvider()
    {
        var result = CommandLineParser.Parse(["run", "--llm=copilot"]);

        var ok = Assert.IsType<Result<CommandLineArgs, string>.Ok>(result);
        Assert.Equal("copilot", ok.Value.Options.Llm);
    }

    /// <summary>Verifies that <c>--llm=claude</c> sets the LLM provider override to <c>"claude"</c>.</summary>
    [Fact]
    public void Parse_LlmFlagWithClaude_SetsLlmProvider()
    {
        var result = CommandLineParser.Parse(["run", "--llm=claude"]);

        var ok = Assert.IsType<Result<CommandLineArgs, string>.Ok>(result);
        Assert.Equal("claude", ok.Value.Options.Llm);
    }

    // -------------------------------------------------------------------------
    // --config flag
    // -------------------------------------------------------------------------

    /// <summary>Verifies that <c>--config=myconfig.json</c> sets the config path override.</summary>
    [Fact]
    public void Parse_ConfigFlag_SetsConfigPath()
    {
        var result = CommandLineParser.Parse(["run", "--config=myconfig.json"]);

        var ok = Assert.IsType<Result<CommandLineArgs, string>.Ok>(result);
        Assert.Equal("myconfig.json", ok.Value.Options.ConfigPath);
    }

    // -------------------------------------------------------------------------
    // --verbose and --quiet flags
    // -------------------------------------------------------------------------

    /// <summary>Verifies that <c>--verbose</c> sets <see cref="CommandLineOptions.Verbose"/> to <see langword="true"/>.</summary>
    [Fact]
    public void Parse_VerboseFlag_SetsVerboseTrue()
    {
        var result = CommandLineParser.Parse(["run", "--verbose"]);

        var ok = Assert.IsType<Result<CommandLineArgs, string>.Ok>(result);
        Assert.True(ok.Value.Options.Verbose);
        Assert.False(ok.Value.Options.Quiet);
    }

    /// <summary>Verifies that <c>--quiet</c> sets <see cref="CommandLineOptions.Quiet"/> to <see langword="true"/>.</summary>
    [Fact]
    public void Parse_QuietFlag_SetsQuietTrue()
    {
        var result = CommandLineParser.Parse(["run", "--quiet"]);

        var ok = Assert.IsType<Result<CommandLineArgs, string>.Ok>(result);
        Assert.False(ok.Value.Options.Verbose);
        Assert.True(ok.Value.Options.Quiet);
    }

    /// <summary>Verifies that combining <c>--verbose</c> and <c>--quiet</c> returns an error.</summary>
    [Fact]
    public void Parse_VerboseAndQuietTogether_ReturnsError()
    {
        var result = CommandLineParser.Parse(["run", "--verbose", "--quiet"]);

        var err = Assert.IsType<Result<CommandLineArgs, string>.Err>(result);
        Assert.Contains("--verbose", err.Error, StringComparison.OrdinalIgnoreCase);
        Assert.Contains("--quiet", err.Error, StringComparison.OrdinalIgnoreCase);
    }

    /// <summary>Verifies that the error is returned regardless of flag order when both <c>--quiet</c> and <c>--verbose</c> are present.</summary>
    [Fact]
    public void Parse_QuietAndVerboseOrdering_ReturnsError()
    {
        var result = CommandLineParser.Parse(["run", "--quiet", "--verbose"]);

        Assert.IsType<Result<CommandLineArgs, string>.Err>(result);
    }

    // -------------------------------------------------------------------------
    // Defaults when flags are absent
    // -------------------------------------------------------------------------

    /// <summary>Verifies that all option properties default to <see langword="null"/> or <see langword="false"/> when no flags are provided.</summary>
    [Fact]
    public void Parse_NoFlags_HasDefaultOptionValues()
    {
        var result = CommandLineParser.Parse(["run"]);

        var ok = Assert.IsType<Result<CommandLineArgs, string>.Ok>(result);
        Assert.Null(ok.Value.Options.Llm);
        Assert.Null(ok.Value.Options.ConfigPath);
        Assert.Null(ok.Value.Options.FromStep);
        Assert.False(ok.Value.Options.Verbose);
        Assert.False(ok.Value.Options.Quiet);
    }

    // -------------------------------------------------------------------------
    // Unknown flags
    // -------------------------------------------------------------------------

    /// <summary>Verifies that an unrecognised flag returns an error naming the flag.</summary>
    [Fact]
    public void Parse_UnknownFlag_ReturnsError()
    {
        var result = CommandLineParser.Parse(["run", "--unknown"]);

        var err = Assert.IsType<Result<CommandLineArgs, string>.Err>(result);
        Assert.Contains("--unknown", err.Error, StringComparison.OrdinalIgnoreCase);
    }

    /// <summary>Verifies that an unrecognised flag with a value returns an error naming the full flag.</summary>
    [Fact]
    public void Parse_UnknownFlagWithValue_ReturnsError()
    {
        var result = CommandLineParser.Parse(["run", "--foo=bar"]);

        var err = Assert.IsType<Result<CommandLineArgs, string>.Err>(result);
        Assert.Contains("--foo=bar", err.Error, StringComparison.OrdinalIgnoreCase);
    }

    // -------------------------------------------------------------------------
    // Empty / missing command
    // -------------------------------------------------------------------------

    /// <summary>Verifies that an empty argument array returns an error.</summary>
    [Fact]
    public void Parse_EmptyArgs_ReturnsError()
    {
        var result = CommandLineParser.Parse([]);

        var err = Assert.IsType<Result<CommandLineArgs, string>.Err>(result);
        Assert.NotEmpty(err.Error);
    }

    /// <summary>Verifies that an unknown command name returns an error mentioning the supplied token.</summary>
    [Fact]
    public void Parse_UnknownCommand_ReturnsError()
    {
        var result = CommandLineParser.Parse(["build"]);

        var err = Assert.IsType<Result<CommandLineArgs, string>.Err>(result);
        Assert.Contains("build", err.Error, StringComparison.OrdinalIgnoreCase);
    }

    // -------------------------------------------------------------------------
    // Combined flag scenarios
    // -------------------------------------------------------------------------

    /// <summary>Verifies that both a positional task ID and a flag are parsed correctly in the same invocation.</summary>
    [Fact]
    public void Parse_RunWithTaskIdAndLlmFlag_ParsesBoth()
    {
        var result = CommandLineParser.Parse(["run", "TASK-001", "--llm=copilot"]);

        var ok = Assert.IsType<Result<CommandLineArgs, string>.Ok>(result);
        Assert.Equal(Command.Run, ok.Value.Command);
        Assert.Equal(new TaskId("TASK-001"), ok.Value.TaskId);
        Assert.Equal("copilot", ok.Value.Options.Llm);
    }

    /// <summary>Verifies that multiple flags for <c>resume</c> are all parsed correctly.</summary>
    [Fact]
    public void Parse_ResumeWithMultipleFlags_ParsesAll()
    {
        var result = CommandLineParser.Parse(["resume", "--from-step=3", "--llm=claude", "--config=custom.json"]);

        var ok = Assert.IsType<Result<CommandLineArgs, string>.Ok>(result);
        Assert.Equal(Command.Resume, ok.Value.Command);
        Assert.Null(ok.Value.TaskId);
        Assert.Equal(3, ok.Value.Options.FromStep);
        Assert.Equal("claude", ok.Value.Options.Llm);
        Assert.Equal("custom.json", ok.Value.Options.ConfigPath);
    }

    /// <summary>Verifies that flags before the positional task ID position do not accidentally capture the task ID.</summary>
    [Fact]
    public void Parse_RunWithOnlyFlags_NoTaskId()
    {
        var result = CommandLineParser.Parse(["run", "--verbose", "--llm=copilot"]);

        var ok = Assert.IsType<Result<CommandLineArgs, string>.Ok>(result);
        Assert.Null(ok.Value.TaskId);
        Assert.True(ok.Value.Options.Verbose);
        Assert.Equal("copilot", ok.Value.Options.Llm);
    }
}
