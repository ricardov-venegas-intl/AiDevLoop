using System.Collections.Generic;
using System.IO;

using AiDevLoop.Core.Domain;
using AiDevLoop.Shell.Adapters;

using Xunit;

namespace AiDevLoop.Shell.Tests;

/// <summary>
/// Unit tests for <see cref="ConsoleIO"/> covering all output modes and interaction methods.
/// </summary>
public class ConsoleIOTests
{
    private static ConsoleIO Create(OutputMode mode, StringWriter output, StringReader input) =>
        new(mode, output, input);

    // ── WriteStep ──────────────────────────────────────────────────────────────

    [Fact]
    public void WriteStep_Normal_WritesMessage()
    {
        using StringWriter output = new();
        using StringReader input = new(string.Empty);
        ConsoleIO sut = Create(OutputMode.Normal, output, input);

        sut.WriteStep("step message");

        Assert.Contains("step message", output.ToString());
    }

    [Fact]
    public void WriteStep_Verbose_WritesMessage()
    {
        using StringWriter output = new();
        using StringReader input = new(string.Empty);
        ConsoleIO sut = Create(OutputMode.Verbose, output, input);

        sut.WriteStep("step message");

        Assert.Contains("step message", output.ToString());
    }

    [Fact]
    public void WriteStep_Quiet_WritesNothing()
    {
        using StringWriter output = new();
        using StringReader input = new(string.Empty);
        ConsoleIO sut = Create(OutputMode.Quiet, output, input);

        sut.WriteStep("step message");

        Assert.Equal(string.Empty, output.ToString());
    }

    // ── WriteError ─────────────────────────────────────────────────────────────

    [Fact]
    public void WriteError_Normal_WritesMessage()
    {
        using StringWriter output = new();
        using StringReader input = new(string.Empty);
        ConsoleIO sut = Create(OutputMode.Normal, output, input);

        sut.WriteError("error message");

        Assert.Contains("error message", output.ToString());
    }

    [Fact]
    public void WriteError_Verbose_WritesMessage()
    {
        using StringWriter output = new();
        using StringReader input = new(string.Empty);
        ConsoleIO sut = Create(OutputMode.Verbose, output, input);

        sut.WriteError("error message");

        Assert.Contains("error message", output.ToString());
    }

    [Fact]
    public void WriteError_Quiet_WritesMessage()
    {
        using StringWriter output = new();
        using StringReader input = new(string.Empty);
        ConsoleIO sut = Create(OutputMode.Quiet, output, input);

        sut.WriteError("error message");

        Assert.Contains("error message", output.ToString());
    }

    // ── WriteWarning ───────────────────────────────────────────────────────────

    [Fact]
    public void WriteWarning_Normal_WritesMessage()
    {
        using StringWriter output = new();
        using StringReader input = new(string.Empty);
        ConsoleIO sut = Create(OutputMode.Normal, output, input);

        sut.WriteWarning("warning message");

        Assert.Contains("warning message", output.ToString());
    }

    [Fact]
    public void WriteWarning_Verbose_WritesMessage()
    {
        using StringWriter output = new();
        using StringReader input = new(string.Empty);
        ConsoleIO sut = Create(OutputMode.Verbose, output, input);

        sut.WriteWarning("warning message");

        Assert.Contains("warning message", output.ToString());
    }

    [Fact]
    public void WriteWarning_Quiet_WritesMessage()
    {
        using StringWriter output = new();
        using StringReader input = new(string.Empty);
        ConsoleIO sut = Create(OutputMode.Quiet, output, input);

        sut.WriteWarning("warning message");

        Assert.Contains("warning message", output.ToString());
    }

    // ── WriteVerbose ───────────────────────────────────────────────────────────

    [Fact]
    public void WriteVerbose_Verbose_WritesMessageWithPrefix()
    {
        using StringWriter output = new();
        using StringReader input = new(string.Empty);
        ConsoleIO sut = Create(OutputMode.Verbose, output, input);

        sut.WriteVerbose("verbose message");

        Assert.Contains("verbose message", output.ToString());
    }

    [Fact]
    public void WriteVerbose_Normal_WritesNothing()
    {
        using StringWriter output = new();
        using StringReader input = new(string.Empty);
        ConsoleIO sut = Create(OutputMode.Normal, output, input);

        sut.WriteVerbose("verbose message");

        Assert.Equal(string.Empty, output.ToString());
    }

