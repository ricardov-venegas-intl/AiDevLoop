using AiDevLoop.Core;
using AiDevLoop.Core.Domain;
using Xunit;

namespace AiDevLoop.Core.Tests;

public sealed class ReviewAnalyzerTests
{
    // ── Null / empty / whitespace ────────────────────────────────────────────

    [Fact]
    public void AnalyzeReview_NullDocument_ReturnsEmptyResult()
    {
        var result = ReviewAnalyzer.AnalyzeReview(null, iterationNumber: 1);

        Assert.Empty(result.Issues);
        Assert.False(result.HasBlockingIssues);
    }

    [Fact]
    public void AnalyzeReview_EmptyDocument_ReturnsEmptyResult()
    {
        var result = ReviewAnalyzer.AnalyzeReview(string.Empty, iterationNumber: 1);

        Assert.Empty(result.Issues);
        Assert.False(result.HasBlockingIssues);
    }

    [Fact]
    public void AnalyzeReview_WhitespaceDocument_ReturnsEmptyResult()
    {
        var result = ReviewAnalyzer.AnalyzeReview("   \n\t\n  ", iterationNumber: 1);

        Assert.Empty(result.Issues);
        Assert.False(result.HasBlockingIssues);
    }

    // ── APPROVED marker ──────────────────────────────────────────────────────

    [Fact]
    public void AnalyzeReview_ApprovedFirstLine_ReturnsEmptyResult()
    {
        const string doc = "APPROVED\nSome other content with BLOCKING issues";

        var result = ReviewAnalyzer.AnalyzeReview(doc, iterationNumber: 2);

        Assert.Empty(result.Issues);
        Assert.False(result.HasBlockingIssues);
    }

    [Fact]
    public void AnalyzeReview_ApprovedCaseInsensitive_ReturnsEmptyResult()
    {
        const string doc = "approved — no issues found.";

        var result = ReviewAnalyzer.AnalyzeReview(doc, iterationNumber: 1);

        Assert.Empty(result.Issues);
        Assert.False(result.HasBlockingIssues);
    }

    [Theory]
    [InlineData("APPROVED")]
    [InlineData("approved")]
    [InlineData("Approved")]
    [InlineData("## APPROVED")]
    public void AnalyzeReview_ApprovedVariants_AllReturnEmptyResult(string firstLine)
    {
        var result = ReviewAnalyzer.AnalyzeReview(firstLine, iterationNumber: 1);

        Assert.Empty(result.Issues);
    }

    // ── APPROVED only applies to the first non-empty line ───────────────────

    [Fact]
    public void AnalyzeReview_ApprovedOnSecondLine_DoesNotTriggerApprovedPath()
    {
        const string doc = "## Review\nAPPROVED\n- **BLOCKING**: Missing tests";

        var result = ReviewAnalyzer.AnalyzeReview(doc, iterationNumber: 1);

        // First non-empty line is "## Review" — approved on line 2 must be ignored.
        Assert.NotEmpty(result.Issues);
        Assert.True(result.HasBlockingIssues);
    }

    // ── Issue classification ─────────────────────────────────────────────────

    [Fact]
    public void AnalyzeReview_BlockingLine_ReturnsBlockingIssue()
    {
        const string doc = "## Review\n- **BLOCKING**: Missing XML documentation";

        var result = ReviewAnalyzer.AnalyzeReview(doc, iterationNumber: 1);

        var issue = Assert.Single(result.Issues);
        Assert.Equal(IssueClassification.Blocking, issue.Classification);
        Assert.Equal("Missing XML documentation", issue.Description);
    }

    [Fact]
    public void AnalyzeReview_NonBlockingLine_ReturnsNonBlockingIssue()
    {
        const string doc = "## Review\n- **NON-BLOCKING**: Consider using ConfigureAwait(false)";

        var result = ReviewAnalyzer.AnalyzeReview(doc, iterationNumber: 1);

        var issue = Assert.Single(result.Issues);
        Assert.Equal(IssueClassification.NonBlocking, issue.Classification);
        Assert.Equal("Consider using ConfigureAwait(false)", issue.Description);
    }

    [Fact]
    public void AnalyzeReview_NitpickLine_ReturnsNitpickIssue()
    {
        const string doc = "## Review\n* NITPICK: Rename `x` to `index`";

        var result = ReviewAnalyzer.AnalyzeReview(doc, iterationNumber: 1);

        var issue = Assert.Single(result.Issues);
        Assert.Equal(IssueClassification.Nitpick, issue.Classification);
    }

