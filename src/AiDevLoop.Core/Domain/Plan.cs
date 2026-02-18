namespace AiDevLoop.Core.Domain;

/// <summary>
/// The complete parsed representation of the implementation plan document.
/// </summary>
/// <param name="Title">The title of the implementation plan.</param>
/// <param name="Milestones">All milestones defined in the plan, in order.</param>
public record Plan(
    string Title,
    IReadOnlyList<Milestone> Milestones);
