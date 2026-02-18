namespace AiDevLoop.Core.Domain;

/// <summary>
/// A named milestone grouping a set of related tasks in the implementation plan.
/// </summary>
/// <param name="Number">The sequential number of the milestone.</param>
/// <param name="Name">The human-readable name of the milestone.</param>
/// <param name="Tasks">The tasks that belong to this milestone.</param>
public record Milestone(
    int Number,
    string Name,
    IReadOnlyList<TaskDefinition> Tasks);
