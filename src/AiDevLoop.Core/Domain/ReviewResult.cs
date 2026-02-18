namespace AiDevLoop.Core.Domain;

/// <summary>
/// The aggregated result of a single code review iteration.
/// </summary>
/// <param name="Issues">All issues identified during the review.</param>
/// <param name="HasBlockingIssues">
/// <see langword="true"/> when at least one issue is classified as <see cref="IssueClassification.Blocking"/>.
/// </param>
/// <param name="IterationNumber">
/// The one-based index of this review cycle within the current task's review loop.
/// </param>
public record ReviewResult(
    IReadOnlyList<ReviewIssue> Issues,
    bool HasBlockingIssues,
    int IterationNumber);
