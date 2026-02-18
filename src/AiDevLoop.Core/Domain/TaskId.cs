namespace AiDevLoop.Core.Domain;

/// <summary>
/// Uniquely identifies a task by its string identifier (e.g., <c>"TASK-001"</c>).
/// </summary>
/// <param name="Value">The string identifier of the task.</param>
public readonly record struct TaskId(string Value)
{
    /// <inheritdoc/>
    public override string ToString() => Value;
}
