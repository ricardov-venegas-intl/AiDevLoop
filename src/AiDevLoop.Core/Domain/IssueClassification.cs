namespace AiDevLoop.Core.Domain;

/// <summary>
/// Classifies a code review issue by its severity and blocking impact.
/// </summary>
public enum IssueClassification
{
    /// <summary>The issue must be resolved before the task can be considered complete.</summary>
    Blocking,

    /// <summary>The issue should be addressed but does not block completion.</summary>
    NonBlocking,

    /// <summary>A minor stylistic or preference note with no functional impact.</summary>
    Nitpick,
}
