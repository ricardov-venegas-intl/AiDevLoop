namespace AiDevLoop.Core.Domain;

/// <summary>
/// Represents the lifecycle status of a task in the implementation plan.
/// </summary>
public enum TaskStatus
{
    /// <summary>The task has not been started yet.</summary>
    Pending,

    /// <summary>The task is currently being worked on.</summary>
    InProgress,

    /// <summary>The task has been completed successfully.</summary>
    Done,

    /// <summary>The task is blocked by an unresolved dependency or issue.</summary>
    Blocked,
}
