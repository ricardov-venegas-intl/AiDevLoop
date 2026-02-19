using AiDevLoop.Core.Domain;

namespace AiDevLoop.Core;

/// <summary>
/// Analyzes a review document produced by the review agent and extracts classified issues.
/// </summary>
public static class ReviewAnalyzer
{
    private const string ApprovedMarker = "approved";
    private const string NonBlockingMarker = "non-blocking";
    private const string BlockingMarker = "blocking";
    private const string NitpickMarker = "nitpick";

    /// <summary>
    /// Parses <paramref name="reviewDocument"/> and returns a <see cref="ReviewResult"/>.
    /// </summary>
    /// <remarks>
    /// <para>
    /// If the first non-empty line contains "approved" (case-insensitive) the method returns
    /// immediately with an empty issue list and <see cref="ReviewResult.HasBlockingIssues"/>
    /// set to <see langword="false"/>.
    /// </para>
    /// <para>
    /// Otherwise every line is scanned for the markers <c>NON-BLOCKING</c>, <c>BLOCKING</c>,
    /// and <c>NITPICK</c> (all case-insensitive). Because <c>NON-BLOCKING</c> contains the
    /// substring <c>BLOCKING</c>, the longer marker is tested first.
    /// </para>
    /// <para>
    /// Leading Markdown formatting characters (<c>#</c>, <c>-</c>, <c>*</c>, <c>&gt;</c>) and
    /// bold markers (<c>**</c>) are stripped before the description is extracted.
    /// </para>
    /// </remarks>
    /// <param name="reviewDocument">
    /// The raw Markdown text of the review. May be <see langword="null"/>, empty, or
    /// whitespace — each of those inputs produces an empty <see cref="ReviewResult"/>.
    /// </param>
    /// <param name="iterationNumber">
    /// The one-based review-cycle index stored in the returned <see cref="ReviewResult"/>.
    /// </param>
    /// <returns>
    /// A <see cref="ReviewResult"/> whose <see cref="ReviewResult.HasBlockingIssues"/> is
    /// derived from the extracted <see cref="ReviewResult.Issues"/> list.
    /// </returns>
    public static ReviewResult AnalyzeReview(string? reviewDocument, int iterationNumber)
    {
        if (string.IsNullOrWhiteSpace(reviewDocument))
        {
            return new ReviewResult([], HasBlockingIssues: false, iterationNumber);
        }

        var lines = reviewDocument.Split('\n');

        // APPROVED check — inspect only the first non-empty line.
        foreach (var rawLine in lines)
        {
            var firstLine = rawLine.Trim();
            if (firstLine.Length == 0)
            {
                continue;
            }

            // Strip markdown structural characters and bold markers, then check if the
            // normalized line equals or starts with "approved". This avoids false positives
            // such as "Not APPROVED" while still matching "APPROVED", "## APPROVED", etc.
            var normalized = RemoveBoldMarkers(firstLine.TrimStart('#', '-', '*', '>').Trim());
            if (normalized.StartsWith(ApprovedMarker, StringComparison.OrdinalIgnoreCase))
            {
                return new ReviewResult([], HasBlockingIssues: false, iterationNumber);
            }

            break;
        }

        var issues = new List<ReviewIssue>();

        foreach (var rawLine in lines)
        {
            var trimmed = rawLine.Trim();
            if (trimmed.Length == 0)
            {
                continue;
            }

            // NON-BLOCKING must be checked before BLOCKING because it contains the substring.
            if (trimmed.IndexOf(NonBlockingMarker, StringComparison.OrdinalIgnoreCase) >= 0)
            {
                issues.Add(new ReviewIssue(
                    ExtractDescription(trimmed, NonBlockingMarker),
                    IssueClassification.NonBlocking));
            }
            else if (trimmed.IndexOf(BlockingMarker, StringComparison.OrdinalIgnoreCase) >= 0)
            {
                issues.Add(new ReviewIssue(
                    ExtractDescription(trimmed, BlockingMarker),
                    IssueClassification.Blocking));
            }
            else if (trimmed.IndexOf(NitpickMarker, StringComparison.OrdinalIgnoreCase) >= 0)
            {
                issues.Add(new ReviewIssue(
                    ExtractDescription(trimmed, NitpickMarker),
                    IssueClassification.Nitpick));
            }
        }

        var issuesArray = issues.ToArray();

        return new ReviewResult(
            issuesArray,
            HasBlockingIssues: issuesArray.Any(i => i.Classification == IssueClassification.Blocking),
            iterationNumber);
    }

    private static string ExtractDescription(string line, string keyword)
    {
        // Strip leading Markdown structural characters, then strip bold markers.
        var stripped = line.TrimStart('#', '-', '*', '>').Trim();
        stripped = RemoveBoldMarkers(stripped);

        int idx = stripped.IndexOf(keyword, StringComparison.OrdinalIgnoreCase);
        if (idx < 0)
        {
            return stripped.Trim();
        }

        var remainder = stripped[(idx + keyword.Length)..];

        // Strip separator punctuation and whitespace that appears between the keyword and the text.
        remainder = remainder.TrimStart(':', '-', '*', ' ', '\t');

        return remainder.Trim();
    }

    private static string RemoveBoldMarkers(string line)
    {
        var result = line;
        int idx;
        while ((idx = result.IndexOf("**", StringComparison.Ordinal)) >= 0)
        {
            result = result.Remove(idx, 2);
        }

        return result;
    }
}