    [Fact]
    public void AnalyzeReview_NonBlockingNotMisclassifiedAsBlocking()
    {
        const string doc = "- **NON-BLOCKING**: Prefer expression-bodied members";

        var result = ReviewAnalyzer.AnalyzeReview(doc, iterationNumber: 1);

        var issue = Assert.Single(result.Issues);
        Assert.Equal(IssueClassification.NonBlocking, issue.Classification);
        Assert.False(result.HasBlockingIssues);
    }

    // ── HasBlockingIssues derived from Issues list ───────────────────────────

    [Fact]
    public void AnalyzeReview_HasBlockingIssues_TrueWhenBlockingPresent()
    {
        const string doc = "- BLOCKING: Unit tests missing\n- NON-BLOCKING: Improve naming";

        var result = ReviewAnalyzer.AnalyzeReview(doc, iterationNumber: 1);

        Assert.True(result.HasBlockingIssues);
    }

    [Fact]
    public void AnalyzeReview_HasBlockingIssues_FalseWhenOnlyNonBlockingAndNitpick()
    {
        const string doc = "- NON-BLOCKING: Use var\n- NITPICK: Add blank line";

        var result = ReviewAnalyzer.AnalyzeReview(doc, iterationNumber: 1);

        Assert.False(result.HasBlockingIssues);
        Assert.Equal(2, result.Issues.Count);
    }

    // ── IterationNumber ──────────────────────────────────────────────────────

    [Fact]
    public void AnalyzeReview_IterationNumber_StoredInResult()
    {
        var result = ReviewAnalyzer.AnalyzeReview("- BLOCKING: Something", iterationNumber: 3);

        Assert.Equal(3, result.IterationNumber);
    }

    [Fact]
    public void AnalyzeReview_IterationNumber_StoredWhenApproved()
    {
        var result = ReviewAnalyzer.AnalyzeReview("APPROVED", iterationNumber: 2);

        Assert.Equal(2, result.IterationNumber);
    }

    // ── Case-insensitive classification markers ──────────────────────────────

    [Theory]
    [InlineData("blocking: Missing tests")]
    [InlineData("BLOCKING: Missing tests")]
    [InlineData("Blocking: Missing tests")]
    public void AnalyzeReview_BlockingCaseInsensitive_AlwaysClassifiedBlocking(string line)
    {
        var result = ReviewAnalyzer.AnalyzeReview(line, iterationNumber: 1);

        var issue = Assert.Single(result.Issues);
        Assert.Equal(IssueClassification.Blocking, issue.Classification);
    }

    // ── Mixed markers in one document ────────────────────────────────────────

    [Fact]
    public void AnalyzeReview_MixedMarkers_ExtractsAllIssues()
    {
        const string doc = """
            ## Code Review

            ### **BLOCKING**: No unit tests
            ### **NON-BLOCKING**: Use expression-bodied members where readable
            ### **NITPICK**: Extra blank line at end of file
            """;

        var result = ReviewAnalyzer.AnalyzeReview(doc, iterationNumber: 1);

        Assert.Equal(3, result.Issues.Count);
        Assert.True(result.HasBlockingIssues);
        Assert.Contains(result.Issues, i => i.Classification == IssueClassification.Blocking);
        Assert.Contains(result.Issues, i => i.Classification == IssueClassification.NonBlocking);
        Assert.Contains(result.Issues, i => i.Classification == IssueClassification.Nitpick);
    }

    // ── Description stripping ────────────────────────────────────────────────

    [Fact]
    public void AnalyzeReview_MarkdownBold_StrippedFromDescription()
    {
        const string doc = "- **BLOCKING**: Missing cancellation token support";

        var result = ReviewAnalyzer.AnalyzeReview(doc, iterationNumber: 1);

        var issue = Assert.Single(result.Issues);
        Assert.Equal("Missing cancellation token support", issue.Description);
    }

    [Fact]
    public void AnalyzeReview_LeadingHashesAndDash_StrippedFromDescription()
    {
        const string doc = "### NITPICK - Rename variable for clarity";

        var result = ReviewAnalyzer.AnalyzeReview(doc, iterationNumber: 1);

        var issue = Assert.Single(result.Issues);
        Assert.Equal(IssueClassification.Nitpick, issue.Classification);
        Assert.Equal("Rename variable for clarity", issue.Description);
    }
}
