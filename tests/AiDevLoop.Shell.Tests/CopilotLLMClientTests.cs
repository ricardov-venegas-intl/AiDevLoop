namespace AiDevLoop.Shell.Tests;

using System;
using System.Threading;
using System.Threading.Tasks;

using AiDevLoop.Core.Domain;
using AiDevLoop.Shell.Adapters;

using Xunit;

/// <summary>
/// Unit tests for <see cref="CopilotLLMClient"/>.
/// </summary>
public sealed class CopilotLLMClientTests
{
    private readonly FakeProcessRunner _fakeRunner = new();

    /// <summary>
    /// A successful response returns trimmed stdout.
    /// </summary>
    [Fact]
    public async Task InvokeAsync_SuccessfulResponse_ReturnsTrimmedStdout()
    {
        _fakeRunner.ResultToReturn = new CommandResult("copilot", 0, "  hello world  ", "");
        CopilotLLMClient client = new(_fakeRunner);

        string result = await client.InvokeAsync("say hi", CancellationToken.None);

        Assert.Equal("hello world", result);
    }

    /// <summary>
    /// A non-zero exit code raises <see cref="InvalidOperationException"/>.
    /// </summary>
    [Fact]
    public async Task InvokeAsync_NonZeroExitCode_ThrowsInvalidOperationException()
    {
        _fakeRunner.ResultToReturn = new CommandResult("copilot", 1, "", "error output");
        CopilotLLMClient client = new(_fakeRunner);

        InvalidOperationException ex = await Assert.ThrowsAsync<InvalidOperationException>(
            () => client.InvokeAsync("hello", CancellationToken.None));

        Assert.Contains("1", ex.Message);
        Assert.Contains("error output", ex.Message);
    }

    /// <summary>
    /// An empty stdout raises <see cref="InvalidOperationException"/>.
    /// </summary>
    [Fact]
    public async Task InvokeAsync_EmptyStdout_ThrowsInvalidOperationException()
    {
        _fakeRunner.ResultToReturn = new CommandResult("copilot", 0, "", "");
        CopilotLLMClient client = new(_fakeRunner);

        await Assert.ThrowsAsync<InvalidOperationException>(
            () => client.InvokeAsync("hello", CancellationToken.None));
    }

    /// <summary>
    /// Whitespace-only stdout raises <see cref="InvalidOperationException"/>.
    /// </summary>
    [Fact]
    public async Task InvokeAsync_WhitespaceStdout_ThrowsInvalidOperationException()
    {
        _fakeRunner.ResultToReturn = new CommandResult("copilot", 0, "   \n\t  ", "");
        CopilotLLMClient client = new(_fakeRunner);

        await Assert.ThrowsAsync<InvalidOperationException>(
            () => client.InvokeAsync("hello", CancellationToken.None));
    }

    /// <summary>
    /// The invoked command is always <c>copilot</c>.
    /// </summary>
    [Fact]
    public async Task InvokeAsync_InvokesCopilotCommand()
    {
        _fakeRunner.ResultToReturn = new CommandResult("copilot", 0, "response", "");
        CopilotLLMClient client = new(_fakeRunner);

        await client.InvokeAsync("test prompt", CancellationToken.None);

        Assert.Equal("copilot", _fakeRunner.CapturedCommand);
    }

    /// <summary>
    /// Arguments include the <c>-p</c> flag with the prompt text.
    /// </summary>
    [Fact]
    public async Task InvokeAsync_ArgumentsContainPrompt()
    {
        _fakeRunner.ResultToReturn = new CommandResult("copilot", 0, "ok", "");
        CopilotLLMClient client = new(_fakeRunner);

        await client.InvokeAsync("my prompt", CancellationToken.None);

        Assert.NotNull(_fakeRunner.CapturedArguments);
        Assert.Contains("-p", _fakeRunner.CapturedArguments);
        Assert.Contains("my prompt", _fakeRunner.CapturedArguments);
    }

    /// <summary>
    /// Double-quotes in the prompt are escaped so they don't break the argument string.
    /// </summary>
    [Fact]
    public async Task InvokeAsync_PromptWithDoubleQuotes_EscapesQuotes()
    {
        _fakeRunner.ResultToReturn = new CommandResult("copilot", 0, "ok", "");
        CopilotLLMClient client = new(_fakeRunner);

        await client.InvokeAsync("say \"hello\"", CancellationToken.None);

        Assert.NotNull(_fakeRunner.CapturedArguments);
        Assert.Contains("\\\"", _fakeRunner.CapturedArguments);
    }

    private sealed class FakeProcessRunner : IProcessRunner
    {
        public string? CapturedCommand { get; private set; }
        public string? CapturedArguments { get; private set; }
        public CommandResult ResultToReturn { get; set; } = new("copilot", 0, "response text", "");

        public Task<CommandResult> RunAsync(string command, string arguments, CancellationToken cancellationToken)
        {
            CapturedCommand = command;
            CapturedArguments = arguments;
            return Task.FromResult(ResultToReturn);
        }

        public Task<CommandResult> RunAsync(
            string command,
            string arguments,
            string workingDirectory,
            bool verbose,
            CancellationToken cancellationToken)
            => RunAsync(command, arguments, cancellationToken);
    }
}
