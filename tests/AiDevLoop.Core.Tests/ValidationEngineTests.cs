using AiDevLoop.Core;
using AiDevLoop.Core.Domain;
using Xunit;

namespace AiDevLoop.Core.Tests;

public sealed class ValidationEngineTests
{
    // ── Empty input ──────────────────────────────────────────────────────────

    [Fact]
    public void Validate_EmptyList_ReturnsAllPassedTrue()
    {
        var result = ValidationEngine.Validate([]);

        Assert.True(result.AllPassed);
    }

    [Fact]
    public void Validate_EmptyList_ReturnsEmptyFailedCommands()
    {
        var result = ValidationEngine.Validate([]);

        Assert.Empty(result.FailedCommands);
    }

    [Fact]
    public void Validate_EmptyList_ReturnsNoCommandsWarning()
    {
        var result = ValidationEngine.Validate([]);

        var warning = Assert.Single(result.Warnings);
        Assert.Equal("No validation commands were provided.", warning);
    }

    // ── All passing ──────────────────────────────────────────────────────────

    [Fact]
    public void Validate_AllSucceeded_ReturnsAllPassedTrue()
    {
        var commands = new[]
        {
            new CommandResult("build", 0, "ok", ""),
            new CommandResult("test",  0, "ok", ""),
        };

        var result = ValidationEngine.Validate(commands);

        Assert.True(result.AllPassed);
    }

    [Fact]
    public void Validate_AllSucceeded_ReturnsEmptyFailedCommands()
    {
        var commands = new[]
        {
            new CommandResult("lint", 0, "clean", ""),
        };

        var result = ValidationEngine.Validate(commands);

        Assert.Empty(result.FailedCommands);
    }

    [Fact]
    public void Validate_AllSucceeded_ReturnsNoWarnings_WhenNoStderr()
    {
        var commands = new[]
        {
            new CommandResult("build", 0, "ok", ""),
            new CommandResult("test",  0, "ok", ""),
        };

        var result = ValidationEngine.Validate(commands);

        Assert.Empty(result.Warnings);
    }

    // ── Failures ─────────────────────────────────────────────────────────────

    [Fact]
    public void Validate_AnyCommandFailed_ReturnsAllPassedFalse()
    {
        var commands = new[]
        {
            new CommandResult("build", 0,  "ok",    ""),
            new CommandResult("test",  1,  "",      "1 test failed"),
        };

        var result = ValidationEngine.Validate(commands);

        Assert.False(result.AllPassed);
    }

    [Fact]
    public void Validate_MultipleFailures_AllCapturedInFailedCommands()
    {
        var commands = new[]
        {
            new CommandResult("lint",  2, "", "2 errors"),
            new CommandResult("build", 1, "", "build error"),
            new CommandResult("test",  0, "ok", ""),
        };

        var result = ValidationEngine.Validate(commands);

        Assert.Equal(2, result.FailedCommands.Count);
        Assert.Contains(result.FailedCommands, r => r.Name == "lint");
        Assert.Contains(result.FailedCommands, r => r.Name == "build");
    }

    [Fact]
    public void Validate_FailedCommand_NotIncludedInFailedCommands_WhenSucceeded()
    {
        var commands = new[]
        {
            new CommandResult("lint",  2, "", "errors"),
            new CommandResult("build", 0, "ok", ""),
        };

        var result = ValidationEngine.Validate(commands);

        Assert.DoesNotContain(result.FailedCommands, r => r.Name == "build");
    }

    // ── Warnings ─────────────────────────────────────────────────────────────

    [Fact]
    public void Validate_PassingCommandWithStderr_ProducesWarning()
    {
        var commands = new[]
        {
            new CommandResult("build", 0, "ok", "warning: deprecated API"),
        };

        var result = ValidationEngine.Validate(commands);

        var warning = Assert.Single(result.Warnings);
        Assert.Equal("warning: deprecated API", warning);
    }

    [Fact]
    public void Validate_FailingCommandWithStderr_DoesNotProduceWarning()
    {
        var commands = new[]
        {
            new CommandResult("test", 1, "", "fatal error"),
        };

        var result = ValidationEngine.Validate(commands);

        Assert.Empty(result.Warnings);
    }

    [Fact]
    public void Validate_MultiplePassingCommandsWithStderr_AllWarningsCaptured()
    {
        var commands = new[]
        {
            new CommandResult("lint",  0, "", "warn A"),
            new CommandResult("build", 0, "", "warn B"),
            new CommandResult("test",  0, "ok", ""),
        };

        var result = ValidationEngine.Validate(commands);

        Assert.Equal(2, result.Warnings.Count);
        Assert.Contains("warn A", result.Warnings);
        Assert.Contains("warn B", result.Warnings);
    }

    // ── Mixed ────────────────────────────────────────────────────────────────

    [Fact]
    public void Validate_MixedResults_CorrectlyClassifiesEachCommand()
    {
        var commands = new[]
        {
            new CommandResult("lint",  0, "",   "deprecation warning"),
            new CommandResult("build", 1, "",   "compile error"),
            new CommandResult("test",  0, "ok", ""),
        };

        var result = ValidationEngine.Validate(commands);

        Assert.False(result.AllPassed);
        var failed = Assert.Single(result.FailedCommands);
        Assert.Equal("build", failed.Name);
        var warning = Assert.Single(result.Warnings);
        Assert.Equal("deprecation warning", warning);
    }
}
