using AiDevLoop.Core;
using AiDevLoop.Core.Domain;
using Xunit;
using TaskStatus = AiDevLoop.Core.Domain.TaskStatus;

namespace AiDevLoop.Core.Tests;

/// <summary>
/// Unit tests for <see cref="CommitMessageBuilder.GenerateCommitMessage"/>.
/// </summary>
public class CommitMessageBuilderTests
{
    // -----------------------------------------------------------------------
    // Helpers
    // -----------------------------------------------------------------------

    private static TaskDefinition MakeTask(string id, string name) =>
        new(
            Id: new TaskId(id),
            Name: name,
            Status: TaskStatus.Pending,
            Complexity: Complexity.Simple,
            DependsOn: [],
            Description: string.Empty,
            Steps: [],
            AcceptanceCriteria: [],
            FilesInScope: []);

    // -----------------------------------------------------------------------
    // Basic formatting
    // -----------------------------------------------------------------------

    [Fact]
    public void GenerateCommitMessage_BasicTitle_ReturnsConventionalFormat()
    {
        var task = MakeTask("TASK-001", "Create Solution");
        var result = CommitMessageBuilder.GenerateCommitMessage(task);
        Assert.Equal("feat(TASK-001): create solution", result);
    }

    [Fact]
    public void GenerateCommitMessage_UppercaseTitle_IsLowercased()
    {
        var task = MakeTask("TASK-002", "IMPLEMENT PARSER");
        var result = CommitMessageBuilder.GenerateCommitMessage(task);
        Assert.Equal("feat(TASK-002): implement parser", result);
    }

    [Fact]
    public void GenerateCommitMessage_TitleAlreadyLowercase_RemainsUnchanged()
    {
        var task = MakeTask("TASK-003", "add unit tests");
        var result = CommitMessageBuilder.GenerateCommitMessage(task);
        Assert.Equal("feat(TASK-003): add unit tests", result);
    }

    // -----------------------------------------------------------------------
    // Trailing period stripping
    // -----------------------------------------------------------------------

    [Fact]
    public void GenerateCommitMessage_TitleWithTrailingPeriod_PeriodIsStripped()
    {
        var task = MakeTask("TASK-004", "Create Solution.");
        var result = CommitMessageBuilder.GenerateCommitMessage(task);
        Assert.Equal("feat(TASK-004): create solution", result);
    }

    [Fact]
    public void GenerateCommitMessage_TitleWithInternalPeriod_InternalPeriodKept()
    {
        var task = MakeTask("TASK-005", "v1.0 release");
        var result = CommitMessageBuilder.GenerateCommitMessage(task);
        Assert.Equal("feat(TASK-005): v1.0 release", result);
    }

    // -----------------------------------------------------------------------
    // Length truncation
    // -----------------------------------------------------------------------

    [Fact]
    public void GenerateCommitMessage_ResultExactly72Chars_NoTruncation()
    {
        // prefix = "feat(TASK-006): " = 16 chars → title must be 56 chars for total = 72
        var title = new string('a', 56); // exactly 56 chars
        var task = MakeTask("TASK-006", title);
        var result = CommitMessageBuilder.GenerateCommitMessage(task);
        Assert.Equal(72, result.Length);
        Assert.False(result.EndsWith("..."));
    }

    [Fact]
    public void GenerateCommitMessage_ResultExceeds72Chars_TruncatedToExactly72()
    {
        // prefix = "feat(TASK-007): " = 16 chars → title of 57 chars → total = 73 → must truncate
        var title = new string('b', 57);
        var task = MakeTask("TASK-007", title);
        var result = CommitMessageBuilder.GenerateCommitMessage(task);
        Assert.Equal(72, result.Length);
        Assert.EndsWith("...", result);
    }

    [Fact]
    public void GenerateCommitMessage_LongTitleEllipsisReplacesLastThreeChars()
    {
        // prefix = "feat(TASK-008): " = 16 chars; max title = 56; allowed body = 53 chars + "..."
        var title = new string('c', 80); // way over
        var task = MakeTask("TASK-008", title);
        var result = CommitMessageBuilder.GenerateCommitMessage(task);
        Assert.Equal(72, result.Length);
        Assert.Equal("feat(TASK-008): " + new string('c', 53) + "...", result);
    }

    [Fact]
    public void GenerateCommitMessage_TitleWithTrailingPeriodThenLong_PeriodStrippedBeforeTruncation()
    {
        // Strip period first, then check length.
        // prefix = "feat(TASK-009): " = 16 chars; title (after strip) = 80 chars → truncate to 56 → 53 + "..."
        var title = new string('d', 80) + ".";
        var task = MakeTask("TASK-009", title);
        var result = CommitMessageBuilder.GenerateCommitMessage(task);
        Assert.Equal(72, result.Length);
        Assert.Equal("feat(TASK-009): " + new string('d', 53) + "...", result);
    }

    [Fact]
    public void GenerateCommitMessage_TitleAllPeriods_ReturnsJustPrefix()
    {
        // "..." → TrimEnd('.') → "" → empty title should not produce a trailing space
        var task = MakeTask("TASK-010", "...");
        var result = CommitMessageBuilder.GenerateCommitMessage(task);
        Assert.Equal("feat(TASK-010):", result);
    }

    // -----------------------------------------------------------------------
    // Null guard
    // -----------------------------------------------------------------------

    [Fact]
    public void GenerateCommitMessage_NullTask_ThrowsArgumentNullException()
    {
        Assert.Throws<ArgumentNullException>(() =>
            CommitMessageBuilder.GenerateCommitMessage(null!));
    }
}
