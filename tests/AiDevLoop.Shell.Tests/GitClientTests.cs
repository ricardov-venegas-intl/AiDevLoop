namespace AiDevLoop.Shell.Tests;

using System;
using System.Threading;
using System.Threading.Tasks;

using AiDevLoop.Core.Domain;
using AiDevLoop.Shell.Adapters;

using Xunit;

/// <summary>
/// Unit tests for <see cref="GitClient"/>.
/// </summary>
public sealed class GitClientTests
{
    private readonly FakeProcessRunner _fakeRunner = new();

    /// <summary>
    /// StageAllAsync runs <c>git add .</c>.
    /// </summary>
    [Fact]
    public async Task StageAllAsync_ExecutesGitAddDot()
    {
        _fakeRunner.ResultToReturn = new CommandResult("git", 0, "", "");
        GitClient client = new(_fakeRunner);

        await client.StageAllAsync(CancellationToken.None);

        Assert.Equal("git", _fakeRunner.CapturedCommand);
        Assert.Equal("add .", _fakeRunner.CapturedArguments);
    }

    /// <summary>
    /// StageAllAsync throws <see cref="InvalidOperationException"/> on a non-zero exit code.
    /// </summary>
    [Fact]
    public async Task StageAllAsync_NonZeroExitCode_Throws()
    {
        _fakeRunner.ResultToReturn = new CommandResult("git", 1, "", "fatal: not a git repository");
        GitClient client = new(_fakeRunner);

        await Assert.ThrowsAsync<InvalidOperationException>(
            () => client.StageAllAsync(CancellationToken.None));
    }

    /// <summary>
    /// CommitAsync runs <c>git commit -m "…"</c> with the supplied message.
    /// </summary>
    [Fact]
    public async Task CommitAsync_ExecutesGitCommitWithMessage()
    {
        _fakeRunner.ResultToReturn = new CommandResult("git", 0, "[main abc1234] my message", "");
        GitClient client = new(_fakeRunner);

        await client.CommitAsync("my message", CancellationToken.None);

        Assert.Equal("git", _fakeRunner.CapturedCommand);
        Assert.NotNull(_fakeRunner.CapturedArguments);
        Assert.Contains("commit -m", _fakeRunner.CapturedArguments);
        Assert.Contains("my message", _fakeRunner.CapturedArguments);
    }

    /// <summary>
    /// CommitAsync throws <see cref="InvalidOperationException"/> on a non-zero exit code.
    /// </summary>
    [Fact]
    public async Task CommitAsync_NonZeroExitCode_Throws()
    {
        _fakeRunner.ResultToReturn = new CommandResult("git", 1, "", "nothing to commit");
        GitClient client = new(_fakeRunner);

        await Assert.ThrowsAsync<InvalidOperationException>(
            () => client.CommitAsync("oops", CancellationToken.None));
    }

    /// <summary>
    /// GetStatusAsync returns an empty status for a clean repo (empty stdout).
    /// </summary>
    [Fact]
    public async Task GetStatusAsync_CleanRepo_ReturnsEmptyStatus()
    {
        _fakeRunner.ResultToReturn = new CommandResult("git", 0, "", "");
        GitClient client = new(_fakeRunner);

        GitStatus status = await client.GetStatusAsync(CancellationToken.None);

        Assert.False(status.HasStagedChanges);
        Assert.False(status.HasUnstagedChanges);
        Assert.Empty(status.ModifiedFiles);
    }

    /// <summary>
    /// GetStatusAsync sets <see cref="GitStatus.HasStagedChanges"/> when the index column is non-blank.
    /// </summary>
    [Fact]
    public async Task GetStatusAsync_StagedChanges_SetsHasStagedChanges()
    {
        _fakeRunner.ResultToReturn = new CommandResult("git", 0, "M  src/Foo.cs\n", "");
        GitClient client = new(_fakeRunner);

        GitStatus status = await client.GetStatusAsync(CancellationToken.None);

        Assert.True(status.HasStagedChanges);
        Assert.False(status.HasUnstagedChanges);
    }

    /// <summary>
    /// GetStatusAsync sets <see cref="GitStatus.HasUnstagedChanges"/> when the worktree column is non-blank.
    /// </summary>
    [Fact]
    public async Task GetStatusAsync_UnstagedChanges_SetsHasUnstagedChanges()
    {
        _fakeRunner.ResultToReturn = new CommandResult("git", 0, " M src/Bar.cs\n", "");
        GitClient client = new(_fakeRunner);

        GitStatus status = await client.GetStatusAsync(CancellationToken.None);

        Assert.True(status.HasUnstagedChanges);
        Assert.False(status.HasStagedChanges);
    }

    /// <summary>
    /// GetStatusAsync sets both flags when both columns are non-blank.
    /// </summary>
    [Fact]
    public async Task GetStatusAsync_MixedStatus_ParsesBoth()
    {
        _fakeRunner.ResultToReturn = new CommandResult("git", 0, "MM src/Baz.cs\n", "");
        GitClient client = new(_fakeRunner);

        GitStatus status = await client.GetStatusAsync(CancellationToken.None);

        Assert.True(status.HasStagedChanges);
        Assert.True(status.HasUnstagedChanges);
    }

    /// <summary>
    /// GetStatusAsync populates <see cref="GitStatus.ModifiedFiles"/> with the file path from each line.
    /// </summary>
    [Fact]
    public async Task GetStatusAsync_PopulatesModifiedFiles()
    {
        _fakeRunner.ResultToReturn = new CommandResult("git", 0, "M  src/Foo.cs\n", "");
        GitClient client = new(_fakeRunner);

        GitStatus status = await client.GetStatusAsync(CancellationToken.None);

        Assert.Contains("src/Foo.cs", status.ModifiedFiles);
    }

    /// <summary>
    /// GetStatusAsync throws <see cref="InvalidOperationException"/> on a non-zero exit code.
    /// </summary>
    [Fact]
    public async Task GetStatusAsync_NonZeroExitCode_Throws()
    {
        _fakeRunner.ResultToReturn = new CommandResult("git", 128, "", "fatal: not a git repository");
        GitClient client = new(_fakeRunner);

        await Assert.ThrowsAsync<InvalidOperationException>(
            () => client.GetStatusAsync(CancellationToken.None));
    }

    private sealed class FakeProcessRunner : IProcessRunner
    {
        public string? CapturedCommand { get; private set; }
        public string? CapturedArguments { get; private set; }
        public CommandResult ResultToReturn { get; set; } = new("git", 0, "", "");

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
