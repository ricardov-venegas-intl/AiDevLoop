namespace AiDevLoop.Core.Domain;

/// <summary>
/// Represents the relative complexity of a task.
/// </summary>
public enum Complexity
{
    /// <summary>A very simple task requiring minimal effort.</summary>
    Trivial,

    /// <summary>A straightforward task with well-understood scope.</summary>
    Simple,

    /// <summary>A moderately complex task requiring some design decisions.</summary>
    Medium,

    /// <summary>A highly complex task with significant unknowns or large scope.</summary>
    Complex,
}
