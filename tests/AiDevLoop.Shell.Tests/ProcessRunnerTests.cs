namespace AiDevLoop.Shell.Tests;

using System;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;

using AiDevLoop.Core.Domain;
using AiDevLoop.Shell.Adapters;

using Xunit;

/// <summary>
/// Integration tests for <see cref="ProcessRunner"/>.
/// </summary>
[Trait("Category", "Integration")]
public sealed class ProcessRunnerTests
{
    private readonly ProcessRunner _runner = new();

    /// <summary>
    /// Running <c>dotnet --version</c> should succeed and emit a version string.
    /// </summary>
    [Fact]
    public async Task RunAsync_DotnetVersion_ReturnsExitCodeZeroAndVersionInStdout()
    {
        CommandResult result = await _runner.RunAsync("dotnet", "--version", CancellationToken.None);

        Assert.Equal(0, result.ExitCode);
        Assert.True(result.Succeeded);
        Assert.False(string.IsNullOrWhiteSpace(result.Stdout));
        Assert.Equal("dotnet", result.Name);
    }

    /// <summary>
    /// Running <c>dotnet nonexistent-command</c> should return a non-zero exit code.
    /// </summary>
    [Fact]
    public async Task RunAsync_NonZeroExitCode_ReturnedCorrectly()
    {
        CommandResult result = await _runner.RunAsync("dotnet", "nonexistent-command-xyz", CancellationToken.None);

        Assert.NotEqual(0, result.ExitCode);
        Assert.False(result.Succeeded);
    }

    /// <summary>
    /// Running a completely invalid executable returns exit code -1 and a not-found message in stderr.
    /// </summary>
    [Fact]
    public async Task RunAsync_InvalidExecutable_ReturnsNotFound()
    {
        string fakeName = $"fake-exe-{Guid.NewGuid():N}";

        CommandResult result = await _runner.RunAsync(fakeName, "", CancellationToken.None);

        Assert.Equal(-1, result.ExitCode);
        Assert.False(result.Succeeded);
        Assert.Contains("Command not found", result.Stderr);
        Assert.Contains(fakeName, result.Stderr);
    }

    /// <summary>
    /// Verbose mode collects full output without truncation.
    /// </summary>
    [Fact]
    public async Task RunAsync_VerboseMode_CapturesStdout()
    {
        CommandResult result = await _runner.RunAsync(
            "dotnet", "--version",
            workingDirectory: string.Empty,
            verbose: true,
            CancellationToken.None);

        Assert.Equal(0, result.ExitCode);
        Assert.False(string.IsNullOrWhiteSpace(result.Stdout));
    }

    /// <summary>
    /// The simple overload delegates to the full overload and returns valid results.
    /// </summary>
    [Fact]
    public async Task RunAsync_SimpleOverload_DelegatesToFullOverload()
    {
        CommandResult result = await _runner.RunAsync("dotnet", "--version", CancellationToken.None);

        Assert.Equal(0, result.ExitCode);
        Assert.False(string.IsNullOrWhiteSpace(result.Stdout));
    }

    /// <summary>
    /// Cancelling a long-running process causes <see cref="OperationCanceledException"/> to be thrown.
    /// Only runs on Windows.
    /// </summary>
    [Fact]
    public async Task RunAsync_Cancellation_ThrowsOperationCanceledException()
    {
        if (!RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            return; // skip on non-Windows

        using var cts = new CancellationTokenSource(TimeSpan.FromMilliseconds(300));

        await Assert.ThrowsAnyAsync<OperationCanceledException>(
            () => _runner.RunAsync("ping", "-n 30 127.0.0.1", cts.Token));
    }

    /// <summary>
    /// On Windows, <c>cmd /c exit 1</c> returns exit code 1.
    /// </summary>
    [Fact]
    public async Task RunAsync_Windows_ExitCode1()
    {
        if (!RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            return; // skip on non-Windows

        CommandResult result = await _runner.RunAsync("cmd", "/c exit 1", CancellationToken.None);

        Assert.Equal(1, result.ExitCode);
        Assert.False(result.Succeeded);
    }

    /// <summary>
    /// On Windows, stderr can be captured from a cmd echo redirect.
    /// </summary>
    [Fact]
    public async Task RunAsync_Windows_StderrCaptured()
    {
        if (!RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            return; // skip on non-Windows

        CommandResult result = await _runner.RunAsync("cmd", "/c echo error text>&2", CancellationToken.None);

        Assert.Contains("error text", result.Stderr);
    }

    /// <summary>
    /// Non-verbose mode caps stdout at 500 lines; a 600-line command should be truncated.
    /// Only runs on Windows.
    /// </summary>
    [Fact]
    public async Task RunAsync_NonVerbose_TruncatesOutputTo500Lines()
    {
        if (!RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            return; // skip on non-Windows

        // Emit 600 lines via cmd. Non-verbose mode must keep only the last 500.
        CommandResult result = await _runner.RunAsync(
            "cmd", "/c for /l %i in (1,1,600) do echo line %i",
            workingDirectory: string.Empty,
            verbose: false,
            CancellationToken.None);

        int lineCount = result.Stdout.Split(Environment.NewLine, StringSplitOptions.RemoveEmptyEntries).Length;
        Assert.True(lineCount <= 500, $"Expected <= 500 lines but got {lineCount}");
        // The last line should be "line 600", confirming we kept the tail.
        Assert.Contains("line 600", result.Stdout);
    }
}