    [Fact]
    public void WriteVerbose_Quiet_WritesNothing()
    {
        using StringWriter output = new();
        using StringReader input = new(string.Empty);
        ConsoleIO sut = Create(OutputMode.Quiet, output, input);

        sut.WriteVerbose("verbose message");

        Assert.Equal(string.Empty, output.ToString());
    }

    [Fact]
    public void WriteVerbose_Verbose_IncludesVerbosePrefix()
    {
        using StringWriter output = new();
        using StringReader input = new(string.Empty);
        ConsoleIO sut = Create(OutputMode.Verbose, output, input);

        sut.WriteVerbose("diagnostic info");

        Assert.Contains("[VERBOSE]", output.ToString());
    }

    // ── Confirm ────────────────────────────────────────────────────────────────

    [Fact]
    public void Confirm_YLowercase_ReturnsTrue()
    {
        using StringWriter output = new();
        using StringReader input = new("y");
        ConsoleIO sut = Create(OutputMode.Normal, output, input);

        bool result = sut.Confirm("Continue?");

        Assert.True(result);
    }

    [Fact]
    public void Confirm_YUppercase_ReturnsTrue()
    {
        using StringWriter output = new();
        using StringReader input = new("Y");
        ConsoleIO sut = Create(OutputMode.Normal, output, input);

        bool result = sut.Confirm("Continue?");

        Assert.True(result);
    }

    [Fact]
    public void Confirm_N_ReturnsFalse()
    {
        using StringWriter output = new();
        using StringReader input = new("n");
        ConsoleIO sut = Create(OutputMode.Normal, output, input);

        bool result = sut.Confirm("Continue?");

        Assert.False(result);
    }

    [Fact]
    public void Confirm_Empty_ReturnsFalse()
    {
        using StringWriter output = new();
        using StringReader input = new(string.Empty);
        ConsoleIO sut = Create(OutputMode.Normal, output, input);

        bool result = sut.Confirm("Continue?");

        Assert.False(result);
    }

    [Fact]
    public void Confirm_WritesQuestionWithPrompt()
    {
        using StringWriter output = new();
        using StringReader input = new("n");
        ConsoleIO sut = Create(OutputMode.Normal, output, input);

        sut.Confirm("Do the thing?");

        Assert.Contains("Do the thing?", output.ToString());
    }

    // ── PromptChoice ───────────────────────────────────────────────────────────

    [Fact]
    public void PromptChoice_ValidFirstOption_ReturnsFirstValue()
    {
        using StringWriter output = new();
        using StringReader input = new("1");
        ConsoleIO sut = Create(OutputMode.Normal, output, input);

        IReadOnlyList<(string Label, int Value)> options =
        [
            ("Alpha", 10),
            ("Beta", 20),
        ];

        int result = sut.PromptChoice("Pick one:", options);

        Assert.Equal(10, result);
    }

    [Fact]
    public void PromptChoice_ValidSecondOption_ReturnsSecondValue()
    {
        using StringWriter output = new();
        using StringReader input = new("2");
        ConsoleIO sut = Create(OutputMode.Normal, output, input);

        IReadOnlyList<(string Label, string Value)> options =
        [
            ("First", "one"),
            ("Second", "two"),
        ];

        string result = sut.PromptChoice("Pick:", options);

        Assert.Equal("two", result);
    }

    [Fact]
    public void PromptChoice_InvalidThenValid_ReturnsValueAfterRetry()
    {
        using StringWriter output = new();
        // First input is invalid, second is valid
        using StringReader input = new("invalid\n2");
        ConsoleIO sut = Create(OutputMode.Normal, output, input);

        IReadOnlyList<(string Label, int Value)> options =
        [
            ("Alpha", 100),
            ("Beta", 200),
        ];

        int result = sut.PromptChoice("Pick one:", options);

        Assert.Equal(200, result);
    }

    [Fact]
    public void PromptChoice_WritesNumberedOptions()
    {
        using StringWriter output = new();
        using StringReader input = new("1");
        ConsoleIO sut = Create(OutputMode.Normal, output, input);

        IReadOnlyList<(string Label, int Value)> options =
        [
            ("Option A", 1),
            ("Option B", 2),
        ];

        sut.PromptChoice("Choose:", options);

        string written = output.ToString();
        Assert.Contains("1. Option A", written);
        Assert.Contains("2. Option B", written);
    }
}
